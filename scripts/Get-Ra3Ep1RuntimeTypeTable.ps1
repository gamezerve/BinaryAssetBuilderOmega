# Reborn: characterize one pinned EP1 runtime name/hash table without activating compiler registrations or assuming authoring ProcessingHash equivalence.
[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)] [string] $ImagePath,
    [string] $EvidencePath = (Join-Path (Split-Path -Parent $PSScriptRoot) 'docs\RA3EP1_TYPE_TABLE_EVIDENCE.json'),
    [switch] $SelfTest,
    [switch] $AsJson
)
$ErrorActionPreference = 'Stop'

#-------------------------------------------------------------------------------------------------
<# Reborn: read bounded explicitly selected regular files without following reparse ancestors or accepting implicit relative paths. #>
#-------------------------------------------------------------------------------------------------
function Read-RuntimeInput([string] $Path,[int] $Maximum) {
    if (-not [IO.Path]::IsPathFullyQualified($Path)) { throw 'Runtime table inputs must be absolute.' }
    $item = Get-Item -LiteralPath $Path -Force
    if ($item.PSIsContainer -or $item.Length -gt $Maximum) { throw 'Runtime evidence input exceeds its file bound.' }
    $ancestor=$item
    while ($null -ne $ancestor) {
        if (($ancestor.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) { throw 'Reparse evidence paths are not admitted.' }
        if ($ancestor -is [IO.FileInfo]) { $ancestor=$ancestor.Directory } else { $ancestor=$ancestor.Parent }
    }
    $stream=[IO.File]::Open($item.FullName,[IO.FileMode]::Open,[IO.FileAccess]::Read,[IO.FileShare]::Read)
    try {
        if ($stream.Length -gt $Maximum) { throw 'Runtime evidence input grew beyond the bound.' }
        $bytes=[byte[]]::new([int]$stream.Length)
        $stream.ReadExactly($bytes,0,$bytes.Length)
        return ,$bytes
    } finally { $stream.Dispose() }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: interpret only the reviewed fixed PE32 string mapping; arbitrary pointer-shaped values never become runtime names. #>
#-------------------------------------------------------------------------------------------------
function Read-RuntimeName([byte[]] $Bytes,[int] $PointerOffset) {
    if ($PointerOffset -lt 0 -or $PointerOffset -gt $Bytes.Length-4) { throw 'Runtime pointer slot outside image.' }
    $va=[BitConverter]::ToUInt32($Bytes,$PointerOffset)
    $offset=[long]$va-0x400000
    if ($offset -lt 0x7d2000 -or $offset -ge 0x8bc000) { throw 'Runtime name pointer outside pinned .rdata mapping.' }
    $name=[Text.StringBuilder]::new()
    for ($index=0; $index -lt 256 -and $offset+$index -lt 0x8bc000; $index++) {
        $value=$Bytes[$offset+$index]
        if ($value -eq 0) {
            $text=$name.ToString()
            if ($text -cnotmatch '^[A-Za-z_][A-Za-z0-9_]*$') { throw 'Runtime name is not a nonempty type identifier.' }
            return [pscustomobject]@{ Name=$text; StringOffset=('0x{0:X8}' -f $offset) }
        }
        if ($value -lt 32 -or $value -gt 126) { throw 'Runtime name contains non-ASCII bytes.' }
        [void]$name.Append([char]$value)
    }
    throw 'Runtime name is unterminated or exceeds 256 bytes.'
}

#-------------------------------------------------------------------------------------------------
<# Reborn: decode the pinned contiguous parallel arrays plus independently located Texture slot, retaining per-row provenance and rejecting guessed boundaries. #>
#-------------------------------------------------------------------------------------------------
function Read-RuntimeRows([byte[]] $Bytes) {
    if ($Bytes.Length -ne 9484336) { throw 'Runtime table image length differs.' }
    $rows=[Collections.Generic.List[object]]::new()
    $names=[Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    # Reborn: six weather enum pointers precede the array and must never be mistaken for asset-type slots.
    $slots=@([pscustomobject]@{ Pointer=0x8bc00c; Hash=0x7e00d4; Role='separate-texture' }) + @(for($p=0x8bc0a8; $p -lt 0x8bd59c; $p+=4) {
        [pscustomobject]@{ Pointer=$p; Hash=$p-0xdbfd0; Role='parallel-array' }
    })
    foreach ($slot in $slots) {
        $name=Read-RuntimeName $Bytes $slot.Pointer
        if (-not $names.Add($name.Name)) { throw 'Duplicate runtime type name.' }
        if ($slot.Role -eq 'separate-texture' -and $name.Name -cne 'Texture') { throw 'Separate Texture pointer differs.' }
        $hash=[BitConverter]::ToUInt32($Bytes,$slot.Hash)
        if ($hash -eq 0) { throw 'Runtime table contains a zero hash.' }
        $rows.Add([pscustomobject]@{ Name=$name.Name; TypeHash=('0x{0:X8}' -f $hash); NamePointerOffset=('0x{0:X8}' -f $slot.Pointer); HashOffset=('0x{0:X8}' -f $slot.Hash); StringOffset=$name.StringOffset; Role=$slot.Role })
    }
    if ($rows.Count -ne 1342 -or $rows[1].Name -cne 'AudioFileRuntime' -or $rows[-1].Name -cne 'DamageFXSettings') { throw 'Runtime table boundaries differ.' }
    return ,$rows.ToArray()
}

#-------------------------------------------------------------------------------------------------
<# Reborn: correlate every independent observed stock root fingerprint with exactly one decoded runtime name/hash, without admitting unobserved rows to production. #>
#-------------------------------------------------------------------------------------------------
function Compare-RuntimeEvidence([object[]] $Rows,$Evidence) {
    if ($Evidence.Target -cne 'RA3 Uprising EP1' -or $Evidence.ExpectedAllTypesHash -cne '0x5454A8E9' -or
        $Evidence.ObservedTypeCount -ne 254 -or $Evidence.Types.Count -ne 254 -or $Evidence.Inputs.Count -ne 4) { throw 'Independent stock evidence contract differs.' }
    $lookup=[Collections.Generic.Dictionary[string,object]]::new([StringComparer]::Ordinal)
    foreach ($row in $Rows) { $lookup.Add($row.Name,$row) }
    $seen=[Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    foreach ($type in $Evidence.Types) {
        if (-not $seen.Add($type.TypeName) -or $type.Fingerprints.Count -ne 1 -or $type.Fingerprints[0].TypeName -cne $type.TypeName -or
            -not $lookup.ContainsKey($type.TypeName) -or $lookup[$type.TypeName].TypeHash -cne $type.Fingerprints[0].TypeHash) { throw ('Stock/runtime name-hash mismatch: '+$type.TypeName) }
    }
    return $seen.Count
}

# Reborn: absolute artifact pin is required before fixed offsets are interpreted; there is no CLI bypass for other builds or mutated files.
$bytes=Read-RuntimeInput $ImagePath 16777216
$imageHash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bytes))
if ($imageHash -cne 'ABA6A8825A744A78487837D78AD5992962257CE68404F639D600F11DEEA2580B') { throw 'Unreviewed EP1 executable identity; fixed runtime table offsets are not admitted.' }
$evidenceBytes=Read-RuntimeInput $EvidencePath 2097152
$evidenceHash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($evidenceBytes))
# Reborn: independent root observations are a reviewed immutable evidence artifact, not caller-authored name/hash claims.
if ($evidenceHash -cne '1E6220A5BECA5039C20EE4AEE5DB9DDE0D28F56D8FE70D90E217928B86C75C58') { throw 'Unreviewed stock type evidence identity.' }
$evidence=[Text.Encoding]::UTF8.GetString($evidenceBytes) | ConvertFrom-Json
$rows=Read-RuntimeRows $bytes
$matched=Compare-RuntimeEvidence $rows $evidence
# Reborn: fingerprint the ordered decoded name/hash association separately from the exact raw hash-array bytes.
$canonical=($rows | ForEach-Object { $_.Name+"`t"+$_.TypeHash+"`n" }) -join ''
$tableHash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($canonical)))
$hashBlock=[byte[]]::new(5368)
[Array]::Copy($bytes,0x7e00d4,$hashBlock,0,5368)
$hashBlockSha=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($hashBlock))
# Reborn: recheck each actual manifest's raw bytes against the independently captured stock evidence; never open its adjacent BIN/RELO/IMP.
foreach ($input in $evidence.Inputs) {
    $manifestBytes=Read-RuntimeInput $input.Path 16777216
    if ([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($manifestBytes)) -cne $input.Sha256) { throw 'Stock manifest evidence is stale.' }
}
if ($SelfTest) {
    # Reborn: fault tests mutate private memory only after authentic baseline verification, never the pinned game image or public admission policy.
    foreach ($case in @('pointer','duplicate','zero','texture','boundary','stockhash')) {
        $fault=[byte[]]$bytes.Clone()
        switch ($case) {
            'pointer' { [Array]::Clear($fault,0x8bc0a8,4) }
            'duplicate' { [Array]::Copy($fault,0x8bc0a8,$fault,0x8bc0ac,4) }
            'zero' { [Array]::Clear($fault,0x7e00d8,4) }
            'texture' { [Array]::Copy($fault,0x8bc0a8,$fault,0x8bc00c,4) }
            'boundary' { [Array]::Copy($fault,0x8bd59c,$fault,0x8bd598,4) }
            'stockhash' { $fault[0x7e0504]=$fault[0x7e0504] -bxor 1 }
        }
        $rejected=$false
        try { $faultRows=Read-RuntimeRows $fault; $null=Compare-RuntimeEvidence $faultRows $evidence } catch { $rejected=$true }
        if (-not $rejected) { throw ('Runtime table fault admitted: '+$case) }
    }
    # Reborn: evidence shape/name/fingerprint faults must be rejected independently of the image pin and raw table validity.
    foreach ($case in @('target','count','duplicate','unknown','hash','fingerprints')) {
        $faultEvidence=[Text.Encoding]::UTF8.GetString($evidenceBytes) | ConvertFrom-Json
        switch ($case) {
            'target' { $faultEvidence.Target='RA3' }
            'count' { $faultEvidence.ObservedTypeCount=253 }
            'duplicate' { $faultEvidence.Types[1]=$faultEvidence.Types[0] }
            'unknown' { $faultEvidence.Types[0].TypeName='RebornUnknownRuntimeType'; $faultEvidence.Types[0].Fingerprints[0].TypeName='RebornUnknownRuntimeType' }
            'hash' { $faultEvidence.Types[0].Fingerprints[0].TypeHash='0x00000001' }
            'fingerprints' { $faultEvidence.Types[0].Fingerprints=@() }
        }
        $rejected=$false
        try { $null=Compare-RuntimeEvidence $rows $faultEvidence } catch { $rejected=$true }
        if (-not $rejected) { throw ('Runtime evidence fault admitted: '+$case) }
    }
}
$second=Read-RuntimeInput $ImagePath 16777216
$secondEvidence=Read-RuntimeInput $EvidencePath 2097152
if ([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($second)) -cne $imageHash -or
    [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($secondEvidence)) -cne $evidenceHash) { throw 'Runtime inputs changed during review.' }
