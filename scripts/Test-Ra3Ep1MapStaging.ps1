# Reborn: verify map suffix publication with a real compiled EP1 map package in a private workspace; never build or edit a user mod.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$MapPackage,[Parameter(Mandatory)][string]$OutputDirectory)
$ErrorActionPreference='Stop'
$repo=Split-Path -Parent $PSScriptRoot
. "$PSScriptRoot/Ra3Ep1PortablePaths.ps1"
. "$PSScriptRoot/Ra3Ep1BuildLog.ps1"
. "$PSScriptRoot/Ra3Ep1FinalPackage.ps1"
. "$PSScriptRoot/Ra3Ep1Maps.ps1"
$sdk=Join-Path $repo 'alpha';$settings=Read-RebornSdkPaths $sdk
$output=[IO.Path]::GetFullPath($OutputDirectory)
if([IO.Path]::GetDirectoryName($output)-ine (Join-Path $sdk '.work') -or (Test-Path -LiteralPath $output)){throw 'Use a fresh direct child of alpha/.work.'}
$null=New-Item -ItemType Directory -Path $output
$inspector=Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/bin/x86/Release/net8.0/BinaryAssetBuilder.ManifestInspector.exe'
$stage=Join-Path $output 'staged'
& $inspector sdk-stage-map-package $MapPackage $stage
if($LASTEXITCODE){throw 'Native map package staging failed.'}
if([Convert]::ToHexString([IO.File]::ReadAllBytes((Join-Path $stage 'map.version')))-cne '5F6D6F640D0A'){throw 'map.version must contain _mod followed by CRLF.'}
foreach($extension in @('bin','manifest','relo','imp')){if(-not(Test-Path (Join-Path $stage "map_mod.$extension")) -or (Test-Path (Join-Path $stage "map.$extension"))){throw 'Map sidecar suffix differs.'}}
# Reborn: exercise discovery skips and the missing raw-map diagnostic without invoking a compiler or modifying any live source.
$data=Join-Path $output 'Data';$plans=@()
Build-RebornMaps $data $sdk $settings '' '' '' $output '' '' ([ref]$plans)
if($plans.Count){throw 'Missing Official directory produced maps.'}
$folder=Join-Path $data 'Maps/Official/Example';$null=New-Item -ItemType Directory -Path $folder
Build-RebornMaps $data $sdk $settings '' '' '' $output '' '' ([ref]$plans)
Set-Content -LiteralPath (Join-Path $folder 'map.xml') -Value '<AssetDeclaration />'
$rejected=$false
try{Build-RebornMaps $data $sdk $settings '' '' '' $output '' '' ([ref]$plans)}catch{if($_ -notmatch 'MAPFILE:'){throw};$rejected=$true}
if(-not $rejected){throw 'Missing raw .map was accepted.'}
# Reborn: verify the generated family reaches the final Misc archive while unrelated authored files remain intact.
$misc=Join-Path $output 'Misc';$null=New-Item -ItemType Directory -Path $misc
Set-Content -LiteralPath (Join-Path $misc 'keep.txt') -Value 'authored sentinel'
$files=@(Get-ChildItem -LiteralPath $stage -File -Recurse|ForEach-Object{[pscustomobject]@{Relative=[IO.Path]::GetRelativePath($stage,$_.FullName);Sha256=(Get-FileHash -LiteralPath $_.FullName).Hash}})
$plan=[pscustomobject]@{Name='Example';Entry='packaging witness';Stage=$stage;Sources=@();Files=$files}
Publish-RebornMapOutputs @($plan) $misc
if((Get-Content (Join-Path $misc 'keep.txt'))-cne 'authored sentinel'){throw 'Authored Misc content changed.'}
$count=New-RebornMiscBig $misc (Join-Path $output 'Maps_Misc.big')
if($count-ne ($files.Count+1)){throw 'Misc map entry count differs.'}
Write-Output 'Reborn: map staging PASS (real EP1 graph; map_mod sidecars; exact _mod selector; discovery diagnostics; Misc publication and archive verification). Native map XML compilation is not covered.'
