# Reborn: simulate a copied configured overlay at a new location, repair paths and repeat offline sample builds; preserve the original workspace.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$Workspace,[Parameter(Mandatory)][string]$OutputDirectory,
    [Parameter(Mandatory)][string]$StaticManifest,[string]$GlobalManifest)
$ErrorActionPreference='Stop'
if($PSVersionTable.PSVersion.Major-lt 7){throw 'PowerShell 7 is required.'}
$Workspace=[IO.Path]::GetFullPath($Workspace);$OutputDirectory=[IO.Path]::GetFullPath($OutputDirectory)
if((Split-Path -Parent $Workspace)-cne (Split-Path -Parent $OutputDirectory) -or (Test-Path -LiteralPath $OutputDirectory)){throw 'Relocation test requires a fresh sibling directory.'}
for($item=Get-Item -LiteralPath $Workspace;$null-ne $item;$item=$item.Parent){if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected workspace refused.'}}
$manifest=Get-Content -LiteralPath (Join-Path $Workspace 'overlay-manifest.json') -Raw|ConvertFrom-Json
$copies=@($manifest.Files)+@($manifest.Requirements)+@([pscustomobject]@{Relative='overlay-manifest.json'},[pscustomobject]@{Relative='uprising-alpha.paths'})
foreach($entry in $copies){
    if([IO.Path]::IsPathRooted($entry.Relative) -or $entry.Relative.Contains('..') -or $entry.Relative.Contains(':')){throw 'Invalid copy inventory.'}
    $source=Join-Path $Workspace $entry.Relative
    for($item=Get-Item -LiteralPath $source;$null-ne $item;$item=if($item.PSIsContainer){$item.Parent}else{$item.Directory}){if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected copy input refused.'}}
}
$null=New-Item -ItemType Directory -Path $OutputDirectory
foreach($entry in $copies){$target=Join-Path $OutputDirectory $entry.Relative;$null=New-Item -ItemType Directory -Path (Split-Path -Parent $target) -Force;Copy-Item -LiteralPath (Join-Path $Workspace $entry.Relative) -Destination $target}
$null=New-Item -ItemType Directory -Path (Join-Path $OutputDirectory 'builtmods')
# Reborn: owned output sentinel verifies path repair never clears existing build output.
$sentinel=Join-Path $OutputDirectory 'builtmods/retained.txt'
[IO.File]::WriteAllText($sentinel,'Reborn: retained output sentinel.')
$config=Join-Path $OutputDirectory 'uprising-alpha.paths'
$oldHash=(Get-FileHash -LiteralPath $config).Hash
$originalHash=(Get-FileHash -LiteralPath (Join-Path $Workspace 'uprising-alpha.paths')).Hash
$runtime=(Get-Process -Id $PID).Path
$helper=Join-Path $OutputDirectory 'scripts/Update-Ra3Ep1AlphaPaths.ps1'
# Reborn: missing external file must refuse without touching configuration or creating a backup.
& $runtime -NoProfile -File $helper -StaticManifest (Join-Path $OutputDirectory 'missing.manifest') *> (Join-Path $OutputDirectory 'missing-test.log')
if($LASTEXITCODE-eq 0 -or (Get-FileHash -LiteralPath $config).Hash-cne $oldHash -or @(Get-ChildItem -LiteralPath $OutputDirectory -Filter '*.bak').Count){throw 'Missing external dependency changed configuration.'}
& $runtime -NoProfile -File $helper *> (Join-Path $OutputDirectory 'relocation-test.log')
if($LASTEXITCODE-ne 0){throw 'Relocation path repair failed.'}
$backups=@(Get-ChildItem -LiteralPath $OutputDirectory -Filter 'uprising-alpha.paths.*.bak')
if($backups.Count-ne 1 -or (Get-FileHash -LiteralPath $backups[0].FullName).Hash-cne $oldHash){throw 'Relocation recovery backup differs.'}
$text=Get-Content -LiteralPath $config -Raw
if(-not $text.Contains('Driver='+(Join-Path $OutputDirectory 'scripts/Invoke-Ra3Ep1AlphaBuild.ps1')) -or
    -not $text.Contains('CompilerDirectory='+(Join-Path $OutputDirectory 'tools/compiler'))){throw 'Paths still refer to old workspace.'}
# Reborn: relocation must retain explicit weapon metadata rather than silently reverting to armor-only settings.
if($GlobalManifest -and -not $text.Contains('GlobalManifest='+$GlobalManifest)){throw 'Relocation lost explicit GlobalManifest.'}
if((Get-Content -LiteralPath $sentinel -Raw)-cne 'Reborn: retained output sentinel.' -or (Get-FileHash -LiteralPath (Join-Path $Workspace 'uprising-alpha.paths')).Hash-cne $originalHash){throw 'Original config or retained output changed.'}
# Reborn: exercise malformed settings only in the disposable relocated copy, then restore its valid configuration bytes.
$validBytes=[IO.File]::ReadAllBytes($config)
[IO.File]::AppendAllText($config,('Driver='+$helper+[Environment]::NewLine))
$duplicateHash=(Get-FileHash -LiteralPath $config).Hash
& $runtime -NoProfile -File $helper *> (Join-Path $OutputDirectory 'duplicate-test.log')
if($LASTEXITCODE-eq 0 -or (Get-FileHash -LiteralPath $config).Hash-cne $duplicateHash -or @(Get-ChildItem -LiteralPath $OutputDirectory -Filter 'uprising-alpha.paths.*.bak').Count-ne 1){throw 'Duplicate config keys were not refused without mutation.'}
[IO.File]::WriteAllBytes($config,$validBytes)
# Reborn: reuse the established offline regression only with a local synthesized inventory, never archive this test provenance.
$inventory=@($manifest.Files)+@($manifest.Requirements)
[IO.File]::WriteAllText((Join-Path $OutputDirectory 'workspace-preparation.json'),([pscustomobject]@{Files=$inventory}|ConvertTo-Json -Depth 6))
$builds=& (Join-Path $PSScriptRoot 'Test-Ra3Ep1AlphaWorkspace.ps1') -Workspace $OutputDirectory -StaticManifest $StaticManifest -GlobalManifest $GlobalManifest
$report=[pscustomobject]@{RelocatedCopy=$OutputDirectory;InternalPathsUpdated=$true;OriginalConfigurationUnchanged=$true;
    ExactBackupVerified=$true;MissingDependencyRefusedWithoutMutation=$true;DuplicateKeysRefusedWithoutMutation=$true;ExistingOutputPreserved=$true;
    WorkspaceTest=$builds;StudioExecuted=$false;GameExecuted=$false}
[IO.File]::WriteAllText((Join-Path $OutputDirectory 'relocation-test.json'),($report|ConvertTo-Json -Depth 7))
$report
