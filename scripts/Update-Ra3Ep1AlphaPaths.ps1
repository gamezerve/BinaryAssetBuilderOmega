# Reborn: repair local paths after relocating a configured alpha workspace; back up configuration and preserve all mod/build/SDK/game files.
[CmdletBinding()]
param([string]$PowerShell7,[string]$StaticManifest,[string]$GlobalManifest)
$ErrorActionPreference='Stop'
if($PSVersionTable.PSVersion.Major-lt 7){throw 'PowerShell 7 is required.'}
$workspace=[IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))

#-------------------------------------------------------------------------------------------------
# Reborn: validate existing path type and every ancestor before changing only this workspace's local configuration.
#-------------------------------------------------------------------------------------------------
function Test-AlphaLocalPath([string]$Path,[bool]$Directory=$false){
    if(-not [IO.Path]::IsPathFullyQualified($Path) -or $Path.IndexOfAny([char[]]@('"',"`r","`n"))-ge 0){throw 'Expected an unambiguous absolute path.'}
    if(-not (Test-Path -LiteralPath $Path -PathType $(if($Directory){'Container'}else{'Leaf'}))){throw 'Required local dependency is missing or has wrong type.'}
    for($item=Get-Item -LiteralPath $Path;$null-ne $item;$item=if($item.PSIsContainer){$item.Parent}else{$item.Directory}){
        if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected configuration/dependency refused.'}
    }
}
Test-AlphaLocalPath $workspace $true
$config=Join-Path $workspace 'uprising-alpha.paths'
Test-AlphaLocalPath $config
if((Get-Item -LiteralPath $config).Length-gt 16384){throw 'Configuration exceeds 16 KiB.'}
$original=[IO.File]::ReadAllBytes($config)
$originalHash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($original))
$settings=[Collections.Generic.Dictionary[string,string]]::new([StringComparer]::Ordinal)
foreach($line in [IO.File]::ReadAllLines($config)){
    if(-not $line -or $line.StartsWith('#',[StringComparison]::Ordinal)){continue}
    $separator=$line.IndexOf('=')
    if($separator-le 0){throw 'Malformed alpha configuration.'}
    $key=$line.Substring(0,$separator)
    if($key -cnotin @('PowerShell7','StaticManifest','Driver','CompilerDirectory','GlobalManifest') -or $settings.ContainsKey($key)){throw 'Unknown or duplicate configuration key.'}
    $settings.Add($key,$line.Substring($separator+1))
}
foreach($key in @('PowerShell7','StaticManifest','Driver','CompilerDirectory')){if(-not $settings.ContainsKey($key)){throw 'All four base alpha path settings are required.'}}
if(-not $PowerShell7){$PowerShell7=$settings['PowerShell7']}
if(-not $StaticManifest){$StaticManifest=$settings['StaticManifest']}
if(-not $GlobalManifest -and $settings.ContainsKey('GlobalManifest')){$GlobalManifest=$settings['GlobalManifest']}
Test-AlphaLocalPath $PowerShell7
Test-AlphaLocalPath $StaticManifest
foreach($extension in @('.bin','.relo','.imp')){Test-AlphaLocalPath ([IO.Path]::ChangeExtension($StaticManifest,$extension))}
# Reborn: retain/validate optional explicit weapon metadata without making legacy armor configs depend on it.
if($GlobalManifest){Test-AlphaLocalPath $GlobalManifest}
$driver=Join-Path $workspace 'scripts/Invoke-Ra3Ep1AlphaBuild.ps1'
$compiler=Join-Path $workspace 'tools/compiler'
Test-AlphaLocalPath $driver
Test-AlphaLocalPath $compiler $true
Test-AlphaLocalPath (Join-Path $compiler 'BinaryAssetBuilder.ManifestInspector.exe')
Test-AlphaLocalPath (Join-Path $workspace 'builtmods') $true
$probe=Join-Path $workspace 'builtmods/RebornAlphaInput-00000000000000000000000000000000/build/weapon-00000000/weaponprobe_1.0.cfg'
if($probe.Length-gt 240 -or $probe.ToCharArray().Where({[int]$_-gt 126 -or [int]$_-lt 32}).Count){throw 'Choose a shorter ASCII workspace path.'}
$newLines=@('# Reborn: relocated local configuration; backup retained, never include local paths in public archives.',
    ('PowerShell7='+$PowerShell7),('StaticManifest='+$StaticManifest),('Driver='+$driver),('CompilerDirectory='+$compiler))
if($GlobalManifest){$newLines+=('GlobalManifest='+$GlobalManifest)}
# Reborn: replace only the inspected config atomically, keeping a byte-exact uniquely named recovery backup.
$nonce=[guid]::NewGuid().ToString('N')
$temporary=$config+'.'+$nonce+'.pending'
$backup=$config+'.'+$nonce+'.bak'
if(Test-Path -LiteralPath $temporary){throw 'Unexpected occupied staging path.'}
[IO.File]::WriteAllLines($temporary,$newLines)
if((Get-FileHash -LiteralPath $config -Algorithm SHA256).Hash-cne $originalHash){throw 'Configuration changed during validation; pending file retained.'}
[IO.File]::Replace($temporary,$config,$backup)
if((Get-FileHash -LiteralPath $backup -Algorithm SHA256).Hash-cne $originalHash){throw 'Recovery backup differs; inspect before building.'}
[pscustomobject]@{Workspace=$workspace;ConfigurationUpdated=$true;Backup=$backup;BackupSha256=$originalHash;
    Driver=$driver;CompilerDirectory=$compiler;GameExecuted=$false;RegistryModified=$false;InstalledFilesModified=$false}
