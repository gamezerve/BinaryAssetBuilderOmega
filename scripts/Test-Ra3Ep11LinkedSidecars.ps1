# Reborn: verify 1.1 linked setup and header-only stock evidence without claiming payload integrity, fatal diagnostics or game loading.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11LinkedSidecars.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.ReadOnly -or -not $first.SidecarSetupRecovered -or -not $first.NativeChecksumDiagnosticsRecovered -or
    -not $first.PostReadHelperSemanticsRecovered -or $first.LinkedHelperVa-cne '0x00449810' -or $first.WholeSidecarReaderVa-cne '0x0043D050' -or
    $first.HelperRole-cne 'linked-sidecar-setup-not-entry-transformation' -or $first.ObservedSidecarCount-ne 12 -or
    $first.HeaderBytesReadSinglePass-ne 96 -or $first.HeaderBytesReadTotal-ne 192 -or $first.PayloadBytesRead-ne 0 -or
    $first.WorldBuilderBinBytes-ne 1394571528 -or $first.HeaderFaultsExecuted-ne 4 -or $first.MemoryFaultsExecuted-ne 11 -or
    $first.SuppliedEntryArgumentsReadByReviewedHelper -or $first.DirectEntryHashOrSizeWritesObserved -or $first.NativeMagicValidationProved -or
    $first.NativeUnconditionalChecksumRejectionProved -or $first.CompleteStreamPointerProvenanceRecovered -or $first.AllSourceObjectBindingsRecovered -or
    $first.FreshEp11ArchiveExtractionPerformed -or $first.FullSidecarPayloadIntegrityProved -or $first.ReusedCapacitySemanticsRecovered -or
    $first.ProductionBuildReady -or $first.ModPackageLoaded -or $first.TargetExecuted -or $first.ManagedInspectorHashCommandExecuted){throw 'EP1 1.1 sidecar evidence/refusal contract differs.'}
# Reborn: compare every stable nested field across a second invocation, excluding self-test switches and intentional fixture counters only.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','HeaderFaultsExecuted','MemoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 9 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 9 -Compress)){throw "Repeat sidecar field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 linked sidecars: PASS; two complete helper bodies, three suffix pins, twelve header/length/checksum checks, four detached header faults, eleven private code/literal faults and repeat JSON. WorldBuilder included; 192 header bytes per audit, zero payload bytes. Complete provenance and game loading remain open.'
