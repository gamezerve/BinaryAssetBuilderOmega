# Reborn: decode only two reviewed straight-line x86 initializer templates from one pinned image; never execute targets or pretend this is a general disassembler.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$initializerSelfTest=$SelfTest; $initializerAsJson=$AsJson
$runtime=. (Join-Path $PSScriptRoot 'Get-Ra3Ep1RuntimeTypeTable.ps1') -ImagePath $ImagePath
$SelfTest=$initializerSelfTest; $AsJson=$initializerAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: require exact instruction boundaries, mapped data/code addresses and paired hash slots before reporting a tiny straight-line initializer. #>
#-------------------------------------------------------------------------------------------------
function Read-TypeInitializer([byte[]] $Bytes,[int] $Offset,[bool] $HasVtable,$HashRows) {
    $length=if($HasVtable){44}else{34}
    if ($Offset -lt 0x1000 -or $Offset+$length -gt 0x7d2000 -or $Offset%16 -ne 0) { throw 'Initializer is outside the reviewed aligned .text bounds.' }
    $call2=if($HasVtable){35}else{25}
    $finish=if($HasVtable){40}else{30}
    $required=@(@(0,0xA1),@(5,0x68),@(10,0xA3),@(15,0xE8),@(20,0x68),@($call2,0xE8),@($finish,0x83),@(($finish+1),0xC4),@(($finish+2),8),@(($finish+3),0xC3))
    if($HasVtable){$required+=@(@(25,0xC7),@(26,5))}
    foreach($pair in $required){if($Bytes[$Offset+$pair[0]] -ne $pair[1]){throw 'Initializer opcode template differs.'}}
    $source=[BitConverter]::ToUInt32($Bytes,$Offset+1)
    $object=[BitConverter]::ToUInt32($Bytes,$Offset+6)
    $store=[BitConverter]::ToUInt32($Bytes,$Offset+11)
    $cleanup=[BitConverter]::ToUInt32($Bytes,$Offset+21)
    if(-not $HashRows.ContainsKey($source) -or $store -ne [long]$object+8 -or
        $object -lt 0xcbc000 -or $store+4 -gt 0xce9000 -or $cleanup -lt 0x401000 -or $cleanup -ge 0xbd2000){throw 'Initializer hash/object/cleanup address relationship differs.'}
    $call1Va=[long]0x400000+$Offset+20+[BitConverter]::ToInt32($Bytes,$Offset+16)
    $call2Va=[long]0x400000+$Offset+$call2+5+[BitConverter]::ToInt32($Bytes,$Offset+$call2+1)
    if($call1Va -ne 0x417400 -or $call2Va -ne 0x4d9a85){throw 'Reviewed initializer call targets differ.'}
    $vtable=$null
    if($HasVtable){
        if([BitConverter]::ToUInt32($Bytes,$Offset+27) -ne $object){throw 'Vtable write is not to the same runtime object.'}
        $vtable=[BitConverter]::ToUInt32($Bytes,$Offset+31)
        if($vtable -lt 0xbd2000 -or $vtable -ge 0xcbc000){throw 'Vtable candidate is outside pinned .rdata.'}
    }
    $row=$HashRows[$source]
    return [pscustomobject]@{
        Name=$row.Name; TypeHash=$row.TypeHash; Offset=('0x{0:X8}' -f $Offset); PreferredVa=('0x{0:X8}' -f (0x400000+$Offset))
        Length=$length; BytesHex=[Convert]::ToHexString($Bytes,$Offset,$length); Sha256=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$Offset..($Offset+$length-1)]))
        HashSourceVa=('0x{0:X8}' -f $source); ObjectVa=('0x{0:X8}' -f $object); HashStoreVa=('0x{0:X8}' -f $store); ObjectHashOffset=8
        CommonCallVa=('0x{0:X8}' -f $call1Va); SecondCallVa=('0x{0:X8}' -f $call2Va); PushedFunctionVa=('0x{0:X8}' -f $cleanup)
        HasVtableWrite=$HasVtable; VtableCandidateVa=$(if($null-ne $vtable){'0x{0:X8}' -f $vtable}else{$null})
        FunctionEntryFlowProved=$false; CalledFunctionSemanticsProved=$false
    }
}

