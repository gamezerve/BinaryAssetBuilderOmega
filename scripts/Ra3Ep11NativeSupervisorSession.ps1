# Reborn: compose supplied observation and cleanup lifecycles; actions are decisions only, never process/debugger operations.
. (Join-Path $PSScriptRoot 'Ra3Ep11NativeObservationSession.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep11NativeCleanupSession.ps1')

#-------------------------------------------------------------------------------------------------
<# Reborn: start a trusted in-process controller after native admission, with caller-supplied monotonic milliseconds and no execution backend. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11NativeSupervisorSession($Observer,[long]$NowMs) {
    if($NowMs -lt 0 -or $NowMs -gt [long]::MaxValue-60000 -or $Observer.Status -cne 'AwaitStop' -or
       $Observer.StopCount -ne 0 -or $Observer.Admission.Profile -cne 'NativeEp11' -or -not $Observer.Admission.CanIssueObservationContinue){throw 'Supervisor requires a fresh admitted native observer.'}
    @{Status='AwaitStop';Observer=$Observer;Cleanup=$null;Correlation=$null;Failure=$null;CleanupFailure=$null;
        LastMs=$NowMs;DeadlineMs=$NowMs+30000;ObservationDeadlineMs=$NowMs+30000;NextAction='WaitForOwnedStop';NeedsUserAction=$false;NativeRunEnabled=$false}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: reject backward/overflowing supplied clocks; an external backend must still bound OS waits and callbacks independently. #>
#-------------------------------------------------------------------------------------------------
function Update-Ep11NativeSupervisorClock($Session,[long]$NowMs) {
    if($NowMs -lt $Session.LastMs -or $NowMs -gt [long]::MaxValue-60000){throw 'Supervisor clock moved backward or exceeded bounds.'}
    $Session.LastMs=$NowMs
}

#-------------------------------------------------------------------------------------------------
<# Reborn: retain the first observation failure and classify unresolved cleanup without offering kill, resume or detach commands. #>
#-------------------------------------------------------------------------------------------------
function Set-Ep11NativeSupervisorUnresolved($Session,[string]$Reason,[switch]$CleanupOnly) {
    if(-not $CleanupOnly -and $null -eq $Session.Failure){$Session.Failure=$Reason}
    $Session.CleanupFailure=$Reason;$Session.Status='Unresolved';$Session.NextAction='UserAction';$Session.NeedsUserAction=$true
    $Session.Observer.Status='Faulted';$Session.Observer.Decoder.Status='Faulted'
}

