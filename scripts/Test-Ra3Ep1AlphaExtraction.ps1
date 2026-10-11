# Reborn: create a LOCAL-USE-ONLY candidate ZIP from the prepared inventory, extract fresh and test offline; never upload or launch Studio/game.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$Workspace,[Parameter(Mandatory)][string]$OutputDirectory,
    [Parameter(Mandatory)][string]$StaticManifest)
$ErrorActionPreference='Stop'
if($PSVersionTable.PSVersion.Major-lt 7){throw 'PowerShell 7 is required.'}
$Workspace=[IO.Path]::GetFullPath($Workspace);$OutputDirectory=[IO.Path]::GetFullPath($OutputDirectory)
$archive=$OutputDirectory+'-LOCAL-USE-ONLY.zip'
# Reborn: permit only a fresh sibling under the same checked parent, so packaging cannot target or overwrite a broader user directory.
if((Split-Path -Parent $Workspace)-cne (Split-Path -Parent $OutputDirectory)){throw 'Extraction test output must be a fresh sibling of the prepared workspace.'}
foreach($path in @($OutputDirectory,$archive)){if(Test-Path -LiteralPath $path){throw 'Extraction test target already exists.'}}
for($item=Get-Item -LiteralPath $Workspace;$null-ne $item;$item=$item.Parent){if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected extraction source refused.'}}
$inventory=Get-Content -LiteralPath (Join-Path $Workspace 'workspace-preparation.json') -Raw|ConvertFrom-Json
$staging=$OutputDirectory+'-staging'
if(Test-Path -LiteralPath $staging){throw 'Extraction staging already exists.'}
$null=New-Item -ItemType Directory -Path $staging
foreach($entry in $inventory.Files){
    if([IO.Path]::IsPathRooted($entry.Relative) -or $entry.Relative.Contains('..') -or $entry.Relative.Contains(':')){throw 'Extraction inventory path invalid.'}
    $source=Join-Path $Workspace $entry.Relative
    # Reborn: inspect each named input's complete ancestry, not just the workspace root and leaf file.
    for($item=Get-Item -LiteralPath $source;$null-ne $item;$item=if($item.PSIsContainer){$item.Parent}else{$item.Directory}){
        if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected extraction inventory refused.'}
    }
    if(((Get-Item -LiteralPath $source).Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0 -or (Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash-cne $entry.Sha256){throw 'Extraction input differs from inventory.'}
    $destination=Join-Path $staging $entry.Relative
    $null=New-Item -ItemType Directory -Path (Split-Path -Parent $destination) -Force
    Copy-Item -LiteralPath $source -Destination $destination
}
Copy-Item -LiteralPath (Join-Path $Workspace 'workspace-preparation.json') -Destination (Join-Path $staging 'workspace-preparation.json')
# Reborn: package only the freshly staged allowlist, not builtmods, personal config, logs, hidden source or cached test results.
Compress-Archive -Path (Join-Path $staging '*') -DestinationPath $archive
Expand-Archive -LiteralPath $archive -DestinationPath $OutputDirectory
$null=New-Item -ItemType Directory -Path (Join-Path $OutputDirectory 'builtmods')
$runtime=(Get-Process -Id $PID).Path
[IO.File]::WriteAllLines((Join-Path $OutputDirectory 'uprising-alpha.paths'),@(
    '# Reborn: extracted local test configuration; not public release settings.',('PowerShell7='+$runtime),
    ('StaticManifest='+$StaticManifest),('Driver='+(Join-Path $OutputDirectory 'scripts/Invoke-Ra3Ep1AlphaBuild.ps1')),
    ('CompilerDirectory='+(Join-Path $OutputDirectory 'tools/compiler'))))
$result=& (Join-Path $PSScriptRoot 'Test-Ra3Ep1AlphaWorkspace.ps1') -Workspace $OutputDirectory -StaticManifest $StaticManifest
# Reborn: extracted working path is recorded separately from the original preparation provenance; no claim of GUI or redistribution approval.
$report=[pscustomobject]@{Archive=$archive;ArchiveSha256=(Get-FileHash -LiteralPath $archive -Algorithm SHA256).Hash;
    ExtractedWorkspace=$OutputDirectory;OfflineExtractionTestPassed=$true;InventoryFileCount=$inventory.Files.Count;
    OriginalPreparationWorkspace=$inventory.Workspace;WorkspaceTest=$result;
    LocalUseOnly=$true;ContainsUserSuppliedEaFiles=$true;ContainsLocalProvenancePaths=$true;
    PublicRedistributionReviewed=$false;StudioExecuted=$false;GameExecuted=$false}
[IO.File]::WriteAllText((Join-Path $OutputDirectory 'extraction-test.json'),($report|ConvertTo-Json -Depth 7))
$report
