# Reborn: compare twelve core archive sidecar logical headers and declared lengths using only raw eight-byte headers or narrow literal RefPack prefixes, never full instance streams.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[Parameter(Mandatory=$true)] [string] $SkuDefinitionPath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$sidecarSelfTest=$SelfTest;$sidecarAsJson=$AsJson
$bridge=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11CoreManifestArchiveBridge.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath
$SelfTest=$sidecarSelfTest;$AsJson=$sidecarAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: decode only the first eight logical bytes of the observed RefPack literal-prefix subset; reject other forms instead of expanding a large stream. #>
#-------------------------------------------------------------------------------------------------
function Read-Ep11LogicalSidecarPrefix([IO.Stream] $Stream,[long] $Offset,[long] $StoredSize) {
    if($Offset-lt 0 -or $StoredSize-lt 8 -or $Offset-gt $Stream.Length-$StoredSize){throw 'Sidecar prefix outside input range.'}
    $Stream.Position=$Offset;$first=[byte[]]::new(8);$Stream.ReadExactly($first)
    $compressed=($first[1]-eq 0xfb -and ($first[0]-band 0x1f)-eq 0x10)
    if(-not $compressed){return [pscustomobject]@{Header=$first;LogicalBytes=$StoredSize;Compressed=$false;PhysicalPrefixBytesRead=8}}
    if($StoredSize-lt 16 -or ($first[0]-band 1)-ne 0){throw 'Sidecar RefPack header outside reviewed narrow prefix subset.'}
    $Stream.Position=$Offset;$prefix=[byte[]]::new(16);$Stream.ReadExactly($prefix)
    $width=$(if(($prefix[0]-band 0x80)-ne 0){4}else{3});$position=2;[uint64]$logicalBytes=0
    for($index=0;$index-lt $width;$index++){$logicalBytes=($logicalBytes*256)+$prefix[$position];$position++}
    $control=$prefix[$position];$position++
    if($logicalBytes-lt 8 -or $control-lt 0xe0 -or $control-ge 0xfc){throw 'Sidecar requires an unreviewed prefix decoder form.'}
    $literalCount=(($control-band 0x1f)*4)+4
    if($literalCount-lt 8 -or $position+$literalCount-gt $StoredSize -or $position+8-gt $prefix.Length){throw 'Sidecar literal prefix does not supply the complete logical header.'}
    $header=[byte[]]$prefix[$position..($position+7)]
    return [pscustomobject]@{Header=$header;LogicalBytes=$logicalBytes;Compressed=$true;PhysicalPrefixBytesRead=24}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: locate one exact normalized directory name from an already validated index; never resolve duplicate entries by guessed archive precedence. #>
#-------------------------------------------------------------------------------------------------
function Find-Ep11SidecarEntry([byte[]] $Directory,[uint32] $Count,[string] $Name) {
    $position=16;$matches=[Collections.Generic.List[object]]::new()
    for($index=0;$index-lt $Count;$index++){
        $offset=Read-ArchiveU32 $Directory $position;$size=Read-ArchiveU32 $Directory ($position+4);$position+=8;$start=$position
        while($position-lt $Directory.Length -and $Directory[$position]-ne 0){$position++}
        if($position-ge $Directory.Length){throw 'Truncated sidecar directory name.'}
        $candidate=[Text.Encoding]::ASCII.GetString($Directory,$start,$position-$start).Replace('\','/');$position++
        if($candidate-ceq $Name){$matches.Add([pscustomobject]@{Name=$candidate;Offset=$offset;StoredSize=$size})}
    }
    if($matches.Count-ne 1){throw 'Core sidecar directory name missing or ambiguous.'}
    return $matches[0]
}
$rows=[Collections.Generic.List[object]]::new();$prefixBytesRead=[long]0;$extraDirectoryBytes=[long]0
foreach($manifest in $bridge.Manifests){
    $archive=@($stock.Archives|Where-Object{[IO.Path]::GetFileName($_.Path)-ceq $manifest.Archive})[0]
    $resolved=Assert-ArchivePath $archive.Path
    # Reborn: hold a shared-read-only handle across both prefix passes and directory snapshots so ordinary concurrent writers cannot alter the reviewed archive.
    $stream=[IO.File]::Open($resolved,[IO.FileMode]::Open,[IO.FileAccess]::Read,[IO.FileShare]::Read)
    try{
        $snapshot=Read-ArchiveDirectory $resolved
        if($stream.Length-ne $archive.ArchiveBytes -or [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($snapshot.Bytes))-cne $archive.DirectorySha256){throw 'Sidecar archive identity differs.'}
        foreach($kind in @('bin','relo','imp')){
            $name=$manifest.Name.Substring(0,$manifest.Name.Length-8)+$kind
            $entry=Find-Ep11SidecarEntry $snapshot.Bytes $snapshot.Count $name
            $first=Read-Ep11LogicalSidecarPrefix $stream $entry.Offset $entry.StoredSize;$second=Read-Ep11LogicalSidecarPrefix $stream $entry.Offset $entry.StoredSize
            $prefixBytesRead+=$first.PhysicalPrefixBytesRead+$second.PhysicalPrefixBytesRead
            if([Convert]::ToHexString($first.Header)-cne [Convert]::ToHexString($second.Header) -or $first.LogicalBytes-ne $second.LogicalBytes -or $first.Compressed-ne $second.Compressed){throw 'Core sidecar header changed during review.'}
            $magic=[BitConverter]::ToUInt32($first.Header,0);$checksum=[BitConverter]::ToUInt32($first.Header,4)
            # Reborn: parse high-bit magic words explicitly unsigned instead of relying on PowerShell's signed hexadecimal literal conversion.
            $expectedMagic=[Convert]::ToUInt32($(switch($kind){'bin'{'BABB0000'};'relo'{'BABE0000'};'imp'{'BAB10000'}}),16)
            $chunkBytes=$(switch($kind){'bin'{$manifest.Projection.BinBytes};'relo'{$manifest.Projection.ReloBytes};'imp'{$manifest.Projection.ImpBytes}})
            if($magic-ne $expectedMagic -or $checksum-ne $manifest.Checksum -or $first.LogicalBytes-ne 8+$chunkBytes){throw 'Fresh sidecar magic/checksum/logical length differs from fresh manifest projection.'}
            $rows.Add([pscustomobject]@{Archive=$manifest.Archive;Name=$name;Offset=$entry.Offset;StoredBytes=$entry.StoredSize;RefPackCompressed=$first.Compressed;LogicalBytes=$first.LogicalBytes;HeaderHex=[Convert]::ToHexString($first.Header);ManifestChecksum=$manifest.Checksum;ProjectedChunkBytes=$chunkBytes;HeaderAndLogicalLengthMatch=$true})
        }
        $after=Read-ArchiveDirectory $resolved;$extraDirectoryBytes+=2*$snapshot.Bytes.Length
        if($after.Length-ne $snapshot.Length -or [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($after.Bytes))-cne $archive.DirectorySha256){throw 'Sidecar directory changed during review.'}
    }finally{$stream.Dispose()}
}
if($SelfTest){
    # Reborn: detached small/large RefPack headers and raw eight-byte inputs must yield the same logical header without whole-body decoding.
    foreach($fixture in @([byte[]]@(0x10,0xfb,0,0,8,0xe1,0,0,0xbb,0xba,1,2,3,4,0,0),[byte[]]@(0x90,0xfb,0,0,0,8,0xe1,0,0,0xbb,0xba,1,2,3,4,0),[byte[]]@(0,0,0xbb,0xba,1,2,3,4))){
        $memory=[IO.MemoryStream]::new($fixture,$false)
        try{$positive=Read-Ep11LogicalSidecarPrefix $memory 0 $fixture.Length;if([Convert]::ToHexString($positive.Header)-cne '0000BBBA01020304' -or $positive.LogicalBytes-ne 8){throw 'Detached sidecar prefix fixture differs.'}}finally{$memory.Dispose()}
    }
    # Reborn: detached narrow-prefix faults must reject unsupported commands, undersized logical output and insufficient prefix literals.
    foreach($kind in @('command','logical','literal')){
        $fixture=[byte[]]@(0x10,0xfb,0,0,8,0xe1,0,0,0xbb,0xba,1,2,3,4,0,0)
        switch($kind){'command'{$fixture[5]=0x80};'logical'{$fixture[4]=7};'literal'{$fixture[5]=0xe0}}
        $memory=[IO.MemoryStream]::new($fixture,$false);$rejected=$false
        try{$null=Read-Ep11LogicalSidecarPrefix $memory 0 $fixture.Length}catch{$rejected=$true}finally{$memory.Dispose()}
        if(-not $rejected){throw 'Malformed detached sidecar prefix admitted.'}
    }
}
$report=[pscustomobject]@{
    ImageSha256=$bridge.ImageSha256;FreshManifestExpandedShaMatches=$bridge.AllExpandedManifestsMatchCaptured;SidecarCount=$rows.Count;Sidecars=$rows.ToArray()
    CompressedSidecarCount=@($rows|Where-Object RefPackCompressed).Count;RawSidecarCount=@($rows|Where-Object{-not $_.RefPackCompressed}).Count
    SidecarPhysicalPrefixBytesRead=$prefixBytesRead;SidecarLogicalHeaderBytesObserved=8*$rows.Count;ArchiveDirectoryMetadataBytesRead=$bridge.ArchiveDirectoryMetadataBytesRead+$extraDirectoryBytes
    SelectedManifestBytesRead=$bridge.SelectedArchiveManifestBytesRead;CompleteInstanceStreamBytesRead=0;WholeSidecarPayloadsValidated=$false;FullRefPackSidecarBodiesDecoded=$false
    ConfiguredStockArchiveSetComplete=$bridge.ConfiguredStockArchiveSetComplete;MissingConfiguredArchives=$bridge.MissingConfiguredArchives;AuthenticEp1ProcessingHashRecovered=$false
    ReadOnly=$true;TargetExecuted=$false;ModPackageLoaded=$false;ProductionBuildReady=$false;FaultTestsPassed=[bool]$SelfTest;PositivePrefixFixturesExecuted=$(if($SelfTest){3}else{0});MalformedPrefixFixturesExecuted=$(if($SelfTest){3}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 8}else{$report}
