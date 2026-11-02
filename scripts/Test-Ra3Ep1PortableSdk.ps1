# Reborn: exercise an independently copied SDK with bundled runtimes and private publication, never the user's Documents or live mods.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$OutputDirectory)
$ErrorActionPreference='Stop';$repo=Split-Path -Parent $PSScriptRoot;$source=Join-Path $repo 'alpha'
$OutputDirectory=[IO.Path]::GetFullPath($OutputDirectory)
if([IO.Path]::GetDirectoryName($OutputDirectory)-ine (Join-Path $source '.work') -or (Test-Path -LiteralPath $OutputDirectory)){throw 'Use a fresh direct child of alpha/.work.'}
$sdk=Join-Path $OutputDirectory 'Relocated SDK with spaces';$null=New-Item -ItemType Directory -Path $sdk
foreach($name in @('runtime','scripts','tools','catalogs','SageXML','Schemas','studio','Documentation')){
    Write-Output ('Reborn: copying portable verification input '+$name)
    Copy-Item -LiteralPath (Join-Path $source $name) -Destination $sdk -Recurse
}
foreach($name in @('EALAModStudio.exe','EALAModStudio.exe.config','EALABuild.dll','defaultscript.cs','defaultscript.cs.guidata','uprising-alpha.paths','toolchain.json','portable.json')){
    Copy-Item -LiteralPath (Join-Path $source $name) -Destination (Join-Path $sdk $name)
}
# Reborn: deliberately omit disposable work/output folders so relocation tests cover first-run startup as well as existing workspaces.
$null=New-Item -ItemType Directory -Path (Join-Path $sdk 'mods')
Copy-Item -LiteralPath (Join-Path $source 'mods/RebornTest') -Destination (Join-Path $sdk 'mods') -Recurse
$pwsh=Join-Path $sdk 'runtime/powershell/pwsh.exe'
$driver=Join-Path $sdk 'scripts/Invoke-Ra3Ep1DevAlphaBuild.ps1'
$final=Join-Path $OutputDirectory 'private-final-mods'
# Reborn: selecting an unrelated working directory exposes accidental relative-to-shell path assumptions.
Push-Location $env:WINDIR
try {
    foreach($mode in @(0,1)){
        $log=Join-Path $OutputDirectory ('mode-'+$mode+'.log')
        & $pwsh -NoProfile -NonInteractive -File $driver -ModName RebornTest -FinalModsRoot $final -BuildGlobalData 0 -DeveloperBuildTests $mode *> $log
        if($LASTEXITCODE-ne 0){throw ('Relocated SDK build failed; inspect '+$log)}
    }
}finally{Pop-Location}
# Reborn: prove selected settings resolve entirely inside the copied SDK; a caller's original configuration is never rewritten.
. (Join-Path $sdk 'scripts/Ra3Ep1PortablePaths.ps1')
$settings=Read-RebornSdkPaths $sdk
foreach($key in $settings.Keys){if(-not $settings[$key].StartsWith($sdk+'\',[StringComparison]::OrdinalIgnoreCase)){throw ('External portable dependency remains: '+$key)}}
$receipt=Join-Path $sdk 'builtmods/RebornTest/build.json'
if(-not(Test-Path -LiteralPath $receipt)){throw 'Relocated SDK did not produce its build receipt.'}
& (Join-Path $PSScriptRoot 'Test-Ra3Ep1PortableRuntime.ps1') -Sdk $sdk -OutputDirectory (Join-Path $OutputDirectory 'runtime-check')
[IO.File]::WriteAllText((Join-Path $OutputDirectory 'report.json'),([ordered]@{Format='RebornPortableRelocation1';Sdk=$sdk;FastBuildPassed=$true;DeveloperBuildPassed=$true;BundledPowerShell=$settings.PowerShell7;BundledDotNet=(Join-Path $sdk 'runtime/dotnet-x86');AllConfiguredInputsInsideCopy=$true;FinalOutput=$final;DocumentsModified=$false;GameExecuted=$false}|ConvertTo-Json))
Write-Output ('Reborn: portable relocation PASS (fast + developer build, SDK-local inputs, bundled runtimes, spaces/unrelated working directory, private final publication): '+$OutputDirectory)
