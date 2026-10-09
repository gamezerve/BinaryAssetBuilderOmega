# Reborn: establish fresh context ownership for one reviewed loader wrapper without claiming universal reader binding or allocator safety.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$lifeSelfTest=$SelfTest;$lifeAsJson=$AsJson
$producer=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11DescriptorProducer.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath
$SelfTest=$lifeSelfTest;$AsJson=$lifeAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: pin wrapper header dispatch, fresh context initialization/both branches/cleanup and concrete reader construction. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11LoaderLifecycleCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0xcfeb0;Length=30;Hash='BDC7C7B27A1A3E6130889372D07C7C7402BC29DE102F2ABB656A1989E01079FA'},
        [pscustomobject]@{Offset=0xcff5c;Length=205;Hash='2AA453BC67F93DD4A28BAD0FE41542AE1DF3DE16687F954A037EE2AF852592E0'},
        [pscustomobject]@{Offset=0x49700;Length=79;Hash='F14029ACABF98742B30F48AC73571A9AE5F2C65369150A2D9A5568041FD048BB'},
        [pscustomobject]@{Offset=0xaac10;Length=107;Hash='E7D7E6B7922257695D0534A545D4E544B0094FBF0E11A7F7F23B73F2DBDEB4A9'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Lifecycle code outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed loader lifecycle changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: model only successful fresh allocation for a wrapper-local context; decreasing successive counts do not reuse the prior invocation's capacity. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11FreshLoaderCapacity([uint32] $RequestedCount) {
    if($RequestedCount-gt 100000){throw 'Lifecycle diagnostic count exceeds bound.'}
    return [pscustomobject]@{InitialCapacity=0;RequestedCount=$RequestedCount;CapacityAfterSuccessfulPreparation=$RequestedCount;EntryBytes=[uint64]48*$RequestedCount;DescriptorBytes=[uint64]20*$RequestedCount;NonemptyBuffersExpected=($RequestedCount-ne 0)}
}
Assert-Ep11LoaderLifecycleCode $ep11Bytes
# Reborn: the first loader's early producer call uses argument seven: B4h minus 90h locals and two saved registers gives entry ESP+1Ch.
if((Get-Ep11TraceArgument 0x90 2 0xb4 @('arg9','arg8','fresh-context','arg6','arg5','arg4','arg3','arg2','header'))-cne 'fresh-context'){throw 'Fresh producer context argument differs.'}
$capacities=@(foreach($manifest in $producer.Manifests){Get-Ep11FreshLoaderCapacity $manifest.Projection.AssetCount})
if($SelfTest){
    # Reborn: pure successive-invocation fixtures distinguish fresh ownership from reused-capacity behavior; zero count is the producer early-return case.
    foreach($count in @(17339,11357,1,0)){
        $state=Get-Ep11FreshLoaderCapacity $count
        if($state.InitialCapacity-ne 0 -or $state.CapacityAfterSuccessfulPreparation-ne $count -or $state.EntryBytes-ne 48*[uint64]$count -or $state.DescriptorBytes-ne 20*[uint64]$count){throw 'Fresh lifecycle fixture differs.'}
    }
    # Reborn: detached faults reject altered context zeros, count load, branch target, destructor binding and vtable construction.
    foreach($offset in @(0xcff77,0xcff73,0xcffd0,0xd000a,0xd0016,0x4973b,0xaac22)){
        $fault=[byte[]]$ep11Bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11LoaderLifecycleCode $fault}catch{$rejected=$true}
        if(-not $rejected){throw 'Loader lifecycle code fault admitted.'}
    }
}
$lifeAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($lifeAfter))-cne $producer.ImageSha256){throw 'Loader image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$producer.ImageSha256;WrapperVa='0x004CFEB0';ContextInitializerVa='0x004CFF77';ContextDestructorVa='0x00449700';ContextDestructorCallVa='0x004D0016';ConcreteReaderConstructorVa='0x004AAC10'
    WrapperContextBaseStackOffset=4;ContextCapacityOffset=4;ContextEntryPointerOffset=0;ContextDescriptorPointerOffset=8;ContextRequestedCountOffset=20;ContextSourceHandleOffset=24
    WrapperLoaderArgumentCount=9;WrapperArgumentCleanupBytes=36;FirstLoaderContextArgumentOrdinal=7;FreshWrapperContextRecovered=$true;NormalExitBufferReleaseRecovered=$true;CrossWrapperEntryCapacityReused=$false
    FreshCapacityModelAssumesSuccessfulAllocation=$true;AllocatorFailureSemanticsRecovered=$false;AllReaderCapacityReuseRecovered=$false
    ConcreteReaderConstructionRecovered=$true;AllSourceObjectBindingsRecovered=$false;CompleteStreamPointerProvenanceRecovered=$false;ModPackageLoaded=$false;ProductionBuildReady=$false
    ObservedManifestCount=$capacities.Count;Capacities=$capacities;ReadOnly=$true;TargetExecuted=$false;ManagedInspectorHashCommandExecuted=$producer.ManagedInspectorHashCommandExecuted
    FaultTestsPassed=[bool]$SelfTest;FreshInvocationCasesExecuted=$(if($SelfTest){4}else{0});MemoryFaultsExecuted=$(if($SelfTest){7}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 7}else{$report}
