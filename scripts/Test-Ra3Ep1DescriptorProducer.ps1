# Reborn: verify concrete reader/producer evidence and fresh raw projections without granting native transformation or production admission.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep1DescriptorProducer.ps1'
$first=& $audit -ImagePath $ImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.DescriptorProducerRecovered -or -not $first.ConcreteReaderLayoutRecovered -or
    $first.ProjectedRawEntryCount-ne 55519 -or $first.ObservedManifestCount-ne 4 -or $first.RawFixtureRejections-ne 5 -or $first.MemoryFaultsExecuted-ne 7 -or
    $first.CompleteStreamPointerProvenanceRecovered -or $first.LinkedEntryTransformationRecovered -or $first.ReusedCapacitySemanticsRecovered -or
    $first.AllReaderImplementationsRecovered -or $first.Word44SchemaMeaningRecovered -or $first.ProductionBuildReady -or $first.TargetExecuted -or
    $first.NativePayloadBytesRead-ne 0 -or -not $first.ManagedInspectorHashCommandExecuted -or $first.ImageSha256-cne $second.ImageSha256){throw 'Descriptor producer evidence/refusal contract differs.'}
foreach($index in 0..3){if($first.Manifests[$index].Projection.ProjectionSha256-cne $second.Manifests[$index].Projection.ProjectionSha256){throw 'Raw descriptor projection did not repeat.'}}
Write-Output 'Descriptor producer tests: PASS; concrete reader/vtable/producer pins, four manifests/55,519 raw entries, repeated projection hashes, fresh opaque-word fixture, five raw rejections and seven code faults; linked transformations and full stream provenance remain unresolved.'
