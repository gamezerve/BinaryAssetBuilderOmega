# Reborn: distinguish EP1 1.1 config path/probe/read/line-dispatch stages using exact static pins and bounded detached ASCII prefix fixtures.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$configSelfTest=$SelfTest;$configAsJson=$AsJson
$option=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11ModConfigEvidence.ps1') -ImagePath $ImagePath
$SelfTest=$configSelfTest;$AsJson=$configAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: pin complete path join, file probe, reader, line splitter and directive dispatcher plus the top-level gate and literal block. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11ConfigConsumerCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0xd69a0;Length=209;Hash='C97162AB160538037FCE33BBBA756BBC583BCC86B950DE300010068D47E017E7'},
        [pscustomobject]@{Offset=0xd6dc0;Length=329;Hash='6A31AC4DE0B3F2E28B18698FE298B73915C47B127D81415430F2A1E6F1B3177E'},
        [pscustomobject]@{Offset=0xd6f10;Length=227;Hash='D72620817E33865C3BD771B94F6D7E1BEA4FA3DFDF23353EDF2BD0EC5BC609FC'},
        [pscustomobject]@{Offset=0xd86b0;Length=169;Hash='0EE0C54E6C53825FCFF4A43DE01C0B30A7C0ACF3BBF1FA888764C8B118742FFE'},
        [pscustomobject]@{Offset=0xd9040;Length=287;Hash='39D04C96658A634EA0A5D965A2B4EB1B0B5ED68CDEE041A1314C51F9D8192EEE'},
        [pscustomobject]@{Offset=0xd8de0;Length=596;Hash='FCE40940584FADA7E0E3787BF01F52F69BCF145D016ED3360A4D803C684EBAE0'},
        [pscustomobject]@{Offset=0xd985e;Length=77;Hash='870C2332C1463EAAE55E0185E0F018FE9B454163CFA4464E1793D08FEC4CD899'},
        [pscustomobject]@{Offset=0x7f9cb0;Length=140;Hash='39B8639255C42277EDD7AC776546D282EAE417919DC2E65F9669412D81753780'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Config consumer slice outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed config consumer changed.'}
    }
}
$directives=@(
    [pscustomobject]@{Prefix='add-big ';LiteralRaw=0x7f9cb0;RouteVa='0x00995160';Mode=2},
    [pscustomobject]@{Prefix='add-bigs ';LiteralRaw=0x7f9cbc;RouteVa='0x004D8CC0';Mode=0},
    [pscustomobject]@{Prefix='add-bigs-recurse ';LiteralRaw=0x7f9cc8;RouteVa='0x004D8CC0';Mode=1},
    [pscustomobject]@{Prefix='set-search-path ';LiteralRaw=0x7f9cdc;RouteVa='0x004D7000';Mode=$null},
    [pscustomobject]@{Prefix='add-config ';LiteralRaw=0x7f9d04;RouteVa='0x004D86B0';Mode=$null},
    [pscustomobject]@{Prefix='try-add-config ';LiteralRaw=0x7f9d10;RouteVa='0x004D86B0';Mode=$null},
    [pscustomobject]@{Prefix='add-search-path ';LiteralRaw=0x7f9cf0;RouteVa='0x0096A660';Mode=$null},
    [pscustomobject]@{Prefix='add-str ';LiteralRaw=0x7f9d20;RouteVa='0x004D87B0';Mode=$null},
    [pscustomobject]@{Prefix='add-manifest ';LiteralRaw=0x7f9d2c;RouteVa='0x004D87B0';Mode=$null}
)
Assert-Ep11ConfigConsumerCode $modBytes
foreach($directive in $directives){
    if([Text.Encoding]::ASCII.GetString($modBytes,$directive.LiteralRaw,$directive.Prefix.Length+1)-cne ($directive.Prefix+[char]0)){throw 'Config directive literal differs.'}
}
#-------------------------------------------------------------------------------------------------
<# Reborn: characterize only ASCII directive-prefix dispatch on already-trimmed detached lines; do not emulate native file I/O, CRT locale, quoting or mounting. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11DetachedConfigRoute([string] $Line) {
    if($null-eq $Line -or $Line.Length-gt 1023 -or $Line.Contains([char]0) -or $Line-match '[^\x00-\x7F]'){throw 'Detached config fixture outside ASCII diagnostic policy.'}
    foreach($directive in $directives){
        if($Line.StartsWith($directive.Prefix,[StringComparison]::OrdinalIgnoreCase)){
            return [pscustomobject]@{Prefix=$directive.Prefix;Argument=$Line.Substring($directive.Prefix.Length);RouteVa=$directive.RouteVa}
        }
    }
    return [pscustomobject]@{Prefix=$null;Argument=$null;RouteVa=$null}
}
if($SelfTest){
    # Reborn: test the observed nine prefixes plus ASCII case folding and a missing required literal space; these are not native parser executions.
    foreach($directive in $directives){$fixture=Get-Ep11DetachedConfigRoute ($directive.Prefix+'fixture');if($fixture.Prefix-cne $directive.Prefix -or $fixture.Argument-cne 'fixture' -or $fixture.RouteVa-cne $directive.RouteVa){throw 'Detached directive route differs.'}}
    if((Get-Ep11DetachedConfigRoute 'ADD-BIG fixture').Prefix-cne 'add-big ' -or $null-ne (Get-Ep11DetachedConfigRoute 'add-big').RouteVa){throw 'Detached prefix boundary differs.'}
    foreach($line in @(('x'*1024),"add-big a`0b",'add-big é')){
        $rejected=$false;try{$null=Get-Ep11DetachedConfigRoute $line}catch{$rejected=$true};if(-not $rejected){throw 'Detached config diagnostic policy fault admitted.'}
    }
    # Reborn: private byte faults cover probe/read distinction, line dispatch, command branches, literal block and top-level continuation.
    foreach($offset in @(0xd69d1,0xd6f41,0xd6fb6,0xd6e9e,0xd86ed,0xd8723,0xd8734,0xd9152,0xd8e2c,0xd8e58,0xd8e88,0xd8eb6,0xd8ef3,0xd8f4d,0xd8fbd,0xd901d,0x7f9cb0,0xd98a6)){
        $fault=[byte[]]$modBytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11ConfigConsumerCode $fault}catch{$rejected=$true};if(-not $rejected){throw 'Config consumer byte fault admitted.'}
    }
}
$configAfter=Read-PeSnapshot $ImagePath
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($configAfter))-cne $option.ImageSha256){throw 'Config consumer image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$option.ImageSha256;ReviewedPins=8;DirectiveCount=9;Directives=$directives
    PathJoinVa='0x004D69A0';PathBufferCapacity=256;FileProbeVa='0x004D6F10';SinglePathProbeVa='0x004D6DC0';ConfigReadVa='0x004D86B0';LineSplitterVa='0x004D9040';DirectiveDispatchVa='0x004D8DE0'
    ProbeIsDirectiveParser=$false;TopLevelProbeThenConfigReadRecovered=$true;ReadDispatchesWholeNullTerminatedBuffer=$true;NativeReadResultChecked=$false;NativeAllocationFailureHandled=$false
    NativeLineDiagnosticThreshold=1024;LineLengthDiagnosticIsUnconditionalRejection=$false;ConfigReadSuccessMeansAllCommandsSucceeded=$false
    AddConfigRecursiveReadRecovered=$true;TryAddConfigUsesFileProbe=$true;AddManifestAppendsToGlobalListVa='0x00CF2354';AddStrAppendsToGlobalListVa='0x00CF2364'
    FullBigMountSemanticsRecovered=$false;FullManifestQueueConsumptionRecovered=$false;RecursiveConfigCycleSafetyProven=$false;NativeLocaleAndQuotingRecovered=$false
    ModConfigDispatchSupported=$true;ProductionBuildReady=$false;ModPackageLoaded=$false;ReadOnly=$true;TargetExecuted=$false
    FaultTestsPassed=[bool]$SelfTest;PrefixFixturesExecuted=$(if($SelfTest){11}else{0});InvalidFixturePoliciesExecuted=$(if($SelfTest){3}else{0});MemoryFaultsExecuted=$(if($SelfTest){18}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 7}else{$report}
