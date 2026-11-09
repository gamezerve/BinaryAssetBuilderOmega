# Reborn: verified per-map snapshots let the map stage reuse its last successful native output.

#-------------------------------------------------------------------------------------------------
<# Reborn: fingerprint shared dependencies once per build, using the same static manifest for a fresh BIG or a saved stage. #>
#-------------------------------------------------------------------------------------------------
function Get-RebornMapCacheKey([string]$Sdk,[hashtable]$Settings,[string]$Compiler,[string]$Static,[string]$Global,[string]$Context,[string]$Provider,[string]$Work) {
    $rows=[Collections.Generic.List[string]]::new()
    $rows.Add('RebornMapCache1')
    $rows.Add($Sdk)
    $rows.Add('Developer='+$env:REBORN_DEVELOPER_BUILD_TESTS)
    $rows.Add('Silent='+$env:REBORN_SILENT_MODE)
    $trees=@('Schemas','SageXML','catalogs','tools/resources','tools/compiler','scripts')
    $roots=@($trees | ForEach-Object {Join-Path $Sdk $_})
    if($Settings.UprisingXmlRoot -and $Settings.UprisingXmlRoot -ine (Join-Path $Sdk 'SageXML')){$roots+= $Settings.UprisingXmlRoot}
    foreach($root in $roots){
        $pending=[Collections.Generic.Queue[string]]::new()
        $pending.Enqueue($root)
        $files=[Collections.Generic.List[string]]::new()
        while($pending.Count){
            $directory=$pending.Dequeue()
            [Reborn.PackageFile]::CheckPath($directory,$false)
            foreach($item in Get-ChildItem -LiteralPath $directory -Force){
                if($item.Attributes -band [IO.FileAttributes]::ReparsePoint){throw ('Redirected map cache input: '+$item.FullName)}
                if($item.PSIsContainer){$pending.Enqueue($item.FullName)}else{$files.Add($item.FullName)}
            }
        }
        $files.Sort([StringComparer]::Ordinal)
        foreach($file in $files){$rows.Add($file+'='+[Reborn.PackageFile]::Hash($file))}
    }
    foreach($file in @($Static,$Global)){
        [Reborn.PackageFile]::CheckPath($file,$false)
        $rows.Add($file+'='+[Reborn.PackageFile]::Hash($file))
        # Reborn: explicit external stock providers also supply native layout witness slices beside the manifest.
        $localCatalogs=(Join-Path $Sdk 'catalogs')+[IO.Path]::DirectorySeparatorChar
        if(!$file.StartsWith($localCatalogs,[StringComparison]::OrdinalIgnoreCase)){
            foreach($extension in @('.bin','.relo','.imp')){
                $part=[IO.Path]::ChangeExtension($file,$extension)
                [Reborn.PackageFile]::CheckPath($part,$false)
                $value=if([IO.File]::Exists($part)){[Reborn.PackageFile]::Hash($part)}else{'missing'}
                $rows.Add($part+'='+$value)
            }
        }
    }
    if($Context){
        foreach($stream in @(Get-Content -LiteralPath $Context -Raw | ConvertFrom-Json)){
            [Reborn.PackageFile]::CheckPath($stream.Manifest,$false)
            $hash=[Reborn.PackageFile]::Hash($stream.Manifest)
            if($hash-cne $stream.Sha256){throw ('Global map provider changed: '+$stream.Manifest)}
            $rows.Add($stream.Runtime+'='+$hash)
        }
    }
    if($Provider){
        [Reborn.PackageFile]::CheckPath($Provider,$false)
        $manifest=$Provider
        if([IO.Path]::GetExtension($Provider) -ine '.manifest'){
            $manifest=Join-Path $Work ('map-provider-'+[guid]::NewGuid().ToString('N')+'.manifest')
            & (Join-Path $Compiler 'BinaryAssetBuilder.ManifestInspector.exe') sdk-save-map-static-catalog $Provider $manifest | Out-Null
            if($LASTEXITCODE){throw 'Could not fingerprint the static map provider.'}
        }
        $rows.Add('StaticMapProvider='+[Reborn.PackageFile]::Hash($manifest))
    }
    $rows.Add('StockDataRoot='+$Settings.SdkDataRoot)
    return [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes(($rows -join [char]0))))
}

