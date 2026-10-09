# Reborn: verify the narrowly decoded 1.1 initializer sample and metadata identities while preserving unproven startup/full-coverage and authoring gates.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11TypeInitializers.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.SampleInitializerToMetadataHashBindingRecovered -or -not $first.RegistryConsumerBodiesPinned -or
    $first.DecodedInitializers-ne 150 -or $first.PlainTemplateCount-ne 43 -or $first.VtableTemplateCount-ne 107 -or $first.RuntimeRowsNotCovered-ne 1192 -or
    $first.MatchedMetadataNameTypeIds-ne 150 -or $first.ObservedMetadataRootTypes-ne 138 -or $first.ObjectHashStoreOffset-ne 8 -or
    $first.UnknownWord12TokenizedMismatches-ne 122 -or $first.PathMusicEventInDecodedSample -or
    $first.RejectedMemoryFaultsExecuted-ne 10 -or $first.OpaquePreservationCasesExecuted-ne 1 -or $first.UnknownWord12SemanticsProved -or
    $first.StartupReachabilityProved -or $first.FullInitializerCoverageProved -or $first.GeneralDisassembly -or $first.CompleteStreamPointerProvenanceRecovered -or
    $first.Ep1ProcessingHashRecovered -or $first.AllTypesHashDerivationRecovered -or $first.ModPackageLoaded -or $first.ProductionBuildReady -or $first.TargetExecuted -or
    -not $first.ManagedInspectorHashCommandExecuted -or $first.ImageSha256-cne $second.ImageSha256 -or
    ($first.Rows|ConvertTo-Json -Depth 6 -Compress)-cne ($second.Rows|ConvertTo-Json -Depth 6 -Compress) -or
    ($first.Objects|ConvertTo-Json -Depth 6 -Compress)-cne ($second.Objects|ConvertTo-Json -Depth 6 -Compress)){throw 'EP1 1.1 initializer evidence/refusal contract differs.'}
Write-Output 'EP1 1.1 initializer tests: PASS; 150 reviewed template rows (43 short/107 vtable), 150 metadata TypeIds, 138 stock roots, ten private faults, opaque-word preservation and repeat JSON. Full startup reachability/type coverage and authoring hashes remain unresolved.'
