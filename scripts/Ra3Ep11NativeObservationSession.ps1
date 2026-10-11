# Reborn: bind supplied native stops to admission, nonce context and scoped captures; no launch, attach, breakpoint installation or continuation I/O.
. (Join-Path $PSScriptRoot 'Ra3Ep11SupervisorAdmissionGate.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep11ConfigDebuggerTransport.ps1')

#-------------------------------------------------------------------------------------------------
<# Reborn: run the shared native gate and copy fixed site identities before creating an exact-probe decoder. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11NativeObservationSession($Plan,$Identity,[string]$InitialText,[scriptblock]$ReadRange,[string]$ProbePath,[scriptblock]$ReadCapture) {
    if($Plan.Kind -cne 'NativeEp11'){throw 'Observation requires the native admission profile.'}
    $decision=Get-Ep11SupervisorAdmissionDecision $Plan $Identity $InitialText $ReadRange
    $sites=@(0x4d86b0L,0x4d86f2L,0x4d8725L,0x4d9040L,0x4d98abL)
    foreach($site in $sites){
        if(@($Plan.Ranges|Where-Object {$_.Address -le $site -and $site -lt [long]$_.Address+$_.Length}).Count -ne 1){throw 'Observation site not uniquely covered by admitted ranges.'}
    }
    $decoder=New-Ep11ConfigSnapshotSession $Identity.ProcessId $ProbePath $ReadCapture -FilterProbeEntries
    return @{Status='AwaitStop';Decoder=$decoder;Sites=$sites;StopNonce=[guid]::NewGuid().ToString('N');StopCount=0;PendingStop=$null;
        Admission=$decision;SuppliedStopContextPairs=0;NeedsBreakpointCleanup=$false;NativeRunEnabled=$false}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: accept one bounded nonce-bound fixed-site stop and prepare its independent register request, without reading memory or sending commands. #>
#-------------------------------------------------------------------------------------------------
function Start-Ep11NativeObservationStop($Session,[string]$Text) {
    if($null -eq $Session -or $Session.Status -cne 'AwaitStop'){throw 'Observation not awaiting a stop.'}
    try{
        $Session.NeedsBreakpointCleanup=$true
        if($Session.StopCount -ge 128 -or $Text.Length -gt 4096){throw 'Native stop envelope exceeds bounds.'}
        $pattern='^REBORN_NATIVE_STOP '+$Session.StopNonce+' ([0-4]) ([0-9a-fA-F]{1,8}) ([0-9a-fA-F]{1,8}) ([0-9a-fA-F]{1,8})$'
        $records=@(($Text -split '\r?\n')|Where-Object {$_ -cmatch '^REBORN_'})
        if($records.Count -ne 1 -or $records[0] -cnotmatch $pattern -or $Text -match '(?i)syntax error|unable to|memory access error|invalid command'){throw 'Native stop marker missing, stale or ambiguous.'}
        $id=[int]$Matches[1];$owner=[long][Convert]::ToUInt32($Matches[2],16);$thread=[long][Convert]::ToUInt32($Matches[3],16);$site=[long][Convert]::ToUInt32($Matches[4],16)
        if($owner -ne $Session.Decoder.ExpectedProcessId -or $thread -eq 0 -or $site -ne $Session.Sites[$id]){throw 'Native stop owner/thread/site differs.'}
        $Session.PendingStop=[pscustomobject]@{ProcessId=$owner;ThreadId=$thread;SiteAddress=$site;BreakpointId=$id}
        $request=New-Ep11ConfigContextRequest $Session.Decoder
        $Session.StopCount++;$Session.Status='AwaitContext'
        $request
    }catch{$Session.Status='Faulted';$Session.Decoder.Status='Faulted';throw}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: bind an independent completed context window to the pending stop before any exact-probe path/object/buffer captures. #>
#-------------------------------------------------------------------------------------------------
function Add-Ep11NativeObservationContext($Session,[string]$Text) {
    if($null -eq $Session -or $Session.Status -cne 'AwaitContext'){throw 'Observation not awaiting context.'}
    try{
        $window=Get-Ep11CompletedContextWindow $Text $Session.Decoder.PendingContext
        $record=Get-Ep11ConfigContextRecord $window $Session.Decoder.PendingContext
        foreach($field in @('ProcessId','ThreadId','SiteAddress')){
            if($record.$field -ne $Session.PendingStop.$field){throw 'Native context does not match its stopped event.'}
        }
        Add-Ep11ConfigDebuggerResponse $Session.Decoder $window
        $Session.SuppliedStopContextPairs++;$Session.Status='AwaitContinuation'
    }catch{$Session.Status='Faulted';$Session.Decoder.Status='Faulted';throw}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: record the caller's continuation boundary only; rotate the next stop nonce to reject replay and never send g or imply OS execution. #>
#-------------------------------------------------------------------------------------------------
function Complete-Ep11NativeObservationStop($Session) {
    if($null -eq $Session -or $Session.Status -cne 'AwaitContinuation'){throw 'Observation stop cannot be released.'}
    $Session.PendingStop=$null;$Session.StopNonce=[guid]::NewGuid().ToString('N');$Session.Status='AwaitStop'
}

#-------------------------------------------------------------------------------------------------
<# Reborn: export a completed supplied engine chain separately from pending breakpoint cleanup and actual game-read proof. #>
#-------------------------------------------------------------------------------------------------
function Complete-Ep11NativeObservationSession($Session) {
    if($null -eq $Session -or $Session.Status -cne 'AwaitStop'){throw 'Observation has a pending or failed stop.'}
    try{
        $events=@(Complete-Ep11ConfigSnapshotSession $Session.Decoder)
        $correlation=Get-Ep11ConfigReadCorrelation $events $Session.Decoder.ExpectedProcessId $Session.Decoder.ProbePath
        $Session.Status='Closed'
        [pscustomobject]@{SuppliedEngineChainValidated=$correlation.SuppliedEngineChainValidated;CompletedInvocationCount=$correlation.CompletedInvocationCount;
            SuppliedStopContextPairs=$Session.SuppliedStopContextPairs;IgnoredStops=$Session.Decoder.IgnoredContextResponses;
            NeedsBreakpointCleanup=$Session.NeedsBreakpointCleanup;NativeRunEnabled=$false;GameExecuted=$false;LiveGameReadProven=$false;DebuggerCommandsSent=$false}
    }catch{$Session.Status='Faulted';$Session.Decoder.Status='Faulted';throw}
}
