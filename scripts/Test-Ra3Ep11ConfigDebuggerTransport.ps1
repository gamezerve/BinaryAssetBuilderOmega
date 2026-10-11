# Reborn: supply private CDB-format responses only; no debugger commands are sent and no process is accessed.
[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
$null=. (Join-Path $PSScriptRoot 'Test-Ra3Ep11ConfigSnapshotAdapter.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep11ConfigDebuggerTransport.ps1')

#-------------------------------------------------------------------------------------------------
<# Reborn: format deterministic supplied context fields with a requested nonce, never claim they came from a live debugger. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11ContextFixture($Snapshot,[string]$Nonce) {
    'REBORN_CONTEXT '+$Nonce+' '+((@($Snapshot.ProcessId,$Snapshot.ThreadId,$Snapshot.SiteAddress,$Snapshot.Eax,$Snapshot.Ebx,$Snapshot.Esi,$Snapshot.Edi,$Snapshot.Esp)|ForEach-Object {'{0:x}' -f $_}) -join ' ')
}
$positives=0;$refusals=0
$script:snapshotCounter=@{Reads=0};$session=New-Ep11ConfigSnapshotSession 16 $probe (New-Ep11SnapshotReader)
foreach($invocation in @(1,2)){
    foreach($snapshot in (New-Ep11SnapshotFixture)){
        $request=New-Ep11ConfigContextRequest $session
        if($request.Command -match '(?:^|;)\s*(?:g|gc|qd|!|\.shell)(?:\s|$)'){throw 'Context command unexpectedly controls execution.'}
        Add-Ep11ConfigDebuggerResponse $session (New-Ep11ContextFixture $snapshot $request.Nonce)
    }
}
$events=@(Complete-Ep11ConfigSnapshotSession $session)
if($events.Count -ne 10 -or $events[0].InvocationId -ne 1 -or $events[5].InvocationId -ne 2 -or $script:snapshotCounter.Reads -ne 12){throw 'Context streaming positive failed.'}
$positives++
# Reborn: independent per-thread lifetimes must survive interleaved register responses without global debugger scratch state.
$script:snapshotCounter=@{Reads=0};$baseReader=New-Ep11SnapshotReader
$interleavedReader={
    param([long]$Address,[int]$Count)
    $mapped=switch($Address){0x12000 {0x10000};0x21000 {0x20000};0x31000 {0x30000};0x13004 {0x11004};default {$Address}}
    $capture=& $baseReader $mapped $Count
    $capture.Address=$Address
    if($Address -eq 0x13004){$capture.Bytes=[BitConverter]::GetBytes([uint32]0x31000)}
    $capture
}.GetNewClosure()
$candidate=New-Ep11ConfigSnapshotSession 16 $probe $interleavedReader
$first=New-Ep11SnapshotFixture;$second=New-Ep11SnapshotFixture
foreach($snapshot in $second){
    $snapshot.ThreadId=33;$snapshot.Esi=0x21000;$snapshot.Edi=0x31000
    $snapshot.Esp=if($snapshot.SiteAddress -eq 0x4d9040){0x13000}else{0x12000}
    if($snapshot.SiteAddress -eq 0x4d86f2){$snapshot.Eax=0x21000}
}
for($index=0;$index -lt 5;$index++){
    foreach($snapshot in @($first[$index],$second[$index])){
        $request=New-Ep11ConfigContextRequest $candidate
        Add-Ep11ConfigDebuggerResponse $candidate (New-Ep11ContextFixture $snapshot $request.Nonce)
    }
}
$events=@(Complete-Ep11ConfigSnapshotSession $candidate)
if($events.Count -ne 10 -or $events[0].InvocationId -ne 1 -or $events[1].InvocationId -ne 2 -or $script:snapshotCounter.Reads -ne 12){throw 'Interleaved context lifetime failed.'}
$positives++
$nonce='0123456789abcdef0123456789abcdef';$sample=New-Ep11ContextFixture (New-Ep11SnapshotFixture)[0] $nonce
$record=Get-Ep11ConfigContextRecord ("0:000> context command`r`n"+$sample+"`r`n") $nonce
if($record.SiteAddress -ne 0x4d86b0){throw 'Context envelope positive failed.'};$positives++
foreach($mode in @('Nonce','Duplicate','UnknownMarker','Missing','Syntax','MemoryError','Oversized','WideRegister','MissingRegister','NegativeRegister','ZeroPid','ZeroThread','WrongPid','WrongSite','NoRequest','OverlapRequest','WrongThread','Replay','StaleNonce','PendingClose')){
    $script:snapshotCounter=@{Reads=0};$candidate=New-Ep11ConfigSnapshotSession 16 $probe (New-Ep11SnapshotReader)
    $request=New-Ep11ConfigContextRequest $candidate;$snapshot=(New-Ep11SnapshotFixture)[0]
    if($mode -ceq 'WrongPid'){$snapshot.ProcessId=17}
    if($mode -ceq 'WrongSite'){$snapshot.SiteAddress=0x1234}
    if($mode -ceq 'WrongThread'){$snapshot.SiteAddress=0x4d86f2;$snapshot.ThreadId=33}
    $text=New-Ep11ContextFixture $snapshot $request.Nonce
    switch($mode){
        'Nonce' {$text=$text.Replace($request.Nonce,$nonce)}
        'Duplicate' {$text+="`n"+$text}
        'UnknownMarker' {$text+="`nREBORN_UNKNOWN"}
        'Missing' {$text='0:000>'}
        'Syntax' {$text+="`nSyntax error"}
        'MemoryError' {$text+="`nMemory access error"}
        'Oversized' {$text+='x'*4096}
        'WideRegister' {$text+=' 100000000'}
        'MissingRegister' {$text=$text.Substring(0,$text.LastIndexOf(' '))}
        'NegativeRegister' {$text=$text.Replace(' 10 20 ',' -1 20 ')}
        'ZeroPid' {$text=$text.Replace(' 10 20 ',' 0 20 ')}
        'ZeroThread' {$text=$text.Replace(' 10 20 ',' 10 0 ')}
        'NoRequest' {$candidate.PendingContext=$null}
        'Replay' {Add-Ep11ConfigDebuggerResponse $candidate $text}
        'StaleNonce' {Add-Ep11ConfigDebuggerResponse $candidate $text;$null=New-Ep11ConfigContextRequest $candidate}
    }
    $before=$script:snapshotCounter.Reads;$refused=$false
    try{
        if($mode -ceq 'OverlapRequest'){$null=New-Ep11ConfigContextRequest $candidate}
        elseif($mode -ceq 'PendingClose'){$null=Complete-Ep11ConfigSnapshotSession $candidate}
        else{Add-Ep11ConfigDebuggerResponse $candidate $text}
    }catch{$refused=$true}
    if(-not $refused -or $candidate.Status -cne 'Faulted' -or $script:snapshotCounter.Reads -ne $before){throw ('Context fault admitted or read: '+$mode)}
    $retryRefused=$false;try{$null=New-Ep11ConfigContextRequest $candidate}catch{$retryRefused=$true}
    if(-not $retryRefused){throw 'Faulted context session accepted retry.'}
    $refusals+=2
}
# Reborn: response framing must wait for a complete newline-terminated marker and refuse ambiguous or trailing evidence.
$completed=$sample+"`nREBORN_CONTEXT_END "+$nonce+"`n";$windowFaults=0;$partialFaults=0
$null=Get-Ep11CompletedContextWindow ($completed+'0:000> ') $nonce
$windowCases=@(
    $sample,
    ($completed+"REBORN_CONTEXT_END "+$nonce+"`n"),
    $completed.Replace($nonce,'fedcba9876543210fedcba9876543210'),
    ("REBORN_CONTEXT_END "+$nonce+"`n"+$sample+"`n"),
    ($completed+'unexpected trailing output'),
    ($completed+('x'*4096)),
    ("Syntax error`n"+$completed),
    ($sample+"`n"+$completed),
    ("REBORN_CONTEXT "+$nonce+" 10`nREBORN_CONTEXT_END "+$nonce+"`n"),
    $completed.TrimEnd("`n")
)
foreach($fault in $windowCases){
    $refused=$false;try{$null=Get-Ep11CompletedContextWindow $fault $nonce}catch{$refused=$true}
    if(-not $refused){throw 'Context window fault admitted.'};$windowFaults++
}
foreach($length in @(1,5,20,($completed.Length-10),($completed.Length-1))){
    $refused=$false;try{$null=Get-Ep11CompletedContextWindow $completed.Substring(0,$length) $nonce}catch{$refused=$true}
    if(-not $refused){throw 'Partial context chunk admitted.'};$partialFaults++
}
# Reborn: helper-only site/hit protocols must not accept unowned, wrong-site, preexisting-trap or ambiguous markers.
$siteSample='REBORN_SITE '+$nonce+' 77661234 b8';$hitSample='REBORN_BREAK '+$nonce+' 10 20 77661234';$markerFaults=0
$null=Get-Ep11HelperBreakpointSite $siteSample $nonce
$null=Get-Ep11HelperBreakpointHit $hitSample $nonce 16 0x77661234
$clearSample="REBORN_CLEAR "+$nonce+"`n0:000> "
Assert-Ep11HelperBreakpointCleared $clearSample $nonce
$clearFaults=0
foreach($fault in @('',$clearSample.Replace($nonce,'fedcba9876543210fedcba9876543210'),($clearSample+"`nREBORN_CLEAR "+$nonce),($clearSample+"`n 0 e 77661234 0001 (0001)"),($clearSample+"`nSyntax error"))){
    $refused=$false;try{Assert-Ep11HelperBreakpointCleared $fault $nonce}catch{$refused=$true}
    if(-not $refused){throw 'Helper clear fault admitted.'};$clearFaults++
}
foreach($fault in @($siteSample.Replace('b8','cc'),$siteSample.Replace('77661234','0'),($siteSample+"`n"+$siteSample),$siteSample.Replace($nonce,'fedcba9876543210fedcba9876543210'),($siteSample+"`nSyntax error"),$siteSample.Replace('b8','100'),($siteSample+"`nREBORN_UNKNOWN"),'')){
    $refused=$false;try{$null=Get-Ep11HelperBreakpointSite $fault $nonce}catch{$refused=$true}
    if(-not $refused){throw 'Helper site fault admitted.'};$markerFaults++
}
foreach($fault in @($hitSample.Replace(' 10 20 ',' 11 20 '),$hitSample.Replace('77661234','77661235'),$hitSample.Replace(' 10 20 ',' 10 0 '),$hitSample.Replace($nonce,'fedcba9876543210fedcba9876543210'),($hitSample+"`n"+$hitSample),($hitSample+"`nREBORN_UNKNOWN"),($hitSample+"`nMemory access error"),'')){
    $refused=$false;try{$null=Get-Ep11HelperBreakpointHit $fault $nonce 16 0x77661234}catch{$refused=$true}
    if(-not $refused){throw 'Helper hit fault admitted.'};$markerFaults++
}
[pscustomobject]@{DetachedPositiveCases=$positives;PrivateRefusals=$refusals;WindowPositiveCases=1;WindowRefusals=$windowFaults;PartialChunkRefusals=$partialFaults;HelperMarkerPositiveCases=2;HelperMarkerRefusals=$markerFaults;HelperClearPositiveCases=1;HelperClearRefusals=$clearFaults;PerThreadInvocationIdsAssigned=$true;CommandsSent=$false;GameExecuted=$false;DebuggerExecuted=$false;LiveContextProven=$false;CollectorReady=$false}
