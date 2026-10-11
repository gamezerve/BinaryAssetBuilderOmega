# Reborn: correlate one requested CDB register response with the incremental decoder; no debugger I/O, launch, resume or breakpoint API.
. (Join-Path $PSScriptRoot 'Ra3Ep11ConfigSnapshotAdapter.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep11ConfigProbeFilter.ps1')

#-------------------------------------------------------------------------------------------------
<# Reborn: require an empty breakpoint listing after clearing our helper breakpoint, not just a printed acknowledgement or temporarily restored code byte. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11HelperBreakpointCleared([string]$Text,[string]$Nonce) {
    if($Text.Length -gt 4096 -or $Nonce -cnotmatch '^[0-9a-f]{32}$'){throw 'Helper clear envelope invalid.'}
    $marker='REBORN_CLEAR '+$Nonce;$found=0
    foreach($line in ($Text -split '\r?\n')){
        if($line -ceq $marker){$found++;continue}
        if($line -cnotmatch '^\s*(?:[0-9a-fA-F]+:[0-9a-fA-F]+>\s*)?$'){throw 'Helper breakpoint listing not empty or clear command failed.'}
    }
    if($found -ne 1){throw 'Helper clear acknowledgement missing or ambiguous.'}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: parse one helper-only delay-site address/byte query, refusing a preexisting trap byte before installing any breakpoint. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11HelperBreakpointSite([string]$Text,[string]$Nonce) {
    if($Text.Length -gt 4096 -or $Nonce -cnotmatch '^[0-9a-f]{32}$' -or
       $Text -match '(?i)syntax error|could not resolve|unable to|memory access error|invalid command'){throw 'Helper site envelope invalid.'}
    $records=@(($Text -split '\r?\n')|Where-Object {$_ -cmatch '^REBORN_'})
    if($records.Count -ne 1 -or $records[0] -cnotmatch ('^REBORN_SITE '+$Nonce+' ([0-9a-fA-F]{1,8}) ([0-9a-fA-F]{1,2})$')){throw 'Helper site marker missing or ambiguous.'}
    $address=[long][Convert]::ToUInt32($Matches[1],16);$original=[byte][Convert]::ToByte($Matches[2],16)
    Assert-Ep11ReadPointer $address 1
    if($original -eq 0xcc){throw 'Helper site already contains a trap byte.'}
    [pscustomobject]@{Address=$address;OriginalByte=$original;NativeGameSiteValidated=$false}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: admit exactly one bounded helper-only breakpoint marker bound to the requested nonce, retained PID and admitted entry address. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11HelperBreakpointHit([string]$Text,[string]$Nonce,[int]$ExpectedProcessId,[long]$ExpectedAddress) {
    if($Text.Length -gt 65536 -or $Nonce -cnotmatch '^[0-9a-f]{32}$' -or $ExpectedProcessId -le 0 -or
       $Text -match '(?i)syntax error|could not resolve|unable to|memory access error|invalid command'){throw 'Helper breakpoint envelope invalid.'}
    Assert-Ep11ReadPointer $ExpectedAddress 1
    $records=@(($Text -split '\r?\n')|Where-Object {$_ -cmatch '^REBORN_'})
    if($records.Count -ne 1 -or $records[0] -cnotmatch ('^REBORN_BREAK '+$Nonce+' ([0-9a-fA-F]{1,8}) ([0-9a-fA-F]{1,8}) ([0-9a-fA-F]{1,8})$')){throw 'Helper breakpoint marker missing or ambiguous.'}
    $process=[long][Convert]::ToUInt32($Matches[1],16);$thread=[long][Convert]::ToUInt32($Matches[2],16);$address=[long][Convert]::ToUInt32($Matches[3],16)
    if($process -ne $ExpectedProcessId -or $thread -le 0 -or $address -ne $ExpectedAddress){throw 'Helper breakpoint owner/site differs.'}
    [pscustomobject]@{ProcessId=$process;ThreadId=$thread;SiteAddress=$address;SuppliedHitValidated=$true;GameReadProven=$false}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: isolate one fully terminated response window before parsing registers; a completion marker is framing, not stopped-process proof. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11CompletedContextWindow([string]$Text,[string]$Nonce) {
    if($Nonce -cnotmatch '^[0-9a-f]{32}$' -or $Text.Length -gt 4096){throw 'Context window outside bounds.'}
    $lines=@($Text -split '\r?\n');$end='REBORN_CONTEXT_END '+$Nonce
    # Reborn: a chunk ending inside the completion line is not a complete response window.
    if($Text -cnotmatch ('(?m)^'+$end+'\r?\n')){throw 'Context completion line is not terminated.'}
    $ends=@(0..($lines.Count-1)|Where-Object {$lines[$_] -ceq $end})
    if($ends.Count -ne 1){throw 'Context completion marker missing or ambiguous.'}
    $index=$ends[0]
    if($index -lt 1){throw 'Context completion precedes response.'}
    if($index+1 -lt $lines.Count){
        foreach($line in $lines[($index+1)..($lines.Count-1)]){
            if($line -cnotmatch '^\s*(?:[0-9a-fA-F]+:[0-9a-fA-F]+>\s*)?$'){throw 'Unexpected output after context completion.'}
        }
    }
    $window=$lines[0..($index-1)] -join "`n"
    $null=Get-Ep11ConfigContextRecord $window $Nonce
    $window
}

#-------------------------------------------------------------------------------------------------
<# Reborn: prepare a unique read-only register command without sending it or permitting overlapping requests. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11ConfigContextRequest($Session) {
    if($null -eq $Session -or $Session.Status -cne 'Open'){throw 'Context session is not open.'}
    try{
        if($Session.Sequence -ge 320 -or $null -ne $Session.PendingContext){throw 'Context request exceeds bounds or overlaps.'}
        # Reborn: bound every observed stop, including ignored stock calls, before preparing another debugger request.
        if($Session.FilterProbeEntries -and $Session.ContextResponses -ge 128){throw 'Filtered context stop limit exceeded.'}
        $nonce=[guid]::NewGuid().ToString('N')
        # Reborn: eight x86 context fields only; the command deliberately contains no g/gc, memory writes or external commands.
        $command='.printf "REBORN_CONTEXT '+$nonce+' %x %x %x %x %x %x %x %x", @$tpid, @$tid, @eip, @eax, @ebx, @esi, @edi, @esp; .echo'
        $Session.PendingContext=$nonce
        [pscustomobject]@{Nonce=$nonce;Command=$command;SendsCommand=$false;ResumesTarget=$false;LiveContextProven=$false}
    }catch{$Session.Status='Faulted';throw}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: parse one nonce-bound register marker, refusing debugger errors and ambiguous evidence before any memory callback. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11ConfigContextRecord([string]$Text,[string]$Nonce) {
    if($Nonce -cnotmatch '^[0-9a-f]{32}$' -or $Text.Length -gt 4096 -or
       $Text -match '(?i)syntax error|could not resolve|unable to|memory access error|invalid register|invalid command'){throw 'Context response outside bounds or reports an error.'}
    $records=@(($Text -split '\r?\n')|Where-Object {$_ -cmatch '^REBORN_'})
    if($records.Count -ne 1){throw 'Context response missing or ambiguous.'}
    $pattern='^REBORN_CONTEXT '+$Nonce+'((?: [0-9a-fA-F]{1,8}){8})$'
    if($records[0] -cnotmatch $pattern){throw 'Context nonce or register format differs.'}
    $values=@($Matches[1].Trim().Split(' ')|ForEach-Object {[long][Convert]::ToUInt32($_,16)})
    if($values[0] -le 0 -or $values[0] -gt [int]::MaxValue -or $values[1] -le 0){throw 'Context PID/thread invalid.'}
    [pscustomobject]@{ProcessId=$values[0];ThreadId=$values[1];SiteAddress=$values[2];Eax=$values[3];Ebx=$values[4];Esi=$values[5];Edi=$values[6];Esp=$values[7]}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: consume a single pending response, assign invocation IDs from per-thread lifetimes and capture while the caller keeps the target stopped. #>
#-------------------------------------------------------------------------------------------------
function Add-Ep11ConfigDebuggerResponse($Session,[string]$Text) {
    if($null -eq $Session -or $Session.Status -cne 'Open'){throw 'Context session is not open.'}
    try{
        if($null -eq $Session.PendingContext){throw 'Unsolicited context response.'}
        $nonce=$Session.PendingContext;$Session.PendingContext=$null
        $record=Get-Ep11ConfigContextRecord $Text $nonce
        if($record.ProcessId -ne $Session.ExpectedProcessId){throw 'Context process differs from session.'}
        if($Session.FilterProbeEntries){
            if($Session.ContextResponses -ge 128){throw 'Filtered context stop limit exceeded.'}
            $selection=Get-Ep11ConfigProbeSelection $Session $record
            $Session.ContextResponses++
            if($selection -in @('CallerMismatch','PathMismatch','UntrackedEvent')){
                # Reborn: count only reason codes, never unrelated paths or buffers; ignored calls are not successful evidence.
                $Session.IgnoredContextResponses++;$Session.IgnoreReasons[$selection]++;return
            }
        }
        $thread=$record.ThreadId.ToString()
        if($record.SiteAddress -eq 0x4d86b0){
            if($Session.Threads.ContainsKey($thread)){throw 'Context entry overlaps same-thread invocation.'}
            # Reborn: global within-session IDs are assigned here, never taken from debugger scratch registers or supplied log IDs.
            $invocation=[long]$Session.Seen.Count+1
        }else{
            if(-not $Session.Threads.ContainsKey($thread)){throw 'Context return has no scoped thread lifetime.'}
            $invocation=[long]$Session.Threads[$thread]
        }
        $record|Add-Member Sequence ([long]$Session.Sequence+1)
        $record|Add-Member InvocationId $invocation
        Add-Ep11ConfigSnapshot $Session $record
    }catch{$Session.Status='Faulted';throw}
}
