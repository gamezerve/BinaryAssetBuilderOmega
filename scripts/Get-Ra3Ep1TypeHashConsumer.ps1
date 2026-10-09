# Reborn: characterize a pinned runtime TypeHash comparison and diagnostic path, never convert observed engine skip branches into compiler admission policy.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$consumerSelfTest=$SelfTest; $consumerAsJson=$AsJson
$registry=. (Join-Path $PSScriptRoot 'Get-Ra3Ep1RegistryEvidence.ps1') -ImagePath $ImagePath
$SelfTest=$consumerSelfTest; $AsJson=$consumerAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: exact code snapshots pin reviewed field/branch interpretation without executing ResourceManager or decoding unreviewed instructions. #>
#-------------------------------------------------------------------------------------------------
function Assert-HashConsumerSlices([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0xab5c0;Length=96;Hash='7EFDD9891F63790733E8823E93132BEE7ABE9BE8013CD19FF497CC32429C4349'},
        [pscustomobject]@{Offset=0xab61e;Length=230;Hash='1C76143EF396CF4D9E9ED49BE34964FBE3C79E3403A0157D2D5F92861D866DF8'},
        [pscustomobject]@{Offset=0xcea54;Length=69;Hash='61B95C5C3AFDCB3933161AE2F29DFAA8B2FAE1C496EF5FB24EF349E9178DC476'},
        [pscustomobject]@{Offset=0xcf194;Length=69;Hash='C60813E03B61DB81BC9718806CD5C12B4E871E6BFFAE1DD242AAA666BDAE0731'},
        [pscustomobject]@{Offset=0xcf8ce;Length=50;Hash='BF9812B269DAACDFDE22B8D2709887E979DA49295D524847C2006300C8B0D533'}
    )) {
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Consumer slice outside image.'}
        $hash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))
        if($hash-cne $slice.Hash){throw 'Reviewed consumer code slice changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: model only the first observed branch decision; skip/match is not successful loading and a diagnostic is not proven unconditional rejection. #>
#-------------------------------------------------------------------------------------------------
function Get-ObservedHashDecision([bool] $MetadataPresent,[uint32] $EntryWord44,[uint32] $EntryHash,[uint32] $MetadataHash) {
    if(-not $MetadataPresent){return 'skip-no-metadata'}
    if($EntryWord44-ne 0){return 'skip-entry-word44-nonzero'}
    if($EntryHash-eq 0){return 'skip-zero-entry-hash'}
    if($EntryHash-eq $MetadataHash){return 'continue-equal-hash'}
    return 'enter-mismatch-diagnostic-path'
}
Assert-HashConsumerSlices $bytes
# Reborn: locate only raw E8 relative-call candidates in the pinned code region, then require the three separately reviewed call offsets.
$code=[Text.Encoding]::Latin1.GetString($bytes,0x1000,8196096)
$calls=@(foreach($match in [regex]::Matches($code,'(?s)\xE8.{4}')){
    $offset=$match.Index+0x1000
    if(0x400000+$offset+5+[BitConverter]::ToInt32($bytes,$offset+1)-eq 0x417340){'0x{0:X8}' -f $offset}
})
if(($calls -join ',') -cne '0x000CEA6B,0x000CF1AB,0x000CF8D8'){throw 'Reviewed direct lookup call-candidate set differs.'}
if($SelfTest){
    # Reborn: pure branch fixtures include noncanonical raw words to prevent silently treating characterization as manifest validation.
    foreach($case in @(
        [pscustomobject]@{Present=$false;Word=0;Entry=2;Runtime=1;Expected='skip-no-metadata'},
        [pscustomobject]@{Present=$true;Word=1;Entry=2;Runtime=1;Expected='skip-entry-word44-nonzero'},
        [pscustomobject]@{Present=$true;Word=7;Entry=2;Runtime=1;Expected='skip-entry-word44-nonzero'},
        [pscustomobject]@{Present=$true;Word=0;Entry=0;Runtime=1;Expected='skip-zero-entry-hash'},
        [pscustomobject]@{Present=$true;Word=0;Entry=1;Runtime=1;Expected='continue-equal-hash'},
        [pscustomobject]@{Present=$true;Word=0;Entry=2;Runtime=1;Expected='enter-mismatch-diagnostic-path'},
        [pscustomobject]@{Present=$true;Word=0;Entry=1;Runtime=0;Expected='enter-mismatch-diagnostic-path'}
    )) {if((Get-ObservedHashDecision $case.Present $case.Word $case.Entry $case.Runtime)-cne $case.Expected){throw 'Observed consumer branch differs.'}}
    # Reborn: detached instruction mutations must fail the exact code evidence pin rather than reinterpret the changed branch.
    foreach($offset in @(0xab615,0xab618,0xab600,0xab67c,0xcea6b)){
        $fault=[byte[]]$bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-HashConsumerSlices $fault}catch{$rejected=$true}
        if(-not $rejected){throw 'Consumer code fault admitted.'}
    }
}
$after=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($after))-cne $registry.ImageSha256){throw 'Consumer image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$registry.ImageSha256; ConsumerEntryVa='0x004AB5C0'; HashCompareVa='0x004AB615'; MismatchDiagnosticStartVa='0x004AB61E'; CommonContinuationVa='0x004AB704'
    ListHeadVa='0x00CEA428'; EntryTypeIdOffset=0; EntryInstanceIdOffset=4; EntryTypeHashOffset=8; EntryWord44Offset=44; MetadataHashOffset=8
    MismatchMessageVa='0x00BF2F50'; ExpectedHashReadVa='0x004AB6C0'; ActualEntryHashReadVa='0x004AB6E5'
    DirectLookupCallCandidateOffsets=$calls; DirectCallConsumersUseUnknownWord12=$true; DirectCallConsumersAreNotThisHashGate=$true
    ComparisonRecovered=$true; CompleteStreamPointerProvenanceRecovered=$false; UnconditionalMismatchRejectionProved=$false; DiagnosticControlSemanticsRecovered=$false
    EngineSkipBranchesAreCompilerPermission=$false; RuntimeConsumerIsAuthoringProcessingHash=$false; ProductionBuildReady=$false; ReadOnly=$true; TargetExecuted=$false
    ManagedInspectorHashCommandExecuted=$registry.ManagedInspectorHashCommandExecuted
    FaultTestsPassed=[bool]$SelfTest; BranchCasesExecuted=$(if($SelfTest){7}else{0}); MemoryFaultsExecuted=$(if($SelfTest){5}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 6}else{$report}
