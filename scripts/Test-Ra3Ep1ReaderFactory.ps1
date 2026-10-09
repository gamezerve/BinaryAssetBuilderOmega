# Reborn: verify bounded factory/cache/storage evidence and retained refusal flags using repeated reports and detached mutation fixtures.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep1ReaderFactory.ps1'
$first=& $audit -ImagePath $ImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.ReferenceDispatchRecovered -or -not $first.NewOrdinaryReaderCachePublicationRecovered -or
    -not $first.ConcreteTableFactoryTargetsRecovered -or $first.FactoryOwnerArgumentOrdinal-ne 3 -or $first.ListOwnerArgumentOrdinal-ne 4 -or
    $first.FactoryAndListOwnerAreSameArgument -or $first.CacheHitVtableRevalidatedInReviewedBodies -or $first.TagTwoMissCachePublicationInReviewedBody -or
    $first.ReturnedReaderStoredInFreshNodeProven -or $first.LiveFactoryOwnerVtableBindingProven -or $first.AllSourceObjectBindingsRecovered -or
    $first.AdmissionHelperSemanticsRecovered -or $first.AllocatorFailureSafetyProven -or $first.CompleteStreamPointerProvenanceRecovered -or
    $first.ProductionBuildReady -or $first.TargetExecuted -or -not $first.ManagedInspectorHashCommandExecuted -or
    $first.RouteFixturesExecuted-ne 6 -or $first.InvalidTagFixturesExecuted-ne 2 -or $first.MemoryFaultsExecuted-ne 11 -or
    $first.ImageSha256-cne $second.ImageSha256 -or $first.ReturnedReaderStoredAtListHeadOffset-ne 8 -or $first.ListOwnerHeadFieldOffset-ne 12){throw 'Reader factory evidence/refusal contract differs.'}
if(($first.Routes|ConvertTo-Json -Compress)-cne ($second.Routes|ConvertTo-Json -Compress) -or
   ($first.OrdinaryConstructorArguments -join ',')-cne ($second.OrdinaryConstructorArguments -join ',') -or
   ($first.TagTwoConstructorArguments -join ',')-cne ($second.TagTwoConstructorArguments -join ',')){throw 'Factory report did not repeat.'}
Write-Output 'Reader factory tests: PASS; six code/table pins, distinct owner arguments, six tag routes, two invalid tags, eleven detached faults and repeat JSON. Live owner vtable, cache provenance and admission semantics remain unresolved.'
