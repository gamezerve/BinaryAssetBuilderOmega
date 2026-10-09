# Reborn: separate the preserved eighth loader flag from the added ninth argument without assigning unproved runtime semantics.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$argSelfTest=$SelfTest;$argAsJson=$AsJson
$argLife=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11LoaderLifecycle.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath
$SelfTest=$argSelfTest;$AsJson=$argAsJson
$argBaselineBytes=Read-RuntimeInput $BaselineImagePath 16777216
$argBaselineSha='ABA6A8825A744A78487837D78AD5992962257CE68404F639D600F11DEEA2580B'
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($argBaselineBytes))-cne $argBaselineSha){throw 'Argument audit baseline image differs.'}

#-------------------------------------------------------------------------------------------------
<# Reborn: pin both version-specific wrapper contracts and bounded eighth-flag branches/forwarding, not a guessed ninth-argument consumer. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11ArgumentCode([byte[]] $Current,[byte[]] $Baseline) {
    foreach($slice in @(
        [pscustomobject]@{Bytes=$Current;Offset=0xcff5c;Length=205;Hash='2AA453BC67F93DD4A28BAD0FE41542AE1DF3DE16687F954A037EE2AF852592E0'},
        [pscustomobject]@{Bytes=$Current;Offset=0xcec20;Length=43;Hash='CEEB4844F7BAD0A2EBFA4C0146CA28D64B1C7CC1F3C5E5BA57F6841B93752F8A'},
        [pscustomobject]@{Bytes=$Current;Offset=0xcf360;Length=43;Hash='33A7A323D23641703A70463300CC547B06DDAE2BD2283786C603D62B0EFB2367'},
        [pscustomobject]@{Bytes=$Current;Offset=0xcef5c;Length=59;Hash='BADF2F8B1D1C28C861AEAC09B6B29BB6A4FF8C8BA04783FC767322A1CA3C9435'},
        [pscustomobject]@{Bytes=$Baseline;Offset=0xcfc9c;Length=205;Hash='9403EC39651F48BC1A77C7E0C1265024E6095DD2D1651381AEFF4DB972007556'},
        [pscustomobject]@{Bytes=$Baseline;Offset=0xce960;Length=14;Hash='9B1C3D6EA52B79BA29B36C92036E2CC6240D25A126203BE6F4ADDFA8200922F9'}
    )) {
        if($slice.Offset+$slice.Length-gt $slice.Bytes.Length){throw 'Argument evidence outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$slice.Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed loader argument bytes changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: account explicitly for locals, saved registers and temporary pushes before converting an x86 stack load to an entry argument ordinal. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11StackOrdinal([int] $Locals,[int] $Saved,[int] $TemporaryPushes,[int] $Displacement) {
    if($Locals-lt 0 -or $Saved-lt 0 -or $TemporaryPushes-lt 0){throw 'Negative stack adjustment.'}
    $offset=[long]$Displacement-$Locals-4L*$Saved-4L*$TemporaryPushes
    if($offset-lt 4 -or $offset%4-ne 0){throw 'Invalid stack argument position.'}
    return [int]($offset/4)
}
Assert-Ep11ArgumentCode $ep11Bytes $argBaselineBytes
# Reborn: ninth loader argument is wrapper argument six; the eighth is the existing flag, including its one-push-adjusted forwarding to gate argument seven.
if((Get-Ep11StackOrdinal 0x60 1 0 0x7c)-ne 6 -or (Get-Ep11StackOrdinal 0x90 4 0 0xc0)-ne 8 -or
   (Get-Ep11StackOrdinal 0x90 4 1 0xc4)-ne 8 -or
   (Get-Ep11TraceArgument 0 0 28 @('gate8','loader8','gate6','gate5','gate4','gate3','gate2','gate1'))-cne 'loader8'){throw 'Loader/gate argument mapping differs.'}
if($SelfTest){
    # Reborn: distinguish an unadjusted C4h ninth argument from the same displacement after one temporary push, and reject invalid stack models.
    if((Get-Ep11StackOrdinal 0x90 4 0 0xc4)-ne 9 -or (Get-Ep11StackOrdinal 0 0 0 4)-ne 1){throw 'Stack ordinal fixture differs.'}
    foreach($case in @(@(-1,0,0,4),@(0,-1,0,4),@(0,0,-1,4),@(0,0,0,3))){
        $rejected=$false;try{$null=Get-Ep11StackOrdinal $case[0] $case[1] $case[2] $case[3]}catch{$rejected=$true}
        if(-not $rejected){throw 'Invalid stack model admitted.'}
    }
    # Reborn: mutate detached images at the added push/source, preserved flag, forwarding and both baseline contract regions only.
    foreach($offset in @(0xcffa7,0xcffab,0xcffb0,0xcffe3,0xcec23,0xcf363,0xcef6f,0xcef73)){
        $fault=[byte[]]$ep11Bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11ArgumentCode $fault $argBaselineBytes}catch{$rejected=$true}
        if(-not $rejected){throw 'Current argument fault admitted.'}
    }
    foreach($offset in @(0xcfcd5,0xce963)){
        $fault=[byte[]]$argBaselineBytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11ArgumentCode $ep11Bytes $fault}catch{$rejected=$true}
        if(-not $rejected){throw 'Baseline argument fault admitted.'}
    }
}
# Reborn: re-pin both versioned input snapshots at completion; no source or native executable is modified or run.
foreach($spec in @(@($ImagePath,$argLife.ImageSha256),@($BaselineImagePath,$argBaselineSha))){
    $after=Read-RuntimeInput $spec[0] 16777216
    if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($after))-cne $spec[1]){throw 'Argument image changed during audit.'}
}
$argReport=[pscustomobject]@{
    ImageSha256=$argLife.ImageSha256;BaselineImageSha256=$argBaselineSha;CurrentLoaderArguments=9;BaselineLoaderArguments=8
    AddedArgumentOrdinal=9;AddedArgumentSourceWrapperOrdinal=6;FirstAddedPushVa='0x004CFFAB';SecondAddedPushVa='0x004CFFE7'
    PreservedFlagOrdinal=8;FirstBranchFlagValue=1;SecondBranchFlagValue=0;FlagWasAlreadyPresentInBaseline=$true
    FirstFlagBranchVa='0x004CEC20';SecondFlagBranchVa='0x004CF360';FlagForwardingLoadVa='0x004CEF6C';FlagForwardedGateOrdinal=7
    NinthArgumentSourceRecovered=$true;EighthFlagOrdinalAndForwardingRecovered=$true;NinthArgumentConsumerRecovered=$false
    CompleteFlagSemanticsRecovered=$false;FlagIsTypeHashBypassPermission=$false;AllLoaderCallersRecovered=$false;LiveFactoryBindingRecovered=$false
    CompleteStreamPointerProvenanceRecovered=$false;ProductionBuildReady=$false;ModPackageLoaded=$false;ReadOnly=$true;TargetExecuted=$false
    ManagedInspectorHashCommandExecuted=$false;FaultTestsPassed=[bool]$SelfTest;PositiveStackChecks=$(if($SelfTest){6}else{4});InvalidStackFixtures=$(if($SelfTest){4}else{0});MemoryFaultsExecuted=$(if($SelfTest){10}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $argReport -Depth 7}else{$argReport}
