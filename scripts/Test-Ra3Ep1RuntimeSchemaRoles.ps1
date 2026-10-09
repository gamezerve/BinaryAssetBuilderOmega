# Reborn: validate actual schema-role inventory and repeat JSON without compiling schemas or activating runtime/compiler registrations.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep1RuntimeSchemaRoles.ps1'
$first=& $audit -ImagePath $ImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -AsJson | ConvertFrom-Json
if ($first.SchemaFiles -ne 843 -or $first.SchemaComplexTypes -ne 1390 -or $first.RuntimeRows -ne 1342 -or
    $first.DirectChoiceRuntimeTypes -ne 299 -or $first.NonDirectComplexRuntimeTypes -ne 1043 -or
    $first.ObservedStockRoots -ne 254 -or $first.ObservedNonDirectRoots -ne 0 -or $first.SchemaOnlyComplexTypes -ne 48 -or
    $first.DuplicateComplexNames.Count -ne 2 -or $first.FaultCasesExecuted -ne 4 -or -not $first.FaultTestsPassed -or
    $first.SchemaCatalogSha256 -cne $second.SchemaCatalogSha256 -or $first.SchemaCompiled -or $first.ProductionBuildReady -or
    ($first.Rows|ConvertTo-Json -Depth 8 -Compress) -cne ($second.Rows|ConvertTo-Json -Depth 8 -Compress) -or
    ($first.SchemaOnly|ConvertTo-Json -Depth 8 -Compress) -cne ($second.SchemaOnly|ConvertTo-Json -Depth 8 -Compress)) { throw 'Actual schema role/repeat contract differs.' }
$event=@($first.Rows|Where-Object Name -ceq 'PathMusicEvent')[0]
$runtimeEvent=@($first.Rows|Where-Object Name -ceq 'PathMusicEventRuntime')[0]
if ($event.Role -cne 'direct-asset-choice' -or $runtimeEvent.Role -cne 'complex-not-direct-choice' -or
    -not $event.ObservedStockRoot -or $runtimeEvent.ObservedStockRoot -or -not $runtimeEvent.RuntimeSuffix) { throw 'Authored/runtime music roles conflated.' }
Write-Output 'Runtime schema role tests: PASS; 1,342 names, 299 direct choices, 1,043 non-direct complex names, 48 schema-only names, duplicate provenance, authored/runtime distinction, repeat JSON, positive fixture and four rejection cases.'
