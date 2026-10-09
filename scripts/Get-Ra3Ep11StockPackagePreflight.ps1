# Reborn: preflight explicit stock SKU paths and BIG manifest directory entries without reading archive payloads, launching the game or admitting production compiler identity.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[Parameter(Mandatory=$true)] [string] $SkuDefinitionPath,[switch] $AsJson)
$ErrorActionPreference='Stop'
$stockAsJson=$AsJson
$variants=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11ReaderVariants.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath
$skuBytes=Read-RuntimeInput $SkuDefinitionPath 65536
if($skuBytes.Length-eq 0 -or [Text.Encoding]::ASCII.GetString($skuBytes)-match '[^\x00-\x7F]' -or $skuBytes.Contains([byte]0)){throw 'Stock SKU diagnostic requires bounded ASCII text without nulls.'}
foreach($value in $skuBytes){if($value-gt 127){throw 'Stock SKU is not ASCII.'}}
$skuHash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($skuBytes))
$skuDirectory=[IO.Path]::GetDirectoryName([IO.Path]::GetFullPath($SkuDefinitionPath))
$archives=[Collections.Generic.List[string]]::new();$executable=$null;$searchPath=$null
# Reborn: this stock inventory accepts only known launcher/config directives; it is not a general native config interpreter.
foreach($line in ([Text.Encoding]::ASCII.GetString($skuBytes)-split '\r\n|\n|\r')){
    if($line-eq ''){continue}
    if($line.StartsWith('set-exe ',[StringComparison]::Ordinal)){
        if($null-ne $executable){throw 'Duplicate stock executable directive.'};$executable=$line.Substring(8)
    }elseif($line.StartsWith('set-search-path ',[StringComparison]::Ordinal)){
        if($null-ne $searchPath){throw 'Duplicate stock search-path directive.'};$searchPath=$line.Substring(16)
    }elseif($line.StartsWith('add-big ',[StringComparison]::Ordinal)){
        if($archives.Count-ge 32){throw 'Stock archive-count diagnostic bound exceeded.'};$archives.Add($line.Substring(8))
    }else{throw 'Stock SKU directive outside this preflight profile.'}
}
if($executable-cne 'Data\ra3ep1_1.1.game' -or $searchPath-cne 'big:;.' -or $archives.Count-eq 0){throw 'SKU is not the reviewed 1.1 stock launcher/search profile.'}
$paths=[Collections.Generic.List[string]]::new();$missing=[Collections.Generic.List[string]]::new();$seen=[Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
foreach($relative in @($executable)+$archives.ToArray()){
    if([string]::IsNullOrEmpty($relative) -or $relative.Length-gt 255 -or [IO.Path]::IsPathRooted($relative) -or $relative.Contains(':') -or ($relative-split '[\\/]')-contains '..'){throw 'Stock SKU relative path outside preflight policy.'}
    $resolved=[IO.Path]::GetFullPath($relative,$skuDirectory)
    if(-not $resolved.StartsWith($skuDirectory.TrimEnd('\')+'\',[StringComparison]::OrdinalIgnoreCase)){throw 'Stock SKU path escapes directory.'}
    if(-not $seen.Add($resolved)){throw 'Duplicate stock SKU target.'}
    if($relative-ceq $executable){if(-not $resolved.Equals([IO.Path]::GetFullPath($ImagePath),[StringComparison]::OrdinalIgnoreCase)){throw 'SKU executable differs from audited image.'}}
    else{
        # Reborn: report missing configured archives without inventing renamed-file aliases or borrowing files from another installation.
        if([IO.File]::Exists($resolved)){$paths.Add($resolved)}else{$missing.Add($resolved)}
    }
}
if($paths.Count-eq 0){throw 'No configured archive is available for bounded directory inspection.'}
$archiveReports=@(. (Join-Path $PSScriptRoot 'Get-Ra3Ep1ArchiveSourceEvidence.ps1') -ArchivePaths $paths.ToArray())
$manifestEntries=[Collections.Generic.List[object]]::new();$metadataRead=[long]0
foreach($archive in $archiveReports){
    $snapshot=Read-ArchiveDirectory $archive.Path
    if($snapshot.Length-ne $archive.ArchiveBytes -or [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($snapshot.Bytes))-cne $archive.DirectorySha256){throw 'Stock BIG index changed after inventory.'}
    $metadataRead+=$archive.MetadataBytesRead+$snapshot.Bytes.Length;$position=16
    # Reborn: enumerate manifest names and locations from the already-validated directory only; no manifest or sidecar payload is read.
    for($index=0;$index-lt $snapshot.Count;$index++){
        $offset=Read-ArchiveU32 $snapshot.Bytes $position;$size=Read-ArchiveU32 $snapshot.Bytes ($position+4);$position+=8;$start=$position
        while($snapshot.Bytes[$position]-ne 0){$position++}
        $name=[Text.Encoding]::ASCII.GetString($snapshot.Bytes,$start,$position-$start).Replace('\','/');$position++
        if($name.EndsWith('.manifest',[StringComparison]::OrdinalIgnoreCase)){
            if($manifestEntries.Count-ge 10000){throw 'Stock manifest-entry diagnostic bound exceeded.'}
            $manifestEntries.Add([pscustomobject]@{Archive=[IO.Path]::GetFileName($archive.Path);Name=$name;Offset=$offset;StoredBytes=$size})
        }
    }
}
$skuAfter=Read-RuntimeInput $SkuDefinitionPath 65536
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($skuAfter))-cne $skuHash){throw 'Stock SKU changed during preflight.'}
$AsJson=$stockAsJson
$report=[pscustomobject]@{
    ImageSha256=$variants.ImageSha256;SkuDefinitionPath=[IO.Path]::GetFullPath($SkuDefinitionPath);SkuDefinitionSha256=$skuHash;SkuBytes=$skuBytes.Length
    Executable=$executable;SearchPath=$searchPath;ConfiguredArchiveCount=$archives.Count;ArchiveCount=$archiveReports.Count;MissingConfiguredArchives=$missing.ToArray();TotalArchiveEntries=[long](($archiveReports|Measure-Object EntryCount -Sum).Sum)
    ArchiveDirectoryMetadataBytesRead=$metadataRead;SkuMetadataBytesRead=2*$skuBytes.Length;ArchivePayloadBytesRead=0;ManifestEntryCount=$manifestEntries.Count;ManifestEntries=$manifestEntries.ToArray()
    Archives=@($archiveReports|ForEach-Object{[pscustomobject]@{Path=$_.Path;ArchiveBytes=$_.ArchiveBytes;EntryCount=$_.EntryCount;DirectoryBytes=$_.DirectoryBytes;DirectorySha256=$_.DirectorySha256;DuplicateNormalizedNames=$_.DuplicateNormalizedNames}})
    StockSkuExecutableMatchesAuditedImage=$true;AvailableStockArchiveDirectoryBoundsValidated=$true;ConfiguredStockArchiveSetComplete=($missing.Count-eq 0);ReadOnly=$true;TargetExecuted=$false;ModPackageLoaded=$false
    FreshStockManifestPayloadsValidated=$false;FreshStockSidecarPayloadsValidated=$false;AuthenticEp1ProcessingHashRecovered=$false;ProductionBuildReady=$false
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 8}else{$report}
