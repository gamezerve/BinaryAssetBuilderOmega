# Reborn: verify bounded linked stream headers and code evidence without confusing valid metadata with validated native payloads.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep1LinkedSidecars.ps1'
$first=& $audit -ImagePath $ImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.SidecarSetupRecovered -or -not $first.NativeChecksumDiagnosticsRecovered -or
    $first.ObservedSidecarCount-ne 12 -or $first.HeaderBytesReadTotal-ne 192 -or $first.PayloadBytesRead-ne 0 -or $first.WorldBuilderBinBytes-ne 1394571528 -or
    $first.HeaderFaultsExecuted-ne 4 -or $first.MemoryFaultsExecuted-ne 8 -or $first.DirectEntryHashOrSizeWritesObserved -or $first.SuppliedEntryArgumentsReadByReviewedHelper -or
    $first.NativeMagicValidationProved -or $first.NativeUnconditionalChecksumRejectionProved -or $first.CompleteStreamPointerProvenanceRecovered -or
    $first.FullSidecarPayloadIntegrityProved -or $first.ProductionBuildReady -or $first.TargetExecuted -or -not $first.ManagedInspectorHashCommandExecuted -or
    $first.ImageSha256-cne $second.ImageSha256){throw 'Linked sidecar evidence/refusal contract differs.'}
foreach($index in 0..11){if([Convert]::ToHexString($first.Sidecars[$index].Snapshot.Header)-cne [Convert]::ToHexString([byte[]]$second.Sidecars[$index].Snapshot.Header) -or $first.Sidecars[$index].Snapshot.Length-ne $second.Sidecars[$index].Snapshot.Length){throw 'Linked header report did not repeat.'}}
Write-Output 'Linked sidecar tests: PASS; full reviewed helper pins, twelve headers including large WorldBuilder.bin, checksum/exact lengths, repeat JSON, four detached header faults and eight code faults; zero payload reads, no entry transformation or production admission.'
