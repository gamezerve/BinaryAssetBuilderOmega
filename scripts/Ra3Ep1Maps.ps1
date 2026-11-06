# Reborn: compile direct Official map folders into isolated generations before publishing map_mod families into the mod's Misc tree.

#-------------------------------------------------------------------------------------------------
<# Reborn: discover maps, use the actual native source backend and retain all successful map files until the full map stage passes. #>
#-------------------------------------------------------------------------------------------------
function Build-RebornMaps([string]$DataRoot,[string]$Sdk,[hashtable]$Settings,[string]$StaticManifest,[string]$GlobalManifest,[string]$Compiler,[string]$Work,[string]$Detail,[string]$GlobalContext,[ref]$Result,[string]$ModStaticPackage,[string]$OfficialMapDirectory) {
    $root=Join-Path $DataRoot 'Maps/Official';$plans=@()
    if(-not(Test-Path -LiteralPath $root -PathType Container)){
        if($OfficialMapDirectory){throw ('Selected Official map is no longer available: '+$OfficialMapDirectory)}
        Write-SdkBuildLine ('Skipped: [No input] Official maps directory not found: '+$root);$Result.Value=@();return
    }
    $folders=@(Get-ChildItem -LiteralPath $root -Directory | Sort-Object Name)
    # Reborn: match only direct map folders in the selected mod before preflight or compilation.
    if($OfficialMapDirectory){
        $folders=@($folders | Where-Object FullName -ieq $OfficialMapDirectory)
        if($folders.Count -ne 1){throw ('Selected Official map is not a direct folder of this mod: '+$OfficialMapDirectory)}
    }
    if(-not $folders){Write-SdkBuildLine ('Skipped: [No input] No map folders in: '+$root)}
    foreach($folder in $folders){
        Assert-RebornPackagePath $folder.FullName
        $entry=Join-Path $folder.FullName 'map.xml'
        if(-not(Test-Path -LiteralPath $entry -PathType Leaf)){
            # Reborn: a specific map request must never succeed after its source disappears.
            if($OfficialMapDirectory){throw ('Selected map source not found: '+$entry)}
            Write-SdkBuildLine ('Skipped: [No input] Map source not found: '+$entry);continue
        }
        $loose=@(Get-ChildItem -LiteralPath $folder.FullName -File | Where-Object Extension -in @('.map','.tga'))
        if(-not @($loose|Where-Object Extension -ieq '.map')){throw ('MAPFILE: map.xml has no accompanying .map file: '+$entry)}
        foreach($file in $loose){Assert-RebornPackagePath $file.FullName}
        $target=Join-Path $Work ('maps/'+$folder.Name);$null=New-Item -ItemType Directory -Path $target
        Write-SdkBuildLine ('Map source: '+$entry)
        # Reborn: missing terrain sources warn and skip only this map; no placeholder atlas or partial map is published.
        & (Join-Path $Compiler 'BinaryAssetBuilder.ManifestInspector.exe') map-resource-preflight $entry $Sdk (Join-Path $target 'terrain-audit') 2>&1 | ForEach-Object {Write-SdkCompilerLine ([string]$_) $Detail}
        if($LASTEXITCODE-eq 3){Write-SdkBuildLine ('Warning: MAPSKIPPED '' '+$folder.Name+' '' Missing terrain resources; this map was not rebuilt. Existing Misc content is retained.');continue}
        if($LASTEXITCODE-ne 0){throw ('Map resource preflight failed: '+$entry)}
        $arguments=@('-NoProfile','-NonInteractive','-File',(Join-Path $PSScriptRoot 'Invoke-Ra3Ep1ConscriptAlphaBuild.ps1'),'-ModXml',$entry,'-StaticManifest',$StaticManifest,'-GlobalManifest',$GlobalManifest,'-CompilerDirectory',$Compiler,'-OutputRoot',$target)
        if($GlobalContext){$arguments+=@('-GlobalStreamContext',$GlobalContext)}
        # Reborn: the main static package is an external provider for this map, never another XML include.
        if($ModStaticPackage){$arguments+=@('-MapStaticPackage',$ModStaticPackage)}
        & $Settings.PowerShell7 @arguments 2>&1 | ForEach-Object {Write-SdkCompilerLine ([string]$_) $Detail}
        if($LASTEXITCODE-ne 0){throw ('Map compilation failed: '+$entry+'. No map output was copied to Misc. Review the source/native processor diagnostic above.')}
        $receipt=Get-Content -LiteralPath (Join-Path $target 'alpha-build.json') -Raw|ConvertFrom-Json
        $stage=Join-Path $target 'misc-map'
        & (Join-Path $Compiler 'BinaryAssetBuilder.ManifestInspector.exe') sdk-stage-map-package $receipt.PackagePath $stage 2>&1 | ForEach-Object {Write-SdkCompilerLine ([string]$_) $Detail}
        if($LASTEXITCODE-ne 0){throw ('Map stream staging failed: '+$entry)}
        foreach($file in $loose){$hash=(Get-FileHash -LiteralPath $file.FullName).Hash;Copy-Item -LiteralPath $file.FullName -Destination (Join-Path $stage $file.Name);if((Get-FileHash -LiteralPath $file.FullName).Hash-cne $hash -or (Get-FileHash -LiteralPath (Join-Path $stage $file.Name)).Hash-cne $hash){throw ('Map source changed while copying: '+$file.FullName)}}
        $plans+= [pscustomobject]@{Name=$folder.Name;Entry=$entry;Stage=$stage;Sources=$receipt.AuthoredSources;Files=@(Get-ChildItem -LiteralPath $stage -File -Recurse | ForEach-Object {[pscustomobject]@{Relative=[IO.Path]::GetRelativePath($stage,$_.FullName);Sha256=(Get-FileHash -LiteralPath $_.FullName).Hash}})}
    }
    $Result.Value=@($plans)
}

#-------------------------------------------------------------------------------------------------
<# Reborn: publish only fully compiled generations, preserving unrelated authored Misc files and keeping all files under each Official map path. #>
#-------------------------------------------------------------------------------------------------
function Publish-RebornMapOutputs([object[]]$Plans,[string]$Misc) {
    foreach($plan in $Plans){
        foreach($source in $plan.Sources){if((Get-FileHash -LiteralPath $source.Path).Hash-cne $source.Sha256){throw ('Map XML source changed during compilation: '+$source.Path)}}
        $destination=Join-Path $Misc ('Data/Maps/Official/'+$plan.Name);Assert-RebornPackagePath $destination
        foreach($file in $plan.Files){
            $inputPath=Join-Path $plan.Stage $file.Relative;$outputPath=Join-Path $destination $file.Relative
            Assert-RebornPackagePath $outputPath
            if((Get-FileHash -LiteralPath $inputPath).Hash-cne $file.Sha256){throw 'Staged map data changed before Misc publication.'}
            $null=New-Item -ItemType Directory -Force -Path (Split-Path -Parent $outputPath)
            Copy-Item -LiteralPath $inputPath -Destination $outputPath -Force
        }
        Write-SdkBuildLine ('Map output: '+$destination+' (map.version=_mod; '+$plan.Files.Count+' files).')
    }
}
