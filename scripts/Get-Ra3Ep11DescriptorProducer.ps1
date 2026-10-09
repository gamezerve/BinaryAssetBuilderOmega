# Reborn: pin the 1.1 concrete reader and descriptor producer without executing native I/O or admitting reused-capacity semantics.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$ep11ProducerSelfTest=$SelfTest;$ep11ProducerAsJson=$AsJson
$ep11ProducerTrace=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11EntryPointerTrace.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath
$SelfTest=$ep11ProducerSelfTest;$AsJson=$ep11ProducerAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: bind the independently disassembled producer, concrete vtable, constructor write, physical reads and caller to exact 1.1 bytes. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11DescriptorCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0x7f8ea0;Length=28;Hash='42BA6936FD82F8B27014E50F59428A9F5A436796B40F76E3FDBC759120105DB8'},
        [pscustomobject]@{Offset=0xaac22;Length=6;Hash='9F0BCDF9084B1EA1E0664C4DF35F17F534A0DCA03F4C3DCE9EC0468905032687'},
        [pscustomobject]@{Offset=0xaa784;Length=148;Hash='FCF6CABFBB420B20ABB6664567EE3833E31979B305CBAC7FB18D1100835FF084'},
        [pscustomobject]@{Offset=0x49c30;Length=86;Hash='FAD82C53417A95CC7799D26D460D1AA0C75F62802F6DC6B510D620AB51089D7B'},
        [pscustomobject]@{Offset=0x49750;Length=187;Hash='73A26E28690D1BEF007C731CD71F3EE2F5C3C70026C2F0EB8728C081884B0678'},
        [pscustomobject]@{Offset=0x17550;Length=59;Hash='DA8EADC5C5A1E7D529FC357D979062260D7D91C34FCF7BE15C60A2817C4D2684'},
        [pscustomobject]@{Offset=0xce939;Length=20;Hash='52E3D63D77422403FA5C77E10FA139ECC40FF5CEE35BEDF43FC993C6817146D9'}
    )) {
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Descriptor slice outside 1.1 image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed EP1 1.1 descriptor code changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: project count-equals-capacity raw v7 descriptors with wide arithmetic; stricter bounds are audit policy, not native engine behavior. #>
#-------------------------------------------------------------------------------------------------
function Read-Ep11FreshProjection([byte[]] $Bytes,[uint32] $ExpectedCount) {
    if($Bytes.Length-lt 52 -or [BitConverter]::ToUInt32($Bytes,0)-ne 0 -or [BitConverter]::ToUInt16($Bytes,4)-ne 7 -or $Bytes[6]-ne 0 -or $Bytes[7]-gt 1){throw 'Expected prefixed little-endian v7 header.'}
    $count=[BitConverter]::ToUInt32($Bytes,16)
    if($count-ne $ExpectedCount -or $count-gt 100000 -or 52+[uint64]$count*48-gt $Bytes.Length){throw 'Entry table outside projection bounds.'}
    [uint64]$bin=0;[uint64]$relo=0;[uint64]$imp=0
    $rows=[Text.StringBuilder]::new();$last=$null
    for($index=0;$index-lt $count;$index++){
        $offset=52+48*$index
        $last=[pscustomobject]@{EntryOffset=$offset;BinOffset=$bin;ReloOffset=$relo;ImpOffset=$imp;RawWord44=[BitConverter]::ToUInt32($Bytes,$offset+44)}
        $null=$rows.AppendFormat([Globalization.CultureInfo]::InvariantCulture,"{0}`t{1}`t{2}`t{3}`t{4}`n",$offset,$bin,$relo,$imp,$last.RawWord44)
        $bin+=[BitConverter]::ToUInt32($Bytes,$offset+32);$relo+=[BitConverter]::ToUInt32($Bytes,$offset+36);$imp+=[BitConverter]::ToUInt32($Bytes,$offset+40)
        if($bin-gt [uint32]::MaxValue -or $relo-gt [uint32]::MaxValue -or $imp-gt [uint32]::MaxValue){throw 'Native unsigned wrap is not admitted by this audit.'}
    }
    if($bin-ne [BitConverter]::ToUInt32($Bytes,20)){throw 'Instance-size total differs from header.'}
    return [pscustomobject]@{AssetCount=$count;Linked=($Bytes[7]-eq 1);BinBytes=$bin;ReloBytes=$relo;ImpBytes=$imp;LastDescriptor=$last;ProjectionSha256=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($rows.ToString())))}
}
Assert-Ep11DescriptorCode $ep11Bytes
# Reborn: reread already captured stock manifests; these are not newly extracted 1.1 BIG files and no adjacent payload is read.
$ep11Projections=@(foreach($inputManifest in $evidence.Inputs){
    $raw=Read-RuntimeInput $inputManifest.Path 16777216
    if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($raw))-cne $inputManifest.Sha256){throw 'Stock manifest SHA differs.'}
    [pscustomobject]@{Path=$inputManifest.Path;ManifestSha256=$inputManifest.Sha256;Projection=(Read-Ep11FreshProjection $raw $inputManifest.AssetCount)}
})
if($SelfTest){
    # Reborn: detached entries preserve opaque word44 and zero chunks, while malformed headers/counts/totals and overflow must fail.
    $fixture=[byte[]]::new(196);$fixture[4]=7
    [BitConverter]::GetBytes([uint32]3).CopyTo($fixture,16);[BitConverter]::GetBytes([uint32]12).CopyTo($fixture,20)
    [BitConverter]::GetBytes([uint32]4).CopyTo($fixture,84);[BitConverter]::GetBytes([uint32]8).CopyTo($fixture,180)
    [BitConverter]::GetBytes([uint32]4).CopyTo($fixture,88);[BitConverter]::GetBytes([uint32]8).CopyTo($fixture,136)
    [BitConverter]::GetBytes([uint32]7).CopyTo($fixture,192)
    $projected=Read-Ep11FreshProjection $fixture 3
    if($projected.LastDescriptor.EntryOffset-ne 148 -or $projected.LastDescriptor.BinOffset-ne 4 -or $projected.LastDescriptor.ReloOffset-ne 12 -or $projected.LastDescriptor.RawWord44-ne 7){throw 'Fresh descriptor fixture differs.'}
    foreach($kind in @('version','count','truncation','overflow','total')){
        $fault=[byte[]]$fixture.Clone()
        switch($kind){
            'version'{$fault[4]=6}
            'count'{$fault[16]=4}
            'truncation'{$fault=[byte[]]$fault[0..190]}
            'overflow'{[BitConverter]::GetBytes([uint32]::MaxValue).CopyTo($fault,84)}
            'total'{$fault[20]=13}
        }
        $rejected=$false;try{$null=Read-Ep11FreshProjection $fault 3}catch{$rejected=$true}
        if(-not $rejected){throw 'Malformed descriptor fixture admitted.'}
    }
    # Reborn: fault the concrete slot, constructor, header size, entry stride, capacity loop, pointer write, allocator and caller in private arrays only.
    foreach($offset in @(0x7f8eb8,0xaac24,0xaa7ab,0x49c63,0x497be,0x497d9,0x1756d,0xce940)){
        $fault=[byte[]]$ep11Bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11DescriptorCode $fault}catch{$rejected=$true}
        if(-not $rejected){throw 'Descriptor code fault admitted.'}
    }
}
$ep11ProducerAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($ep11ProducerAfter))-cne $ep11ProducerTrace.ImageSha256){throw '1.1 image changed during producer review.'}
$ep11ProducerReport=[pscustomobject]@{
    ImageSha256=$ep11ProducerTrace.ImageSha256;ReaderVtableVa='0x00BF8EA0';ConstructorVtableWriteVa='0x004AAC22';HeaderMethodVa='0x004AA670'
    EntryReadMethodVa='0x00449C30';DescriptorProducerVa='0x00449750';DescriptorAllocatorVa='0x00417550';ProducerCallVa='0x004CE940'
    PhysicalHeaderBytes=52;RawEntryStride=48;DescriptorStride=20;DescriptorEntryPointerOffset=12;DescriptorSourcePointerOffset=16
    DescriptorBinOffset=0;DescriptorReloOffset=4;DescriptorImpOffset=8;EntryBinSizeOffset=32;EntryReloSizeOffset=36;EntryImpSizeOffset=40
    DescriptorProducerRecovered=$true;ConcreteReaderPhysicalEntryReadRecovered=$true;DescriptorToGatePointerPathRecovered=$true
    FreshCountEqualsCapacityProjectionOnly=$true;ProducerNativeLoopUsesCapacity=$true;ReusedCapacitySemanticsRecovered=$false
    PostReadHelperVa='0x00449810';PostReadHelperSemanticsRecovered=$false;CompleteStreamPointerProvenanceRecovered=$false
    AllReaderImplementationsRecovered=$false;AllGateCallerPathsRecovered=$false;Word44SchemaMeaningRecovered=$false
    ObservedManifestCount=$ep11Projections.Count;ProjectedRawEntryCount=($ep11Projections.Projection.AssetCount|Measure-Object -Sum).Sum;Manifests=$ep11Projections
    FreshEp11ArchiveExtractionPerformed=$false;NativePayloadBytesRead=0;ReadOnly=$true;TargetExecuted=$false;ManagedInspectorHashCommandExecuted=$false
    RuntimeConsumerIsAuthoringProcessingHash=$false;ProductionBuildReady=$false;ModPackageLoaded=$false;FaultTestsPassed=[bool]$SelfTest
    RawFixtureRejections=$(if($SelfTest){5}else{0});MemoryFaultsExecuted=$(if($SelfTest){8}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $ep11ProducerReport -Depth 9}else{$ep11ProducerReport}
