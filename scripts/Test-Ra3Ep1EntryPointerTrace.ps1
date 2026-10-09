# Reborn: verify static caller provenance while preserving unknown stream-parser, word44 and diagnostic semantics.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep1EntryPointerTrace.ps1'
$first=& $audit -ImagePath $ImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.BoundedCallerPointerTraceRecovered -or -not $first.DiagnosticWrapperDelegationRecovered -or
    $first.HashGateEntryArgumentOrdinal-ne 4 -or $first.DescriptorStride-ne 20 -or $first.ReviewedCopyBytes-ne 48 -or
    $first.StackPositiveCasesExecuted-ne 3 -or $first.StackRejectionCasesExecuted-ne 2 -or $first.MemoryFaultsExecuted-ne 7 -or
    $first.ImageSha256-cne $second.ImageSha256 -or ($first.DirectHashGateCallCandidates -join ',')-cne ($second.DirectHashGateCallCandidates -join ',') -or
    $first.CompleteStreamPointerProvenanceRecovered -or $first.DescriptorArrayConstructionRecovered -or $first.AllThreeCallerPathsTraced -or
    $first.DiagnosticControlSemanticsRecovered -or $first.Word44SchemaMeaningRecovered -or $first.EngineSkipBranchesAreCompilerPermission -or
    $first.ProductionBuildReady -or $first.TargetExecuted -or -not $first.ManagedInspectorHashCommandExecuted){throw 'Entry trace evidence/refusal contract differs.'}
Write-Output 'Entry pointer trace tests: PASS; seven pinned regions, fourth-argument stack derivation, repeat JSON, three positive/two rejection stack cases and seven private code faults; full stream provenance and diagnostic semantics remain unresolved.'
