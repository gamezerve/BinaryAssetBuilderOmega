# Reborn: inspect bounded config candidates from the reviewed SKU archives and two explicit loose directories; never launch or modify installed files.
[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$ImagePath,[Parameter(Mandatory=$true)][string]$BaselineImagePath,[Parameter(Mandatory=$true)][string]$SkuDefinitionPath,[switch]$SelfTest,[switch]$AsJson)
$ErrorActionPreference='Stop'
$inventorySelfTest=$SelfTest;$inventoryAsJson=$AsJson
$configStock=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11StockPackagePreflight.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath
$SelfTest=$inventorySelfTest;$AsJson=$inventoryAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: classify at most 64 KiB of config bytes without executing directives or decoding compressed data. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11ConfigText([byte[]]$Bytes) {
    if($Bytes.Length-gt 65536){throw 'Config payload exceeds 64 KiB policy.'}
    if($Bytes.Length-ge 2 -and $Bytes[0]-in @(0x10,0x90) -and $Bytes[1]-eq 0xFB){return [pscustomobject]@{Kind='RefPack-marker-only';Text=$null}}
    foreach($value in $Bytes){if($value-ne 9 -and $value-ne 10 -and $value-ne 13 -and ($value-lt 32 -or $value-gt 126)){return [pscustomobject]@{Kind='Opaque';Text=$null}}}
    return [pscustomobject]@{Kind='ASCII';Text=[Text.Encoding]::ASCII.GetString($Bytes)}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: enumerate config names from an independently bounds-checked BIG directory, refusing ambiguous normalized config names. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11ConfigEntries($Snapshot) {
    $cursor=16;$entries=[Collections.Generic.List[object]]::new()
    $seen=[Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
    for($i=0;$i-lt $Snapshot.Count;$i++){
        $offset=Read-ArchiveU32 $Snapshot.Bytes $cursor;$size=Read-ArchiveU32 $Snapshot.Bytes ($cursor+4);$cursor+=8;$start=$cursor
        while($cursor-lt $Snapshot.Bytes.Length -and $Snapshot.Bytes[$cursor]-ne 0){
            if($Snapshot.Bytes[$cursor]-lt 32 -or $Snapshot.Bytes[$cursor]-gt 126 -or $cursor-$start-ge 4096){throw 'Invalid config inventory directory name.'};$cursor++
        }
        if($cursor-ge $Snapshot.Bytes.Length -or $cursor-eq $start){throw 'Empty or unterminated config inventory name.'}
        $name=[Text.Encoding]::ASCII.GetString($Snapshot.Bytes,$start,$cursor-$start).Replace('\','/');$cursor++
        if($offset-lt $Snapshot.Bytes.Length -or [uint64]$offset+[uint64]$size-gt $Snapshot.Length){throw 'Config inventory entry outside archive.'}
        if([IO.Path]::GetExtension($name)-notin @('.cfg','.skudef')){continue}
        if(-not $seen.Add($name)){throw 'Ambiguous normalized config name in archive.'}
        if($entries.Count-ge 64 -or $size-gt 65536){throw 'Config inventory count or payload policy exceeded.'}
        $entries.Add([pscustomobject]@{Name=$name;Offset=$offset;StoredBytes=$size})
    }
    return $entries.ToArray()
}

# Reborn: detached fixtures do not write synthetic archives or exercise the game parser.
if($SelfTest){
    if((Get-Ep11ConfigText ([Text.Encoding]::ASCII.GetBytes("set-search-path big:;.`r`n"))).Kind-cne 'ASCII'){throw 'ASCII fixture failed.'}
    if((Get-Ep11ConfigText ([byte[]]@(0x10,0xFB,0))).Kind-cne 'RefPack-marker-only'){throw 'RefPack marker fixture failed.'}
    if((Get-Ep11ConfigText ([byte[]]@(0))).Kind-cne 'Opaque'){throw 'Opaque fixture failed.'}
    $rejected=$false;try{Get-Ep11ConfigText ([byte[]]::new(65537))|Out-Null}catch{$rejected=$true};if(-not $rejected){throw 'Payload bound fixture failed.'}
    # Reborn: construct private directory bytes only; no synthetic file is written into the installation.
    $fixtureName=[Text.Encoding]::ASCII.GetBytes('filesystem.cfg');$fixtureBytes=[byte[]]::new(24+$fixtureName.Length+1)
    $fixtureBytes[19]=[byte]$fixtureBytes.Length;$fixtureBytes[23]=1;[Array]::Copy($fixtureName,0,$fixtureBytes,24,$fixtureName.Length)
    $fixture=[pscustomobject]@{Bytes=$fixtureBytes;Count=1;Length=$fixtureBytes.Length+1}
    $found=@(Get-Ep11ConfigEntries $fixture)
    if($found.Count-ne 1 -or $found[0].Name-cne 'filesystem.cfg' -or $found[0].Offset-ne $fixtureBytes.Length){throw 'Directory selection fixture failed.'}
    $ignored=$fixtureBytes.Clone();$ignored[35]=[byte][char]'b';$ignored[36]=[byte][char]'i';$ignored[37]=[byte][char]'n'
    if(@(Get-Ep11ConfigEntries ([pscustomobject]@{Bytes=$ignored;Count=1;Length=$fixture.Length})).Count-ne 0){throw 'Non-config fixture failed.'}
    for($fault=0;$fault-lt 6;$fault++){
        $damaged=$fixtureBytes.Clone();$faultCount=1;$faultLength=$fixture.Length
        switch($fault){
            0{$damaged[19]=0}
            1{$damaged[$damaged.Length-1]=65}
            2{$damaged[24]=128}
            3{$faultCount=2}
            4{$faultLength=$fixtureBytes.Length}
            5{
                $damaged=[byte[]]::new(16+2*($fixtureBytes.Length-16));$faultCount=2;$faultLength=$damaged.Length+1
                [Array]::Copy($fixtureBytes,16,$damaged,16,$fixtureBytes.Length-16)
                [Array]::Copy($fixtureBytes,16,$damaged,$fixtureBytes.Length,$fixtureBytes.Length-16)
                $damaged[19]=[byte]$damaged.Length;$damaged[$fixtureBytes.Length+3]=[byte]$damaged.Length
            }
        }
        $rejected=$false;try{Get-Ep11ConfigEntries ([pscustomobject]@{Bytes=$damaged;Count=$faultCount;Length=$faultLength})|Out-Null}catch{$rejected=$true}
        if(-not $rejected){throw "Directory fault admitted: $fault"}
    }
}
$configItems=[Collections.Generic.List[object]]::new();$configMetadata=[long]$configStock.ArchiveDirectoryMetadataBytesRead;$configPayload=[long]0;$looseBytes=[long]0
foreach($archive in $configStock.Archives){
    $snapshot=Read-ArchiveDirectory $archive.Path;$configMetadata+=$snapshot.Bytes.Length
    if($snapshot.Length-ne $archive.ArchiveBytes -or [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($snapshot.Bytes))-cne $archive.DirectorySha256){throw 'Config inventory archive changed since SKU preflight.'}
    $selected=@(Get-Ep11ConfigEntries $snapshot)
    if($configItems.Count+$selected.Count-gt 64){throw 'Combined config candidate count exceeds policy.'}
    # Reborn: hold a read-only handle denying writers, compare payload twice, and recheck directory identity after the selected reads.
    $handle=[IO.File]::Open($archive.Path,[IO.FileMode]::Open,[IO.FileAccess]::Read,[IO.FileShare]::Read)
    try{
        if($handle.Length-ne $snapshot.Length){throw 'Config archive length changed.'}
        foreach($entry in $selected){
            $first=[byte[]]::new([int]$entry.StoredBytes);$second=[byte[]]::new([int]$entry.StoredBytes)
            $handle.Position=$entry.Offset;$handle.ReadExactly($first,0,$first.Length)
            $handle.Position=$entry.Offset;$handle.ReadExactly($second,0,$second.Length);$configPayload+=2*$first.Length
            $hash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($first))
            if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($second))-cne $hash){throw 'Config payload changed.'}
            $text=Get-Ep11ConfigText $first
            $configItems.Add([pscustomobject]@{Source='ConfiguredArchive';Path=$archive.Path;Name=$entry.Name;Offset=$entry.Offset;StoredBytes=$entry.StoredBytes;Sha256=$hash;Kind=$text.Kind;Text=$text.Text})
        }
        $after=Read-ArchiveDirectory $archive.Path;$configMetadata+=$after.Bytes.Length
        if($after.Length-ne $snapshot.Length -or [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($after.Bytes))-cne $archive.DirectorySha256){throw 'Config directory changed during inspection.'}
    }finally{$handle.Dispose()}
}
$looseDirectories=@([IO.Path]::GetDirectoryName([IO.Path]::GetFullPath($SkuDefinitionPath)),[IO.Path]::GetDirectoryName([IO.Path]::GetFullPath($ImagePath)))
foreach($directory in $looseDirectories){
    # Reborn: nonrecursive enumeration is restricted to the SKU root and selected image directory; file readers reject reparse paths.
    $loose=@(Get-ChildItem -LiteralPath $directory -File -Force|Where-Object{$_.Extension-in @('.cfg','.skudef')}|Sort-Object Name)
    if($configItems.Count+$loose.Count-gt 64){throw 'Combined loose/archive config count exceeds policy.'}
    foreach($file in $loose){
        $bytes=Read-RuntimeInput $file.FullName 65536;$again=Read-RuntimeInput $file.FullName 65536;$looseBytes+=$bytes.Length+$again.Length
        $hash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bytes))
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($again))-cne $hash){throw 'Loose config changed.'}
        $text=Get-Ep11ConfigText $bytes
        $configItems.Add([pscustomobject]@{Source='Loose';Path=$file.FullName;Name=$file.Name;Offset=0;StoredBytes=$bytes.Length;Sha256=$hash;Kind=$text.Kind;Text=$text.Text})
    }
}
$report=[pscustomobject]@{
    ImageSha256=$configStock.ImageSha256;SkuDefinitionSha256=$configStock.SkuDefinitionSha256
    ConfiguredArchiveCount=$configStock.ConfiguredArchiveCount;AvailableArchiveCount=$configStock.ArchiveCount;MissingConfiguredArchives=$configStock.MissingConfiguredArchives
    Archives=$configStock.Archives;TotalArchiveEntries=$configStock.TotalArchiveEntries;SkuMetadataBytesRead=$configStock.SkuMetadataBytesRead
    LooseDirectories=$looseDirectories;CandidateCount=$configItems.Count;Candidates=$configItems.ToArray()
    FilesystemConfigCandidateCount=@($configItems|Where-Object{[IO.Path]::GetFileName($_.Name)-ieq 'filesystem.cfg'}).Count
    ArchiveDirectoryMetadataBytesRead=$configMetadata;ArchivePayloadBytesRead=$configPayload;LooseConfigBytesRead=$looseBytes
    TextFixturesExecuted=$(if($SelfTest){3}else{0});PolicyRejectionsExecuted=$(if($SelfTest){1}else{0})
    DirectoryFixturesExecuted=$(if($SelfTest){2}else{0});DirectoryFaultsExecuted=$(if($SelfTest){6}else{0})
    ReadOnly=$true;CompressedConfigDecoded=$false;SearchWasRecursive=$false;UnconfiguredArchivesRead=$false
    EffectiveConfigPrecedenceProven=$false;FullFilesystemConfigAbsenceProven=$false;TargetExecuted=$false;ModPackageLoaded=$false;ProductionBuildReady=$false
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 8}else{$report}
