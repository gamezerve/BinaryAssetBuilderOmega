# Reborn: test scoped source-forwarding and membership evidence while retaining unresolved container, vtable and native authoring gates.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep1ReaderOwnership.ps1'
$first=& $audit -ImagePath $ImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.MembershipComparesReaderPointers -or $first.MembershipValidatesAssetContents -or
    -not $first.FirstLoaderFactoryHandleEqualsWrapperSourceHandle -or -not $first.HeaderAndFactoryUseSameWrapperSource -or
    -not $first.ScopedIteratorSourceForwardingRecovered -or -not $first.OrdinaryMembershipSemanticsRecovered -or
    $first.WrapperSourceHandleArgumentOrdinal-ne 2 -or $first.WrapperListOwnerArgumentOrdinal-ne 3 -or
    $first.FirstLoaderFactoryOwnerArgumentOrdinal-ne 3 -or $first.FirstLoaderListOwnerArgumentOrdinal-ne 4 -or
    $first.MembershipListStride-ne 56 -or $first.MembershipValueOffset-ne 8 -or $first.IteratorOwnerHeadOffset-ne 12 -or $first.IteratorSourceHandleOffset-ne 8 -or
    $first.TagTwoAdmissionSemanticsRecovered -or $first.StoredHeadReaderToSpecificIterationProven -or $first.ListContainerOwnershipRecovered -or
    $first.LiveFactoryOwnerVtableBindingProven -or $first.AllSourceObjectBindingsRecovered -or $first.CompleteStreamPointerProvenanceRecovered -or
    $first.ProductionBuildReady -or $first.TargetExecuted -or -not $first.ManagedInspectorHashCommandExecuted -or
    $first.MembershipFixturesExecuted-ne 6 -or $first.InvalidGroupFixturesExecuted-ne 2 -or $first.MemoryFaultsExecuted-ne 11 -or
    $first.ImageSha256-cne $second.ImageSha256){throw 'Reader ownership evidence/refusal contract differs.'}
if(($first.MembershipListHeads -join ',')-cne ($second.MembershipListHeads -join ',') -or
   ($first.IteratorWrapperCalls -join ',')-cne ($second.IteratorWrapperCalls -join ',')){throw 'Reader ownership report did not repeat.'}
Write-Output 'Reader ownership tests: PASS; three complete code pins, wrapper owner forwarding, iterator source-handle branches, six pointer-membership fixtures, two invalid group counts, eleven private code faults and repeat JSON. Container/vtable/all-source provenance remains unresolved.'
