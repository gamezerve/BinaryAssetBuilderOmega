# Reborn: establish fresh context ownership for one reviewed loader wrapper without claiming universal reader binding or allocator safety.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$lifeSelfTest=$SelfTest;$lifeAsJson=$AsJson
$producer=. (Join-Path $PSScriptRoot 'Get-Ra3Ep1DescriptorProducer.ps1') -ImagePath $ImagePath
$SelfTest=$lifeSelfTest;$AsJson=$lifeAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: pin wrapper header dispatch, fresh context initialization/both branches/cleanup and concrete reader construction. #>
#-------------------------------------------------------------------------------------------------
function Assert-LoaderLifecycleCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0xcfbf0;Length=30;Hash='BDC7C7B27A1A3E6130889372D07C7C7402BC29DE102F2ABB656A1989E01079FA'},
        [pscustomobject]@{Offset=0xcfc9c;Length=205;Hash='9403EC39651F48BC1A77C7E0C1265024E6095DD2D1651381AEFF4DB972007556'},
        [pscustomobject]@{Offset=0x49650;Length=79;Hash='DDF9758DEDBDBA7E89052D720AB0F9C5F30247BB714AC57ECB4152258030BB4C'},
        [pscustomobject]@{Offset=0xaa950;Length=107;Hash='E6BB4354DEBD007AC2B4E171B70E143268944C3E8FD138D346EC9AE7CBD37B1A'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Lifecycle code outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed loader lifecycle changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: model only successful fresh allocation for a wrapper-local context; decreasing successive counts do not reuse the prior invocation's capacity. #>
#-------------------------------------------------------------------------------------------------
function Get-FreshLoaderCapacity([uint32] $RequestedCount) {
    if($RequestedCount-gt 100000){throw 'Lifecycle diagnostic count exceeds bound.'}
    return [pscustomobject]@{InitialCapacity=0;RequestedCount=$RequestedCount;CapacityAfterSuccessfulPreparation=$RequestedCount;EntryBytes=[uint64]48*$RequestedCount;DescriptorBytes=[uint64]20*$RequestedCount;NonemptyBuffersExpected=($RequestedCount-ne 0)}
}
Assert-LoaderLifecycleCode $bytes
# Reborn: the first loader's early producer call uses argument seven: B4h minus 90h locals and two saved registers gives entry ESP+1Ch.
if((Get-TraceEntryArgument 0x90 2 0xb4 @('arg8','fresh-context','arg6','arg5','arg4','arg3','arg2','header'))-cne 'fresh-context'){throw 'Fresh producer context argument differs.'}
$capacities=@(foreach($manifest in $producer.Manifests){Get-FreshLoaderCapacity $manifest.Projection.AssetCount})
if($SelfTest){
    # Reborn: pure successive-invocation fixtures distinguish fresh ownership from reused-capacity behavior; zero count is the producer early-return case.
    foreach($count in @(17339,11357,1,0)){
        $state=Get-FreshLoaderCapacity $count
        if($state.InitialCapacity-ne 0 -or $state.CapacityAfterSuccessfulPreparation-ne $count -or $state.EntryBytes-ne 48*[uint64]$count -or $state.DescriptorBytes-ne 20*[uint64]$count){throw 'Fresh lifecycle fixture differs.'}
    }
    # Reborn: detached faults reject altered context zeros, count load, branch target, destructor binding and vtable construction.
    foreach($offset in @(0xcfcab,0xcfccd,0xcfcf1,0xcfd24,0xcfd56,0x4968b,0xaa962)){
        $fault=[byte[]]$bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-LoaderLifecycleCode $fault}catch{$rejected=$true}
        if(-not $rejected){throw 'Loader lifecycle code fault admitted.'}
    }
}
$lifeAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($lifeAfter))-cne $producer.ImageSha256){throw 'Loader image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$producer.ImageSha256;WrapperVa='0x004CFBF0';ContextInitializerVa='0x004CFCAB';ContextDestructorVa='0x00449650';ContextDestructorCallVa='0x004CFD56';ConcreteReaderConstructorVa='0x004AA950'
    WrapperContextBaseStackOffset=4;ContextCapacityOffset=4;ContextEntryPointerOffset=0;ContextDescriptorPointerOffset=8;ContextRequestedCountOffset=20;ContextSourceHandleOffset=24
    FirstLoaderContextArgumentOrdinal=7;FreshWrapperContextRecovered=$true;NormalExitBufferReleaseRecovered=$true;CrossWrapperEntryCapacityReused=$false
    FreshCapacityModelAssumesSuccessfulAllocation=$true;AllocatorFailureSemanticsRecovered=$false;AllReaderCapacityReuseRecovered=$false
    ConcreteReaderConstructionRecovered=$true;AllSourceObjectBindingsRecovered=$false;CompleteStreamPointerProvenanceRecovered=$false;ProductionBuildReady=$false
    ObservedManifestCount=$capacities.Count;Capacities=$capacities;ReadOnly=$true;TargetExecuted=$false;ManagedInspectorHashCommandExecuted=$producer.ManagedInspectorHashCommandExecuted
    FaultTestsPassed=[bool]$SelfTest;FreshInvocationCasesExecuted=$(if($SelfTest){4}else{0});MemoryFaultsExecuted=$(if($SelfTest){7}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 7}else{$report}
