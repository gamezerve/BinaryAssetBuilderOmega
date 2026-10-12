# Reborn: prepare a fresh local-use Studio workspace from an explicit allowlist; no game, registry, reference modification or public redistribution.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$OutputDirectory,
    [Parameter(Mandatory)][string]$PowerShell7,
    [Parameter(Mandatory)][string]$StaticManifest,
    [Parameter(Mandatory)][string]$GlobalManifest,
    [string]$StudioDirectory)
$ErrorActionPreference='Stop'
$repo=Split-Path -Parent $PSScriptRoot
if($PSVersionTable.PSVersion.Major-lt 7){throw 'PowerShell 7 is required.'}
if(-not $StudioDirectory){$StudioDirectory=Join-Path $repo 'Working RA3 Compiler for Reference'}

#-------------------------------------------------------------------------------------------------
# Reborn: validate fully resolved ancestry before copying only named files into a fresh owned tree.
#-------------------------------------------------------------------------------------------------
function Test-WorkspacePath([string]$Path){
    if(-not [IO.Path]::IsPathFullyQualified($Path) -or $Path.Contains('"') -or $Path.Contains("`r") -or $Path.Contains("`n")){throw 'Workspace paths must be absolute and unambiguous.'}
    $current=$Path
    while(-not (Test-Path -LiteralPath $current)){
        $next=Split-Path -Parent $current
        if(-not $next -or $next-eq $current){throw 'Workspace ancestry cannot be resolved.'}
        $current=$next
    }
    for($item=Get-Item -LiteralPath $current;$null-ne $item;$item=if($item.PSIsContainer){$item.Parent}else{$item.Directory}){
        if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected workspace path refused.'}
    }
}
$OutputDirectory=[IO.Path]::GetFullPath($OutputDirectory)
foreach($path in @($OutputDirectory,$PowerShell7,$StaticManifest,$GlobalManifest,$StudioDirectory)){Test-WorkspacePath $path}
if(Test-Path -LiteralPath $OutputDirectory){throw 'Alpha workspace output already exists; nothing was overwritten.'}
foreach($path in @($PowerShell7,$StaticManifest,$GlobalManifest)){if(-not (Test-Path -LiteralPath $path -PathType Leaf)){throw ('Required file is missing: '+$path)}}
if(-not (Test-Path -LiteralPath $StudioDirectory -PathType Container)){throw 'Existing user-supplied Studio directory is required.'}
# Reborn: enforce the compiler's conservative config path bound before creating the workspace.
$probe=Join-Path $OutputDirectory 'builtmods/RebornAlphaInput-00000000000000000000000000000000/build/weapon-00000000/weaponprobe_1.0.cfg'
if($probe.Length-gt 240 -or $probe.ToCharArray().Where({[int]$_-gt 126 -or [int]$_-lt 32}).Count-gt 0){throw 'Choose a shorter ASCII workspace path (generated config must fit 240 characters).'}

