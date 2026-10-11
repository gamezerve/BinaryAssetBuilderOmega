# Reborn: detached probe selection with private path bytes only; no debugger commands, processes or real files are read.
[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
$null=. (Join-Path $PSScriptRoot 'Test-Ra3Ep11ConfigSnapshotAdapter.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep11ConfigDebuggerTransport.ps1')

#-------------------------------------------------------------------------------------------------
<# Reborn: supply private entry bytes and retain only requested-address/count diagnostics for selection tests. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11FilterReader([string]$Mode='Exact') {
    $baseReader=New-Ep11SnapshotReader;$path=[Text.Encoding]::ASCII.GetBytes($probe+[char]0);$trace=$script:filterTrace
    {
        param([long]$Address,[int]$Count)
        $trace.Reads++;$trace.Requests.Add([pscustomobject]@{Address=$Address;Count=$Count})
        if($Count -eq 1 -and $Address -ge 0x40000 -and $Address -lt 0x40000+$path.Length){
            $index=[int]($Address-0x40000);$value=$path[$index]
            switch($Mode){
                'FirstMismatch' {if($index -eq 0){$value=88}}
                'ShortPath' {if($index -eq 1){$value=0}}
                'LongPrefix' {if($index -eq $path.Length-1){$value=97}}
                'CaseMismatch' {if($index -eq 0){$value=[byte]([char]([char]$value).ToString().ToLowerInvariant())}}
            }
            $result=[pscustomobject]@{Address=$Address;Success=$true;ActualCount=1;Bytes=[byte[]]@($value)}
            if($Mode -ceq 'PathReadFailure'){$result.Success=$false;$result.ActualCount=0}
            if($Mode -ceq 'WrongReadAddress'){$result.Address++}
            $result;return
        }
        $result=& $baseReader $Address $Count
        if($Address -eq 0x10000){
            if($Mode -ceq 'WrongCaller'){$result.Bytes[0]=$result.Bytes[0] -bxor 1}
            if($Mode -ceq 'NullPath'){[BitConverter]::GetBytes([uint32]0).CopyTo($result.Bytes,4)}
        }
        # Reborn: a changing exact path must fail the decoder's independent recapture, not become accepted filter evidence.
        if($Mode -ceq 'PathChanges' -and $Address -eq 0x40000 -and $Count -gt 1){$result.Bytes[0]=$result.Bytes[0] -bxor 1}
        $result
    }.GetNewClosure()
}

#-------------------------------------------------------------------------------------------------
<# Reborn: send a supplied record through the same request/response nonce protocol used by the decoder. #>
#-------------------------------------------------------------------------------------------------
function Send-Ep11FilterFixture($Session,$Snapshot) {
    $request=New-Ep11ConfigContextRequest $Session
    $text='REBORN_CONTEXT '+$request.Nonce+' '+((@($Snapshot.ProcessId,$Snapshot.ThreadId,$Snapshot.SiteAddress,$Snapshot.Eax,$Snapshot.Ebx,$Snapshot.Esi,$Snapshot.Edi,$Snapshot.Esp)|ForEach-Object {'{0:x}' -f $_}) -join ' ')
    Add-Ep11ConfigDebuggerResponse $Session $text
}

