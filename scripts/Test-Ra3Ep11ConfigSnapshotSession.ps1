# Reborn: test stop-by-stop capture and terminal session states without debugger or process access.
[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
# Reborn: reuse the detached fixture builders and require their compatibility tests to pass first.
$null=. (Join-Path $PSScriptRoot 'Test-Ra3Ep11ConfigSnapshotAdapter.ps1')
$script:snapshotCounter=@{Reads=0};$phase=@{Value=0}
$reader=New-Ep11SnapshotReader
# Reborn: permit memory access only for the current synthetic stop; earlier memory cannot be replayed later.
$streamReader={
    param([long]$Address,[int]$Count)
    $allowed=switch($phase.Value){
        1 {@(0x10000,0x40000)}
        2 {@(0x20000)}
        3 {@(0x30000)}
        4 {@(0x11004,0x30000)}
        default {@()}
    }
    if($Address -notin $allowed){throw 'Read outside current synthetic stop.'}
    & $reader $Address $Count
}.GetNewClosure()
$session=New-Ep11ConfigSnapshotSession 16 $probe $streamReader
$snapshots=New-Ep11SnapshotFixture;$index=0
foreach($snapshot in $snapshots){
    $index++;$phase.Value=$index;Add-Ep11ConfigSnapshot $session $snapshot
    if($session.Sequence -ne $index -or $session.Events.Count -ne $index){throw 'Incremental event did not commit at current stop.'}
}
$phase.Value=0;$events=@(Complete-Ep11ConfigSnapshotSession $session)
if($session.Status -cne 'Closed' -or $events.Count -ne 5 -or $script:snapshotCounter.Reads -ne 6){throw 'Streaming positive failed.'}
# Reborn: exported mutable byte arrays must not alias the retained capture.
$events[2].Bytes[0]=11
if($session.Events[2].Bytes[0] -ne 10){throw 'Exported capture aliases retained bytes.'}
$positives=2;$refusals=0
foreach($mode in @('AddAfterClose','CloseTwice','EmptyClose','PartialClose','WrongSequence','CaptureFailure','UnknownSite','WrongThread','EventLimit')){
    $script:snapshotCounter=@{Reads=0};$reader=New-Ep11SnapshotReader
    $candidate=New-Ep11ConfigSnapshotSession 16 $probe $reader;$fixture=New-Ep11SnapshotFixture
    switch($mode){
        'AddAfterClose' {$candidate=$session}
        'CloseTwice' {$candidate=$session}
        'PartialClose' {Add-Ep11ConfigSnapshot $candidate $fixture[0]}
        'WrongSequence' {$fixture[0].Sequence=2}
        'CaptureFailure' {$candidate.ReadAdapter=New-Ep11SnapshotReader -Mode FailedRead}
        'UnknownSite' {$fixture[0].SiteAddress=0x1234}
        'WrongThread' {Add-Ep11ConfigSnapshot $candidate $fixture[0];$fixture[1].ThreadId=33}
        # Reborn: inject a private boundary-state fixture; production callers must not mutate session internals.
        'EventLimit' {$candidate.Sequence=320;$fixture[0].Sequence=321}
    }
    $before=$script:snapshotCounter.Reads;$refused=$false
    try{
        if($mode -in @('CloseTwice','EmptyClose','PartialClose')){$null=Complete-Ep11ConfigSnapshotSession $candidate}
        elseif($mode -ceq 'WrongThread'){Add-Ep11ConfigSnapshot $candidate $fixture[1]}
        else{Add-Ep11ConfigSnapshot $candidate $fixture[0]}
    }catch{$refused=$true}
    if(-not $refused){throw ('Session fault admitted: '+$mode)}
    if($mode -cne 'CaptureFailure' -and $script:snapshotCounter.Reads -ne $before){throw ('Refused session caused reads: '+$mode)}
    $after=$script:snapshotCounter.Reads;$retryRefused=$false
    try{Add-Ep11ConfigSnapshot $candidate (New-Ep11SnapshotFixture)[0]}catch{$retryRefused=$true}
    if(-not $retryRefused -or $script:snapshotCounter.Reads -ne $after){throw ('Terminal session resumed: '+$mode)}
    $refusals+=2
}
# Reborn: sequential complete invocations can reuse stack/object/buffer addresses with fresh invocation IDs.
$script:snapshotCounter=@{Reads=0};$candidate=New-Ep11ConfigSnapshotSession 16 $probe (New-Ep11SnapshotReader)
$sequence=0
foreach($invocation in @(1,2)){
    foreach($snapshot in (New-Ep11SnapshotFixture)){$sequence++;$snapshot.Sequence=$sequence;$snapshot.InvocationId=$invocation;Add-Ep11ConfigSnapshot $candidate $snapshot}
}
$events=@(Complete-Ep11ConfigSnapshotSession $candidate)
if($events.Count -ne 10 -or $script:snapshotCounter.Reads -ne 12){throw 'Streaming sequential reuse failed.'}
$positives++
[pscustomobject]@{StreamingPositiveCases=$positives;PrivateRefusals=$refusals;CaptureAtCurrentStopValidated=$true;TerminalRetryRefused=$true;ExportIsolationValidated=$true;GameExecuted=$false;ProcessQueried=$false;CollectorReady=$false}
