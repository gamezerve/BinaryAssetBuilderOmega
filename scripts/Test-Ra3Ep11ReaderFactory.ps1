# Reborn: verify scoped 1.1 factory dispatch/cache evidence without claiming live vtable, fresh-node aliasing or allocator safety.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11ReaderFactory.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.ReadOnly -or -not $first.ReferenceDispatchRecovered -or -not $first.ConcreteTableFactoryTargetsRecovered -or
   -not $first.NewOrdinaryReaderCachePublicationRecovered -or $first.OrdinaryFactoryVa-cne '0x004AAC80' -or $first.TagTwoFactoryVa-cne '0x004AAE80' -or
   $first.ConcreteVtableVa-cne '0x00BF8EA0' -or $first.ConstructorVa-cne '0x004AAC10' -or $first.FactoryOwnerArgumentOrdinal-ne 3 -or
   $first.ListOwnerArgumentOrdinal-ne 4 -or $first.ReturnedReaderStoredAtListHeadOffset-ne 8 -or $first.CacheHitValueOffset-ne 32 -or
   $first.RouteFixturesExecuted-ne 6 -or $first.InvalidTagFixturesExecuted-ne 2 -or $first.MemoryFaultsExecuted-ne 11 -or
   $first.FactoryAndListOwnerAreSameArgument -or $first.TagTwoMissCachePublicationInReviewedBody -or $first.CacheHitVtableRevalidatedInReviewedBodies -or
   $first.ReturnedReaderStoredInFreshNodeProven -or $first.CachePublicationHelperSemanticsRecovered -or $first.LiveFactoryOwnerVtableBindingProven -or
   $first.AllSourceObjectBindingsRecovered -or $first.AdmissionHelperSemanticsRecovered -or $first.AllocatorFailureSafetyProven -or
   $first.CompleteStreamPointerProvenanceRecovered -or $first.ProductionBuildReady -or $first.ModPackageLoaded -or
   $first.TargetExecuted -or $first.ManagedInspectorHashCommandExecuted){throw 'EP1 1.1 factory evidence/refusal contract differs.'}
# Reborn: compare all stable nested report fields, excluding intentional fixture counters and self-test switches.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','RouteFixturesExecuted','InvalidTagFixturesExecuted','MemoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 7 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 7 -Compress)){throw "Repeat factory field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 reader factory: PASS; six code/vtable pins, separate factory/list-owner argument mapping, six tag routes, two invalid tags, eleven detached faults and repeat JSON. Cache hits lack reviewed vtable revalidation; universal ownership, helper semantics and game loading remain open.'
