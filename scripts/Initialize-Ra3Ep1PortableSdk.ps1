# Reborn: maintenance-only assembly of the agreed local portable SDK; no installers, mod builds, game launches or source edits.
[CmdletBinding()]
param(
    [string]$Sdk=(Join-Path (Split-Path -Parent $PSScriptRoot) 'alpha'),
    [Parameter(Mandatory)][string]$PowerShellSource,
    [Parameter(Mandatory)][string]$DotNetSource,
    [Parameter(Mandatory)][string]$DotNetVersion,
    [Parameter(Mandatory)][string]$HostFxrVersion,
    [Parameter(Mandatory)][string]$StaticSource,
    [Parameter(Mandatory)][string]$GlobalSource,
    [Parameter(Mandatory)][string]$AudioSource
)
$ErrorActionPreference='Stop'
$Sdk=[IO.Path]::GetFullPath($Sdk)
# Reborn: existing Studio builds keep their compiler/dependency generation until this maintenance operation can own the SDK lock.
$sdkLock=[IO.File]::Open((Join-Path $Sdk '.sdk.lock'),[IO.FileMode]::OpenOrCreate,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None)
try {
& (Join-Path $PSScriptRoot 'Sync-Ra3Ep1PortableSupport.ps1') -Sdk $Sdk
#-------------------------------------------------------------------------------------------------
<# Reborn: copy runtime inputs into one managed SDK location while keeping each original installation unchanged. #>
#-------------------------------------------------------------------------------------------------
function Copy-RebornPortableTree([string]$Source,[string]$Relative) {
    $target=Join-Path $Sdk $Relative;$null=New-Item -ItemType Directory -Force -Path $target
    foreach($item in Get-ChildItem -LiteralPath $Source -Force){
        if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw ('Runtime source contains a redirected entry: '+$item.FullName)}
        Copy-Item -LiteralPath $item.FullName -Destination $target -Recurse -Force
    }
}
Write-Output 'Reborn: copying the private PowerShell runtime.'
Copy-RebornPortableTree $PowerShellSource 'runtime/powershell'
Write-Output 'Reborn: copying the private x86 .NET runtime.'
Copy-RebornPortableTree (Join-Path $DotNetSource ('shared/Microsoft.NETCore.App/'+$DotNetVersion)) ('runtime/dotnet-x86/shared/Microsoft.NETCore.App/'+$DotNetVersion)
Copy-RebornPortableTree (Join-Path $DotNetSource ('host/fxr/'+$HostFxrVersion)) ('runtime/dotnet-x86/host/fxr/'+$HostFxrVersion)
Copy-Item -LiteralPath (Join-Path $DotNetSource 'dotnet.exe') -Destination (Join-Path $Sdk 'runtime/dotnet-x86/dotnet.exe') -Force
foreach($name in @('LICENSE.txt','ThirdPartyNotices.txt')){if(Test-Path -LiteralPath (Join-Path $DotNetSource $name)){Copy-Item -LiteralPath (Join-Path $DotNetSource $name) -Destination (Join-Path $Sdk ('runtime/dotnet-x86/'+$name)) -Force}}
Write-Output 'Reborn: copying native x86 audio dependencies and stock stream baselines.'
$vc=Join-Path $Sdk 'runtime/vc-x86';$null=New-Item -ItemType Directory -Force -Path $vc
foreach($name in @('msvcp140.dll','vcruntime140.dll','vcruntime140_1.dll')){
    $path=Join-Path $env:WINDIR ('SysWOW64/'+$name)
    if(Test-Path -LiteralPath $path){Copy-Item -LiteralPath $path -Destination (Join-Path $vc $name) -Force}
}
# Reborn: the optional shader utility and EA FXC load legacy DirectX helpers app-locally instead of requiring a system DirectX redistributable.
$shader=Join-Path $Sdk 'ShaderCompiler'
if(Test-Path -LiteralPath (Join-Path $shader 'fxc.exe')){
    foreach($name in @('d3dx9_31.dll','d3dx9_35.dll','d3dx10_35.dll','D3DCompiler_35.dll')){
        $path=Join-Path $env:WINDIR ('SysWOW64/'+$name)
        if(-not(Test-Path -LiteralPath $path)){throw ('Shader runtime dependency not found: '+$path)}
        Copy-Item -LiteralPath $path -Destination (Join-Path $shader $name) -Force
    }
}
foreach($group in @(@('static',$StaticSource),@('global',$GlobalSource),@('audio',$AudioSource))){
    $target=Join-Path $Sdk ('catalogs/ra3ep1/streams/'+$group[0]);$null=New-Item -ItemType Directory -Force -Path $target
    foreach($file in Get-ChildItem -LiteralPath $group[1] -File | Where-Object Extension -in @('.manifest','.bin','.imp','.relo')){Copy-Item -LiteralPath $file.FullName -Destination (Join-Path $target $file.Name) -Force}
}
# Reborn: retain an owned backup of the prior configuration before replacing only the managed dependency locations.
$config=Join-Path $Sdk 'uprising-alpha.paths';$settings=@{}
foreach($line in Get-Content -LiteralPath $config){if($line-and -not $line.StartsWith('#')){$pair=$line.Split('=',2);$settings[$pair[0]]=$pair[1]}}
$backup=Join-Path $Sdk '.work/portable-paths-before.txt';if(-not(Test-Path -LiteralPath $backup)){Copy-Item -LiteralPath $config -Destination $backup}
$settings.PowerShell7='runtime/powershell/pwsh.exe';$settings.Driver='scripts/Invoke-Ra3Ep1DevAlphaBuild.ps1';$settings.CompilerDirectory='tools/compiler'
$settings.StaticManifest='catalogs/ra3ep1/streams/static/static.manifest';$settings.GlobalManifest='catalogs/ra3ep1/streams/global/global.manifest'
$settings.WorldBuilderManifest='catalogs/ra3ep1/worldbuilder.manifest';$settings.UprisingXmlRoot='SageXML'
$lines=@('# Reborn: portable SDK paths are relative to EALAModStudio.exe; final mods use the current Windows Documents folder.')
$lines+=@($settings.Keys|Sort-Object|ForEach-Object {$_+'='+$settings[$_]});[IO.File]::WriteAllLines($config,$lines)
[IO.File]::WriteAllText((Join-Path $Sdk 'portable.json'),([ordered]@{Format='RebornPortableSdk1';DotNetArchitecture='x86';DotNetRuntime=$DotNetVersion;HostFxr=$HostFxrVersion;FinalOutput='WindowsDocuments';RequiresWindows=$true;RequiresNetFramework4=$true}|ConvertTo-Json))
Write-Output 'Reborn: portable dependencies assembled; relocation verification still required.'
}finally{$sdkLock.Dispose()}
