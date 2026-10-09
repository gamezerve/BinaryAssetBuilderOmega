# Reborn: verify scoped EP1 1.1 circular-list alias and release evidence with detached fixtures, exact pins and repeat JSON only.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11ReaderListLifetime.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.ReadOnly -or
   -not $first.FreshNodePayloadRecoveredUnderHealthySentinelInvariant -or
   -not $first.SourceListResetRestoresSelfLinks -or -not $first.ScopedResourceCleanupRecovered -or
   -not $first.ScopedSourceListReleaseRecovered -or -not $first.ConcreteReleaseConditionalObjectFreeRecovered -or
   $first.InsertionVa-cne '0x004CEA62' -or $first.CleanupVa-cne '0x004184B0' -or
   $first.SourceListReleaseVa-cne '0x004ABDEB' -or $first.NodeCleanupVa-cne '0x00497090' -or
   $first.ReleaseThunkVa-cne '0x004171C0' -or $first.ConcreteDeletingTargetVa-cne '0x0049F800' -or
   $first.OwnerSentinelOffset-ne 12 -or $first.NodeReaderOffset-ne 8 -or
   $first.ConcreteReleaseVirtualSlot-ne 8 -or $first.ConcreteReleaseDelegatedSlot-ne 60 -or
   $first.ResourceCleanupClearsSourceField-ne 24 -or $first.ResourceCleanupClearsLinkedField-ne 84 -or
   $first.InsertionScenarioFixturesExecuted-ne 4 -or $first.BrokenListFixturesExecuted-ne 2 -or $first.MemoryFaultsExecuted-ne 12 -or
   $first.ResourceCleanupFreesReaderObjectInReviewedBody -or $first.DeletingTargetFullCacheLifetimeRecovered -or
   $first.AllListInitializersAndInvariantsProven -or $first.ArbitraryCallbackSafetyProven -or
   $first.AllSourceObjectBindingsRecovered -or $first.CompleteStreamPointerProvenanceRecovered -or
   $first.ProductionBuildReady -or $first.ModPackageLoaded -or $first.TargetExecuted -or $first.ManagedInspectorHashCommandExecuted){throw 'Reader lifetime evidence/refusal contract differs.'}
# Reborn: compare every stable nested field while excluding deliberate self-test counters.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','InsertionScenarioFixturesExecuted','BrokenListFixturesExecuted','MemoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 7 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 7 -Compress)){throw "Repeat lifetime field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 reader list lifetime: PASS; seven exact pins, four insertion scenarios, two broken-list fixtures, twelve detached byte faults and repeat JSON. Healthy-list payload alias and scoped cleanup/deleting thunk recovered; full cache lifetime and game loading remain open.'
