# Reborn: compose the full supplied lifecycle with private records and monotonic test times; no execution backend or OS process access.
[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
$null=. (Join-Path $PSScriptRoot 'Test-Ra3Ep11NativeCleanupSession.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep11NativeSupervisorSession.ps1')

#-------------------------------------------------------------------------------------------------
<# Reborn: create a fresh controller and one independent supplied stopped identity bound to the fixture entry. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11SupervisorFixture {
    $observer=New-Ep11NativeFixture;$controller=New-Ep11NativeSupervisorSession $observer 1000
    $identity=$nativeIdentity.PSObject.Copy();$identity|Add-Member TargetStopped $true
    $identity|Add-Member TargetThreadId 32;$identity|Add-Member TargetSiteAddress 0x4d86b0L
    @{Controller=$controller;Identity=$identity;Snapshot=(New-Ep11SnapshotFixture)[0]}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: finish only a known supplied cleanup through its real five-site validators; no detach command is issued. #>
#-------------------------------------------------------------------------------------------------
function Complete-Ep11SupervisorFixture($Fixture,[long]$NowMs) {
    $controller=$Fixture.Controller;$script:cleanupTrace=@{Reads=0}
    $reader=New-Ep11CleanupReader @{Session=$controller.Cleanup}
    Add-Ep11NativeSupervisorClear $controller $Fixture.Identity ("REBORN_CLEAR "+$controller.Cleanup.Nonce+"`n") $reader $NowMs
    $post=$Fixture.Identity.PSObject.Copy();$post.DebuggerHasExited=$true;$post|Add-Member DebuggerExitCode ([int]0)
    Complete-Ep11NativeSupervisorSession $controller $post $reader ($NowMs+1)
}
$positives=0;$refusals=0
$fixture=New-Ep11SupervisorFixture;$controller=$fixture.Controller;$now=1000L
foreach($snapshot in (New-Ep11SnapshotFixture)){
    $now+=10;$fixture.Identity.TargetSiteAddress=$snapshot.SiteAddress
    $request=Add-Ep11NativeSupervisorStop $controller (New-Ep11NativeStopFixture $controller.Observer $snapshot) $now
    Add-Ep11NativeSupervisorContext $controller $fixture.Identity (New-Ep11NativeContextFixture $snapshot $request.Nonce) ($now+1)
    if($snapshot.SiteAddress -eq 0x4d98ab){
        if($controller.Status -cne 'AwaitClear' -or $null -eq $controller.Observer.PendingStop){throw 'Final stop released before cleanup.'}
        $refused=$false;try{Complete-Ep11NativeSupervisorStop $controller $fixture.Identity ($now+2)}catch{$refused=$true};if(-not $refused){throw 'Final stop allowed continuation.'};$refusals++
    }else{Complete-Ep11NativeSupervisorStop $controller $fixture.Identity ($now+2)}
}
$result=Complete-Ep11SupervisorFixture $fixture ($now+3)
if(-not $result.SuppliedObservationValidated -or -not $result.SuppliedCleanupValidated -or $script:cleanupTrace.Reads -ne 10 -or
   $controller.Status -cne 'Completed' -or $result.LiveGameReadProven){throw 'Full supplied supervisor chain failed.'};$positives++
foreach($mode in @('WrongContextThread','ContextTimeout')){
    $fixture=New-Ep11SupervisorFixture;$controller=$fixture.Controller
    $request=Add-Ep11NativeSupervisorStop $controller (New-Ep11NativeStopFixture $controller.Observer $fixture.Snapshot) 1001
    $text=New-Ep11NativeContextFixture $fixture.Snapshot $request.Nonce;$time=1002L
    if($mode -ceq 'WrongContextThread'){$text=$text.Replace(' 10 20 ',' 10 21 ')}else{$time=6001L}
    Add-Ep11NativeSupervisorContext $controller $fixture.Identity $text $time
    if($controller.Status -cne 'AwaitClear' -or $null -eq $controller.Failure -or $script:filterTrace.Reads -ne 0){throw 'Known-stop failure did not choose capture-free cleanup.'}
    $failure=$controller.Failure;$result=Complete-Ep11SupervisorFixture $fixture ($time+1)
    if($result.SuppliedObservationValidated -or -not $result.SuppliedCleanupValidated -or $result.ObservationFailure -cne $failure){throw 'Cleanup erased observation failure.'};$positives++
}
foreach($mode in @('UnknownStop','RunningTimeout','BackwardClock','ChangedPid','ChangedStop','ClearTimeout','ClearListing','ClearByte','DetachTimeout','DebuggerExit','DetachByte')){
    $fixture=New-Ep11SupervisorFixture;$controller=$fixture.Controller;$script:cleanupTrace=@{Reads=0}
    if($mode -in @('UnknownStop','RunningTimeout')){
        $stop=New-Ep11NativeStopFixture $controller.Observer $fixture.Snapshot;$time=1001L
        if($mode -ceq 'UnknownStop'){$stop=$stop.Replace('4d86b0','4d86b1')}else{$time=31000L}
        $null=Add-Ep11NativeSupervisorStop $controller $stop $time
    }else{
        $request=Add-Ep11NativeSupervisorStop $controller (New-Ep11NativeStopFixture $controller.Observer $fixture.Snapshot) 1001
        $text=New-Ep11NativeContextFixture $fixture.Snapshot $request.Nonce
        if($mode -in @('BackwardClock','ChangedPid','ChangedStop')){
            $time=1002L
            switch($mode){'BackwardClock' {$time=1000};'ChangedPid' {$fixture.Identity.ProcessId++};'ChangedStop' {$fixture.Identity.TargetSiteAddress++}}
            Add-Ep11NativeSupervisorContext $controller $fixture.Identity $text $time
        }else{
            Add-Ep11NativeSupervisorContext $controller $fixture.Identity $text.Replace(' 10 20 ',' 10 21 ') 1002
            $clear="REBORN_CLEAR "+$controller.Cleanup.Nonce+"`n";$time=1003L;$readMode='Valid'
            if($mode -ceq 'ClearTimeout'){$time=6002}
            if($mode -ceq 'ClearListing'){$clear+=' 0 e 004d86b0 0001 (0001)'}
            if($mode -ceq 'ClearByte'){$readMode='Trap'}
            Add-Ep11NativeSupervisorClear $controller $fixture.Identity $clear (New-Ep11CleanupReader @{Session=$controller.Cleanup} $readMode) $time
            if($mode -like 'Detach*' -or $mode -ceq 'DebuggerExit'){
                $script:cleanupTrace.Reads=0;$post=$fixture.Identity.PSObject.Copy();$post.DebuggerHasExited=$true;$post|Add-Member DebuggerExitCode ([int]0)
                $time=1004L;$readMode='Valid'
                if($mode -ceq 'DetachTimeout'){$time=6003}
                if($mode -ceq 'DebuggerExit'){$post.DebuggerExitCode=-1}
                if($mode -ceq 'DetachByte'){$readMode='Trap'}
                $null=Complete-Ep11NativeSupervisorSession $controller $post (New-Ep11CleanupReader @{Session=$controller.Cleanup} $readMode) $time
            }
        }
    }
    $expectedReads=if($mode -in @('ClearByte','DetachByte')){1}else{0}
    if($controller.Status -cne 'Unresolved' -or -not $controller.NeedsUserAction -or $controller.NextAction -cne 'UserAction' -or
       $script:cleanupTrace.Reads -ne $expectedReads -or ($mode -in @('ChangedPid','ChangedStop') -and $script:filterTrace.Reads -ne 0)){throw ('Supervisor fault bypassed boundary: '+$mode)}
    $before=$script:cleanupTrace.Reads;$refused=$false
    try{Complete-Ep11NativeSupervisorStop $controller $fixture.Identity 1005}catch{$refused=$true}
    if(-not $refused -or $script:cleanupTrace.Reads -ne $before){throw 'Unresolved controller allowed continuation.'};$refusals+=2
}
# Reborn: deadlines must be enforceable without receiving a context or cleanup response; a stopped timeout can still recover cooperatively.
$fixture=New-Ep11SupervisorFixture;$controller=$fixture.Controller
$null=Add-Ep11NativeSupervisorStop $controller (New-Ep11NativeStopFixture $controller.Observer $fixture.Snapshot) 1001
Test-Ep11NativeSupervisorDeadline $controller $fixture.Identity 6001
if($controller.Status -cne 'AwaitClear' -or $script:filterTrace.Reads -ne 0){throw 'No-response stopped timeout did not enter cleanup.'}
$result=Complete-Ep11SupervisorFixture $fixture 6002
if(-not $result.SuppliedCleanupValidated -or $result.SuppliedObservationValidated){throw 'No-response cleanup proved observation.'};$positives++
foreach($phase in @('AwaitStop','AwaitClear')){
    $fixture=New-Ep11SupervisorFixture;$controller=$fixture.Controller;$time=31000L
    if($phase -ceq 'AwaitClear'){
        $null=Add-Ep11NativeSupervisorStop $controller (New-Ep11NativeStopFixture $controller.Observer $fixture.Snapshot) 1001
        Test-Ep11NativeSupervisorDeadline $controller $fixture.Identity 6001;$time=11001
    }
    Test-Ep11NativeSupervisorDeadline $controller $fixture.Identity $time
    if($controller.Status -cne 'Unresolved' -or -not $controller.NeedsUserAction){throw 'Missing response bypassed timeout.'}
    $refused=$false;try{Test-Ep11NativeSupervisorDeadline $controller $fixture.Identity ($time+1)}catch{$refused=$true};if(-not $refused){throw 'Terminal supervisor accepted deadline replay.'};$refusals+=2
}
# Reborn: a cleanup-only failure after a valid chain must not be mislabeled as an observation failure, and still cannot complete the controller.
$fixture=New-Ep11SupervisorFixture;$controller=$fixture.Controller;$now=1000L
foreach($snapshot in (New-Ep11SnapshotFixture)){
    $now+=10;$fixture.Identity.TargetSiteAddress=$snapshot.SiteAddress
    $request=Add-Ep11NativeSupervisorStop $controller (New-Ep11NativeStopFixture $controller.Observer $snapshot) $now
    Add-Ep11NativeSupervisorContext $controller $fixture.Identity (New-Ep11NativeContextFixture $snapshot $request.Nonce) ($now+1)
    if($snapshot.SiteAddress -ne 0x4d98ab){Complete-Ep11NativeSupervisorStop $controller $fixture.Identity ($now+2)}
}
$script:cleanupTrace=@{Reads=0}
Add-Ep11NativeSupervisorClear $controller $fixture.Identity 'Syntax error' (New-Ep11CleanupReader @{Session=$controller.Cleanup}) ($now+3)
if($controller.Status -cne 'Unresolved' -or $null -ne $controller.Failure -or $null -eq $controller.CleanupFailure -or
   -not $controller.Correlation.SuppliedEngineChainValidated -or $script:cleanupTrace.Reads -ne 0){throw 'Cleanup-only failure polluted observation result.'};$refusals++
[pscustomobject]@{DetachedPositives=$positives;RefusalAndRetryChecks=$refusals;FinalStopRetainedForCleanup=$true;GameExecuted=$false;DebuggerExecuted=$false;NativeRunEnabled=$false;LiveGameReadProven=$false}
