# Reborn: test static PE inspection on owned copies of the existing native fixture; never execute the fixture or codecs.
[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$audit = Join-Path $PSScriptRoot 'Get-Ra3Ep1PeEvidence.ps1'
$fixture = Join-Path (Split-Path -Parent $PSScriptRoot) 'audio.dll'
$root = Join-Path ([IO.Path]::GetTempPath()) ('Reborn-PeEvidence-'+[Guid]::NewGuid().ToString('N'))
[void][IO.Directory]::CreateDirectory($root)
$original = [IO.File]::ReadAllBytes($fixture)
$baseline = @(& $audit -ImagePaths @($fixture))[0]
if ($baseline.Machine -cne 'I386' -or $baseline.Format -cne 'PE32' -or $baseline.NamedExportCount -ne 23 -or
    $baseline.ImportDlls -cnotcontains 'VCRUNTIME140.dll' -or $baseline.ExportNameSamples.Count -ne 16 -or
    $baseline.TargetExecuted -or -not $baseline.ReadOnly) { throw 'Native fixture baseline differs.' }
# Reborn: append owned non-executable ASCII/UTF16 probe text and one literal word; offsets, not merely matching counts, must be exact.
$ascii = [Text.Encoding]::ASCII.GetBytes("`0Reborn PathMusicProbe ASCII`0`0")
$wide = [Text.Encoding]::Unicode.GetBytes("Reborn PathfinderProbe UTF16`0")
$word = [BitConverter]::GetBytes([Convert]::ToUInt32('599CDAF2',16))
$modified = [byte[]]::new($original.Length+$ascii.Length+$wide.Length+$word.Length)
[Array]::Copy($original,$modified,$original.Length)
[Array]::Copy($ascii,0,$modified,$original.Length,$ascii.Length)
[Array]::Copy($wide,0,$modified,$original.Length+$ascii.Length,$wide.Length)
[Array]::Copy($word,0,$modified,$modified.Length-4,4)
$path = Join-Path $root 'owned.dll'
[IO.File]::WriteAllBytes($path,$modified)
$result = @(& $audit -ImagePaths @($path))[0]
$repeat = @(& $audit -ImagePaths @($path) -AsJson | ConvertFrom-Json)[0]
$asciiMatches = @($result.TargetedStrings | Where-Object { $_.Value -ceq 'Reborn PathMusicProbe ASCII' -and $_.Encoding -ceq 'ASCII' })
$wideMatches = @($result.TargetedStrings | Where-Object { $_.Value -ceq 'Reborn PathfinderProbe UTF16' -and $_.Encoding -ceq 'UTF16LE' })
if ($asciiMatches.Count -ne 1 -or $asciiMatches[0].Offset -cne ('0x{0:X8}' -f ($original.Length+1)) -or
    $wideMatches.Count -ne 1 -or $wideMatches[0].Offset -cne ('0x{0:X8}' -f ($original.Length+$ascii.Length)) -or
    $result.Sha256 -cne $repeat.Sha256 -or $result.Sha256 -ceq $baseline.Sha256 -or $result.NamedExportCount -ne 23 -or
    $result.Ep1ProcessingHashRecovered -or $result.ProductionBuildReady -or $result.AuthenticHeaderRecovered) { throw 'Targeted offset/provenance contract differs.' }
$literal = @($result.StockWordEvidence | Where-Object { $_.Word -ceq '0x599CDAF2' })[0]
$before = @($baseline.StockWordEvidence | Where-Object { $_.Word -ceq '0x599CDAF2' })[0]
if ($literal.Count -ne $before.Count+1 -or $literal.SemanticsVerified -or $literal.Matches[-1].Offset -cne ('0x{0:X8}' -f ($modified.Length-4))) { throw 'Literal evidence was miscounted or interpreted.' }

#-------------------------------------------------------------------------------------------------
<# Reborn: malformed owned image copies must fail before metadata is treated as trustworthy evidence. #>
#-------------------------------------------------------------------------------------------------
function Assert-PeRejected([byte[]] $Bytes,[string] $Name) {
    $invalidPath = Join-Path $root $Name
    [IO.File]::WriteAllBytes($invalidPath,$Bytes)
    $failed=$false
    try { $null = & $audit -ImagePaths @($invalidPath) } catch { $failed=$true }
    if (-not $failed) { throw "Malformed image admitted: $Name" }
}
Assert-PeRejected ([byte[]]::new(8)) 'truncated.dll'
$bad = [byte[]]$original.Clone(); $bad[0]=0
Assert-PeRejected $bad 'not-mz.dll'
$bad = [byte[]]$original.Clone(); $peOffset=[BitConverter]::ToInt32($bad,60); $bad[$peOffset]=0
Assert-PeRejected $bad 'not-pe.dll'
$bad = [byte[]]$original.Clone(); [Array]::Copy([BitConverter]::GetBytes([int]::MaxValue),0,$bad,60,4)
Assert-PeRejected $bad 'offset.dll'
# Reborn: sparse oversized owned input tests the pre-read size guard, not a multi-megabyte binary dump.
$oversized = Join-Path $root 'oversized.dll'
$stream = [IO.File]::Open($oversized,[IO.FileMode]::CreateNew,[IO.FileAccess]::Write)
try { $stream.SetLength(16777217) } finally { $stream.Dispose() }
foreach ($invalidPath in @($oversized,'audio.dll')) {
    $failed=$false
    try { $null = & $audit -ImagePaths @($invalidPath) } catch { $failed=$true }
    if (-not $failed) { throw 'Size/absolute-path guard did not reject input.' }
}
Write-Output "PE evidence tests: PASS; static baseline/import/export, ASCII/UTF16 offsets, repeat/JSON/hash/literal and six rejection cases. Owned fixtures retained: $root"
