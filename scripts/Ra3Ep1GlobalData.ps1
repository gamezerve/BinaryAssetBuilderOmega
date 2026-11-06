# Reborn: EA's AdditionalMaps discovery path loads separately compiled MapMetaData_* streams; the stock global stream is never rebuilt.

#-------------------------------------------------------------------------------------------------
<# Reborn: atomically keep the last successful AdditionalMaps manifests for compile-only map dependency checks. #>
#-------------------------------------------------------------------------------------------------
function Save-RebornMapGlobalCatalogs($GlobalBuild,[string]$Cache,[string]$Work) {
    Assert-RebornGlobalSources $GlobalBuild
    Assert-RebornPackagePath $Cache
    $folder=Join-Path $Work 'global-catalog-cache'
    $null=New-Item -ItemType Directory -Path $folder
    $index=@(foreach($stream in $GlobalBuild.AdditionalStreams){
        $name=[IO.Path]::GetFileName($stream.Runtime)
        $copy=Join-Path $folder $name
        Copy-Item -LiteralPath $stream.Manifest -Destination $copy
        if((Get-FileHash -LiteralPath $copy).Hash -cne $stream.Sha256){throw 'Global catalog changed before caching.'}
        [pscustomobject]@{Runtime=$stream.Runtime;Sha256=$stream.Sha256}
    })
    [IO.File]::WriteAllText((Join-Path $folder 'index.json'),(ConvertTo-Json -InputObject $index))
    $stage=Join-Path $Work 'global-catalog-cache.zip'
    [IO.Compression.ZipFile]::CreateFromDirectory($folder,$stage)
    $null=New-Item -ItemType Directory -Force -Path (Split-Path -Parent $Cache)
    [IO.File]::Move($stage,$Cache,$true)
}

