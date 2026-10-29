# Reborn: privately test missing ART warnings, real native compilation and the same alias replay/log helpers used by Studio.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$OutputDirectory,[Parameter(Mandatory)][string]$StaticManifest,[Parameter(Mandatory)][string]$GlobalManifest)
$ErrorActionPreference='Stop'
$repo=Split-Path -Parent $PSScriptRoot
$OutputDirectory=[IO.Path]::GetFullPath($OutputDirectory)
if(Test-Path -LiteralPath $OutputDirectory){throw 'Fresh missing ART test output is required.'}
$null=New-Item -ItemType Directory -Path $OutputDirectory
$exe=Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/bin/x86/Release/net8.0/BinaryAssetBuilder.ManifestInspector.exe'
$fixture=Join-Path $OutputDirectory 'frontend'
& $exe missing-art-include-self-test $fixture *> (Join-Path $OutputDirectory 'frontend.log')
if($LASTEXITCODE-ne 0){throw 'Missing ART frontend tests failed; see frontend.log.'}
. (Join-Path $PSScriptRoot 'Ra3Ep1GlobalData.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep1BuildLog.ps1')
$receipt=Get-Content -LiteralPath (Join-Path $fixture 'build-source/source-input.json') -Raw | ConvertFrom-Json
Assert-RebornSourceSearch $receipt.Searches
Assert-RebornGlobalSources ([pscustomobject]@{Evidence=@([pscustomobject]@{Sources=$receipt.Sources;Searches=$receipt.Searches})})
# Reborn: the final publication helpers must reject a file which appeared after its warning-only absence was captured.
$candidate=Join-Path $fixture 'mod/Art/Missing.w3x'
[IO.File]::WriteAllText($candidate,'Reborn: appeared after source capture.')
$refused=$false
try{Assert-RebornSourceSearch $receipt.Searches}catch{$refused=$true}
if(-not $refused){throw 'Static publication accepted a newly appearing ART file.'}
$refused=$false
try{Assert-RebornGlobalSources ([pscustomobject]@{Evidence=@([pscustomobject]@{Sources=$receipt.Sources;Searches=$receipt.Searches})})}catch{$refused=$true}
if(-not $refused){throw 'Global publication accepted a newly appearing ART file.'}
Remove-Item -LiteralPath $candidate
Assert-RebornSourceSearch $receipt.Searches
# Reborn: old receipts without ExpectedMissing keep their original ordinary-winner semantics.
$ordinary=[pscustomobject]@{Candidates=@((Join-Path $fixture 'mod/Data/asset.xml'));PhysicalPath=(Join-Path $fixture 'mod/Data/asset.xml');LogicalPath='DATA:asset.xml';Layer='mod'}
Assert-RebornSourceSearch @($ordinary)
# Reborn: preserve every raw phase warning while displaying identical source-site warnings once.
$detail=Join-Path $OutputDirectory 'warning-detail.log'
$warning="Warning: MISSINGART ' ART:Missing.w3x ' (referenced by Include/source) skipped. Source: fixture.xml(3,10)."
$null=Write-SdkCompilerLine $warning $detail
$null=Write-SdkCompilerLine $warning $detail
$null=Write-SdkCompilerLine ($warning.Replace('(3,10)','(4,10)')) $detail
if(@($script:BuildLogLines|Where-Object {$_-like 'Warning: MISSINGART *'}).Count-ne 2 -or @(Get-Content -LiteralPath $detail).Count-ne 3){throw 'Missing ART warning display/raw-log counts differ.'}
# Reborn: compile only the isolated fixture; no Documents mods, user XML or game files are touched.
$savedMode=$env:REBORN_DEVELOPER_BUILD_TESTS
try{
    $env:REBORN_DEVELOPER_BUILD_TESTS='0'
    & $exe conscript-alpha-build (Join-Path $repo 'alpha/SageXML') $StaticManifest $GlobalManifest (Join-Path $fixture 'build-source/game.xml') (Join-Path $OutputDirectory 'compiled') *> (Join-Path $OutputDirectory 'compile.log')
    if($LASTEXITCODE-ne 0){throw 'Warning-only missing ART native build failed; see compile.log.'}
}finally{$env:REBORN_DEVELOPER_BUILD_TESTS=$savedMode}
Write-Output 'Reborn: missing ART includes: PASS (frontend, native stream/readback, static/global publication replay, legacy receipts, warning display deduplication; private output only).'
