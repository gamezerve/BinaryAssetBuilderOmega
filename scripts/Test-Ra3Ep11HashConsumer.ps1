# Reborn: verify independently pinned 1.1 registry/hash-consumer evidence and refusal flags without replacing diagnostics with an authoring acceptance policy.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11HashConsumer.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.ComparisonRecovered -or -not $first.RegistryBodyLayoutRecovered -or
    -not $first.LookupUsesTypeId -or $first.LookupConsumesTypeHash -or -not $first.RegistrationRepeatsTypeIdNotHash -or
    -not $first.DirectLookupConsumersUseOpaqueMetadataWord12 -or $first.BranchCasesExecuted-ne 7 -or $first.MemoryFaultsExecuted-ne 9 -or
    $first.HashCompareVa-cne '0x004AB8D5' -or $first.CommonContinuationVa-cne '0x004AB9C4' -or $first.ListHeadVa-cne '0x00CF1528' -or
    $first.InitializerToMetadataHashBindingRecovered -or $first.CompleteStreamPointerProvenanceRecovered -or $first.FullRegistryCoverageProved -or
    $first.UnconditionalMismatchRejectionProved -or $first.DiagnosticControlSemanticsRecovered -or $first.UnknownMetadataWord12SemanticsProved -or
    $first.EngineSkipBranchesAreCompilerPermission -or $first.RuntimeConsumerIsAuthoringProcessingHash -or $first.ModPackageLoaded -or
    $first.ProductionBuildReady -or $first.TargetExecuted -or $first.ManagedInspectorHashCommandExecuted -or $first.ImageSha256-cne $second.ImageSha256 -or
    ($first.DirectLookupCallCandidateOffsets -join ',')-cne ($second.DirectLookupCallCandidateOffsets -join ',') -or
    ($first.HashGateCallCandidateOffsets -join ',')-cne ($second.HashGateCallCandidateOffsets -join ',')){throw 'EP1 1.1 consumer evidence/refusal contract differs.'}
Write-Output 'EP1 1.1 hash consumer tests: PASS; seven registry/gate/diagnostic/caller pins, two candidate sets, seven branch fixtures, nine private code faults and repeat JSON. Initializer binding, complete pointer provenance and unconditional rejection remain unresolved.'
