# Reborn: require fresh twelve-sidecar header/length identity and bounded read accounting while refusing whole-stream, native game and authentic compiler claims.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[Parameter(Mandatory=$true)] [string] $SkuDefinitionPath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11ArchiveSidecarHeaders.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.ReadOnly -or -not $first.FreshManifestExpandedShaMatches -or
   $first.SidecarCount-ne 12 -or $first.CompressedSidecarCount-ne 6 -or $first.RawSidecarCount-ne 6 -or
   $first.PositivePrefixFixturesExecuted-ne 3 -or $first.MalformedPrefixFixturesExecuted-ne 3 -or
   $first.SidecarPhysicalPrefixBytesRead-ne 384 -or $first.SidecarLogicalHeaderBytesObserved-ne 96 -or $first.CompleteInstanceStreamBytesRead-ne 0 -or
   $first.WholeSidecarPayloadsValidated -or $first.FullRefPackSidecarBodiesDecoded -or $first.AuthenticEp1ProcessingHashRecovered -or
   $first.TargetExecuted -or $first.ModPackageLoaded -or $first.ProductionBuildReady){throw 'Archive sidecar header evidence/refusal contract differs.'}
foreach($row in $first.Sidecars){if(-not $row.HeaderAndLogicalLengthMatch -or $row.LogicalBytes-ne 8+$row.ProjectedChunkBytes){throw 'Fresh archive sidecar length differs.'}}
# Reborn: retain the large WorldBuilder stream as a raw bounded-header observation, never a full dump or decompression.
$world=@($first.Sidecars|Where-Object Name -CEQ 'data/worldbuilder.bin')
if($world.Count-ne 1 -or $world[0].StoredBytes-ne 1394571528 -or $world[0].RefPackCompressed -or $world[0].HeaderHex-cne '0000BBBA8002ACF2'){throw 'WorldBuilder bounded-header evidence differs.'}
# Reborn: compare all nested stable fields while excluding deliberate detached prefix fixture counters.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','PositivePrefixFixturesExecuted','MalformedPrefixFixturesExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 8 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 8 -Compress)){throw "Repeat archive sidecar field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 archive sidecar headers: PASS; twelve unique headers/lengths match fresh manifest checksums/totals, six raw/six compressed literal prefixes, three positive/three malformed detached fixtures and repeat JSON. Only 384 sidecar prefix bytes per invocation; whole streams, authentic compiler hash and game loading remain open.'
