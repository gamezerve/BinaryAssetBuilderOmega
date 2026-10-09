# Reborn: characterize linked chunk addressing and validate metadata-only physical ranges without executing native reads or admitting engine buffer policy.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$chunkSelfTest=$SelfTest;$chunkAsJson=$AsJson
$linked=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11LinkedSidecars.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath
$SelfTest=$chunkSelfTest;$AsJson=$chunkAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: pin reviewed queue/vtable/dispatcher/chunk-reader bodies and the memcpy import thunk, not an arbitrary pointer-shaped copy routine. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11LinkedChunkCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0x17210;Length=53;Hash='D30FD07ECCB15324EC3572DD1539B84A1AF2C0D131A5A8DAE3D9321D223EBB4D'},
        [pscustomobject]@{Offset=0x7f8ea0;Length=36;Hash='FD4ACEBBF76E701A03CD2236F93C18CC6A7896AFB990645CA30FB3A8AD2AB641'},
        [pscustomobject]@{Offset=0x18340;Length=216;Hash='C8AF501E61D4E1467225FE9658D788DD2EFFEAD0E6599BE49E8E311C51DA52CF'},
        [pscustomobject]@{Offset=0x18420;Length=137;Hash='707D9E1CC5731EC843DB249034A2B25B40DA5F751EF27C6B3E8F5A39B53F4FE9'},
        [pscustomobject]@{Offset=0xda17a;Length=6;Hash='0FA2BAE65AF459E550EF1A428C69F5A092CA133C076BE54DD14379E040CE1AEE'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Linked chunk code outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed linked chunk code changed.'}
    }
    if([BitConverter]::ToUInt32($Bytes,0x7d832c)-ne 0x8b9f04 -or [Text.Encoding]::ASCII.GetString($Bytes,0x8b9f06,7)-cne "memcpy`0"){throw 'Reviewed memcpy import binding differs.'}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: compute a checked header-relative half-open range, including empty chunks; this diagnostic refusal is not inferred native bounds checking. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11LinkedChunkRange([uint64] $RelativeOffset,[uint32] $Size,[uint64] $FileLength) {
    if($RelativeOffset-gt [uint32]::MaxValue-8 -or $FileLength-lt 8){throw 'Linked chunk start exceeds modeled address bounds.'}
    $start=8+$RelativeOffset;$end=$start+[uint64]$Size
    if($end-gt $FileLength -or $end-gt [uint32]::MaxValue){throw 'Linked chunk range outside file/address bounds.'}
    return [pscustomobject]@{Start=$start;End=$end;Size=$Size}
}
Assert-Ep11LinkedChunkCode $ep11Bytes
$chunkReports=@(foreach($manifest in $producer.Manifests){
    $rawManifest=Read-RuntimeInput $manifest.Path 16777216
    if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($rawManifest))-cne $manifest.ManifestSha256){throw 'Chunk manifest pin changed.'}
    $snapshots=@(foreach($extension in @('.bin','.relo','.imp')){
        $path=[IO.Path]::ChangeExtension($manifest.Path,$extension)
        $matches=@($linked.Sidecars|Where-Object {$_.Snapshot.Path-ieq $path})
        if($matches.Count-ne 1){throw 'Chunk sidecar snapshot is missing or ambiguous.'}
        $matches[0].Snapshot
    })
    [uint64[]]$offsets=@(0,0,0);[int[]]$nonempty=@(0,0,0);$digestText=[Text.StringBuilder]::new();$lastRanges=$null
    for($index=0;$index-lt $manifest.Projection.AssetCount;$index++){
        $entry=52+48*$index
        $lastRanges=@(foreach($kind in 0..2){
            $size=[BitConverter]::ToUInt32($rawManifest,$entry+32+4*$kind)
            $range=Get-Ep11LinkedChunkRange $offsets[$kind] $size $snapshots[$kind].Length
            if($size-ne 0){$nonempty[$kind]++}
            $offsets[$kind]+=$size
            $null=$digestText.AppendFormat([Globalization.CultureInfo]::InvariantCulture,"{0}`t{1}`t{2}`t{3}`n",$index,$kind,$range.Start,$range.End)
            $range
        })
    }
    foreach($kind in 0..2){if(8+$offsets[$kind]-ne $snapshots[$kind].Length){throw 'Final chunk endpoint differs from physical stream end.'}}
    [pscustomobject]@{Path=$manifest.Path;AssetCount=$manifest.Projection.AssetCount;RangesChecked=3*$manifest.Projection.AssetCount;NonemptyCounts=$nonempty;LastRanges=$lastRanges;OrderedRangeSha256=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($digestText.ToString())))}
})
if($SelfTest){
    # Reborn: pure range fixtures cover beginning, middle, empty-at-end and exact-end chunks without reading any sidecar body.
    $cases=@(@(0,4,20,8,12),@(4,8,20,12,20),@(12,0,20,20,20),@(0,0,8,8,8))
    foreach($case in $cases){$range=Get-Ep11LinkedChunkRange $case[0] $case[1] $case[2];if($range.Start-ne $case[3] -or $range.End-ne $case[4]){throw 'Linked range fixture differs.'}}
    foreach($case in @(@(0,1,8),@(0,0,7),@([uint64]4294967295,0,[uint64]4294967303),@(0,4,11))){
        $rejected=$false;try{$null=Get-Ep11LinkedChunkRange $case[0] $case[1] $case[2]}catch{$rejected=$true}
        if(-not $rejected){throw 'Invalid linked range fixture admitted.'}
    }
    # Reborn: private code faults reject altered header bias, descriptor forwarding, linked dispatch, memcpy thunk and import identity.
    foreach($offset in @(0x17241,0x1835f,0x18366,0x1836a,0x183fe,0x18462,0xda17c,0x7d832c,0x8b9f06)){
        $fault=[byte[]]$ep11Bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11LinkedChunkCode $fault}catch{$rejected=$true}
        if(-not $rejected){throw 'Linked chunk code/import fault admitted.'}
    }
}
# Reborn: compare twelve final tiny header snapshots to inherited snapshots; no full-payload integrity or atomic-state claim follows.
foreach($sidecar in $linked.Sidecars){
    $final=Read-Ep11SidecarHeader $sidecar.Snapshot.Path
    if($final.Length-ne $sidecar.Snapshot.Length -or $final.LastWriteUtc-ne $sidecar.Snapshot.LastWriteUtc -or [Convert]::ToHexString($final.Header)-cne [Convert]::ToHexString($sidecar.Snapshot.Header)){throw 'Chunk sidecar metadata changed.'}
}
$chunkAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($chunkAfter))-cne $linked.ImageSha256){throw 'Chunk image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$linked.ImageSha256;QueueMethodVa='0x00417210';DispatchMethodVa='0x00418420';LinkedChunkMethodVa='0x00418340';CopyThunkVa='0x004DA17A';CopyImport='memcpy'
    HeaderBias=8;RelativeDescriptorOffsets=@(0,4,8);ChunkSizesEntryOffsets=@(32,36,40);QueueDescriptorPointerOffset=16
    ConcreteLinkedChunkAddressingRecovered=$true;ObservedManifestCount=$chunkReports.Count;AssetCount=($chunkReports.AssetCount|Measure-Object -Sum).Sum;RangesChecked=($chunkReports.RangesChecked|Measure-Object -Sum).Sum;Manifests=$chunkReports
    HeaderBytesReadTotal=288;PayloadBytesRead=0;NativeReadReturnCheckedInReviewedMethod=$false;NativeBoundsCheckingProved=$false
    AllSourceObjectBindingsRecovered=$false;CompleteStreamPointerProvenanceRecovered=$false;FreshEp11ArchiveExtractionPerformed=$false;ModPackageLoaded=$false;FullPayloadIntegrityProved=$false;ReusedCapacitySemanticsRecovered=$false
    ProductionBuildReady=$false;ReadOnly=$true;TargetExecuted=$false;ManagedInspectorHashCommandExecuted=$linked.ManagedInspectorHashCommandExecuted;FaultTestsPassed=[bool]$SelfTest
    PositiveRangeCasesExecuted=$(if($SelfTest){4}else{0});RangeFaultsExecuted=$(if($SelfTest){4}else{0});MemoryFaultsExecuted=$(if($SelfTest){9}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 9}else{$report}
