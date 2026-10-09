# Reborn: verify the post-config basename identity copy and local CRT crash-offset correspondence without executing either image.
[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$ImagePath,[switch]$SelfTest,[switch]$AsJson)
$ErrorActionPreference='Stop'
$identitySelfTest=$SelfTest;$identityAsJson=$AsJson
$null=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11ConfigConsumer.ps1') -ImagePath $ImagePath
$SelfTest=$identitySelfTest;$AsJson=$identityAsJson
. (Join-Path $PSScriptRoot 'Ra3Ep11ConfigNamePolicy.ps1')
$slices=@(
    [pscustomobject]@{Offset=0xd98ab;Length=310;Hash='04B0A870215320721B84D219DFD5AC2A409E25410FD894376DCD8CB1532973B5'},
    [pscustomobject]@{Offset=0xd0f10;Length=22;Hash='92156331A7F8DFE797C9404D33A03DFE9B81BCB20DEF669D83BB8D16466D3470'},
    [pscustomobject]@{Offset=0xd5820;Length=62;Hash='9B62DF8D4ECD119854A68FC3B18045C29933AF2ADBCBE7B192104531138B3948'}
)
#-------------------------------------------------------------------------------------------------
<# Reborn: admit only reviewed complete basename/helper bodies and post-reader startup bytes; mutation tests affect private copies only. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11ConfigIdentityCode([byte[]]$Bytes) {
    foreach($slice in $slices){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Config identity bytes truncated.'}
        $part=[byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($part))-cne $slice.Hash){throw 'Config identity evidence differs.'}
    }
}
Assert-Ep11ConfigIdentityCode $modBytes
#-------------------------------------------------------------------------------------------------
<# Reborn: read only budgeted export-table bytes in existing 4-KiB checked PE chunks; do not weaken the shared range guard. #>
#-------------------------------------------------------------------------------------------------
function Read-Ep11CrtExportTable($Reader,[uint32]$Rva,[int]$Count) {
    if($Count-lt 1 -or $Count-gt 20000){throw 'CRT table size exceeds budget.'}
    $buffer=[byte[]]::new($Count)
    for($offset=0;$offset-lt $Count;$offset+=4096){
        $part=Read-PeRange $Reader ($Rva+$offset) ([Math]::Min(4096,$Count-$offset))
        [Array]::Copy($part,0,$buffer,$offset,$part.Length)
    }
    return ,$buffer
}
$crtPath='C:\WINDOWS\WinSxS\x86_microsoft.vc80.crt_1fc8b3b9a1e18e3b_8.0.50727.9680_none_d090cb7c44278b28\MSVCR80.dll'
$crtBytes=Read-PeSnapshot $crtPath
$crtHash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($crtBytes))
if($crtHash-cne 'A74E18C475E7853D6158AE668DCC3F6C30C7B9EB7BB2360A8B58D945C1E81FBB'){throw 'Local CRT identity differs; fault offset correspondence refused.'}
$memory=[IO.MemoryStream]::new($crtBytes,$false);$pe=[System.Reflection.PortableExecutable.PEReader]::new($memory)
try{
    # Reborn: resolve the actual export name through bounded name/ordinal/function tables, not a nearest-symbol guess.
    $exports=Read-PeRange $pe $pe.PEHeaders.PEHeader.ExportTableDirectory.RelativeVirtualAddress 40
    $functions=[BitConverter]::ToUInt32($exports,20);$names=[BitConverter]::ToUInt32($exports,24)
    if($functions-eq 0 -or $functions-gt 5000 -or $names-eq 0 -or $names-gt 5000){throw 'CRT export budget differs.'}
    $functionTable=Read-Ep11CrtExportTable $pe ([BitConverter]::ToUInt32($exports,28)) (4*$functions)
    $nameTable=Read-Ep11CrtExportTable $pe ([BitConverter]::ToUInt32($exports,32)) (4*$names)
    $ordinalTable=Read-Ep11CrtExportTable $pe ([BitConverter]::ToUInt32($exports,36)) (2*$names)
    $hits=@(for($i=0;$i-lt $names;$i++){
        if((Read-PeName $pe ([BitConverter]::ToUInt32($nameTable,4*$i)))-cne 'strcpy_s'){continue}
        $ordinal=[BitConverter]::ToUInt16($ordinalTable,2*$i)
        if($ordinal-ge $functions){throw 'CRT export ordinal outside table.'}
        [BitConverter]::ToUInt32($functionTable,4*$ordinal)
    })
    if($hits.Count-ne 1 -or $hits[0]-ne 0x1455b){throw 'CRT strcpy_s export binding differs.'}
    $crtCode=Read-PeRange $pe 0x1455b 101
    if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($crtCode))-cne '577D2A4DE2E618A441A81F6BB116453520B4A406B310E4B8EF5D3F417DA9A05F'){throw 'CRT strcpy_s body differs.'}
}finally{$pe.Dispose();$memory.Dispose()}
if($SelfTest){
    # Reborn: exercise identity boundaries and historical failure independently of the native parser, then corrupt each private instruction slice.
    foreach($case in @(
        [pscustomobject]@{Name='config-read-only.cfg';Fits=$false},
        [pscustomobject]@{Name='probe_1.0.cfg';Fits=$true},
        [pscustomobject]@{Name=('a'*15);Fits=$true},
        [pscustomobject]@{Name=('a'*16);Fits=$false}
    )){if((Get-Ep11ConfigNameProfile $case.Name).CopyFits-ne $case.Fits){throw 'Detached identity boundary differs.'}}
    foreach($slice in $slices){$fault=[byte[]]$modBytes.Clone();$fault[$slice.Offset]=$fault[$slice.Offset]-bxor 1;$refused=$false;try{Assert-Ep11ConfigIdentityCode $fault}catch{$refused=$true};if(-not $refused){throw 'Identity code fault admitted.'}}
}
# Reborn: distinguish a reproducible static overflow candidate from the still-unobserved runtime caller and config-read result.
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData((Read-PeSnapshot $ImagePath)))-cne $modHash -or
   [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData((Read-PeSnapshot $crtPath)))-cne $crtHash){throw 'Evidence image changed during analysis.'}
$report=[pscustomobject]@{ImageSha256=$modHash;PostReaderSliceVa='0x004D98AB';BaseNameHelperVa='0x004D5820';IdentitySetterVa='0x004D0F10';IdentitySetterCallVa='0x004D98FD';IdentityDestinationVa='0x00CC4B6C';IdentityDestinationBytes=16
    HistoricalProbe=Get-Ep11ConfigNameProfile 'config-read-only.cfg';ReplacementProbe=Get-Ep11ConfigNameProfile 'probe_1.0.cfg'
    CrtPath=$crtPath;CrtSha256=$crtHash;StrcpySExportRva='0x0001455B';RecordedFaultRva='0x00014584';FaultOffsetFromExport=41
    ExactRuntimeCallerProven=$false;ConfigReadProven=$false;CrashCauseProven=$false;TargetExecuted=$false;ProductionBuildReady=$false
    DetachedNameCases=$(if($SelfTest){4}else{0});PrivateCodeFaultRefusals=$(if($SelfTest){3}else{0})}
if($AsJson){$report|ConvertTo-Json -Depth 5}else{$report}
