# Reborn: pin limited runtime registry/lookup bodies and static object identities without executing target code or assigning unknown metadata flags compiler meanings.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$registrySelfTest=$SelfTest; $registryAsJson=$AsJson
$initializers=. (Join-Path $PSScriptRoot 'Get-Ra3Ep1TypeInitializers.ps1') -ImagePath $ImagePath
$SelfTest=$registrySelfTest; $AsJson=$registryAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: require exact reviewed lookup and registration bodies before applying the bounded static field/control-flow interpretation. #>
#-------------------------------------------------------------------------------------------------
function Assert-RegistryBodies([byte[]] $Bytes) {
    foreach($body in @(
        [pscustomobject]@{Offset=0x17340;Length=34;Hash='DBEE0BF378AECD49BCF8D0DFE9418BF39C6E61760B37C1F22A71D20547B32A3E'},
        [pscustomobject]@{Offset=0x17400;Length=211;Hash='F6347C4BE68B782CF6889175AAD7EFA3FD06E918A2431BDB6746C2699A77E9F1'}
    )) {
        if($body.Offset+$body.Length-gt $Bytes.Length){throw 'Registry body outside image.'}
        $hash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$body.Offset..($body.Offset+$body.Length-1)]))
        if($hash-cne $body.Hash){throw 'Reviewed registry body changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: correlate raw object TypeId fields with the existing managed FastHash provider; preserve the unknown +12 word without guessing tokenization. #>
#-------------------------------------------------------------------------------------------------
function Read-RegistryObjects([byte[]] $Bytes,$Initializers,$NameIds,$Observed) {
    return @(foreach($row in $Initializers.Rows){
        $offset=[Convert]::ToInt32($row.ObjectVa.Substring(2),16)-0x400000
        if($offset-lt 0x8bc000 -or $offset+16-gt 0x8e9000 -or $offset%4-ne 0){throw 'Metadata object outside reviewed raw .data bounds.'}
        $id='0x{0:X8}' -f [BitConverter]::ToUInt32($Bytes,$offset+4)
        $rawHash=[BitConverter]::ToUInt32($Bytes,$offset+8)
        if(-not $NameIds.ContainsKey($row.Name) -or $id-cne $NameIds[$row.Name] -or $rawHash-ne 0){throw 'Static metadata TypeId/zero-before-initialization relationship differs.'}
        $flag=[BitConverter]::ToUInt32($Bytes,$offset+12)
        $stock=$Observed.ContainsKey($row.Name)
        if($stock -and $Observed[$row.Name].TypeId-cne $id){throw 'Object TypeId disagrees with stock manifest.'}
        [pscustomobject]@{Name=$row.Name;ObjectVa=$row.ObjectVa;RawTypeId=$id;RawHashBeforeInitialization='0x00000000';InitializerTypeHash=$row.TypeHash;UnknownWord12=('0x{0:X8}' -f $flag);ObservedStockRoot=$stock;StockTokenized=$(if($stock){$Observed[$row.Name].Fingerprints[0].Tokenized}else{$null});UnknownWord12EqualsStockTokenized=$(if($stock){$flag-eq $Observed[$row.Name].Fingerprints[0].Tokenized}else{$null})}
    })
}
Assert-RegistryBodies $bytes
# Reborn: only the already built repo-owned managed inspector computes name hashes; the inspected game/codec binaries are never invoked.
$inspector=Join-Path (Split-Path -Parent $PSScriptRoot) 'source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe'
$null=Read-RuntimeInput $inspector 16777216
$hashLines=& $inspector hash @($initializers.Rows.Name)
if($LASTEXITCODE-ne 0){throw 'Managed name hash comparison failed.'}
$nameIds=[Collections.Generic.Dictionary[string,string]]::new([StringComparer]::Ordinal)
foreach($line in $hashLines){if($line-cnotmatch '^(0x[0-9A-F]{8}) ([A-Za-z_][A-Za-z0-9_]*)$'){throw 'Unexpected managed hash output.'};$nameIds.Add($Matches[2],$Matches[1])}
if($nameIds.Count-ne 150){throw 'Managed name hash count differs.'}
$stockBytes=Read-RuntimeInput (Join-Path (Split-Path -Parent $PSScriptRoot) 'docs\RA3EP1_TYPE_TABLE_EVIDENCE.json') 2097152
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($stockBytes))-cne $runtime.EvidenceSha256){throw 'Registry stock evidence changed.'}
$observed=[Collections.Generic.Dictionary[string,object]]::new([StringComparer]::Ordinal)
foreach($type in ([Text.Encoding]::UTF8.GetString($stockBytes)|ConvertFrom-Json).Types){$observed.Add($type.TypeName,$type)}
$objects=Read-RegistryObjects $bytes $initializers $nameIds $observed
if($SelfTest){
    # Reborn: test exact body and object-field refusal with detached arrays, never modifications to the game or public artifact pins.
    foreach($case in @('lookup','registration','typeid','rawhash')){
        $fault=[byte[]]$bytes.Clone()
        switch($case){
            'lookup'{$fault[0x17350]=$fault[0x17350]-bxor 1}
            'registration'{$fault[0x174c3]=$fault[0x174c3]-bxor 1}
            'typeid'{$fault[0x8be84c]=$fault[0x8be84c]-bxor 1}
            'rawhash'{$fault[0x8be850]=1}
        }
        $rejected=$false;try{Assert-RegistryBodies $fault;$null=Read-RegistryObjects $fault $initializers $nameIds $observed}catch{$rejected=$true}
        if(-not $rejected){throw ('Registry fault admitted: '+$case)}
    }
    # Reborn: an unknown word must remain opaque even when changed; no Boolean/tokenized coercion or unsupported semantic rejection is allowed.
    $opaque=[byte[]]$bytes.Clone()
    [Array]::Copy([BitConverter]::GetBytes([Convert]::ToUInt32('C0DEFFFF',16)),0,$opaque,0x8be854,4)
    $opaqueRows=Read-RegistryObjects $opaque $initializers $nameIds $observed
    $opaqueTexture=@($opaqueRows|Where-Object Name -ceq Texture)[0]
    if($opaqueTexture.UnknownWord12-cne '0xC0DEFFFF' -or $opaqueTexture.UnknownWord12EqualsStockTokenized){throw 'Opaque metadata word was reinterpreted.'}
}
$after=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($after))-cne $initializers.ImageSha256){throw 'Registry image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$initializers.ImageSha256; RegistrationVa='0x00417400'; RegistrationBytes=211; RegistrationSha256='F6347C4BE68B782CF6889175AAD7EFA3FD06E918A2431BDB6746C2699A77E9F1'
    LookupVa='0x00417340'; LookupBytes=34; LookupSha256='DBEE0BF378AECD49BCF8D0DFE9418BF39C6E61760B37C1F22A71D20547B32A3E'
    ListHeadVa='0x00CEA428'; NodeAllocationBytes=16; AllocationCallVa='0x004168F0'; NodeNextOffset=0; NodeTypeIdOffset=4; NodeRepeatedTypeIdOffset=8; NodeObjectOffset=12
    ObjectTypeIdOffset=4; InitializerHashStoreOffset=8; MatchedObjectNameTypeIds=$objects.Count
    ObservedObjectRootTypes=@($objects|Where-Object ObservedStockRoot).Count
    UnknownWord12TokenizedMismatches=@($objects|Where-Object {$_.ObservedStockRoot -and -not $_.UnknownWord12EqualsStockTokenized}).Count
    LookupUsesTypeId=$true; LookupConsumesTypeHash=$false; UnknownWord12SemanticsProved=$false; StartupReachabilityProved=$false; FullRegistryCoverageProved=$false
    ReadOnly=$true; TargetExecuted=$false; ManagedInspectorHashCommandExecuted=$true; Ep1ProcessingHashRecovered=$false; ProductionBuildReady=$false; FaultTestsPassed=[bool]$SelfTest
    Objects=$objects
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 8}else{$report}
