# Reborn: verify an overlay archive's external dependency boundary, clean setup and offline sample builds; no GUI/game/public upload.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$Archive,[Parameter(Mandatory)][string]$OutputDirectory,
    [Parameter(Mandatory)][string]$StudioDirectory,[Parameter(Mandatory)][string]$SchemaDirectory,
    [Parameter(Mandatory)][string]$StaticManifest)
$ErrorActionPreference='Stop'
if($PSVersionTable.PSVersion.Major-lt 7){throw 'PowerShell 7 is required.'}
$OutputDirectory=[IO.Path]::GetFullPath($OutputDirectory)
$negative=$OutputDirectory+'-refusal'
foreach($path in @($OutputDirectory,$negative)){if(Test-Path -LiteralPath $path){throw 'Overlay test output occupied.'}}
# Reborn: inspect ZIP paths before extraction; only tested relative files plus the path-free manifest may be present.
$zip=[IO.Compression.ZipFile]::OpenRead($Archive)
try{
    foreach($entry in $zip.Entries){
        $name=$entry.FullName.Replace('\','/')
        if($name.StartsWith('/') -or $name.Contains('..') -or $name.Contains(':') -or
            $name -match '(^|/)(EALAModStudio.exe|EALABuild.dll|workspace-preparation.json|uprising-alpha.paths|overlay-setup.json)$' -or
            $name.StartsWith('schemas/') -or $name.StartsWith('builtmods/') -or $name.Contains('EULA')){throw ('Disallowed overlay archive entry: '+$name)}
    }
}finally{$zip.Dispose()}
Expand-Archive -LiteralPath $Archive -DestinationPath $OutputDirectory
Expand-Archive -LiteralPath $Archive -DestinationPath $negative
$runtime=(Get-Process -Id $PID).Path
$manifest=Get-Content -LiteralPath (Join-Path $OutputDirectory 'overlay-manifest.json') -Raw|ConvertFrom-Json
if($manifest.Files.Count-ne 29 -or $manifest.Requirements.Count-ne 8){throw 'Unexpected overlay inventory.'}
# Reborn: require complete pinned notice files and preserved project attribution, not just a release flag.
foreach($relative in @('LICENSE','THIRD_PARTY_NOTICES.md','notices/dotnet/LICENSE.txt','notices/dotnet/ThirdPartyNotices.txt')){
    $entry=@($manifest.Files|Where-Object Relative -CEQ $relative)
    if($entry.Count-ne 1 -or (Get-FileHash -LiteralPath (Join-Path $OutputDirectory $relative)).Hash-cne $entry[0].Sha256){throw 'Required distribution notice missing or changed.'}
}
if(-not $manifest.SuppliedDotNetNoticesIncluded -or $manifest.ExactHostNoticeCoverageReviewed){throw 'Notice coverage state differs from candidate scope.'}
$manifestText=Get-Content -LiteralPath (Join-Path $OutputDirectory 'overlay-manifest.json') -Raw
if($manifestText -match '(?i)[a-z]:\\|C:/Users/|D:/|"Source"|"Workspace"'){throw 'Manifest exposes absolute provenance.'}
# Reborn: alter only a disposable negative manifest digest, never the user SDK or schema files.
$negativeManifest=Get-Content -LiteralPath (Join-Path $negative 'overlay-manifest.json') -Raw|ConvertFrom-Json
$negativeManifest.Requirements[0].Sha256='0'*64
[IO.File]::WriteAllText((Join-Path $negative 'overlay-manifest.json'),($negativeManifest|ConvertTo-Json -Depth 6))
& $runtime -NoProfile -File (Join-Path $negative 'scripts/Initialize-Ra3Ep1AlphaOverlay.ps1') -StudioDirectory $StudioDirectory -SchemaDirectory $SchemaDirectory -StaticManifest $StaticManifest -PowerShell7 $runtime *> (Join-Path $negative 'refusal.log')
if($LASTEXITCODE-eq 0 -or (Test-Path -LiteralPath (Join-Path $negative 'overlay-setup.json')) -or (Test-Path -LiteralPath (Join-Path $negative 'EALAModStudio.exe')) -or (Test-Path -LiteralPath (Join-Path $negative 'builtmods'))){throw 'Wrong dependency digest was not refused before setup.'}
& $runtime -NoProfile -File (Join-Path $OutputDirectory 'scripts/Initialize-Ra3Ep1AlphaOverlay.ps1') -StudioDirectory $StudioDirectory -SchemaDirectory $SchemaDirectory -StaticManifest $StaticManifest -PowerShell7 $runtime *> (Join-Path $OutputDirectory 'setup-test.log')
if($LASTEXITCODE-ne 0){throw 'Clean overlay setup failed.'}
$configHash=(Get-FileHash -LiteralPath (Join-Path $OutputDirectory 'uprising-alpha.paths')).Hash
& $runtime -NoProfile -File (Join-Path $OutputDirectory 'scripts/Initialize-Ra3Ep1AlphaOverlay.ps1') -StudioDirectory $StudioDirectory -SchemaDirectory $SchemaDirectory -StaticManifest $StaticManifest -PowerShell7 $runtime *> (Join-Path $OutputDirectory 'occupied-test.log')
if($LASTEXITCODE-eq 0 -or (Get-FileHash -LiteralPath (Join-Path $OutputDirectory 'uprising-alpha.paths')).Hash-cne $configHash){throw 'Occupied setup did not preserve configuration.'}
# Reborn: synthesize only a local test inventory to reuse offline build checks; never put this provenance in the overlay ZIP.
$inventory=[Collections.Generic.List[object]]::new()
foreach($entry in @($manifest.Files)+@($manifest.Requirements)){$inventory.Add([pscustomobject]@{Relative=$entry.Relative;Sha256=$entry.Sha256})}
[IO.File]::WriteAllText((Join-Path $OutputDirectory 'workspace-preparation.json'),([pscustomobject]@{Files=$inventory}|ConvertTo-Json -Depth 6))
$builds=& (Join-Path $PSScriptRoot 'Test-Ra3Ep1AlphaWorkspace.ps1') -Workspace $OutputDirectory -StaticManifest $StaticManifest
$report=[pscustomobject]@{ArchiveSha256=(Get-FileHash -LiteralPath $Archive).Hash;ExternalFilesAbsentFromArchive=$true;
    ManifestHasNoAbsoluteProvenance=$true;DependencyMismatchRefusedBeforeCopy=$true;CleanSetupPassed=$true;
    OccupiedSetupRefused=$true;RequiredNoticeHashesPassed=$true;WorkspaceTest=$builds;PublicRedistributionReviewed=$false;StudioExecuted=$false;GameExecuted=$false}
[IO.File]::WriteAllText((Join-Path $OutputDirectory 'overlay-test.json'),($report|ConvertTo-Json -Depth 7))
$report
