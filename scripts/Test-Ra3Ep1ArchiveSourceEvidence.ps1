# Reborn: exercise bounded directory inventory with owned tiny fixtures; no game payloads, registry or native codecs are touched.
[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$fixtureRoot = Join-Path ([IO.Path]::GetTempPath()) ('Reborn-ArchiveEvidence-' + [Guid]::NewGuid().ToString('N'))
[void][IO.Directory]::CreateDirectory($fixtureRoot)
$auditScript = Join-Path $PSScriptRoot 'Get-Ra3Ep1ArchiveSourceEvidence.ps1'

#-------------------------------------------------------------------------------------------------
<# Reborn: create an owned BIG fixture with one or more canonical metadata entries and uninterpreted one-byte payloads. #>
#-------------------------------------------------------------------------------------------------
function New-ArchiveFixture([string[]] $Names, [string] $Name) {
    $headerLength = 16
    foreach ($entryName in $Names) { $headerLength += 9 + $entryName.Length }
    $bytes = [byte[]]::new($headerLength + $Names.Count)
    [Array]::Copy([Text.Encoding]::ASCII.GetBytes('BIG4'), $bytes, 4)
    Set-ArchiveWord $bytes 8 $Names.Count
    Set-ArchiveWord $bytes 12 $headerLength
    $position = 16
    for ($index = 0; $index -lt $Names.Count; $index++) {
        Set-ArchiveWord $bytes $position ($headerLength+$index)
        Set-ArchiveWord $bytes ($position+4) 1
        $position += 8
        $entryBytes = [Text.Encoding]::ASCII.GetBytes($Names[$index])
        [Array]::Copy($entryBytes, 0, $bytes, $position, $entryBytes.Length)
        $position += 1+$entryBytes.Length
    }
    $path = Join-Path $fixtureRoot $Name
    [IO.File]::WriteAllBytes($path, $bytes)
    return $path
}

#-------------------------------------------------------------------------------------------------
<# Reborn: write test-owned big-endian metadata fields, never external archive data. #>
#-------------------------------------------------------------------------------------------------
function Set-ArchiveWord([byte[]] $Bytes, [int] $Offset, [uint32] $Value) {
    for ($index = 0; $index -lt 4; $index++) { $Bytes[$Offset+$index] = [byte](($Value -shr (24-8*$index)) -band 255) }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: malformed metadata must fail rather than silently omit candidates or read payloads. #>
#-------------------------------------------------------------------------------------------------
function Assert-ArchiveRejected([byte[]] $Bytes, [string] $Name) {
    $path = Join-Path $fixtureRoot $Name
    [IO.File]::WriteAllBytes($path, $Bytes)
    $rejected = $false
    try { $null = & $auditScript -ArchivePaths @($path) } catch { $rejected = $true }
    if (-not $rejected) { throw "Invalid fixture admitted: $Name" }
}

# Reborn: exact header candidates, normalized duplicate evidence and repeat identities must coexist without any payload reads.
$valid = New-ArchiveFixture @('Pathfinder\RA3EPMus\PC\RA3EPMus.h','data/audio.bin','pathfinder/ra3epmus/pc/ra3epmus.h') 'valid.big'
$result = @(& $auditScript -ArchivePaths @($valid))[0]
$repeat = @(& $auditScript -ArchivePaths @($valid))[0]
if ($result.EntryCount -ne 3 -or $result.Candidates.Count -ne 2 -or $result.DuplicateNormalizedNames.Count -ne 1 -or
    $result.PayloadBytesRead -ne 0 -or $result.MetadataBytesRead -ne 2*$result.DirectoryBytes -or
    $result.DirectorySha256 -cne $repeat.DirectorySha256 -or $result.ProductionBuildReady -or $result.AuthenticHeaderRecovered) { throw 'Valid archive evidence contract differs.' }
$json = & $auditScript -ArchivePaths @($valid) -AsJson | ConvertFrom-Json
if ($json[0].DirectorySha256 -cne $result.DirectorySha256) { throw 'JSON report differs.' }
$canonical = [IO.File]::ReadAllBytes($valid)
Assert-ArchiveRejected ([byte[]]::new(8)) 'truncated.big'
foreach ($case in @('magic','count','header','smallheader','offset','size','empty','unterminated','nonascii')) {
    # Reborn: each negative case starts from independent fixture bytes, preserving the accepted original.
    $mutated = [byte[]]$canonical.Clone()
    switch ($case) {
        'magic' { $mutated[0] = 0 }
        'count' { Set-ArchiveWord $mutated 8 100001 }
        'header' { Set-ArchiveWord $mutated 12 16777217 }
        'smallheader' { Set-ArchiveWord $mutated 12 15 }
        'offset' { Set-ArchiveWord $mutated 16 15 }
        'size' { Set-ArchiveWord $mutated 20 ([uint32]::MaxValue) }
        'empty' { $mutated[24]=0 }
        'unterminated' { for ($i=24; $i -lt $result.DirectoryBytes; $i++) { $mutated[$i]=65 } }
        'nonascii' { $mutated[24]=255 }
    }
    Assert-ArchiveRejected $mutated "$case.big"
}
# Reborn: the second accepted BIG signature changes directory provenance, not payload admission.
$bigf = [byte[]]$canonical.Clone()
$bigf[3] = [byte][char]'F'
$bigfPath = Join-Path $fixtureRoot 'signature.big'
[IO.File]::WriteAllBytes($bigfPath, $bigf)
$bigfResult = @(& $auditScript -ArchivePaths @($bigfPath))[0]
if ($bigfResult.Magic -cne 'BIGF' -or $bigfResult.EntryCount -ne 3 -or $bigfResult.DirectorySha256 -ceq $result.DirectorySha256) { throw 'BIGF provenance differs.' }
# Reborn: implicit relative lookup must not broaden the caller's selected archive scope.
$relativeRejected = $false
try { $null = & $auditScript -ArchivePaths @('valid.big') } catch { $relativeRejected = $true }
if (-not $relativeRejected) { throw 'Relative archive path admitted.' }
# Reborn: inventory of a runtime music container is not evidence of an authoring header.
$runtime = New-ArchiveFixture @('data/music/compiled.mpf','data/global.bin') 'runtime.big'
$runtimeResult = @(& $auditScript -ArchivePaths @($runtime))[0]
if ($runtimeResult.Candidates.Count -ne 1 -or $runtimeResult.AuthenticHeaderRecovered) { throw 'Runtime container was mistaken for source recovery.' }
Write-Output "Archive source evidence tests: PASS; valid/repeat/JSON/duplicate/runtime/BIGF/relative and ten malformed cases. Owned fixtures retained: $fixtureRoot"
