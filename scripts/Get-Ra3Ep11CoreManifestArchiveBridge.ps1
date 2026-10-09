# Reborn: compare four uniquely indexed stock manifest payloads with pinned unpacked evidence using bounded managed RefPack only; never read adjacent instance streams or run the game.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[Parameter(Mandatory=$true)] [string] $SkuDefinitionPath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$bridgeSelfTest=$SelfTest;$bridgeAsJson=$AsJson
$stock=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11StockPackagePreflight.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath
$SelfTest=$bridgeSelfTest;$AsJson=$bridgeAsJson
$refPackPath=Join-Path (Split-Path -Parent $PSScriptRoot) 'source/BinaryAssetBuilder.ManifestInspector/RefPack.cs'
$refPackBytes=Read-RuntimeInput $refPackPath 65536
$refPackHash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($refPackBytes))
if($refPackHash-cne '2CFA7E92E6C56BA8319F9110FFAD0AA2C61C3345A65DD40EE24A24433695A5E4'){throw 'Managed RefPack source changed; re-review bounded decoding before admission.'}
# Reborn: compile only the existing managed decoder and a byte-array adapter; no EA/native DLL or codec is loaded.
if(-not ('BinaryAssetBuilder.ManifestInspector.Ep11ArchiveBridgeDecoder'-as [type])){
    $adapter=@'
public static class Ep11ArchiveBridgeDecoder
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: expose the reviewed bounded managed decoder to detached PowerShell byte arrays only. */
    //-------------------------------------------------------------------------------------------------
    public static byte[] Decode(byte[] input, int maximumOutputSize) => RefPack.Decompress(input, maximumOutputSize);
}
'@
    # Reborn: standalone compilation supplies the same System/IO namespaces normally supplied by the inspector project's implicit usings.
    Add-Type -TypeDefinition ("using System; using System.IO;`n"+[Text.Encoding]::UTF8.GetString($refPackBytes)+$adapter)
}

