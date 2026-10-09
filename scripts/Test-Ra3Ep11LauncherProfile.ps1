# Reborn: validate pinned launcher metadata/imports/literals and repeated evidence without claiming executed forwarding or SKU selection.
[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$LauncherPath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11LauncherProfile.ps1'
$first=& $audit -LauncherPath $LauncherPath -SelfTest
$second=& $audit -LauncherPath $LauncherPath -AsJson|ConvertFrom-Json
if(-not $first.ReadOnly -or $first.LauncherBytes-ne 9209808 -or $first.EntryPointSection-cne '.bind' -or $first.EntryPointRva-cne '0x008B4310' -or
    $first.EntryPointRaw-ne 9024272 -or $first.LiteralPins-ne 5 -or $first.LiteralFaultsExecuted-ne 5 -or $first.TruncationRefusalsExecuted-ne 1){throw 'Launcher profile contract differs.'}
$imports=@{GetCommandLineA='0x0048F2E4';CreateProcessW='0x0048F3A0';GetCommandLineW='0x0048F444';ShellExecuteW='0x0048F5E0'}
if($first.SelectedImports.Count-ne 4){throw 'Launcher import count differs.'}
foreach($binding in $first.SelectedImports){if($imports[$binding.Name]-cne $binding.IatVa){throw 'Launcher import binding differs.'}}
foreach($literal in $first.Literals){if($literal.InstructionXrefsProven -or $literal.TextSectionLiteralVaWordOccurrences-ne 0){throw 'Launcher word occurrence profile differs.'}}
foreach($name in @('TargetExecuted','ProtectionLayerDecoded','LauncherExecutableSelectionProven','LauncherArgumentForwardingProven','ActualChildCommandLineObserved','ConfigConsumptionObserved','ModPackageLoaded','ProductionBuildReady')){
    if($first.$name){throw "Unresolved launcher gate claimed: $name"}
}
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('LiteralFaultsExecuted','TruncationRefusalsExecuted')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 8 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 8 -Compress)){throw "Repeat launcher field differs: $($property.Name)"}
}
Write-Output 'EP1 launcher profile: PASS; five literal pins/five private mutations/one truncation refusal, four launch-related import bindings and repeat JSON. Entry point is in .bind; static executable selection, argument forwarding and actual config consumption remain unproven.'
