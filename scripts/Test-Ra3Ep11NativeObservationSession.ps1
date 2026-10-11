# Reborn: connect the real pinned disk admission plan to private stop/context/capture fixtures; never query, launch or attach to a process.
[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
$null=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11NativeSupervisorPreflight.ps1') -SelfTest
$nativePlan=$plan;$nativeIdentity=$context.PSObject.Copy();$nativeInitial=$initial;$nativeRead=$adapter
$null=. (Join-Path $PSScriptRoot 'Test-Ra3Ep11ConfigProbeFilter.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep11NativeObservationSession.ps1')

#-------------------------------------------------------------------------------------------------
<# Reborn: create a fresh private observer with the real shared admission decision and counted synthetic memory captures. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11NativeFixture([string]$ReaderMode='Exact') {
    $script:snapshotCounter=@{Reads=0};$script:filterTrace=@{Reads=0;Requests=[Collections.Generic.List[object]]::new()}
    New-Ep11NativeObservationSession $nativePlan $nativeIdentity $nativeInitial $nativeRead $probe (New-Ep11FilterReader $ReaderMode)
}

#-------------------------------------------------------------------------------------------------
<# Reborn: format a nonce-bound native stop and independent completed context, never send them to a debugger. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11NativeStopFixture($Session,$Snapshot) {
    $id=[array]::IndexOf($Session.Sites,[long]$Snapshot.SiteAddress)
    'REBORN_NATIVE_STOP '+$Session.StopNonce+' '+$id+' '+(('{0:x} {1:x} {2:x}' -f $Snapshot.ProcessId,$Snapshot.ThreadId,$Snapshot.SiteAddress))
}
#-------------------------------------------------------------------------------------------------
<# Reborn: frame private registers independently of their stop marker; completion does not prove OS stopped state. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11NativeContextFixture($Snapshot,[string]$Nonce) {
    'REBORN_CONTEXT '+$Nonce+' '+((@($Snapshot.ProcessId,$Snapshot.ThreadId,$Snapshot.SiteAddress,$Snapshot.Eax,$Snapshot.Ebx,$Snapshot.Esi,$Snapshot.Edi,$Snapshot.Esp)|ForEach-Object {'{0:x}' -f $_}) -join ' ')+"`nREBORN_CONTEXT_END "+$Nonce+"`n"
}
$positives=0;$refusals=0
$candidate=New-Ep11NativeFixture
foreach($snapshot in (New-Ep11SnapshotFixture)){
    $request=Start-Ep11NativeObservationStop $candidate (New-Ep11NativeStopFixture $candidate $snapshot)
    Add-Ep11NativeObservationContext $candidate (New-Ep11NativeContextFixture $snapshot $request.Nonce)
    Complete-Ep11NativeObservationStop $candidate
}
$result=Complete-Ep11NativeObservationSession $candidate
if(-not $result.SuppliedEngineChainValidated -or $result.SuppliedStopContextPairs -ne 5 -or $result.CompletedInvocationCount -ne 1 -or
   -not $result.NeedsBreakpointCleanup -or $result.LiveGameReadProven -or $result.DebuggerCommandsSent){throw 'Native supplied chain boundary failed.'};$positives++
