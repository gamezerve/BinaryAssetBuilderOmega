# Reborn: bind reviewed reader/descriptor code to pinned raw v7 manifest layouts without executing native I/O or claiming linked-entry transformations.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$producerSelfTest=$SelfTest; $producerAsJson=$AsJson
$trace=. (Join-Path $PSScriptRoot 'Get-Ra3Ep1EntryPointerTrace.ps1') -ImagePath $ImagePath
$SelfTest=$producerSelfTest; $AsJson=$producerAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: exact snapshots bind the concrete reader vtable, construction write, physical header/entry reads, descriptor producer and allocator. #>
#-------------------------------------------------------------------------------------------------
function Assert-DescriptorProducerCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0x7f2e78;Length=28;Hash='03F6FC4DB3D4D907CA8D003A92708A68505B84A2C35B8E985B5DB827F1B88AD1'},
        [pscustomobject]@{Offset=0xaa962;Length=6;Hash='DE7CC2D0B7061436CF605F76164FC003D1DE092584CA07A3856CE42197888E49'},
        [pscustomobject]@{Offset=0xaa4c4;Length=148;Hash='4319BB7821C2C32C7080B235D120D45B7EFAA1C7E6657CCCCFE556C0D72EF5CE'},
        [pscustomobject]@{Offset=0x49b80;Length=86;Hash='FAD82C53417A95CC7799D26D460D1AA0C75F62802F6DC6B510D620AB51089D7B'},
        [pscustomobject]@{Offset=0x496a0;Length=187;Hash='CD2354457736A189856ECE49A2C2A148632F9B5596175602DF4D72AF76116224'},
        [pscustomobject]@{Offset=0x17520;Length=59;Hash='DA8EADC5C5A1E7D529FC357D979062260D7D91C34FCF7BE15C60A2817C4D2684'},
        [pscustomobject]@{Offset=0xce679;Length=20;Hash='4C33E614BD43184C31838A68EFE9FD076147EC3BFB8B94C2206B9CE153D6C193'}
    )) {
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Descriptor code outside image.'}
        $digest=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))
        if($digest-cne $slice.Hash){throw 'Reviewed descriptor producer changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: project fresh count-equals-capacity raw descriptors with wide bounded arithmetic; this is not execution of reused-capacity or linked native processing. #>
#-------------------------------------------------------------------------------------------------
function Read-FreshDescriptorProjection([byte[]] $Bytes,[uint32] $ExpectedCount) {
    if($Bytes.Length-lt 52 -or [BitConverter]::ToUInt32($Bytes,0)-ne 0 -or [BitConverter]::ToUInt16($Bytes,4)-ne 7 -or $Bytes[6]-ne 0 -or $Bytes[7]-gt 1){throw 'Projection requires a prefixed little-endian v7 raw header.'}
    $count=[BitConverter]::ToUInt32($Bytes,16)
    if($count-ne $ExpectedCount -or $count-gt 100000 -or 52+[uint64]$count*48-gt $Bytes.Length){throw 'Raw record table outside projection bounds.'}
    [uint64]$bin=0;[uint64]$relo=0;[uint64]$imp=0
    $projection=[Text.StringBuilder]::new();$last=$null
    for($index=0;$index-lt $count;$index++){
        $offset=52+48*$index
        $last=[pscustomobject]@{EntryOffset=$offset;BinOffset=$bin;ReloOffset=$relo;ImpOffset=$imp;RawWord44=[BitConverter]::ToUInt32($Bytes,$offset+44)}
        $null=$projection.AppendFormat([Globalization.CultureInfo]::InvariantCulture,"{0}`t{1}`t{2}`t{3}`t{4}`n",$offset,$bin,$relo,$imp,$last.RawWord44)
        $bin+=[BitConverter]::ToUInt32($Bytes,$offset+32)
        $relo+=[BitConverter]::ToUInt32($Bytes,$offset+36)
        $imp+=[BitConverter]::ToUInt32($Bytes,$offset+40)
        if($bin-gt [uint32]::MaxValue -or $relo-gt [uint32]::MaxValue -or $imp-gt [uint32]::MaxValue){throw 'Projection exceeds unsigned 32-bit range; native wrap semantics are not admitted.'}
    }
    if($bin-ne [BitConverter]::ToUInt32($Bytes,20)){throw 'Raw instance-size total differs from header.'}
    return [pscustomobject]@{AssetCount=$count;Linked=($Bytes[7]-eq 1);BinBytes=$bin;ReloBytes=$relo;ImpBytes=$imp;LastDescriptor=$last;ProjectionSha256=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($projection.ToString())))}
}
Assert-DescriptorProducerCode $bytes
# Reborn: imported stock evidence already pins four manifests; re-read/re-pin them for this raw-layout projection, with zero native payload reads.
$projections=@(foreach($manifestInput in $evidence.Inputs){
    $rawManifest=Read-RuntimeInput $manifestInput.Path 16777216
    if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($rawManifest))-cne $manifestInput.Sha256){throw 'Producer stock manifest pin differs.'}
    $projection=Read-FreshDescriptorProjection $rawManifest $manifestInput.AssetCount
    [pscustomobject]@{Path=$manifestInput.Path;ManifestSha256=$manifestInput.Sha256;Projection=$projection}
})
if($SelfTest){
    # Reborn: detached raw records test fresh projection offsets, zero chunks and opaque word44 preservation without native allocation or filesystem mutation.
    $fixture=[byte[]]::new(52+3*48);$fixture[4]=7
    [BitConverter]::GetBytes([uint32]3).CopyTo($fixture,16);[BitConverter]::GetBytes([uint32]12).CopyTo($fixture,20)
    [BitConverter]::GetBytes([uint32]4).CopyTo($fixture,84);[BitConverter]::GetBytes([uint32]8).CopyTo($fixture,180)
    [BitConverter]::GetBytes([uint32]4).CopyTo($fixture,88);[BitConverter]::GetBytes([uint32]8).CopyTo($fixture,136)
    [BitConverter]::GetBytes([uint32]7).CopyTo($fixture,192)
    $fixtureProjection=Read-FreshDescriptorProjection $fixture 3
    if($fixtureProjection.LastDescriptor.EntryOffset-ne 148 -or $fixtureProjection.LastDescriptor.BinOffset-ne 4 -or $fixtureProjection.LastDescriptor.ReloOffset-ne 12 -or $fixtureProjection.LastDescriptor.RawWord44-ne 7){throw 'Fresh projection fixture differs.'}
    foreach($faultKind in @('version','count','truncation','overflow','total')){
        $fault=[byte[]]$fixture.Clone()
        switch($faultKind){
            'version'{$fault[4]=6}
            'count'{[BitConverter]::GetBytes([uint32]4).CopyTo($fault,16)}
            'truncation'{$fault=[byte[]]$fault[0..190]}
            'overflow'{[BitConverter]::GetBytes([uint32]::MaxValue).CopyTo($fault,84)}
            'total'{$fault[20]=13}
        }
        $rejected=$false;try{$null=Read-FreshDescriptorProjection $fault 3}catch{$rejected=$true}
        if(-not $rejected){throw 'Invalid raw projection fixture admitted.'}
    }
    # Reborn: code/vtable/private instruction faults must fail exact evidence pins instead of selecting an unreviewed reader.
    foreach($offset in @(0x7f2e84,0xaa964,0xaa4ea,0x49b9f,0x49727,0x1753c,0xce680)){
        $fault=[byte[]]$bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-DescriptorProducerCode $fault}catch{$rejected=$true}
        if(-not $rejected){throw 'Descriptor code fault admitted.'}
    }
}
$producerAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($producerAfter))-cne $trace.ImageSha256){throw 'Producer image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$trace.ImageSha256;ReaderVtableVa='0x00BF2E78';HeaderMethodVa='0x004AA3B0';EntryReadMethodVa='0x00449B80';DescriptorProducerVa='0x004496A0';DescriptorAllocatorVa='0x00417520'
    PhysicalHeaderBytes=52;RawEntryStride=48;DescriptorStride=20;DescriptorEntryPointerOffset=12;DescriptorProducerRecovered=$true;ConcreteReaderLayoutRecovered=$true
    ObservedManifestCount=$projections.Count;ProjectedRawEntryCount=($projections.Projection.AssetCount|Measure-Object -Sum).Sum;Manifests=$projections
    FreshCountEqualsCapacityProjectionOnly=$true;ProducerNativeLoopUsesCapacity=$true;ReusedCapacitySemanticsRecovered=$false;LinkedEntryTransformationRecovered=$false
    CompleteStreamPointerProvenanceRecovered=$false;AllReaderImplementationsRecovered=$false;Word44SchemaMeaningRecovered=$false;NativePayloadBytesRead=0
    ReadOnly=$true;TargetExecuted=$false;ManagedInspectorHashCommandExecuted=$trace.ManagedInspectorHashCommandExecuted;ProductionBuildReady=$false
    FaultTestsPassed=[bool]$SelfTest;RawFixtureRejections=$(if($SelfTest){5}else{0});MemoryFaultsExecuted=$(if($SelfTest){7}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 9}else{$report}
