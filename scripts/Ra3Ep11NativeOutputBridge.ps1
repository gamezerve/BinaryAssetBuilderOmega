# Reborn: bind bounded completed stdout windows to native supervisor phases; no launch, command writes, detach, disposal or waits.
. (Join-Path $PSScriptRoot 'Ra3Ep11DebuggerOutputPump.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep11NativeSupervisorSession.ps1')

#-------------------------------------------------------------------------------------------------
<# Reborn: scope a single future response before its external command/continuation, freezing phase, nonce, stream and start cursor. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11NativeOutputWindow($Controller,[object[]]$Pump) {
    if($Controller.Status -notin @('AwaitStop','AwaitContext','AwaitClear') -or $Controller.OutputWindowActive -or
       $Pump.Count -ne 2 -or @($Pump|Where-Object {$_.Faulted -or $_.Eof}).Count -gt 0 -or $Pump[1].Text.Length -ne 0){throw 'Output window scope unavailable.'}
    $nonce=switch($Controller.Status){'AwaitStop' {$Controller.Observer.StopNonce};'AwaitContext' {$Controller.Observer.Decoder.PendingContext};'AwaitClear' {$Controller.Cleanup.Nonce}}
    if($nonce -cnotmatch '^[0-9a-f]{32}$'){throw 'Output window nonce unavailable.'}
    $Controller.OutputWindowActive=$true
    @{Status='Pending';Controller=$Controller;Pump=$Pump;Phase=$Controller.Status;Nonce=$nonce;Start=$Pump[0].Text.Length;Failure=$null}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: require a complete terminal CDB prompt and strict phase markers; remove only one recognized prompt prefix immediately before a marker. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11NativeOutputFrame([string]$Text,[string]$Phase,[string]$Nonce) {
    if($Text.Length -gt 4096 -or $Nonce -cnotmatch '^[0-9a-f]{32}$'){throw 'Output response window exceeds bounds.'}
    # Reborn: exact calibrated prompt subset ends with a space; partial prompt/CRLF chunks cannot authorize captures.
    $prompt=[regex]::Match($Text,'(?m)^[0-9a-fA-F]{1,8}:[0-9a-fA-F]{1,8}> (?:\r?\n)?\z')
    if(-not $prompt.Success){return}
    $content=$Text.Substring(0,$prompt.Index)
    $content=[regex]::Replace($content,'(?m)^[0-9a-fA-F]{1,8}:[0-9a-fA-F]{1,8}> (?=REBORN_)','')
    if([string]::IsNullOrWhiteSpace($content)){return}
    foreach($line in ($content -split '\r?\n')){
        if([string]::IsNullOrWhiteSpace($line)){continue}
        $accepted=switch($Phase){
            'AwaitStop' {$line -cmatch ('^REBORN_NATIVE_STOP '+$Nonce+' [0-4](?: [0-9a-fA-F]{1,8}){3}$')}
            'AwaitContext' {$line -cmatch ('^REBORN_CONTEXT '+$Nonce+'(?: [0-9a-fA-F]{1,8}){8}$') -or $line -ceq ('REBORN_CONTEXT_END '+$Nonce)}
            'AwaitClear' {$line -ceq ('REBORN_CLEAR '+$Nonce)}
            default {$false}
        }
        if(-not $accepted){throw 'Unexpected response line, phase or nonce.'}
    }
    switch($Phase){
        'AwaitStop' {if(@(($content -split '\r?\n')|Where-Object {$_ -cmatch '^REBORN_NATIVE_STOP '}).Count -ne 1){throw 'Stop response duplicated or missing.'}}
        'AwaitContext' {$null=Get-Ep11CompletedContextWindow $content $Nonce}
        'AwaitClear' {Assert-Ep11HelperBreakpointCleared $content $Nonce}
    }
    [pscustomobject]@{Text=$content}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: pump available chunks and deadlines, dispatch one complete phase window, and classify transport loss separately from recoverable known-stop protocol errors. #>
#-------------------------------------------------------------------------------------------------
function Update-Ep11NativeOutputWindow($Window,$Identity,[scriptblock]$ReadByte,[long]$NowMs) {
    if($Window.Status -cne 'Pending'){throw 'Output window already consumed or failed.'}
    $controller=$Window.Controller;$cleanupOnly=$Window.Phase -ceq 'AwaitClear'
    try{
        Update-Ep11DebuggerOutputPump $Window.Pump
        if($Window.Pump[1].Text.Length -ne 0 -or @($Window.Pump|Where-Object {$_.Eof}).Count -gt 0){throw 'Output transport stderr or EOF before response completion.'}
        if($controller.Status -cne $Window.Phase){throw 'Controller phase changed outside the active output window.'}
    }catch{
        $Window.Status='Faulted';$Window.Failure=$_.Exception.Message;$controller.OutputWindowActive=$false
        Set-Ep11NativeSupervisorUnresolved $controller $Window.Failure -CleanupOnly:$cleanupOnly;return
    }
    Test-Ep11NativeSupervisorDeadline $controller $Identity $NowMs
    if($controller.Status -cne $Window.Phase){$Window.Status='Interrupted';$controller.OutputWindowActive=$false;return}
    try{
        $frame=Get-Ep11NativeOutputFrame $Window.Pump[0].Text.Substring($Window.Start) $Window.Phase $Window.Nonce
        if($null -eq $frame){return}
    }catch{
        $Window.Status='Faulted';$Window.Failure=$_.Exception.Message;$controller.OutputWindowActive=$false
        if($Window.Phase -ceq 'AwaitContext'){
            # Reborn: intact bounded transport and a still-known stop permit cooperative cleanup, not successful observation.
            Start-Ep11NativeSupervisorCleanup $controller $Identity $Window.Failure
        }else{Set-Ep11NativeSupervisorUnresolved $controller $Window.Failure -CleanupOnly:$cleanupOnly}
        return
    }
    $Window.Status='Consumed';$controller.OutputWindowActive=$false
    switch($Window.Phase){
        'AwaitStop' {Add-Ep11NativeSupervisorStop $controller $frame.Text $NowMs}
        'AwaitContext' {Add-Ep11NativeSupervisorContext $controller $Identity $frame.Text $NowMs}
        'AwaitClear' {Add-Ep11NativeSupervisorClear $controller $Identity $frame.Text $ReadByte $NowMs}
    }
}
