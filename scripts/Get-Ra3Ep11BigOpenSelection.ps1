# Reborn: recover scoped native BIG open selection and distinguish its query preparation from iterator normalization using pinned code and detached fixtures only.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[Parameter(Mandatory=$true)] [string] $SkuDefinitionPath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$selectionSelfTest=$SelfTest;$selectionAsJson=$AsJson
$index=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11BigNameIndex.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath
$SelfTest=$selectionSelfTest;$AsJson=$selectionAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: independently pin the complete outer open body, iterator initializer, byte-preserving copy helper and exact big-prefix literal. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11BigOpenSelectionCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0x5956d0;Length=768;Hash='ED71F8C01ABD1082EDEFCB6A16DFF103FE6803F7CB6A87EBD8DA7AC890AB03B8'},
        [pscustomobject]@{Offset=0x595620;Length=105;Hash='3D08C7FF897BDC705AE5363C1C8992F48E508798F0DBE7196AEBE33329CEA024'},
        [pscustomobject]@{Offset=0x592d70;Length=36;Hash='481DCEBDB37CD96E99AA976DFAC13D5CBBC55C601CA52ED44A688B61D7F0A6CB'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'BIG open selection slice outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed BIG open selection changed.'}
    }
    if([Text.Encoding]::ASCII.GetString($Bytes,0x884eac,5)-cne ('big:'+[char]0)){throw 'BIG open prefix literal differs.'}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: bound detached query input before modeling exact prefix stripping, a single leading separator and first-pipe selection; native buffer safety is not inferred. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11DetachedBigOpenQuery([string] $Path) {
    if($Path.Length-gt 255){throw 'Detached BIG query exceeds conservative policy bound.'}
    foreach($character in $Path.ToCharArray()){if([int]$character-lt 32 -or [int]$character-gt 126){throw 'Detached BIG query requires printable ASCII.'}}
    $prefix=$Path.StartsWith('big:',[StringComparison]::Ordinal)
    $text=$Path;if($prefix){$text=$text.Substring(4)}
    $leading=$text.Length-gt 0 -and ($text[0]-eq '/' -or $text[0]-eq '\')
    if($leading){$text=$text.Substring(1)}
    $pipe=$text.IndexOf('|');$selector=$null;$name=$text
    if($pipe-ge 0){$selector=$text.Substring(0,$pipe);$name=$text.Substring($pipe+1)}
    return [pscustomobject]@{PrefixRemoved=$prefix;SingleLeadingSeparatorRemoved=$leading;HasSelector=($pipe-ge 0);Selector=$selector;Name=$name}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: model list traversal over pre-supplied hit booleans, preserving the ordinary selected-miss stop and separate Viv4 continuation, not native name lookup or actual mounts. #>
#-------------------------------------------------------------------------------------------------
function Select-Ep11DetachedBigOpen([object] $Query,[object[]] $Nodes) {
    $visited=0
    foreach($node in $Nodes){
        if($node.Class-ne 0 -and $node.Class-ne 2){throw 'Detached selection fixture supports ordinary BIG or Viv4 only.'}
        $visited++;$label=[string]$node.LabelPath;$colon=$label.IndexOf(':');$key=$label
        if($colon-ge 0){$key=$label.Substring($colon+1)}
        if($Query.HasSelector -and $key-cne $Query.Selector){continue}
        if($node.Present){return [pscustomobject]@{Selected=$node.Id;Visited=$visited;SelectedOrdinaryMiss=$false}}
        if($Query.HasSelector -and $node.Class-ne 2){return [pscustomobject]@{Selected=$null;Visited=$visited;SelectedOrdinaryMiss=$true}}
    }
    return [pscustomobject]@{Selected=$null;Visited=$visited;SelectedOrdinaryMiss=$false}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: separately model the iterator initializer's slash conversion and trailing-slash removal after its byte-preserving copy. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11DetachedBigIteratorPattern([string] $Pattern) {
    $null=Get-Ep11DetachedBigOpenQuery $Pattern
    return $Pattern.Replace('\','/').TrimEnd('/')
}
Assert-Ep11BigOpenSelectionCode $ep11Bytes
# Reborn: check the pinned image's reviewed original import thunks and symbol bytes without loading the CRT.
$imports=@(foreach($entry in @(
    [pscustomobject]@{Va='0x00BD8374';ThunkRaw=9148736;NameRva=0x8bae5e;NameRaw=9154144;Name='strncmp'},
    [pscustomobject]@{Va='0x00BD848C';ThunkRaw=9149016;NameRva=0x8b9f46;NameRaw=9150280;Name='strchr'}
)){
    if([BitConverter]::ToUInt32($ep11Bytes,$entry.ThunkRaw)-ne $entry.NameRva -or [Text.Encoding]::ASCII.GetString($ep11Bytes,$entry.NameRaw,$entry.Name.Length+1)-cne ($entry.Name+[char]0)){throw 'BIG open selection import differs.'}
    [pscustomobject]@{IatVa=$entry.Va;Name=$entry.Name;Dll='MSVCR80.dll'}
})
if($SelfTest){
    # Reborn: query fixtures preserve interior separators, dot segments, case and additional pipes while removing only the observed prefix/one leading separator.
    foreach($fixture in @(
        [pscustomobject]@{Path='big:/data/static.manifest';Name='data/static.manifest';Selector=$null;Prefix=$true;Leading=$true},
        [pscustomobject]@{Path='BIG:/data/static.manifest';Name='BIG:/data/static.manifest';Selector=$null;Prefix=$false;Leading=$false},
        [pscustomobject]@{Path='//data/static.manifest';Name='/data/static.manifest';Selector=$null;Prefix=$false;Leading=$true},
        [pscustomobject]@{Path='big:\data\static.manifest';Name='data\static.manifest';Selector=$null;Prefix=$true;Leading=$true},
        [pscustomobject]@{Path='big:second.big|Data/../static.manifest';Name='Data/../static.manifest';Selector='second.big';Prefix=$true;Leading=$false},
        [pscustomobject]@{Path='first.big|data|other';Name='data|other';Selector='first.big';Prefix=$false;Leading=$false},
        [pscustomobject]@{Path='|data';Name='data';Selector='';Prefix=$false;Leading=$false},
        [pscustomobject]@{Path='';Name='';Selector=$null;Prefix=$false;Leading=$false}
    )){
        $query=Get-Ep11DetachedBigOpenQuery $fixture.Path
        if($query.Name-cne $fixture.Name -or $query.Selector-cne $fixture.Selector -or $query.PrefixRemoved-ne $fixture.Prefix -or $query.SingleLeadingSeparatorRemoved-ne $fixture.Leading -or $query.HasSelector-ne ($null-ne $fixture.Selector)){throw 'Detached BIG query preparation differs.'}
    }
    # Reborn: supplied hits exercise first-success order, exact selector labels, ordinary selected failure and Viv4-specific continuation.
    $first=[pscustomobject]@{Id='first';LabelPath='big:first.big';Class=0;Present=$true}
    $second=[pscustomobject]@{Id='second';LabelPath='big:second.big';Class=0;Present=$true}
    foreach($case in @('first','later','absent','selector','case','selected-miss','viv4-miss','drive-key')){
        $a=$first.PSObject.Copy();$b=$second.PSObject.Copy();$path='data';$expected='first';$visited=1;$stop=$false
        switch($case){
            'later'{$a.Present=$false;$expected='second';$visited=2}
            'absent'{$a.Present=$false;$b.Present=$false;$expected=$null;$visited=2}
            'selector'{$path='second.big|data';$expected='second';$visited=2}
            'case'{$path='SECOND.big|data';$expected=$null;$visited=2}
            'selected-miss'{$path='first.big|data';$a.Present=$false;$b.LabelPath='big:first.big';$expected=$null;$stop=$true}
            'viv4-miss'{$path='first.big|data';$a.Present=$false;$a.Class=2;$b.LabelPath='big:first.big';$expected='second';$visited=2}
            'drive-key'{$a.LabelPath='D:\Data\first.big';$path='\Data\first.big|data';$expected=$null;$visited=2}
        }
        $result=Select-Ep11DetachedBigOpen (Get-Ep11DetachedBigOpenQuery $path) @($a,$b)
        if($result.Selected-cne $expected -or $result.Visited-ne $visited -or $result.SelectedOrdinaryMiss-ne $stop){throw "Detached BIG open selection differs: $case"}
    }
    foreach($fixture in @(
        [pscustomobject]@{Input='data\sub\';Expected='data/sub'},[pscustomobject]@{Input='////';Expected=''},
        [pscustomobject]@{Input='Data/../File';Expected='Data/../File'},[pscustomobject]@{Input='';Expected=''}
    )){if((Get-Ep11DetachedBigIteratorPattern $fixture.Input)-cne $fixture.Expected){throw 'Detached BIG iterator pattern differs.'}}
    # Reborn: input-policy rejection and private byte faults do not mutate any game or archive file.
    foreach($path in @(([string][char]0),([string][char]233),('x'*256))){$rejected=$false;try{$null=Get-Ep11DetachedBigOpenQuery $path}catch{$rejected=$true};if(-not $rejected){throw 'Out-of-policy BIG query admitted.'}}
    foreach($offset in @(0x595712,0x59571b,0x595727,0x59576f,0x5957d7,0x5957f3,0x595810,0x595821,0x595823,0x59582f,0x595860,0x595927,0x595665,0x595676,0x592d84,0x884eac)){
        $fault=[byte[]]$ep11Bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11BigOpenSelectionCode $fault}catch{$rejected=$true};if(-not $rejected){throw 'BIG open selection fault admitted.'}
    }
}
$selectionAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($selectionAfter))-cne $index.ImageSha256){throw 'BIG open selection image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$index.ImageSha256;OpenVa='0x009956D0';IteratorInitVa='0x00995620';CopyVa='0x00992D70';ReviewedWholeBodyPins=3;ReviewedLiteralPins=1;Imports=$imports
    ScopedManagerListSelectionRecovered=$true;ArchiveTraversalStartsAtManagerOffset=16;ArchiveNextLinkOffset=0;FirstSuccessfulNodeWins=$true
    QueryExactOptionalPrefix='big:';QueryStripsAtMostOneLeadingSeparator=$true;QuerySplitsAtFirstPipe=$true;ArchiveSelectorCaseSensitive=$true;ArchiveSelectorKeyRule='LabelPathAfterFirstColonOrWholeLabel'
    OrdinarySelectedArchiveMissStopsSearch=$true;Viv4SelectedArchiveMissContinues=$true;QueryBodyNormalizesSeparators=$false;QueryBodyCollapsesDotSegments=$false
    IteratorConvertsBackslashToSlash=$true;IteratorTrimsTrailingSlashes=$true;CopyHelperPreservesBytes=$true
    NativeBufferSafetyProven=$false;ActualStartupMountOrderRecovered=$false;GlobalFileProviderPrecedenceRecovered=$false;NativeOpenStreamLifecycleRecovered=$false;FullArchivePrecedenceAndUnmountSemanticsRecovered=$false
    StockArchiveCount=$index.StockArchiveCount;StockEntryCount=$index.StockEntryCount;ArchiveDirectoryMetadataBytesRead=$index.ArchiveDirectoryMetadataBytesRead;ArchivePayloadBytesRead=0
    ConfiguredStockArchiveSetComplete=$index.ConfiguredStockArchiveSetComplete;MissingConfiguredArchives=$index.MissingConfiguredArchives
    ReadOnly=$true;TargetExecuted=$false;ModPackageLoaded=$false;AuthenticEp1ProcessingHashRecovered=$false;ProductionBuildReady=$false
    FaultTestsPassed=[bool]$SelfTest;QueryFixturesExecuted=$(if($SelfTest){8}else{0});SelectionFixturesExecuted=$(if($SelfTest){8}else{0});IteratorFixturesExecuted=$(if($SelfTest){4}else{0});PolicyRejectionsExecuted=$(if($SelfTest){3}else{0});MemoryFaultsExecuted=$(if($SelfTest){16}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 8}else{$report}
