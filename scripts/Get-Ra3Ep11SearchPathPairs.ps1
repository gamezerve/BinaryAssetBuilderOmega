# Reborn: recover scoped semicolon search-pair rebuilding and its config writer bridge with bounded detached lists, not runtime initialization or native buffer emulation.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[Parameter(Mandatory=$true)] [string] $SkuDefinitionPath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$pairsSelfTest=$SelfTest;$pairsAsJson=$AsJson
$forwarding=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11ProviderOpenForwarding.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath
$SelfTest=$pairsSelfTest;$AsJson=$pairsAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: pin complete native pair rebuilding and config search-path writing independently; full startup/config execution remains outside this evidence. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11SearchPathPairCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0x56a660;Length=199;Hash='9CF953E1D2D8F419116E77C2AE7B903173B9349CE4FA81853905DD63C6A72242'},
        [pscustomobject]@{Offset=0xd7000;Length=273;Hash='CB1DAB9D671147307CB68F9E8354EFE4C166DD24A4ADFBBC47DF7F86A0234ACD'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Search-pair slice outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed search-pair code changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: model bounded printable-ASCII semicolon pairs, preserving byte offsets, whitespace, duplicates and single-slash trimming while refusing unsafe empty/capacity cases. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11DetachedSearchPairs([string] $Paths,[object[]] $Providers,[object] $Default,[int] $Capacity=100) {
    if($Capacity-lt 1 -or $Capacity-gt 100 -or $Paths.Length-eq 0 -or $Paths.Length-gt 1023){throw 'Detached search-pair capacity/text policy exceeded.'}
    foreach($character in $Paths.ToCharArray()){if([int]$character-lt 32 -or [int]$character-gt 126){throw 'Detached search pairs require printable ASCII.'}}
    $tokens=$Paths.Split(';',[StringSplitOptions]::None)
    if($tokens.Count-gt $Capacity){throw 'Detached search-pair count exceeds supplied capacity.'}
    $position=0;$pairs=[Collections.Generic.List[object]]::new()
    for($i=0;$i-lt $tokens.Count;$i++){
        $token=$tokens[$i]
        if($token.Length-eq 0){throw 'Empty search tokens are outside detached diagnostic policy.'}
        $root=$token
        if($root.EndsWith('/',[StringComparison]::Ordinal) -or $root.EndsWith('\',[StringComparison]::Ordinal)){$root=$root.Substring(0,$root.Length-1)}
        if($root.Length-eq 0){throw 'Single-separator search roots are outside detached diagnostic policy.'}
        # Reborn: intermediate paths are trimmed before routing; the final path is routed before the final trailing-slash write.
        $routingPath=$root;if($i-eq $tokens.Count-1){$routingPath=$token}
        $route=Select-Ep11DetachedProvider $routingPath $Providers $Default
        $pairs.Add([pscustomobject]@{Index=$i;BufferRelativeRootOffset=$position;Root=$root;Provider=$route.Selected;ProviderResolved=($null-ne $route.Selected)})
        $position+=$token.Length+1
    }
    return $pairs.ToArray()
}
Assert-Ep11SearchPathPairCode $ep11Bytes
if($SelfTest){
    $providers=@([pscustomobject]@{Id='big';HasInterface=$true;Aliases=@('big:')},[pscustomobject]@{Id='disk';HasInterface=$true;Aliases=@('disk:')})
    # Reborn: pair fixtures preserve order, duplicates, whitespace, case and one-only slash trimming, including unresolved explicit aliases.
    foreach($fixture in @(
        [pscustomobject]@{Input='big:/a/;disk:\b\';Roots=@('big:/a','disk:\b');Ids=@('big','disk');Offsets=@(0,8)},
        [pscustomobject]@{Input='relative';Roots=@('relative');Ids=@('default');Offsets=@(0)},
        [pscustomobject]@{Input='big:/a;big:/a';Roots=@('big:/a','big:/a');Ids=@('big','big');Offsets=@(0,7)},
        [pscustomobject]@{Input=' big:/a ';Roots=@(' big:/a ');Ids=@($null);Offsets=@(0)},
        [pscustomobject]@{Input='BIG:/a//';Roots=@('BIG:/a/');Ids=@('big');Offsets=@(0)},
        [pscustomobject]@{Input='unknown:/a';Roots=@('unknown:/a');Ids=@($null);Offsets=@(0)}
    )){
        $pairs=@(Get-Ep11DetachedSearchPairs $fixture.Input $providers 'default')
        if($pairs.Count-ne $fixture.Roots.Count){throw 'Detached search-pair count differs.'}
        for($i=0;$i-lt $pairs.Count;$i++){if($pairs[$i].Root-cne $fixture.Roots[$i] -or $pairs[$i].Provider-cne $fixture.Ids[$i] -or $pairs[$i].BufferRelativeRootOffset-ne $fixture.Offsets[$i]){throw 'Detached search-pair field differs.'}}
    }
    # Reborn: each rebuild returns only the current snapshot; no previous path survives an implicit append.
    $old=@(Get-Ep11DetachedSearchPairs 'big:/old;disk:/old' $providers 'default')
    $new=@(Get-Ep11DetachedSearchPairs 'big:/new' $providers 'default')
    if($old.Count-ne 2 -or $new.Count-ne 1 -or $new[0].Root-cne 'big:/new'){throw 'Detached search-pair rebuild differs.'}
    # Reborn: connect supplied resolved pairs to the previous open-plan model without claiming native stream success or special default-node pointer substitution.
    $pairs=@(Get-Ep11DetachedSearchPairs 'big:/a/;disk:/b/' $providers 'default')
    $targets=@([pscustomobject]@{Id=$pairs[0].Provider;Root=$pairs[0].Root;Hit=$false},[pscustomobject]@{Id=$pairs[1].Provider;Root=$pairs[1].Root;Hit=$true})
    $plan=Get-Ep11DetachedOpenPlan 'data/file' $true $targets
    if($plan.Selected-cne 'disk' -or $plan.Attempts.Count-ne 2 -or $plan.Attempts[0].Path-cne 'big:/a/data/file' -or $plan.Attempts[1].Path-cne 'disk:/b/data/file'){throw 'Detached search-pair/open composition differs.'}
    foreach($policyPath in @('',';a','a;','a;;b','/',([string][char]0),([string][char]233),('x'*1024))){
        $rejected=$false;try{$null=Get-Ep11DetachedSearchPairs $policyPath $providers 'default'}catch{$rejected=$true};if(-not $rejected){throw 'Out-of-policy search pairs admitted.'}
    }
    $rejected=$false;try{$null=Get-Ep11DetachedSearchPairs 'a;b' $providers 'default' 1}catch{$rejected=$true};if(-not $rejected){throw 'Detached search-pair overflow admitted.'}
    # Reborn: code faults remain private byte copies, not modifications to installed engine/config files.
    foreach($offset in @(0x56a670,0x56a685,0x56a696,0x56a6c7,0x56a6d0,0x56a6dd,0x56a6e0,0x56a6e8,0x56a6ef,0x56a6fb,0x56a71c,0xd702b,0xd704a,0xd70bd,0xd70d4,0xd7104)){
        $fault=[byte[]]$ep11Bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11SearchPathPairCode $fault}catch{$rejected=$true};if(-not $rejected){throw 'Search-pair code fault admitted.'}
    }
}
$pairsAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($pairsAfter))-cne $forwarding.ImageSha256){throw 'Search-pair image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$forwarding.ImageSha256;PairRebuilderVa='0x0096A660';ConfigSetSearchPathVa='0x004D7000';ReviewedWholeBodyPins=2
    ContextPathBufferOffset=44;ContextPathBufferCapacityOffset=52;ContextPairCapacityOffset=56;ContextPairBufferOffset=60;PairStride=8
    RebuilderClearsPriorTextAndPairs=$true;PairCapacityFieldRewrittenByRebuilder=$false;SemicolonSplitsInInputOrder=$true;RootPointersReferIntoCopiedBuffer=$true;RemovesAtMostOneTrailingSeparatorPerToken=$true
    PreservesWhitespaceAndInteriorSeparators=$true;PreservesDuplicateRoots=$true;FinalTokenProviderResolvedBeforeFinalTrim=$true;PairProviderResolutionVa='0x0096A050'
    ConfigWriterResetsSearchText=$true;ConfigSearchTextGlobalVa='0x00CF1910';ConfigWriterCallsPairRebuilder=$true;ScopedSearchPairRebuildRecovered=$true
    NativeEmptyTokenSafetyProven=$false;NativeTextAndPairCapacitySafetyProven=$false;NativeResolvedProviderNonNullGuaranteed=$false;FullConfigJoinAndTokenizationSemanticsModeled=$false;ActualStartupSearchPathsRecovered=$false;ActualStartupMountOrderRecovered=$false;GlobalFileProviderPrecedenceRecovered=$false
    StockArchiveCount=$forwarding.StockArchiveCount;StockEntryCount=$forwarding.StockEntryCount;ArchiveDirectoryMetadataBytesRead=$forwarding.ArchiveDirectoryMetadataBytesRead;ArchivePayloadBytesRead=0
    ConfiguredStockArchiveSetComplete=$forwarding.ConfiguredStockArchiveSetComplete;MissingConfiguredArchives=$forwarding.MissingConfiguredArchives
    ReadOnly=$true;TargetExecuted=$false;ModPackageLoaded=$false;AuthenticEp1ProcessingHashRecovered=$false;ProductionBuildReady=$false
    FaultTestsPassed=[bool]$SelfTest;PairFixturesExecuted=$(if($SelfTest){6}else{0});RebuildFixturesExecuted=$(if($SelfTest){1}else{0});OpenCompositionFixturesExecuted=$(if($SelfTest){1}else{0});PolicyRejectionsExecuted=$(if($SelfTest){9}else{0});MemoryFaultsExecuted=$(if($SelfTest){16}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 8}else{$report}
