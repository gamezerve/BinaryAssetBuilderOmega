# Reborn: connect scoped referenced-reader factory dispatch/cache branches to observed loader storage without claiming universal reader provenance.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$factorySelfTest=$SelfTest;$factoryAsJson=$AsJson
$lifecycle=. (Join-Path $PSScriptRoot 'Get-Ra3Ep1LoaderLifecycle.ps1') -ImagePath $ImagePath
$SelfTest=$factorySelfTest;$AsJson=$factoryAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: pin complete reviewed factories, dispatch/storage instructions, concrete vtable slots and allocation helper; never execute native code. #>
#-------------------------------------------------------------------------------------------------
function Assert-ReaderFactoryCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0xaa9c0;Length=472;Hash='E339978141CB185F24DDD32B8EA5F71578EFF4A0ADF55C5A1163E6477C5AE739'},
        [pscustomobject]@{Offset=0xaaba0;Length=18;Hash='337C86181301781B021A6A8F314F589B566900970C132979DFD67C68B386B1C5'},
        [pscustomobject]@{Offset=0xaabc0;Length=354;Hash='AE3CBA65D3860C6BF533E15070DBC23CDB11CBB6BE6DA2DD9F35C40511439651'},
        [pscustomobject]@{Offset=0xce713;Length=183;Hash='2F6436AB9A9A20C8E605FD9A1E9B55F720D15CF307CF6C4FC1C0C0A597DE8A00'},
        [pscustomobject]@{Offset=0x7f2e78;Length=48;Hash='8F602650019BA3A0D9152B05FFB5CFA9E43FAF40D2C6912918F95DCFEFE60D23'},
        [pscustomobject]@{Offset=0x85ec0;Length=81;Hash='1E52DCE34B1DDDD46811A0663CAB1B72D428C53232E7A052C66EBFADADD7533C'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Factory slice outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed reader factory changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: model only the reviewed signed-byte tag routing; raw tag two is not assigned an unproven authoring or patch semantic. #>
#-------------------------------------------------------------------------------------------------
function Get-ReaderFactoryRoute([int] $RawTag) {
    if($RawTag-lt 0 -or $RawTag-gt 255){throw 'Reference tag outside one byte.'}
    $signedTag=if($RawTag-ge 128){$RawTag-256}else{$RawTag}
    $tagTwo=$signedTag-eq 2
    [pscustomobject]@{RawTag=$RawTag;SignedTag=$signedTag;VirtualSlot=$(if($tagTwo){44}else{40});ConcreteTableTargetVa=$(if($tagTwo){'0x004AABC0'}else{'0x004AABA0'});CacheVa=$(if($tagTwo){'0x00CBDAF8'}else{'0x00CBDADC'});OrdinaryListBranch=(-not $tagTwo)}
}
Assert-ReaderFactoryCode $bytes
# Reborn: pathname is still pushed during factory-owner load, whereas the list-owner load has no temporary argument; these are distinct entry arguments.
$pushOrder=@('arg8','arg7','arg6','arg5','list-owner','factory-owner','arg2','arg1')
if((Get-TraceEntryArgument 0x94 4 0xb0 $pushOrder)-cne 'factory-owner' -or
   (Get-TraceEntryArgument 0x90 4 0xb0 $pushOrder)-cne 'list-owner'){throw 'Factory/list owner stack binding differs.'}
if($SelfTest){
    # Reborn: detached tag fixtures include signed-byte values and demonstrate that all tags except two use slot 28h, without admitting malformed streams.
    foreach($tag in @(0,1,2,127,128,255)){
        $route=Get-ReaderFactoryRoute $tag
        if($route.VirtualSlot-ne $(if($tag-eq 2){44}else{40}) -or $route.OrdinaryListBranch-ne ($tag-ne 2)){throw 'Factory route fixture differs.'}
        if($route.SignedTag-ne $(if($tag-ge 128){$tag-256}else{$tag})){throw 'Signed tag fixture differs.'}
    }
    foreach($tag in @(-1,256)){
        $rejected=$false;try{$null=Get-ReaderFactoryRoute $tag}catch{$rejected=$true}
        if(-not $rejected){throw 'Out-of-byte tag admitted.'}
    }
    # Reborn: mutate private copies at cache hits, constructor calls, cache publication, dispatch, stored pointer, vtable and helper allocation.
    foreach($offset in @(0xaab0a,0xaab3a,0xaab71,0xaaba7,0xaace0,0xaad09,0xce743,0xce753,0xce7c7,0x7f2ea0,0x85ed4)){
        $fault=[byte[]]$bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-ReaderFactoryCode $fault}catch{$rejected=$true}
        if(-not $rejected){throw 'Factory code fault admitted.'}
    }
}
$factoryAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($factoryAfter))-cne $lifecycle.ImageSha256){throw 'Factory image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$lifecycle.ImageSha256;ConcreteVtableVa='0x00BF2E78';ConstructorVa='0x004AA950';OrdinaryFactoryVa='0x004AA9C0';TagTwoFactoryVa='0x004AABC0'
    FactoryOwnerArgumentOrdinal=3;ListOwnerArgumentOrdinal=4;FactoryAndListOwnerAreSameArgument=$false
    OrdinaryConstructorArguments=@('prepared-path',1,1,0,1);TagTwoConstructorArguments=@('prepared-path',0,0,1,0)
    NewOrdinaryReaderCachePublicationRecovered=$true;TagTwoMissCachePublicationInReviewedBody=$false
    CacheHitValueOffset=32;CacheHitVtableRevalidatedInReviewedBodies=$false;ReferenceDispatchRecovered=$true
    ReturnedReaderStoredAtListHeadOffset=8;ListOwnerHeadFieldOffset=12;ReturnedReaderStoredInFreshNodeProven=$false
    StorageCondition='tag != 2; reader != null; helper 0045F000 returns AL == 0'
    ConcreteTableFactoryTargetsRecovered=$true;LiveFactoryOwnerVtableBindingProven=$false;AllSourceObjectBindingsRecovered=$false
    AdmissionHelperSemanticsRecovered=$false;AllocatorFailureSafetyProven=$false;CompleteStreamPointerProvenanceRecovered=$false;ProductionBuildReady=$false
    Routes=@(Get-ReaderFactoryRoute 1;Get-ReaderFactoryRoute 2;Get-ReaderFactoryRoute 255)
    ReadOnly=$true;TargetExecuted=$false;ManagedInspectorHashCommandExecuted=$lifecycle.ManagedInspectorHashCommandExecuted
    FaultTestsPassed=[bool]$SelfTest;RouteFixturesExecuted=$(if($SelfTest){6}else{0});InvalidTagFixturesExecuted=$(if($SelfTest){2}else{0});MemoryFaultsExecuted=$(if($SelfTest){11}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 7}else{$report}
