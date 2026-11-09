# Reborn: compile direct Official map folders into isolated generations before publishing map_mod families into the mod's Misc tree.

#-------------------------------------------------------------------------------------------------
<# Reborn: read literal absolute paths from Studio's UTF-8 list without treating an empty selection as All. #>
#-------------------------------------------------------------------------------------------------
function Read-RebornMapSelection([string]$Path) {
    if([string]::IsNullOrWhiteSpace($Path) -or !(Test-Path -LiteralPath $Path -PathType Leaf)){
        throw 'Official map selection file is missing.'
    }
    $selected=@([IO.File]::ReadAllLines([IO.Path]::GetFullPath($Path),[Text.UTF8Encoding]::new($false,$true)))
    if(!$selected.Count -or @($selected | Where-Object {[string]::IsNullOrWhiteSpace($_) -or $_.Contains("`r") -or $_.Contains("`n")}).Count){
        throw 'Select at least one Official map or All.'
    }
    return $selected
}

#-------------------------------------------------------------------------------------------------
<# Reborn: validate every explicit folder before compiling any map, preserving All and the legacy single-map parameter. #>
#-------------------------------------------------------------------------------------------------
function Get-RebornOfficialMapFolders([string]$DataRoot,[string]$OfficialMapDirectory,[string[]]$OfficialMapDirectories) {
    if($OfficialMapDirectory -and $null-ne $OfficialMapDirectories){throw 'Choose a single map directory or a list of map directories, not both.'}
    $explicit=$OfficialMapDirectory -or $null-ne $OfficialMapDirectories
    $requested=if($OfficialMapDirectory){@($OfficialMapDirectory)}else{$OfficialMapDirectories}
    if($explicit -and !$requested.Count){throw 'Select at least one Official map or All.'}
    $root=Join-Path $DataRoot 'Maps/Official'
    if(-not(Test-Path -LiteralPath $root -PathType Container)){
        if($explicit){throw ('Selected Official maps directory is no longer available: '+$root)}
        Write-SdkBuildLine ('Skipped: [No input] Official maps directory not found: '+$root)
        return
    }
    $folders=@(Get-ChildItem -LiteralPath $root -Directory | Sort-Object Name)
    if($explicit){
        $names=[Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
        foreach($path in $requested){
            if([string]::IsNullOrWhiteSpace($path) -or ![IO.Path]::IsPathRooted($path)){throw 'Selected map folders must use absolute paths.'}
            $null=$names.Add([IO.Path]::GetFullPath($path).TrimEnd([IO.Path]::DirectorySeparatorChar,[IO.Path]::AltDirectorySeparatorChar))
        }
        $folders=@($folders | Where-Object {$names.Contains($_.FullName)})
        if($folders.Count-ne $names.Count){throw 'A selected Official map is no longer a direct folder of this mod.'}
        foreach($folder in $folders){
            if(-not(Test-Path -LiteralPath (Join-Path $folder.FullName 'map.xml') -PathType Leaf)){
                throw ('Selected map source not found: '+(Join-Path $folder.FullName 'map.xml'))
            }
        }
    }
    if(-not $folders){Write-SdkBuildLine ('Skipped: [No input] No map folders in: '+$root)}
    return $folders
}

#-------------------------------------------------------------------------------------------------
<# Reborn: discover maps, use the actual native source backend and retain all successful map files until the full map stage passes. #>
#-------------------------------------------------------------------------------------------------
function Build-RebornMaps([string]$DataRoot,[string]$Sdk,[hashtable]$Settings,[string]$StaticManifest,[string]$GlobalManifest,[string]$Compiler,[string]$Work,[string]$Detail,[string]$GlobalContext,[ref]$Result,[string]$ModStaticPackage,[string]$OfficialMapDirectory,[string[]]$OfficialMapDirectories,[string]$CacheRoot,[string]$CacheKey,[hashtable]$Witness,[switch]$ForceRebuild) {
    $plans=[Collections.Generic.List[object]]::new()
    $folders=@(Get-RebornOfficialMapFolders $DataRoot -OfficialMapDirectory $OfficialMapDirectory -OfficialMapDirectories $OfficialMapDirectories)
    $explicit=$OfficialMapDirectory -or $null-ne $OfficialMapDirectories
    foreach($folder in $folders){
        Assert-RebornPackagePath $folder.FullName
        $entry=Join-Path $folder.FullName 'map.xml'
        if(-not(Test-Path -LiteralPath $entry -PathType Leaf)){
            # Reborn: a specific map request must never succeed after its source disappears.
            if($explicit){throw ('Selected map source not found: '+$entry)}
            Write-SdkBuildLine ('Skipped: [No input] Map source not found: '+$entry);continue
        }
        $loose=@(Get-ChildItem -LiteralPath $folder.FullName -File | Where-Object Extension -in @('.map','.tga'))
        if(-not @($loose|Where-Object Extension -ieq '.map')){throw ('MAPFILE: map.xml has no accompanying .map file: '+$entry)}
        foreach($file in $loose){Assert-RebornPackagePath $file.FullName}
        $target=Join-Path $Work ('maps/'+$folder.Name);$null=New-Item -ItemType Directory -Path $target
        Write-SdkBuildLine ('Map source: '+$entry)
        if($CacheRoot -and !$ForceRebuild){
            $saved=Read-RebornMapCache $CacheRoot $CacheKey $folder.Name $entry $Witness $target
            if($saved){
                $plans.Add($saved)
                Write-SdkBuildLine ('Map cache hit: '+$folder.Name)
                # Reborn: a cache hit retains the source warnings of the compilation it replaces.
                foreach($warning in $saved.Warnings){Write-SdkCompilerLine $warning $Detail}
                continue
            }
        }
        # Reborn: bind loose file bytes before the compiler runs, alongside its XML and resource witnesses.
        $warnings=[Collections.Generic.List[string]]::new()
        $looseEvidence=@($loose | Sort-Object Name | ForEach-Object {[pscustomobject]@{Name=$_.Name;Sha256=[Reborn.PackageFile]::Hash($_.FullName)}})
        # Reborn: missing terrain sources warn and skip only this map; no placeholder atlas or partial map is published.
        & (Join-Path $Compiler 'BinaryAssetBuilder.ManifestInspector.exe') map-resource-preflight $entry $Sdk (Join-Path $target 'terrain-audit') 2>&1 | ForEach-Object {
            $line=[string]$_
            if($line -match '^\s*(?:\[[^\]]+\]\s*)?Warning:'){$warnings.Add($line)}
            Write-SdkCompilerLine $line $Detail
        }
        if($LASTEXITCODE-eq 3){Write-SdkBuildLine ('Warning: MAPSKIPPED '' '+$folder.Name+' '' Missing terrain resources; this map was not rebuilt. Existing Misc content is retained.');continue}
        if($LASTEXITCODE-ne 0){throw ('Map resource preflight failed: '+$entry)}
        $arguments=@('-NoProfile','-NonInteractive','-File',(Join-Path $PSScriptRoot 'Invoke-Ra3Ep1ConscriptAlphaBuild.ps1'),'-ModXml',$entry,'-StaticManifest',$StaticManifest,'-GlobalManifest',$GlobalManifest,'-CompilerDirectory',$Compiler,'-OutputRoot',$target)
        if($GlobalContext){$arguments+=@('-GlobalStreamContext',$GlobalContext)}
        # Reborn: the main static package is an external provider for this map, never another XML include.
        if($ModStaticPackage){$arguments+=@('-MapStaticPackage',$ModStaticPackage)}
        & $Settings.PowerShell7 @arguments 2>&1 | ForEach-Object {
            $line=[string]$_
            if($line -match '^\s*(?:\[[^\]]+\]\s*)?Warning:'){$warnings.Add($line)}
            Write-SdkCompilerLine $line $Detail
        }
        if($LASTEXITCODE-ne 0){throw ('Map compilation failed: '+$entry+'. No map output was copied to Misc. Review the source/native processor diagnostic above.')}
        $receipt=Get-Content -LiteralPath (Join-Path $target 'alpha-build.json') -Raw|ConvertFrom-Json
        $stage=Join-Path $target 'misc-map'
        & (Join-Path $Compiler 'BinaryAssetBuilder.ManifestInspector.exe') sdk-stage-map-package $receipt.PackagePath $stage 2>&1 | ForEach-Object {
            $line=[string]$_
            if($line -match '^\s*(?:\[[^\]]+\]\s*)?Warning:'){$warnings.Add($line)}
            Write-SdkCompilerLine $line $Detail
        }
        if($LASTEXITCODE-ne 0){throw ('Map stream staging failed: '+$entry)}
        foreach($file in $looseEvidence){
            $input=Join-Path $folder.FullName $file.Name;$destination=Join-Path $stage $file.Name
            Copy-Item -LiteralPath $input -Destination $destination
            if([Reborn.PackageFile]::Hash($destination)-cne $file.Sha256){throw ('Map source changed while copying: '+$input)}
        }
        $plans.Add([pscustomobject]@{Name=$folder.Name;Entry=$entry;Stage=$stage;Sources=$receipt.AuthoredSources;Searches=$receipt.PathSearches;Loose=$looseEvidence;Warnings=$warnings.ToArray();Fresh=$true;Target=$target;Files=@(Get-ChildItem -LiteralPath $stage -File -Recurse | ForEach-Object {[pscustomobject]@{Relative=[IO.Path]::GetRelativePath($stage,$_.FullName);Sha256=[Reborn.PackageFile]::Hash($_.FullName)}})})
    }
    $Result.Value=$plans.ToArray()
}

#-------------------------------------------------------------------------------------------------
<# Reborn: publish only fully compiled generations, preserving unrelated authored Misc files and keeping all files under each Official map path. #>
#-------------------------------------------------------------------------------------------------
function Publish-RebornMapOutputs([object[]]$Plans,[string]$Misc,[switch]$Witnessed) {
    foreach($plan in $Plans){
        if(!$Witnessed){foreach($source in $plan.Sources){if([Reborn.PackageFile]::Hash($source.Path)-cne $source.Sha256){throw ('Map XML source changed during compilation: '+$source.Path)}}}
        $destination=Join-Path $Misc ('Data/Maps/Official/'+$plan.Name);Assert-RebornPackagePath $destination
        foreach($file in $plan.Files){
            $inputPath=Join-Path $plan.Stage $file.Relative;$outputPath=Join-Path $destination $file.Relative
            Assert-RebornPackagePath $outputPath
            if([Reborn.PackageFile]::Hash($inputPath)-cne $file.Sha256){throw 'Staged map data changed before Misc publication.'}
            # Reborn: a cache hit can repopulate a missing Misc file without rewriting an already identical one.
            if([IO.File]::Exists($outputPath) -and [Reborn.PackageFile]::Hash($outputPath)-ceq $file.Sha256){continue}
            $null=New-Item -ItemType Directory -Force -Path (Split-Path -Parent $outputPath)
            Copy-Item -LiteralPath $inputPath -Destination $outputPath -Force
        }
        Write-SdkBuildLine ('Map output: '+$destination+' (map.version=_mod; '+$plan.Files.Count+' files).')
    }
}
