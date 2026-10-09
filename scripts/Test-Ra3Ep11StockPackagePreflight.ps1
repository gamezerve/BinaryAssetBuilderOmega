# Reborn: require repeatable stock SKU/archive-directory evidence while explicitly refusing payload validation and game-load readiness.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[Parameter(Mandatory=$true)] [string] $SkuDefinitionPath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11StockPackagePreflight.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -AsJson|ConvertFrom-Json
if(-not $first.ReadOnly -or -not $first.StockSkuExecutableMatchesAuditedImage -or -not $first.AvailableStockArchiveDirectoryBoundsValidated -or
   $first.SkuDefinitionSha256-cne '416C0752EA6891F13916D60FB1C8C3B8E5F342F7E919214866FC5D6EE962D75E' -or $first.SkuBytes-ne 455 -or
   $first.Executable-cne 'Data\ra3ep1_1.1.game' -or $first.SearchPath-cne 'big:;.' -or $first.ConfiguredArchiveCount-ne 14 -or
   $first.ArchiveCount+$first.MissingConfiguredArchives.Count-ne 14 -or $first.ArchivePayloadBytesRead-ne 0 -or $first.SkuMetadataBytesRead-ne 910 -or
   $first.ArchiveDirectoryMetadataBytesRead-ne 3*($first.Archives|Measure-Object DirectoryBytes -Sum).Sum -or
   $first.ConfiguredStockArchiveSetComplete-ne ($first.MissingConfiguredArchives.Count-eq 0) -or
   $first.FreshStockManifestPayloadsValidated -or $first.FreshStockSidecarPayloadsValidated -or
   $first.AuthenticEp1ProcessingHashRecovered -or $first.ProductionBuildReady -or $first.ModPackageLoaded -or $first.TargetExecuted){throw 'Stock package preflight evidence/refusal contract differs.'}
# Reborn: require unique core stock manifest directory names without reading their payloads or resolving duplicate map entries.
foreach($name in @('data/static.manifest','data/static_l.manifest','data/static_m.manifest','data/global.manifest','data/worldbuilder.manifest')){
    if(@($first.ManifestEntries|Where-Object Name -CEQ $name).Count-ne 1){throw 'Expected stock core manifest directory entry differs.'}
}
# Reborn: compare every complete nested field; archive names, offsets, sizes, duplicate-name evidence and index hashes must remain stable.
foreach($property in $first.PSObject.Properties){
    if((ConvertTo-Json -InputObject $property.Value -Depth 8 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 8 -Compress)){throw "Repeat stock preflight field differs: $($property.Name)"}
}
Write-Output "EP1 1.1 stock package preflight: PASS; $($first.ArchiveCount)/$($first.ConfiguredArchiveCount) configured archive directories, $($first.TotalArchiveEntries) entries, $($first.ManifestEntryCount) manifest directory entries, stable repeat JSON and zero archive payload bytes. Missing configured archives: $($first.MissingConfiguredArchives.Count). Stock completeness and payload/game validation remain separate gates."
