# Reborn: test EA's CLR4 adapter/configuration seam without opening Studio, launching a build or editing a live mod.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$OutputDirectory)
$ErrorActionPreference='Stop'
$repo=Split-Path -Parent $PSScriptRoot
$sdk=Join-Path $repo 'alpha'
$output=[IO.Path]::GetFullPath($OutputDirectory)
$work=[IO.Path]::GetFullPath((Join-Path $sdk '.work'))
if([IO.Path]::GetDirectoryName($output) -ine $work -or (Test-Path -LiteralPath $output)){throw 'Use a fresh direct child of alpha/.work.'}
$null=New-Item -ItemType Directory -Path $output
# Reborn: reproduce EA's exact CodeDOM references instead of relying on the compiler application's broader dependencies.
$provider=New-Object Microsoft.CSharp.CSharpCodeProvider
$parameters=New-Object System.CodeDom.Compiler.CompilerParameters
$parameters.GenerateInMemory=$true
$null=$parameters.ReferencedAssemblies.Add('System.dll')
$null=$parameters.ReferencedAssemblies.Add((Join-Path $sdk 'EALABuild.dll'))
# Reborn: Studio already has its build library loaded; reproduce that dependency context before reflecting the compiled adapter.
$null=[Reflection.Assembly]::LoadFrom((Join-Path $sdk 'EALABuild.dll'))
try{
    $result=$provider.CompileAssemblyFromFile($parameters,[string[]]@((Join-Path $sdk 'studio/defaultscript.cs')))
    if($result.Errors.HasErrors){throw ($result.Errors | Out-String)}
    $method=$result.CompiledAssembly.GetType('EALABuild.BuildScript').GetMethod('ReadPaths',[Reflection.BindingFlags]'Static,NonPublic')
    # Reborn: a typed delegate avoids PowerShell wrapping reflection arguments as PSObject instead of CLR strings.
    $readPaths=[Delegate]::CreateDelegate([Func[string,object]],$method)
    $settings=$readPaths.Invoke([string](Join-Path $sdk 'uprising-alpha.paths'))
    if(-not $settings.ContainsKey('WorldBuilderManifest')){throw 'Live WorldBuilder configuration was not accepted.'}
    $baseKeys=@('PowerShell7','Driver','StaticManifest','CompilerDirectory')
    $legacy=@($baseKeys | ForEach-Object {$_+'='+$settings[$_]})
    $legacyPath=Join-Path $output 'legacy.paths'
    [IO.File]::WriteAllLines($legacyPath,[string[]]$legacy)
    $legacySettings=$readPaths.Invoke([string]$legacyPath)
    if($legacySettings.Count -ne 4){throw 'Legacy four-key configuration changed.'}
    # Reborn: optional catalog support must not disable typo, duplicate or required-setting checks.
    foreach($case in @('unknown','duplicate','required')){
        $lines=switch($case){
            'unknown' {$legacy; 'UnrecognizedSetting=test'}
            'duplicate' {$legacy; 'WorldBuilderManifest=first'; 'WorldBuilderManifest=second'}
            'required' {$legacy | Where-Object {-not $_.StartsWith('Driver=')}}
        }
        $path=Join-Path $output ($case+'.paths')
        [IO.File]::WriteAllLines($path,[string[]]$lines)
        $rejected=$false
        try{$null=$readPaths.Invoke([string]$path)}catch{if($_.Exception.ToString() -notmatch 'InvalidDataException'){throw};$rejected=$true}
        if(-not $rejected){throw ('Invalid configuration accepted: '+$case)}
    }
    Write-Output 'Reborn: Studio paths: PASS (EA CLR4 CodeDOM; live WorldBuilder setting, legacy keys, unknown/duplicate/missing-key controls; no UI or mod build).'
}finally{$provider.Dispose()}
