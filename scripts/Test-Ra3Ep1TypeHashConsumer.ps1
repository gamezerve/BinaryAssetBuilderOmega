# Reborn: verify static hash-consumer evidence without changing compiler acceptance, game data or runtime diagnostics.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep1TypeHashConsumer.ps1'
$first=& $audit -ImagePath $ImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -AsJson|ConvertFrom-Json
if(-not $first.ComparisonRecovered -or -not $first.FaultTestsPassed -or $first.BranchCasesExecuted-ne 7 -or $first.MemoryFaultsExecuted-ne 5 -or
    $first.HashCompareVa-cne '0x004AB615' -or $first.CommonContinuationVa-cne '0x004AB704' -or
    ($first.DirectLookupCallCandidateOffsets -join ',')-cne ($second.DirectLookupCallCandidateOffsets -join ',') -or
    $first.ImageSha256-cne $second.ImageSha256 -or $first.UnconditionalMismatchRejectionProved -or
    $first.CompleteStreamPointerProvenanceRecovered -or $first.EngineSkipBranchesAreCompilerPermission -or $first.ProductionBuildReady -or $first.TargetExecuted -or
    -not $first.ManagedInspectorHashCommandExecuted){throw 'Hash consumer evidence/refusal contract differs.'}
Write-Output 'TypeHash consumer tests: PASS; pinned comparison/diagnostic/three separate lookup caller slices, repeat JSON, seven branch cases and five private code faults; compiler admission and unconditional rejection remain unproved.'
