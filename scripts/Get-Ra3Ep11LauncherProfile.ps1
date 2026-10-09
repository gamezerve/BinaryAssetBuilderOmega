# Reborn: pin launcher PE/literals/imports while leaving protected-code interpretation, executable selection and argument forwarding unproved.
[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$LauncherPath,[switch]$SelfTest,[switch]$AsJson)
$ErrorActionPreference='Stop'
$launcherSelfTest=$SelfTest;$launcherAsJson=$AsJson
$launcherMetadata=@(. (Join-Path $PSScriptRoot 'Get-Ra3Ep1PeEvidence.ps1') -ImagePaths @($LauncherPath))[0]
$SelfTest=$launcherSelfTest;$AsJson=$launcherAsJson
$launcherBytes=Read-PeSnapshot $LauncherPath
$launcherHash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($launcherBytes))
if($launcherHash-cne '07694EBBCF21232B1A1B401C07ABC2CFB1EDB9F50FA8D0C36DA0F94820943A4A' -or $launcherMetadata.Sha256-cne $launcherHash){throw 'Unreviewed launcher identity; fixed literal offsets are not admitted.'}
$launcherLiterals=@(
    [pscustomobject]@{Raw=0x92b74;Encoding='ASCII';Text='set-exe '},
    [pscustomobject]@{Raw=0x92c34;Encoding='UTF16LE';Text='.SkuDef'},
    [pscustomobject]@{Raw=0x92c44;Encoding='UTF16LE';Text='-modconfig "'},
    [pscustomobject]@{Raw=0x92c60;Encoding='UTF16LE';Text='-config "'},
    [pscustomobject]@{Raw=0x92c74;Encoding='UTF16LE';Text='_*.SkuDef'}
)

