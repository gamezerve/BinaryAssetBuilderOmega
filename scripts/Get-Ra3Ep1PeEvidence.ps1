# Reborn: statically inspect small PE images only; never load target code, dump asset streams or infer compiler identity from runtime symbols.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string[]] $ImagePaths, [switch] $AsJson)
$ErrorActionPreference = 'Stop'

#-------------------------------------------------------------------------------------------------
<# Reborn: snapshot an explicit small file after rejecting reparse ancestors and oversized asset-stream inputs. #>
#-------------------------------------------------------------------------------------------------
function Read-PeSnapshot([string] $Path) {
    if (-not [IO.Path]::IsPathFullyQualified($Path)) { throw 'PE paths must be absolute.' }
    $item = Get-Item -LiteralPath $Path -Force
    if ($item.PSIsContainer -or $item.Length -gt 16777216) { throw 'PE image exceeds the 16 MiB file bound or is not a file.' }
    $ancestor = $item
    while ($null -ne $ancestor) {
        if (($ancestor.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) { throw 'Reparse PE paths are not admitted.' }
        if ($ancestor -is [IO.FileInfo]) { $ancestor=$ancestor.Directory } else { $ancestor=$ancestor.Parent }
    }
    $stream = [IO.File]::Open($item.FullName,[IO.FileMode]::Open,[IO.FileAccess]::Read,[IO.FileShare]::Read)
    try {
        if ($stream.Length -gt 16777216) { throw 'PE image grew beyond the bound.' }
        $bytes = [byte[]]::new([int]$stream.Length)
        $stream.ReadExactly($bytes,0,$bytes.Length)
        return ,$bytes
    } finally { $stream.Dispose() }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: map only small checked RVA ranges using managed PEReader, not the native loader. #>
#-------------------------------------------------------------------------------------------------
function Read-PeRange($Pe,[int] $Rva,[int] $Count) {
    if ($Rva -le 0 -or $Count -lt 1 -or $Count -gt 4096) { throw 'PE RVA range exceeds bounds.' }
    return ,([byte[]]$Pe.GetSectionData($Rva).GetContent(0,$Count))
}

#-------------------------------------------------------------------------------------------------
<# Reborn: require terminated printable PE names within a 1,024-byte bound. #>
#-------------------------------------------------------------------------------------------------
function Read-PeName($Pe,[int] $Rva) {
    $name = [Text.StringBuilder]::new()
    for ($index=0; $index -lt 1024; $index++) {
        $bytes = Read-PeRange $Pe ($Rva+$index) 1
        if ($bytes[0] -eq 0) {
            if ($name.Length -eq 0) { throw 'Empty PE name.' }
            return $name.ToString()
        }
        if ($bytes[0] -lt 32 -or $bytes[0] -gt 126) { throw 'Non-ASCII PE name.' }
        [void]$name.Append([char]$bytes[0])
    }
    throw 'Unterminated PE name exceeds bounds.'
}

#-------------------------------------------------------------------------------------------------
<# Reborn: retain bounded literal byte matches and small contexts without claiming instruction, pointer or table semantics. #>
#-------------------------------------------------------------------------------------------------
function Find-PeWord([string] $Raw,[byte[]] $Bytes,[uint32] $Word,[string] $Label) {
    $needle = [Text.Encoding]::Latin1.GetString([BitConverter]::GetBytes($Word))
    $matches = [Collections.Generic.List[object]]::new()
    $start = 0
    while (($offset=$Raw.IndexOf($needle,$start,[StringComparison]::Ordinal)) -ge 0) {
        if ($matches.Count -ge 128) { throw 'Literal word evidence exceeds match bound.' }
        $begin = [Math]::Max(0,$offset-16)
        $length = [Math]::Min(40,$Bytes.Length-$begin)
        $matches.Add([pscustomobject]@{ Offset=('0x{0:X8}' -f $offset); ContextOffset=('0x{0:X8}' -f $begin); ContextHex=[Convert]::ToHexString($Bytes,$begin,$length) })
        $start=$offset+1
    }
    return [pscustomobject]@{ Label=$Label; Word=('0x{0:X8}' -f $Word); Count=$matches.Count; Matches=$matches.ToArray(); SemanticsVerified=$false }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: record bounded static metadata and targeted printable-string offsets, explicitly leaving runtime/compiler compatibility unproved. #>
#-------------------------------------------------------------------------------------------------
function Get-PeEvidence([string] $Path) {
    $bytes = Read-PeSnapshot $Path
    $hash = [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bytes))
    $memory = [IO.MemoryStream]::new($bytes,$false)
    $pe = [System.Reflection.PortableExecutable.PEReader]::new($memory)
    try {
        $header = $pe.PEHeaders.PEHeader
        if ($null -eq $header) { throw 'Missing PE optional header.' }
        $imports = [Collections.Generic.List[string]]::new()
        $exports = [Collections.Generic.List[object]]::new()
        # Reborn: retain a small name sample to distinguish runtime API exports from compiler metadata entry points.
        $exportSamples = [Collections.Generic.List[string]]::new()
        $directory = $header.ImportTableDirectory
        if ($directory.RelativeVirtualAddress -ne 0) {
            $ended = $false
            for ($index=0; $index -lt 128; $index++) {
                if ($index*20+20 -gt $directory.Size) { throw 'Truncated import directory.' }
                $descriptor = Read-PeRange $pe ($directory.RelativeVirtualAddress+$index*20) 20
                if (@($descriptor | Where-Object { $_ -ne 0 }).Count -eq 0) { $ended=$true; break }
                $imports.Add((Read-PeName $pe ([BitConverter]::ToUInt32($descriptor,12))))
            }
            if (-not $ended) { throw 'Import descriptor count exceeds bounds.' }
        }
        $directory = $header.ExportTableDirectory
        $exportCount = 0
        if ($directory.RelativeVirtualAddress -ne 0) {
            $table = Read-PeRange $pe $directory.RelativeVirtualAddress 40
            $functionCount = [BitConverter]::ToUInt32($table,20)
            $exportCount = [BitConverter]::ToUInt32($table,24)
            if ($functionCount -gt 16384 -or $exportCount -gt 4096) { throw 'Export table count exceeds bounds.' }
            $functions = [BitConverter]::ToUInt32($table,28)
            $names = [BitConverter]::ToUInt32($table,32)
            $ordinals = [BitConverter]::ToUInt32($table,36)
            for ($index=0; $index -lt $exportCount; $index++) {
                $nameWord = Read-PeRange $pe ($names+$index*4) 4
                $name = Read-PeName $pe ([BitConverter]::ToUInt32($nameWord,0))
                if ($exportSamples.Count -lt 16) { $exportSamples.Add($name) }
                $ordinalWord = Read-PeRange $pe ($ordinals+$index*2) 2
                $slot = [BitConverter]::ToUInt16($ordinalWord,0)
                if ($slot -ge $functionCount) { throw 'Export ordinal outside function table.' }
                $functionWord = Read-PeRange $pe ($functions+$slot*4) 4
                $rva = [BitConverter]::ToUInt32($functionWord,0)
                if ($name -match '(?i)PathMusic|Pathfinder|Asset|Schema|TypeHash|BinaryAsset') {
                    $exports.Add([pscustomobject]@{ Name=$name; Rva=('0x{0:X8}' -f $rva); Forwarded=($rva -ge $directory.RelativeVirtualAddress -and $rva -lt [long]$directory.RelativeVirtualAddress+$directory.Size) })
                }
            }
        }
        $debug = @($pe.ReadDebugDirectory() | Where-Object { $_.Type.ToString() -eq 'CodeView' } | ForEach-Object {
            $cv = $pe.ReadCodeViewDebugDirectoryData($_)
            [pscustomobject]@{ Path=$cv.Path; Guid=$cv.Guid.ToString(); Age=$cv.Age }
        })
        $strings = [Collections.Generic.List[object]]::new()
        $raw = [Text.Encoding]::Latin1.GetString($bytes)
        $filter = '(?i)Pathfinder|PathMusic|RA3EPMus|BinaryAssetBuilder|\.pdb|\.mus|\.mpf|AllTypesHash|ProcessingHash|TypeHash'
        # Reborn: scan bounded printable windows, not arbitrary output dumps; both byte offsets and encoding are retained.
        foreach ($encoding in @('ASCII','UTF16LE')) {
            $pattern = if ($encoding -eq 'ASCII') { '[\x20-\x7e]{4,1024}' } else { '(?:[\x20-\x7e]\x00){4,1024}' }
            foreach ($match in [regex]::Matches($raw,$pattern)) {
                $value = if ($encoding -eq 'ASCII') { $match.Value } else { [Text.Encoding]::Unicode.GetString($bytes,$match.Index,$match.Length) }
                if ($value -match $filter) {
                    if ($strings.Count -ge 512) { throw 'Targeted string report bound exceeded.' }
                    $strings.Add([pscustomobject]@{ Offset=('0x{0:X8}' -f $match.Index); Encoding=$encoding; Value=$value })
                }
            }
        }
        # Reborn: stock identity literals are comparison needles only; their occurrence is not recovery of the compiler ProcessingHash.
        $wordEvidence = @(
            Find-PeWord $raw $bytes ([Convert]::ToUInt32('9A651D89',16)) 'Stock PathMusicEvent TypeId'
            Find-PeWord $raw $bytes ([Convert]::ToUInt32('599CDAF2',16)) 'Stock PathMusicEvent TypeHash'
            Find-PeWord $raw $bytes ([Convert]::ToUInt32('5454A8E9',16)) 'Stock AllTypesHash'
        )
        # Reborn: derive PE32 preferred string VAs from section mappings and inventory literal references; these are not executed xrefs.
        $pointerCandidates = @($strings | Where-Object { $_.Encoding -eq 'ASCII' -and $_.Value -in @('PathMusicEvent','PathMusicEventRuntime','PathMusicMap','PathMusicMapRuntime','PathMusicTrack') } | ForEach-Object {
            $fileOffset = [Convert]::ToInt32($_.Offset.Substring(2),16)
            $sections = @($pe.PEHeaders.SectionHeaders | Where-Object { $fileOffset -ge $_.PointerToRawData -and $fileOffset -lt [long]$_.PointerToRawData+$_.SizeOfRawData })
            if ($sections.Count -ne 1) { throw 'Targeted string has no unique file/RVA mapping.' }
            $va = [uint64]$header.ImageBase+$sections[0].VirtualAddress+$fileOffset-$sections[0].PointerToRawData
            if ($va -le [uint32]::MaxValue) { Find-PeWord $raw $bytes ([uint32]$va) ('Preferred string VA: '+$_.Value) }
        })
        $second = Read-PeSnapshot $Path
        if ([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($second)) -cne $hash) { throw 'PE image changed during static review.' }
        return [pscustomobject]@{
            Path=[IO.Path]::GetFullPath($Path); ImageBytes=$bytes.Length; Sha256=$hash
            Machine=$pe.PEHeaders.CoffHeader.Machine.ToString(); Format=$header.Magic.ToString(); Managed=$pe.HasMetadata
            ImageBase=('0x{0:X}' -f $header.ImageBase); EntryPointRva=('0x{0:X8}' -f $header.AddressOfEntryPoint)
            Sections=@($pe.PEHeaders.SectionHeaders | ForEach-Object { [pscustomobject]@{ Name=$_.Name; Rva=('0x{0:X8}' -f $_.VirtualAddress); RawOffset=$_.PointerToRawData; RawBytes=$_.SizeOfRawData } })
            ImportDlls=$imports.ToArray(); NamedExportCount=$exportCount; ExportNameSamples=$exportSamples.ToArray(); TargetedExports=$exports.ToArray(); CodeView=$debug; TargetedStrings=$strings.ToArray()
            StockWordEvidence=$wordEvidence; StringPointerCandidates=$pointerCandidates
            ReadOnly=$true; TargetExecuted=$false; DisassemblyPerformed=$false; AuthenticHeaderRecovered=$false; Ep1ProcessingHashRecovered=$false; ProductionBuildReady=$false
        }
    } finally { $pe.Dispose(); $memory.Dispose() }
}
# Reborn: return evidence only; no report writes, global scans, registry mutations or target execution.
$reports = @($ImagePaths | ForEach-Object { Get-PeEvidence $_ })
if ($AsJson) { ConvertTo-Json -InputObject $reports -Depth 8 } else { $reports }