$compiler=Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/bin/x86/Release/net8.0'
# Reborn: build once at preparation time, never rely on stale binaries or copy every build-directory artifact.
$buildLog=Join-Path $repo ('artifacts/RebornAlphaWorkspaceBuild-'+[guid]::NewGuid().ToString('N')+'.log')
# Reborn: distribution candidates must not embed local managed PDB paths; normal developer builds retain their own debug defaults.
# Reborn: force recompilation when switching from ordinary developer symbols; incremental outputs can otherwise retain old CodeView paths.
& dotnet build (Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/BinaryAssetBuilder.ManifestInspector.csproj') --no-restore --no-incremental -c Release -p:Platform=x86 -p:DebugType=None -p:DebugSymbols=false -m:1 -nr:false -v:q *> $buildLog
if($LASTEXITCODE-ne 0){throw ('Workspace compiler build failed; see '+$buildLog)}
$files=[Collections.Generic.List[object]]::new()
foreach($name in @('BinaryAssetBuilder.ManifestInspector.exe','BinaryAssetBuilder.ManifestInspector.dll',
    'BinaryAssetBuilder.ManifestInspector.deps.json','BinaryAssetBuilder.ManifestInspector.runtimeconfig.json',
    'BinaryAssetBuilder.Core.dll','BinaryAssetBuilder.Utility.dll','BinaryAssetBuilder.XmlCompiler.dll','SageBinaryData.dll')){
    $files.Add([pscustomobject]@{Source=(Join-Path $compiler $name);Relative=('tools/compiler/'+$name);Origin='project-build'})
}
foreach($relative in @('scripts/Invoke-Ra3Ep1AlphaBuild.ps1','LICENSE','alpha/README.md',
    'tests/fixtures/ReferencePipeline.xsd','tests/fixtures/ArmorDocumentPipeline.xsd',
    'tests/fixtures/ArmorSourcePipeline.xsd',
    'tests/fixtures/ArmorTokenPipeline.xsd','tests/fixtures/AlliedConYardArmorProbe.xml',
    'schemas/ra3ep1/xsd/Includes/Base.xsd','schemas/ra3ep1/xsd/Base/AssetBase.xsd',
    'schemas/ra3ep1/xsd/Includes/Ref.xsd','schemas/ra3ep1/xsd/Includes/DamageDef.xsd','schemas/ra3ep1/xsd/AssetTypeArmorTemplate.xsd')){
    $destination=if($relative-eq 'alpha/README.md'){'README.md'}else{$relative}
    $files.Add([pscustomobject]@{Source=(Join-Path $repo $relative);Relative=$destination;Origin='repository-reference-or-code'})
}
# Reborn: copy only the reviewed weapon schema graph plus enum drift-check source, not the full SDK schema catalog.
foreach($relative in @('tests/fixtures/WeaponDocumentPipeline.xsd','tests/fixtures/WeaponAuthoringProbe.xml',
    'schemas/ra3ep1/xsd/Includes/ModelState.xsd','schemas/ra3ep1/xsd/Includes/ObjectStatus.xsd',
    'schemas/ra3ep1/xsd/Includes/KindOf.xsd','schemas/ra3ep1/xsd/Includes/ObjectFilter.xsd',
    'schemas/ra3ep1/xsd/Includes/GlobalGameData.xsd','schemas/ra3ep1/xsd/Includes/View.xsd',
    'schemas/ra3ep1/xsd/Includes/Common.xsd','schemas/ra3ep1/xsd/Includes/Death.xsd',
    'schemas/ra3ep1/xsd/Includes/DamageFX.xsd','schemas/ra3ep1/xsd/Includes/DistTestType.xsd',
    'schemas/ra3ep1/xsd/AssetTypeAttributeModifier.xsd','schemas/ra3ep1/xsd/AssetTypeShaderOverride.xsd',
    'schemas/ra3ep1/xsd/AssetTypeAudio.xsd','schemas/ra3ep1/xsd/AssetTypeFXList.xsd',
    'schemas/ra3ep1/xsd/AssetTypeWeaponTemplate.xsd','schemas/ra3ep1/xsd/Modules/BaseModules.xsd')){
    $files.Add([pscustomobject]@{Source=(Join-Path $repo $relative);Relative=$relative;Origin='repository-reference-or-code'})
}
# Reborn: the complete stock-shaped weapon sample edits only Damage; no armor asset is bundled with it.
$files.Add([pscustomobject]@{Source=(Join-Path $repo 'alpha/mods/ConscriptAK47/data/mod.xml');Relative='mods/ConscriptAK47/data/mod.xml';Origin='project-sample'})
# Reborn: explicitly copy the entire reviewed source sample closure, never scan/include arbitrary mod directories.
foreach($name in @('mod.xml','Base.xml','Defs.xml')){
    $files.Add([pscustomobject]@{Source=(Join-Path $repo ('alpha/mods/ArmorSource/data/'+$name));Relative=('mods/ArmorSource/data/'+$name);Origin='project-sample'})
}
$files.Add([pscustomobject]@{Source=(Join-Path $repo 'alpha/studio/defaultscript.cs');Relative='defaultscript.cs';Origin='project-adapter'})
$files.Add([pscustomobject]@{Source=(Join-Path $repo 'alpha/Documentation/index.htm');Relative='Documentation/index.htm';Origin='project-local-help'})
$files.Add([pscustomobject]@{Source=(Join-Path $repo 'alpha/studio/EALAModStudio.exe.config');Relative='EALAModStudio.exe.config';Origin='project-isolated-CLR4-config'})
$files.Add([pscustomobject]@{Source=(Join-Path $repo 'alpha/mods/Reborn/data/mod.xml');Relative='mods/Reborn/data/mod.xml';Origin='project-sample'})
# Reborn: include a non-GUN editable example without claiming gameplay validation or enabling another asset identity.
$files.Add([pscustomobject]@{Source=(Join-Path $repo 'alpha/mods/ArmorCannon/data/mod.xml');Relative='mods/ArmorCannon/data/mod.xml';Origin='project-sample'})
$files.Add([pscustomobject]@{Source=(Join-Path $repo 'samples/armor/AlliedConYardArmor.xml');Relative='mods/Armor250/data/mod.xml';Origin='project-sample'})
# Reborn: expose only the separately stock-matched Soviet yard identity; still one armor per selected mod.
$files.Add([pscustomobject]@{Source=(Join-Path $repo 'samples/armor/SovietConYardArmor.xml');Relative='mods/SovietArmor/data/mod.xml';Origin='project-sample'})
# Reborn: package the bounded two-identity sample; its pair path never replays two independent single-asset archives.
$files.Add([pscustomobject]@{Source=(Join-Path $repo 'alpha/mods/CombinedArmor/data/mod.xml');Relative='mods/CombinedArmor/data/mod.xml';Origin='project-sample'})
# Reborn: use the <=15-character Studio name CombinedStrong for the high-contrast pair; preserve the original sample.
$files.Add([pscustomobject]@{Source=(Join-Path $repo 'alpha/mods/CombinedArmorStrong/data/mod.xml');Relative='mods/CombinedStrong/data/mod.xml';Origin='project-sample'})
foreach($name in @('EALAModStudio.exe','EALABuild.dll','EULA for RA3 MOD SDK.rtf')){
    $files.Add([pscustomobject]@{Source=(Join-Path $StudioDirectory $name);Relative=$name;Origin='user-supplied-EA-SDK-local-use-only'})
}
# Reborn: preflight the entire inventory before creating output; hashes bind the copies and preserve provenance.
foreach($entry in $files){
    Test-WorkspacePath $entry.Source
    if(-not (Test-Path -LiteralPath $entry.Source -PathType Leaf)){throw ('Workspace source missing: '+$entry.Source)}
    $entry|Add-Member -NotePropertyName Sha256 -NotePropertyValue (Get-FileHash -LiteralPath $entry.Source -Algorithm SHA256).Hash
}
$null=New-Item -ItemType Directory -Path $OutputDirectory
foreach($entry in $files){
    $destination=Join-Path $OutputDirectory $entry.Relative
    $null=New-Item -ItemType Directory -Path (Split-Path -Parent $destination) -Force
    Copy-Item -LiteralPath $entry.Source -Destination $destination
    if((Get-FileHash -LiteralPath $destination -Algorithm SHA256).Hash-ne $entry.Sha256 -or (Get-FileHash -LiteralPath $entry.Source -Algorithm SHA256).Hash-ne $entry.Sha256){throw 'Workspace copy/source hash mismatch.'}
}
$null=New-Item -ItemType Directory -Path (Join-Path $OutputDirectory 'builtmods')
# Reborn: generated configuration points only to this workspace's driver/compiler and explicitly supplied runtime/stock files.
[IO.File]::WriteAllLines((Join-Path $OutputDirectory 'uprising-alpha.paths'),@(
    '# Reborn: generated local-use paths; update Driver/CompilerDirectory after relocation.',
    ('PowerShell7='+$PowerShell7),('StaticManifest='+$StaticManifest),('GlobalManifest='+$GlobalManifest),
    ('Driver='+(Join-Path $OutputDirectory 'scripts/Invoke-Ra3Ep1AlphaBuild.ps1')),
    ('CompilerDirectory='+(Join-Path $OutputDirectory 'tools/compiler'))))
$report=[pscustomobject]@{Workspace=$OutputDirectory;CreatedUtc=[DateTime]::UtcNow.ToString('o');
    Files=$files;CompilerBuildLog=$buildLog;ReferenceSourcesUnchanged=$true;SourceRepositoryNeededForBuild=$false;
    RuntimePrerequisites=@('PowerShell 7','x86 .NET 8 runtime','EA Studio compatible .NET Framework 4/CodeDOM environment');
    GameExecuted=$false;StudioExecuted=$false;RegistryModified=$false;InstalledFilesModified=$false;
    PublicRedistributionReviewed=$false;PortableUiValidated=$false;NextGate='Offline clean-directory backend test, then separately observed Studio script/dropdown/log/callback test.'}
[IO.File]::WriteAllText((Join-Path $OutputDirectory 'workspace-preparation.json'),($report|ConvertTo-Json -Depth 6))
$report
