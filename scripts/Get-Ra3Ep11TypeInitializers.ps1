# Reborn: bind a narrowly decoded 150-object EP1 1.1 initializer sample to independently pinned runtime hashes and registry bodies without claiming full startup reachability.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$init11SelfTest=$SelfTest;$init11AsJson=$AsJson
$consumer11=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11HashConsumer.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath
$SelfTest=$init11SelfTest;$AsJson=$init11AsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: decode only two aligned straight-line x86 templates and require exact hash/object/registration relationships using reviewed 1.1 section bounds. #>
#-------------------------------------------------------------------------------------------------
function Read-Ep11TypeInitializer([byte[]] $Bytes,[int] $Offset,[bool] $HasVtable,$HashRows) {
    $length=if($HasVtable){44}else{34};$call2=if($HasVtable){35}else{25};$finish=if($HasVtable){40}else{30}
    if($Offset-lt 0x1000 -or $Offset+$length-gt 0x7d8000 -or $Offset%16-ne 0){throw 'EP1 1.1 initializer outside aligned .text bounds.'}
    $required=@(@(0,0xa1),@(5,0x68),@(10,0xa3),@(15,0xe8),@(20,0x68),@($call2,0xe8),@($finish,0x83),@(($finish+1),0xc4),@(($finish+2),8),@(($finish+3),0xc3))
    if($HasVtable){$required+=@(@(25,0xc7),@(26,5))}
    foreach($pair in $required){if($Bytes[$Offset+$pair[0]]-ne $pair[1]){throw 'EP1 1.1 initializer opcode template differs.'}}
    $source=[BitConverter]::ToUInt32($Bytes,$Offset+1);$object=[BitConverter]::ToUInt32($Bytes,$Offset+6)
    $store=[BitConverter]::ToUInt32($Bytes,$Offset+11);$cleanup=[BitConverter]::ToUInt32($Bytes,$Offset+21)
    if(-not $HashRows.ContainsKey($source) -or $store-ne [long]$object+8 -or $object-lt 0xcc3000 -or
       [long]$object+16-gt 0xcf0000 -or $object%4-ne 0 -or $cleanup-lt 0x401000 -or $cleanup-ge 0xbd8000){throw 'EP1 1.1 initializer source/object/store relationship differs.'}
    $register=[long]0x400000+$Offset+20+[BitConverter]::ToInt32($Bytes,$Offset+16)
    $second=[long]0x400000+$Offset+$call2+5+[BitConverter]::ToInt32($Bytes,$Offset+$call2+1)
    if($register-ne 0x417430 -or $second-ne 0x4d9e45){throw 'EP1 1.1 initializer call targets differ.'}
    $vtable=$null
    if($HasVtable){
        if([BitConverter]::ToUInt32($Bytes,$Offset+27)-ne $object){throw 'EP1 1.1 vtable store uses another object.'}
        $vtable=[BitConverter]::ToUInt32($Bytes,$Offset+31)
        if($vtable-lt 0xbd8000 -or $vtable-ge 0xcc3000){throw 'EP1 1.1 vtable candidate outside .rdata.'}
    }
    $row=$HashRows[$source]
    [pscustomobject]@{Name=$row.Name;TypeHash=$row.TypeHash;Offset=('0x{0:X8}' -f $Offset);PreferredVa=('0x{0:X8}' -f (0x400000+$Offset));Length=$length
        Sha256=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$Offset..($Offset+$length-1)]))
        HashSourceVa=('0x{0:X8}' -f $source);ObjectVa=('0x{0:X8}' -f $object);HashStoreVa=('0x{0:X8}' -f $store);ObjectHashOffset=8
        RegistrationCallVa='0x00417430';SecondCallVa='0x004D9E45';PushedFunctionVa=('0x{0:X8}' -f $cleanup);HasVtableWrite=$HasVtable
        VtableCandidateVa=$(if($null-ne $vtable){'0x{0:X8}' -f $vtable}else{$null});FunctionEntryReachabilityProved=$false;SecondCallSemanticsProved=$false}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: verify the sample's raw metadata TypeIds and zero-before-initialization hashes; preserve opaque word twelve instead of equating it with tokenization. #>
