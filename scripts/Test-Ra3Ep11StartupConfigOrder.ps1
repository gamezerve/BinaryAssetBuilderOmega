# Reborn: verify scoped config ordering and repeat evidence while refusing complete runtime reachability, effective overrides and production readiness claims.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[Parameter(Mandatory=$true)] [string] $SkuDefinitionPath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11StartupConfigOrder.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -AsJson|ConvertFrom-Json
if(-not $first.ReadOnly -or -not $first.FaultTestsPassed -or $first.ReviewedWholeBodyPins-ne 0 -or $first.ReviewedStartupSlicePins-ne 3 -or $first.ReviewedConfigStringPins-ne 5 -or
   $first.AdditionalConfigVectorStride-ne 112 -or $first.BaseConfigName-cne 'filesystem.cfg' -or $first.ConfigReadVa-cne '0x004D86B0' -or $first.ProbeVa-cne '0x004D6F10' -or
   $first.SyntheticDirectivesUseLineDispatcherVa-cne '0x004D9040' -or $first.OrderingFixturesExecuted-ne 6 -or $first.PolicyRejectionsExecuted-ne 3 -or $first.MemoryFaultsExecuted-ne 19 -or $first.ArchivePayloadBytesRead-ne 0){throw 'Startup config evidence contract differs.'}
foreach($name in @('ModConfigReadIsProbeConditional','AdditionalConfigCandidatesTraverseReverse','AdditionalCandidateReadIsProbeConditional','BaseConfigReadFollowsAdditionalCandidates','BaseReadFailureSynthesizesSetSearchPath','BaseReadSuccessCanAppendLanguagePath','ScopedStartupConfigOrderRecovered')){if(-not $first.$name){throw "Startup recovered field differs: $name"}}
foreach($name in @('FullStartupReachabilityRecovered','ActualAdditionalCandidatePathsRecovered','LanguageFlagMeaningFullyRecovered','ConfigOverridesRemainEffectiveAfterLaterReadsProven','ActualStartupMountOrderRecovered','TargetExecuted','ModPackageLoaded','AuthenticEp1ProcessingHashRecovered','ProductionBuildReady')){if($first.$name){throw "Startup unresolved gate unexpectedly claimed: $name"}}
# Reborn: compare all nested stable fields except deliberate detached-test counters.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','OrderingFixturesExecuted','PolicyRejectionsExecuted','MemoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 8 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 8 -Compress)){throw "Repeat startup field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 startup config order: PASS; three scoped slices/five config strings, six ordering fixtures, three policy rejections, nineteen private faults and repeat JSON. Mod config precedes reverse additional candidates and filesystem.cfg; complete reachability, effective overrides and game loading remain open.'