#-------------------------------------------------------------------------------------------------
<# Reborn: recover reference-only manifests under the private run, with their original runtime names and hashes. #>
#-------------------------------------------------------------------------------------------------
function Read-RebornMapGlobalCatalogs([string]$Cache,[string]$Work) {
    Assert-RebornPackagePath $Cache
    if(!(Test-Path -LiteralPath $Cache -PathType Leaf)){return}
    try{
        $folder=Join-Path $Work 'cached-global-catalogs'
        [IO.Compression.ZipFile]::ExtractToDirectory($Cache,$folder)
        $index=Get-Content -LiteralPath (Join-Path $folder 'index.json') -Raw | ConvertFrom-Json -NoEnumerate
        foreach($item in $index){
            if($item.Runtime -notmatch '\Adata\\additionalmaps\\mapmetadata_[a-z0-9_-]+\.manifest\z'){throw 'Invalid cached global runtime name.'}
            $manifest=Join-Path $folder ([IO.Path]::GetFileName($item.Runtime))
            if((Get-FileHash -LiteralPath $manifest).Hash -cne $item.Sha256){throw 'Cached global manifest hash differs.'}
            [pscustomobject]@{Manifest=$manifest;Runtime=$item.Runtime;Sha256=$item.Sha256}
        }
    }catch{
        throw ('Could not read the last global catalog. Enable Build Global Data to refresh it. '+$_.Exception.Message)
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: compile each selected global source into an independently verified stream and retain its source/search evidence for publication. #>
#-------------------------------------------------------------------------------------------------
function Build-RebornGlobalData([string]$DataRoot,[string]$Sdk,[hashtable]$Settings,[string]$StaticManifest,[string]$GlobalManifest,[string]$Compiler,[string]$Work,[string]$Detail,[ref]$Result,[bool]$IncludeGlobalData=$true,[bool]$IncludeMapCatalog=$false,[object[]]$ReferenceStreams=@()) {
    $exe=Join-Path $Compiler 'BinaryAssetBuilder.ManifestInspector.exe'
    $directory=Join-Path $DataRoot 'AdditionalMaps';$streams=@();$additionalStreams=@();$evidence=@();$context=Join-Path $Work 'global-streams.json'
    # Reborn: absence is an ordinary static-only mod, matching the reference SDK's wildcard build stage.
    $entries=@(if($IncludeGlobalData -and (Test-Path -LiteralPath $directory -PathType Container)){Get-ChildItem -LiteralPath $directory -File -Filter 'MapMetaData_*.xml' | Sort-Object Name})
    # Reborn: missing optional inputs are yellow skips, distinct from unsupported compiler stages.
    if($IncludeGlobalData -and -not(Test-Path -LiteralPath $directory -PathType Container)){Write-SdkBuildLine ('Skipped: [No input] AdditionalMaps directory not found: '+$directory)}
    elseif($IncludeGlobalData -and -not $entries){Write-SdkBuildLine ('Skipped: [No input] No MapMetaData_*.xml files found in '+$directory)}
    # Reborn: Build Maps discovers the optional standalone catalog at the Official root.
    if($IncludeMapCatalog){
        $catalog=Join-Path $DataRoot 'Maps/Official/mapmetadata_singleplayer.xml'
        if(Test-Path -LiteralPath $catalog -PathType Leaf){$entries+= Get-Item -LiteralPath $catalog}
        else{Write-SdkBuildLine ('Skipped: [No input] Optional map catalog not found: '+$catalog)}
    }
    # Reborn: Different source paths must never publish the same AdditionalMaps stream.
    $duplicate=@($entries | Group-Object { $_.BaseName.ToLowerInvariant() } | Where-Object Count -gt 1)
    if($duplicate){throw ('Duplicate AdditionalMaps stream name: '+($duplicate.Name -join ', '))}
    foreach($entry in $entries){
        if($entry.BaseName -cnotmatch '(?i)\AMapMetaData_[a-z0-9_-]+\z'){throw ('Invalid global stream name: '+$entry.FullName)}
        # Reborn: a genuinely blank optional entry has no XML to compile; malformed nonblank XML still reaches normal diagnostics.
        if($entry.Length-le 4MB -and [string]::IsNullOrWhiteSpace([IO.File]::ReadAllText($entry.FullName))){Write-SdkBuildLine ('Skipped: [No input] Empty global source: '+$entry.FullName);continue}
        $target=Join-Path $Work ('global/'+$entry.BaseName);$null=New-Item -ItemType Directory -Path $target
        Write-SdkBuildLine ('['+$script:BuildClock.Elapsed.ToString('c')+'] Global source: '+$entry.FullName)
        # Reborn: a map-only check may need cached globals to compile its fresh single-player map catalog.
        [IO.File]::WriteAllText($context,(ConvertTo-Json -InputObject @($ReferenceStreams+$streams) -Depth 8))
        $stockData=if($Settings.SdkDataRoot){$Settings.SdkDataRoot}else{Join-Path $Sdk 'SageXML'}
        $auditArgs=@('sdk-missing-dependencies',$entry.FullName,$Sdk,$StaticManifest,$GlobalManifest,(Join-Path $target 'dependencies.json'),$stockData,$context)
        & $exe @auditArgs 2>&1 | ForEach-Object {Write-SdkCompilerLine ([string]$_) $Detail};if($LASTEXITCODE-ne 0){throw ('Global dependency preflight failed: '+$entry.FullName)}
        # Reborn: count the complete all-Include emission graph; empty wrappers, definitions and instance-only bases produce no stream.
        $audit=Get-Content -LiteralPath (Join-Path $target 'dependencies.json') -Raw | ConvertFrom-Json
        if($audit.SourceGraphComplete -and $audit.HasEmittedContent-eq $false){Write-SdkBuildLine ('Skipped: [No input] No assets to compile in global source or its includes: '+$entry.FullName);continue}
        & $exe conscript-source-prepare $entry.FullName (Join-Path $target 'source') $Sdk $stockData 2>&1 | ForEach-Object {Write-SdkCompilerLine ([string]$_) $Detail};if($LASTEXITCODE-ne 0){throw ('Global source preparation failed: '+$entry.FullName)}
        $inputReceipt=Get-Content -LiteralPath (Join-Path $target 'source/source-input.json') -Raw | ConvertFrom-Json
        # Reborn: the early-global mode emits only actual runtime dependency manifests, never an automatic static-stream dependency.
        & $exe conscript-alpha-build $Settings.UprisingXmlRoot $StaticManifest $GlobalManifest $inputReceipt.GamePath (Join-Path $target 'compiled') $context global 2>&1 | ForEach-Object {Write-SdkCompilerLine ([string]$_) $Detail};if($LASTEXITCODE-ne 0){throw ('Global stream compilation failed: '+$entry.FullName)}
        $manifest=Join-Path $target 'compiled/stream-readback/diagnostic.manifest'
        $streams+= [pscustomobject]@{Manifest=$manifest;Runtime=('data\additionalmaps\'+$entry.BaseName+'.manifest');Sha256=(Get-FileHash -LiteralPath $manifest -Algorithm SHA256).Hash}
        # Reborn: the Official map catalog is rebuilt by Build Maps and stays out of the reusable global cache.
        if($entry.DirectoryName -ieq $directory){$additionalStreams+=$streams[-1]}
        $evidence+= [pscustomobject]@{Entry=$entry.FullName;Runtime=$streams[-1].Runtime;AssetCount=$inputReceipt.AssetCount;Sources=$inputReceipt.Sources;Searches=$inputReceipt.Searches}
        Write-SdkBuildLine ('Global stream complete: data\additionalmaps\'+$entry.BaseName+' ('+$inputReceipt.AssetCount+' assets).')
    }
    [IO.File]::WriteAllText($context,(ConvertTo-Json -InputObject @($streams) -Depth 8))
    $mapContext=$context
    if($ReferenceStreams.Count){
        $mapContext=Join-Path $Work 'map-global-streams.json'
        [IO.File]::WriteAllText($mapContext,(ConvertTo-Json -InputObject @($ReferenceStreams+$streams) -Depth 8))
    }
    $Result.Value=[pscustomobject]@{Context=$context;MapContext=$mapContext;Streams=@($streams);AdditionalStreams=@($additionalStreams);Evidence=@($evidence)}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: global sources and their alias selections must still match the qualified snapshot when the static build finishes. #>
#-------------------------------------------------------------------------------------------------
function Assert-RebornGlobalSources($GlobalBuild) {
    foreach($entry in $GlobalBuild.Evidence){
        foreach($source in $entry.Sources){if((Get-FileHash -LiteralPath $source.Path -Algorithm SHA256).Hash -cne $source.Sha256){throw ('Global source changed during compilation: '+$source.Path)}}
        Assert-RebornSourceSearch $entry.Searches
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: replay captured alias winners or warned ART absence before publishing either static or global streams. #>
#-------------------------------------------------------------------------------------------------
function Assert-RebornSourceSearch([object[]]$Selections) {
    foreach($selection in $Selections){
        $winner=$null
        foreach($candidate in $selection.Candidates){
            for($ancestor=$candidate;$ancestor;$ancestor=[IO.Path]::GetDirectoryName($ancestor)){if((Test-Path -LiteralPath $ancestor) -and ((Get-Item -LiteralPath $ancestor -Force).Attributes-band [IO.FileAttributes]::ReparsePoint)){throw 'Redirected alias search candidate.'}}
            if(Test-Path -LiteralPath $candidate -PathType Container){throw 'Alias candidate became a directory.'}
            if(Test-Path -LiteralPath $candidate -PathType Leaf){$winner=$candidate;break}
        }
        if($selection.PSObject.Properties['ExpectedMissing'] -and $selection.ExpectedMissing){
            if($selection.Layer-cnotin @('missing-art-all','missing-art-instance') -or $selection.LogicalPath-notmatch '^ART:' -or $null-ne $winner){throw ('Skipped ART include changed: '+$selection.LogicalPath)}
            continue
        }
        if($winner-ine $selection.PhysicalPath){throw ('Alias precedence changed: '+$selection.LogicalPath)}
    }
}
