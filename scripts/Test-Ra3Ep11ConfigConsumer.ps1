# Reborn: enforce the distinction between option dispatch, file probing and config command consumption without native execution.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11ConfigConsumer.ps1'
$first=& $audit -ImagePath $ImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.ReadOnly -or -not $first.ModConfigDispatchSupported -or
   -not $first.TopLevelProbeThenConfigReadRecovered -or -not $first.ReadDispatchesWholeNullTerminatedBuffer -or
   -not $first.AddConfigRecursiveReadRecovered -or -not $first.TryAddConfigUsesFileProbe -or
   $first.ReviewedPins-ne 8 -or $first.DirectiveCount-ne 9 -or $first.PrefixFixturesExecuted-ne 11 -or
   $first.InvalidFixturePoliciesExecuted-ne 3 -or $first.MemoryFaultsExecuted-ne 18 -or
   $first.PathJoinVa-cne '0x004D69A0' -or $first.FileProbeVa-cne '0x004D6F10' -or $first.SinglePathProbeVa-cne '0x004D6DC0' -or
   $first.ConfigReadVa-cne '0x004D86B0' -or $first.LineSplitterVa-cne '0x004D9040' -or $first.DirectiveDispatchVa-cne '0x004D8DE0' -or
   $first.PathBufferCapacity-ne 256 -or $first.NativeLineDiagnosticThreshold-ne 1024 -or
   $first.AddManifestAppendsToGlobalListVa-cne '0x00CF2354' -or $first.AddStrAppendsToGlobalListVa-cne '0x00CF2364' -or
   $first.ProbeIsDirectiveParser -or $first.NativeReadResultChecked -or $first.NativeAllocationFailureHandled -or
   $first.LineLengthDiagnosticIsUnconditionalRejection -or $first.ConfigReadSuccessMeansAllCommandsSucceeded -or
   $first.FullBigMountSemanticsRecovered -or $first.FullManifestQueueConsumptionRecovered -or
   $first.RecursiveConfigCycleSafetyProven -or $first.NativeLocaleAndQuotingRecovered -or
   $first.ProductionBuildReady -or $first.ModPackageLoaded -or $first.TargetExecuted){throw 'Config consumer evidence/refusal contract differs.'}
# Reborn: compare every stable nested field, retaining command order, literal paths and helper routing.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','PrefixFixturesExecuted','InvalidFixturePoliciesExecuted','MemoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 7 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 7 -Compress)){throw "Repeat config consumer field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 config consumer: PASS; eight pins, nine native directive prefixes, eleven detached prefix fixtures, three policy rejections, eighteen detached byte faults and repeat JSON. Probe/read/dispatch separated; full BIG mounting, manifest consumption and game loading remain open.'
