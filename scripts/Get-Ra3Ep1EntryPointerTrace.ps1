# Reborn: pin a bounded entry-pointer/caller trace without promoting an unresolved upstream stream reader to a proven manifest parser.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$traceSelfTest=$SelfTest; $traceAsJson=$AsJson
$consumer=. (Join-Path $PSScriptRoot 'Get-Ra3Ep1TypeHashConsumer.ps1') -ImagePath $ImagePath
$SelfTest=$traceSelfTest; $AsJson=$traceAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: retain exact reviewed instruction regions for row selection, deferred pointer use, 48-byte copy, hash call and diagnostic delegation. #>
#-------------------------------------------------------------------------------------------------
function Assert-EntryTraceSlices([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0xce630;Length=40;Hash='4CDEECD2EF5F73CB4693AAA168F6E6FC452FBB4E492A0B14E599C2F17BF8BB5F'},
        [pscustomobject]@{Offset=0xce88f;Length=85;Hash='8B7A74058728BB3DB25D8BD8E54DA2EEDF29622016C2FB195F8939712B82BBE6'},
        [pscustomobject]@{Offset=0xce9cc;Length=30;Hash='F540EF3F38161E24975B6E8BD0FADE73C86F8826D96988A888C8B4FC4A1C5FA5'},
        [pscustomobject]@{Offset=0xceaf6;Length=62;Hash='38724C11909FA9143840559C0F561AA4575388310006F45EA5DBA2DE8ADFAB0D'},
        [pscustomobject]@{Offset=0xcebe7;Length=70;Hash='C391AD9E1BD1B994014F519544965F658C1A02A5A957507059FC2EA1D11B9BF0'},
        [pscustomobject]@{Offset=0xcec84;Length=97;Hash='B20EC2B398A556FCCA1F6E4A6253F46E5F8CFAA54630C10F63E002EDBFA9AADD'},
        [pscustomobject]@{Offset=0xedb0;Length=26;Hash='05D2F6D048EDD98FA3DB958279E0B9594EF6328363FAB6F981FBE28E2DF0CA98'}
    )) {
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Entry trace slice outside image.'}
        $digest=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))
        if($digest-cne $slice.Hash){throw 'Reviewed entry trace code changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: derive the entry argument from reviewed x86 stack adjustment and right-to-left pushes, not from field-name resemblance. #>
#-------------------------------------------------------------------------------------------------
function Get-TraceEntryArgument([int] $LocalBytes,[int] $SavedBeforeLoad,[int] $LoadDisplacement,[string[]] $PushOrder) {
    $entryEspOffset=$LoadDisplacement-$LocalBytes-4*$SavedBeforeLoad
    if($entryEspOffset-lt 4 -or $entryEspOffset%4-ne 0){throw 'Invalid reviewed entry argument displacement.'}
    $argument=$entryEspOffset/4
    if($argument-gt $PushOrder.Count){throw 'Reviewed entry argument exceeds pushes.'}
    return $PushOrder[$PushOrder.Count-$argument]
}
Assert-EntryTraceSlices $bytes
# Reborn: overlapping raw-call candidates are not a full call graph; exact reviewed E8 targets only constrain this pinned image.
$traceCode=[Text.Encoding]::Latin1.GetString($bytes,0x1000,8196096)
$traceCalls=@(foreach($match in [regex]::Matches($traceCode,'(?s)(?=\xE8(.{4}))')){
    $offset=$match.Index+0x1000
    if(0x400000+$offset+5+[BitConverter]::ToInt32($bytes,$offset+1)-eq 0x4ab5c0){'0x{0:X8}' -f $offset}
})
if(($traceCalls -join ',')-cne '0x000CECE0,0x000CF420,0x000CFB66'){throw 'Hash-gate direct-call candidate set differs.'}
if((Get-TraceEntryArgument 0x24 3 0x40 @('arg8','arg7','diagnostic-path','state-pointer','entry-esi','arg3','arg2','arg1'))-cne 'entry-esi'){throw 'Caller/callee entry pointer contract differs.'}
if($SelfTest){
    # Reborn: synthetic stack fixtures validate arithmetic only, never claim a live engine trace or dereference native pointers.
    if((Get-TraceEntryArgument 0 0 4 @('first'))-cne 'first'){throw 'First argument fixture differs.'}
    if((Get-TraceEntryArgument 8 2 24 @('third','second','first'))-cne 'second'){throw 'Adjusted argument fixture differs.'}
    foreach($displacement in @(0x31,0x54)){
        $rejected=$false
        try{$null=Get-TraceEntryArgument 0x24 3 $displacement @('arg1')}catch{$rejected=$true}
        if(-not $rejected){throw 'Invalid stack fixture admitted.'}
    }
    # Reborn: private code faults must fail exact evidence pins, including pointer source, copy count, call argument and virtual slot.
    foreach($offset in @(0xce63e,0xce8cd,0xce9df,0xceb1e,0xcebe7,0xcecd3,0xedbe)){
        $fault=[byte[]]$bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-EntryTraceSlices $fault}catch{$rejected=$true}
        if(-not $rejected){throw 'Entry trace code fault admitted.'}
    }
}
$traceAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($traceAfter))-cne $consumer.ImageSha256){throw 'Entry trace image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$consumer.ImageSha256; ReviewedLoaderVa='0x004CE630'; VersionWordOffset=4; RequiredVersionWord=7
    DescriptorArrayPointerOffset=8; DescriptorStride=20; DescriptorEntryPointerOffset=12; PendingStackRecordStride=28; PendingEntryPointerOffset=4
    ReviewedCopyBytes=48; CopyDwordCount=12; HashGateEntryArgumentOrdinal=4; HashGateEntryRegister='ESI'; DirectHashGateCallCandidates=$traceCalls
    EntryHashReadsRetainOffsets8And12=$true; Word44ControlsAdditionalBranches=$true
    DiagnosticWrapperVa='0x0040EDB0'; DiagnosticGlobalObjectVa='0x00CE959C'; DelegatedVtableByteOffset=100
    BoundedCallerPointerTraceRecovered=$true; CompleteStreamPointerProvenanceRecovered=$false; DescriptorArrayConstructionRecovered=$false
    AllThreeCallerPathsTraced=$false; DiagnosticWrapperDelegationRecovered=$true; DiagnosticControlSemanticsRecovered=$false
    Word44SchemaMeaningRecovered=$false; EngineSkipBranchesAreCompilerPermission=$false; ProductionBuildReady=$false
    ReadOnly=$true; TargetExecuted=$false; ManagedInspectorHashCommandExecuted=$consumer.ManagedInspectorHashCommandExecuted
    FaultTestsPassed=[bool]$SelfTest; StackPositiveCasesExecuted=$(if($SelfTest){3}else{1}); StackRejectionCasesExecuted=$(if($SelfTest){2}else{0}); MemoryFaultsExecuted=$(if($SelfTest){7}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 6}else{$report}
