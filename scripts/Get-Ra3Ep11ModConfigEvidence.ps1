# Reborn: audit one pinned EP1 1.1 command-line registration and config-path flow without admitting old offsets, executing the game or proving mod loading.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$modSelfTest=$SelfTest;$modAsJson=$AsJson
$metadata=. (Join-Path $PSScriptRoot 'Get-Ra3Ep1PeEvidence.ps1') -ImagePaths @($ImagePath)
$SelfTest=$modSelfTest;$AsJson=$modAsJson
$modBytes=Read-PeSnapshot $ImagePath
$modHash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($modBytes))
if($modHash-cne 'B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B'){throw 'Unreviewed EP1 1.1 identity; command-line offsets are not admitted.'}

#-------------------------------------------------------------------------------------------------
<# Reborn: pin the option literal/table row, complete handler/parser, table dispatch caller, setter and bounded downstream consumer slice. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11ModConfigCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0x823094;Length=11;Hash='F680C8832D92B9C2B212505E7CA88A33273F01979BE395584D35AB313B3AF14E'},
        [pscustomobject]@{Offset=0x823104;Length=8;Hash='D38EC59248EF1519023E4CF341910BC63B42934B9710AEA8CE522EF7C2B79A9F'},
        [pscustomobject]@{Offset=0x232180;Length=29;Hash='8CD73C47E90460BA34E4756F93809C7CC701FA6DD4DC2C1C30AE997E984C17DD'},
        [pscustomobject]@{Offset=0x2321a0;Length=230;Hash='21BE7E19BE142BE4DB5C9D7E2AA80BA073FB75F80E1607F9B49FF8A411336AFA'},
        [pscustomobject]@{Offset=0x2327e0;Length=46;Hash='3A1BAA71067CEAFBA6EE87D256E79E28668A08CC7EE3B86157D397B93E5586D0'},
        [pscustomobject]@{Offset=0xd6960;Length=25;Hash='BFD6A81766A3E8D6F7FD6E97AF12F47FB06FF9C50DAB621D8DD09C7BDB94D7DE'},
        [pscustomobject]@{Offset=0xd985e;Length=64;Hash='78C27B5EB6C5E5A9DCB4939A112B447206257171D8C5EBA9FF67BFAFB89E036B'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Modconfig slice outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed 1.1 modconfig evidence changed.'}
    }
}
Assert-Ep11ModConfigCode $modBytes
# Reborn: import names use this reviewed image's identity RVA/raw mapping; full identity is checked before fixed offsets are inspected.
if([BitConverter]::ToUInt32($modBytes,0x7d8594)-ne 0x8ba02c -or
   [Text.Encoding]::ASCII.GetString($modBytes,0x8ba02e,9)-cne "strcpy_s`0" -or
   [BitConverter]::ToUInt32($modBytes,0x7d8348)-ne 0x8badf2 -or
   [Text.Encoding]::ASCII.GetString($modBytes,0x8badf4,10)-cne "_strnicmp`0"){throw 'Reviewed modconfig import binding differs.'}
if($SelfTest){
    # Reborn: mutate private literal/table/dispatch/setter/consumer copies; no image or config file is modified.
    foreach($offset in @(0x823094,0x823108,0x23218f,0x232244,0x232809,0xd696a,0xd985e)){
        $fault=[byte[]]$modBytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11ModConfigCode $fault}catch{$rejected=$true}
        if(-not $rejected){throw 'Modconfig code fault admitted.'}
    }
}
$modAfter=Read-PeSnapshot $ImagePath
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($modAfter))-cne $modHash){throw 'EP1 1.1 image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$modHash;ImageBytes=$modBytes.Length;ImageVersionScope='EP1 1.1 pinned image';Option='-modconfig';OptionTableVa='0x00C230FC';OptionTableEntryCount=4;OptionEntryIndex=1
    HandlerVa='0x00632180';ParserVa='0x006321A0';ParserCallVa='0x00632809';PathSetterVa='0x004D6960';PathBufferVa='0x00CF2118';PathBufferCapacity=256
    OptionRegistrationRecovered=$true;ParserHandlerDispatchRecovered=$true;ConfigPathStorageRecovered=$true;NonemptyPathConsumerRecovered=$true
    CompleteConfigFileParserRecovered=$false;LauncherArgumentForwardingProven=$false;ModPackageLoaded=$false;Ep1ProcessingHashRecovered=$false;OldRuntimeOffsetsCompatible=$false
    ReadOnly=$true;TargetExecuted=$false;ProductionBuildReady=$false;FaultTestsPassed=[bool]$SelfTest;MemoryFaultsExecuted=$(if($SelfTest){7}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 6}else{$report}
