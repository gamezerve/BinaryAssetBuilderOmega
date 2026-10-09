# Reborn: verify two reproducible launch requests and the inert config identity without starting a native process or claiming loading success.
[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$ImagePath,[Parameter(Mandatory=$true)][string]$BaselineImagePath,[Parameter(Mandatory=$true)][string]$SkuDefinitionPath,[Parameter(Mandatory=$true)][string]$LauncherPath,[Parameter(Mandatory=$true)][string]$ConfigPath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11SmokeTestPlan.ps1'
$arguments=@{ImagePath=$ImagePath;BaselineImagePath=$BaselineImagePath;SkuDefinitionPath=$SkuDefinitionPath;LauncherPath=$LauncherPath;ConfigPath=$ConfigPath}
$first=& $audit @arguments -SelfTest
$second=& $audit @arguments -AsJson|ConvertFrom-Json
if(-not $first.ReadOnly -or $first.ConfigBytes-ne 1 -or $first.ConfigSha256-cne '01BA4719C80B6FE911B091A7C05124B64EEECE964E09C058EF8F9805DACA546B' -or
    $first.ConfigDirectiveCount-ne 0 -or $first.Cases.Count-ne 2 -or $first.WhitespaceFixturesExecuted-ne 2 -or $first.PolicyRefusalsExecuted-ne 9 -or
    $first.InstallationProfile-cne 'IncompleteStock-CampaignMissing-NoAlias' -or $first.MissingConfiguredArchives.Count-ne 1){throw 'Reviewed smoke-test profile differs.'}
if($first.Cases[0].Name-cne 'baseline' -or $first.Cases[0].RequestedArgumentList.Count-ne 0 -or
    $first.Cases[1].Name-cne 'isolated-config-read' -or $first.Cases[1].RequestedArgumentList.Count-ne 2 -or
    $first.Cases[1].RequestedArgumentList[0]-cne '-modconfig' -or $first.Cases[1].RequestedArgumentList[1]-cne $first.ConfigPath){throw 'Smoke-test request arguments differ.'}
foreach($name in @('TargetExecuted','ArgumentsForwardedProven','ConfigConsumedProven','EffectiveModPathsProven','AuthoredAssetsIncluded','ModPackageLoaded','ProductionBuildReady','AutomaticLaunchAuthorizedByReport')){
    if($first.$name){throw "Unresolved smoke-test gate claimed: $name"}
}
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('WhitespaceFixturesExecuted','PolicyRefusalsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 8 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 8 -Compress)){throw "Repeat smoke-test field differs: $($property.Name)"}
}
Write-Output 'EP1 Phase A plan: PASS; single-LF/no-directive config, two explicit launch requests, two whitespace fixtures, nine policy refusals and repeat JSON. Incomplete campaign profile is disclosed, not aliased. No launch, forwarding, config consumption or authored mod load is claimed.'