#-------------------------------------------------------------------------------------------------
function Read-Ep11InitializerObjects([byte[]] $Bytes,[object[]] $Initializers,$NameIds,$Observed) {
    return @(foreach($row in $Initializers){
        $offset=[Convert]::ToInt32($row.ObjectVa.Substring(2),16)-0x400000
        if($offset-lt 0x8c3000 -or $offset+16-gt 0x8f0000 -or $offset%4-ne 0){throw 'EP1 1.1 static metadata outside raw .data.'}
        $id='0x{0:X8}' -f [BitConverter]::ToUInt32($Bytes,$offset+4)
        if(-not $NameIds.ContainsKey($row.Name) -or $id-cne $NameIds[$row.Name] -or [BitConverter]::ToUInt32($Bytes,$offset+8)-ne 0){throw 'EP1 1.1 metadata TypeId/initial-zero relation differs.'}
        $opaque=[BitConverter]::ToUInt32($Bytes,$offset+12);$stock=$Observed.ContainsKey($row.Name)
        if($stock -and $Observed[$row.Name].TypeId-cne $id){throw 'EP1 1.1 metadata TypeId differs from stock evidence.'}
        [pscustomobject]@{Name=$row.Name;ObjectVa=$row.ObjectVa;TypeId=$id;RawHashBeforeInitialization='0x00000000';InitializedTypeHash=$row.TypeHash
            UnknownWord12=('0x{0:X8}' -f $opaque);ObservedStockRoot=$stock;UnknownWord12EqualsStockTokenized=$(if($stock){$opaque-eq $Observed[$row.Name].Fingerprints[0].Tokenized}else{$null})}
    })
}
$init11Hashes=[Collections.Generic.Dictionary[uint32,object]]::new()
foreach($row in $runtime11.Rows){$init11Hashes.Add([uint32](0x400000+[Convert]::ToInt32($row.HashOffset.Substring(2),16)),$row)}
$init11Raw=[Text.Encoding]::Latin1.GetString($ep11Bytes,0x1000,0x7d7000)
$init11Patterns=@(
    [pscustomobject]@{Vtable=$false;Regex='(?s)\xA1.{4}\x68.{4}\xA3.{4}\xE8.{4}\x68.{4}\xE8.{4}\x83\xC4\x08\xC3'},
    [pscustomobject]@{Vtable=$true;Regex='(?s)\xA1.{4}\x68.{4}\xA3.{4}\xE8.{4}\x68.{4}\xC7\x05.{8}\xE8.{4}\x83\xC4\x08\xC3'}
)
$init11Rows=@(foreach($shape in $init11Patterns){foreach($match in [regex]::Matches($init11Raw,$shape.Regex)){Read-Ep11TypeInitializer $ep11Bytes ($match.Index+0x1000) $shape.Vtable $init11Hashes}})|Sort-Object Name
if($init11Rows.Count-ne 150 -or @($init11Rows|Group-Object Name|Where-Object Count -ne 1).Count-ne 0 -or
   @($init11Rows|Where-Object HasVtableWrite).Count-ne 107){throw 'EP1 1.1 initializer coverage/uniqueness differs.'}
