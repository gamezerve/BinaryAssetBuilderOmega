# Reborn: connect scoped referenced-reader factory dispatch/cache branches to observed loader storage without claiming universal reader provenance.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$factorySelfTest=$SelfTest;$factoryAsJson=$AsJson
$lifecycle=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11LoaderLifecycle.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath
$SelfTest=$factorySelfTest;$AsJson=$factoryAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: pin complete reviewed factories, dispatch/storage instructions, concrete vtable slots and allocation helper; never execute native code. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11ReaderFactoryCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0xaac80;Length=472;Hash='3D20F517457A22F58957DD3337A48F137AF2EE745E1E070D4D0CB3FABEB5DF0A'},
        [pscustomobject]@{Offset=0xaae60;Length=18;Hash='337C86181301781B021A6A8F314F589B566900970C132979DFD67C68B386B1C5'},
        [pscustomobject]@{Offset=0xaae80;Length=354;Hash='045D82B013FDB303B5BD43E96F25C7EB0706562736D85024D1BCA8C639A75261'},
        [pscustomobject]@{Offset=0xce9d3;Length=183;Hash='22632B3E2E1E18202D78FD9B31113D405ABC81AE6567D2E050CF142FBFD058D2'},
        [pscustomobject]@{Offset=0x7f8ea0;Length=48;Hash='7EB7C63FDEE57177BA67FEDFD449A803CE32573039333224C4623CD27A1D612D'},
        [pscustomobject]@{Offset=0x85f80;Length=81;Hash='69B47655BC52D2585469A55606D2EB7DF8C131CEC53595F88EBB491449089D3B'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Factory slice outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed reader factory changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: model only the reviewed signed-byte tag routing; raw tag two is not assigned an unproven authoring or patch semantic. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11ReaderFactoryRoute([int] $RawTag) {
    if($RawTag-lt 0 -or $RawTag-gt 255){throw 'Reference tag outside one byte.'}
    $signedTag=if($RawTag-ge 128){$RawTag-256}else{$RawTag}
    $tagTwo=$signedTag-eq 2
    [pscustomobject]@{RawTag=$RawTag;SignedTag=$signedTag;VirtualSlot=$(if($tagTwo){44}else{40});ConcreteTableTargetVa=$(if($tagTwo){'0x004AAE80'}else{'0x004AAE60'});CacheVa=$(if($tagTwo){'0x00CC4AF8'}else{'0x00CC4ADC'});OrdinaryListBranch=(-not $tagTwo)}
}
Assert-Ep11ReaderFactoryCode $ep11Bytes
# Reborn: pathname is still pushed during factory-owner load, whereas the list-owner load has no temporary argument; these are distinct entry arguments.
$pushOrder=@('arg9','arg8','arg7','arg6','arg5','list-owner','factory-owner','arg2','arg1')
if((Get-Ep11TraceArgument 0x94 4 0xb0 $pushOrder)-cne 'factory-owner' -or
   (Get-Ep11TraceArgument 0x90 4 0xb0 $pushOrder)-cne 'list-owner'){throw 'Factory/list owner stack binding differs.'}
if($SelfTest){
    # Reborn: detached tag fixtures include signed-byte values and demonstrate that all tags except two use slot 28h, without admitting malformed streams.
    foreach($tag in @(0,1,2,127,128,255)){
        $route=Get-Ep11ReaderFactoryRoute $tag
        if($route.VirtualSlot-ne $(if($tag-eq 2){44}else{40}) -or $route.OrdinaryListBranch-ne ($tag-ne 2)){throw 'Factory route fixture differs.'}
        if($route.SignedTag-ne $(if($tag-ge 128){$tag-256}else{$tag})){throw 'Signed tag fixture differs.'}
    }
    foreach($tag in @(-1,256)){
        $rejected=$false;try{$null=Get-Ep11ReaderFactoryRoute $tag}catch{$rejected=$true}
        if(-not $rejected){throw 'Out-of-byte tag admitted.'}
    }
    # Reborn: mutate private copies at cache hits, constructor calls, cache publication, dispatch, stored pointer, vtable and helper allocation.
    foreach($offset in @(0xaadca,0xaadfa,0xaae31,0xaae67,0xaafa0,0xaafc9,0xcea03,0xcea13,0xcea87,0x7f8ec8,0x85f94)){
        $fault=[byte[]]$ep11Bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11ReaderFactoryCode $fault}catch{$rejected=$true}
        if(-not $rejected){throw 'Factory code fault admitted.'}
    }
}
$factoryAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($factoryAfter))-cne $lifecycle.ImageSha256){throw 'Factory image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$lifecycle.ImageSha256;ConcreteVtableVa='0x00BF8EA0';ConstructorVa='0x004AAC10';OrdinaryFactoryVa='0x004AAC80';TagTwoFactoryVa='0x004AAE80'
    FactoryOwnerArgumentOrdinal=3;ListOwnerArgumentOrdinal=4;FactoryAndListOwnerAreSameArgument=$false
    OrdinaryConstructorArguments=@('prepared-path',1,1,0,1);TagTwoConstructorArguments=@('prepared-path',0,0,1,0)
    NewOrdinaryReaderCachePublicationRecovered=$true;TagTwoMissCachePublicationInReviewedBody=$false
    CachePublicationHelperVa='0x006A34B0';CachePublicationHelperSemanticsRecovered=$false;CacheHitValueOffset=32;CacheHitVtableRevalidatedInReviewedBodies=$false;ReferenceDispatchRecovered=$true
    ReturnedReaderStoredAtListHeadOffset=8;ListOwnerHeadFieldOffset=12;ReturnedReaderStoredInFreshNodeProven=$false
    StorageCondition='tag != 2; reader != null; helper 0045F060 returns AL == 0'
    ConcreteTableFactoryTargetsRecovered=$true;LiveFactoryOwnerVtableBindingProven=$false;AllSourceObjectBindingsRecovered=$false
    AdmissionHelperSemanticsRecovered=$false;AllocatorFailureSafetyProven=$false;CompleteStreamPointerProvenanceRecovered=$false;ModPackageLoaded=$false;ProductionBuildReady=$false
    Routes=@(Get-Ep11ReaderFactoryRoute 1;Get-Ep11ReaderFactoryRoute 2;Get-Ep11ReaderFactoryRoute 255)
    ReadOnly=$true;TargetExecuted=$false;ManagedInspectorHashCommandExecuted=$lifecycle.ManagedInspectorHashCommandExecuted
    FaultTestsPassed=[bool]$SelfTest;RouteFixturesExecuted=$(if($SelfTest){6}else{0});InvalidTagFixturesExecuted=$(if($SelfTest){2}else{0});MemoryFaultsExecuted=$(if($SelfTest){11}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 7}else{$report}
