# Reborn: fixed-base native image admission and bounded immutable-range comparison, never a loader or debugger.

#-------------------------------------------------------------------------------------------------
<# Reborn: this pinned native build has stripped relocations; refuse nonpreferred bases instead of applying helper CLR assumptions. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11NativeLoadPolicy($Header,[string]$Machine,[bool]$Managed,[bool]$RelocsStripped,[long]$LiveBase) {
    if($Machine -cne 'I386' -or $Managed -or $Header.Magic.ToString() -cne 'PE32' -or -not $RelocsStripped -or
       $Header.ImageBase -ne 0x400000 -or $LiveBase -ne 0x400000 -or $Header.DllCharacteristics -ne 0 -or
       $Header.BaseRelocationTableDirectory.RelativeVirtualAddress -ne 0 -or $Header.BaseRelocationTableDirectory.Size -ne 0 -or
       $Header.SizeOfImage -ne 13754368){throw 'Pinned native fixed-base load policy differs.'}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: map only small uniquely raw-backed immutable code/table ranges, refusing loader-mutated IAT and writable sections. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11NativeRawRange($Pe,[byte[]]$Bytes,[int]$Rva,[int]$Length) {
    if($Rva -le 0 -or $Length -lt 1 -or $Length -gt 4096 -or [long]$Rva+$Length -gt $Pe.PEHeaders.PEHeader.SizeOfImage){throw 'Native admission range outside bounds.'}
    $sections=@($Pe.PEHeaders.SectionHeaders|Where-Object {$Rva -ge $_.VirtualAddress -and [long]$Rva+$Length -le [long]$_.VirtualAddress+[Math]::Min($_.VirtualSize,$_.SizeOfRawData)})
    if($sections.Count -ne 1 -or ($sections[0].SectionCharacteristics -band [Reflection.PortableExecutable.SectionCharacteristics]::MemWrite) -ne 0){throw 'Native admission section ambiguous or writable.'}
    $iat=$Pe.PEHeaders.PEHeader.ImportAddressTableDirectory
    if($iat.Size -gt 0 -and $Rva -lt [long]$iat.RelativeVirtualAddress+$iat.Size -and [long]$Rva+$Length -gt $iat.RelativeVirtualAddress){throw 'Loader-mutated IAT range cannot be a raw-byte pin.'}
    $raw=[long]$sections[0].PointerToRawData+$Rva-$sections[0].VirtualAddress
    if($raw -lt 0 -or $raw+$Length -gt $Bytes.Length){throw 'Native admission raw bytes truncated.'}
    [pscustomobject]@{Raw=$raw;Section=$sections[0].Name;Bytes=[byte[]]$Bytes[$raw..($raw+$Length-1)]}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: compare every trusted in-process plan range with exact successful transfers; detached fixtures never imply live proof. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11NativeAdmissionObservations($Plan,[object[]]$Observations,[long]$LiveBase) {
    if($LiveBase -ne 0x400000 -or $Plan.PreferredBase -ne 0x400000 -or $Observations.Count -ne $Plan.Ranges.Count){throw 'Native observation base/count differs.'}
    $seen=@{}
    foreach($observation in $Observations){
        $range=@($Plan.Ranges|Where-Object {$_.Name -ceq $observation.Name})
        if($range.Count -ne 1 -or $seen.ContainsKey($observation.Name)){throw 'Unknown or duplicate native range observation.'}
        $seen[$observation.Name]=$true
        if($observation.Succeeded -isnot [bool] -or -not $observation.Succeeded -or $observation.Address -ne $range[0].Address -or
           $observation.ActualCount -ne $range[0].Length -or $observation.Bytes -isnot [byte[]] -or $observation.Bytes.Length -ne $range[0].Length){throw 'Native range transfer incomplete or misaddressed.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($observation.Bytes)) -cne $range[0].ExpectedSha256){throw 'Native live range differs from reviewed disk bytes.'}
    }
}
