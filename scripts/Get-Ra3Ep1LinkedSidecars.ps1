# Reborn: review linked sidecar setup and bounded eight-byte headers without executing native loaders or reading large stream payloads.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$sidecarSelfTest=$SelfTest;$sidecarAsJson=$AsJson
$producer=. (Join-Path $PSScriptRoot 'Get-Ra3Ep1DescriptorProducer.ps1') -ImagePath $ImagePath
$SelfTest=$sidecarSelfTest;$AsJson=$sidecarAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: pin the entire reviewed sidecar helper and bounded full-file read helper, preserving unresolved diagnostic/allocator behavior. #>
#-------------------------------------------------------------------------------------------------
function Assert-LinkedSidecarCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0x49760;Length=1055;Hash='68841952584F2F76BEE266E09C508E60EC82BA472BC5F3F0D5462A55D5720C1B'},
        [pscustomobject]@{Offset=0x3d040;Length=112;Hash='660D5346B8004FD9577CF1CA8049EB924CFD5730FC62B4DED3C99415131B4BF3'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Linked sidecar code outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed linked sidecar code changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: snapshot only eight bytes plus metadata from explicitly selected regular stream files, rejecting reparse ancestors and never allocating the payload. #>
#-------------------------------------------------------------------------------------------------
function Read-SidecarHeader([string] $Path) {
    if(-not [IO.Path]::IsPathFullyQualified($Path)){throw 'Sidecar path must be absolute.'}
    $item=Get-Item -LiteralPath $Path -Force
    if($item.PSIsContainer -or $item.Length-lt 8 -or $item.Length-gt 4294967303){throw 'Sidecar size outside diagnostic bounds.'}
    $ancestor=$item
    while($null-ne $ancestor){
        if(($ancestor.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Reparse sidecar paths are not admitted.'}
        if($ancestor-is [IO.FileInfo]){$ancestor=$ancestor.Directory}else{$ancestor=$ancestor.Parent}
    }
    $stream=[IO.File]::Open($item.FullName,[IO.FileMode]::Open,[IO.FileAccess]::Read,[IO.FileShare]::Read)
    try{
        $length=$stream.Length;$header=[byte[]]::new(8);$stream.ReadExactly($header,0,8)
        if($stream.Length-ne $length){throw 'Sidecar length changed during header read.'}
        return [pscustomobject]@{Path=$item.FullName;Length=$length;Header=$header;LastWriteUtc=$item.LastWriteTimeUtc.Ticks}
    }finally{$stream.Dispose()}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: apply diagnostic magic/checksum/exact-length checks to a header snapshot; stricter magic/length checks are not inferred native policy. #>
#-------------------------------------------------------------------------------------------------
function Assert-SidecarHeader($Snapshot,[uint32] $Magic,[uint32] $Checksum,[uint64] $PayloadBytes) {
    if($Snapshot.Header.Length-ne 8 -or [BitConverter]::ToUInt32($Snapshot.Header,0)-ne $Magic -or
        [BitConverter]::ToUInt32($Snapshot.Header,4)-ne $Checksum -or $Snapshot.Length-ne 8+$PayloadBytes){throw 'Sidecar diagnostic header/length mismatch.'}
}
Assert-LinkedSidecarCode $bytes
$sidecars=@(foreach($manifest in $producer.Manifests){
    $manifestBytes=Read-RuntimeInput $manifest.Path 16777216
    if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($manifestBytes))-cne $manifest.ManifestSha256){throw 'Linked manifest snapshot changed.'}
    $checksum=[BitConverter]::ToUInt32($manifestBytes,8)
    foreach($spec in @(
        [pscustomobject]@{Extension='.bin';Magic=[uint32]0xbabb0000L;Payload=$manifest.Projection.BinBytes},
        [pscustomobject]@{Extension='.relo';Magic=[uint32]0xbabe0000L;Payload=$manifest.Projection.ReloBytes},
        [pscustomobject]@{Extension='.imp';Magic=[uint32]0xbab10000L;Payload=$manifest.Projection.ImpBytes}
    )){
        $snapshot=Read-SidecarHeader ([IO.Path]::ChangeExtension($manifest.Path,$spec.Extension))
        Assert-SidecarHeader $snapshot $spec.Magic $checksum $spec.Payload
        [pscustomobject]@{Snapshot=$snapshot;ExpectedMagic=('0x{0:X8}' -f $spec.Magic);ManifestChecksum=('0x{0:X8}' -f $checksum);PayloadBytes=$spec.Payload}
    }
})
if($SelfTest){
    # Reborn: detached snapshots test bad magic/checksum/length/truncation without touching shipping streams.
    foreach($kind in @('magic','checksum','length','short')){
        $fault=[pscustomobject]@{Header=[byte[]]$sidecars[0].Snapshot.Header.Clone();Length=$sidecars[0].Snapshot.Length}
        switch($kind){'magic'{$fault.Header[0]=$fault.Header[0]-bxor 1};'checksum'{$fault.Header[4]=$fault.Header[4]-bxor 1};'length'{$fault.Length++};'short'{$fault.Header=[byte[]]$fault.Header[0..6]}}
        $rejected=$false;try{Assert-SidecarHeader $fault ([Convert]::ToUInt32($sidecars[0].ExpectedMagic.Substring(2),16)) ([Convert]::ToUInt32($sidecars[0].ManifestChecksum.Substring(2),16)) $sidecars[0].PayloadBytes}catch{$rejected=$true}
        if(-not $rejected){throw 'Corrupt detached sidecar snapshot admitted.'}
    }
    # Reborn: exact helper pins reject private changes to suffixes, checksum comparisons, cursor adjustment and return flow.
    foreach($offset in @(0x49792,0x497c9,0x497cd,0x49848,0x49901,0x49af7,0x49b73,0x3d096)){
        $fault=[byte[]]$bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-LinkedSidecarCode $fault}catch{$rejected=$true}
        if(-not $rejected){throw 'Linked helper code fault admitted.'}
    }
}
# Reborn: re-read exactly the same twelve small headers/metadata; neither repeat equality nor timestamps prove full payload integrity or an atomic snapshot.
foreach($sidecar in $sidecars){
    $repeat=Read-SidecarHeader $sidecar.Snapshot.Path
    if($repeat.Length-ne $sidecar.Snapshot.Length -or $repeat.LastWriteUtc-ne $sidecar.Snapshot.LastWriteUtc -or
        [Convert]::ToHexString($repeat.Header)-cne [Convert]::ToHexString($sidecar.Snapshot.Header)){throw 'Sidecar header/metadata changed during review.'}
}
$sidecarAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($sidecarAfter))-cne $producer.ImageSha256){throw 'Linked helper image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$producer.ImageSha256;LinkedHelperVa='0x00449760';WholeSidecarReaderVa='0x0043D040';HelperRole='linked-sidecar-setup-not-entry-transformation'
    SuppliedEntryArgumentsReadByReviewedHelper=$false;DirectEntryHashOrSizeWritesObserved=$false;SidecarSetupRecovered=$true;NativeChecksumDiagnosticsRecovered=$true
    NativeMagicValidationProved=$false;NativeUnconditionalChecksumRejectionProved=$false;CompleteStreamPointerProvenanceRecovered=$false;AllSourceObjectBindingsRecovered=$false
    DescriptorProducerRecovered=$true;SidecarHeaderBytes=8;ObservedSidecarCount=$sidecars.Count;HeaderBytesReadSinglePass=96;HeaderBytesReadTotal=192;PayloadBytesRead=0
    WorldBuilderBinBytes=($sidecars|Where-Object {$_.Snapshot.Path-like '*WorldBuilder*worldbuilder.bin'}).Snapshot.Length;Sidecars=$sidecars
    FullSidecarPayloadIntegrityProved=$false;ReusedCapacitySemanticsRecovered=$false;ProductionBuildReady=$false;ReadOnly=$true;TargetExecuted=$false
    ManagedInspectorHashCommandExecuted=$producer.ManagedInspectorHashCommandExecuted;FaultTestsPassed=[bool]$SelfTest;HeaderFaultsExecuted=$(if($SelfTest){4}else{0});MemoryFaultsExecuted=$(if($SelfTest){8}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 9}else{$report}
