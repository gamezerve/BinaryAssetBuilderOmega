# Reborn: recover scoped provider-prefix routing and BIG registration through pinned code, static vtables and detached provider lists, without startup execution.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[Parameter(Mandatory=$true)] [string] $SkuDefinitionPath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$routingSelfTest=$SelfTest;$routingAsJson=$AsJson
$selection=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11BigOpenSelection.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath
$SelfTest=$routingSelfTest;$AsJson=$routingAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: pin reviewed routing/append/name/constructor bodies, the explicitly partial startup slice and two four-slot static vtables; do not count the slice as a whole startup function. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11ProviderRoutingCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0x56a050;Length=372;Hash='FC167FF9A83526E6036F76C03A68FBD8B8289DA7978ED2AD6538072CC7438AA9'},
        [pscustomobject]@{Offset=0x56a8a0;Length=98;Hash='72A924DD1B334B7BD0B0C21C6F194F051B0E55BACD23843BC7E7975CDF517432'},
        [pscustomobject]@{Offset=0x569da0;Length=70;Hash='F7968932D88A9B90B9F088F82843487D794A1E652CF2167657E8D814F415B11B'},
        [pscustomobject]@{Offset=0x569c60;Length=99;Hash='71CAB34A208C4DED38E51E84550C7022B46E7AF2EED48911F51EF770EEFAFCAD'},
        [pscustomobject]@{Offset=0x595690;Length=43;Hash='975FAE2F2F3553DBA2E1748C9AD62551A4E475103208224FD7152C1ADA4007F6'},
        [pscustomobject]@{Offset=0x569f80;Length=36;Hash='AA1AD7FBD0502B8BEE8CAC627DEAE1D07478F36F83C88F58A3B44CBE47885CE0'},
        [pscustomobject]@{Offset=0x569df0;Length=113;Hash='DF7C16AFACE38FC941013B21E54B0E74283DCF6E236D7785ED08BB21EC2C198E'},
        [pscustomobject]@{Offset=0xd980c;Length=43;Hash='28F54A5EC7C85E5FD5B113F1104B8DBA76AB771A1EACAC8993A16FE6AA916004'},
        [pscustomobject]@{Offset=0x7f9d78;Length=16;Hash='538E42A0920199500D87E4A523889C4245DE00F159897445EC9BFF34F36E61AD'},
        [pscustomobject]@{Offset=0x884e60;Length=16;Hash='0E516A79AB5FF6B9C18313CCD2D17064248B4C02B3A3D5E3FCC59E854BEAEB74'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Provider routing slice outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed provider routing evidence changed.'}
    }
    if([Text.Encoding]::ASCII.GetString($Bytes,0x87f1d0,6)-cne ('null:'+[char]0)){throw 'Builtin default provider alias differs.'}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: model printable ASCII, already-qualified prefix selection only; null-interface nodes terminate the native scan, and unprefixed paths select the supplied default. #>
