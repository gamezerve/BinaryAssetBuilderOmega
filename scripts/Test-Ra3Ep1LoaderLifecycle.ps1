# Reborn: verify fresh wrapper ownership without elevating scoped lifecycle evidence to all-reader or production compatibility.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep1LoaderLifecycle.ps1'
$first=& $audit -ImagePath $ImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.FreshWrapperContextRecovered -or -not $first.NormalExitBufferReleaseRecovered -or
    -not $first.ConcreteReaderConstructionRecovered -or $first.CrossWrapperEntryCapacityReused -or $first.FreshInvocationCasesExecuted-ne 4 -or $first.MemoryFaultsExecuted-ne 7 -or
    $first.AllReaderCapacityReuseRecovered -or $first.AllocatorFailureSemanticsRecovered -or $first.AllSourceObjectBindingsRecovered -or
    $first.CompleteStreamPointerProvenanceRecovered -or $first.ProductionBuildReady -or $first.TargetExecuted -or -not $first.ManagedInspectorHashCommandExecuted -or
    $first.ImageSha256-cne $second.ImageSha256 -or $first.FirstLoaderContextArgumentOrdinal-ne 7){throw 'Loader lifecycle evidence/refusal contract differs.'}
foreach($index in 0..3){if($first.Capacities[$index].EntryBytes-ne $second.Capacities[$index].EntryBytes -or $first.Capacities[$index].InitialCapacity-ne 0){throw 'Fresh lifecycle report did not repeat.'}}
Write-Output 'Loader lifecycle tests: PASS; wrapper/context/destructor/constructor pins, argument-seven binding, four fresh successive/zero-count fixtures, repeat JSON and seven private code faults; all-reader binding and allocator behavior remain unresolved.'
