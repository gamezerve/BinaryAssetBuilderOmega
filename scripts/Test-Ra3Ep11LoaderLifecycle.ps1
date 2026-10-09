# Reborn: verify one 1.1 wrapper's fresh context and normal cleanup without claiming universal reuse, allocator safety or game compatibility.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11LoaderLifecycle.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.ReadOnly -or -not $first.FreshWrapperContextRecovered -or
   -not $first.NormalExitBufferReleaseRecovered -or -not $first.FreshCapacityModelAssumesSuccessfulAllocation -or -not $first.ConcreteReaderConstructionRecovered -or
   $first.WrapperVa-cne '0x004CFEB0' -or $first.ContextDestructorVa-cne '0x00449700' -or $first.ContextDestructorCallVa-cne '0x004D0016' -or
   $first.WrapperLoaderArgumentCount-ne 9 -or $first.WrapperArgumentCleanupBytes-ne 36 -or $first.FirstLoaderContextArgumentOrdinal-ne 7 -or
   $first.ObservedManifestCount-ne 4 -or $first.FreshInvocationCasesExecuted-ne 4 -or $first.MemoryFaultsExecuted-ne 7 -or
   $first.CrossWrapperEntryCapacityReused -or $first.AllocatorFailureSemanticsRecovered -or $first.AllReaderCapacityReuseRecovered -or
   $first.AllSourceObjectBindingsRecovered -or $first.CompleteStreamPointerProvenanceRecovered -or $first.ProductionBuildReady -or
   $first.ModPackageLoaded -or $first.TargetExecuted -or $first.ManagedInspectorHashCommandExecuted){throw 'EP1 1.1 lifecycle evidence/refusal contract differs.'}
# Reborn: compare all stable evidence fields, excluding only intentional self-test switches and fixture counters.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','FreshInvocationCasesExecuted','MemoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 7 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 7 -Compress)){throw "Repeat lifecycle field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 loader lifecycle: PASS; four exact wrapper/cleanup/constructor pins, nine-argument seventh-context stack mapping, four fresh invocation fixtures, seven detached code faults and repeat JSON. Fresh capacity proven for this wrapper normal-flow scope only; allocator failures, all source bindings and game loading remain open.'
