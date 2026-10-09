# Reborn: require scoped native cache evidence and explicit unresolved safety/provenance flags, with detached fault tests and repeat JSON.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11ReaderCacheLifetime.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.ReadOnly -or -not $first.HealthyListInsertionRecovered -or
   -not $first.ScopedTwoCacheRemovalCallsRecovered -or -not $first.ScopedPublicationPayloadSlotRecovered -or
   -not $first.RemovalFreesNodeInReviewedBody -or -not $first.DestructorCallsResourceCleanup -or
   $first.ReviewedWholeBodyPins-ne 6 -or $first.DeletingFlagFixturesExecuted-ne 5 -or $first.MemoryFaultsExecuted-ne 18 -or
   $first.DestructorVa-cne '0x0049B6F0' -or $first.LookupVa-cne '0x00677520' -or $first.RemovalVa-cne '0x004970F0' -or
   $first.PublicationVa-cne '0x006A34B0' -or $first.LessThanVa-cne '0x004495D0' -or $first.MemberCleanupVa-cne '0x004D1970' -or
   $first.OrdinaryCacheVa-cne '0x00CC4ADC' -or $first.OrdinarySentinelVa-cne '0x00CC4AE0' -or
   $first.TagTwoCacheVa-cne '0x00CC4AF8' -or $first.TagTwoSentinelVa-cne '0x00CC4AFC' -or
   $first.OrdinaryDestructorKeyReaderOffset-ne 16 -or $first.TagTwoDestructorKeyReaderOffset-ne 12 -or
   $first.RemovalDecrementsCacheCountOffset-ne 20 -or $first.PublicationExistingPayloadAddressOffset-ne 32 -or
   $first.PublicationInsertedPayloadAddressOffset-ne 32 -or $first.DestructorFinalVtableVa-cne '0x00BF7EB8' -or
   ($first.DestructorAdditionalFreedReaderFields -join ',')-cne '8,12,16,20' -or
   ($first.DestructorMemberCleanupReaderOffsets -join ',')-cne '96,92,88' -or
   $first.RemovalDirectlyFreesReaderPayload -or $first.KeyNormalizationSemanticsRecovered -or
   $first.ByteComparisonCalleeSemanticsRecovered -or $first.FullTreeLookupAndInsertionRecovered -or $first.FullTreeRemovalRecovered -or
   $first.MemberCleanupLockAndReferenceSafetyProven -or $first.AllSourceObjectBindingsRecovered -or
   $first.DeletingTargetFullCacheLifetimeRecovered -or $first.CompleteStreamPointerProvenanceRecovered -or
   $first.ProductionBuildReady -or $first.ModPackageLoaded -or $first.TargetExecuted -or $first.ManagedInspectorHashCommandExecuted){throw 'Cache lifetime evidence/refusal contract differs.'}
# Reborn: preserve nested report identity while excluding intentional self-test counters.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','DeletingFlagFixturesExecuted','MemoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 7 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 7 -Compress)){throw "Repeat cache lifetime field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 reader cache lifetime: PASS; six whole-body pins, five deleting flag predicates, eighteen detached byte faults and repeat JSON. Scoped two-cache removal and publication payload addresses recovered; full tree/key/member safety and game loading remain open.'
