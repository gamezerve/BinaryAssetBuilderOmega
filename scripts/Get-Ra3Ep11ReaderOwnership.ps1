# Reborn: recover scoped wrapper/source-owner flow and pointer-membership semantics without executing native readers or assuming all-source provenance.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$ownerSelfTest=$SelfTest;$ownerAsJson=$AsJson
$factory=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11ReaderFactory.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath
$SelfTest=$ownerSelfTest;$AsJson=$ownerAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: retain exact membership, outer-wrapper and source-list iterator bodies for the reviewed engine; byte pins are not native execution. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11ReaderOwnershipCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0x5f060;Length=55;Hash='51D67A1A649BD0FBB06F994E1E119ED47AD95D043FF518E42EA1A4B3CCE4D6A7'},
        [pscustomobject]@{Offset=0xcfeb0;Length=377;Hash='34F61F895AF698B410E4F8B154ADF9B471CE6D5826A08EC7A5A9F4989C733327'},
        [pscustomobject]@{Offset=0xd04a0;Length=320;Hash='64E91964806AC4B4BD8AE444D49419ED46052E34D28EFD163D320CDD092525B0'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Reader ownership slice outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed reader ownership changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: model only pointer equality across three already-enumerated lists; do not dereference addresses or emulate unchecked native list walking. #>
#-------------------------------------------------------------------------------------------------
function Test-Ep11ReaderPointerMembership([uint32] $Reader,[object[]] $Groups) {
    if($Groups.Count-ne 3){throw 'Membership model requires exactly three groups.'}
    foreach($group in $Groups){
        if($null-eq $group -or $null-eq $group.Values -or $group.Values.Count-gt 100000){throw 'Membership diagnostic group invalid or exceeds bound.'}
        foreach($value in $group.Values){if([uint32]$value-eq $Reader){return $true}}
    }
    return $false
}
Assert-Ep11ReaderOwnershipCode $ep11Bytes
# Reborn: wrapper argument two supplies both header/cleanup dispatch and first-loader argument three; argument three supplies first-loader argument four.
$wrapperPushOrder=@('extra','arg5','arg4','list-owner','source-handle','workspace')
if((Get-Ep11TraceArgument 0x60 1 0x6c $wrapperPushOrder)-cne 'source-handle' -or
   (Get-Ep11TraceArgument 0x60 1 0x70 $wrapperPushOrder)-cne 'list-owner'){throw 'Outer wrapper source/list arguments differ.'}
# Reborn: pin the bounded overlapping call candidate set; only the two iterator calls are reviewed as complete caller paths here.
$ownerWrapperCalls=@(Find-Ep11ConsumerCallCandidates $ep11Bytes 0x4cfeb0)
if(($ownerWrapperCalls -join ',')-cne '0x000D0566,0x000D05BA,0x000D06DB,0x000D07F8'){throw 'Wrapper call candidate set differs.'}
if((Get-Ep11TraceArgument 0x6c 2 0x78 @('arg3','arg2','owner'))-cne 'owner' -or
   (Get-Ep11TraceArgument 0x70 4 0x8c @('forwarded-wrapper-arg5','arg2','owner'))-cne 'forwarded-wrapper-arg5'){throw 'Iterator owner/forwarding stack mapping differs.'}
if($SelfTest){
    # Reborn: pure fixtures cover empty/absent inputs, a hit in each global-list position and raw zero-pointer equality; callers reject null separately.
    foreach($case in @(-2,-1,0,1,2,3)){
        $groups=@(foreach($index in 0..2){[pscustomobject]@{Values=[uint32[]]@()}})
        $reader=[uint32]0x1234
        if($case-eq -1){$groups[0].Values=[uint32[]]@(0x4321)}
        if($case-ge 0 -and $case-le 2){$groups[$case].Values=[uint32[]]@(0x4321,0x1234)}
        if($case-eq 3){$reader=0;$groups[2].Values=[uint32[]]@(0)}
        if((Test-Ep11ReaderPointerMembership $reader $groups)-ne ($case-ge 0)){throw 'Pointer-membership fixture differs.'}
    }
    foreach($count in @(2,4)){
        $groups=@(foreach($index in 1..$count){[pscustomobject]@{Values=[uint32[]]@()}})
        $rejected=$false;try{$null=Test-Ep11ReaderPointerMembership 1 $groups}catch{$rejected=$true}
        if(-not $rejected){throw 'Wrong membership group count admitted.'}
    }
    # Reborn: detached faults cover pointer compare/stride/bound, wrapper owner loads, source forwarding, iterator payload and both wrapper calls.
    foreach($offset in @(0x5f076,0x5f084,0x5f087,0xcfeb4,0xcff6f,0xcffc9,0xd050e,0xd0507,0xd0566,0xd05b1,0xd05ba,0xd05c2)){
        $fault=[byte[]]$ep11Bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11ReaderOwnershipCode $fault}catch{$rejected=$true}
        if(-not $rejected){throw 'Reader ownership code fault admitted.'}
    }
}
$ownerAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($ownerAfter))-cne $factory.ImageSha256){throw 'Ownership image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$factory.ImageSha256;WrapperVa='0x004CFEB0';IteratorVa='0x004D04A0';PointerMembershipHelperVa='0x0045F060'
    MembershipListHeads=@('0x00CF154C','0x00CF1584','0x00CF15BC');MembershipListStride=56;MembershipValueOffset=8;MembershipComparesReaderPointers=$true;MembershipValidatesAssetContents=$false
    WrapperSourceHandleArgumentOrdinal=2;WrapperListOwnerArgumentOrdinal=3;FirstLoaderFactoryOwnerArgumentOrdinal=3;FirstLoaderListOwnerArgumentOrdinal=4
    FirstLoaderFactoryHandleEqualsWrapperSourceHandle=$true;HeaderAndFactoryUseSameWrapperSource=$true
    WrapperCallCandidateOffsets=$ownerWrapperCalls;ReviewedWrapperCallerPaths=2;AllWrapperCallerPathsRecovered=$false;IteratorOwnerHeadOffset=12;IteratorSourceHandleOffset=8;IteratorWrapperCalls=@('0x004D0566','0x004D05BA');IteratorSourceHandleIsCopiedOnAlternateBranch=$true
    ScopedIteratorSourceForwardingRecovered=$true;IteratorSixthWrapperArgumentValue=0;IteratorNinthLoaderArgumentValue=0;NinthArgumentConsumerRecovered=$false;OrdinaryMembershipSemanticsRecovered=$true;TagTwoAdmissionSemanticsRecovered=$false
    StoredHeadReaderToSpecificIterationProven=$false;ListContainerOwnershipRecovered=$false;LiveFactoryOwnerVtableBindingProven=$false
    AllSourceObjectBindingsRecovered=$false;CompleteStreamPointerProvenanceRecovered=$false;ModPackageLoaded=$false;ProductionBuildReady=$false;ReadOnly=$true;TargetExecuted=$false
    ManagedInspectorHashCommandExecuted=$factory.ManagedInspectorHashCommandExecuted;FaultTestsPassed=[bool]$SelfTest
    MembershipFixturesExecuted=$(if($SelfTest){6}else{0});InvalidGroupFixturesExecuted=$(if($SelfTest){2}else{0});MemoryFaultsExecuted=$(if($SelfTest){12}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 7}else{$report}
