# Reborn: verify scoped provider routing/registration evidence and deterministic repeat output without claiming actual startup order or working mods.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[Parameter(Mandatory=$true)] [string] $SkuDefinitionPath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11ProviderRouting.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -AsJson|ConvertFrom-Json
if(-not $first.ReadOnly -or -not $first.FaultTestsPassed -or $first.ReviewedWholeBodyPins-ne 7 -or $first.ReviewedStartupSlicePins-ne 1 -or $first.ReviewedVtableSlicePins-ne 2 -or $first.ReviewedLiteralPins-ne 1 -or
   $first.RoutingVa-cne '0x0096A050' -or $first.RegistrationVa-cne '0x0096A8A0' -or $first.AliasAppendVa-cne '0x00969DA0' -or $first.AliasCompareVa-cne '0x00969C60' -or
   $first.RegistrationNodeAllocationBytes-ne 208 -or $first.ProviderInterfaceOffset-ne 192 -or $first.ContextProviderHeadOffset-ne 0 -or $first.ContextProviderTailOffset-ne 4 -or $first.ContextProviderCountOffset-ne 8 -or
   $first.InterfaceAliasHeadOffset-ne 4 -or $first.AliasNodeNameOffset-ne 4 -or $first.AliasNodeAllocationBytes-ne 20 -or
   $first.BigInterfaceVa-cne '0x00CF231C' -or $first.BigRegistrationNodeVa-cne '0x00CF2320' -or $first.StartupBigRegistrationFlags-ne 0 -or $first.BigOpenVirtualSlotOffset-ne 12 -or $first.BigOpenVirtualTarget-cne '0x009956D0' -or
   $first.BigAlias-cne 'big:' -or $first.BuiltinDefaultAlias-cne 'null:' -or $first.ArchivePayloadBytesRead-ne 0 -or
   $first.RoutingFixturesExecuted-ne 8 -or $first.PolicyRejectionsExecuted-ne 3 -or $first.MemoryFaultsExecuted-ne 20){throw 'Provider routing evidence contract differs.'}
# Reborn: require scoped behavior but retain unresolved qualification, startup, allocation, production and game-load gates.
foreach($name in @('PrefixIncludesFirstColon','AliasAsciiCaseInsensitive','FirstMatchingRegisteredProviderWins','UnknownPrefixReturnsNull','UnprefixedPathUsesContextDefault','NullInterfaceStopsProviderScan','RegistrationAppendsToTail','AliasAppendToTail','ScopedProviderPrefixRoutingRecovered','ScopedBigRegistrationBridgeRecovered')){
    if(-not $first.$name){throw "Provider recovered field differs: $name"}
}
foreach($name in @('NativeAliasCopyBounded','NativeAllocationFailureSafetyProven','RelativePathQualificationModeled','ActualStartupProviderOrderRecovered','ActualStartupMountOrderRecovered','GlobalFileProviderPrecedenceRecovered','TargetExecuted','ModPackageLoaded','AuthenticEp1ProcessingHashRecovered','ProductionBuildReady')){
    if($first.$name){throw "Provider unresolved gate unexpectedly claimed: $name"}
}
# Reborn: compare every stable nested report field, excluding only deliberate detached-test counters.
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('FaultTestsPassed','RoutingFixturesExecuted','PolicyRejectionsExecuted','MemoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 8 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 8 -Compress)){throw "Repeat provider field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 provider routing: PASS; seven whole-body pins, scoped startup/two vtables/alias literal, eight detached prefix fixtures, three policy rejections, twenty private faults and repeat JSON. BIG registration reaches open slot +0C; actual startup order, qualification, native safety, compiler identity and game loading remain open.'
