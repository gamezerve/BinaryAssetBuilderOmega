# Reborn: validate the pinned real runtime table and public rejection policy without changing or executing stock game files.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath)
$ErrorActionPreference='Stop'
$scriptPath=Join-Path $PSScriptRoot 'Get-Ra3Ep1RuntimeTypeTable.ps1'
$first=& $scriptPath -ImagePath $ImagePath -SelfTest
$second=& $scriptPath -ImagePath $ImagePath -AsJson | ConvertFrom-Json
if ($first.RuntimeRowCount -ne 1342 -or $first.MatchedStockRootTypes -ne 254 -or $first.FaultCasesExecuted -ne 12 -or
    -not $first.FaultTestsPassed -or $first.ImageSha256 -cne $second.ImageSha256 -or $first.EvidenceSha256 -cne $second.EvidenceSha256 -or
    $first.OrderedNameHashSha256 -cne $second.OrderedNameHashSha256 -or $first.RawHashBlockSha256 -cne $second.RawHashBlockSha256 -or
    ($first.Rows | ConvertTo-Json -Depth 6 -Compress) -cne ($second.Rows | ConvertTo-Json -Depth 6 -Compress) -or
    $first.ProductionBuildReady -or $first.Ep1ProcessingHashRecovered -or $first.TableCallersVerified) { throw 'Runtime table repeat/read-only contract differs.' }
# Reborn: raw artifact pins cannot be bypassed by other valid native images, implicit relative lookup or another evidence document.
foreach ($case in @('image','relative','evidence')) {
    $rejected=$false
    try {
        switch ($case) {
            'image' { $null=& $scriptPath -ImagePath (Join-Path (Split-Path -Parent $PSScriptRoot) 'audio.dll') }
            'relative' { $null=& $scriptPath -ImagePath 'ra3ep1_1.0.game' }
            'evidence' { $null=& $scriptPath -ImagePath $ImagePath -EvidencePath (Join-Path (Split-Path -Parent $PSScriptRoot) 'README.md') }
        }
    } catch { $rejected=$true }
    if (-not $rejected) { throw ('Public runtime pin rejection failed: '+$case) }
}
Write-Output 'Runtime type table tests: PASS; 1,342 named hashes, all 254 stock roots, exact repeat/JSON, twelve memory faults and three public input rejections; no target execution or file writes.'