#-------------------------------------------------------------------------------------------------
function Select-Ep11DetachedProvider([string] $Path,[object[]] $Providers,[object] $Default) {
    if($Path.Length-gt 255){throw 'Detached provider path exceeds policy bound.'}
    foreach($character in $Path.ToCharArray()){if([int]$character-lt 32 -or [int]$character-gt 126){throw 'Detached provider path requires printable ASCII.'}}
    $colon=$Path.IndexOf(':')
    if($colon-lt 0){return [pscustomobject]@{Selected=$Default;Prefix=$null;Visited=0;NullInterfaceStopped=$false}}
    $prefix=$Path.Substring(0,$colon+1);$visited=0
    foreach($provider in $Providers){
        $visited++
        if(-not $provider.HasInterface){return [pscustomobject]@{Selected=$null;Prefix=$prefix;Visited=$visited;NullInterfaceStopped=$true}}
        foreach($alias in $provider.Aliases){if([string]::Equals($prefix,$alias,[StringComparison]::OrdinalIgnoreCase)){return [pscustomobject]@{Selected=$provider.Id;Prefix=$prefix;Visited=$visited;NullInterfaceStopped=$false}}}
    }
    return [pscustomobject]@{Selected=$null;Prefix=$prefix;Visited=$visited;NullInterfaceStopped=$false}
}
Assert-Ep11ProviderRoutingCode $ep11Bytes
if($SelfTest){
    # Reborn: detached aliases cover default selection, first matching provider, ASCII case, unknown/drive prefixes and null-interface early termination.
    $a=[pscustomobject]@{Id='first';HasInterface=$true;Aliases=@('big:','archive:')}
    $b=[pscustomobject]@{Id='second';HasInterface=$true;Aliases=@('big:','other:')}
    foreach($fixture in @(
        [pscustomobject]@{Path='data/file';Expected='default';Visited=0;Stopped=$false;Nodes=@($a,$b);Default='default'},
        [pscustomobject]@{Path='BIG:/file';Expected='first';Visited=1;Stopped=$false;Nodes=@($a,$b);Default='default'},
        [pscustomobject]@{Path='archive:file';Expected='first';Visited=1;Stopped=$false;Nodes=@($a,$b);Default='default'},
        [pscustomobject]@{Path='other:file';Expected='second';Visited=2;Stopped=$false;Nodes=@($a,$b);Default='default'},
        [pscustomobject]@{Path='unknown:file';Expected=$null;Visited=2;Stopped=$false;Nodes=@($a,$b);Default='default'},
        [pscustomobject]@{Path='D:\file';Expected=$null;Visited=2;Stopped=$false;Nodes=@($a,$b);Default='default'},
        [pscustomobject]@{Path='big:file';Expected=$null;Visited=1;Stopped=$true;Nodes=@([pscustomobject]@{Id='null';HasInterface=$false;Aliases=@()},$b);Default='default'},
        [pscustomobject]@{Path='';Expected=$null;Visited=0;Stopped=$false;Nodes=@($a,$b);Default=$null}
    )){
        $result=Select-Ep11DetachedProvider $fixture.Path $fixture.Nodes $fixture.Default
        if($result.Selected-cne $fixture.Expected -or $result.Visited-ne $fixture.Visited -or $result.NullInterfaceStopped-ne $fixture.Stopped){throw 'Detached provider prefix fixture differs.'}
    }
    # Reborn: reject unsupported query representations and damage only private copies of the native/static evidence.
    foreach($path in @(([string][char]0),([string][char]233),('x'*256))){$rejected=$false;try{$null=Select-Ep11DetachedProvider $path @() $null}catch{$rejected=$true};if(-not $rejected){throw 'Unsupported provider path admitted.'}}
    foreach($offset in @(0x56a077,0x56a176,0x56a190,0x56a197,0x56a1b9,0x56a8ce,0x56a8e7,0x56a8fa,0x569db9,0x569ddc,0x569c84,0x595698,0x569f99,0x569e54,0xd9813,0xd9818,0xd9832,0x7f9d84,0x884e6c,0x87f1d0)){
        $fault=[byte[]]$ep11Bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11ProviderRoutingCode $fault}catch{$rejected=$true};if(-not $rejected){throw 'Provider routing byte fault admitted.'}
    }
}
$routingAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($routingAfter))-cne $selection.ImageSha256){throw 'Provider routing image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$selection.ImageSha256;RoutingVa='0x0096A050';RegistrationVa='0x0096A8A0';AliasAppendVa='0x00969DA0';AliasCompareVa='0x00969C60'
    ReviewedWholeBodyPins=7;ReviewedStartupSlicePins=1;ReviewedVtableSlicePins=2;ReviewedLiteralPins=1
    PrefixIncludesFirstColon=$true;AliasAsciiCaseInsensitive=$true;FirstMatchingRegisteredProviderWins=$true;UnknownPrefixReturnsNull=$true;UnprefixedPathUsesContextDefault=$true;NullInterfaceStopsProviderScan=$true
    RegistrationNodeAllocationBytes=208;ContextProviderHeadOffset=0;ContextProviderTailOffset=4;ContextProviderCountOffset=8;RegistrationAppendsToTail=$true
    ProviderInterfaceOffset=192;InterfaceAliasHeadOffset=4;AliasNodeNameOffset=4;AliasNodeAllocationBytes=20;AliasAppendToTail=$true;NativeAliasCopyBounded=$false;NativeAllocationFailureSafetyProven=$false
    BigInterfaceVa='0x00CF231C';BigRegistrationNodeVa='0x00CF2320';StartupBigRegistrationFlags=0;StartupBigVtableVa='0x00BF9D78';BaseBigVtableVa='0x00C84E60';BigOpenVirtualSlotOffset=12;BigOpenVirtualTarget='0x009956D0';BigAlias='big:';BuiltinDefaultAlias='null:'
    ScopedProviderPrefixRoutingRecovered=$true;ScopedBigRegistrationBridgeRecovered=$true;RelativePathQualificationModeled=$false;ActualStartupProviderOrderRecovered=$false;ActualStartupMountOrderRecovered=$false;GlobalFileProviderPrecedenceRecovered=$false
    StockArchiveCount=$selection.StockArchiveCount;StockEntryCount=$selection.StockEntryCount;ArchiveDirectoryMetadataBytesRead=$selection.ArchiveDirectoryMetadataBytesRead;ArchivePayloadBytesRead=0
    ConfiguredStockArchiveSetComplete=$selection.ConfiguredStockArchiveSetComplete;MissingConfiguredArchives=$selection.MissingConfiguredArchives
    ReadOnly=$true;TargetExecuted=$false;ModPackageLoaded=$false;AuthenticEp1ProcessingHashRecovered=$false;ProductionBuildReady=$false
    FaultTestsPassed=[bool]$SelfTest;RoutingFixturesExecuted=$(if($SelfTest){8}else{0});PolicyRejectionsExecuted=$(if($SelfTest){3}else{0});MemoryFaultsExecuted=$(if($SelfTest){20}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 8}else{$report}
