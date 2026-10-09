# Reborn: check concrete 1.1 chunk addressing and metadata-only bounds without claiming decoded payloads, engine safety or working mods.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11LinkedChunkRanges.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.ReadOnly -or -not $first.ConcreteLinkedChunkAddressingRecovered -or
   $first.QueueMethodVa-cne '0x00417210' -or $first.DispatchMethodVa-cne '0x00418420' -or $first.LinkedChunkMethodVa-cne '0x00418340' -or
   $first.CopyThunkVa-cne '0x004DA17A' -or $first.CopyImport-cne 'memcpy' -or $first.HeaderBias-ne 8 -or $first.QueueDescriptorPointerOffset-ne 16 -or
   $first.ObservedManifestCount-ne 4 -or $first.AssetCount-ne 55519 -or $first.RangesChecked-ne 166557 -or
   $first.HeaderBytesReadTotal-ne 288 -or $first.PayloadBytesRead-ne 0 -or $first.PositiveRangeCasesExecuted-ne 4 -or
   $first.RangeFaultsExecuted-ne 4 -or $first.MemoryFaultsExecuted-ne 9 -or $first.NativeReadReturnCheckedInReviewedMethod -or
   $first.NativeBoundsCheckingProved -or $first.AllSourceObjectBindingsRecovered -or $first.CompleteStreamPointerProvenanceRecovered -or
   $first.FullPayloadIntegrityProved -or $first.ReusedCapacitySemanticsRecovered -or $first.FreshEp11ArchiveExtractionPerformed -or
   $first.ModPackageLoaded -or $first.ProductionBuildReady -or $first.TargetExecuted -or $first.ManagedInspectorHashCommandExecuted){throw 'EP1 1.1 chunk evidence/refusal contract differs.'}
# Reborn: compare all stable nested evidence fields, excluding deliberate differences in fixture switches/counters only.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','PositiveRangeCasesExecuted','RangeFaultsExecuted','MemoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 9 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 9 -Compress)){throw "Repeat chunk field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 linked chunk ranges: PASS; queue/vtable/dispatcher/reader/memcpy pins, four manifests/55,519 entries/166,557 ranges, exact final endpoints, four positive/four invalid range fixtures, nine detached code/import faults and repeat JSON. WorldBuilder included, 288 header bytes per audit, zero payload bytes; no native safety/game-loading proof.'
