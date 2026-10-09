# Reborn: validate separately pinned EP1 1.1 arrays, exact cross-version row identity and rejection boundaries without authoring/game-load claims.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11RuntimeTypeTable.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.IndependentlyLocatedVersionArrays -or -not $first.RuntimeIdentityMatchesPinnedBaseline -or
    $first.RuntimeRowCount-ne 1342 -or $first.IdenticalOrderedBaselineRows-ne 1342 -or $first.MatchedStockRootTypes-ne 254 -or $first.UnobservedRuntimeRows-ne 1088 -or
    $first.FaultCasesExecuted-ne 10 -or $first.ProductionBuildReady -or $first.Ep1ProcessingHashRecovered -or $first.AllTypesHashDerivationRecovered -or
    $first.TableCallersVerified -or $first.CompleteGameTypeUniverseProved -or $first.StreamLoaderCompatibilityProved -or $first.ModPackageLoaded -or $first.TargetExecuted -or
    $first.ImageSha256-cne $second.ImageSha256 -or $first.EvidenceSha256-cne $second.EvidenceSha256 -or
    $first.OrderedNameHashSha256-cne $second.OrderedNameHashSha256 -or $first.RawHashBlockSha256-cne $second.RawHashBlockSha256 -or
    ($first.Rows|ConvertTo-Json -Depth 6 -Compress)-cne ($second.Rows|ConvertTo-Json -Depth 6 -Compress)){throw 'EP1 1.1 runtime evidence/refusal contract differs.'}
# Reborn: public version swapping, relative lookup and arbitrary evidence must fail; neither exact version pin has a bypass switch.
foreach($case in @('swapped','wrong-current','relative','evidence')){
    $rejected=$false
    try{
        switch($case){
            'swapped' {$null=& $audit -ImagePath $BaselineImagePath -BaselineImagePath $ImagePath}
            'wrong-current' {$null=& $audit -ImagePath $BaselineImagePath -BaselineImagePath $BaselineImagePath}
            'relative' {$null=& $audit -ImagePath 'ra3ep1_1.1.game' -BaselineImagePath $BaselineImagePath}
            'evidence' {$null=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -EvidencePath (Join-Path (Split-Path -Parent $PSScriptRoot) 'README.md')}
        }
    }catch{$rejected=$true}
    if(-not $rejected){throw ('EP1 1.1 public boundary admitted: '+$case)}
}
Write-Output 'EP1 1.1 runtime table tests: PASS; independently located 1,342 rows, all 254 stock roots, exact ordered 1.0 identity/hash-block equality, ten memory faults, four public boundary refusals and repeat JSON. No native execution or authoring/loader compatibility claim.'
