# Reborn: bind bounded completed stdout windows to native supervisor phases; no launch, command writes, detach, disposal or waits.
. (Join-Path $PSScriptRoot 'Ra3Ep11DebuggerOutputPump.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep11NativeSupervisorSession.ps1')

#-------------------------------------------------------------------------------------------------
<# Reborn: scope a single future response before its external command/continuation, freezing phase, nonce, stream and start cursor. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11NativeOutputWindow($Controller,[object[]]$Pump,[switch]$AllowModuleLoadPrefix) {
    if($Controller.Status -notin @('AwaitStop','AwaitContext','AwaitClear') -or $Controller.OutputWindowActive -or
       $Pump.Count -ne 2 -or @($Pump|Where-Object {$_.Faulted -or $_.Eof}).Count -gt 0 -or $Pump[1].Text.Length -ne 0){throw 'Output window scope unavailable.'}
    # Reborn: informational module records are opt-in only before a stopped-event marker, never context/clear acknowledgements.
    if($AllowModuleLoadPrefix -and $Controller.Status -cne 'AwaitStop'){throw 'Module-load prefix requires a stop window.'}
    $nonce=switch($Controller.Status){'AwaitStop' {$Controller.Observer.StopNonce};'AwaitContext' {$Controller.Observer.Decoder.PendingContext};'AwaitClear' {$Controller.Cleanup.Nonce}}
    if($nonce -cnotmatch '^[0-9a-f]{32}$'){throw 'Output window nonce unavailable.'}
    $Controller.OutputWindowActive=$true
    @{Status='Pending';Controller=$Controller;Pump=$Pump;Phase=$Controller.Status;Nonce=$nonce;Start=$Pump[0].Text.Length;Failure=$null;
        AllowModuleLoadPrefix=[bool]$AllowModuleLoadPrefix;ModuleLoads=@()}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: classify at most 32 bounded module-load lines before one exact stop marker; never accept exceptions or treat printed paths as live module identity. #>
