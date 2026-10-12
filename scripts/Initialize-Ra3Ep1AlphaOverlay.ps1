# Reborn: configure an extracted overlay with explicitly supplied EA components/schema files; no downloads, registry, game launch or stock modification.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$StudioDirectory,
    [Parameter(Mandatory)][string]$SchemaDirectory,
    [Parameter(Mandatory)][string]$StaticManifest,
    [string]$GlobalManifest,
    [Parameter(Mandatory)][string]$PowerShell7)
$ErrorActionPreference='Stop'
if($PSVersionTable.PSVersion.Major-lt 7){throw 'PowerShell 7 is required.'}
$workspace=Split-Path -Parent $PSScriptRoot

#-------------------------------------------------------------------------------------------------
# Reborn: require unambiguous existing paths with no redirected ancestry before copying user dependencies.
#-------------------------------------------------------------------------------------------------
function Test-OverlayPath([string]$Path){
    if(-not [IO.Path]::IsPathFullyQualified($Path) -or $Path.IndexOfAny([char[]]@('"',"`r","`n"))-ge 0){throw 'Paths must be absolute and unambiguous.'}
    for($item=Get-Item -LiteralPath $Path;$null-ne $item;$item=if($item.PSIsContainer){$item.Parent}else{$item.Directory}){
        if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected overlay path refused.'}
    }
}

#-------------------------------------------------------------------------------------------------
# Reborn: confine every manifest-relative dependency destination to the extracted overlay.
#-------------------------------------------------------------------------------------------------
function Get-OverlayTarget([string]$Relative){
    if(-not $Relative -or [IO.Path]::IsPathRooted($Relative) -or $Relative.Contains('..') -or $Relative.Contains(':')){throw 'Invalid overlay relative path.'}
    $target=[IO.Path]::GetFullPath((Join-Path $workspace $Relative))
    if(-not $target.StartsWith($workspace+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)){throw 'Overlay target escapes workspace.'}
    $ancestor=Split-Path -Parent $target
    while(-not (Test-Path -LiteralPath $ancestor)){$ancestor=Split-Path -Parent $ancestor}
    Test-OverlayPath $ancestor
    return $target
}
foreach($path in @($workspace,$StudioDirectory,$SchemaDirectory,$StaticManifest,$PowerShell7)){Test-OverlayPath $path}
foreach($path in @($StaticManifest,$PowerShell7)){if(-not (Test-Path -LiteralPath $path -PathType Leaf)){throw 'Required file is missing.'}}
# Reborn: weapon-enabled overlays require explicit metadata; legacy armor overlays may omit it.
if(Test-Path -LiteralPath (Join-Path $workspace 'mods/ConscriptAK47/data/mod.xml')){
    if(-not $GlobalManifest){throw 'Weapon-enabled overlay requires GlobalManifest.'}
    Test-OverlayPath $GlobalManifest
    if(-not (Test-Path -LiteralPath $GlobalManifest -PathType Leaf)){throw 'Global manifest is missing.'}
}
foreach($extension in @('.bin','.relo','.imp')){Test-OverlayPath ([IO.Path]::ChangeExtension($StaticManifest,$extension))}
$config=Join-Path $workspace 'uprising-alpha.paths'
$receipt=Join-Path $workspace 'overlay-setup.json'
$outputs=Join-Path $workspace 'builtmods'
foreach($path in @($config,$receipt,$outputs)){if(Test-Path -LiteralPath $path){throw 'Overlay is already configured or occupied; nothing overwritten.'}}
$probe=Join-Path $workspace 'builtmods/RebornAlphaInput-00000000000000000000000000000000/build/package/reborn_1.0.cfg'
if($probe.Length-gt 240 -or $probe.ToCharArray().Where({[int]$_-gt 126 -or [int]$_-lt 32}).Count){throw 'Choose a shorter ASCII overlay path.'}
$manifest=Get-Content -LiteralPath (Join-Path $workspace 'overlay-manifest.json') -Raw|ConvertFrom-Json
if($manifest.Format-cne 'RebornAlphaOverlay1'){throw 'Unknown overlay manifest format.'}
$names=[Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
foreach($entry in $manifest.Files){
    $target=Get-OverlayTarget $entry.Relative
    if(-not $names.Add($entry.Relative)){throw 'Duplicate overlay inventory path.'}
    Test-OverlayPath $target
    if((Get-FileHash -LiteralPath $target -Algorithm SHA256).Hash-cne $entry.Sha256){throw ('Overlay inventory differs: '+$entry.Relative)}
}
$copies=[Collections.Generic.List[object]]::new()
foreach($entry in $manifest.Requirements){
    $target=Get-OverlayTarget $entry.Relative
    if(-not $names.Add($entry.Relative) -or (Test-Path -LiteralPath $target)){throw 'Dependency destination occupied or duplicated.'}
    if($entry.Kind-ceq 'EA-Studio'){$source=Join-Path $StudioDirectory $entry.InputRelative}
    elseif($entry.Kind-ceq 'EA-Schema'){$source=Join-Path $SchemaDirectory $entry.InputRelative}
    else{throw 'Unknown external dependency kind.'}
    # Reborn: validate dependency-relative paths separately before joining them to the user-supplied root.
    if([IO.Path]::IsPathRooted($entry.InputRelative) -or $entry.InputRelative.Contains('..') -or $entry.InputRelative.Contains(':')){throw 'Invalid dependency input path.'}
    Test-OverlayPath $source
    if((Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash-cne $entry.Sha256){throw ('User dependency version differs: '+$entry.InputRelative)}
    $copies.Add([pscustomobject]@{Source=$source;Target=$target;Relative=$entry.Relative;Sha256=$entry.Sha256})
}
# Reborn: only after the whole input set passes may new isolated dependency files be created; never overwrite SDK originals.
foreach($copy in $copies){
    $null=New-Item -ItemType Directory -Path (Split-Path -Parent $copy.Target) -Force
    Copy-Item -LiteralPath $copy.Source -Destination $copy.Target
    if((Get-FileHash -LiteralPath $copy.Target -Algorithm SHA256).Hash-cne $copy.Sha256){throw 'Dependency copy hash differs; keep this partial workspace for inspection.'}
}
$null=New-Item -ItemType Directory -Path $outputs
[IO.File]::WriteAllLines($config,@('# Reborn: local explicit configuration; never include this file in public archives.',
    ('PowerShell7='+$PowerShell7),('StaticManifest='+$StaticManifest),
    ('Driver='+(Join-Path $workspace 'scripts/Invoke-Ra3Ep1AlphaBuild.ps1')),
    ('CompilerDirectory='+(Join-Path $workspace 'tools/compiler'))))
if($GlobalManifest){[IO.File]::AppendAllLines($config,[string[]]@('GlobalManifest='+$GlobalManifest))}
# Reborn: keep personal setup provenance local, separate from the path-free distributable manifest.
$report=[pscustomobject]@{Workspace=$workspace;DependenciesVerified=$true;DependencyCount=$copies.Count;
    GameExecuted=$false;StudioExecuted=$false;RegistryModified=$false;InstalledFilesModified=$false}
[IO.File]::WriteAllText($receipt,($report|ConvertTo-Json))
$report
