# Reborn: validate supplied five-site cooperative cleanup evidence; no debugger I/O, process operations, memory writes or forced-exit fallback.
. (Join-Path $PSScriptRoot 'Ra3Ep11ConfigDebuggerTransport.ps1')

#-------------------------------------------------------------------------------------------------
<# Reborn: compare retained supplied identity with admission before any cleanup byte callback; post-detach additionally requires normal debugger exit. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11NativeCleanupIdentity($Session,$Identity,[bool]$AfterDetach) {
    foreach($field in @('ProcessId','DebuggerProcessId','CreationUtcTicks','DebuggerCreationUtcTicks','ModuleBase','ModuleSize','ExpectedExecutablePath','ActualExecutablePath','DiskImageSha256')){
        if($Identity.$field -cne $Session.Identity.$field){throw 'Cleanup identity changed after admission.'}
    }
    if($Identity.TargetHasExited -isnot [bool] -or $Identity.TargetHasExited -or $Identity.DebuggerHasExited -isnot [bool] -or
       $Identity.DebuggerHasExited -ne $AfterDetach){throw 'Cleanup target/debugger lifetime differs.'}
    if($AfterDetach){
        if($Identity.DebuggerExitCode -isnot [int] -or $Identity.DebuggerExitCode -ne 0){throw 'Cleanup debugger did not exit normally.'}
    }elseif($Identity.TargetStopped -isnot [bool] -or -not $Identity.TargetStopped -or
       $Identity.TargetThreadId -ne $Session.StopThreadId -or $Identity.TargetSiteAddress -ne $Session.StopSiteAddress){throw 'Cleanup requires the supplied matching known stop.'}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: freeze a known-stop observer for cleanup, preserving its original failure status and the admission-captured five bytes. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11NativeCleanupSession($Observer,$Identity) {
    if($null -eq $Observer -or $Observer.Status -notin @('AwaitContext','AwaitContinuation','Faulted') -or
       $null -eq $Observer.PendingStop -or -not $Observer.NeedsBreakpointCleanup -or $Observer.OriginalSiteBytes -isnot [byte[]] -or
       $Observer.OriginalSiteBytes.Length -ne 5 -or $Observer.Sites.Count -ne 5){throw 'Cleanup has no known pending native stop.'}
    $session=@{Status='AwaitClear';Identity=$Observer.AdmittedIdentity.PSObject.Copy();Sites=@($Observer.Sites.Clone());OriginalBytes=[byte[]]$Observer.OriginalSiteBytes.Clone();
        Nonce=[guid]::NewGuid().ToString('N');StopThreadId=$Observer.PendingStop.ThreadId;StopSiteAddress=$Observer.PendingStop.SiteAddress;
        ObserverStatus=$Observer.Status;SuppliedCleanupValidated=$false;NeedsUserAction=$false;Failure=$null}
    Assert-Ep11NativeCleanupIdentity $session $Identity $false
    # Reborn: terminate observation, not the process; the external supervisor still owns all actual cleanup actions.
    $Observer.Status='Cleanup';$Observer.Decoder.Status='Faulted'
    $session
}

#-------------------------------------------------------------------------------------------------
<# Reborn: read exactly one original byte at each of the five admitted sites, refusing a short/failed/misaddressed transfer immediately. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11NativeCleanupBytes($Session,[scriptblock]$ReadByte) {
    if($null -eq $ReadByte){throw 'Cleanup byte adapter absent.'}
    for($index=0;$index -lt 5;$index++){
        $bytes=Read-Ep11SnapshotBytes $ReadByte $Session.Sites[$index] 1
        if($bytes[0] -ne $Session.OriginalBytes[$index]){throw 'Cleanup original site byte differs.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: require a terminated nonce acknowledgement and empty breakpoint list plus all original bytes before accepting detach eligibility. #>
#-------------------------------------------------------------------------------------------------
function Add-Ep11NativeCleanupClear($Session,$Identity,[string]$Text,[scriptblock]$ReadByte) {
    if($null -eq $Session -or $Session.Status -cne 'AwaitClear'){throw 'Cleanup not awaiting a clear acknowledgement.'}
    try{
        Assert-Ep11NativeCleanupIdentity $Session $Identity $false
        if($Text -cnotmatch ('(?m)^REBORN_CLEAR '+$Session.Nonce+'\r?\n')){throw 'Cleanup acknowledgement not terminated.'}
        Assert-Ep11HelperBreakpointCleared $Text $Session.Nonce
        Assert-Ep11NativeCleanupBytes $Session $ReadByte
        $Session.Status='AwaitDetach'
    }catch{$Session.Status='Faulted';$Session.Failure=$_.Exception.Message;$Session.NeedsUserAction=$true;throw}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: after supplied normal debugger exit, require the retained target still live and repeat all five byte checks without claiming actual OS proof. #>
#-------------------------------------------------------------------------------------------------
function Complete-Ep11NativeCleanupSession($Session,$Identity,[scriptblock]$ReadByte) {
    if($null -eq $Session -or $Session.Status -cne 'AwaitDetach'){throw 'Cleanup not awaiting detach evidence.'}
    try{
        Assert-Ep11NativeCleanupIdentity $Session $Identity $true
        Assert-Ep11NativeCleanupBytes $Session $ReadByte
        $Session.Status='Validated';$Session.SuppliedCleanupValidated=$true
        [pscustomobject]@{SuppliedCleanupValidated=$true;OriginalObservationStatus=$Session.ObserverStatus;SiteCount=5;
            LiveCleanupProven=$false;TargetCompletionProven=$false;NativeRunEnabled=$false;CommandsSent=$false;ForcedExitAllowed=$false}
    }catch{$Session.Status='Faulted';$Session.Failure=$_.Exception.Message;$Session.NeedsUserAction=$true;throw}
}
