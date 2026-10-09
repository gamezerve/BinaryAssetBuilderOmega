# Reborn: decode independently located EP1 1.1 name/hash arrays and compare exact rows with pinned 1.0/stock evidence without relaxing either version identity.
[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)] [string] $ImagePath,
    [Parameter(Mandatory=$true)] [string] $BaselineImagePath,
    [string] $EvidencePath=(Join-Path (Split-Path -Parent $PSScriptRoot) 'docs\RA3EP1_TYPE_TABLE_EVIDENCE.json'),
    [switch] $SelfTest,[switch] $AsJson
)
$ErrorActionPreference='Stop'
$ep11Path=$ImagePath;$ep11SelfTest=$SelfTest;$ep11AsJson=$AsJson
# Reborn: reuse read-only file/evidence guards through the unchanged, strictly pinned 1.0 audit; preserve this wrapper's parameters across its dot-sourced param block.
$baseline=. (Join-Path $PSScriptRoot 'Get-Ra3Ep1RuntimeTypeTable.ps1') -ImagePath $BaselineImagePath -EvidencePath $EvidencePath
$ImagePath=$ep11Path;$SelfTest=$ep11SelfTest;$AsJson=$ep11AsJson
$ep11Bytes=Read-RuntimeInput $ImagePath 16777216
$ep11Hash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($ep11Bytes))
if($ep11Hash-cne 'B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B'){throw 'Unreviewed EP1 1.1 identity; independently reviewed table offsets are not admitted.'}

#-------------------------------------------------------------------------------------------------
<# Reborn: decode bounded ASCII identifiers using only the reviewed 1.1 PE32 .rdata mapping, not the older image's section bounds. #>
#-------------------------------------------------------------------------------------------------
function Read-Ep11RuntimeName([byte[]] $Bytes,[int] $PointerOffset) {
    if($PointerOffset-lt 0 -or $PointerOffset-gt $Bytes.Length-4){throw 'EP1 1.1 pointer slot outside image.'}
    $offset=[long][BitConverter]::ToUInt32($Bytes,$PointerOffset)-0x400000
    if($offset-lt 0x7d8000 -or $offset-ge 0x8c3000){throw 'EP1 1.1 name pointer outside reviewed .rdata.'}
    $name=[Text.StringBuilder]::new()
    for($index=0;$index-lt 256 -and $offset+$index-lt 0x8c3000;$index++){
        $value=$Bytes[$offset+$index]
        if($value-eq 0){
            $text=$name.ToString()
            if($text-cnotmatch '^[A-Za-z_][A-Za-z0-9_]*$'){throw 'EP1 1.1 runtime name is not a type identifier.'}
            return [pscustomobject]@{Name=$text;StringOffset=('0x{0:X8}' -f $offset)}
        }
        if($value-lt 32 -or $value-gt 126){throw 'EP1 1.1 runtime name contains non-ASCII bytes.'}
        [void]$name.Append([char]$value)
    }
    throw 'EP1 1.1 runtime name exceeds termination bound.'
}

