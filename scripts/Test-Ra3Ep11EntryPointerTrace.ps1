# Reborn: verify the scoped 1.1 entry-pointer trace, deterministic evidence and explicit upstream/all-caller refusal flags.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11EntryPointerTrace.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.DescriptorToGatePointerPathRecovered -or -not $first.ReadOnly -or
   $first.ReviewedCallerCount-ne 1 -or $first.DescriptorStride-ne 20 -or $first.DescriptorEntryPointerOffset-ne 12 -or
   $first.PendingStackRecordStride-ne 28 -or $first.PendingEntryPointerOffset-ne 4 -or $first.ReviewedCopyBytes-ne 48 -or
   $first.HashGateEntryArgumentOrdinal-ne 4 -or $first.ReviewedGateCallVa-cne '0x004CEFA0' -or
   $first.ValidStackFixturesExecuted-ne 3 -or $first.MalformedStackFixturesExecuted-ne 4 -or $first.MemoryFaultsExecuted-ne 7 -or
   $first.StreamEntryReadRecovered -or $first.DescriptorConstructionRecovered -or $first.CompleteStreamPointerProvenanceRecovered -or
   $first.AllGateCallerPathsRecovered -or $first.FullRegistryCoverageProved -or $first.RuntimeConsumerIsAuthoringProcessingHash -or
   $first.EngineSkipBranchesAreCompilerPermission -or $first.ModPackageLoaded -or $first.ProductionBuildReady -or
   $first.TargetExecuted -or $first.ManagedInspectorHashCommandExecuted){throw 'EP1 1.1 trace evidence/refusal contract differs.'}
# Reborn: compare every stable report field, excluding only counters/switches that intentionally differ between self-test and plain audit.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','ValidStackFixturesExecuted','MalformedStackFixturesExecuted','MemoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Compress)){throw "Repeat trace field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 entry pointer trace: PASS; six code pins, three stack fixtures, four malformed stacks, seven private code faults and repeat JSON. One descriptor-to-gate caller path only; stream reads/construction/all callers remain open.'
