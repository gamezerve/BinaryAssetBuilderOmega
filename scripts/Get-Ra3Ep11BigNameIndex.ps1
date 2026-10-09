# Reborn: recover scoped BIG name-index construction and collision-search evidence using pinned code and bounded directory-only snapshots.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[Parameter(Mandatory=$true)] [string] $SkuDefinitionPath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$indexSelfTest=$SelfTest;$indexAsJson=$AsJson
$mount=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11BigMountMetadata.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath
$SelfTest=$indexSelfTest;$AsJson=$indexAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: pin complete index builder, lowercase hash, unsigned comparator, count, enumeration, indexed lookup and ASCII comparison bodies independently. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11BigNameIndexCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0x596880;Length=149;Hash='21B1DE63A32E05EEC569C217BBC9B0C9E9FB9ACF907EBA7C3222762FFFBFE632'},
        [pscustomobject]@{Offset=0x5960e0;Length=97;Hash='69AD727781F97A7F74D1A82F39CF8C6D61329985809625CB7E748D3C3DE144A6'},
        [pscustomobject]@{Offset=0x596150;Length=25;Hash='FA89382A113AF6C57C4847346AD9A78E1B8F1647ED8B9749390D0D25056E1045'},
        [pscustomobject]@{Offset=0x5961f0;Length=87;Hash='F9428BC0E16DD5EA8DF6404A018950DF333FC673DAA961A3B1B709CACAEECD21'},
        [pscustomobject]@{Offset=0x596450;Length=763;Hash='796D6FFA3B17AA3CDE022DEB87B87E7AB15D0EBA458A4EEF32AD92E0D4D3F10B'},
        [pscustomobject]@{Offset=0x5962e0;Length=357;Hash='B02A2A2FF9261933F023D1059007B0F55D587F4EA6DFBB2ECF5FECBCB8583B7B'},
        [pscustomobject]@{Offset=0x592de0;Length=109;Hash='5647F4DBD7E9DFF6EDF9C39C715B144245ADAA060A920CB818518AFD2327B97C'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'BIG index slice outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed BIG name index code changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: model only printable ASCII lowercase hash arithmetic, seed 5381 and final byte swap; do not normalize separators or claim native locale behavior. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11BigAsciiNameHash([string] $Name) {
    if($Name.Length-gt 4096){throw 'Detached BIG name exceeds policy bound.'}
    [uint64]$value=5381
    foreach($character in $Name.ToCharArray()){
        [int]$code=$character
        if($code-lt 32 -or $code-gt 126){throw 'Detached BIG hash supports printable ASCII only.'}
        if($code-ge 65 -and $code-le 90){$code+=32}
        $value=(33*$value+$code)-band 4294967295
    }
    return [uint32]((($value-band 255)*16777216)+((($value-shr 8)-band 255)*65536)+((($value-shr 16)-band 255)*256)+(($value-shr 24)-band 255))
}