#-------------------------------------------------------------------------------------------------
<# Reborn: verify exact terminated launch-related literals without treating their existence as executed command construction. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11LauncherLiterals([byte[]]$Bytes) {
    foreach($literal in $launcherLiterals){
        $encoding=if($literal.Encoding-ceq 'ASCII'){[Text.Encoding]::ASCII}else{[Text.Encoding]::Unicode}
        $expected=$encoding.GetBytes($literal.Text+[char]0)
        if($literal.Raw-lt 0 -or $literal.Raw+$expected.Length-gt $Bytes.Length){throw 'Launcher literal outside snapshot.'}
        for($i=0;$i-lt $expected.Length;$i++){if($Bytes[$literal.Raw+$i]-ne $expected[$i]){throw 'Launcher literal pin differs.'}}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: map file-backed PE32 RVAs only through one checked section, never by a guessed global raw/VA delta. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11LauncherRaw($Pe,[uint32]$Rva) {
    $matches=@($Pe.PEHeaders.SectionHeaders|Where-Object{$Rva-ge $_.VirtualAddress -and [uint64]$Rva-lt [uint64]$_.VirtualAddress+$_.SizeOfRawData})
    if($matches.Count-ne 1){throw 'Launcher RVA lacks a unique file-backed section.'}
    return [long]$matches[0].PointerToRawData+$Rva-$matches[0].VirtualAddress
}
Assert-Ep11LauncherLiterals $launcherBytes
if($SelfTest){
    # Reborn: corrupt private copies at each literal and require exact-byte refusal; target files remain untouched.
    foreach($literal in $launcherLiterals){$fault=[byte[]]$launcherBytes.Clone();$fault[$literal.Raw]=$fault[$literal.Raw]-bxor 1;$rejected=$false
        try{Assert-Ep11LauncherLiterals $fault}catch{$rejected=$true};if(-not $rejected){throw 'Launcher literal fault admitted.'}
    }
    $rejected=$false;try{Assert-Ep11LauncherLiterals ([byte[]]::new(16))}catch{$rejected=$true};if(-not $rejected){throw 'Truncated launcher fixture admitted.'}
}
$launcherMemory=[IO.MemoryStream]::new($launcherBytes,$false);$launcherPe=[System.Reflection.PortableExecutable.PEReader]::new($launcherMemory)
try{
    $header=$launcherPe.PEHeaders.PEHeader
    if($launcherPe.PEHeaders.CoffHeader.Machine.ToString()-cne 'I386' -or $header.ImageBase-ne 0x400000){throw 'Launcher PE32 profile differs.'}
    $entrySections=@($launcherPe.PEHeaders.SectionHeaders|Where-Object{$header.AddressOfEntryPoint-ge $_.VirtualAddress -and [long]$header.AddressOfEntryPoint-lt [long]$_.VirtualAddress+$_.VirtualSize})
    if($entrySections.Count-ne 1 -or $entrySections[0].Name-cne '.bind'){throw 'Launcher entry section differs.'}
    $entryRaw=Get-Ep11LauncherRaw $launcherPe $header.AddressOfEntryPoint
    $wanted=@('CreateProcessW','GetCommandLineW','GetCommandLineA','ShellExecuteW')
    $bindings=[Collections.Generic.List[object]]::new();$importDirectory=$header.ImportTableDirectory;$ended=$false
    # Reborn: enumerate checked import-name thunks, retaining only launch/command-line APIs and their IAT identities, not call-site semantics.
    for($index=0;$index-lt 128;$index++){
        if($index*20+20-gt $importDirectory.Size){throw 'Launcher import directory truncated.'}
        $descriptor=Read-PeRange $launcherPe ($importDirectory.RelativeVirtualAddress+20*$index) 20
        if(@($descriptor|Where-Object{$_-ne 0}).Count-eq 0){$ended=$true;break}
        $dll=Read-PeName $launcherPe ([BitConverter]::ToUInt32($descriptor,12))
        $thunk=[BitConverter]::ToUInt32($descriptor,0);$iat=[BitConverter]::ToUInt32($descriptor,16)
        if($thunk-eq 0){throw 'Launcher import lacks original name thunks.'}
        $thunkEnded=$false
        for($slot=0;$slot-lt 1024;$slot++){
            $word=Read-PeRange $launcherPe ($thunk+4*$slot) 4;$value=[BitConverter]::ToUInt32($word,0)
            if($value-eq 0){$thunkEnded=$true;break};if(($value-band 0x80000000)-ne 0){continue}
            $name=Read-PeName $launcherPe ($value+2)
            if($name-cin $wanted){$bindings.Add([pscustomobject]@{Dll=$dll;Name=$name;IatVa=('0x{0:X8}'-f ($header.ImageBase+$iat+4*$slot));IatRaw=Get-Ep11LauncherRaw $launcherPe ($iat+4*$slot)})}
        }
        if(-not $thunkEnded){throw 'Launcher thunk bound exceeded.'}
    }
    if(-not $ended -or $bindings.Count-ne 4){throw 'Launcher selected import profile differs.'}
    $literalRows=[Collections.Generic.List[object]]::new()
    $textSections=@($launcherPe.PEHeaders.SectionHeaders|Where-Object{$_.Name-ceq '.text'})
    if($textSections.Count-ne 1){throw 'Launcher text section differs.'}
    $textSection=$textSections[0];$rawText=[Text.Encoding]::Latin1.GetString($launcherBytes,$textSection.PointerToRawData,$textSection.SizeOfRawData)
    foreach($literal in $launcherLiterals){
        $section=@($launcherPe.PEHeaders.SectionHeaders|Where-Object{$literal.Raw-ge $_.PointerToRawData -and $literal.Raw-lt [long]$_.PointerToRawData+$_.SizeOfRawData})
        if($section.Count-ne 1){throw 'Launcher literal raw mapping differs.'}
        $va=[uint32]($header.ImageBase+$section[0].VirtualAddress+$literal.Raw-$section[0].PointerToRawData)
        $needle=[Text.Encoding]::Latin1.GetString([BitConverter]::GetBytes($va));$wordCount=0;$start=0
        while(($offset=$rawText.IndexOf($needle,$start,[StringComparison]::Ordinal))-ge 0){$wordCount++;$start=$offset+1;if($wordCount-gt 128){throw 'Launcher word occurrence bound exceeded.'}}
        $literalRows.Add([pscustomobject]@{Raw=$literal.Raw;PreferredVa=('0x{0:X8}'-f $va);Encoding=$literal.Encoding;Text=$literal.Text;TextSectionLiteralVaWordOccurrences=$wordCount;InstructionXrefsProven=$false})
    }
}finally{$launcherPe.Dispose();$launcherMemory.Dispose()}
$launcherAfter=Read-PeSnapshot $LauncherPath
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($launcherAfter))-cne $launcherHash){throw 'Launcher changed during inspection.'}
$report=[pscustomobject]@{
    LauncherPath=[IO.Path]::GetFullPath($LauncherPath);LauncherBytes=$launcherBytes.Length;LauncherSha256=$launcherHash
    Machine=$launcherMetadata.Machine;Format=$launcherMetadata.Format;EntryPointRva=$launcherMetadata.EntryPointRva;EntryPointSection=$entrySections[0].Name;EntryPointRaw=$entryRaw
    Sections=$launcherMetadata.Sections;CodeView=$launcherMetadata.CodeView;SelectedImports=$bindings.ToArray();Literals=$literalRows.ToArray()
    LiteralPins=5;LiteralFaultsExecuted=$(if($SelfTest){5}else{0});TruncationRefusalsExecuted=$(if($SelfTest){1}else{0})
    ReadOnly=$true;TargetExecuted=$false;ProtectionLayerDecoded=$false;LauncherExecutableSelectionProven=$false;LauncherArgumentForwardingProven=$false
    ActualChildCommandLineObserved=$false;ConfigConsumptionObserved=$false;ModPackageLoaded=$false;ProductionBuildReady=$false
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 8}else{$report}
