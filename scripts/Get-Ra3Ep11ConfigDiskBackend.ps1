# Reborn: bind a concrete disk-provider open/read route and Windows imports without executing the game, a DLL or a debugger.
[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$ImagePath,[switch]$SelfTest,[switch]$AsJson)
$ErrorActionPreference='Stop'
$diskSelfTest=$SelfTest;$diskAsJson=$AsJson
$route=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11ConfigReadRoute.ps1') -ImagePath $ImagePath
$SelfTest=$diskSelfTest;$AsJson=$diskAsJson
$diskPins=@(
    [pscustomobject]@{Raw=0xd6af0;Length=351;Hash='EA67AE263FF27DB818FB8196A03C274C4490D6D46273893A149086D5AA8FF9DB'},
    [pscustomobject]@{Raw=0x56c330;Length=171;Hash='D7F8B8986A07EE0FF4B23838C7FA3493A0C19A9DA207C66ABF97D11C8AC66A6E'},
    [pscustomobject]@{Raw=0x87f200;Length=64;Hash='F50B31FE03A94D5B1C4E9D7CDF376C8C0CA82422EDFF22EC0DD48A6DAE335987'},
    [pscustomobject]@{Raw=0x56c1d0;Length=338;Hash='79137B85BE3E7159914002D96FFE15CB5160D03DCC850C787D0542F45BA82008'},
    [pscustomobject]@{Raw=0x56bbf0;Length=46;Hash='47F21CAA858C815C455C24F12B0933EDA40D0865DE2058F8E4ED95146986CBB0'}
)
#-------------------------------------------------------------------------------------------------
<# Reborn: verify reviewed source-open, drive registration, concrete backend vtable and full backend open/read bodies in private snapshots. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11DiskBackend([byte[]]$Bytes) {
    foreach($pin in $diskPins){
        if($pin.Raw+$pin.Length-gt $Bytes.Length){throw 'Disk-provider bytes truncated.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$pin.Raw..($pin.Raw+$pin.Length-1)]))-cne $pin.Hash){throw 'Disk-provider evidence differs.'}
    }
    if([BitConverter]::ToUInt32($Bytes,0x87f200+0xc)-ne 0x96c1d0 -or [BitConverter]::ToUInt32($Bytes,0x87f200+0x14)-ne 0x96bbf0){throw 'Concrete disk-provider vtable differs.'}
}
Assert-Ep11DiskBackend $modBytes
$memory=[IO.MemoryStream]::new($modBytes,$false);$pe=[System.Reflection.PortableExecutable.PEReader]::new($memory)
try{
    # Reborn: verify the DLL descriptor, original name thunks and file IAT bindings rather than treating an API string occurrence as a call.
    $directory=$pe.PEHeaders.PEHeader.ImportTableDirectory;$found=$false
    for($index=0;$index-lt 128;$index++){
        if(20*$index+20-gt $directory.Size){throw 'Import descriptor range truncated.'}
        $descriptor=Read-PeRange $pe ($directory.RelativeVirtualAddress+20*$index) 20
        if(@($descriptor|Where-Object{$_-ne 0}).Count-eq 0){break}
        if((Read-PeName $pe ([BitConverter]::ToUInt32($descriptor,12)))-ine 'KERNEL32.dll'){continue}
        if($found){throw 'Duplicate kernel import descriptor.'};$found=$true
        $original=[BitConverter]::ToUInt32($descriptor,0);$iat=[BitConverter]::ToUInt32($descriptor,16)
        if($original-eq 0 -or $iat-ne 0x7d8088){throw 'Reviewed kernel thunk base differs.'}
        foreach($binding in @(
            [pscustomobject]@{Rva=0x7d81bc;Name='ReadFile'},
            [pscustomobject]@{Rva=0x7d82b0;Name='CreateFileW'},
            [pscustomobject]@{Rva=0x7d8230;Name='MultiByteToWideChar'}
        )){
            $offset=$binding.Rva-$iat
            if($offset-lt 0 -or $offset-gt 4092 -or $offset%4-ne 0){throw 'Kernel slot outside budget.'}
            $nameRva=[BitConverter]::ToUInt32((Read-PeRange $pe ($original+$offset) 4),0)
            if(($nameRva-band 0x80000000)-ne 0 -or (Read-PeName $pe ($nameRva+2))-cne $binding.Name -or
               [BitConverter]::ToUInt32((Read-PeRange $pe $binding.Rva 4),0)-ne $nameRva){throw 'Reviewed kernel API binding differs.'}
        }
    }
    if(-not $found){throw 'Kernel import descriptor unavailable.'}
}finally{$pe.Dispose();$memory.Dispose()}
if($SelfTest){
    # Reborn: fault-inject only private instruction/table copies; this is neither a native file-open trial nor import hooking.
    foreach($raw in @($diskPins.Raw)+@((0x87f200+0xc),(0x87f200+0x14))){
        $fault=[byte[]]$modBytes.Clone();$fault[$raw]=$fault[$raw]-bxor 1;$refused=$false
        try{Assert-Ep11DiskBackend $fault}catch{$refused=$true};if(-not $refused){throw 'Disk-provider mutation admitted.'}
    }
}
# Reborn: preserve the static/live boundary and recheck the complete image identity before returning proposed observation sites.
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData((Read-PeSnapshot $ImagePath)))-cne $route.ImageSha256){throw 'Disk-provider image changed during analysis.'}
$report=[pscustomobject]@{ImageSha256=$route.ImageSha256;ReviewedPins=5;DriveRegistrationVa='0x0096C330';DiskVtableVa='0x00C7F200';DiskOpenVa='0x0096C1D0';DiskReadVa='0x0096BBF0';CreateFileWIatVa='0x00BD82B0';ReadFileIatVa='0x00BD81BC';ReadFileCallVa='0x0096BC09';ReadFileReturnVa='0x0096BC0F'
    IncomingConfigFlags='0x401';MappedBackendFlags='0x20';ScopedDesiredAccess='0x80000000';ScopedShareMode=1;ScopedCreationDisposition=3;ScopedFileAttributes='0x80';ReadOverlappedPointerIsNull=$true;ReadFileBooleanCheckedByWrapper=$false;ReadCountStorageAliasesIncomingLength=$true
    LiveDiskProviderSelectedProven=$false;WindowsReadSucceededProven=$false;ExactProbeConsumedProven=$false;DebuggerRecipeReady=$false;TargetExecuted=$false;ProductionBuildReady=$false
    PrivateMutationRefusals=$(if($SelfTest){7}else{0})}
if($AsJson){$report|ConvertTo-Json -Depth 4}else{$report}
