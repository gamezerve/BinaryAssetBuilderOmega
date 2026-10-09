# Reborn: helper-only initial-break admission and live-base-mapped raw x86 CLR thunk evidence, not a game recipe.
. (Join-Path $PSScriptRoot 'Ra3Ep11DebuggerOwnershipPolicy.ps1')

#-------------------------------------------------------------------------------------------------
<# Reborn: require one complete initial wait record and one owner, with no continuation or unknown evidence records. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11InitialAdmissionOwner([string]$Text) {
    if($Text.Length -gt 65536){throw 'Initial admission text exceeds bound.'}
    $lines=@($Text -split '\r?\n')
    if(@($lines|Where-Object {$_ -ceq 'REBORN_ADMISSION_WAIT'}).Count -ne 1){throw 'Initial admission wait marker missing or ambiguous.'}
    $filtered=($lines|Where-Object {$_ -cne 'REBORN_ADMISSION_WAIT'}) -join "`n"
    $owners=@($lines|Where-Object {$_ -match '^REBORN_OWNER ([0-9a-f]+)$'})
    if($owners.Count -ne 1){throw 'Initial owner missing or ambiguous.'}
    if([array]::IndexOf($lines,$owners[0]) -gt [array]::IndexOf($lines,'REBORN_ADMISSION_WAIT')){throw 'Wait record precedes owner.'}
    $owner=[Convert]::ToInt32($owners[0].Substring(13),16)
    $null=Get-Ep11OwnedDebuggerEvidence $filtered $owner $false
    $owner
}

#-------------------------------------------------------------------------------------------------
<# Reborn: map the reviewed helper's entry RVA to its live address but require raw thunk bytes at this observed initial-break stage. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11InitialHelperEntry([byte[]]$Bytes,[long]$LiveBase) {
    if($Bytes.Length -lt 512 -or $Bytes.Length -gt 65536 -or $Bytes[0] -ne 0x4d -or $Bytes[1] -ne 0x5a -or $LiveBase -le 0 -or $LiveBase%4096 -ne 0){throw 'Helper image envelope differs.'}
    $pe=[BitConverter]::ToInt32($Bytes,60)
    if($pe -lt 64 -or $pe+248 -gt $Bytes.Length -or [BitConverter]::ToUInt32($Bytes,$pe) -ne 0x4550 -or [BitConverter]::ToUInt16($Bytes,$pe+4) -ne 0x14c -or [BitConverter]::ToUInt16($Bytes,$pe+24) -ne 0x10b){throw 'Helper PE32 header differs.'}
    $entry=[BitConverter]::ToUInt32($Bytes,$pe+40);$preferred=[BitConverter]::ToUInt32($Bytes,$pe+52);$size=[BitConverter]::ToUInt32($Bytes,$pe+80)
    $count=[BitConverter]::ToUInt16($Bytes,$pe+6);$optional=[BitConverter]::ToUInt16($Bytes,$pe+20);$sections=$pe+24+$optional
    if($count -lt 1 -or $count -gt 8 -or $optional -lt 96 -or $sections+40*$count -gt $Bytes.Length -or $entry+6 -gt $size -or $LiveBase+$size -gt 0x100000000L){throw 'Helper section/address envelope differs.'}
    $offsets=@()
    for($index=0;$index -lt $count;$index++){
        $section=$sections+40*$index;$rva=[BitConverter]::ToUInt32($Bytes,$section+12);$rawSize=[BitConverter]::ToUInt32($Bytes,$section+16);$raw=[BitConverter]::ToUInt32($Bytes,$section+20)
        if($entry -ge $rva -and [long]$entry+6 -le [long]$rva+$rawSize){$offsets+=([long]$raw+$entry-$rva)}
    }
    if($offsets.Count -ne 1 -or $offsets[0]+6 -gt $Bytes.Length){throw 'Helper entry raw mapping ambiguous.'}
    $expected=[byte[]]$Bytes[$offsets[0]..($offsets[0]+5)]
    if($expected[0] -ne 0xff -or $expected[1] -ne 0x25){throw 'Helper entry is not the reviewed absolute-indirect CLR thunk.'}
    $pointer=[BitConverter]::ToUInt32($expected,2)
    if($pointer -lt $preferred -or [long]$pointer+4 -gt [long]$preferred+$size){throw 'Helper thunk pointer outside image.'}
    # Reborn: actual helper controls showed raw thunk operands at initial break despite a relocated module base; never admit both byte variants.
    [pscustomobject]@{Address=$LiveBase+$entry;ExpectedBytes=$expected;PreferredBase=$preferred;LiveBase=$LiveBase;SizeOfImage=$size;EntryRva=$entry;BytePolicy='RawClrThunkAtInitialBreak-HelperOnly';GameRelocationValidated=$false}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: demand actual successful bounded transfer and equality, not only a requested read length. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11AdmissionBytes([bool]$Success,[long]$Actual,[byte[]]$Expected,[byte[]]$Observed) {
    if(-not $Success -or $Expected.Length -lt 1 -or $Expected.Length -gt 64 -or $Actual -ne $Expected.Length -or $Observed.Length -ne $Expected.Length){throw 'Initial live byte transfer incomplete.'}
    for($index=0;$index -lt $Expected.Length;$index++){if($Expected[$index] -ne $Observed[$index]){throw 'Initial live byte mismatch.'}}
}