#-------------------------------------------------------------------------------------------------
<# Reborn: switch from observation to known-stop cleanup while preserving failures; no known stop or changed identity becomes unresolved. #>
#-------------------------------------------------------------------------------------------------
function Start-Ep11NativeSupervisorCleanup($Session,$Identity,[string]$Reason='') {
    if($Session.Status -notin @('AwaitContext','AwaitContinuation')){throw 'Supervisor has no cleanup-eligible observation phase.'}
    if($Reason.Length -gt 0 -and $null -eq $Session.Failure){$Session.Failure=$Reason}
    try{
        $Session.Cleanup=New-Ep11NativeCleanupSession $Session.Observer $Identity
        $Session.Status='AwaitClear';$Session.DeadlineMs=$Session.LastMs+5000;$Session.NextAction='ClearOwnedBreakpoints'
    }catch{Set-Ep11NativeSupervisorUnresolved $Session $_.Exception.Message -CleanupOnly}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: bind a supplied owned stop before requesting registers; observation timeout while running cannot assume a safe cleanup stop. #>
#-------------------------------------------------------------------------------------------------
function Add-Ep11NativeSupervisorStop($Session,[string]$Text,[long]$NowMs) {
    if($Session.Status -cne 'AwaitStop'){throw 'Supervisor not awaiting a stop.'}
    try{
        Update-Ep11NativeSupervisorClock $Session $NowMs
        if($NowMs -ge $Session.ObservationDeadlineMs){throw 'Observation timeout without a verified stopped event.'}
        $request=Start-Ep11NativeObservationStop $Session.Observer $Text
        $Session.Status='AwaitContext';$Session.DeadlineMs=[Math]::Min($NowMs+5000,$Session.ObservationDeadlineMs);$Session.NextAction='ReadStoppedContext'
        $request
    }catch{Set-Ep11NativeSupervisorUnresolved $Session $_.Exception.Message}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: stop at the final reader return, validate the complete supplied chain there and enter cleanup without releasing the known stop. #>
#-------------------------------------------------------------------------------------------------
function Add-Ep11NativeSupervisorContext($Session,$Identity,[string]$Text,[long]$NowMs) {
    if($Session.Status -cne 'AwaitContext'){throw 'Supervisor not awaiting context.'}
    try{Update-Ep11NativeSupervisorClock $Session $NowMs}catch{Set-Ep11NativeSupervisorUnresolved $Session $_.Exception.Message;return}
    if($NowMs -ge $Session.DeadlineMs){Start-Ep11NativeSupervisorCleanup $Session $Identity 'Stopped context observation timed out.';return}
    try{
        # Reborn: validate fresh supplied lifetime/stop identity before the decoder's first memory callback at this event.
        $stopScope=@{Identity=$Session.Observer.AdmittedIdentity;StopThreadId=$Session.Observer.PendingStop.ThreadId;StopSiteAddress=$Session.Observer.PendingStop.SiteAddress}
        Assert-Ep11NativeCleanupIdentity $stopScope $Identity $false
        $previousEvents=$Session.Observer.Decoder.Events.Count
        Add-Ep11NativeObservationContext $Session.Observer $Text
        $decoder=$Session.Observer.Decoder
        # Reborn: ignored reader returns cannot complete a chain; require an actually captured ReaderExit event from this stop.
        if($decoder.Events.Count -gt $previousEvents -and $Session.Observer.PendingStop.SiteAddress -eq 0x4d98ab -and
           $decoder.Events[$decoder.Events.Count-1].Kind -ceq 'ReaderExit' -and $decoder.Threads.Count -eq 0){
            $Session.Correlation=Get-Ep11ConfigReadCorrelation $decoder.Events.ToArray() $decoder.ExpectedProcessId $decoder.ProbePath
            Start-Ep11NativeSupervisorCleanup $Session $Identity
        }else{$Session.Status='AwaitContinuation';$Session.NextAction='ContinueObservation'}
    }catch{Start-Ep11NativeSupervisorCleanup $Session $Identity $_.Exception.Message}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: record continuation only after capture succeeded, or recover a still-known stop on supplied timeout; send no g command. #>
#-------------------------------------------------------------------------------------------------
function Complete-Ep11NativeSupervisorStop($Session,$Identity,[long]$NowMs) {
    if($Session.Status -cne 'AwaitContinuation'){throw 'Supervisor continuation not eligible.'}
    try{Update-Ep11NativeSupervisorClock $Session $NowMs}catch{Set-Ep11NativeSupervisorUnresolved $Session $_.Exception.Message;return}
    if($NowMs -ge $Session.DeadlineMs){Start-Ep11NativeSupervisorCleanup $Session $Identity 'Stopped continuation timed out.';return}
    try{
        # Reborn: revalidate supplied identity at the decision boundary rather than relying on the earlier capture's identity.
        $stopScope=@{Identity=$Session.Observer.AdmittedIdentity;StopThreadId=$Session.Observer.PendingStop.ThreadId;StopSiteAddress=$Session.Observer.PendingStop.SiteAddress}
        Assert-Ep11NativeCleanupIdentity $stopScope $Identity $false
        Complete-Ep11NativeObservationStop $Session.Observer
        $Session.Status='AwaitStop';$Session.DeadlineMs=$Session.ObservationDeadlineMs;$Session.NextAction='WaitForOwnedStop'
    }catch{Start-Ep11NativeSupervisorCleanup $Session $Identity $_.Exception.Message}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: let an external bounded pump report elapsed time even when no response arrives; only a retained known stop permits cooperative recovery. #>
#-------------------------------------------------------------------------------------------------
function Test-Ep11NativeSupervisorDeadline($Session,$Identity,[long]$NowMs) {
    if($Session.Status -notin @('AwaitStop','AwaitContext','AwaitContinuation','AwaitClear','AwaitDetach')){throw 'Supervisor deadline not active.'}
    try{Update-Ep11NativeSupervisorClock $Session $NowMs}catch{Set-Ep11NativeSupervisorUnresolved $Session $_.Exception.Message -CleanupOnly:($Session.Status -in @('AwaitClear','AwaitDetach'));return}
    if($NowMs -lt $Session.DeadlineMs){return}
    if($Session.Status -in @('AwaitContext','AwaitContinuation')){Start-Ep11NativeSupervisorCleanup $Session $Identity 'Stopped observation deadline expired without a response.'}
    else{Set-Ep11NativeSupervisorUnresolved $Session ('Supervisor deadline expired in '+$Session.Status+'.') -CleanupOnly:($Session.Status -in @('AwaitClear','AwaitDetach'))}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: permit a detach decision only after identity, empty listing and all five pre-detach bytes validate inside their supplied deadline. #>
#-------------------------------------------------------------------------------------------------
function Add-Ep11NativeSupervisorClear($Session,$Identity,[string]$Text,[scriptblock]$ReadByte,[long]$NowMs) {
    if($Session.Status -cne 'AwaitClear'){throw 'Supervisor not awaiting cleanup clear.'}
    try{
        Update-Ep11NativeSupervisorClock $Session $NowMs
        if($NowMs -ge $Session.DeadlineMs){throw 'Cleanup clear timed out; detach is not eligible.'}
        Add-Ep11NativeCleanupClear $Session.Cleanup $Identity $Text $ReadByte
        $Session.Status='AwaitDetach';$Session.DeadlineMs=$NowMs+5000;$Session.NextAction='NormalDetach'
    }catch{Set-Ep11NativeSupervisorUnresolved $Session $_.Exception.Message -CleanupOnly}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: finish supplied normal detach and post-exit byte validation without converting cleanup success into observation success or live proof. #>
#-------------------------------------------------------------------------------------------------
function Complete-Ep11NativeSupervisorSession($Session,$Identity,[scriptblock]$ReadByte,[long]$NowMs) {
    if($Session.Status -cne 'AwaitDetach'){throw 'Supervisor not awaiting detach completion.'}
    try{
        Update-Ep11NativeSupervisorClock $Session $NowMs
        if($NowMs -ge $Session.DeadlineMs){throw 'Cleanup detach timed out.'}
        $cleanupResult=Complete-Ep11NativeCleanupSession $Session.Cleanup $Identity $ReadByte
        $Session.Status='Completed';$Session.NextAction='None'
        [pscustomobject]@{SuppliedObservationValidated=($null -ne $Session.Correlation -and $null -eq $Session.Failure);
            ObservationFailure=$Session.Failure;SuppliedCleanupValidated=$cleanupResult.SuppliedCleanupValidated;NeedsUserAction=$false;
            GameExecuted=$false;DebuggerExecuted=$false;LiveGameReadProven=$false;LiveCleanupProven=$false;NativeRunEnabled=$false}
    }catch{Set-Ep11NativeSupervisorUnresolved $Session $_.Exception.Message -CleanupOnly}
}
