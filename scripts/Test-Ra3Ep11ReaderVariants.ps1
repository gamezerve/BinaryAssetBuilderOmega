# Reborn: enforce scoped native suffix/probe evidence with detached filename policies and exact repeat reports, not native filename or game execution.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11ReaderVariants.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.ReadOnly -or -not $first.SetterArgumentOneIsSuffixIndex -or
   -not $first.SetterArgumentTwoIsSuffixText -or -not $first.NonemptyReaderSlotOverridesGlobal -or
   -not $first.ReviewedSuffixInsertionBeforeLastDot -or -not $first.BypassFlagCopiesOriginalPath -or
   -not $first.TagTwoPublicationStoresReader -or -not $first.ProbeAdapterMetadataOutputIsNull -or
   $first.ReviewedPins-ne 4 -or $first.FilenameFixturesExecuted-ne 5 -or $first.InvalidFixturePoliciesExecuted-ne 5 -or $first.MemoryFaultsExecuted-ne 16 -or
   $first.SetterVa-cne '0x004AA300' -or $first.ReaderSuffixArrayOffset-ne 88 -or $first.SuffixSlotCount-ne 3 -or
   $first.GlobalSuffixArrayVa-cne '0x00CF15E8' -or $first.OriginalPathReaderOffset-ne 16 -or $first.EffectivePathReaderOffset-ne 12 -or
   $first.DerivedDirectoryReaderOffset-ne 20 -or $first.ReaderBypassFlagOffset-ne 104 -or $first.TagTwoCachePublicationVa-cne '0x006A34B0' -or
   $first.ProbeAdapterVa-cne '0x004D7840' -or $first.ProbeAdapterCallsFileProbeVa-cne '0x004D6F10' -or
   $first.VersionFallbackFormat-cne '_v%d' -or $first.VersionMarkerExtension-cne '.version' -or $first.VersionFallbackSetterIndex-ne 2 -or
   $first.FallbackProviderSemanticsRecovered -or $first.NativeSuffixIndexBoundsProven -or $first.NativeMissingDotSafetyProven -or
   $first.NativeLocaleAndNonAsciiSemanticsRecovered -or $first.FullVariantFilenameAndFallbackSemanticsRecovered -or
   $first.CompleteStreamPointerProvenanceRecovered -or $first.ProductionBuildReady -or $first.ModPackageLoaded -or
   $first.TargetExecuted -or $first.ManagedInspectorHashCommandExecuted){throw 'Reader variant evidence/refusal contract differs.'}
# Reborn: compare nested fixture expectations and every stable field while excluding self-test counters.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','FilenameFixturesExecuted','InvalidFixturePoliciesExecuted','MemoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 7 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 7 -Compress)){throw "Repeat reader variant field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 reader variants: PASS; four pins, five detached filename fixtures, five diagnostic policy rejections, sixteen detached byte faults and repeat JSON. Three-slot suffix insertion, tag-two cache publication and file-probe adapter recovered; full fallback/non-ASCII safety and game loading remain open.'