#-------------------------------------------------------------------------------------------------
<# Reborn: share path and content checks across maps only for one validation pass, then discard them before publication. #>
#-------------------------------------------------------------------------------------------------
function New-RebornMapWitness([string[]]$Roots) {
    @{
        Roots=@($Roots | Where-Object {$_} | ForEach-Object {[IO.Path]::TrimEndingDirectorySeparator([IO.Path]::GetFullPath($_))+[IO.Path]::DirectorySeparatorChar})
        Files=[Collections.Generic.Dictionary[string,string]]::new([StringComparer]::OrdinalIgnoreCase)
        Kinds=[Collections.Generic.Dictionary[string,int]]::new([StringComparer]::OrdinalIgnoreCase)
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: replay captured alias precedence and missing ART witnesses without following a redirected candidate. #>
#-------------------------------------------------------------------------------------------------
function Get-RebornMapInputKind([string]$Path,[hashtable]$Witness) {
    if(![IO.Path]::IsPathFullyQualified($Path) -or [IO.Path]::GetFullPath($Path)-ine $Path){throw 'Invalid map input witness path.'}
    $inside=$false
    foreach($root in $Witness.Roots){if($Path.StartsWith($root,[StringComparison]::OrdinalIgnoreCase)){$inside=$true;break}}
    if(!$inside){throw ('Map input witness escaped SDK input roots: '+$Path)}
    $kind=0
    if($Witness.Kinds.TryGetValue($Path,[ref]$kind)){return $kind}
    [Reborn.PackageFile]::CheckPath($Path,$false)
    $kind=if([IO.Directory]::Exists($Path)){2}elseif([IO.File]::Exists($Path)){1}else{0}
    $Witness.Kinds.Add($Path,$kind)
    return $kind
}

#-------------------------------------------------------------------------------------------------
<# Reborn: verify every source and resource hash, including late mod shadows of previously selected stock files. #>
#-------------------------------------------------------------------------------------------------
function Assert-RebornMapWitness($Plan,[hashtable]$Witness) {
    $hasEntry=$false
    foreach($source in $Plan.Sources){
        $path=[string]$source.Path
        if($path-ieq $Plan.Entry){$hasEntry=$true}
        if((Get-RebornMapInputKind $path $Witness)-ne 1){throw ('Map source disappeared: '+$path)}
        $hash=$null
        if(!$Witness.Files.TryGetValue($path,[ref]$hash)){$hash=[Reborn.PackageFile]::Hash($path);$Witness.Files.Add($path,$hash)}
        if($hash-cne $source.Sha256){throw ('Map input changed: '+$path)}
    }
    if(!$hasEntry){throw 'Map receipt has no entry witness.'}
    foreach($search in $Plan.Searches){
        $winner=$null
        foreach($path in $search.Candidates){
            $kind=Get-RebornMapInputKind $path $Witness
            if($kind-eq 2){throw ('Map alias is now a directory: '+$path)}
            if($kind-eq 1){$winner=$path;break}
        }
        if($search.ExpectedMissing){
            if($winner -or $search.Layer-cnotin @('missing-art-all','missing-art-instance') -or $search.LogicalPath-notmatch '^ART:'){throw ('Missing map ART include changed: '+$search.LogicalPath)}
        }elseif(!$winner -or $winner-ine $search.PhysicalPath){throw ('Map alias winner changed: '+$search.LogicalPath)}
    }
    $folder=[IO.Path]::GetDirectoryName($Plan.Entry)
    $loose=@(Get-ChildItem -LiteralPath $folder -File | Where-Object Extension -in @('.map','.tga') | Sort-Object Name)
    if(($loose.Name -join [char]0)-ine (@($Plan.Loose.Name) -join [char]0)){throw 'Map loose-file inventory changed.'}
    foreach($item in $loose){
        if((Get-RebornMapInputKind $item.FullName $Witness)-ne 1){throw 'Map loose file disappeared.'}
        $expected=$Plan.Loose | Where-Object Name -ieq $item.Name
        if([Reborn.PackageFile]::Hash($item.FullName)-cne $expected.Sha256){throw ('Map loose file changed: '+$item.Name)}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: verify paths and stored bytes before a saved map is exposed to publication. #>
#-------------------------------------------------------------------------------------------------
function Assert-RebornMapPayload([string]$Root,[object[]]$Files) {
    [Reborn.PackageFile]::CheckPath($Root,$false)
    $seen=[Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
    foreach($file in $Files){
        $relative=[string]$file.Relative
        if(!$relative -or [IO.Path]::IsPathRooted($relative) -or $relative -match '(^|[\\/])\.\.?([\\/]|$)|:' -or !$seen.Add($relative)){throw 'Invalid saved map payload name.'}
        $path=Join-Path $Root $relative
        [Reborn.PackageFile]::CheckPath($path,$false)
        if([Reborn.PackageFile]::Hash($path)-cne $file.Sha256){throw ('Saved map payload changed: '+$relative)}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: resolve only a complete last-successful generation, replay its witnesses and restore advisory metadata. #>
#-------------------------------------------------------------------------------------------------
function Read-RebornMapCache([string]$Root,[string]$Key,[string]$Name,[string]$Entry,[hashtable]$Witness,[string]$Target) {
    $folder=Join-Path $Root $Name
    $pointer=Join-Path $folder 'current.json'
    if(![IO.File]::Exists($pointer)){return}
    try{
        [Reborn.PackageFile]::CheckPath($pointer,$false)
        $record=Get-Content -LiteralPath $pointer -Raw | ConvertFrom-Json
        if($record.Format-cne 'RebornMapCache1' -or $record.Key-cne $Key -or $record.Name-ine $Name -or $record.Entry-ine $Entry){return}
        if($record.Generation-cnotmatch '\A[a-f0-9]{32}\z'){throw 'Invalid map cache generation.'}
        $generation=Join-Path $folder $record.Generation
        $payload=Join-Path $generation 'payload'
        $plan=[pscustomobject]@{Name=$Name;Entry=$Entry;Stage=$payload;Sources=$record.Sources;Searches=$record.Searches;Loose=$record.Loose;Warnings=$record.Warnings;Files=$record.Files;Fresh=$false;Target=$Target}
        Assert-RebornMapWitness $plan $Witness
        Assert-RebornMapPayload $payload $plan.Files
        if([IO.File]::ReadAllText((Join-Path $payload 'map.version')) -cne ('_mod'+[char]13+[char]10)){throw 'Saved map marker differs.'}
        $names=@($plan.Files.Relative)
        foreach($required in @('map.version','map_mod.bin','map_mod.imp','map_mod.relo','map_mod.manifest')){
            if($required -notin $names){throw ('Saved map has no '+$required)}
        }
        $metadata=Join-Path $generation 'metadata'
        Assert-RebornMapPayload $metadata $record.Metadata
        foreach($file in $record.Metadata){
            if($file.Relative -cnotin @('asset-usage.json','source-input.json')){throw 'Invalid saved map diagnostic name.'}
            Copy-Item -LiteralPath (Join-Path $metadata $file.Relative) -Destination (Join-Path $Target $file.Relative)
        }
        return $plan
    }catch{
        # Reborn: any stale or damaged evidence falls back to the normal source compiler.
        return
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: install each fresh map generation after the build succeeds, retaining only its verified predecessor until the pointer is replaced. #>
#-------------------------------------------------------------------------------------------------
function Save-RebornMapCache([string]$Root,[string]$Key,[object[]]$Plans) {
    foreach($plan in $Plans){
        if(!$plan.Fresh){continue}
        $folder=Join-Path $Root $plan.Name
        [Reborn.PackageFile]::CheckPath($folder,$false)
        $id=[guid]::NewGuid().ToString('N')
        $directory=Join-Path $folder $id
        $meta=Join-Path $directory 'metadata'
        $pointer=Join-Path $folder 'current.json'
        [Reborn.PackageFile]::CheckPath($pointer,$false)
        $old=$null
        if([IO.File]::Exists($pointer)){
            try{$old=(Get-Content -LiteralPath $pointer -Raw | ConvertFrom-Json).Generation}catch{}
        }
        $null=New-Item -ItemType Directory -Path $meta -Force
        $metadata=@(foreach($relative in @('conscript/asset-usage.json','source/source-input.json')){
            $input=Join-Path $plan.Target $relative
            if([IO.File]::Exists($input)){
                $name=[IO.Path]::GetFileName($input)
                $destination=Join-Path $meta $name
                Copy-Item -LiteralPath $input -Destination $destination
                [pscustomobject]@{Relative=$name;Sha256=[Reborn.PackageFile]::Hash($destination)}
            }
        })
        [IO.Directory]::Move($plan.Stage,(Join-Path $directory 'payload'))
        $record=[pscustomobject]@{Format='RebornMapCache1';Key=$Key;Name=$plan.Name;Entry=$plan.Entry;Generation=$id;Sources=$plan.Sources;Searches=$plan.Searches;Loose=$plan.Loose;Warnings=$plan.Warnings;Files=$plan.Files;Metadata=$metadata}
        $next=Join-Path $directory 'receipt.json'
        [IO.File]::WriteAllText($next,($record|ConvertTo-Json -Depth 15 -Compress))
        [IO.File]::Move($next,$pointer,$true)
        if($old -cmatch '\A[a-f0-9]{32}\z' -and $old -cne $id){
            $previous=Join-Path $folder $old
            if([IO.Directory]::Exists($previous)){
                # Reborn: an old generation's cleanup cannot invalidate the pointer that already committed.
                try{Remove-DevAlphaOwned $previous $folder}catch{Write-SdkBuildLine ('Warning: Old map cache generation retained: '+$previous)}
            }
        }
    }
}
