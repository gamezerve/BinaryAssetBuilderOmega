# Reborn: drive the real shared pump/controller with private fragment tasks only; no debugger/game/process API.
[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
$null=. (Join-Path $PSScriptRoot 'Test-Ra3Ep11NativeSupervisorSession.ps1')
$null=. (Join-Path $PSScriptRoot 'Test-Ra3Ep11DebuggerOutputPump.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep11NativeOutputBridge.ps1')

#-------------------------------------------------------------------------------------------------
<# Reborn: create independent caller-owned test readers and a freshly admitted detached controller. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11BridgeFixture {
    $fixture=New-Ep11SupervisorFixture
    $fixture.Stdout=[RebornPumpTestReader]::new();$fixture.Stderr=[RebornPumpTestReader]::new()
    $fixture.Pump=New-Ep11DebuggerOutputPump $fixture.Stdout $fixture.Stderr
    $fixture
}

#-------------------------------------------------------------------------------------------------
<# Reborn: feed each character independently to exercise partial markers/prompts while forbidding captures before the terminal prompt completes. #>
#-------------------------------------------------------------------------------------------------
function Send-Ep11BridgeFragments($Fixture,$Window,[string]$Text,[long]$NowMs,[scriptblock]$ReadByte) {
    $before=$script:filterTrace.Reads
    for($index=0;$index -lt $Text.Length;$index++){
        $Fixture.Stdout.Supply($Text.Substring($index,1))
        $request=Update-Ep11NativeOutputWindow $Window $Fixture.Identity $ReadByte $NowMs
        if($index -lt $Text.Length-1 -and ($Window.Status -cne 'Pending' -or $script:filterTrace.Reads -ne $before)){throw 'Partial frame authorized capture/transition.'}
    }
    $request
}
$positives=0;$refusals=0;$fixture=New-Ep11BridgeFixture;$controller=$fixture.Controller;$now=1000L
foreach($snapshot in (New-Ep11SnapshotFixture)){
    $now+=10;$fixture.Identity.TargetSiteAddress=$snapshot.SiteAddress
    $window=New-Ep11NativeOutputWindow $controller $fixture.Pump
    $stop='0:000> '+(New-Ep11NativeStopFixture $controller.Observer $snapshot)+"`r`n0:000> "
    $request=Send-Ep11BridgeFragments $fixture $window $stop $now
    if($window.Status -cne 'Consumed' -or $controller.Status -cne 'AwaitContext'){throw 'Chunked stop was not dispatched.'}
    $window=New-Ep11NativeOutputWindow $controller $fixture.Pump
    $context='0:000> '+(New-Ep11NativeContextFixture $snapshot $request.Nonce)+"0:000> "
    $null=Send-Ep11BridgeFragments $fixture $window $context ($now+1)
    if($snapshot.SiteAddress -ne 0x4d98ab){Complete-Ep11NativeSupervisorStop $controller $fixture.Identity ($now+2)}
}
if($controller.Status -cne 'AwaitClear' -or $null -eq $controller.Observer.PendingStop){throw 'Bridge released final stop.'}
$script:cleanupTrace=@{Reads=0};$reader=New-Ep11CleanupReader @{Session=$controller.Cleanup}
$window=New-Ep11NativeOutputWindow $controller $fixture.Pump
$null=Send-Ep11BridgeFragments $fixture $window ("0:000> REBORN_CLEAR "+$controller.Cleanup.Nonce+"`n0:000> ") ($now+3) $reader
if($controller.Status -cne 'AwaitDetach' -or $script:cleanupTrace.Reads -ne 5){throw 'Bridge clear was not validated.'}
$post=$fixture.Identity.PSObject.Copy();$post.DebuggerHasExited=$true;$post|Add-Member DebuggerExitCode ([int]0)
$result=Complete-Ep11NativeSupervisorSession $controller $post $reader ($now+4)
if(-not $result.SuppliedObservationValidated -or -not $result.SuppliedCleanupValidated -or $script:cleanupTrace.Reads -ne 10 -or $fixture.Stdout.Disposed -or $fixture.Stderr.Disposed){throw 'Bridge full supplied chain failed.'};$positives++
$refused=$false;try{$null=Update-Ep11NativeOutputWindow $window $fixture.Identity $reader ($now+5)}catch{$refused=$true};if(-not $refused){throw 'Consumed window replayed.'};$refusals++
# Reborn: no-response timeout at a verified stop interrupts the old window and opens the separate cooperative cleanup decision.
$fixture=New-Ep11BridgeFixture;$controller=$fixture.Controller
$window=New-Ep11NativeOutputWindow $controller $fixture.Pump
$fixture.Stdout.Supply((New-Ep11NativeStopFixture $controller.Observer $fixture.Snapshot)+"`n0:000> ")
$request=Update-Ep11NativeOutputWindow $window $fixture.Identity $null 1001
$window=New-Ep11NativeOutputWindow $controller $fixture.Pump
$null=Update-Ep11NativeOutputWindow $window $fixture.Identity $null 6001
if($window.Status -cne 'Interrupted' -or $controller.Status -cne 'AwaitClear' -or $script:filterTrace.Reads -ne 0 -or $controller.OutputWindowActive){throw 'Bridge timeout did not release response ownership for cleanup.'};$positives++
foreach($mode in @('WrongNonce','DuplicateStop','UnknownLine','StopDiagnostic','Oversized','Stderr','Eof','ReadFault','WrongContextNonce','ContextDuplicate','ClearListing','Overlap')){
    $fixture=New-Ep11BridgeFixture;$controller=$fixture.Controller;$script:cleanupTrace=@{Reads=0}
    $window=New-Ep11NativeOutputWindow $controller $fixture.Pump
    $stop=New-Ep11NativeStopFixture $controller.Observer $fixture.Snapshot
    if($mode -ceq 'Overlap'){
        $refused=$false;try{$null=New-Ep11NativeOutputWindow $controller $fixture.Pump}catch{$refused=$true}
        if(-not $refused -or $fixture.Stdout.ReadCalls -ne 1){throw 'Overlapping window started another read.'};$refusals++;continue
    }
    $text=$stop+"`n0:000> "
    if($mode -in @('WrongContextNonce','ContextDuplicate','ClearListing')){
        $fixture.Stdout.Supply($text);$request=Update-Ep11NativeOutputWindow $window $fixture.Identity $null 1001
        $window=New-Ep11NativeOutputWindow $controller $fixture.Pump
        $text=New-Ep11NativeContextFixture $fixture.Snapshot $request.Nonce
        if($mode -ceq 'WrongContextNonce'){$text=$text.Replace($request.Nonce,'0'*32)}
        if($mode -ceq 'ContextDuplicate'){$text+=$text}
        if($mode -ceq 'ClearListing'){
            $fixture.Stdout.Supply('unexpected'+"`n0:000> ");$null=Update-Ep11NativeOutputWindow $window $fixture.Identity $null 1002
            $window=New-Ep11NativeOutputWindow $controller $fixture.Pump
            $text=" 0 e 004d86b0 0001 (0001)`nREBORN_CLEAR "+$controller.Cleanup.Nonce+"`n"
        }
        $text+="0:000> "
    }
    switch($mode){
        'WrongNonce' {$text=$text.Replace($controller.Observer.StopNonce,'0'*32)}
        'DuplicateStop' {$text=$stop+"`n"+$stop+"`n0:000> "}
        'UnknownLine' {$text='unexpected'+"`n"+$text}
        'StopDiagnostic' {$text=$stop+"`neax=ffffffff ebx=0098eff8 ecx=0098eff8`n0:000> "}
        'Oversized' {$text='x'*4097}
        'Stderr' {$fixture.Stderr.Supply('private error')}
        'Eof' {$fixture.Stdout.Supply('')}
        'ReadFault' {$fixture.Stdout.Fail()}
    }
    if($mode -notin @('Eof','ReadFault')){
        # Reborn: large output is fed in legal reader fragments; only the response window, not the reader buffer, exceeds its limit.
        for($offset=0;$offset -lt $text.Length;$offset+=1024){
            $length=[Math]::Min(1024,$text.Length-$offset);$fixture.Stdout.Supply($text.Substring($offset,$length))
            $null=Update-Ep11NativeOutputWindow $window $fixture.Identity $null 1003
        }
    }else{$null=Update-Ep11NativeOutputWindow $window $fixture.Identity $null 1003}
    $expected=if($mode -in @('WrongContextNonce','ContextDuplicate')){'AwaitClear'}else{'Unresolved'}
    if($controller.Status -cne $expected -or $window.Status -cne 'Faulted' -or $script:filterTrace.Reads -ne 0 -or $script:cleanupTrace.Reads -ne 0 -or $fixture.Stdout.Disposed -or $fixture.Stderr.Disposed){throw ('Bridge refusal bypassed capture/ownership boundary: '+$mode)}
    $refused=$false;try{$null=Update-Ep11NativeOutputWindow $window $fixture.Identity $null 1004}catch{$refused=$true};if(-not $refused){throw 'Faulted bridge replayed.'};$refusals+=2
}
# Reborn: replay exact context/clear format excerpts from the prior owned-helper recording; this validates framing only, never native identity/read proof.
$recordedContext="0:000> REBORN_CONTEXT aa3506393e2b4cde8e14030165ee2db1 c150 7b20 77969ee0 ffffffff 98eff8 77d000 98eff8 98ef94`r`nREBORN_CONTEXT_END aa3506393e2b4cde8e14030165ee2db1`r`n0:000> "
$frame=Get-Ep11NativeOutputFrame $recordedContext 'AwaitContext' 'aa3506393e2b4cde8e14030165ee2db1'
$record=Get-Ep11ConfigContextRecord (Get-Ep11CompletedContextWindow $frame.Text 'aa3506393e2b4cde8e14030165ee2db1') 'aa3506393e2b4cde8e14030165ee2db1'
if($record.ProcessId -ne 49488 -or $record.ThreadId -ne 31520 -or $record.SiteAddress -ne 0x77969ee0){throw 'Recorded helper context format changed.'};$positives++
$frame=Get-Ep11NativeOutputFrame "0:000> REBORN_CLEAR be924cbdf3a94dd5905e2fc4ffc61d3d`r`n0:000> " 'AwaitClear' 'be924cbdf3a94dd5905e2fc4ffc61d3d'
Assert-Ep11HelperBreakpointCleared $frame.Text 'be924cbdf3a94dd5905e2fc4ffc61d3d';$positives++
[pscustomobject]@{DetachedPositives=$positives;RefusalAndRetryChecks=$refusals;FinalStopRetainedForCleanup=$true;GameExecuted=$false;DebuggerExecuted=$false;NativeRunEnabled=$false;LiveGameReadProven=$false}
