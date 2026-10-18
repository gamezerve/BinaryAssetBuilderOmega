# Reborn: maintain one repository-local SDK and replace its compiler generation only while no cooperative mod build is using it.
[CmdletBinding()]
param([string]$CompilerDirectory,[switch]$Initialize,[string]$PowerShell7)
$ErrorActionPreference='Stop'
# Reborn: MSBuild may inherit PowerShell 7's module path while invoking Windows PowerShell; load this host's own modules explicitly.
Import-Module (Join-Path $PSHOME 'Modules/Microsoft.PowerShell.Utility/Microsoft.PowerShell.Utility.psd1') -ErrorAction Stop
Import-Module (Join-Path $PSHOME 'Modules/Microsoft.PowerShell.Management/Microsoft.PowerShell.Management.psd1') -ErrorAction Stop
$repo=Split-Path -Parent $PSScriptRoot;$sdk=Join-Path $repo 'alpha'
if(-not $CompilerDirectory){$CompilerDirectory=Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/bin/x86/Release/net8.0'}
$names=@('BinaryAssetBuilder.ManifestInspector.exe','BinaryAssetBuilder.ManifestInspector.dll','BinaryAssetBuilder.ManifestInspector.deps.json','BinaryAssetBuilder.ManifestInspector.runtimeconfig.json','BinaryAssetBuilder.Core.dll','BinaryAssetBuilder.Utility.dll','BinaryAssetBuilder.XmlCompiler.dll','SageBinaryData.dll')
#-------------------------------------------------------------------------------------------------
<# Reborn: refuse reparse ancestry for both dependency sources and managed SDK destinations. #>
#-------------------------------------------------------------------------------------------------
function Assert-DevSdkPath([string]$Path){$current=[IO.Path]::GetFullPath($Path);while(-not(Test-Path -LiteralPath $current)){$current=Split-Path -Parent $current};for($item=Get-Item -LiteralPath $current;$null-ne $item;$item=if($item.PSIsContainer){$item.Parent}else{$item.Directory}){if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected SDK path refused.'}}}
foreach($path in @($sdk,$CompilerDirectory)){Assert-DevSdkPath $path}
$lock=$null;$timer=[Diagnostics.Stopwatch]::StartNew()
while($null-eq $lock){try{$lock=[IO.File]::Open((Join-Path $sdk '.sdk.lock'),[IO.FileMode]::OpenOrCreate,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None)}catch [IO.IOException]{if($timer.Elapsed.TotalSeconds-ge 15){throw 'SDK is in use; compiler build is complete but SDK sync must be retried after the mod build finishes.'};Start-Sleep -Milliseconds 100}}
try{
    $inventory=@();foreach($name in $names){$path=Join-Path $CompilerDirectory $name;Assert-DevSdkPath $path;if(-not(Test-Path -LiteralPath $path -PathType Leaf)){throw ('Compiler generation is incomplete: '+$name)};$inventory+= [pscustomobject]@{Name=$name;Sha256=(Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash}}
    $tools=Join-Path $sdk 'tools';$null=New-Item -ItemType Directory -Path $tools -Force;$destination=Join-Path $tools 'compiler'
    $different=-not(Test-Path -LiteralPath $destination)
    if(-not $different){foreach($file in Get-ChildItem -LiteralPath $destination -Force){if($file.PSIsContainer -or $file.Name-notin $names){throw 'Unmanaged file in SDK compiler directory; preserve it before syncing.'}};foreach($entry in $inventory){$path=Join-Path $destination $entry.Name;if(-not(Test-Path -LiteralPath $path) -or (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash-cne $entry.Sha256){$different=$true}}}
    if($different){
        $stage=Join-Path $tools ('.compiler-next-'+[guid]::NewGuid().ToString('N'));$previous=Join-Path $tools ('.compiler-old-'+[guid]::NewGuid().ToString('N'));$null=New-Item -ItemType Directory -Path $stage
        foreach($entry in $inventory){$inputPath=Join-Path $CompilerDirectory $entry.Name;$outputPath=Join-Path $stage $entry.Name;Copy-Item -LiteralPath $inputPath -Destination $outputPath;if((Get-FileHash -LiteralPath $inputPath -Algorithm SHA256).Hash-cne $entry.Sha256 -or (Get-FileHash -LiteralPath $outputPath -Algorithm SHA256).Hash-cne $entry.Sha256){throw 'Compiler changed during SDK synchronization; staged files retained.'}}
        # Reborn: the old generation is recoverable until the complete new directory takes its stable name.
        if(Test-Path -LiteralPath $destination){[IO.Directory]::Move($destination,$previous)}
        try{[IO.Directory]::Move($stage,$destination)}catch{if(Test-Path -LiteralPath $previous){[IO.Directory]::Move($previous,$destination)};throw}
        if(Test-Path -LiteralPath $previous){foreach($name in $names){$path=Join-Path $previous $name;if(Test-Path -LiteralPath $path){Remove-Item -LiteralPath $path}};[IO.Directory]::Delete($previous,$false)}
    }
    # Reborn: live repository scripts are referenced directly; only EA loader files are derived copies in the stable SDK root.
    foreach($pair in @(@('alpha/studio/defaultscript.cs','defaultscript.cs'),@('alpha/studio/EALAModStudio.exe.config','EALAModStudio.exe.config'))){Copy-Item -LiteralPath (Join-Path $repo $pair[0]) -Destination (Join-Path $sdk $pair[1]) -Force}
    if($Initialize){
        if(-not $PowerShell7){$command=Get-Command pwsh -ErrorAction SilentlyContinue;if($command){$PowerShell7=$command.Source}else{$PowerShell7=Join-Path $env:USERPROFILE '.cache/codex-runtimes/codex-primary-runtime/dependencies/native/powershell/pwsh.exe'}}
        Assert-DevSdkPath $PowerShell7;if(-not(Test-Path -LiteralPath $PowerShell7 -PathType Leaf)){throw 'Supply the installed PowerShell 7 executable.'}
        foreach($name in @('EALAModStudio.exe','EALABuild.dll','EULA for RA3 MOD SDK.rtf')){Copy-Item -LiteralPath (Join-Path $repo ('Working RA3 Compiler for Reference/'+$name)) -Destination (Join-Path $sdk $name) -Force}
        foreach($name in @('builtmods','.work')){$null=New-Item -ItemType Directory -Path (Join-Path $sdk $name) -Force}
        $config=Join-Path $sdk 'uprising-alpha.paths'
        # Reborn: initialize the explicit official source root for the bounded complete Conscript profile without replacing existing personal settings.
        if(-not(Test-Path -LiteralPath $config)){[IO.File]::WriteAllLines($config,@('# Reborn: local development SDK paths; not a redistributable configuration.',('PowerShell7='+$PowerShell7),'StaticManifest=D:\TEMP\Red Alert 3 Uprising Source Data\Static Data\data\static.manifest','GlobalManifest=D:\TEMP\Red Alert 3 Uprising Source Data\Global Data\data\global.manifest','UprisingXmlRoot=D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)\Uprising\Xml (Uprising)',('Driver='+(Join-Path $repo 'scripts/Invoke-Ra3Ep1DevAlphaBuild.ps1')),('CompilerDirectory='+$destination)))}
    }
    [IO.File]::WriteAllText((Join-Path $sdk 'toolchain.json'),([pscustomobject]@{Format='RebornDevSdk1';CompilerSource=[IO.Path]::GetFullPath($CompilerDirectory);UpdatedUtc=[DateTime]::UtcNow.ToString('o');Files=$inventory}|ConvertTo-Json -Depth 5))
    Write-Output ('Reborn: stable alpha toolchain synchronized; changed='+$different+'; '+$destination)
}finally{if($null-ne $lock){$lock.Dispose()}}
