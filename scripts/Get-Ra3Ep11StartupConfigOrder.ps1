# Reborn: recover explicitly scoped startup config ordering and fallback formats without claiming complete startup reachability or running the game.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[Parameter(Mandatory=$true)] [string] $SkuDefinitionPath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$orderSelfTest=$SelfTest;$orderAsJson=$AsJson
$pairs=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11SearchPathPairs.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath
$SelfTest=$orderSelfTest;$AsJson=$orderAsJson
$formats=@(
    [pscustomobject]@{Raw=8365464;Text='\mod.skudef'},
    [pscustomobject]@{Raw=8365404;Text='set-search-path big:;%s'},
    [pscustomobject]@{Raw=8365428;Text='set-search-path big:;%s\lang\%s;%s'},
    [pscustomobject]@{Raw=8365376;Text='add-search-path %s\lang\%s'},
    [pscustomobject]@{Raw=9195792;Text='filesystem.cfg'}
)

#-------------------------------------------------------------------------------------------------
<# Reborn: pin three reviewed startup slices and exact config format strings; these slices do not constitute a whole startup function. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11StartupConfigOrderCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0xd985e;Length=77;Hash='870C2332C1463EAAE55E0185E0F018FE9B454163CFA4464E1793D08FEC4CD899'},
        [pscustomobject]@{Offset=0xd99e1;Length=229;Hash='2BB503CBE4ADA0EAF1C5B14CE6F952F08A779E52FAFFABFCAC59DAB8B24BBE1B'},
        [pscustomobject]@{Offset=0xd9ac6;Length=215;Hash='4626FE76774E12E56147032D2A24C1621E94D6049A4F82CD73EFD4D2029C2A9E'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Startup-order slice outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed startup config order changed.'}
    }
    foreach($format in $formats){if([Text.Encoding]::ASCII.GetString($Bytes,$format.Raw,$format.Text.Length+1)-cne ($format.Text+[char]0)){throw 'Startup config format differs.'}}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: model supplied branch outcomes and candidate labels only, retaining reverse vector traversal and base-config-dependent synthetic directives without guessing path joins or I/O. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11DetachedStartupStages([bool] $ModProbeSucceeded,[string[]] $Candidates,[bool] $BaseReadSucceeded,[bool] $LanguageFlag) {
    if($Candidates.Count-gt 100){throw 'Detached startup candidate count exceeds policy.'}
    foreach($candidate in $Candidates){if($candidate.Length-eq 0 -or $candidate.Length-gt 200 -or $candidate-match '[^\x20-\x7E]'){throw 'Detached startup candidate outside ASCII policy.'}}
    $stages=[Collections.Generic.List[string]]::new()
    if($ModProbeSucceeded){$stages.Add('modconfig-read')}
    for($i=$Candidates.Count-1;$i-ge 0;$i--){$stages.Add('candidate-probe:'+ $Candidates[$i])}
    $stages.Add('filesystem.cfg-read')
    if(-not $BaseReadSucceeded){$stages.Add($(if($LanguageFlag){'fallback-set-search-path-language'}else{'fallback-set-search-path'}))}
    elseif($LanguageFlag){$stages.Add('add-search-path-language')}
    return $stages.ToArray()
}
Assert-Ep11StartupConfigOrderCode $ep11Bytes
if($SelfTest){
    # Reborn: ordering fixtures use supplied probe/read/flag outcomes, not actual file existence or native flag interpretation.
    foreach($fixture in @(
        [pscustomobject]@{Mod=$false;Candidates=@();Base=$true;Language=$false;Expected=@('filesystem.cfg-read')},
        [pscustomobject]@{Mod=$true;Candidates=@('a','b');Base=$true;Language=$false;Expected=@('modconfig-read','candidate-probe:b','candidate-probe:a','filesystem.cfg-read')},
        [pscustomobject]@{Mod=$false;Candidates=@('a');Base=$false;Language=$false;Expected=@('candidate-probe:a','filesystem.cfg-read','fallback-set-search-path')},
        [pscustomobject]@{Mod=$true;Candidates=@();Base=$false;Language=$true;Expected=@('modconfig-read','filesystem.cfg-read','fallback-set-search-path-language')},
        [pscustomobject]@{Mod=$false;Candidates=@('a','a');Base=$true;Language=$true;Expected=@('candidate-probe:a','candidate-probe:a','filesystem.cfg-read','add-search-path-language')},
        [pscustomobject]@{Mod=$true;Candidates=@();Base=$true;Language=$true;Expected=@('modconfig-read','filesystem.cfg-read','add-search-path-language')}
    )){
        $actual=@(Get-Ep11DetachedStartupStages $fixture.Mod $fixture.Candidates $fixture.Base $fixture.Language)
        if(($actual-join '|')-cne ($fixture.Expected-join '|')){throw 'Detached startup stage order differs.'}
    }
    foreach($bad in @(@(''),@([string][char]233),@('x'*201))){$rejected=$false;try{$null=Get-Ep11DetachedStartupStages $false $bad $true $false}catch{$rejected=$true};if(-not $rejected){throw 'Unsupported startup candidate admitted.'}}
    # Reborn: perturb only private code/literal byte copies at ordering, branch, vector stride and config dispatch sites.
    foreach($offset in @(0xd9865,0xd9891,0xd99e1,0xd9a1e,0xd9a96,0xd9aa2,0xd9ab3,0xd9abb,0xd9ae5,0xd9aef,0xd9b16,0xd9b47,0xd9b5c,0xd9b8c,8365464,8365404,8365428,8365376,9195792)){
        $fault=[byte[]]$ep11Bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11StartupConfigOrderCode $fault}catch{$rejected=$true};if(-not $rejected){throw 'Startup config byte fault admitted.'}
    }
}
$orderAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($orderAfter))-cne $pairs.ImageSha256){throw 'Startup-order image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$pairs.ImageSha256;ReviewedWholeBodyPins=0;ReviewedStartupSlicePins=3;ReviewedConfigStringPins=5
    ModConfigBufferVa='0x00CF2118';AdditionalConfigVectorBeginVa='0x00CF499C';AdditionalConfigVectorEndVa='0x00CF49A0';AdditionalConfigVectorStride=112
    ModConfigReadIsProbeConditional=$true;AdditionalConfigCandidatesTraverseReverse=$true;AdditionalCandidateReadIsProbeConditional=$true;BaseConfigReadFollowsAdditionalCandidates=$true;BaseConfigName='filesystem.cfg'
    BaseReadFailureSynthesizesSetSearchPath=$true;BaseReadSuccessCanAppendLanguagePath=$true;SyntheticDirectivesUseLineDispatcherVa='0x004D9040';ConfigReadVa='0x004D86B0';ProbeVa='0x004D6F10'
    Formats=@($formats|ForEach-Object Text);ScopedStartupConfigOrderRecovered=$true;FullStartupReachabilityRecovered=$false;ActualAdditionalCandidatePathsRecovered=$false;LanguageFlagMeaningFullyRecovered=$false;ConfigOverridesRemainEffectiveAfterLaterReadsProven=$false;ActualStartupMountOrderRecovered=$false
    StockArchiveCount=$pairs.StockArchiveCount;StockEntryCount=$pairs.StockEntryCount;ArchiveDirectoryMetadataBytesRead=$pairs.ArchiveDirectoryMetadataBytesRead;ArchivePayloadBytesRead=0
    ConfiguredStockArchiveSetComplete=$pairs.ConfiguredStockArchiveSetComplete;MissingConfiguredArchives=$pairs.MissingConfiguredArchives
    ReadOnly=$true;TargetExecuted=$false;ModPackageLoaded=$false;AuthenticEp1ProcessingHashRecovered=$false;ProductionBuildReady=$false
    FaultTestsPassed=[bool]$SelfTest;OrderingFixturesExecuted=$(if($SelfTest){6}else{0});PolicyRejectionsExecuted=$(if($SelfTest){3}else{0});MemoryFaultsExecuted=$(if($SelfTest){19}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 8}else{$report}
