# Reborn: prepare and validate a fixed CDB display-only profile; no debugger I/O, target execution or register writes.

#-------------------------------------------------------------------------------------------------
<# Reborn: disable all five automatic stop-display categories in the debugger session, with a fresh acknowledgement nonce. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11DebuggerDisplayRequest {
    $nonce=[guid]::NewGuid().ToString('N')
    [pscustomobject]@{Nonce=$nonce;Command=('.prompt_allow -reg -dis -ea -src -sym; .echo REBORN_DISPLAY '+$nonce);CommandsSent=$false;TargetResumed=$false}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: require the pinned English CDB report to place exactly all five categories in its disabled list and end with one fresh acknowledgement/prompt. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11DebuggerDisplayProfile([string]$Text,[string]$Nonce) {
    if($Text.Length -gt 4096 -or $Nonce -cnotmatch '^[0-9a-f]{32}$' -or $Text -cnotmatch ('(?m)^REBORN_DISPLAY '+$Nonce+'\r?\n') -or
       $Text -cnotmatch '(?m)^[0-9a-fA-F]{1,8}:[0-9a-fA-F]{1,8}> \z'){throw 'Display profile response incomplete or oversized.'}
    $disabled=$false;$seen=@{};$ack=0;$allowHeader=0;$disabledHeader=0
    # Reborn: the calibrated CDB can prefix the first report heading with its prompt and spells an empty allow list as plain None.
    $Text=[regex]::Replace($Text,'(?m)^[0-9a-fA-F]{1,8}:[0-9a-fA-F]{1,8}> (?=Allow the following information to be displayed at the prompt:)','')
    foreach($line in ($Text -split '\r?\n')){
        if($line -ceq 'Allow the following information to be displayed at the prompt:'){
            if($allowHeader -ne 0 -or $disabled -or $ack -ne 0){throw 'Display report header order differs.'};$allowHeader++;continue
        }
        if($line -ceq 'Do not allow the following information to be displayed at the prompt:'){
            if($allowHeader -ne 1 -or $disabled -or $ack -ne 0){throw 'Display disabled header order differs.'};$disabled=$true;$disabledHeader++;continue
        }
        if($line -cmatch '^\s*(reg|dis|ea|src|sym) - .+$'){
            if(-not $disabled -or $ack -ne 0 -or $seen.ContainsKey($Matches[1])){throw 'Display category still allowed, duplicated or after acknowledgement.'};$seen[$Matches[1]]=$true;continue
        }
        if($line -ceq ('REBORN_DISPLAY '+$Nonce)){
            if($seen.Count -ne 5 -or $ack -ne 0){throw 'Display acknowledgement precedes complete report or is duplicated.'};$ack++;continue
        }
        if($line -ceq '(Other settings can affect whether the information is actually displayed)' -or $line -cmatch '^\s*(?:None|\(none\))\s*$' -or
           $line -cmatch '^\s*(?:[0-9a-fA-F]{1,8}:[0-9a-fA-F]{1,8}> )?$'){continue}
        throw 'Unexpected display profile report line.'
    }
    if($allowHeader -ne 1 -or $disabledHeader -ne 1 -or $seen.Count -ne 5 -or $ack -ne 1){throw 'Display profile report missing or ambiguous.'}
}
