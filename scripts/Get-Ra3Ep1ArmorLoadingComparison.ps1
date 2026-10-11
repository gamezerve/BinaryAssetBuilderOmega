# Reborn: compare archive routing metadata only; bounded directories/markers, selected manifest metadata, no BIN payloads or game execution.
[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
$repo=Split-Path -Parent $PSScriptRoot
$inspector=Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/bin/x86/Release/net8.0/BinaryAssetBuilder.ManifestInspector.exe'
#-------------------------------------------------------------------------------------------------
<# Reborn: read a BIG4/BIGF directory within four MiB and only tiny raw version markers; never allocate an entire archive. #>
#-------------------------------------------------------------------------------------------------
function Read-RebornLoadingDirectory([string]$Path) {
    $stream=[IO.File]::OpenRead($Path)
    try {
        $reader=[IO.BinaryReader]::new($stream,[Text.Encoding]::ASCII,$true)
        $magic=[Text.Encoding]::ASCII.GetString($reader.ReadBytes(4))
        if($magic-notin @('BIG4','BIGF')){throw 'Unsupported archive.'}
        $null=$reader.ReadBytes(4)
        $count=[Buffers.Binary.BinaryPrimitives]::ReadUInt32BigEndian($reader.ReadBytes(4))
        $size=[Buffers.Binary.BinaryPrimitives]::ReadUInt32BigEndian($reader.ReadBytes(4))
        if($count-gt 100000 -or $size-lt 16 -or $size-gt 4MB -or $size-gt $stream.Length){throw 'Directory bound exceeded.'}
        $entries=[Collections.Generic.List[object]]::new()
        for($index=0;$index-lt $count;$index++){
            if($stream.Position+8-gt $size){throw 'Truncated record.'}
            $offset=[Buffers.Binary.BinaryPrimitives]::ReadUInt32BigEndian($reader.ReadBytes(4))
            $length=[Buffers.Binary.BinaryPrimitives]::ReadUInt32BigEndian($reader.ReadBytes(4))
            $name=[Text.StringBuilder]::new();$terminated=$false
            while($stream.Position-lt $size){
                $value=$reader.ReadByte();if($value-eq 0){$terminated=$true;break}
                if($value-lt 32 -or $value-gt 126 -or $name.Length-ge 512){throw 'Invalid directory name.'}
                $null=$name.Append([char]$value)
            }
            if(-not $terminated -or $offset-lt $size -or [long]$offset+$length-gt $stream.Length){throw 'Invalid record range.'}
            $entries.Add([pscustomobject]@{Name=$name.ToString();Offset=$offset;Bytes=$length})
        }
        $markers=@(foreach($entry in $entries){if($entry.Name-match '(?:^|[\\/])(?:mod(?:_[lm])?|map)\.version$'){
            if($entry.Bytes-gt 64){throw 'Version marker exceeds bound.'}
            $stream.Position=$entry.Offset;$bytes=$reader.ReadBytes($entry.Bytes)
            if($bytes.Length-ne $entry.Bytes){throw 'Truncated marker.'}
            [pscustomobject]@{Name=$entry.Name;Hex=[Convert]::ToHexString($bytes);Text=[Text.Encoding]::ASCII.GetString($bytes)}
        }})
        [pscustomobject]@{Path=$Path;ArchiveBytes=$stream.Length;DirectoryBytes=$size;EntryCount=$count;Markers=$markers;ManifestEntries=@($entries|Where-Object {$_.Name-match '(?:^|[\\/])(?:mod(?:_[lm])?|map(?:_mod)?|reborn_armor_probe(?:_[lm])?)\.manifest$'})}
    }finally{$stream.Dispose()}
}
$inputs=@(
    'D:\OneDrive\Documents\Red Alert 3\Mods\Reborn\Reborn_1.07_Streams.big',
    'D:\OneDrive\Documents\Red Alert 3\Mods\deneme\Deneme_1.00_Streams.big',
    (Join-Path $repo 'artifacts/RebornArmorTrial-0f2a34bd39dd4eef9946d7e3b818f57f/RebornArmorProbe.big')
)
$results=@(foreach($path in $inputs){
    $directory=Read-RebornLoadingDirectory $path
    # Reborn: inspect only one selected mod manifest per package; campaign override markers remain a separate namespace.
    $selected=@($directory.ManifestEntries|Where-Object {$_.Name-eq 'data\mod.manifest' -or $_.Name-eq 'data\reborn_armor_probe.manifest'})
    $manifest=$null
    if($selected.Count-eq 1){
        if($selected[0].Bytes-gt 4MB){throw 'Selected manifest exceeds bound.'}
        $json=@(& $inspector inspect $path --entry $selected[0].Name --json)
        if($LASTEXITCODE-ne 0){throw 'Manifest inspection failed.'}
        $parsed=($json-join [Environment]::NewLine)|ConvertFrom-Json
        $manifest=$parsed|Select-Object Source,Name,Header,ReferencedManifests,ValidationErrors
    }
    [pscustomobject]@{Directory=$directory;SelectedManifest=$manifest;DefaultModManifestPresent=(@($directory.ManifestEntries|Where-Object Name -EQ 'data\mod.manifest').Count-eq 1)}
})
[pscustomobject]@{Archives=$results;BinPayloadRead=$false;GameExecuted=$false;ConfigConsumptionProven=$false;FailureCauseEstablished=$false}|ConvertTo-Json -Depth 12
