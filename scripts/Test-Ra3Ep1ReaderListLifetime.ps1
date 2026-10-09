# Reborn: verify conditional fresh-node alias recovery and separate cleanup/release scopes without promoting fixture invariants to universal native safety.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep1ReaderListLifetime.ps1'
$first=& $audit -ImagePath $ImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.FreshNodePayloadRecoveredUnderHealthySentinelInvariant -or
    -not $first.SourceListResetRestoresSelfLinks -or -not $first.ScopedResourceCleanupRecovered -or -not $first.ScopedSourceListReleaseRecovered -or -not $first.ConcreteReleaseConditionalObjectFreeRecovered -or
    $first.OwnerSentinelOffset-ne 12 -or $first.NodeReaderOffset-ne 8 -or $first.ConcreteReleaseVirtualSlot-ne 8 -or $first.ConcreteReleaseDelegatedSlot-ne 60 -or
    $first.ResourceCleanupFreesReaderObjectInReviewedBody -or $first.DeletingTargetFullCacheLifetimeRecovered -or $first.AllListInitializersAndInvariantsProven -or
    $first.ArbitraryCallbackSafetyProven -or $first.AllSourceObjectBindingsRecovered -or $first.CompleteStreamPointerProvenanceRecovered -or
    $first.ProductionBuildReady -or $first.TargetExecuted -or -not $first.ManagedInspectorHashCommandExecuted -or
    $first.InsertionScenarioFixturesExecuted-ne 4 -or $first.BrokenListFixturesExecuted-ne 2 -or $first.MemoryFaultsExecuted-ne 12 -or
    $first.ImageSha256-cne $second.ImageSha256){throw 'Reader lifetime evidence/refusal contract differs.'}
if(($first.Scenarios|ConvertTo-Json -Compress)-cne ($second.Scenarios|ConvertTo-Json -Compress)){throw 'Reader lifetime report did not repeat.'}
Write-Output 'Reader list lifetime tests: PASS; seven exact pins, four empty/successive insertion scenarios, two malformed list fixtures, twelve private byte faults and repeat JSON. Fresh-node payload is proven under healthy sentinel invariants; all-list/cache/callback guarantees remain unresolved.'
