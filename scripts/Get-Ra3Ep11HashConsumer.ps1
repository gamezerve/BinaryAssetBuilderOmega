# Reborn: independently pin EP1 1.1 registry and TypeHash consumer bodies without reusing 1.0 offsets or treating engine skip branches as authoring permission.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$ep11ConsumerSelfTest=$SelfTest;$ep11ConsumerAsJson=$AsJson
$runtime11=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11RuntimeTypeTable.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath
$SelfTest=$ep11ConsumerSelfTest;$AsJson=$ep11ConsumerAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: pin independently reviewed registry insertion/lookup, hash comparison/diagnostic and three distinct opaque-word consumers in the 1.1 image. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11HashConsumerCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0xab880;Length=96;Hash='2421A8766269EDE315C48AF93EDD47AB5CAD03817E5CA119C69A9EA434835E2B'},
        [pscustomobject]@{Offset=0xab8de;Length=230;Hash='8AC663C230EEE087AEC40D5C70AA687A75279525610828CE238C27C55C39012C'},
        [pscustomobject]@{Offset=0x17370;Length=34;Hash='E0B701CA34CD8B61B559EC150ECBD7267449EA61E98DA48680CFB15D7F7DCD5B'},
        [pscustomobject]@{Offset=0x17430;Length=211;Hash='C7E40AC48AC8EDD6D342BBBCA658A10B8D0F90BBA6FEC0B1E6C790F52D9D3106'},
        [pscustomobject]@{Offset=0xced14;Length=69;Hash='AB9B5F41F3F4B707D9F4E7589C15AEA5D5EB364AB3A7C180DCD7FCF4503485ED'},
        [pscustomobject]@{Offset=0xcf454;Length=69;Hash='1B6CCD8075F7B3F784CF1F7561A35F168D58C1BFF68AC3FA98B64FD25208B827'},
        [pscustomobject]@{Offset=0xcfb8e;Length=50;Hash='E4B75876A83D4AA5127ACB22A3D8A577B0D03612D1F22C9F653B69A24BE966C4'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'EP1 1.1 consumer slice outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed EP1 1.1 hash consumer changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: characterize the first reviewed hash decision only; diagnostic entry is not an unconditional rejection and skip is not successful mod loading. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11ObservedHashDecision([bool] $MetadataPresent,[uint32] $EntryWord44,[uint32] $EntryHash,[uint32] $MetadataHash) {
    if(-not $MetadataPresent){return 'skip-no-metadata'}
    if($EntryWord44-ne 0){return 'skip-entry-word44-nonzero'}
    if($EntryHash-eq 0){return 'skip-zero-entry-hash'}
    if($EntryHash-eq $MetadataHash){return 'continue-equal-hash'}
    return 'enter-mismatch-diagnostic-path'
}

#-------------------------------------------------------------------------------------------------
<# Reborn: find overlapping raw E8 candidates only within the pinned .text extent; this scan is not a complete direct/indirect call graph. #>
#-------------------------------------------------------------------------------------------------
function Find-Ep11ConsumerCallCandidates([byte[]] $Bytes,[uint32] $TargetVa) {
    $code=[Text.Encoding]::Latin1.GetString($Bytes,0x1000,0x7d7000)
    return @(foreach($match in [regex]::Matches($code,'(?s)(?=\xE8(.{4}))')){
        $offset=$match.Index+0x1000
        if(0x400000+$offset+5+[BitConverter]::ToInt32($Bytes,$offset+1)-eq $TargetVa){'0x{0:X8}' -f $offset}
    })
}
Assert-Ep11HashConsumerCode $ep11Bytes
$ep11LookupCalls=@(Find-Ep11ConsumerCallCandidates $ep11Bytes 0x417370)
$ep11GateCalls=@(Find-Ep11ConsumerCallCandidates $ep11Bytes 0x4ab880)
if(($ep11LookupCalls -join ',')-cne '0x000CED2B,0x000CF46B,0x000CFB98' -or
   ($ep11GateCalls -join ',')-cne '0x000CEFA0,0x000CF6E0,0x000CFE26'){throw 'EP1 1.1 consumer call-candidate sets differ.'}
