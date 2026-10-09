# Reborn: enforce version-specific argument provenance without conflating the existing eighth flag with an unproved ninth-argument feature.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11LoaderArguments.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.ReadOnly -or -not $first.NinthArgumentSourceRecovered -or -not $first.EighthFlagOrdinalAndForwardingRecovered -or
   -not $first.FlagWasAlreadyPresentInBaseline -or $first.CurrentLoaderArguments-ne 9 -or $first.BaselineLoaderArguments-ne 8 -or
   $first.AddedArgumentOrdinal-ne 9 -or $first.AddedArgumentSourceWrapperOrdinal-ne 6 -or $first.PreservedFlagOrdinal-ne 8 -or
   $first.FirstBranchFlagValue-ne 1 -or $first.SecondBranchFlagValue-ne 0 -or $first.FlagForwardedGateOrdinal-ne 7 -or
   $first.PositiveStackChecks-ne 6 -or $first.InvalidStackFixtures-ne 4 -or $first.MemoryFaultsExecuted-ne 10 -or
   $first.NinthArgumentConsumerRecovered -or $first.CompleteFlagSemanticsRecovered -or $first.FlagIsTypeHashBypassPermission -or
   $first.AllLoaderCallersRecovered -or $first.LiveFactoryBindingRecovered -or $first.CompleteStreamPointerProvenanceRecovered -or
   $first.ProductionBuildReady -or $first.ModPackageLoaded -or $first.TargetExecuted -or $first.ManagedInspectorHashCommandExecuted){throw 'Argument evidence/refusal contract differs.'}
# Reborn: compare every stable report field, excluding self-test-only counters and switches.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','PositiveStackChecks','InvalidStackFixtures','MemoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Compress)){throw "Repeat argument field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 loader arguments: PASS; six current/baseline pins, six positive stack checks, four invalid stack fixtures, ten detached image faults and repeat JSON. Added ninth argument comes from wrapper argument six; existing eighth flag is distinct. Ninth consumer/full semantics/game loading remain open.'
