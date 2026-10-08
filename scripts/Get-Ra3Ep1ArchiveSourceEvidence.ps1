# Reborn: read explicit BIG directory metadata only; never extract payloads, execute codecs or infer authentic EP1 compiler identity.
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)] [string[]] $ArchivePaths,
    [switch] $AsJson
)
$ErrorActionPreference = 'Stop'

#-------------------------------------------------------------------------------------------------
<# Reborn: reject reparse ancestors before opening explicitly selected read-only archive inputs. #>
#-------------------------------------------------------------------------------------------------
function Assert-ArchivePath([string] $Path) {
    if (-not [IO.Path]::IsPathFullyQualified($Path)) { throw 'Archive paths must be absolute.' }
    $item = Get-Item -LiteralPath $Path -Force
    if ($item.PSIsContainer) { throw 'Expected an archive file.' }
    for ($current = $item; $null -ne $current; $current = $current.Parent) {
        if (($current.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) { throw 'Reparse archive paths are not admitted.' }
        # Reborn: FileInfo uses Directory while DirectoryInfo uses Parent.
        if ($current -is [IO.FileInfo]) {
            $current = $current.Directory
            if ($null -eq $current) { break }
            if (($current.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) { throw 'Reparse archive parent is not admitted.' }
        }
    }
    return $item.FullName
}

#-------------------------------------------------------------------------------------------------
<# Reborn: interpret a checked four-byte BIG directory integer without depending on host endianness. #>
#-------------------------------------------------------------------------------------------------
function Read-ArchiveU32([byte[]] $Bytes, [int] $Offset) {
    if ($Offset -lt 0 -or $Offset -gt $Bytes.Length - 4) { throw 'Truncated BIG directory integer.' }
    return [uint32](([uint64]$Bytes[$Offset] * 16777216) + ([uint64]$Bytes[$Offset+1] * 65536) + ([uint64]$Bytes[$Offset+2] * 256) + $Bytes[$Offset+3])
}

#-------------------------------------------------------------------------------------------------
<# Reborn: snapshot only the capped directory, keeping raw payload bytes entirely unread. #>
#-------------------------------------------------------------------------------------------------
function Read-ArchiveDirectory([string] $Path) {
    $resolved = Assert-ArchivePath $Path
    $stream = [IO.File]::Open($resolved, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::Read)
    try {
        [byte[]] $prefix = [byte[]]::new(16)
        $stream.ReadExactly($prefix, 0, 16)
        $magic = [Text.Encoding]::ASCII.GetString($prefix, 0, 4)
        if ($magic -cne 'BIG4' -and $magic -cne 'BIGF') { throw 'Input is not a BIG archive; raw BIN streams require manifest-directed inspection.' }
        $count = Read-ArchiveU32 $prefix 8
        $headerSize = Read-ArchiveU32 $prefix 12
        if ($count -gt 100000 -or $headerSize -lt 16 -or $headerSize -gt 16777216 -or $headerSize -gt $stream.Length) { throw 'BIG directory bounds exceeded.' }
        [byte[]] $directory = [byte[]]::new([int]$headerSize)
        [Array]::Copy($prefix, $directory, 16)
        $stream.ReadExactly($directory, 16, [int]$headerSize-16)
        return [pscustomobject]@{ Path=$resolved; Length=$stream.Length; Magic=$magic; Count=$count; Bytes=$directory }
    } finally { $stream.Dispose() }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: report name-based source candidates and index provenance, not claims about embedded payload contents or recovered headers. #>
#-------------------------------------------------------------------------------------------------
function Get-ArchiveEvidence([string] $Path) {
    $snapshot = Read-ArchiveDirectory $Path
    $position = 16
    $names = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
    # Reborn: duplicate names are inventory evidence; this command never resolves or extracts ambiguous entries.
    $duplicates = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
    $extensions = @{}
    $candidates = [Collections.Generic.List[object]]::new()
    for ($index = 0; $index -lt $snapshot.Count; $index++) {
        $offset = Read-ArchiveU32 $snapshot.Bytes $position
        $size = Read-ArchiveU32 $snapshot.Bytes ($position+4)
        $position += 8
        $start = $position
        while ($position -lt $snapshot.Bytes.Length -and $snapshot.Bytes[$position] -ne 0) {
            if ($snapshot.Bytes[$position] -gt 127 -or $snapshot.Bytes[$position] -lt 32 -or $position-$start -ge 4096) { throw 'Invalid or oversized BIG entry name.' }
            $position++
        }
        if ($position -ge $snapshot.Bytes.Length -or $position -eq $start) { throw 'Empty or unterminated BIG entry name.' }
        $name = [Text.Encoding]::ASCII.GetString($snapshot.Bytes, $start, $position-$start).Replace('\','/')
        $position++
        if (-not $names.Add($name)) { [void]$duplicates.Add($name) }
        if ($offset -lt $snapshot.Bytes.Length -or [uint64]$offset+[uint64]$size -gt $snapshot.Length) { throw 'BIG entry is outside payload bounds.' }
        $extension = [IO.Path]::GetExtension($name).ToLowerInvariant()
        if ($extension -eq '') { $extension = '(none)' }
        $extensions[$extension] = 1 + $extensions[$extension]
        # Reborn: list likely authoring/header/compiler names and runtime music containers without opening any of them.
        if ($extension -in @('.h','.hpp','.xml','.xsd','.pdb','.dll','.exe','.mpf','.mus') -or $name -match '(?i)pathfinder|pathmusic|ra3epmus|audioassets|music|track') {
            if ($candidates.Count -ge 10000) { throw 'Source-candidate report bound exceeded.' }
            $candidates.Add([pscustomobject]@{ Name=$name; Offset=$offset; StoredBytes=$size; Extension=$extension })
        }
    }
    $hash = [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($snapshot.Bytes))
    $second = Read-ArchiveDirectory $Path
    if ($second.Length -ne $snapshot.Length -or [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($second.Bytes)) -cne $hash) { throw 'Archive directory changed during review.' }
    return [pscustomobject]@{
        Path=$snapshot.Path; ArchiveBytes=$snapshot.Length; Magic=$snapshot.Magic; EntryCount=$snapshot.Count
        DirectoryBytes=$snapshot.Bytes.Length; DirectorySha256=$hash; MetadataBytesRead=2*$snapshot.Bytes.Length
        PayloadBytesRead=0; ReadOnly=$true; EmbeddedContentSearched=$false; AuthenticHeaderRecovered=$false
        Ep1ProcessingHashRecovered=$false; ProductionBuildReady=$false
        DuplicateNormalizedNames=@($duplicates | Sort-Object)
        ExtensionCounts=@($extensions.GetEnumerator() | Sort-Object Name | ForEach-Object { [pscustomobject]@{ Extension=$_.Name; Count=$_.Value } })
        Candidates=$candidates.ToArray()
    }
}

# Reborn: no recursive drive scan or output writes; callers choose explicit archives and any report destination themselves.
$reports = @($ArchivePaths | ForEach-Object { Get-ArchiveEvidence $_ })
if ($AsJson) { ConvertTo-Json -InputObject $reports -Depth 8 } else { $reports }
