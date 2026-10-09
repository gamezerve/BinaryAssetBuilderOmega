# Reborn: enforce the scoped 1.1 producer/read evidence, raw projections and refusal of unresolved linked-helper/capacity/game semantics.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11DescriptorProducer.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.DescriptorProducerRecovered -or -not $first.ConcreteReaderPhysicalEntryReadRecovered -or
   -not $first.DescriptorToGatePointerPathRecovered -or -not $first.FreshCountEqualsCapacityProjectionOnly -or -not $first.ProducerNativeLoopUsesCapacity -or
   -not $first.ReadOnly -or $first.DescriptorProducerVa-cne '0x00449750' -or $first.ReaderVtableVa-cne '0x00BF8EA0' -or
   $first.PhysicalHeaderBytes-ne 52 -or $first.RawEntryStride-ne 48 -or $first.DescriptorStride-ne 20 -or $first.DescriptorEntryPointerOffset-ne 12 -or
   $first.ObservedManifestCount-ne 4 -or $first.ProjectedRawEntryCount-ne 55519 -or $first.RawFixtureRejections-ne 5 -or $first.MemoryFaultsExecuted-ne 8 -or
   $first.ReusedCapacitySemanticsRecovered -or $first.PostReadHelperSemanticsRecovered -or $first.CompleteStreamPointerProvenanceRecovered -or
   $first.AllReaderImplementationsRecovered -or $first.AllGateCallerPathsRecovered -or $first.Word44SchemaMeaningRecovered -or
   $first.FreshEp11ArchiveExtractionPerformed -or $first.NativePayloadBytesRead-ne 0 -or $first.TargetExecuted -or
   $first.ManagedInspectorHashCommandExecuted -or $first.RuntimeConsumerIsAuthoringProcessingHash -or $first.ProductionBuildReady -or $first.ModPackageLoaded){throw 'Producer evidence/refusal contract differs.'}
# Reborn: compare every stable nested evidence field, omitting only intentional self-test switches and counters.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','RawFixtureRejections','MemoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 9 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 9 -Compress)){throw "Repeat producer field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 descriptor producer: PASS; seven code/vtable pins, four stock manifest projections (55,519 entries), five malformed raw fixtures, eight private code faults and repeat JSON. Linked helper, reused capacity and complete/game provenance remain open.'
