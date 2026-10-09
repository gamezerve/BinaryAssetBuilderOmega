# Reborn: verify scoped native BIG mount evidence, detached faults and stable repeat output without claiming native execution or production compatibility.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[Parameter(Mandatory=$true)] [string] $SkuDefinitionPath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11BigMountMetadata.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -AsJson|ConvertFrom-Json
if(-not $first.FaultTestsPassed -or -not $first.ReadOnly -or $first.ReviewedWholeBodyPins-ne 4 -or
   $first.MountVa-cne '0x00995160' -or $first.NodeConstructorVa-cne '0x009950F0' -or
   $first.HeaderClassifierVa-cne '0x00996060' -or $first.DirectorySizeVa-cne '0x009961A0' -or
   $first.IndexHelperVa-cne '0x00996880' -or $first.ConfigAddBigMode-ne 2 -or
   $first.NativeInitialHeaderRequestBytes-ne 16 -or $first.ArchiveNodeAllocationBytes-ne 104 -or
   $first.ArchiveListFirstOffset-ne 16 -or $first.ArchiveListTailOffset-ne 20 -or $first.ArchiveListCountOffset-ne 24 -or
   -not $first.NativeModeLowBitControlsAlternateNameAttempt -or -not $first.NativeModeBitTwoCallsIndexHelper -or
   -not $first.ScopedArchiveListAppendRecovered -or -not $first.NativeClassifierIgnoresFourthBigMagicByte -or
   -not $first.StrictDiagnosticBigReaderAcceptsOnlyBigFourOrBigF -or
   $first.HeaderArithmeticFixturesExecuted-ne 6 -or $first.TruncatedHeaderFixturesExecuted-ne 1 -or $first.MemoryFaultsExecuted-ne 12 -or
   $first.StockHeaderCount-ne $first.StockHeaders.Count -or $first.StockHeaderCount-lt 1 -or $first.ArchivePayloadBytesRead-ne 0){throw 'BIG mount metadata evidence contract differs.'}
# Reborn: retain separate unresolved native safety, full lookup/precedence and production gates.
foreach($name in @('NativeInitialHeaderReadResultChecked','NativeDirectoryReadResultChecked','NativeAllocationFailureSafetyProven','FullIndexAndPathLookupSemanticsRecovered','FullArchivePrecedenceAndUnmountSemanticsRecovered','TargetExecuted','ModPackageLoaded','AuthenticEp1ProcessingHashRecovered','ProductionBuildReady')){
    if($first.$name){throw "BIG mount unresolved gate unexpectedly claimed: $name"}
}
foreach($row in $first.StockHeaders){if($row.Magic-cne 'BIG4' -or $row.NativeHeaderClass-ne 0 -or $row.NativeDeclaredDirectoryBytes-lt 16){throw 'Stock BIG header differs.'}}
# Reborn: compare every nested stable field, excluding only deliberate detached-test counters.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','HeaderArithmeticFixturesExecuted','TruncatedHeaderFixturesExecuted','MemoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 8 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 8 -Compress)){throw "Repeat BIG mount field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 BIG mount metadata: PASS; four whole-body pins, stock directory headers, six arithmetic fixtures, one truncation rejection, twelve detached byte faults and repeat JSON. Zero archive payload bytes; full lookup, native safety, authentic compiler hash and game loading remain open.'