#-------------------------------------------------------------------------------------------------
<# Reborn: produce deterministic hash/directory-relative record pairs from checked BIG4/BIGF metadata, not native absolute pointers or native equal-key qsort order. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11DetachedBigNameIndex([byte[]] $Directory,[long] $ArchiveBytes) {
    if($Directory.Length-lt 16){throw 'Truncated BIG name directory.'}
    $magic=[Text.Encoding]::ASCII.GetString($Directory,0,4)
    if($magic-cne 'BIG4' -and $magic-cne 'BIGF'){throw 'Detached name index requires BIG4 or BIGF.'}
    $count=Read-ArchiveU32 $Directory 8
    if($count-gt 100000 -or (Read-ArchiveU32 $Directory 12)-ne $Directory.Length -or $ArchiveBytes-lt $Directory.Length){throw 'Detached name index directory bounds differ.'}
    $position=16;$rows=[Collections.Generic.List[object]]::new()
    for($i=0;$i-lt $count;$i++){
        $record=$position;$offset=Read-ArchiveU32 $Directory $position;$size=Read-ArchiveU32 $Directory ($position+4);$position+=8;$start=$position
        while($position-lt $Directory.Length -and $Directory[$position]-ne 0){
            if($Directory[$position]-lt 32 -or $Directory[$position]-gt 126 -or $position-$start-ge 4096){throw 'Unsupported detached BIG name.'}
            $position++
        }
        if($position-ge $Directory.Length -or $position-eq $start){throw 'Empty or unterminated detached BIG name.'}
        if($offset-lt $Directory.Length -or [uint64]$offset+[uint64]$size-gt $ArchiveBytes){throw 'Detached BIG entry payload range invalid.'}
        $name=[Text.Encoding]::ASCII.GetString($Directory,$start,$position-$start);$position++
        $rows.Add([pscustomobject]@{Hash=(Get-Ep11BigAsciiNameHash $name);RecordOffset=$record;Name=$name})
    }
    return @($rows|Sort-Object Hash,RecordOffset)
}
Assert-Ep11BigNameIndexCode $ep11Bytes
# Reborn: attribute the two observed IAT calls through the pinned image's original import-name thunks; no DLL is loaded.
$imports=@(foreach($entry in @(
    [pscustomobject]@{Va='0x00BD8360';ThunkRaw=9148716;NameRva=0x008bae30;NameRaw=9154098;Name='qsort'},
    [pscustomobject]@{Va='0x00BD859C';ThunkRaw=9149288;NameRva=0x008ba016;NameRaw=9150488;Name='tolower'}
)){
    $rva=[BitConverter]::ToUInt32($ep11Bytes,$entry.ThunkRaw)
    if($rva-ne $entry.NameRva){throw 'BIG name import thunk differs.'}
    if([Text.Encoding]::ASCII.GetString($ep11Bytes,$entry.NameRaw,$entry.Name.Length+1)-cne ($entry.Name+[char]0)){throw 'BIG name import symbol differs.'}
    [pscustomobject]@{IatVa=$entry.Va;Name=$entry.Name;Dll='MSVCR80.dll'}
})
$archives=@(foreach($archive in $stock.Archives){
    $snapshot=Read-ArchiveDirectory $archive.Path
    if($snapshot.Length-ne $archive.ArchiveBytes -or [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($snapshot.Bytes))-cne $archive.DirectorySha256){throw 'BIG name index directory changed.'}
    $rows=@(Get-Ep11DetachedBigNameIndex $snapshot.Bytes $snapshot.Length)
    if($rows.Count-ne $archive.EntryCount){throw 'BIG name index count differs.'}
    $pairs=[byte[]]::new(8*$rows.Count)
    for($i=0;$i-lt $rows.Count;$i++){
        [BitConverter]::GetBytes([uint32]$rows[$i].Hash).CopyTo($pairs,8*$i)
        [BitConverter]::GetBytes([uint32]$rows[$i].RecordOffset).CopyTo($pairs,8*$i+4)
    }
    $groups=@($rows|Group-Object Hash|Where-Object Count -GT 1)
    $collisions=@(foreach($group in $groups){
        $distinct=[Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
        foreach($row in $group.Group){[void]$distinct.Add($row.Name)}
        if($distinct.Count-gt 1){[pscustomobject]@{Hash=('{0:X8}' -f $group.Group[0].Hash);Names=@($distinct|Sort-Object)}}
    })
    [pscustomobject]@{Archive=[IO.Path]::GetFileName($archive.Path);EntryCount=$rows.Count;DirectorySha256=$archive.DirectorySha256;RelativePairSha256=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($pairs));RepeatedHashGroups=$groups.Count;DistinctNameHashCollisions=$collisions}
})
if($SelfTest){
    # Reborn: independent constants cover empty seed, ASCII case, separator distinction and a real two-name djb2 collision.
    foreach($fixture in @(
        [pscustomobject]@{Name='';Hash='05150000'},[pscustomobject]@{Name='a';Hash='06B60200'},[pscustomobject]@{Name='A';Hash='06B60200'},
        [pscustomobject]@{Name='data/static.manifest';Hash='5B543C9C'},[pscustomobject]@{Name='DATA/STATIC.MANIFEST';Hash='5B543C9C'},
        [pscustomobject]@{Name='data\static.manifest';Hash='E8FC93C1'},[pscustomobject]@{Name='ar';Hash='38775900'},[pscustomobject]@{Name='c0';Hash='38775900'}
    )){if(('{0:X8}' -f (Get-Ep11BigAsciiNameHash $fixture.Name))-cne $fixture.Hash){throw 'Detached BIG name hash fixture differs.'}}
    # Reborn: reject names outside printable ASCII diagnostic policy, rather than silently invoking locale-dependent conversions.
    foreach($name in @(([string][char]0),([string][char]233),('x'*4097))){$rejected=$false;try{$null=Get-Ep11BigAsciiNameHash $name}catch{$rejected=$true};if(-not $rejected){throw 'Unsupported BIG hash name admitted.'}}
    # Reborn: a detached two-entry collision directory retains both distinct names and deterministic relative record addresses.
    $directory=[byte[]]::new(38);[Text.Encoding]::ASCII.GetBytes('BIG4').CopyTo($directory,0)
    $directory[11]=2;$directory[15]=38;$directory[19]=38;$directory[30]=38
    [Text.Encoding]::ASCII.GetBytes('ar').CopyTo($directory,24);[Text.Encoding]::ASCII.GetBytes('c0').CopyTo($directory,35)
    $collision=@(Get-Ep11DetachedBigNameIndex $directory 38)
    if($collision.Count-ne 2 -or $collision[0].Hash-ne $collision[1].Hash -or $collision[0].RecordOffset-ne 16 -or $collision[1].RecordOffset-ne 27 -or $collision[0].Name-cne 'ar' -or $collision[1].Name-cne 'c0'){throw 'Detached collision directory differs.'}
    # Reborn: reject malformed magic, declared size, count, name termination/emptiness and payload range without touching any installed archive.
    foreach($damage in @('magic','size','count','termination','empty','range')){
        $faultDirectory=[byte[]]$directory.Clone()
        switch($damage){'magic'{$faultDirectory[0]=88};'size'{$faultDirectory[15]=39};'count'{$faultDirectory[8]=1};'termination'{$faultDirectory[37]=65};'empty'{$faultDirectory[24]=0};'range'{$faultDirectory[19]=37}}
        $rejected=$false;try{$null=Get-Ep11DetachedBigNameIndex $faultDirectory 38}catch{$rejected=$true};if(-not $rejected){throw 'Malformed detached BIG index admitted.'}
    }
    # Reborn: damage only private code copies across every complete reviewed body.
    foreach($offset in @(0x59689e,0x5968d3,0x5968de,0x596904,0x596102,0x59611e,0x59615c,0x596166,0x59621c,0x596238,0x5966fd,0x59672b,0x59635c,0x596391,0x5963cd,0x592dff)){
        $fault=[byte[]]$ep11Bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11BigNameIndexCode $fault}catch{$rejected=$true};if(-not $rejected){throw 'BIG name index byte fault admitted.'}
    }
}
$indexAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($indexAfter))-cne $mount.ImageSha256){throw 'BIG name index image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$mount.ImageSha256;ReviewedWholeBodyPins=7;IndexBuilderVa='0x00996880';HashVa='0x009960E0';ComparatorVa='0x00996150';IndexedLookupVa='0x009962E0'
    IndexRecordBytes=8;HashSeed=5381;HashMultiplier=33;HashFinalByteSwap=$true;HashAsciiCaseInsensitive=$true;HashNormalizesPathSeparators=$false;NativeNonAsciiLocaleSemanticsRecovered=$false
    IndexPointerMemberOffset=8;NativeRecordsStoreAbsoluteDirectoryPointers=$true;DiagnosticRecordsUseDirectoryRelativeOffsets=$true;DiagnosticEqualHashTieBreak='RecordOffset';NativeEqualHashSortOrderProven=$false
    Imports=$imports;Archives=$archives;StockArchiveCount=$archives.Count;StockEntryCount=($archives|Measure-Object EntryCount -Sum).Sum
    ArchiveDirectoryMetadataBytesRead=$mount.ArchiveDirectoryMetadataBytesRead+($stock.Archives|Measure-Object DirectoryBytes -Sum).Sum;ArchivePayloadBytesRead=0
    IndexedLookupUsesUnsignedHashSearch=$true;IndexedLookupInitiallyComparesQueryName=$true;AdjacentCollisionComparisonUsesMidpointName=$true;CollisionResolutionCorrectnessProven=$false
    NativePreparedIndexBytesValidated=$false;FullIndexAndPathLookupSemanticsRecovered=$false;FullArchivePrecedenceAndUnmountSemanticsRecovered=$false
    ConfiguredStockArchiveSetComplete=$mount.ConfiguredStockArchiveSetComplete;MissingConfiguredArchives=$mount.MissingConfiguredArchives
    ReadOnly=$true;TargetExecuted=$false;ModPackageLoaded=$false;AuthenticEp1ProcessingHashRecovered=$false;ProductionBuildReady=$false
    FaultTestsPassed=[bool]$SelfTest;HashFixturesExecuted=$(if($SelfTest){8}else{0});NamePolicyRejectionsExecuted=$(if($SelfTest){3}else{0});CollisionDirectoryFixturesExecuted=$(if($SelfTest){1}else{0});MalformedDirectoryFixturesExecuted=$(if($SelfTest){6}else{0});MemoryFaultsExecuted=$(if($SelfTest){16}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 8}else{$report}