# Reborn: execute only the existing repo-owned managed name-hash diagnostic; native initializer/registration/cleanup code is never loaded or run.
$init11Inspector=Join-Path (Split-Path -Parent $PSScriptRoot) 'source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe'
$null=Read-RuntimeInput $init11Inspector 16777216
$init11Lines=& $init11Inspector hash @($init11Rows.Name)
if($LASTEXITCODE-ne 0){throw 'EP1 1.1 managed name-hash comparison failed.'}
$init11NameIds=[Collections.Generic.Dictionary[string,string]]::new([StringComparer]::Ordinal)
foreach($line in $init11Lines){if($line-cnotmatch '^(0x[0-9A-F]{8}) ([A-Za-z_][A-Za-z0-9_]*)$'){throw 'Unexpected managed name-hash output.'};$init11NameIds.Add($Matches[2],$Matches[1])}
if($init11NameIds.Count-ne 150){throw 'EP1 1.1 managed name count differs.'}
$init11Observed=[Collections.Generic.Dictionary[string,object]]::new([StringComparer]::Ordinal)
foreach($type in $evidence.Types){$init11Observed.Add($type.TypeName,$type)}
$init11Objects=Read-Ep11InitializerObjects $ep11Bytes $init11Rows $init11NameIds $init11Observed
if(@($init11Objects|Where-Object ObservedStockRoot).Count-ne 138){throw 'EP1 1.1 observed initializer root count differs.'}
if($SelfTest){
    # Reborn: reject detached opcode/source/store/call/vtable/alignment faults for the Texture long template and the ArmorTemplate short-call boundary.
    $sample=@($init11Rows|Where-Object Name -ceq Texture)[0];$offset=[Convert]::ToInt32($sample.Offset.Substring(2),16)
    foreach($case in @('opcode','source','store','call','vtable-object','vtable-range','alignment')){
        $fault=[byte[]]$ep11Bytes.Clone();$faultOffset=$offset
        switch($case){
            'opcode' {$fault[$offset]=0x90}
            'source' {[Array]::Clear($fault,$offset+1,4)}
            'store' {$fault[$offset+11]=$fault[$offset+11]-bxor 4}
            'call' {$fault[$offset+16]=$fault[$offset+16]-bxor 1}
            'vtable-object' {$fault[$offset+27]=$fault[$offset+27]-bxor 4}
            'vtable-range' {[Array]::Clear($fault,$offset+31,4)}
            'alignment' {$faultOffset++}
        }
        $rejected=$false;try{$null=Read-Ep11TypeInitializer $fault $faultOffset $true $init11Hashes}catch{$rejected=$true}
        if(-not $rejected){throw ('EP1 1.1 initializer fault admitted: '+$case)}
    }
    $plain=@($init11Rows|Where-Object Name -ceq ArmorTemplate)[0];$plainOffset=[Convert]::ToInt32($plain.Offset.Substring(2),16)
    $fault=[byte[]]$ep11Bytes.Clone();$fault[$plainOffset+25]=0x90;$rejected=$false
    try{$null=Read-Ep11TypeInitializer $fault $plainOffset $false $init11Hashes}catch{$rejected=$true}
    if(-not $rejected){throw 'EP1 1.1 short-template call fault admitted.'}
    # Reborn: separately reject static TypeId/raw-hash changes, while preserving an arbitrary opaque metadata word without Boolean/tokenized coercion.
    $objectOffset=[Convert]::ToInt32($sample.ObjectVa.Substring(2),16)-0x400000
    foreach($field in @(4,8)){
        $fault=[byte[]]$ep11Bytes.Clone();$fault[$objectOffset+$field]=$fault[$objectOffset+$field]-bxor 1;$rejected=$false
        try{$null=Read-Ep11InitializerObjects $fault $init11Rows $init11NameIds $init11Observed}catch{$rejected=$true}
        if(-not $rejected){throw 'EP1 1.1 metadata identity fault admitted.'}
    }
    $opaque=[byte[]]$ep11Bytes.Clone();[Array]::Copy([BitConverter]::GetBytes([Convert]::ToUInt32('C0DEFFFF',16)),0,$opaque,$objectOffset+12,4)
    $opaqueObjects=Read-Ep11InitializerObjects $opaque $init11Rows $init11NameIds $init11Observed
    if(@($opaqueObjects|Where-Object Name -ceq Texture)[0].UnknownWord12-cne '0xC0DEFFFF'){throw 'EP1 1.1 opaque metadata word was coerced.'}
}
$init11After=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($init11After))-cne $consumer11.ImageSha256){throw 'EP1 1.1 initializer image changed during review.'}
$init11Report=[pscustomobject]@{
    ImageSha256=$consumer11.ImageSha256;DecodedInitializers=$init11Rows.Count;PlainTemplateCount=43;VtableTemplateCount=107;RuntimeRowsNotCovered=1192
    MatchedMetadataNameTypeIds=$init11Objects.Count;ObservedMetadataRootTypes=138;UnknownWord12TokenizedMismatches=@($init11Objects|Where-Object {$_.ObservedStockRoot -and -not $_.UnknownWord12EqualsStockTokenized}).Count
    SampleInitializerToMetadataHashBindingRecovered=$true;RegistryConsumerBodiesPinned=$true;ObjectHashStoreOffset=8;UnknownWord12SemanticsProved=$false
    PathMusicEventInDecodedSample=(@($init11Rows|Where-Object Name -ceq PathMusicEvent).Count-ne 0)
    StartupReachabilityProved=$false;FullInitializerCoverageProved=$false;GeneralDisassembly=$false;CompleteStreamPointerProvenanceRecovered=$false
    Ep1ProcessingHashRecovered=$false;AllTypesHashDerivationRecovered=$false;ModPackageLoaded=$false;ProductionBuildReady=$false;ReadOnly=$true;TargetExecuted=$false;ManagedInspectorHashCommandExecuted=$true
    FaultTestsPassed=[bool]$SelfTest;RejectedMemoryFaultsExecuted=$(if($SelfTest){10}else{0});OpaquePreservationCasesExecuted=$(if($SelfTest){1}else{0});Rows=$init11Rows;Objects=$init11Objects
}
if($AsJson){ConvertTo-Json -InputObject $init11Report -Depth 8}else{$init11Report}
