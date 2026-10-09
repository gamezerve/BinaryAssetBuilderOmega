# Reborn: recover scoped wrapper/source-owner flow and pointer-membership semantics without executing native readers or assuming all-source provenance.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$ownerSelfTest=$SelfTest;$ownerAsJson=$AsJson
$factory=. (Join-Path $PSScriptRoot 'Get-Ra3Ep1ReaderFactory.ps1') -ImagePath $ImagePath
$SelfTest=$ownerSelfTest;$AsJson=$ownerAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: retain exact membership, outer-wrapper and source-list iterator bodies for the reviewed engine; byte pins are not native execution. #>
#-------------------------------------------------------------------------------------------------
function Assert-ReaderOwnershipCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0x5f000;Length=55;Hash='E6804CB2E752BEA9009AD8AD011C43275DCF803C9AD5177162EDFBD00DA8F4FE'},
        [pscustomobject]@{Offset=0xcfbf0;Length=377;Hash='47C0C9B3BC24EC1C4458BB469A712FD1B6D79817364BC946026EEFD7D335631C'},
        [pscustomobject]@{Offset=0xd01e0;Length=320;Hash='D35A1502107C2D5D192D5ED60E7EFBA8977ED083623E5968DD93409E94512BC8'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Reader ownership slice outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed reader ownership changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: model only pointer equality across three already-enumerated lists; do not dereference addresses or emulate unchecked native list walking. #>
#-------------------------------------------------------------------------------------------------
function Test-ReaderPointerMembership([uint32] $Reader,[object[]] $Groups) {
    if($Groups.Count-ne 3){throw 'Membership model requires exactly three groups.'}
    foreach($group in $Groups){
        if($null-eq $group -or $null-eq $group.Values -or $group.Values.Count-gt 100000){throw 'Membership diagnostic group invalid or exceeds bound.'}
        foreach($value in $group.Values){if([uint32]$value-eq $Reader){return $true}}
    }
    return $false
}
Assert-ReaderOwnershipCode $bytes
# Reborn: wrapper argument two supplies both header/cleanup dispatch and first-loader argument three; argument three supplies first-loader argument four.
$wrapperPushOrder=@('arg5','arg4','list-owner','source-handle','workspace')
if((Get-TraceEntryArgument 0x60 1 0x6c $wrapperPushOrder)-cne 'source-handle' -or
   (Get-TraceEntryArgument 0x60 1 0x70 $wrapperPushOrder)-cne 'list-owner'){throw 'Outer wrapper source/list arguments differ.'}
if($SelfTest){
    # Reborn: pure fixtures cover empty/absent inputs, a hit in each global-list position and raw zero-pointer equality; callers reject null separately.
    foreach($case in @(-2,-1,0,1,2,3)){
        $groups=@(foreach($index in 0..2){[pscustomobject]@{Values=[uint32[]]@()}})
        $reader=[uint32]0x1234
        if($case-eq -1){$groups[0].Values=[uint32[]]@(0x4321)}
        if($case-ge 0 -and $case-le 2){$groups[$case].Values=[uint32[]]@(0x4321,0x1234)}
        if($case-eq 3){$reader=0;$groups[2].Values=[uint32[]]@(0)}
        if((Test-ReaderPointerMembership $reader $groups)-ne ($case-ge 0)){throw 'Pointer-membership fixture differs.'}
    }
    foreach($count in @(2,4)){
        $groups=@(foreach($index in 1..$count){[pscustomobject]@{Values=[uint32[]]@()}})
        $rejected=$false;try{$null=Test-ReaderPointerMembership 1 $groups}catch{$rejected=$true}
        if(-not $rejected){throw 'Wrong membership group count admitted.'}
    }
    # Reborn: detached faults cover pointer compare/stride/bound, wrapper owner loads, source forwarding, iterator payload and both wrapper calls.
    foreach($offset in @(0x5f016,0x5f024,0x5f027,0xcfbf4,0xcfcd1,0xcfd01,0xd024d,0xd02a5,0xd02f1,0xd02fa,0xd0302)){
        $fault=[byte[]]$bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-ReaderOwnershipCode $fault}catch{$rejected=$true}
        if(-not $rejected){throw 'Reader ownership code fault admitted.'}
    }
}
$ownerAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($ownerAfter))-cne $factory.ImageSha256){throw 'Ownership image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$factory.ImageSha256;WrapperVa='0x004CFBF0';IteratorVa='0x004D01E0';PointerMembershipHelperVa='0x0045F000'
    MembershipListHeads=@('0x00CEA44C','0x00CEA484','0x00CEA4BC');MembershipListStride=56;MembershipValueOffset=8;MembershipComparesReaderPointers=$true;MembershipValidatesAssetContents=$false
    WrapperSourceHandleArgumentOrdinal=2;WrapperListOwnerArgumentOrdinal=3;FirstLoaderFactoryOwnerArgumentOrdinal=3;FirstLoaderListOwnerArgumentOrdinal=4
    FirstLoaderFactoryHandleEqualsWrapperSourceHandle=$true;HeaderAndFactoryUseSameWrapperSource=$true
    IteratorOwnerHeadOffset=12;IteratorSourceHandleOffset=8;IteratorWrapperCalls=@('0x004D02A5','0x004D02FA');IteratorSourceHandleIsCopiedOnAlternateBranch=$true
    ScopedIteratorSourceForwardingRecovered=$true;OrdinaryMembershipSemanticsRecovered=$true;TagTwoAdmissionSemanticsRecovered=$false
    StoredHeadReaderToSpecificIterationProven=$false;ListContainerOwnershipRecovered=$false;LiveFactoryOwnerVtableBindingProven=$false
    AllSourceObjectBindingsRecovered=$false;CompleteStreamPointerProvenanceRecovered=$false;ProductionBuildReady=$false;ReadOnly=$true;TargetExecuted=$false
    ManagedInspectorHashCommandExecuted=$factory.ManagedInspectorHashCommandExecuted;FaultTestsPassed=[bool]$SelfTest
    MembershipFixturesExecuted=$(if($SelfTest){6}else{0});InvalidGroupFixturesExecuted=$(if($SelfTest){2}else{0});MemoryFaultsExecuted=$(if($SelfTest){11}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 7}else{$report}