#-------------------------------------------------------------------------------------------------
<# Reborn: decode the independently located 1.1 parallel arrays and separate Texture slot; adjacent weather enum names are not type entries. #>
#-------------------------------------------------------------------------------------------------
function Read-Ep11RuntimeRows([byte[]] $Bytes) {
    if($Bytes.Length-ne 13381632){throw 'EP1 1.1 image length differs.'}
    $rows=[Collections.Generic.List[object]]::new();$names=[Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    $slots=@([pscustomobject]@{Pointer=0x8c300c;Hash=0x7e61d4;Role='separate-texture'})+@(for($index=0;$index-lt 1341;$index++){
        [pscustomobject]@{Pointer=0x8c30a8+4*$index;Hash=0x7e61d8+4*$index;Role='parallel-array'}
    })
    foreach($slot in $slots){
        $name=Read-Ep11RuntimeName $Bytes $slot.Pointer
        if(-not $names.Add($name.Name)){throw 'Duplicate EP1 1.1 runtime name.'}
        if($slot.Role-eq 'separate-texture' -and $name.Name-cne 'Texture'){throw 'EP1 1.1 separate Texture differs.'}
        $hash=[BitConverter]::ToUInt32($Bytes,$slot.Hash)
        if($hash-eq 0){throw 'EP1 1.1 runtime hash is zero.'}
        $rows.Add([pscustomobject]@{Name=$name.Name;TypeHash=('0x{0:X8}' -f $hash);NamePointerOffset=('0x{0:X8}' -f $slot.Pointer);HashOffset=('0x{0:X8}' -f $slot.Hash);StringOffset=$name.StringOffset;Role=$slot.Role})
    }
    if($rows.Count-ne 1342 -or $rows[1].Name-cne 'AudioFileRuntime' -or $rows[-1].Name-cne 'DamageFXSettings'){throw 'EP1 1.1 table boundaries differ.'}
    return ,$rows.ToArray()
}

#-------------------------------------------------------------------------------------------------
<# Reborn: require ordered row identity across the two pinned runtime versions while intentionally ignoring their independently decoded file coordinates. #>
#-------------------------------------------------------------------------------------------------
function Compare-Ep11BaselineRows([object[]] $Current,[object[]] $Baseline) {
    if($Current.Count-ne 1342 -or $Baseline.Count-ne 1342){throw 'Cross-version runtime row count differs.'}
    for($index=0;$index-lt $Current.Count;$index++){
        if($Current[$index].Name-cne $Baseline[$index].Name -or $Current[$index].TypeHash-cne $Baseline[$index].TypeHash -or
           $Current[$index].Role-cne $Baseline[$index].Role){throw ('Cross-version runtime identity differs at row '+$index)}
    }
    return $Current.Count
}
$ep11Rows=Read-Ep11RuntimeRows $ep11Bytes
$ep11Matched=Compare-RuntimeEvidence $ep11Rows $evidence
$ep11Identical=Compare-Ep11BaselineRows $ep11Rows $baseline.Rows
$ep11Canonical=($ep11Rows|ForEach-Object {$_.Name+"`t"+$_.TypeHash+"`n"}) -join ''
$ep11TableHash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($ep11Canonical)))
$ep11BlockHash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$ep11Bytes[0x7e61d4..0x7e76cb]))
if($ep11TableHash-cne $baseline.OrderedNameHashSha256 -or $ep11BlockHash-cne $baseline.RawHashBlockSha256){throw 'Cross-version runtime fingerprints differ.'}
if($SelfTest){
    # Reborn: detached mutations reject out-of-section pointers, duplicates, zero hashes, Texture substitution, weather inclusion, shifted tail and stock hash changes.
    foreach($case in @('pointer','duplicate','zero','texture','weather','boundary','stockhash')){
        $fault=[byte[]]$ep11Bytes.Clone()
        switch($case){
            'pointer' {[Array]::Clear($fault,0x8c30a8,4)}
            'duplicate' {[Array]::Copy($fault,0x8c30a8,$fault,0x8c30ac,4)}
            'zero' {[Array]::Clear($fault,0x7e61d8,4)}
            'texture' {[Array]::Copy($fault,0x8c30a8,$fault,0x8c300c,4)}
            'weather' {[Array]::Copy($fault,0x8c30a4,$fault,0x8c30a8,4)}
            'boundary' {[Array]::Copy($fault,0x8c459c,$fault,0x8c4598,4)}
            'stockhash' {$fault[0x7e6604]=$fault[0x7e6604]-bxor 1}
        }
        $rejected=$false
        try{$faultRows=Read-Ep11RuntimeRows $fault;$null=Compare-RuntimeEvidence $faultRows $evidence;$null=Compare-Ep11BaselineRows $faultRows $baseline.Rows}catch{$rejected=$true}
        if(-not $rejected){throw ('EP1 1.1 runtime fault admitted: '+$case)}
    }
    # Reborn: independently reject ordered-row identity faults, including an unobserved nested type that stock-root-only checks would miss.
    foreach($case in @('name','hash','order')){
        $faultRows=$ep11Rows|ConvertTo-Json -Depth 6|ConvertFrom-Json
        switch($case){
            'name' {$faultRows[1].Name='RebornUnknownNestedType'}
            'hash' {$faultRows[1].TypeHash='0x00000001'}
            'order' {$swap=$faultRows[1];$faultRows[1]=$faultRows[2];$faultRows[2]=$swap}
        }
        $rejected=$false;try{$null=Compare-Ep11BaselineRows $faultRows $baseline.Rows}catch{$rejected=$true}
        if(-not $rejected){throw ('EP1 1.1 baseline fault admitted: '+$case)}
    }
}
# Reborn: recheck selected version/evidence snapshots; the inherited 1.0 audit already rechecked all four raw manifest identities without payload reads.
$ep11After=Read-RuntimeInput $ImagePath 16777216;$baselineAfter=Read-RuntimeInput $BaselineImagePath 16777216;$evidenceAfter=Read-RuntimeInput $EvidencePath 2097152
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($ep11After))-cne $ep11Hash -or
   [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($baselineAfter))-cne $baseline.ImageSha256 -or
   [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($evidenceAfter))-cne $baseline.EvidenceSha256){throw 'Version/evidence inputs changed during comparison.'}
$ep11Report=[pscustomobject]@{
    ImageSha256=$ep11Hash;BaselineImageSha256=$baseline.ImageSha256;EvidenceSha256=$baseline.EvidenceSha256;RuntimeRowCount=$ep11Rows.Count;ParallelRowCount=1341;SeparateTextureRows=1
    MatchedStockRootTypes=$ep11Matched;IdenticalOrderedBaselineRows=$ep11Identical;UnobservedRuntimeRows=$ep11Rows.Count-$ep11Matched;ObservedManifestCount=4
    OrderedNameHashSha256=$ep11TableHash;RawHashBlockSha256=$ep11BlockHash;RawHashBlockBytes=5368
    ParallelPointerStart='0x008C30A8';ParallelPointerEndExclusive='0x008C459C';ParallelHashStart='0x007E61D8';ParallelHashEndExclusive='0x007E76CC'
    SeparateTexturePointerOffset='0x008C300C';SeparateTextureHashOffset='0x007E61D4';PointerHashDelta='0x000DCED0'
    IndependentlyLocatedVersionArrays=$true;RuntimeIdentityMatchesPinnedBaseline=$true;ReadOnly=$true;TargetExecuted=$false;TableCallersVerified=$false
    CompleteGameTypeUniverseProved=$false;Ep1ProcessingHashRecovered=$false;AllTypesHashDerivationRecovered=$false;StreamLoaderCompatibilityProved=$false;ModPackageLoaded=$false;ProductionBuildReady=$false
    FaultTestsPassed=[bool]$SelfTest;FaultCasesExecuted=$(if($SelfTest){10}else{0});Inputs=$baseline.Inputs;Rows=$ep11Rows
}
if($AsJson){ConvertTo-Json -InputObject $ep11Report -Depth 8}else{$ep11Report}
