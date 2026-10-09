# Reborn: verify scoped wrapper-open forwarding, default search plans and prefix composition without claiming game execution or global startup priority.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[Parameter(Mandatory=$true)] [string] $SkuDefinitionPath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11ProviderOpenForwarding.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.ReadOnly -or $first.ReviewedWholeBodyPins-ne 2 -or $first.ReviewedLiteralPins-ne 1 -or
   $first.WrapperOpenVa-cne '0x0096BA00' -or $first.ExistsProbeVa-cne '0x0096ABC0' -or $first.WrapperHandleOffset-ne 8 -or $first.WrapperRegistrationOffset-ne 12 -or $first.WrapperInterfaceOffset-ne 16 -or
   $first.ProviderOpenVirtualSlotOffset-ne 12 -or $first.ProviderSizeVirtualSlotOffset-ne 32 -or $first.DefaultSearchCountContextOffset-ne 56 -or $first.DefaultSearchPairsContextOffset-ne 60 -or $first.DefaultSearchPairStride-ne 8 -or
   $first.DefaultRelativeJoinFormat-cne '%s/%s' -or $first.ExistsProbeOpenOption-ne 0 -or $first.ArchivePayloadBytesRead-ne 0 -or
   $first.ForwardPathFixturesExecuted-ne 6 -or $first.OpenPlanFixturesExecuted-ne 8 -or $first.ComposedPrefixFixturesExecuted-ne 4 -or $first.PolicyRejectionsExecuted-ne 3 -or $first.MemoryFaultsExecuted-ne 18){throw 'Provider-open forwarding contract differs.'}
# Reborn: require reviewed scoped behavior and retain explicit unresolved safety, runtime ordering and production gates.
foreach($name in @('RemovesSingleLeadingDotSeparator','ExplicitProviderPreservesPrefixAndInteriorSeparators','DefaultSearchStopsAtNullRoot','DefaultRootedQueryBypassesJoin','DefaultFirstSuccessfulAttemptWins','DefaultNoSearchEntriesMeansNoOpen','DefaultRegistrationPairUsesNextNode','ExistsProbePassesNullQualificationOutput','ExistsProbeTestsWrapperHandle','AsciiInsensitiveProviderRoutingDoesNotGuaranteeExactBigPrefixStripping','ScopedProviderOpenForwardingRecovered')){
    if(-not $first.$name){throw "Provider-open recovered field differs: $name"}
}
foreach($name in @('NativeWrapperNullRegistrationSafetyProven','NativeJoinedPathBufferSafetyProven','ActualSearchPathContentsRecovered','ActualStartupMountOrderRecovered','GlobalFileProviderPrecedenceRecovered','NativeStreamLifecycleRecovered','TargetExecuted','ModPackageLoaded','AuthenticEp1ProcessingHashRecovered','ProductionBuildReady')){
    if($first.$name){throw "Provider-open unresolved gate unexpectedly claimed: $name"}
}
# Reborn: compare every stable nested field except intentional detached-test counters.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','ForwardPathFixturesExecuted','OpenPlanFixturesExecuted','ComposedPrefixFixturesExecuted','PolicyRejectionsExecuted','MemoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 8 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 8 -Compress)){throw "Repeat provider-open field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 provider-open forwarding: PASS; two complete bodies/one literal, six forward/eight search-plan/four composed-prefix fixtures, three policy rejections, eighteen private faults and repeat JSON. Default search paths differ from explicit-provider opens; native lifecycle/safety, runtime order, authentic compiler identity and game loading remain open.'
