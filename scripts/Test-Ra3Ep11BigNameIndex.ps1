# Reborn: verify scoped BIG hash/index evidence and repeat determinism while refusing native-array, collision-correctness and production claims.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[Parameter(Mandatory=$true)] [string] $SkuDefinitionPath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11BigNameIndex.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.ReadOnly -or $first.ReviewedWholeBodyPins-ne 7 -or
   $first.IndexBuilderVa-cne '0x00996880' -or $first.HashVa-cne '0x009960E0' -or $first.ComparatorVa-cne '0x00996150' -or $first.IndexedLookupVa-cne '0x009962E0' -or
   $first.IndexRecordBytes-ne 8 -or $first.IndexPointerMemberOffset-ne 8 -or $first.HashSeed-ne 5381 -or $first.HashMultiplier-ne 33 -or
   -not $first.HashFinalByteSwap -or -not $first.HashAsciiCaseInsensitive -or -not $first.NativeRecordsStoreAbsoluteDirectoryPointers -or -not $first.DiagnosticRecordsUseDirectoryRelativeOffsets -or
   -not $first.IndexedLookupUsesUnsignedHashSearch -or -not $first.IndexedLookupInitiallyComparesQueryName -or -not $first.AdjacentCollisionComparisonUsesMidpointName -or
   $first.HashFixturesExecuted-ne 8 -or $first.NamePolicyRejectionsExecuted-ne 3 -or $first.CollisionDirectoryFixturesExecuted-ne 1 -or $first.MalformedDirectoryFixturesExecuted-ne 6 -or $first.MemoryFaultsExecuted-ne 16 -or
   $first.StockArchiveCount-ne $first.Archives.Count -or $first.StockArchiveCount-lt 1 -or $first.StockEntryCount-ne ($first.Archives|Measure-Object EntryCount -Sum).Sum -or $first.ArchivePayloadBytesRead-ne 0){throw 'BIG name index evidence contract differs.'}
# Reborn: a directory-only model cannot certify native locale, equal-key order, collision handling, game loading or compiler identity.
foreach($name in @('HashNormalizesPathSeparators','NativeNonAsciiLocaleSemanticsRecovered','NativeEqualHashSortOrderProven','CollisionResolutionCorrectnessProven','NativePreparedIndexBytesValidated','FullIndexAndPathLookupSemanticsRecovered','FullArchivePrecedenceAndUnmountSemanticsRecovered','TargetExecuted','ModPackageLoaded','AuthenticEp1ProcessingHashRecovered','ProductionBuildReady')){
    if($first.$name){throw "BIG name index unresolved gate unexpectedly claimed: $name"}
}
if($first.Imports.Count-ne 2 -or $first.Imports[0].Name-cne 'qsort' -or $first.Imports[1].Name-cne 'tolower' -or $first.DiagnosticEqualHashTieBreak-cne 'RecordOffset'){throw 'BIG name index attribution differs.'}
foreach($row in $first.Archives){if($row.EntryCount-lt 1 -or $row.RelativePairSha256-notmatch '^[0-9A-F]{64}$'){throw 'Detached stock BIG index report differs.'}}
# Reborn: compare all nested stable fields, excluding deliberate detached-test counters only.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','HashFixturesExecuted','NamePolicyRejectionsExecuted','CollisionDirectoryFixturesExecuted','MalformedDirectoryFixturesExecuted','MemoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 8 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 8 -Compress)){throw "Repeat BIG name index field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 BIG name index: PASS; seven complete code pins, checked stock directory-only models, eight hash fixtures, three name-policy rejections, one collision directory/six malformed directories, sixteen byte faults and repeat JSON. Native collision correctness, native prepared arrays, compiler identity and game loading remain open.'