$report=[pscustomobject]@{
    ImageSha256=$imageHash; EvidenceSha256=$evidenceHash; RuntimeRowCount=$rows.Count; ParallelRowCount=1341; SeparateTextureRows=1
    OrderedNameHashSha256=$tableHash; RawHashBlockSha256=$hashBlockSha; RawHashBlockBytes=5368
    MatchedStockRootTypes=$matched; ObservedManifestCount=4; UnobservedRuntimeRows=$rows.Count-$matched
    ParallelPointerStart='0x008BC0A8'; ParallelPointerEndExclusive='0x008BD59C'; ParallelHashStart='0x007E00D8'; ParallelHashEndExclusive='0x007E15CC'; PointerHashDelta='0x000DBFD0'
    ReadOnly=$true; TargetExecuted=$false; TableCallersVerified=$false; CompleteGameTypeUniverseProved=$false
    Ep1ProcessingHashRecovered=$false; AllTypesHashDerivationRecovered=$false; ProductionBuildReady=$false; FaultTestsPassed=[bool]$SelfTest; FaultCasesExecuted=$(if($SelfTest){12}else{0})
    Inputs=$evidence.Inputs; Rows=$rows
}
if ($AsJson) { ConvertTo-Json -InputObject $report -Depth 8 } else { $report }
