# Reborn: validate 1.1 pointer membership and scoped iterator/wrapper forwarding without claiming universal reader lifetime or ninth-argument semantics.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11ReaderOwnership.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.ReadOnly -or -not $first.MembershipComparesReaderPointers -or
   -not $first.FirstLoaderFactoryHandleEqualsWrapperSourceHandle -or -not $first.HeaderAndFactoryUseSameWrapperSource -or
   -not $first.ScopedIteratorSourceForwardingRecovered -or -not $first.OrdinaryMembershipSemanticsRecovered -or
   -not $first.IteratorSourceHandleIsCopiedOnAlternateBranch -or $first.PointerMembershipHelperVa-cne '0x0045F060' -or
   $first.IteratorVa-cne '0x004D04A0' -or $first.MembershipListStride-ne 56 -or $first.MembershipValueOffset-ne 8 -or
   $first.WrapperSourceHandleArgumentOrdinal-ne 2 -or $first.WrapperListOwnerArgumentOrdinal-ne 3 -or
   $first.FirstLoaderFactoryOwnerArgumentOrdinal-ne 3 -or $first.FirstLoaderListOwnerArgumentOrdinal-ne 4 -or
   $first.ReviewedWrapperCallerPaths-ne 2 -or $first.WrapperCallCandidateOffsets.Count-ne 4 -or
   $first.IteratorSixthWrapperArgumentValue-ne 0 -or $first.IteratorNinthLoaderArgumentValue-ne 0 -or
   $first.MembershipFixturesExecuted-ne 6 -or $first.InvalidGroupFixturesExecuted-ne 2 -or $first.MemoryFaultsExecuted-ne 12 -or
   $first.MembershipValidatesAssetContents -or $first.NinthArgumentConsumerRecovered -or $first.AllWrapperCallerPathsRecovered -or
   $first.TagTwoAdmissionSemanticsRecovered -or $first.StoredHeadReaderToSpecificIterationProven -or $first.ListContainerOwnershipRecovered -or
   $first.LiveFactoryOwnerVtableBindingProven -or $first.AllSourceObjectBindingsRecovered -or $first.CompleteStreamPointerProvenanceRecovered -or
   $first.ProductionBuildReady -or $first.ModPackageLoaded -or $first.TargetExecuted -or $first.ManagedInspectorHashCommandExecuted){throw 'Ownership evidence/refusal contract differs.'}
# Reborn: compare every stable nested field except deliberate fixture counters/self-test switches.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','MembershipFixturesExecuted','InvalidGroupFixturesExecuted','MemoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 7 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 7 -Compress)){throw "Repeat ownership field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 reader ownership: PASS; three complete helper/wrapper/iterator pins, four raw call candidates/two reviewed iterator paths, owner stack mappings, six membership fixtures/two invalid groups, twelve detached code faults and repeat JSON. Iterator forwards zero to ninth loader argument; full consumer, list insertion/lifetime and game loading remain open.'
