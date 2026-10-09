# Reborn: test pinned static lookup/control-flow characterization and independent object identities; never execute target registry initialization.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep1RegistryEvidence.ps1'
$first=& $audit -ImagePath $ImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -AsJson|ConvertFrom-Json
if($first.MatchedObjectNameTypeIds-ne 150 -or $first.ObservedObjectRootTypes-ne 138 -or $first.UnknownWord12TokenizedMismatches-ne 122 -or
    -not $first.LookupUsesTypeId -or $first.LookupConsumesTypeHash -or -not $first.FaultTestsPassed -or
    $first.NodeTypeIdOffset-ne 4 -or $first.NodeRepeatedTypeIdOffset-ne 8 -or $first.NodeObjectOffset-ne 12 -or
    $first.UnknownWord12SemanticsProved -or $first.StartupReachabilityProved -or $first.TargetExecuted -or $first.ProductionBuildReady -or
    ($first.Objects|ConvertTo-Json -Depth 6 -Compress)-cne ($second.Objects|ConvertTo-Json -Depth 6 -Compress)){throw 'Registry evidence/repeat contract differs.'}
$armor=@($first.Objects|Where-Object Name -ceq ArmorTemplate)[0]
$texture=@($first.Objects|Where-Object Name -ceq Texture)[0]
if($armor.RawTypeId-cne '0x3A6C5E8E' -or $texture.RawTypeId-cne '0x21E727DA' -or $texture.UnknownWord12-cne '0x00000000'){throw 'Reviewed object identities differ.'}
Write-Output 'Registry evidence tests: PASS; pinned registration/lookup bodies, 150 FastHash object IDs, 138 stock roots, 122 opaque-flag/tokenized mismatches, repeat JSON, four private-memory rejection faults and opaque-word preservation; no target execution.'
