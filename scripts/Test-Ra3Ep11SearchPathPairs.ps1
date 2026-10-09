# Reborn: verify scoped search-pair rebuilding and config bridge with deterministic repeats while retaining unresolved native safety and startup gates.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[Parameter(Mandatory=$true)] [string] $SkuDefinitionPath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11SearchPathPairs.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -AsJson|ConvertFrom-Json
if(-not $first.ReadOnly -or -not $first.FaultTestsPassed -or $first.ReviewedWholeBodyPins-ne 2 -or $first.PairRebuilderVa-cne '0x0096A660' -or $first.ConfigSetSearchPathVa-cne '0x004D7000' -or
   $first.ContextPathBufferOffset-ne 44 -or $first.ContextPathBufferCapacityOffset-ne 52 -or $first.ContextPairCapacityOffset-ne 56 -or $first.ContextPairBufferOffset-ne 60 -or $first.PairStride-ne 8 -or
   $first.PairProviderResolutionVa-cne '0x0096A050' -or $first.ConfigSearchTextGlobalVa-cne '0x00CF1910' -or $first.ArchivePayloadBytesRead-ne 0 -or
   $first.PairFixturesExecuted-ne 6 -or $first.RebuildFixturesExecuted-ne 1 -or $first.OpenCompositionFixturesExecuted-ne 1 -or $first.PolicyRejectionsExecuted-ne 9 -or $first.MemoryFaultsExecuted-ne 16){throw 'Search-pair evidence contract differs.'}
# Reborn: require recovered semantics, but do not convert detached policy checks into native safety or complete config/startup claims.
foreach($name in @('RebuilderClearsPriorTextAndPairs','SemicolonSplitsInInputOrder','RootPointersReferIntoCopiedBuffer','RemovesAtMostOneTrailingSeparatorPerToken','PreservesWhitespaceAndInteriorSeparators','PreservesDuplicateRoots','FinalTokenProviderResolvedBeforeFinalTrim','ConfigWriterResetsSearchText','ConfigWriterCallsPairRebuilder','ScopedSearchPairRebuildRecovered')){
    if(-not $first.$name){throw "Search-pair recovered field differs: $name"}
}
foreach($name in @('PairCapacityFieldRewrittenByRebuilder','NativeEmptyTokenSafetyProven','NativeTextAndPairCapacitySafetyProven','NativeResolvedProviderNonNullGuaranteed','FullConfigJoinAndTokenizationSemanticsModeled','ActualStartupSearchPathsRecovered','ActualStartupMountOrderRecovered','GlobalFileProviderPrecedenceRecovered','TargetExecuted','ModPackageLoaded','AuthenticEp1ProcessingHashRecovered','ProductionBuildReady')){
    if($first.$name){throw "Search-pair unresolved gate unexpectedly claimed: $name"}
}
# Reborn: compare every stable nested field except intentional detached-test counters.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','PairFixturesExecuted','RebuildFixturesExecuted','OpenCompositionFixturesExecuted','PolicyRejectionsExecuted','MemoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 8 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 8 -Compress)){throw "Repeat search-pair field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 search-path pairs: PASS; two whole-body pins, six pair/one rebuild/one open-composition fixtures, nine policy rejections, sixteen private faults and repeat JSON. Rebuild preserves order, trims one trailing separator and leaves capacity unchanged; native safety, complete startup/config order, compiler identity and game loading remain open.'
