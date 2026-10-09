# Reborn: validate the bounded installed-config inventory and repeat JSON without claiming actual startup reads or mod loading.
[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$ImagePath,[Parameter(Mandatory=$true)][string]$BaselineImagePath,[Parameter(Mandatory=$true)][string]$SkuDefinitionPath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11ConfigInventory.ps1'
$first=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -SelfTest
$second=& $audit -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath -AsJson|ConvertFrom-Json
if(-not $first.ReadOnly -or $first.AvailableArchiveCount-ne 13 -or $first.ConfiguredArchiveCount-ne 14 -or $first.TotalArchiveEntries-ne 17383 -or
    $first.CandidateCount-ne 4 -or $first.FilesystemConfigCandidateCount-ne 0 -or $first.ArchivePayloadBytesRead-ne 0 -or $first.LooseConfigBytesRead-ne 3240 -or
    $first.ArchiveDirectoryMetadataBytesRead-ne 5390165 -or $first.TextFixturesExecuted-ne 3 -or $first.PolicyRejectionsExecuted-ne 1 -or
    $first.DirectoryFixturesExecuted-ne 2 -or $first.DirectoryFaultsExecuted-ne 6){throw 'Installed config inventory contract differs; re-review changed profile.'}
# Reborn: pin the four observed small-file contents, not a generalized claim about all installations.
$expected=@{
    'RA3EP1_english_1.0.SkuDef'='63BDBE83BDA6976052A56EAC96A5381756C80FE4445C81C2EAAEE8CC521DF1B3'
    'RA3EP1_english_1.1.SkuDef'='416C0752EA6891F13916D60FB1C8C3B8E5F342F7E919214866FC5D6EE962D75E'
    'ra3ep1_wb_1.0.cfg'='33D4D748937E83979B81EA50DD914AFE6C7EA50D3AA512D8F42D20FD6058B0D0'
    'WorldBuilder.cfg'='33D4D748937E83979B81EA50DD914AFE6C7EA50D3AA512D8F42D20FD6058B0D0'
}
foreach($candidate in $first.Candidates){if($candidate.Source-cne 'Loose' -or $candidate.Kind-cne 'ASCII' -or $expected[$candidate.Name]-cne $candidate.Sha256){throw 'Config content pin differs.'}}
foreach($name in @('CompressedConfigDecoded','SearchWasRecursive','UnconfiguredArchivesRead','EffectiveConfigPrecedenceProven','FullFilesystemConfigAbsenceProven','TargetExecuted','ModPackageLoaded','ProductionBuildReady')){
    if($first.$name){throw "Unresolved config gate claimed: $name"}
}
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('TextFixturesExecuted','PolicyRejectionsExecuted','DirectoryFixturesExecuted','DirectoryFaultsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 8 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 8 -Compress)){throw "Repeat config field differs: $($property.Name)"}
}
Write-Output 'EP1 1.1 bounded config inventory: PASS; four pinned ASCII loose configs, no config entries in 13 configured available archives, three text/two directory fixtures, one size refusal, six directory faults and repeat JSON. No filesystem.cfg found within this scope; runtime fallback and game loading remain unproven.'