if($SelfTest){
    # Reborn: include noncanonical word seven and zero metadata hash to preserve exact observed characterization without schema validation or bypass permission.
    foreach($case in @(
        [pscustomobject]@{Present=$false;Word=0;Entry=2;Runtime=1;Expected='skip-no-metadata'},
        [pscustomobject]@{Present=$true;Word=1;Entry=2;Runtime=1;Expected='skip-entry-word44-nonzero'},
        [pscustomobject]@{Present=$true;Word=7;Entry=2;Runtime=1;Expected='skip-entry-word44-nonzero'},
        [pscustomobject]@{Present=$true;Word=0;Entry=0;Runtime=1;Expected='skip-zero-entry-hash'},
        [pscustomobject]@{Present=$true;Word=0;Entry=1;Runtime=1;Expected='continue-equal-hash'},
        [pscustomobject]@{Present=$true;Word=0;Entry=2;Runtime=1;Expected='enter-mismatch-diagnostic-path'},
        [pscustomobject]@{Present=$true;Word=0;Entry=1;Runtime=0;Expected='enter-mismatch-diagnostic-path'}
    )){if((Get-Ep11ObservedHashDecision $case.Present $case.Word $case.Entry $case.Runtime)-cne $case.Expected){throw 'EP1 1.1 hash decision fixture differs.'}}
    # Reborn: mutate detached snapshots at registry fields, hash compare, diagnostic reads and all three opaque-word consumer regions.
    foreach($offset in @(0x17380,0x174f6,0xab8c0,0xab8d5,0xab980,0xab9a5,0xced47,0xcf487,0xcfbb4)){
        $fault=[byte[]]$ep11Bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11HashConsumerCode $fault}catch{$rejected=$true}
        if(-not $rejected){throw 'EP1 1.1 consumer code fault admitted.'}
    }
}
$consumer11After=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($consumer11After))-cne $runtime11.ImageSha256){throw 'EP1 1.1 consumer image changed during review.'}
$ep11ConsumerReport=[pscustomobject]@{
    ImageSha256=$runtime11.ImageSha256;LookupVa='0x00417370';RegistrationVa='0x00417430';ListHeadVa='0x00CF1528';NodeAllocationBytes=16
    NodeNextOffset=0;NodeTypeIdOffset=4;NodeRepeatedTypeIdOffset=8;NodeMetadataPointerOffset=12;MetadataTypeIdOffset=4;MetadataHashOffset=8
    LookupUsesTypeId=$true;LookupConsumesTypeHash=$false;RegistrationRepeatsTypeIdNotHash=$true;RegistryBodyLayoutRecovered=$true
    ConsumerEntryVa='0x004AB880';HashCompareVa='0x004AB8D5';MismatchDiagnosticStartVa='0x004AB8DE';CommonContinuationVa='0x004AB9C4'
    EntryTypeIdOffset=0;EntryInstanceIdOffset=4;EntryTypeHashOffset=8;EntryWord44Offset=44;ExpectedHashReadVa='0x004AB980';ActualEntryHashReadVa='0x004AB9A5'
    DiagnosticObjectGlobalVa='0x00CF069C';MismatchMessageVa='0x00BF8F78';DiagnosticSourcePathVa='0x00BF8FC0'
    DirectLookupCallCandidateOffsets=$ep11LookupCalls;HashGateCallCandidateOffsets=$ep11GateCalls;DirectLookupConsumersUseOpaqueMetadataWord12=$true
    ComparisonRecovered=$true;InitializerToMetadataHashBindingRecovered=$false;CompleteStreamPointerProvenanceRecovered=$false;FullRegistryCoverageProved=$false
    UnconditionalMismatchRejectionProved=$false;DiagnosticControlSemanticsRecovered=$false;UnknownMetadataWord12SemanticsProved=$false
    EngineSkipBranchesAreCompilerPermission=$false;RuntimeConsumerIsAuthoringProcessingHash=$false;ModPackageLoaded=$false;ProductionBuildReady=$false;ReadOnly=$true;TargetExecuted=$false
    ManagedInspectorHashCommandExecuted=$false;FaultTestsPassed=[bool]$SelfTest;BranchCasesExecuted=$(if($SelfTest){7}else{0});MemoryFaultsExecuted=$(if($SelfTest){9}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $ep11ConsumerReport -Depth 7}else{$ep11ConsumerReport}