# Reborn: known untracked returns are ignored only after their stop/context identity still matches, and cannot create a completed chain.
$candidate=New-Ep11NativeFixture;$snapshot=(New-Ep11SnapshotFixture)[1]
$request=Start-Ep11NativeObservationStop $candidate (New-Ep11NativeStopFixture $candidate $snapshot)
Add-Ep11NativeObservationContext $candidate (New-Ep11NativeContextFixture $snapshot $request.Nonce)
Complete-Ep11NativeObservationStop $candidate
if($candidate.Decoder.IgnoredContextResponses -ne 1 -or $script:filterTrace.Reads -ne 0){throw 'Untracked native stop read memory.'};$positives++
$refused=$false;try{$null=Complete-Ep11NativeObservationSession $candidate}catch{$refused=$true};if(-not $refused){throw 'Ignored-only observer proved a chain.'};$refusals++
foreach($mode in @('StopNonce','StopPid','StopThread','StopSite','StopId','Duplicate','StopError','Oversized','Budget','ContextNonce','ContextPid','ContextThread','ContextSite','ContextPartial','ContextDuplicate','ReplayStop')){
    $candidate=New-Ep11NativeFixture;$snapshot=(New-Ep11SnapshotFixture)[0]
    $stop=New-Ep11NativeStopFixture $candidate $snapshot
    switch($mode){
        'StopNonce' {$stop=$stop.Replace($candidate.StopNonce,'0'*32)}
        'StopPid' {$stop=$stop.Replace(' 10 20 ',' 11 20 ')}
        'StopThread' {$stop=$stop.Replace(' 10 20 ',' 10 0 ')}
        'StopSite' {$stop=$stop.Replace('4d86b0','4d86f2')}
        'StopId' {$stop=$stop.Replace(' 0 10 ',' 5 10 ')}
        'Duplicate' {$stop+="`n"+$stop}
        'StopError' {$stop+="`nSyntax error"}
        'Oversized' {$stop+='x'*4096}
        'Budget' {$candidate.StopCount=128}
    }
    $refused=$false
    try{
        $request=Start-Ep11NativeObservationStop $candidate $stop
        $text=New-Ep11NativeContextFixture $snapshot $request.Nonce
        switch($mode){
            'ContextNonce' {$text=$text.Replace($request.Nonce,'0'*32)}
            'ContextPid' {$text=$text.Replace(' 10 20 ',' 11 20 ')}
            'ContextThread' {$text=$text.Replace(' 10 20 ',' 10 21 ')}
            'ContextSite' {$text=$text.Replace('4d86b0','4d86f2')}
            'ContextPartial' {$text=$text.TrimEnd("`n")}
            'ContextDuplicate' {$text+=$text}
            'ReplayStop' {
                Add-Ep11NativeObservationContext $candidate $text;Complete-Ep11NativeObservationStop $candidate
                $script:filterTrace.Reads=0;$null=Start-Ep11NativeObservationStop $candidate $stop
            }
        }
        Add-Ep11NativeObservationContext $candidate $text
    }catch{$refused=$true}
    if(-not $refused -or $candidate.Status -cne 'Faulted' -or $script:filterTrace.Reads -ne 0 -or -not $candidate.NeedsBreakpointCleanup){throw ('Native binding fault admitted or read: '+$mode)}
    $retry=$false;try{$null=Start-Ep11NativeObservationStop $candidate $stop}catch{$retry=$true};if(-not $retry){throw 'Faulted observer allowed retry.'};$refusals+=2
}
# Reborn: pending context and pending continuation cannot be exported as completed observations.
foreach($phase in @('AwaitContext','AwaitContinuation')){
    $candidate=New-Ep11NativeFixture;$snapshot=(New-Ep11SnapshotFixture)[0]
    $request=Start-Ep11NativeObservationStop $candidate (New-Ep11NativeStopFixture $candidate $snapshot)
    if($phase -ceq 'AwaitContinuation'){Add-Ep11NativeObservationContext $candidate (New-Ep11NativeContextFixture $snapshot $request.Nonce)}
    $refused=$false;try{$null=Complete-Ep11NativeObservationSession $candidate}catch{$refused=$true};if(-not $refused){throw 'Pending stop completed.'};$refusals++
}
# Reborn: a valid binding still cannot hide a capture failure; unrelated paths stay bounded and ignored rather than becoming evidence.
$candidate=New-Ep11NativeFixture 'FirstMismatch';$snapshot=(New-Ep11SnapshotFixture)[0]
$request=Start-Ep11NativeObservationStop $candidate (New-Ep11NativeStopFixture $candidate $snapshot)
Add-Ep11NativeObservationContext $candidate (New-Ep11NativeContextFixture $snapshot $request.Nonce)
if($candidate.Decoder.IgnoredContextResponses -ne 1 -or $script:filterTrace.Reads -ne 2){throw 'Native path mismatch exceeded its bounded reads.'};$positives++
$candidate=New-Ep11NativeFixture 'PathReadFailure'
$request=Start-Ep11NativeObservationStop $candidate (New-Ep11NativeStopFixture $candidate $snapshot)
$refused=$false;try{Add-Ep11NativeObservationContext $candidate (New-Ep11NativeContextFixture $snapshot $request.Nonce)}catch{$refused=$true}
if(-not $refused -or $candidate.Status -cne 'Faulted' -or -not $candidate.NeedsBreakpointCleanup -or $script:filterTrace.Reads -ne 2){throw 'Native capture failure bypassed cleanup/fault state.'};$refusals++
# Reborn: admission identity rejection must precede all range and decoder capture callbacks in the composed factory.
foreach($mode in @('Owner','Base','Hash')){
    $fault=$nativeIdentity.PSObject.Copy()
    switch($mode){'Owner' {$fault.ProcessId=17};'Base' {$fault.ModuleBase=0x500000};'Hash' {$fault.DiskImageSha256='0'*64}}
    $script:rebornAdmissionReadCalls=0;$script:filterTrace.Reads=0;$refused=$false
    try{$null=New-Ep11NativeObservationSession $nativePlan $fault $nativeInitial $nativeRead $probe (New-Ep11FilterReader)}catch{$refused=$true}
    if(-not $refused -or $script:rebornAdmissionReadCalls -ne 0 -or $script:filterTrace.Reads -ne 0){throw 'Native factory admission fault read memory.'};$refusals++
}
[pscustomobject]@{DetachedPositives=$positives;RefusalAndRetryChecks=$refusals;NativeDiskAdmissionConnected=$true;GameExecuted=$false;DebuggerExecuted=$false;ProcessQueried=$false;NativeRunEnabled=$false;LiveGameReadProven=$false}
