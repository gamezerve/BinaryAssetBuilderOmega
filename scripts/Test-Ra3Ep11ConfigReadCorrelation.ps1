# Reborn: detached supplied-event tests exercise thread/object/buffer correlation without launching or inspecting a process.
[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
. (Join-Path $PSScriptRoot 'Ra3Ep11ConfigReadCorrelation.ps1')
$probe=Join-Path (Split-Path -Parent $PSScriptRoot) 'fixtures/ra3ep11/phase-a/probe_1.0.cfg'
#-------------------------------------------------------------------------------------------------
<# Reborn: generate private five-stage records for a fixed one-byte fixture, never pretend they came from CDB or a game. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11ReadFixture([int]$Invocation=1,[int]$Thread=32,[long]$Object=0x10000,[long]$Buffer=0x20000,[long]$Vtable=0xbfa208) {
    $events=@(
        [pscustomobject]@{Kind='ReaderEnter';SiteAddress=0x4d86b0;Path=$probe;CallerReturnAddress=0x4d98ab},
        [pscustomobject]@{Kind='OpenReturn';SiteAddress=0x4d86f2;ObjectAddress=$Object;ReaderVtable=$Vtable;CaptureSucceeded=$true;CapturedCount=4;Bytes=[BitConverter]::GetBytes([uint32]$Vtable)},
        [pscustomobject]@{Kind='ReadReturn';SiteAddress=0x4d8725;ObjectAddress=$Object;BufferAddress=$Buffer;RequestedCount=1;ReturnedCount=1;CaptureSucceeded=$true;CapturedCount=1;Bytes=[byte[]]@(10)},
        [pscustomobject]@{Kind='ParserEnter';SiteAddress=0x4d9040;BufferAddress=$Buffer;CaptureSucceeded=$true;CapturedCount=2;Bytes=[byte[]]@(10,0)},
        [pscustomobject]@{Kind='ReaderExit';SiteAddress=0x4d98ab}
    )
    $sequence=0
    foreach($event in $events){$sequence++;$event|Add-Member Sequence $sequence;$event|Add-Member ProcessId 16;$event|Add-Member ThreadId $Thread;$event|Add-Member InvocationId $Invocation}
    $events
}
$memory=New-Ep11ReadFixture
$source=New-Ep11ReadFixture -Vtable 0xbfa270
$null=Get-Ep11ConfigReadCorrelation $memory 16 $probe
$null=Get-Ep11ConfigReadCorrelation $source 16 $probe
# Reborn: interleave distinct thread/object/buffer lifetimes, unlike unsafe global debugger scratch registers.
$second=New-Ep11ReadFixture -Invocation 2 -Thread 33 -Object 0x11000 -Buffer 0x21000
$interleaved=@();for($index=0;$index -lt 5;$index++){$interleaved+=$memory[$index].PSObject.Copy();$interleaved+=$second[$index].PSObject.Copy()}
for($index=0;$index -lt $interleaved.Count;$index++){$interleaved[$index].Sequence=$index+1}
if((Get-Ep11ConfigReadCorrelation $interleaved 16 $probe).CompletedInvocationCount -ne 2){throw 'Interleaved reader chains failed.'}
# Reborn: sequential invocations may recycle pointers only after the earlier lifetime is closed.
$sequential=@(New-Ep11ReadFixture)+@(New-Ep11ReadFixture -Invocation 2)
for($index=0;$index -lt $sequential.Count;$index++){$sequential[$index].Sequence=$index+1}
if((Get-Ep11ConfigReadCorrelation $sequential 16 $probe).CompletedInvocationCount -ne 2){throw 'Sequential pointer reuse failed.'}
$refusals=0
foreach($mode in @('Pid','Tid','Invocation','Sequence','Path','Caller','EntrySite','OpenSite','UnknownVtable','ObjectCapture','ObjectCount','ObjectByte','NullObject','ReadObject','NullBuffer','BufferOverflow','RequestedCount','ReturnedCount','ReadCapture','ReadCount','ReadByte','ReadLength','ReadSite','ParserBuffer','ParserCapture','ParserCount','ParserByte','ParserNul','ParserSite','ExitSite','UnknownKind','Missing','Duplicate','Unclosed','WrongBase','WrongPidScope')){
    $fault=New-Ep11ReadFixture;$base=0x400000L;$pidScope=16
    switch($mode){
        'Pid' {$fault[2].ProcessId=17}
        'Tid' {$fault[2].ThreadId=33}
        'Invocation' {$fault[2].InvocationId=2}
        'Sequence' {$fault[2].Sequence++}
        'Path' {$fault[0].Path=$probe+'.other'}
        'Caller' {$fault[0].CallerReturnAddress++}
        'EntrySite' {$fault[0].SiteAddress++}
        'OpenSite' {$fault[1].SiteAddress++}
        'UnknownVtable' {$fault[1].ReaderVtable=0x1234}
        'ObjectCapture' {$fault[1].CaptureSucceeded=$false}
        'ObjectCount' {$fault[1].CapturedCount=0}
        'ObjectByte' {$fault[1].Bytes[0]=$fault[1].Bytes[0] -bxor 1}
        'NullObject' {$fault[1].ObjectAddress=0}
        'ReadObject' {$fault[2].ObjectAddress++}
        'NullBuffer' {$fault[2].BufferAddress=0}
        'BufferOverflow' {$fault[2].BufferAddress=0xffffffffL}
        'RequestedCount' {$fault[2].RequestedCount=2}
        'ReturnedCount' {$fault[2].ReturnedCount=0}
        'ReadCapture' {$fault[2].CaptureSucceeded=$false}
        'ReadCount' {$fault[2].CapturedCount=0}
        'ReadByte' {$fault[2].Bytes[0]=11}
        'ReadLength' {$fault[2].Bytes=[byte[]]::new(0)}
        'ReadSite' {$fault[2].SiteAddress++}
        'ParserBuffer' {$fault[3].BufferAddress++}
        'ParserCapture' {$fault[3].CaptureSucceeded=$false}
        'ParserCount' {$fault[3].CapturedCount=1}
        'ParserByte' {$fault[3].Bytes[0]=11}
        'ParserNul' {$fault[3].Bytes[1]=1}
        'ParserSite' {$fault[3].SiteAddress++}
        'ExitSite' {$fault[4].SiteAddress++}
        'UnknownKind' {$fault[3].Kind='Other'}
        'Missing' {$fault=@($fault|Select-Object -Skip 1)}
        'Duplicate' {$fault[2]=$fault[1]}
        'Unclosed' {$fault=@($fault[0..3])+@((New-Ep11ReadFixture -Invocation 2 -Thread 33)[0]);$fault[4].Sequence=5}
        'WrongBase' {$base=0x500000}
        'WrongPidScope' {$pidScope=17}
    }
    $refused=$false;try{$null=Get-Ep11ConfigReadCorrelation $fault $pidScope $probe $base}catch{$refused=$true};if(-not $refused){throw ('Reader fault admitted: '+$mode)};$refusals++
}
foreach($mode in @('SameThread','SameObject','SameBuffer','OverlappingBuffer','ReusedInvocation')){
    $first=New-Ep11ReadFixture;$second=New-Ep11ReadFixture -Invocation 2 -Thread 33 -Object 0x11000 -Buffer 0x21000
    switch($mode){
        'SameThread' {foreach($event in $second){$event.ThreadId=32}}
        'SameObject' {$second[1].ObjectAddress=0x10000;$second[2].ObjectAddress=0x10000}
        'SameBuffer' {$second[2].BufferAddress=0x20000;$second[3].BufferAddress=0x20000}
        'OverlappingBuffer' {$second[2].BufferAddress=0x20001;$second[3].BufferAddress=0x20001}
        'ReusedInvocation' {foreach($event in $second){$event.InvocationId=1}}
    }
    $fault=@();for($index=0;$index -lt 5;$index++){$fault+=$first[$index];$fault+=$second[$index]}
    for($index=0;$index -lt $fault.Count;$index++){$fault[$index].Sequence=$index+1}
    $refused=$false;try{$null=Get-Ep11ConfigReadCorrelation $fault 16 $probe}catch{$refused=$true};if(-not $refused){throw ('Reader lifetime fault admitted: '+$mode)};$refusals++
}
# Reborn: repeat the genuine fixture after fault injection to catch contamination of baseline arrays/records.
$null=Get-Ep11ConfigReadCorrelation (New-Ep11ReadFixture) 16 $probe
[pscustomobject]@{DetachedPositiveCases=4;PrivateRefusals=$refusals;InterleavedThreadsValidated=$true;SequentialPointerReuseValidated=$true;GameExecuted=$false;LiveGameReadProven=$false;WindowsFileReadProven=$false;CollectorReady=$false}
