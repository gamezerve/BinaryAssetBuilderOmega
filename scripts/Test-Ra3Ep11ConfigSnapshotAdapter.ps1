# Reborn: exercise private register/memory fixtures only; never open a process or execute a target.
[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
. (Join-Path $PSScriptRoot 'Ra3Ep11ConfigSnapshotAdapter.ps1')
$probe=Join-Path (Split-Path -Parent $PSScriptRoot) 'fixtures/ra3ep11/phase-a/probe_1.0.cfg'

#-------------------------------------------------------------------------------------------------
<# Reborn: build private register snapshots matching the five reviewed entry/return ABI sites. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11SnapshotFixture {
    $index=0
    foreach($site in @(0x4d86b0,0x4d86f2,0x4d8725,0x4d9040,0x4d98ab)){
        $index++
        [pscustomobject]@{Sequence=$index;ProcessId=16;ThreadId=32;InvocationId=1;SiteAddress=$site;Eax=$(if($index -eq 2){0x20000}else{1});Ebx=1;Esi=0x20000;Edi=0x30000;Esp=$(if($index -eq 4){0x11000}else{0x10000})}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: expose deterministic address-bound byte captures and targeted transfer/content failures without OS memory access. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11SnapshotReader([string]$Mode='Valid',[long]$Vtable=0xbfa208) {
    $path=[Text.Encoding]::ASCII.GetBytes($probe+[char]0)
    # Reborn: retain one private mutable counter across the closure's module scope.
    $counter=$script:snapshotCounter
    {
        param([long]$Address,[int]$Count)
        $counter.Reads++
        $bytes=switch($Address){
            0x10000 { [byte[]]([BitConverter]::GetBytes([uint32]0x4d98ab)+[BitConverter]::GetBytes([uint32]0x40000)) }
            0x40000 { [byte[]]$path.Clone() }
            0x20000 { [BitConverter]::GetBytes([uint32]$Vtable) }
            0x30000 { if($Count -eq 1){[byte[]]@(10)}else{[byte[]]@(10,0)} }
            0x11004 { [BitConverter]::GetBytes([uint32]0x30000) }
            default { throw 'Private reader received unexpected address.' }
        }
        $bytes=[byte[]]$bytes;$success=$true;$actual=$Count;$reported=$Address
        if($Address -eq 0x10000){
            switch($Mode){
                'FailedRead' {$success=$false}
                'ShortRead' {$actual--}
                'WrongAddress' {$reported++}
                'ShortBuffer' {$bytes=[byte[]]@(0)}
                'Nonboolean' {$success=1}
                'Caller' {$bytes[0]=$bytes[0] -bxor 1}
            }
        }
        if($Mode -ceq 'Path' -and $Address -eq 0x40000){$bytes[0]=$bytes[0] -bxor 1}
        if($Mode -ceq 'ReadByte' -and $Address -eq 0x30000 -and $Count -eq 1){$bytes[0]=11}
        if($Mode -ceq 'ParserPointer' -and $Address -eq 0x11004){$bytes[0]=1}
        if($Mode -ceq 'ParserNul' -and $Address -eq 0x30000 -and $Count -eq 2){$bytes[1]=1}
        [pscustomobject]@{Address=$reported;Success=$success;ActualCount=$actual;Bytes=$bytes}
    }.GetNewClosure()
}
$positives=0;$refusals=0
foreach($vtable in @(0xbfa208,0xbfa270)){
    $script:snapshotCounter=@{Reads=0}
    $events=ConvertTo-Ep11ConfigReadEvents (New-Ep11SnapshotFixture) 16 $probe (New-Ep11SnapshotReader -Vtable $vtable)
    if($events.Count -ne 5 -or $script:snapshotCounter.Reads -ne 6 -or -not (Get-Ep11ConfigReadCorrelation $events 16 $probe).SuppliedEngineChainValidated){throw 'Snapshot positive failed.'}
    $positives++
}
foreach($mode in @('FailedRead','ShortRead','WrongAddress','ShortBuffer','Nonboolean','Caller','Path','ReadByte','ParserPointer','ParserNul','UnknownVtable','Pid','Thread','Invocation','Sequence','Site','RegisterString','RegisterFraction','RegisterOverflow','NullStack','ReadObject','ReadCount','ReadSize','NullBuffer','Incomplete','WrongBase')){
    $snapshots=New-Ep11SnapshotFixture;$readerMode=$mode;$vtable=0xbfa208;$base=0x400000L;$script:snapshotCounter=@{Reads=0}
    switch($mode){
        'UnknownVtable' {$vtable=0x1234}
        'Pid' {$snapshots[0].ProcessId=17}
        'Thread' {$snapshots[2].ThreadId=33}
        'Invocation' {$snapshots[2].InvocationId=2}
        'Sequence' {$snapshots[0].Sequence=2}
        'Site' {$snapshots[0].SiteAddress=0x1234}
        'RegisterString' {$snapshots[0].Eax='1'}
        'RegisterFraction' {$snapshots[0].Eax=1.5}
        'RegisterOverflow' {$snapshots[0].Eax=0x100000000L}
        'NullStack' {$snapshots[0].Esp=0}
        'ReadObject' {$snapshots[2].Esi++}
        'ReadCount' {$snapshots[2].Eax=0}
        'ReadSize' {$snapshots[2].Ebx=2}
        'NullBuffer' {$snapshots[2].Edi=0}
        'Incomplete' {$snapshots[4].SiteAddress=0x4d9040}
        'WrongBase' {$base=0x500000}
    }
    $refused=$false
    try{$null=ConvertTo-Ep11ConfigReadEvents $snapshots 16 $probe (New-Ep11SnapshotReader $readerMode $vtable) $base}catch{$refused=$true}
    if(-not $refused){throw ('Snapshot fault admitted: '+$mode)}
    if($mode -in @('Pid','Sequence','Site','RegisterString','RegisterFraction','RegisterOverflow','NullStack','WrongBase') -and $script:snapshotCounter.Reads -ne 0){throw ('Invalid initial snapshot caused reads: '+$mode)}
    $refusals++
}
# Reborn: repeat baseline after private fault mutations to detect shared capture contamination.
$null=ConvertTo-Ep11ConfigReadEvents (New-Ep11SnapshotFixture) 16 $probe (New-Ep11SnapshotReader)
[pscustomobject]@{DetachedPositiveCases=$positives;PrivateRefusals=$refusals;ReadsPerPositive=6;GameExecuted=$false;ProcessQueried=$false;LiveEvidenceCollected=$false;CollectorReady=$false}
