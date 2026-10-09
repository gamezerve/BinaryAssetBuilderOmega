# Reborn: verify static linked addressing and metadata-only chunk bounds without confusing ranges with decoded payload/game compatibility.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep1LinkedChunkRanges.ps1'
$first=& $audit -ImagePath $ImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.ConcreteLinkedChunkAddressingRecovered -or $first.AssetCount-ne 55519 -or $first.RangesChecked-ne 166557 -or
    $first.HeaderBytesReadTotal-ne 288 -or $first.PayloadBytesRead-ne 0 -or $first.PositiveRangeCasesExecuted-ne 4 -or $first.RangeFaultsExecuted-ne 4 -or $first.MemoryFaultsExecuted-ne 9 -or
    $first.NativeReadReturnCheckedInReviewedMethod -or $first.NativeBoundsCheckingProved -or $first.AllSourceObjectBindingsRecovered -or
    $first.CompleteStreamPointerProvenanceRecovered -or $first.FullPayloadIntegrityProved -or $first.ProductionBuildReady -or $first.TargetExecuted -or
    -not $first.ManagedInspectorHashCommandExecuted -or $first.ImageSha256-cne $second.ImageSha256){throw 'Linked chunk evidence/refusal contract differs.'}
foreach($index in 0..3){if($first.Manifests[$index].OrderedRangeSha256-cne $second.Manifests[$index].OrderedRangeSha256){throw 'Linked chunk ranges did not repeat.'}}
Write-Output 'Linked chunk tests: PASS; queue/dispatcher/reader/memcpy pins, four manifests/55,519 entries/166,557 ranges, exact final endpoints, repeat digests, four positive/four rejection range cases and nine code/import faults; no payload reads or production admission.'
