# Reborn: verify bundled apphost selection and prohibit fallback to the original repository in a private relocation copy.
param([Parameter(Mandatory)][string]$Sdk,[Parameter(Mandatory)][string]$OutputDirectory)
$ErrorActionPreference='Stop';$Sdk=[IO.Path]::GetFullPath($Sdk);$OutputDirectory=[IO.Path]::GetFullPath($OutputDirectory)
$privateRoot=Join-Path (Split-Path -Parent $PSScriptRoot) 'alpha/.work'
if(-not $Sdk.StartsWith([IO.Path]::GetFullPath($privateRoot)+'\',[StringComparison]::OrdinalIgnoreCase)){throw 'Runtime negative control requires a private SDK copy under alpha/.work.'}
$null=New-Item -ItemType Directory -Path $OutputDirectory -Force
. (Join-Path $Sdk 'scripts/Ra3Ep1PortablePaths.ps1');$settings=Read-RebornSdkPaths $Sdk
$exe=Join-Path $settings.CompilerDirectory 'BinaryAssetBuilder.ManifestInspector.exe'
$trace=Join-Path $OutputDirectory 'corehost.txt';$env:COREHOST_TRACE='1';$env:COREHOST_TRACEFILE=$trace
try{& $exe w3d-coordinate-proof-self-test;if($LASTEXITCODE-ne 0){throw 'Portable compiler apphost failed.'}}
finally{Remove-Item Env:COREHOST_TRACE,Env:COREHOST_TRACEFILE -ErrorAction SilentlyContinue}
$text=[IO.File]::ReadAllText($trace)
if(-not $text.Contains((Join-Path $Sdk 'runtime/dotnet-x86/shared/Microsoft.NETCore.App'))){throw 'Apphost trace did not select the copied .NET runtime.'}
# Reborn: temporarily remove only the private copy's harness; the original repository is deliberately left available as a forbidden fallback.
$fixture=Join-Path $Sdk 'tools/resources/tests/fixtures/ReferencePipeline.xsd';$held=$fixture+'.negative-control'
[IO.File]::Move($fixture,$held)
try{
    $log=Join-Path $OutputDirectory 'missing-resource.txt';& $exe sdk-reviewed-schema-candidate *> $log
    if($LASTEXITCODE-eq 0 -or -not([IO.File]::ReadAllText($log).Contains('Bundled SDK compiler resources are missing.'))){throw 'Portable compiler fell back outside its SDK or reported the wrong missing-resource failure.'}
}finally{[IO.File]::Move($held,$fixture)}
[IO.File]::WriteAllText((Join-Path $OutputDirectory 'runtime-report.json'),([ordered]@{BundledDotNetSelected=$true;ExternalResourceFallbackRejected=$true;CopiedResourceRestored=$true}|ConvertTo-Json))
Write-Output 'Reborn: portable runtime PASS (bundled x86 host trace; missing local resources reject external repository fallback).'
