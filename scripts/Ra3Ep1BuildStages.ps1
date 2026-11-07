# Reborn: independent last-successful stream generations are selected by one atomically replaced mod index.

#-------------------------------------------------------------------------------------------------
<# Reborn: create one private replacement family; no persistent generation is changed before the build succeeds. #>
#-------------------------------------------------------------------------------------------------
function New-RebornBuildStage([string]$Run,[string]$Family,[object]$Metadata) {
    $directory=Join-Path $Run ('stage-'+$Family)
    $null=New-Item -ItemType Directory -Path (Join-Path $directory 'files') -Force
    $null=New-Item -ItemType Directory -Path (Join-Path $directory 'metadata') -Force
    [pscustomobject]@{Family=$Family;Directory=$directory;Payload=(Join-Path $directory 'files');Metadata=$Metadata;Fresh=$true;Receipt=$null}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: persist weak-reference evidence and dependency diagnostics alongside their owning compiled stage. #>
#-------------------------------------------------------------------------------------------------
function Save-RebornStageDiagnostics($Stage,[string[]]$Directories,[string]$DependencyReport) {
    $index=0
    foreach($directory in $Directories){
        if(!(Test-Path -LiteralPath $directory -PathType Container)){continue}
        foreach($file in Get-ChildItem -LiteralPath $directory -Filter 'asset-usage.json' -File -Recurse){
            $target=Join-Path $Stage.Directory ('metadata/usage-'+$index+'/asset-usage.json')
            $null=New-Item -ItemType Directory -Path (Split-Path -Parent $target) -Force
            Copy-Item -LiteralPath $file.FullName -Destination $target;$index++
        }
    }
    if($DependencyReport -and (Test-Path -LiteralPath $DependencyReport -PathType Leaf)){
        Copy-Item -LiteralPath $DependencyReport -Destination (Join-Path $Stage.Directory 'metadata/dependencies.json')
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: fingerprint every persisted byte and retain source provenance separately from runtime files. #>
#-------------------------------------------------------------------------------------------------
function Complete-RebornBuildStage($Stage,[string]$Name) {
    $files=@(Get-ChildItem -LiteralPath $Stage.Directory -File -Recurse | Where-Object Name -ne 'stage.json' | Sort-Object FullName | ForEach-Object {
        Assert-RebornPackagePath $_.FullName
        [pscustomobject]@{Path=[IO.Path]::GetRelativePath($Stage.Directory,$_.FullName);Length=$_.Length;Sha256=(Get-FileHash -LiteralPath $_.FullName).Hash}
    })
    $receipt=[pscustomobject]@{Format='RebornBuildStage1';ModName=$Name;Family=$Stage.Family;CreatedUtc=[DateTime]::UtcNow.ToString('o');Files=$files;Metadata=$Stage.Metadata}
    [IO.File]::WriteAllText((Join-Path $Stage.Directory 'stage.json'),($receipt|ConvertTo-Json -Depth 30))
    $Stage.Receipt=$receipt
}

#-------------------------------------------------------------------------------------------------
<# Reborn: cached outputs must match their recorded bytes, stay local and contain no untracked payloads. #>
#-------------------------------------------------------------------------------------------------
function Read-RebornBuildStage([string]$Root,[string]$Name,[string]$Family,$Pointer) {
    if($Pointer.Generation -cnotmatch '\A[a-f0-9]{32}\z' -or $Pointer.Sha256 -cnotmatch '\A[A-F0-9]{64}\z'){throw 'Invalid saved stage pointer.'}
    $directory=Join-Path $Root ($Family+'/'+$Pointer.Generation);Assert-RebornPackagePath $directory
    $receiptPath=Join-Path $directory 'stage.json'
    if((Get-FileHash -LiteralPath $receiptPath).Hash -cne $Pointer.Sha256){throw ('Saved '+$Family+' receipt changed. Rebuild that stage.')}
    $receipt=Get-Content -LiteralPath $receiptPath -Raw|ConvertFrom-Json
    if($receipt.Format -cne 'RebornBuildStage1' -or $receipt.ModName -cne $Name -or $receipt.Family -cne $Family){throw 'Saved stage identity differs.'}
    $seen=[Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
    foreach($file in $receipt.Files){
        if([IO.Path]::IsPathRooted($file.Path) -or $file.Path -match '(^|[\\/])\.\.?([\\/]|$)' -or !$seen.Add($file.Path)){throw 'Invalid saved stage path.'}
        $path=[IO.Path]::GetFullPath((Join-Path $directory $file.Path));Assert-RebornPackagePath $path
        if(!$path.StartsWith($directory+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)){throw 'Saved stage path escaped its generation.'}
        if((Get-Item -LiteralPath $path).Length -ne $file.Length -or (Get-FileHash -LiteralPath $path).Hash -cne $file.Sha256){throw ('Saved '+$Family+' output changed: '+$file.Path+'. Rebuild that stage.')}
    }
    $actual=@(Get-ChildItem -LiteralPath $directory -File -Recurse | Where-Object Name -ne 'stage.json')
    if($actual.Count-ne $seen.Count){throw ('Untracked files in saved '+$Family+' output.')}
    [pscustomobject]@{Family=$Family;Directory=$directory;Payload=(Join-Path $directory 'files');Metadata=$receipt.Metadata;Fresh=$false;Receipt=$receipt}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: reuse independent generations, or privately split a verified pre-upgrade package on first use. #>
#-------------------------------------------------------------------------------------------------
function Get-RebornBuildStages([string]$Output,[string]$Name,[string]$Run,[string]$Compiler,[string[]]$Selected=@()) {
    $root=Join-Path $Output $Name;$indexPath=Join-Path $root 'current.json';$stages=@{}
    if(Test-Path -LiteralPath $indexPath -PathType Leaf){
        Assert-RebornPackagePath $indexPath
        $index=Get-Content -LiteralPath $indexPath -Raw|ConvertFrom-Json
        if($index.Format -cne 'RebornStageIndex1' -or $index.ModName -cne $Name){throw 'Invalid saved mod stage index.'}
        foreach($property in $index.Stages.PSObject.Properties){
            if($property.Name -cnotin @('Static','Global','AptUI','MapCatalog')){throw 'Unknown saved stage family.'}
            if($property.Name -cin $Selected){continue}
            $stages[$property.Name]=Read-RebornBuildStage $root $Name $property.Name $property.Value
            Write-SdkBuildLine ('Saved '+$property.Name+' output verified.') | ForEach-Object {[Console]::WriteLine($_)}
        }
    }else{
        $big=Join-Path $root ($Name+'.big');$receiptPath=Join-Path $root 'build.json'
        if((Test-Path -LiteralPath $big -PathType Leaf) -and (Test-Path -LiteralPath $receiptPath -PathType Leaf)){
            $old=Get-Content -LiteralPath $receiptPath -Raw|ConvertFrom-Json
            if($old.Format -cne 'RebornLiveAlpha1' -or $old.ModName -cne $Name -or (Get-FileHash -LiteralPath $big).Hash -cne $old.PackageSha256){throw 'Previous legacy mod output failed receipt verification.'}
            foreach($family in @('Static','Global','AptUI','MapCatalog')){
                if($family -cin $Selected){continue}
                $metadata=[pscustomobject]@{Legacy=$true;Backend=$old.Backend;Core=$old.Core;Evidence=@($old.GlobalData.Streams)}
                $stage=New-RebornBuildStage $Run $family $metadata
                & $Compiler sdk-export-build-stage $big $stage.Payload $family
                if($LASTEXITCODE){throw ('Could not migrate previous '+$family+' output.')}
                Complete-RebornBuildStage $stage $Name;$stages[$family]=$stage
            }
            Write-SdkBuildLine 'Previous successful package split into Static, Global and AptUI output families.' | ForEach-Object {[Console]::WriteLine($_)}
        }
    }
    return ,$stages
}

#-------------------------------------------------------------------------------------------------
<# Reborn: derive relocatable global dependency contexts from the selected fresh or saved runtime files. #>
#-------------------------------------------------------------------------------------------------
function Get-RebornStageGlobalStreams([hashtable]$Stages) {
    foreach($family in @('Global','MapCatalog')){
        if(!$Stages.ContainsKey($family)){continue}
        $folder=Join-Path $Stages[$family].Payload 'data/additionalmaps'
        if(!(Test-Path -LiteralPath $folder -PathType Container)){continue}
        foreach($file in Get-ChildItem -LiteralPath $folder -Filter '*.manifest' -File | Sort-Object Name){
            [pscustomobject]@{Manifest=$file.FullName;Runtime=('data\additionalmaps\'+$file.Name);Sha256=(Get-FileHash -LiteralPath $file.FullName).Hash}
        }
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: retain each freshly compiled global family, including all CDATA behind its actual runtime stream. #>
#-------------------------------------------------------------------------------------------------
function New-RebornGlobalBuildStage($Build,[string]$Family,[string]$Run,[string]$Name,[string]$Compiler) {
    $streams=@($Build.Streams | Where-Object { ($_.Runtime -match '(?i)\\mapmetadata_singleplayer\.manifest$') -eq ($Family -eq 'MapCatalog') })
    $evidence=@($Build.Evidence | Where-Object { $_.Runtime -in $streams.Runtime })
    $stage=New-RebornBuildStage $Run $Family ([pscustomobject]@{Legacy=$false;Evidence=$evidence})
    $context=Join-Path $Run ($Family+'-export.json');[IO.File]::WriteAllText($context,(ConvertTo-Json -InputObject $streams -Depth 8))
    & $Compiler sdk-export-global-stage $context $stage.Payload
    if($LASTEXITCODE){throw ('Could not save '+$Family+' streams.')}
    Save-RebornStageDiagnostics $stage @($streams | ForEach-Object {Split-Path -Parent (Split-Path -Parent $_.Manifest)}) $null
    & $Compiler sdk-save-stage-diagnostics $stage.Payload $Run (Join-Path $stage.Directory 'metadata/cached-origins.json')
    if($LASTEXITCODE){throw 'Could not save global diagnostic evidence.'}
    Complete-RebornBuildStage $stage $Name
    return $stage
}

#-------------------------------------------------------------------------------------------------
<# Reborn: package only the selected verified runtime files, with bounded copying and full BIG payload readback. #>
#-------------------------------------------------------------------------------------------------
function New-RebornStagePackage([hashtable]$Stages,[string]$Destination,[string]$Compiler,[string]$Sdk,[string]$Run) {
    $roots=@($Stages.Values | ForEach-Object Payload)
    $rootsPath=Join-Path $Run 'selected-stage-roots.json';[IO.File]::WriteAllText($rootsPath,(ConvertTo-Json -InputObject $roots))
    & $Compiler sdk-verify-stage-links $rootsPath $Sdk
    if($LASTEXITCODE){throw 'Saved and fresh stream dependencies are incompatible. Rebuild the stage named above.'}
    $entries=[Collections.Generic.List[object]]::new();$names=[Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
    foreach($stage in $Stages.Values){foreach($file in $stage.Receipt.Files){
        if($file.Path -notmatch '^files[\\/]'){continue}
        $path=Join-Path $stage.Directory $file.Path;$name=$file.Path.Substring(6).Replace('/','\')
        if(!$names.Add($name)){throw ('Duplicate saved stream runtime path: '+$name)}
        $entries.Add([pscustomobject]@{Name=$name;Bytes=[Text.Encoding]::ASCII.GetBytes($name+[char]0);File=$path;Length=[long]$file.Length;Hash=$file.Sha256})
    }}
    $sorted=@($entries | Sort-Object Name)
    Write-RebornMiscBig $sorted $Destination
    Write-SdkBuildLine ('Streams BIG verified: '+$entries.Count+' files from separate saved output families.')
}

#-------------------------------------------------------------------------------------------------
<# Reborn: retain weak-use information from unselected stages for the normal final advisory audits. #>
#-------------------------------------------------------------------------------------------------
function Restore-RebornStageDiagnostics([hashtable]$Stages,[string]$Run) {
    foreach($stage in $Stages.Values){
        $metadata=Join-Path $stage.Directory 'metadata'
        foreach($file in Get-ChildItem -LiteralPath $metadata -File -Recurse | Where-Object Name -in @('asset-usage.json','cached-origins.json')){
            $relative=[IO.Path]::GetRelativePath($metadata,$file.FullName)
            $target=Join-Path $Run ('reused-usage/'+$stage.Family+'/'+$relative)
            $null=New-Item -ItemType Directory -Force -Path (Split-Path -Parent $target)
            Copy-Item -LiteralPath $file.FullName -Destination $target
        }
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: publish all selected generations with one atomic pointer replacement; interrupted preparation leaves the previous index valid. #>
#-------------------------------------------------------------------------------------------------
function Save-RebornBuildStages([hashtable]$Stages,[string]$Output,[string]$Name) {
    $root=Join-Path $Output $Name;Assert-RebornPackagePath $root
    $null=New-Item -ItemType Directory -Path $root -Force
    $oldPath=Join-Path $root 'current.json';$old=$null
    if(Test-Path -LiteralPath $oldPath){$old=Get-Content -LiteralPath $oldPath -Raw|ConvertFrom-Json}
    $pointers=@{}
    foreach($stage in $Stages.Values){
        if($stage.Fresh){
            $generation=[guid]::NewGuid().ToString('N');$parent=Join-Path $root $stage.Family
            $null=New-Item -ItemType Directory -Path $parent -Force
            $target=Join-Path $parent $generation;Assert-RebornPackagePath $target
            [IO.Directory]::Move($stage.Directory,$target);$stage.Directory=$target;$stage.Payload=Join-Path $target 'files'
        }else{$generation=Split-Path -Leaf $stage.Directory}
        $pointers[$stage.Family]=[pscustomobject]@{Generation=$generation;Sha256=(Get-FileHash -LiteralPath (Join-Path $stage.Directory 'stage.json')).Hash}
    }
    $index=[pscustomobject]@{Format='RebornStageIndex1';ModName=$Name;Stages=$pointers}
    $temporary=Join-Path $root ('.current-'+[guid]::NewGuid().ToString('N')+'.json')
    [IO.File]::WriteAllText($temporary,($index|ConvertTo-Json -Depth 8));[IO.File]::Move($temporary,$oldPath,$true)
    if($old){foreach($property in $old.Stages.PSObject.Properties){
        $family=$property.Name;$generation=$property.Value.Generation
        if($family -cin @('Static','Global','AptUI','MapCatalog') -and $generation -cmatch '\A[a-f0-9]{32}\z' -and $pointers[$family].Generation -cne $generation){
            try{Remove-DevAlphaOwned (Join-Path $root ($family+'/'+$generation)) (Join-Path $root $family)}catch{Write-SdkBuildLine ('Warning: previous stage generation retained: '+$_.Exception.Message)}
        }
    }}
    # Reborn: the legacy combined copy is redundant only after all replacement families have been committed.
    $legacyReceiptPath=Join-Path $root 'build.json'
    if(Test-Path -LiteralPath $legacyReceiptPath){
        $legacyReceipt=Get-Content -LiteralPath $legacyReceiptPath -Raw|ConvertFrom-Json
        $legacyBig=Join-Path $root ($Name+'.big')
        if($legacyReceipt.Format -ceq 'RebornLiveAlpha1' -and (Test-Path -LiteralPath $legacyBig) -and (Get-FileHash -LiteralPath $legacyBig).Hash -ceq $legacyReceipt.PackageSha256){
            Remove-Item -LiteralPath $legacyBig
            $legacySku=Join-Path $root 'mod_1.0.skudef'
            if((Test-Path -LiteralPath $legacySku) -and [IO.File]::ReadAllText($legacySku).Trim() -ceq ('add-big '+$Name+'.big')){Remove-Item -LiteralPath $legacySku}
        }
    }
    Write-SdkBuildLine ('Successful stage outputs saved: '+$root)
}
