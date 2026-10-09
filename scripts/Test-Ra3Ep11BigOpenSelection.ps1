# Reborn: verify scoped open-selection and iterator distinctions without claiming actual startup order, global provider precedence or native execution.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[Parameter(Mandatory=$true)] [string] $SkuDefinitionPath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11BigOpenSelection.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.ReadOnly -or $first.ReviewedWholeBodyPins-ne 3 -or $first.ReviewedLiteralPins-ne 1 -or
   $first.OpenVa-cne '0x009956D0' -or $first.IteratorInitVa-cne '0x00995620' -or $first.CopyVa-cne '0x00992D70' -or
   $first.ArchiveTraversalStartsAtManagerOffset-ne 16 -or $first.ArchiveNextLinkOffset-ne 0 -or $first.QueryExactOptionalPrefix-cne 'big:' -or
   $first.ArchiveSelectorKeyRule-cne 'LabelPathAfterFirstColonOrWholeLabel' -or $first.ArchivePayloadBytesRead-ne 0 -or
   $first.QueryFixturesExecuted-ne 8 -or $first.SelectionFixturesExecuted-ne 8 -or $first.IteratorFixturesExecuted-ne 4 -or $first.PolicyRejectionsExecuted-ne 3 -or $first.MemoryFaultsExecuted-ne 16){throw 'BIG open selection evidence contract differs.'}
# Reborn: require each recovered scoped behavior separately so iterator normalization cannot silently become an open-path rule.
foreach($name in @('ScopedManagerListSelectionRecovered','FirstSuccessfulNodeWins','QueryStripsAtMostOneLeadingSeparator','QuerySplitsAtFirstPipe','ArchiveSelectorCaseSensitive','OrdinarySelectedArchiveMissStopsSearch','Viv4SelectedArchiveMissContinues','IteratorConvertsBackslashToSlash','IteratorTrimsTrailingSlashes','CopyHelperPreservesBytes')){
    if(-not $first.$name){throw "BIG open selection recovered field differs: $name"}
}
foreach($name in @('QueryBodyNormalizesSeparators','QueryBodyCollapsesDotSegments','NativeBufferSafetyProven','ActualStartupMountOrderRecovered','GlobalFileProviderPrecedenceRecovered','NativeOpenStreamLifecycleRecovered','FullArchivePrecedenceAndUnmountSemanticsRecovered','TargetExecuted','ModPackageLoaded','AuthenticEp1ProcessingHashRecovered','ProductionBuildReady')){
    if($first.$name){throw "BIG open selection unresolved gate unexpectedly claimed: $name"}
}
if($first.Imports.Count-ne 2 -or $first.Imports[0].Name-cne 'strncmp' -or $first.Imports[1].Name-cne 'strchr'){throw 'BIG open import attribution differs.'}
# Reborn: compare every nested stable field except deliberate detached-test counters.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','QueryFixturesExecuted','SelectionFixturesExecuted','IteratorFixturesExecuted','PolicyRejectionsExecuted','MemoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 8 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 8 -Compress)){throw "Repeat BIG open selection field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 BIG open selection: PASS; three complete code pins/one literal, eight query/eight selection/four iterator fixtures, three policy rejections, sixteen private faults and repeat JSON. First-success manager traversal is scoped; actual startup/global precedence, stream lifecycle, authentic compiler hash and game loading remain open.'