# Reborn: immutable image bytes are already artifact-pinned by the table tool; associate source addresses only with its reviewed rows.
$hashRows=[Collections.Generic.Dictionary[uint32,object]]::new()
foreach($row in $runtime.Rows){$hashRows.Add([uint32](0x400000+[Convert]::ToInt32($row.HashOffset.Substring(2),16)),$row)}
$raw=[Text.Encoding]::Latin1.GetString($bytes,0x1000,8196096)
$patterns=@(
    [pscustomobject]@{Vtable=$false;Regex='(?s)\xA1.{4}\x68.{4}\xA3.{4}\xE8.{4}\x68.{4}\xE8.{4}\x83\xC4\x08\xC3'},
    [pscustomobject]@{Vtable=$true;Regex='(?s)\xA1.{4}\x68.{4}\xA3.{4}\xE8.{4}\x68.{4}\xC7\x05.{8}\xE8.{4}\x83\xC4\x08\xC3'}
)
$rows=@(foreach($pattern in $patterns){foreach($match in [regex]::Matches($raw,$pattern.Regex)){Read-TypeInitializer $bytes ($match.Index+0x1000) $pattern.Vtable $hashRows}}) | Sort-Object Name
if($rows.Count-ne 150 -or @($rows|Group-Object Name|Where-Object Count -ne 1).Count-ne 0){throw 'Initializer coverage or duplicate-name contract differs.'}
if($SelfTest){
    # Reborn: fault tests alter detached memory only after the authentic baseline succeeds; public image pins remain unchanged.
    $sample=@($rows|Where-Object Name -ceq Texture)[0];$offset=[Convert]::ToInt32($sample.Offset.Substring(2),16)
    foreach($case in @('opcode','source','store','call','vtable-object','vtable-range','alignment')){
        $fault=[byte[]]$bytes.Clone();$faultOffset=$offset
        switch($case){
            'opcode'{$fault[$offset]=0x90}
            'source'{[Array]::Clear($fault,$offset+1,4)}
            'store'{$fault[$offset+11]=$fault[$offset+11] -bxor 4}
            'call'{$fault[$offset+16]=$fault[$offset+16] -bxor 1}
            'vtable-object'{$fault[$offset+27]=$fault[$offset+27] -bxor 4}
            'vtable-range'{[Array]::Clear($fault,$offset+31,4)}
            'alignment'{$faultOffset++}
        }
        $rejected=$false;try{$null=Read-TypeInitializer $fault $faultOffset $true $hashRows}catch{$rejected=$true}
        if(-not $rejected){throw ('Initializer fault admitted: '+$case)}
    }
    # Reborn: the shorter template has its own second-call boundary and must not be decoded as the vtable-writing shape.
    $plain=@($rows|Where-Object Name -ceq ArmorTemplate)[0];$plainOffset=[Convert]::ToInt32($plain.Offset.Substring(2),16)
    $plainFault=[byte[]]$bytes.Clone();$plainFault[$plainOffset+25]=0x90;$rejected=$false
    try{$null=Read-TypeInitializer $plainFault $plainOffset $false $hashRows}catch{$rejected=$true}
    if(-not $rejected){throw 'Plain initializer second-call fault admitted.'}
}
$after=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($after)) -cne $runtime.ImageSha256){throw 'Image changed during initializer review.'}
$report=[pscustomobject]@{
    ImageSha256=$runtime.ImageSha256; RuntimeNameHashSha256=$runtime.OrderedNameHashSha256; DecodedInitializers=$rows.Count
    VtableTemplateCount=@($rows|Where-Object HasVtableWrite).Count; PlainTemplateCount=@($rows|Where-Object {-not $_.HasVtableWrite}).Count
    RuntimeRowsNotCovered=1342-$rows.Count; ReadOnly=$true; TargetExecuted=$false; GeneralDisassembly=$false; FullTableCallersVerified=$false
    Ep1ProcessingHashRecovered=$false; AllTypesHashDerivationRecovered=$false; ProductionBuildReady=$false; FaultTestsPassed=[bool]$SelfTest; FaultCasesExecuted=$(if($SelfTest){8}else{0})
    Rows=$rows
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 8}else{$report}
