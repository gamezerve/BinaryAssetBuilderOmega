# Reborn: derive native fixed-base byte admission from the exact reviewed 1.1 image without querying or launching a process.
[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$ImagePath,[switch]$SelfTest,[switch]$AsJson)
$ErrorActionPreference='Stop'
$nativeSelfTest=$SelfTest;$nativeAsJson=$AsJson
$null=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11ConfigDiskBackend.ps1') -ImagePath $ImagePath
. (Join-Path $PSScriptRoot 'Ra3Ep11NativeAdmissionPolicy.ps1')
$SelfTest=$nativeSelfTest;$AsJson=$nativeAsJson
$memory=[IO.MemoryStream]::new($modBytes,$false);$pe=[Reflection.PortableExecutable.PEReader]::new($memory)
try{
    $header=$pe.PEHeaders.PEHeader
    $stripped=($pe.PEHeaders.CoffHeader.Characteristics -band [Reflection.PortableExecutable.Characteristics]::RelocsStripped) -ne 0
    Assert-Ep11NativeLoadPolicy $header $pe.PEHeaders.CoffHeader.Machine.ToString() $pe.HasMetadata $stripped 0x400000
    # Reborn: reuse complete previously reviewed code/table pins, adding the config reader/gate/splitter and source vtable slots.
    $pins=@($routePins)+@($diskPins)+@(
        [pscustomobject]@{Raw=0xd86b0;Length=169;Hash='0EE0C54E6C53825FCFF4A43DE01C0B30A7C0ACF3BBF1FA888764C8B118742FFE'},
        [pscustomobject]@{Raw=0xd9040;Length=287;Hash='39D04C96658A634EA0A5D965A2B4EB1B0B5ED68CDEE041A1314C51F9D8192EEE'},
        [pscustomobject]@{Raw=0xd985e;Length=77;Hash='870C2332C1463EAAE55E0185E0F018FE9B454163CFA4464E1793D08FEC4CD899'},
        [pscustomobject]@{Raw=0xd69a0;Length=209;Hash='C97162AB160538037FCE33BBBA756BBC583BCC86B950DE300010068D47E017E7'}
    )
    $ranges=[Collections.Generic.List[object]]::new()
    foreach($pin in $pins){
        # Reborn: recover RVA through section mapping even when this image's reviewed text/table raw offsets happen to equal RVAs.
        $sections=@($pe.PEHeaders.SectionHeaders|Where-Object {$pin.Raw -ge $_.PointerToRawData -and [long]$pin.Raw+$pin.Length -le [long]$_.PointerToRawData+$_.SizeOfRawData})
        if($sections.Count -ne 1){throw 'Reviewed pin raw section ambiguous.'}
        $rva=$sections[0].VirtualAddress+$pin.Raw-$sections[0].PointerToRawData
        $mapped=Get-Ep11NativeRawRange $pe $modBytes $rva $pin.Length
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($mapped.Bytes)) -cne $pin.Hash){throw 'Mapped reviewed pin differs.'}
        $ranges.Add([pscustomobject]@{Name=('PinRva{0:X8}' -f $rva);Rva=$rva;Address=0x400000L+$rva;Length=$pin.Length;Section=$mapped.Section;ExpectedSha256=$pin.Hash})
    }
    foreach($rva in @(0x7fa27c,0x7fa2a8)){
        # Reborn: source reader/conversion slots are bound by the inherited route audit and the complete admitted image SHA.
        $mapped=Get-Ep11NativeRawRange $pe $modBytes $rva 4
        $ranges.Add([pscustomobject]@{Name=('SourceSlotRva{0:X8}' -f $rva);Rva=$rva;Address=0x400000L+$rva;Length=4;Section=$mapped.Section;ExpectedSha256=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($mapped.Bytes))})
    }
    $plan=[pscustomobject]@{Kind='NativeEp11';ImageSha256=$modHash;PreferredBase=0x400000L;SizeOfImage=$header.SizeOfImage;RelocationsStripped=$stripped;RelocationDirectoryRva=0;RelocationDirectoryBytes=0;DynamicBase=$false;NativeImage=$true;RequireExactPreferredBase=$true;Ranges=$ranges.ToArray();IatRawByteComparisonAllowed=$false;OwnershipValidated=$false;LiveBytesValidated=$false;TargetExecuted=$false;DebuggerRecipeReady=$false;GameRecipeReady=$false;ProductionBuildReady=$false;DetachedObservationMatches=0;PrivateRefusals=0}
    if($SelfTest){
        # Reborn: observations are private disk snapshots, never ReadProcessMemory output or game execution evidence.
        $observations=@($plan.Ranges|ForEach-Object {$mapped=Get-Ep11NativeRawRange $pe $modBytes $_.Rva $_.Length;[pscustomobject]@{Name=$_.Name;Address=$_.Address;Succeeded=$true;ActualCount=$_.Length;Bytes=$mapped.Bytes}})
        Assert-Ep11NativeAdmissionObservations $plan $observations 0x400000
        $refusals=0
        foreach($mode in @('Byte','Count','Status','Address','Duplicate','Missing','Unknown','Base','BufferLength','NonBooleanStatus')){
            $fault=@($observations|ForEach-Object {[pscustomobject]@{Name=$_.Name;Address=$_.Address;Succeeded=$_.Succeeded;ActualCount=$_.ActualCount;Bytes=[byte[]]$_.Bytes.Clone()}});$base=0x400000L
            switch($mode){
                'Byte' {$fault[0].Bytes[0]=$fault[0].Bytes[0] -bxor 1}
                'Count' {$fault[0].ActualCount--}
                'Status' {$fault[0].Succeeded=$false}
                'Address' {$fault[0].Address++}
                'Duplicate' {$fault[1]=$fault[0]}
                'Missing' {$fault=@($fault|Select-Object -Skip 1)}
                'Unknown' {$fault[0].Name='Other'}
                'Base' {$base=0x500000}
                'BufferLength' {$fault[0].Bytes=[byte[]]$fault[0].Bytes[1..($fault[0].Bytes.Length-1)]}
                'NonBooleanStatus' {$fault[0].Succeeded='true'}
            }
            $refused=$false;try{Assert-Ep11NativeAdmissionObservations $plan $fault $base}catch{$refused=$true};if(-not $refused){throw 'Native observation fault admitted.'};$refusals++
        }
        foreach($fault in @(@(0x7d8000,4),@(0x8c3000,4),@(0,4),@(0xd86b0,4097),@(0xd86b0,0))){
            $refused=$false;try{$null=Get-Ep11NativeRawRange $pe $modBytes $fault[0] $fault[1]}catch{$refused=$true};if(-not $refused){throw 'Mutable or invalid native range admitted.'};$refusals++
        }
        foreach($base in @(0L,0x500000L,0x400001L)){
            $refused=$false;try{Assert-Ep11NativeLoadPolicy $header 'I386' $false $true $base}catch{$refused=$true};if(-not $refused){throw 'Unknown native base admitted.'};$refusals++
        }
        # Reborn: mutate private profile metadata to exercise policy fields independently of the complete disk-SHA guard.
        foreach($mode in @('RelocRva','RelocSize','DynamicBase','Managed','NotStripped','PE32Plus','Machine','ImageSize')){
            $fault=[pscustomobject]@{Magic='PE32';ImageBase=0x400000L;DllCharacteristics=0;BaseRelocationTableDirectory=[pscustomobject]@{RelativeVirtualAddress=0;Size=0};SizeOfImage=13754368}
            $managed=$false;$strippedFixture=$true;$machine='I386'
            switch($mode){
                'RelocRva' {$fault.BaseRelocationTableDirectory.RelativeVirtualAddress=0x1000}
                'RelocSize' {$fault.BaseRelocationTableDirectory.Size=8}
                'DynamicBase' {$fault.DllCharacteristics=0x40}
                'Managed' {$managed=$true}
                'NotStripped' {$strippedFixture=$false}
                'PE32Plus' {$fault.Magic='PE32Plus'}
                'Machine' {$machine='Amd64'}
                'ImageSize' {$fault.SizeOfImage++}
            }
            $refused=$false;try{Assert-Ep11NativeLoadPolicy $fault $machine $managed $strippedFixture 0x400000}catch{$refused=$true};if(-not $refused){throw 'Unknown native profile admitted.'};$refusals++
        }
        $plan.DetachedObservationMatches=$observations.Count;$plan.PrivateRefusals=$refusals
    }
}finally{$pe.Dispose();$memory.Dispose()}
# Reborn: recheck the complete pinned disk image, preserving the boundary between this plan and live admission.
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData((Read-PeSnapshot $ImagePath))) -cne $modHash){throw 'Native admission source changed.'}
if($AsJson){$plan|ConvertTo-Json -Depth 6}else{$plan}
