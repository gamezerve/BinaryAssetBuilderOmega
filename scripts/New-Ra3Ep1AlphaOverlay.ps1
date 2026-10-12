# Reborn: stage an overlay candidate from a verified local workspace, excluding external EA/schema files and all absolute provenance; no public upload.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$Workspace,[Parameter(Mandatory)][string]$OutputDirectory,
    [Parameter(Mandatory)][string]$DotNetNoticeDirectory)
$ErrorActionPreference='Stop'
if($PSVersionTable.PSVersion.Major-lt 7){throw 'PowerShell 7 is required.'}
$Workspace=[IO.Path]::GetFullPath($Workspace);$OutputDirectory=[IO.Path]::GetFullPath($OutputDirectory)
if((Split-Path -Parent $Workspace)-cne (Split-Path -Parent $OutputDirectory)){throw 'Overlay must be a fresh sibling of the local workspace.'}
$archive=$OutputDirectory+'-OVERLAY-CANDIDATE.zip'
$auditPath=$OutputDirectory+'-binary-audit.json'
foreach($path in @($OutputDirectory,$archive,$auditPath)){if(Test-Path -LiteralPath $path){throw 'Overlay output already exists; nothing overwritten.'}}
for($item=Get-Item -LiteralPath $Workspace;$null-ne $item;$item=$item.Parent){if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected workspace refused.'}}
$inventory=Get-Content -LiteralPath (Join-Path $Workspace 'workspace-preparation.json') -Raw|ConvertFrom-Json
$files=[Collections.Generic.List[object]]::new();$requirements=[Collections.Generic.List[object]]::new()
$copies=[Collections.Generic.List[object]]::new();$seen=[Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
foreach($entry in $inventory.Files){
    if(-not $entry.Relative -or [IO.Path]::IsPathRooted($entry.Relative) -or $entry.Relative.Contains('..') -or $entry.Relative.Contains(':') -or -not $seen.Add($entry.Relative)){throw 'Invalid or duplicate inventory path.'}
    $source=Join-Path $Workspace $entry.Relative
    for($item=Get-Item -LiteralPath $source;$null-ne $item;$item=if($item.PSIsContainer){$item.Parent}else{$item.Directory}){if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected inventory input refused.'}}
    if((Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash-cne $entry.Sha256){throw 'Source workspace inventory differs.'}
    # Reborn: EA executable/DLL/EULA and all official XSD graph files are requirements, not redistribution payload.
    if($entry.Origin-ceq 'user-supplied-EA-SDK-local-use-only'){
        $requirements.Add([pscustomobject]@{Relative=$entry.Relative;InputRelative=$entry.Relative;Kind='EA-Studio';Sha256=$entry.Sha256})
    }elseif($entry.Relative.StartsWith('schemas/ra3ep1/xsd/',[StringComparison]::Ordinal)){
        $requirements.Add([pscustomobject]@{Relative=$entry.Relative;InputRelative=$entry.Relative.Substring('schemas/ra3ep1/xsd/'.Length);Kind='EA-Schema';Sha256=$entry.Sha256})
    }else{
        $files.Add([pscustomobject]@{Relative=$entry.Relative;Sha256=$entry.Sha256})
        $copies.Add([pscustomobject]@{Source=$source;Relative=$entry.Relative;Sha256=$entry.Sha256})
    }
}
# Reborn: own setup/docs are explicit additions rather than copying arbitrary repository files.
foreach($pair in @(@('Initialize-Ra3Ep1AlphaOverlay.ps1','scripts/Initialize-Ra3Ep1AlphaOverlay.ps1'),@('Update-Ra3Ep1AlphaPaths.ps1','scripts/Update-Ra3Ep1AlphaPaths.ps1'),@('../alpha/OVERLAY.md','OVERLAY.md'),@('../alpha/THIRD_PARTY_NOTICES.md','THIRD_PARTY_NOTICES.md'))){
    $source=Join-Path $PSScriptRoot $pair[0];$hash=(Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash
    if(-not $seen.Add($pair[1])){throw 'Overlay addition duplicates inventory.'}
    $files.Add([pscustomobject]@{Relative=$pair[1];Sha256=$hash});$copies.Add([pscustomobject]@{Source=$source;Relative=$pair[1];Sha256=$hash})
}
# Reborn: preserve complete explicitly supplied vendor notice bytes, not an invented replacement license or automatic network download.
if(-not [IO.Path]::IsPathFullyQualified($DotNetNoticeDirectory)){throw 'Notice directory must be an explicit absolute path.'}
foreach($name in @('LICENSE.txt','ThirdPartyNotices.txt')){
    $source=Join-Path $DotNetNoticeDirectory $name
    for($item=Get-Item -LiteralPath $source;$null-ne $item;$item=if($item.PSIsContainer){$item.Parent}else{$item.Directory}){
        if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected notice input refused.'}
    }
    $noticeFile=Get-Item -LiteralPath $source
    if($noticeFile.PSIsContainer -or $noticeFile.Length-eq 0 -or $noticeFile.Length-gt 1MB){throw 'Notice file must be nonempty and at most 1 MiB.'}
    $relative='notices/dotnet/'+$name
    if(-not $seen.Add($relative)){throw 'Duplicate notice destination.'}
    $hash=(Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash
    $files.Add([pscustomobject]@{Relative=$relative;Sha256=$hash})
    $copies.Add([pscustomobject]@{Source=$source;Relative=$relative;Sha256=$hash})
}
# Reborn: armor-only legacy and weapon-enabled inventories have explicitly bounded external schema graphs.
if($requirements.Count -notin @(8,24)){throw 'Expected three Studio files plus the reviewed armor/weapon schema graph.'}
# Reborn: block a candidate with private path hints or absolute managed debug paths before creating the overlay; keep the audit report outside the ZIP.
$audit=& (Join-Path $PSScriptRoot 'Get-Ra3Ep1AlphaBinaryAudit.ps1') -CompilerDirectory (Join-Path $Workspace 'tools/compiler') -OutputPath $auditPath
if($audit.FilesWithPrivateHints.Count-gt 0 -or @($audit.Files|Where-Object {$_.Managed -and @($_.DebugEntries|Where-Object AbsolutePath).Count-gt 0}).Count-gt 0){throw 'Overlay contains private path hints or absolute managed debug paths; inspect the separate binary audit.'}
$null=New-Item -ItemType Directory -Path $OutputDirectory
foreach($copy in $copies){
    $target=Join-Path $OutputDirectory $copy.Relative;$null=New-Item -ItemType Directory -Path (Split-Path -Parent $target) -Force
    Copy-Item -LiteralPath $copy.Source -Destination $target
    if((Get-FileHash -LiteralPath $target -Algorithm SHA256).Hash-cne $copy.Sha256){throw 'Overlay copy differs.'}
}
# Reborn: export only relative file names and digests, never Source/Workspace/compiler log or personal path fields.
$manifest=[pscustomobject]@{Format='RebornAlphaOverlay1';Files=$files;Requirements=$requirements;
    PublicRedistributionReviewed=$false;RequiresUserSuppliedEaFiles=$true;SuppliedDotNetNoticesIncluded=$true;ExactHostNoticeCoverageReviewed=$false}
[IO.File]::WriteAllText((Join-Path $OutputDirectory 'overlay-manifest.json'),($manifest|ConvertTo-Json -Depth 6))
Compress-Archive -Path (Join-Path $OutputDirectory '*') -DestinationPath $archive
[pscustomobject]@{Archive=$archive;ArchiveSha256=(Get-FileHash -LiteralPath $archive -Algorithm SHA256).Hash;
    IncludedFiles=$files.Count;ExternalDependencies=$requirements.Count;PublicRedistributionReviewed=$false;GameExecuted=$false}