#-------------------------------------------------------------------------------------------------
<# Reborn: reset a private trace and create an explicitly filtered test session with no native read API. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11FilterFixture([string]$Mode='Exact') {
    $script:snapshotCounter=@{Reads=0};$script:filterTrace=@{Reads=0;Requests=[Collections.Generic.List[object]]::new()}
    New-Ep11ConfigSnapshotSession 16 $probe (New-Ep11FilterReader $Mode) -FilterProbeEntries
}
$positives=0;$refusals=0
$candidate=New-Ep11FilterFixture
foreach($snapshot in (New-Ep11SnapshotFixture)){Send-Ep11FilterFixture $candidate $snapshot}
$events=@(Complete-Ep11ConfigSnapshotSession $candidate)
if($events.Count -ne 5 -or $candidate.IgnoredContextResponses -ne 0 -or $script:filterTrace.Reads -ne 7+$candidate.PathBytes.Length){throw 'Exact probe selection failed.'}
$positives++
foreach($mode in @('WrongCaller','FirstMismatch','ShortPath','LongPrefix','CaseMismatch')){
    $candidate=New-Ep11FilterFixture $mode
    Send-Ep11FilterFixture $candidate (New-Ep11SnapshotFixture)[0]
    if($candidate.Events.Count -ne 0 -or $candidate.Active.Count -ne 0 -or $candidate.IgnoredContextResponses -ne 1){throw ('Unrelated entry retained: '+$mode)}
    $expected=switch($mode){'WrongCaller' {1};'FirstMismatch' {2};'ShortPath' {3};'LongPrefix' {1+$candidate.PathBytes.Length};'CaseMismatch' {2}}
    if($script:filterTrace.Reads -ne $expected){throw ('Unrelated path read beyond first mismatch: '+$mode)}
    $positives++
}
# Reborn: unrelated known-site contexts do not cause reader object or buffer reads, even while another thread has an active probe.
$candidate=New-Ep11FilterFixture
Send-Ep11FilterFixture $candidate (New-Ep11SnapshotFixture)[0]
$before=$script:filterTrace.Reads
foreach($snapshot in (New-Ep11SnapshotFixture|Select-Object -Skip 1)){$snapshot.ThreadId=33;Send-Ep11FilterFixture $candidate $snapshot}
if($candidate.IgnoredContextResponses -ne 4 -or $script:filterTrace.Reads -ne $before){throw 'Untracked thread caused captures.'}
foreach($snapshot in (New-Ep11SnapshotFixture|Select-Object -Skip 1)){Send-Ep11FilterFixture $candidate $snapshot}
$events=@(Complete-Ep11ConfigSnapshotSession $candidate)
if($events.Count -ne 5 -or $candidate.ContextResponses -ne 9){throw 'Mixed tracked/untracked stream failed.'};$positives++
foreach($mode in @('WrongPid','UnknownSite','NullStack','NullPath','PathReadFailure','WrongReadAddress','PathChanges','NestedEntry','StopLimit','IgnoredOnlyComplete')){
    $candidate=New-Ep11FilterFixture $(if($mode -eq 'IgnoredOnlyComplete'){'WrongCaller'}elseif($mode -in @('NullPath','PathReadFailure','WrongReadAddress','PathChanges')){$mode}else{'Exact'})
    $snapshot=(New-Ep11SnapshotFixture)[0]
    switch($mode){
        'WrongPid' {$snapshot.ProcessId=17}
        'UnknownSite' {$snapshot.SiteAddress=0x1234}
        'NullStack' {$snapshot.Esp=0}
        'NestedEntry' {Send-Ep11FilterFixture $candidate $snapshot}
        # Reborn: inject a private boundary-state fixture; no actual game-stop count is implied.
        'StopLimit' {$candidate.ContextResponses=128}
        'IgnoredOnlyComplete' {Send-Ep11FilterFixture $candidate $snapshot}
    }
    $before=$script:filterTrace.Reads;$refused=$false
    try{if($mode -ceq 'IgnoredOnlyComplete'){$null=Complete-Ep11ConfigSnapshotSession $candidate}else{Send-Ep11FilterFixture $candidate $snapshot}}catch{$refused=$true}
    if(-not $refused -or $candidate.Status -cne 'Faulted'){throw ('Filter fault admitted: '+$mode)}
    if($mode -in @('WrongPid','UnknownSite','NullStack','NestedEntry','StopLimit','IgnoredOnlyComplete') -and $script:filterTrace.Reads -ne $before){throw ('Refused filter caused reads: '+$mode)}
    $before=$script:filterTrace.Reads;$retryRefused=$false
    try{Send-Ep11FilterFixture $candidate (New-Ep11SnapshotFixture)[0]}catch{$retryRefused=$true}
    if(-not $retryRefused -or $script:filterTrace.Reads -ne $before){throw 'Faulted filter resumed.'}
    $refusals+=2
}
[pscustomobject]@{DetachedPositiveCases=$positives;PrivateRefusals=$refusals;UnrelatedPathReadsStopAtMismatch=$true;UntrackedEventsCauseNoReads=$true;IgnoredCallsAreNotProof=$true;GameExecuted=$false;DebuggerExecuted=$false;CollectorReady=$false}