#-------------------------------------------------------------------------------------------------
function Convert-Ep11ModuleLoadStopPrefix([string]$Text,[string]$Phase,[string]$Nonce) {
    if($Text.Length -gt 4096 -or $Phase -cnotin @('AwaitStop','AwaitHelperStop') -or $Nonce -cnotmatch '^[0-9a-f]{32}$'){
        throw 'Module-load prefix scope differs.'
    }
    $marker=if($Phase -ceq 'AwaitStop'){'REBORN_NATIVE_STOP'}else{'REBORN_BREAK'}
    $records=[Collections.Generic.List[object]]::new();$kept=[Collections.Generic.List[string]]::new();$stopSeen=$false
    foreach($rawLine in ($Text -split '\r?\n')){
        # Reborn: the recorded continuation begins with up to two adjacent prompts; normalization is restricted to a ModLoad line before the marker.
        $line=[regex]::Replace($rawLine,'^(?:[0-9a-fA-F]{1,8}:[0-9a-fA-F]{1,8}> ){1,2}(?=ModLoad: )','')
        if($line -cmatch '^ModLoad: ([0-9a-fA-F]{8}) ([0-9a-fA-F]{8})   (.+)$'){
            if($stopSeen -or $records.Count -ge 32){throw 'Module-load line after stop or count exceeded.'}
            $start=[long][Convert]::ToUInt32($Matches[1],16);$end=[long][Convert]::ToUInt32($Matches[2],16);$name=$Matches[3]
            # Reborn: accept only bounded printable calibrated Windows image shapes; this parser never resolves or opens the printed path.
            if($start -eq 0 -or $end -le $start -or $name.Length -gt 260 -or $name -cmatch '[\x00-\x1f\x7f]' -or
               $name -cnotmatch '^(?:image[0-9a-fA-F]{8}|[A-Za-z0-9_.-]+\.(?:dll|DLL|exe|EXE)|[A-Za-z]:\\[A-Za-z0-9_ .()\\-]+\.(?:dll|DLL|exe|EXE))$'){
                throw 'Module-load range/name differs.'
            }
            $records.Add([pscustomobject]@{Start=$start;End=$end;PrintedName=$name;IdentityValidated=$false});continue
        }
        if($line -cmatch ('^(?:[0-9a-fA-F]{1,8}:[0-9a-fA-F]{1,8}> )?'+$marker+' '+$Nonce+' ')){$stopSeen=$true}
        $kept.Add($rawLine)
    }
    # Reborn: unknown/stale/duplicate lines remain for the strict existing frame validator; no blanket diagnostic filtering.
    [pscustomobject]@{Text=($kept -join "`n");ModuleLoads=$records.ToArray()}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: require a complete terminal CDB prompt and strict phase markers; remove only one recognized prompt prefix immediately before a marker. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11NativeOutputFrame([string]$Text,[string]$Phase,[string]$Nonce,[switch]$AllowModuleLoadPrefix) {
    if($Text.Length -gt 4096 -or $Nonce -cnotmatch '^[0-9a-f]{32}$'){throw 'Output response window exceeds bounds.'}
    # Reborn: exact calibrated prompt subset ends with a space; partial prompt/CRLF chunks cannot authorize captures.
    $prompt=[regex]::Match($Text,'(?m)^[0-9a-fA-F]{1,8}:[0-9a-fA-F]{1,8}> (?:\r?\n)?\z')
    if(-not $prompt.Success){return}
    $content=$Text.Substring(0,$prompt.Index)
    # Reborn: classification follows complete framing; module metadata never bypasses nonce/site/context ownership validation.
    $moduleLoads=@()
    if($AllowModuleLoadPrefix){$classified=Convert-Ep11ModuleLoadStopPrefix $content $Phase $Nonce;$content=$classified.Text;$moduleLoads=$classified.ModuleLoads}
    $content=[regex]::Replace($content,'(?m)^[0-9a-fA-F]{1,8}:[0-9a-fA-F]{1,8}> (?=REBORN_)','')
    if([string]::IsNullOrWhiteSpace($content)){return}
    foreach($line in ($content -split '\r?\n')){
        if([string]::IsNullOrWhiteSpace($line)){continue}
        $accepted=switch($Phase){
            'AwaitStop' {$line -cmatch ('^REBORN_NATIVE_STOP '+$Nonce+' [0-4](?: [0-9a-fA-F]{1,8}){3}$')}
            'AwaitHelperStop' {$line -cmatch ('^REBORN_BREAK '+$Nonce+'(?: [0-9a-fA-F]{1,8}){3}$')}
            'AwaitContext' {$line -cmatch ('^REBORN_CONTEXT '+$Nonce+'(?: [0-9a-fA-F]{1,8}){8}$') -or $line -ceq ('REBORN_CONTEXT_END '+$Nonce)}
            'AwaitClear' {$line -ceq ('REBORN_CLEAR '+$Nonce)}
            default {$false}
        }
        if(-not $accepted){throw 'Unexpected response line, phase or nonce.'}
    }
    switch($Phase){
        'AwaitStop' {if(@(($content -split '\r?\n')|Where-Object {$_ -cmatch '^REBORN_NATIVE_STOP '}).Count -ne 1){throw 'Stop response duplicated or missing.'}}
        'AwaitHelperStop' {if(@(($content -split '\r?\n')|Where-Object {$_ -cmatch '^REBORN_BREAK '}).Count -ne 1){throw 'Helper stop response duplicated or missing.'}}
        'AwaitContext' {$null=Get-Ep11CompletedContextWindow $content $Nonce}
        'AwaitClear' {Assert-Ep11HelperBreakpointCleared $content $Nonce}
    }
    [pscustomobject]@{Text=$content;ModuleLoads=$moduleLoads}
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
        $frame=Get-Ep11NativeOutputFrame $Window.Pump[0].Text.Substring($Window.Start) $Window.Phase $Window.Nonce -AllowModuleLoadPrefix:$Window.AllowModuleLoadPrefix
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
    # Reborn: retain classified printed records for audit only; stop/context identity still comes from the admitted observer.
    $Window.ModuleLoads=$frame.ModuleLoads
    switch($Window.Phase){
        'AwaitStop' {Add-Ep11NativeSupervisorStop $controller $frame.Text $NowMs}
        'AwaitContext' {Add-Ep11NativeSupervisorContext $controller $Identity $frame.Text $NowMs}
        'AwaitClear' {Add-Ep11NativeSupervisorClear $controller $Identity $frame.Text $ReadByte $NowMs}
    }
}
