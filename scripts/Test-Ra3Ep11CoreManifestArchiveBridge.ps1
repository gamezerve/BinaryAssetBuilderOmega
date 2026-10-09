# Reborn: verify four fresh, bounded stock manifest selections match captured evidence without reading neighbouring instance streams or claiming game loading.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[Parameter(Mandatory=$true)] [string] $SkuDefinitionPath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11CoreManifestArchiveBridge.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.ReadOnly -or -not $first.ManagedDecoderExecuted -or
   -not $first.FreshSelectedManifestArchivePayloadsValidated -or -not $first.AllExpandedManifestsMatchCaptured -or
   $first.SelectedManifestCount-ne 4 -or $first.ProjectedEntryCount-ne 55519 -or $first.MalformedRefPackFixturesExecuted-ne 3 -or
   $first.SelectedArchiveManifestBytesRead-ne 2*($first.Manifests|Measure-Object StoredBytes -Sum).Sum -or
   $first.AdjacentBinReloImpPayloadBytesRead-ne 0 -or $first.MaximumStoredManifestBytes-ne 4194304 -or $first.MaximumExpandedManifestBytes-ne 33554432 -or
   $first.CompleteStockArchivePayloadsValidated -or $first.AuthenticEp1ProcessingHashRecovered -or $first.ProductionBuildReady -or
   $first.ModPackageLoaded -or $first.TargetExecuted -or $first.ManagedInspectorHashCommandExecuted){throw 'Core manifest archive bridge evidence/refusal contract differs.'}
# Reborn: require per-row whole expanded SHA identity, observed compression counts and scoped projection/link flags.
if(@($first.Manifests|Where-Object RefPackCompressed).Count-ne 3){throw 'Selected stock manifest compression evidence differs.'}
foreach($row in $first.Manifests){if(-not $row.ExpandedMatchesCaptured -or $row.ExpandedSha256-cne $row.CapturedSha256 -or -not $row.Projection.Linked){throw 'Selected archive/captured manifest bridge differs.'}}
# Reborn: compare every nested stable field while excluding intentional detached RefPack self-test counters.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','MalformedRefPackFixturesExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 9 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 9 -Compress)){throw "Repeat core archive bridge field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 core manifest archive bridge: PASS; four fresh unique archive entries, three managed RefPack expansions, full expanded SHA equality to captured manifests, 55519 projected entries, three malformed detached decoder fixtures and repeat JSON. Zero adjacent BIN/RELO/IMP bytes; authentic compiler hash and game loading remain open.'
