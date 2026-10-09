# Reborn: verify the scoped config-queue/reader/wrapper bridge and retain conditional loading and unresolved whole-engine gates.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11ManifestQueue.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.ReadOnly -or -not $first.QueueReaderCreationWhenVectorEmptyRecovered -or
   -not $first.QueueLoadConditionalOnProbeAndDriverBranch -or -not $first.DirectReaderWrapperSourceIsAddressOfStoredNodePayload -or
   -not $first.DirectReaderWrapperSixthArgumentIsHelperArgumentTwo -or $first.ReviewedPins-ne 4 -or $first.MemoryFaultsExecuted-ne 18 -or
   $first.ConfigQueueVa-cne '0x00CF2354' -or $first.QueueEndVa-cne '0x00CF2358' -or $first.QueueDriverVa-cne '0x0064D440' -or
   $first.ReaderVectorOwnerOffset-ne 864 -or $first.QueueEntryStride-ne 4 -or $first.QueueFactoryVa-cne '0x004AAC80' -or
   $first.QueueFactoryCallVa-cne '0x0064D4CF' -or $first.QueueFactoryArgumentTwo-ne 1 -or $first.QueueVariantSetterArgumentOne-ne 1 -or
   $first.QueueProbeVa-cne '0x004AA5B0' -or $first.QueueLoadCallVa-cne '0x0064D6AF' -or $first.DirectReaderLoadVa-cne '0x004D05E0' -or
   $first.QueueDirectLoadArgumentTwo-ne 0 -or $first.ScopedQueueNinthLoaderArgumentValue-ne 0 -or
   $first.DirectReaderMembershipGateVa-cne '0x0045F060' -or $first.DirectReaderListOwnerVa-cne '0x00CF1578' -or
   $first.DirectReaderSentinelVa-cne '0x00CF1584' -or $first.DirectReaderWrapperCallVa-cne '0x004D06DB' -or
   $first.DirectReaderWrapperSourceArgumentOrdinal-ne 2 -or $first.ReviewedWrapperCallerPathsIncludingPriorIterators-ne 3 -or
   $first.DefaultStaticManifest-cne 'static.manifest' -or $first.DefaultModManifest-cne 'mod.manifest' -or ($first.VariantSuffixes -join ',')-cne '_L,_M' -or
   $first.QueueDrainedOrClearedInReviewedDriver -or $first.FactoryNullResultRejectedBeforeVectorAppend -or
   $first.AllWrapperCallerPathsRecovered -or $first.FullVariantFilenameAndFallbackSemanticsRecovered -or
   $first.FullDriverStartupReachabilityRecovered -or $first.FullBigMountSemanticsRecovered -or $first.CompleteStreamPointerProvenanceRecovered -or
   $first.ProductionBuildReady -or $first.ModPackageLoaded -or $first.TargetExecuted -or $first.ManagedInspectorHashCommandExecuted){throw 'Manifest queue evidence/refusal contract differs.'}
# Reborn: compare stable nested report fields, excluding only intentional self-test state.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','MemoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 7 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 7 -Compress)){throw "Repeat manifest queue field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 manifest queue: PASS; four independent pins, config queue to ordinary reader factory and conditional direct-load wrapper bridge, eighteen detached byte faults and repeat JSON. A third wrapper path forwards queue zero to loader argument nine; complete engine provenance and game loading remain open.'
