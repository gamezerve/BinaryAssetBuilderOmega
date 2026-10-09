# Reborn: pin the independently disassembled 1.1 descriptor-to-gate path without claiming upstream stream reads or all caller paths.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$ep11TraceSelfTest=$SelfTest;$ep11TraceAsJson=$AsJson
$ep11TraceConsumer=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11HashConsumer.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath
$SelfTest=$ep11TraceSelfTest;$AsJson=$ep11TraceAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: retain exact instruction-boundary slices located from the reviewed 1.1 gate call, not a presumed version address delta. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11EntryTraceCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0xce8f0;Length=40;Hash='4CDEECD2EF5F73CB4693AAA168F6E6FC452FBB4E492A0B14E599C2F17BF8BB5F'},
        [pscustomobject]@{Offset=0xceb4f;Length=85;Hash='8B7A74058728BB3DB25D8BD8E54DA2EEDF29622016C2FB195F8939712B82BBE6'},
        [pscustomobject]@{Offset=0xcec8c;Length=30;Hash='385D535BA5D597248992B4DE6025EDAA7B7033164F2740ADD1FCFF6581EB01E0'},
        [pscustomobject]@{Offset=0xcedcf;Length=39;Hash='F600D298F6D614E32020BF62D173E66C402F3DFCAB1DD528F3BDBD5C83C41271'},
        [pscustomobject]@{Offset=0xcef44;Length=97;Hash='3729F410E166836A4EE99FE236B87E5A57650F9E54B17CA06D3B9F832CD0F354'},
        [pscustomobject]@{Offset=0xab880;Length=96;Hash='2421A8766269EDE315C48AF93EDD47AB5CAD03817E5CA119C69A9EA434835E2B'}
    )) {
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'EP1 1.1 entry trace slice outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed EP1 1.1 entry trace changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: map reviewed x86 stack adjustment to the caller push sequence; fixtures model arithmetic, not a running engine. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11TraceArgument([int] $LocalBytes,[int] $SavedBeforeLoad,[int] $LoadDisplacement,[string[]] $PushOrder) {
    if($LocalBytes-lt 0 -or $SavedBeforeLoad-lt 0){throw 'Negative stack adjustment.'}
    $entryOffset=$LoadDisplacement-$LocalBytes-4*$SavedBeforeLoad
    if($entryOffset-lt 4 -or $entryOffset%4-ne 0){throw 'Invalid entry argument displacement.'}
    $ordinal=$entryOffset/4
    if($ordinal-gt $PushOrder.Count){throw 'Entry argument exceeds reviewed pushes.'}
    return $PushOrder[$PushOrder.Count-$ordinal]
}
Assert-Ep11EntryTraceCode $ep11Bytes
$ep11TracePushes=@('arg8','arg7','diagnostic-path','state-pointer','entry-esi','arg3','arg2','arg1')
if((Get-Ep11TraceArgument 0x24 3 0x40 $ep11TracePushes)-cne 'entry-esi'){throw 'EP1 1.1 caller/callee argument differs.'}
if($SelfTest){
    # Reborn: exercise normal and adjusted stacks plus malformed displacement/count inputs without dereferencing native pointers.
    if((Get-Ep11TraceArgument 0 0 4 @('first'))-cne 'first' -or
       (Get-Ep11TraceArgument 8 2 24 @('third','second','first'))-cne 'second'){throw 'Stack fixture differs.'}
    foreach($case in @(@(0x24,3,0x31),@(0x24,3,0x54),@(-1,0,4),@(0,-1,4))){
        $rejected=$false
        try{$null=Get-Ep11TraceArgument $case[0] $case[1] $case[2] @('first')}catch{$rejected=$true}
        if(-not $rejected){throw 'Malformed stack fixture admitted.'}
    }
    # Reborn: flip version, descriptor stride/pointer, copy count, pending pointer, entry push and callee load in private snapshots only.
    foreach($offset in @(0xce900,0xceb83,0xceb8f,0xceca0,0xceddf,0xcef93,0xab890)){
        $fault=[byte[]]$ep11Bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11EntryTraceCode $fault}catch{$rejected=$true}
        if(-not $rejected){throw 'Entry trace code fault admitted.'}
    }
}
$ep11TraceAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($ep11TraceAfter))-cne $ep11TraceConsumer.ImageSha256){throw 'EP1 1.1 image changed during trace.'}
$ep11TraceReport=[pscustomobject]@{
    ImageSha256=$ep11TraceConsumer.ImageSha256;ReviewedLoaderVa='0x004CE8F0';VersionWordOffset=4;RequiredVersionWord=7
    DescriptorSelectionVa='0x004CEB80';DescriptorArrayPointerOffset=8;DescriptorStride=20;DescriptorEntryPointerOffset=12
    PendingStackRecordStride=28;PendingEntryPointerOffset=4;PendingPointerReloadVa='0x004CEDDE'
    CopyStartVa='0x004CEC8C';ReviewedCopyBytes=48;CopyDwordCount=12;EntryPushVa='0x004CEF93';ReviewedGateCallVa='0x004CEFA0'
    GateEntryVa=$ep11TraceConsumer.ConsumerEntryVa;HashGateEntryArgumentOrdinal=4;HashGateEntryRegister='ESI';EntryTypeHashOffset=8
    DirectHashGateCallCandidates=$ep11TraceConsumer.HashGateCallCandidateOffsets;ReviewedCallerCount=1
    DescriptorToGatePointerPathRecovered=$true;StreamEntryReadRecovered=$false;DescriptorConstructionRecovered=$false
    CompleteStreamPointerProvenanceRecovered=$false;AllGateCallerPathsRecovered=$false;FullRegistryCoverageProved=$false
    RuntimeConsumerIsAuthoringProcessingHash=$false;EngineSkipBranchesAreCompilerPermission=$false;ModPackageLoaded=$false;ProductionBuildReady=$false
    TargetExecuted=$false;ManagedInspectorHashCommandExecuted=$false;ReadOnly=$true;FaultTestsPassed=[bool]$SelfTest
    ValidStackFixturesExecuted=$(if($SelfTest){3}else{0});MalformedStackFixturesExecuted=$(if($SelfTest){4}else{0});MemoryFaultsExecuted=$(if($SelfTest){7}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $ep11TraceReport -Depth 7}else{$ep11TraceReport}
