# Reborn: verify the separate EP1 1.1 modconfig evidence boundary without claiming package loading or reusing 1.0 type-table addresses.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11ModConfigEvidence.ps1'
$first=& $audit -ImagePath $ImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.OptionRegistrationRecovered -or -not $first.ParserHandlerDispatchRecovered -or
    -not $first.ConfigPathStorageRecovered -or -not $first.NonemptyPathConsumerRecovered -or $first.OptionEntryIndex-ne 1 -or
    $first.OptionTableEntryCount-ne 4 -or $first.PathBufferCapacity-ne 256 -or $first.MemoryFaultsExecuted-ne 7 -or
    $first.CompleteConfigFileParserRecovered -or $first.LauncherArgumentForwardingProven -or $first.ModPackageLoaded -or
    $first.Ep1ProcessingHashRecovered -or $first.OldRuntimeOffsetsCompatible -or $first.TargetExecuted -or $first.ProductionBuildReady -or
    $first.ImageSha256-cne $second.ImageSha256 -or $first.PathBufferVa-cne $second.PathBufferVa){throw 'EP1 1.1 modconfig evidence/refusal contract differs.'}
Write-Output 'EP1 1.1 modconfig evidence: PASS; seven pins, parser/table/handler/setter/consumer flow, import binding, seven detached code faults and repeat JSON. Actual mod loading and launcher argument forwarding are not proven.'