#-------------------------------------------------------------------------------------------------
<# Reborn: read only one reviewed manifest entry under a strict stored-size bound after revalidating archive directory identity; no neighbouring payload is read. #>
#-------------------------------------------------------------------------------------------------
function Read-Ep11SelectedManifestEntry([object] $Archive,[object] $Entry) {
    $snapshot=Read-ArchiveDirectory $Archive.Path
    if($snapshot.Length-ne $Archive.ArchiveBytes -or [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($snapshot.Bytes))-cne $Archive.DirectorySha256){throw 'Selected manifest archive directory changed.'}
    if($Entry.StoredBytes-le 0 -or $Entry.StoredBytes-gt 4194304 -or $Entry.Offset-lt $snapshot.Bytes.Length -or [uint64]$Entry.Offset+[uint64]$Entry.StoredBytes-gt $snapshot.Length){throw 'Selected manifest range outside reviewed diagnostic bounds.'}
    $resolved=Assert-ArchivePath $Archive.Path
    $stream=[IO.File]::Open($resolved,[IO.FileMode]::Open,[IO.FileAccess]::Read,[IO.FileShare]::Read)
    try{
        if($stream.Length-ne $snapshot.Length){throw 'Selected manifest archive length changed.'}
        $stream.Position=$Entry.Offset;$selected=[byte[]]::new([int]$Entry.StoredBytes);$stream.ReadExactly($selected)
        return ,$selected
    }finally{$stream.Dispose()}
}
$selections=@(
    [pscustomobject]@{Archive='GlobalStream.big';Name='data/global.manifest';EvidenceIndex=0},
    [pscustomobject]@{Archive='StaticStream.big';Name='data/static.manifest';EvidenceIndex=1},
    [pscustomobject]@{Archive='WBData.big';Name='data/worldbuilder.manifest';EvidenceIndex=2},
    [pscustomobject]@{Archive='EnglishAudio.big';Name='data/audio.manifest';EvidenceIndex=3}
)
$rows=[Collections.Generic.List[object]]::new();$selectedStoredRead=[long]0;$selectedDirectoryRead=[long]0
foreach($selection in $selections){
    $entries=@($stock.ManifestEntries|Where-Object{ $_.Archive-ceq $selection.Archive -and $_.Name-ceq $selection.Name })
    $archives=@($stock.Archives|Where-Object{[IO.Path]::GetFileName($_.Path)-ceq $selection.Archive})
    if($entries.Count-ne 1 -or $archives.Count-ne 1){throw 'Selected manifest archive/name is missing or ambiguous.'}
    $entry=$entries[0];$archive=$archives[0];$first=Read-Ep11SelectedManifestEntry $archive $entry;$second=Read-Ep11SelectedManifestEntry $archive $entry
    $firstHash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($first))
    if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($second))-cne $firstHash){throw 'Selected stored manifest changed during review.'}
    $selectedStoredRead+=2*$entry.StoredBytes;$selectedDirectoryRead+=2*$archive.DirectoryBytes
    $compressed=($first.Length-ge 2 -and $first[1]-eq 0xfb -and ($first[0]-band 0x1f)-eq 0x10)
    [byte[]]$expanded=if($compressed){ ,[BinaryAssetBuilder.ManifestInspector.Ep11ArchiveBridgeDecoder]::Decode($first,33554432) }else{ ,$first }
    $expandedHash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$expanded))
    $captured=$evidence.Inputs[$selection.EvidenceIndex]
    $projection=Read-Ep11FreshProjection ([byte[]]$expanded) $captured.AssetCount
    if([BitConverter]::ToUInt32([byte[]]$expanded,12)-ne 0x5454a8e9){throw 'Fresh selected manifest all-types identity differs.'}
    $rows.Add([pscustomobject]@{Archive=$selection.Archive;Name=$selection.Name;Offset=$entry.Offset;StoredBytes=$entry.StoredBytes;StoredSha256=$firstHash;RefPackCompressed=$compressed;ExpandedBytes=$expanded.Length;ExpandedSha256=$expandedHash;CapturedPath=$captured.Path;CapturedSha256=$captured.Sha256;ExpandedMatchesCaptured=($expandedHash-ceq $captured.Sha256);Projection=$projection})
}
if($SelfTest){
    # Reborn: malformed detached RefPack examples must reject truncation, output mismatch and a declared output exceeding the bridge cap.
    foreach($fault in @([byte[]]@(0x10,0xfb,0,0,1,0xfc),[byte[]]@(0x10,0xfb,0,0,16,0xff),[byte[]]@(0x90,0xfb,0xff,0xff,0xff,0xff,0xfc))){
        $rejected=$false;try{$null=[BinaryAssetBuilder.ManifestInspector.Ep11ArchiveBridgeDecoder]::Decode($fault,33554432)}catch{$rejected=$true};if(-not $rejected){throw 'Malformed detached RefPack fixture admitted.'}
    }
}
$report=[pscustomobject]@{
    ImageSha256=$stock.ImageSha256;SkuDefinitionSha256=$stock.SkuDefinitionSha256;ConfiguredStockArchiveSetComplete=$stock.ConfiguredStockArchiveSetComplete;MissingConfiguredArchives=$stock.MissingConfiguredArchives
    SelectedManifestCount=$rows.Count;ProjectedEntryCount=[long](($rows.Projection.AssetCount|Measure-Object -Sum).Sum);Manifests=$rows.ToArray();AllExpandedManifestsMatchCaptured=(@($rows|Where-Object{-not $_.ExpandedMatchesCaptured}).Count-eq 0)
    SelectedArchiveManifestBytesRead=$selectedStoredRead;ArchiveDirectoryMetadataBytesRead=$stock.ArchiveDirectoryMetadataBytesRead+$selectedDirectoryRead;AdjacentBinReloImpPayloadBytesRead=0
    MaximumStoredManifestBytes=4194304;MaximumExpandedManifestBytes=33554432;ManagedRefPackSourceSha256=$refPackHash;ManagedDecoderExecuted=$true
    FreshSelectedManifestArchivePayloadsValidated=$true;CompleteStockArchivePayloadsValidated=$false;AuthenticEp1ProcessingHashRecovered=$false;ProductionBuildReady=$false;ModPackageLoaded=$false
    ReadOnly=$true;TargetExecuted=$false;ManagedInspectorHashCommandExecuted=$false;FaultTestsPassed=[bool]$SelfTest;MalformedRefPackFixturesExecuted=$(if($SelfTest){3}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 9}else{$report}
