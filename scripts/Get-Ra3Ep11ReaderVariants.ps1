# Reborn: characterize scoped EP1 1.1 suffix insertion and source probing using pinned bodies and detached short ASCII filenames only.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$variantSelfTest=$SelfTest;$variantAsJson=$AsJson
$queue=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11ManifestQueue.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath
$SelfTest=$variantSelfTest;$AsJson=$variantAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: pin the complete suffix setter and probe adapter, plus derived-directory and version-fallback literals. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11ReaderVariantCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0xaa300;Length=675;Hash='8BE03CB73D3CD92278F797FFA1DBB5A87F2BF176B6A5DED9AD511642F34EF8E7'},
        [pscustomobject]@{Offset=0xd7840;Length=15;Hash='2ED182CA7BB573A3DE5B8B19ABE439F62A2C918B90C0F9D0130D01D3194681E9'},
        [pscustomobject]@{Offset=0x7f7e58;Length=2;Hash='1472D0645F552820B5472B91341C2D9A118C8A96F4A72758D6AEEB14C10A1107'},
        [pscustomobject]@{Offset=0x7f8f30;Length=21;Hash='C7E17B6B9ACAC8BA4123D944665FAE3EC5395D9BC8B682E6DDC3B296F9B10676'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Reader variant slice outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed reader variant changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: replay only the reviewed suffix selection/insertion on detached ASCII filenames; diagnostic restrictions are not engine rejection guarantees. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11DetachedVariantFilename([string] $Path,[string[]] $ReaderSuffixes,[string[]] $GlobalSuffixes) {
    if([string]::IsNullOrEmpty($Path) -or $ReaderSuffixes.Count-ne 3 -or $GlobalSuffixes.Count-ne 3){throw 'Detached variant inputs differ.'}
    foreach($value in @($Path)+$ReaderSuffixes+$GlobalSuffixes){if($value.Length-gt 255 -or $value.Contains([char]0) -or $value-match '[^\x00-\x7F]'){throw 'Detached variant ASCII policy differs.'}}
    $dot=$Path.LastIndexOf('.');$separator=[Math]::Max($Path.LastIndexOf('/'),$Path.LastIndexOf('\'))
    if($dot-le $separator -or $dot-le 0){throw 'Detached variant requires filename extension.'}
    $suffix=''
    foreach($index in 0..2){if($ReaderSuffixes[$index].Length-gt 0){$suffix+=$ReaderSuffixes[$index]}else{$suffix+=$GlobalSuffixes[$index]}}
    $result=$Path.Substring(0,$dot)+$suffix+$Path.Substring($dot)
    if($result.Length-gt 255){throw 'Detached variant result exceeds initial PoC policy.'}
    return $result
}
Assert-Ep11ReaderVariantCode $ep11Bytes
$fixtures=@(
    [pscustomobject]@{Path='static.manifest';Reader=@('','_L','');Global=@('','','');Expected='static_L.manifest'},
    [pscustomobject]@{Path='data/mod.manifest';Reader=@('','_M','_v2');Global=@('','','');Expected='data/mod_M_v2.manifest'},
    [pscustomobject]@{Path='a.b.manifest';Reader=@('','','');Global=@('_base','_L','');Expected='a.b_base_L.manifest'},
    [pscustomobject]@{Path='mod.manifest';Reader=@('_local','','');Global=@('_ignored','_M','_v4');Expected='mod_local_M_v4.manifest'},
    [pscustomobject]@{Path='mod.manifest';Reader=@('','','');Global=@('','','');Expected='mod.manifest'}
)
if($SelfTest){
    # Reborn: exercise detached last-dot insertion, three-slot ordering and nonempty reader-over-global selection.
    foreach($fixture in $fixtures){if((Get-Ep11DetachedVariantFilename $fixture.Path $fixture.Reader $fixture.Global)-cne $fixture.Expected){throw 'Detached variant filename differs.'}}
    foreach($path in @('mod','dir.name/mod','mód.manifest',"mod`0.manifest",('a'*256)+'.manifest')){
        $rejected=$false;try{$null=Get-Ep11DetachedVariantFilename $path @('','','') @('','','')}catch{$rejected=$true};if(-not $rejected){throw 'Detached variant diagnostic fault admitted.'}
    }
    # Reborn: detached byte faults cover suffix index, three-slot loop, last-dot operations, publication, derived directory and probe adapter.
    foreach($offset in @(0xaa383,0xaa389,0xaa3b2,0xaa3c1,0xaa41d,0xaa448,0xaa484,0xaa490,0xaa53e,0xaa54c,0xaa56c,0xaa58f,0xd7847,0x7f7e58,0x7f8f30,0x7f8f3c)){
        $fault=[byte[]]$ep11Bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11ReaderVariantCode $fault}catch{$rejected=$true};if(-not $rejected){throw 'Reader variant byte fault admitted.'}
    }
}
$variantAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($variantAfter))-cne $queue.ImageSha256){throw 'Reader variant image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$queue.ImageSha256;ReviewedPins=4;SetterVa='0x004AA300';SetterArgumentOneIsSuffixIndex=$true;SetterArgumentTwoIsSuffixText=$true
    ReaderSuffixArrayOffset=88;SuffixSlotCount=3;GlobalSuffixArrayVa='0x00CF15E8';NonemptyReaderSlotOverridesGlobal=$true
    OriginalPathReaderOffset=16;EffectivePathReaderOffset=12;DerivedDirectoryReaderOffset=20;ReviewedSuffixInsertionBeforeLastDot=$true
    ReaderBypassFlagOffset=104;BypassFlagCopiesOriginalPath=$true;TagTwoCachePublicationVa='0x006A34B0';TagTwoPublicationStoresReader=$true
    ProbeAdapterVa='0x004D7840';ProbeAdapterCallsFileProbeVa='0x004D6F10';ProbeAdapterMetadataOutputIsNull=$true
    VersionFallbackFormat='_v%d';VersionMarkerExtension='.version';VersionFallbackSetterIndex=2;FallbackProviderSemanticsRecovered=$false
    FixtureExpectations=$fixtures;NativeSuffixIndexBoundsProven=$false;NativeMissingDotSafetyProven=$false;NativeLocaleAndNonAsciiSemanticsRecovered=$false
    FullVariantFilenameAndFallbackSemanticsRecovered=$false;CompleteStreamPointerProvenanceRecovered=$false;ProductionBuildReady=$false;ModPackageLoaded=$false
    ReadOnly=$true;TargetExecuted=$false;ManagedInspectorHashCommandExecuted=$false;FaultTestsPassed=[bool]$SelfTest
    FilenameFixturesExecuted=$(if($SelfTest){5}else{0});InvalidFixturePoliciesExecuted=$(if($SelfTest){5}else{0});MemoryFaultsExecuted=$(if($SelfTest){16}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 7}else{$report}
