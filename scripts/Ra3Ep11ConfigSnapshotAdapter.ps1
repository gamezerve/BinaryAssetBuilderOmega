# Reborn: decode scoped x86 snapshots through a caller-owned read adapter; no process creation, debugger commands or memory writes.
. (Join-Path $PSScriptRoot 'Ra3Ep11ConfigReadCorrelation.ps1')

#-------------------------------------------------------------------------------------------------
<# Reborn: require actual exact-address full transfers from the supplied reader before interpreting stack or target bytes. #>
#-------------------------------------------------------------------------------------------------
function Read-Ep11SnapshotBytes([scriptblock]$ReadAdapter,[long]$Address,[int]$Count) {
    Assert-Ep11ReadPointer $Address $Count
    if($Count -gt 241){throw 'Snapshot read exceeds probe capture bound.'}
    $record=& $ReadAdapter $Address $Count
    if($null -eq $record -or $record -is [array] -or $record.Address -ne $Address -or
       $record.Success -isnot [bool] -or -not $record.Success -or $record.ActualCount -ne $Count -or
       $record.Bytes -isnot [byte[]] -or $record.Bytes.Length -ne $Count){throw 'Snapshot adapter transfer incomplete or incorrectly addressed.'}
    return ,([byte[]]$record.Bytes.Clone())
}

#-------------------------------------------------------------------------------------------------
<# Reborn: create bounded per-observation state without retaining stale process-memory snapshots for later replay. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11ConfigSnapshotSession([int]$ExpectedProcessId,[string]$ProbePath,[scriptblock]$ReadAdapter,[long]$ModuleBase=0x400000) {
    if($ExpectedProcessId -le 0 -or $ModuleBase -ne 0x400000 -or
       $null -eq $ReadAdapter -or -not [IO.Path]::IsPathFullyQualified($ProbePath) -or $ProbePath.Length -gt 240 -or
       $ProbePath -match '[^\x20-\x7E]|[";]'){throw 'Snapshot scope outside bounds.'}
    Assert-Ep11ConfigNamePolicy $ProbePath
    return @{Status='Open';Sequence=0;ExpectedProcessId=$ExpectedProcessId;ProbePath=$ProbePath;ReadAdapter=$ReadAdapter;ModuleBase=$ModuleBase;
        PathBytes=[Text.Encoding]::ASCII.GetBytes($ProbePath+[char]0);Active=@{};Seen=@{};Threads=@{};Objects=@{};Buffers=@{};Events=[Collections.Generic.List[object]]::new()}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: consume exactly one currently stopped-context snapshot; a failed capture permanently faults this trusted in-process session. #>
#-------------------------------------------------------------------------------------------------
function Add-Ep11ConfigSnapshot($Session,$snapshot) {
    if($null -eq $Session -or $Session.Status -cne 'Open'){throw 'Snapshot session is not open.'}
    try{
        if($Session.Sequence -ge 320){throw 'Snapshot session event limit exceeded.'}
        $sequence=$Session.Sequence+1;$ExpectedProcessId=$Session.ExpectedProcessId;$ProbePath=$Session.ProbePath
        $ReadAdapter=$Session.ReadAdapter;$pathBytes=$Session.PathBytes
        $active=$Session.Active;$seen=$Session.Seen;$threads=$Session.Threads;$objects=$Session.Objects;$buffers=$Session.Buffers
        $capture=$null
        foreach($field in @('Sequence','ProcessId','ThreadId','InvocationId','SiteAddress','Eax','Ebx','Esi','Edi','Esp')){
            # Reborn: accept integral register values only; strings, booleans and fractional values cannot impersonate a machine snapshot.
            $value=$snapshot.$field
            if($value -isnot [int] -and $value -isnot [uint32] -and $value -isnot [long]){throw 'Snapshot field is not an integer.'}
            if($value -lt 0 -or $value -gt [uint32]::MaxValue){throw 'Snapshot field outside x86 bounds.'}
        }
        if($snapshot.Sequence -ne $sequence -or $snapshot.ProcessId -ne $ExpectedProcessId -or
           $snapshot.ThreadId -eq 0 -or $snapshot.InvocationId -eq 0){throw 'Snapshot identity/order differs.'}
        $key=$snapshot.InvocationId.ToString();$thread=$snapshot.ThreadId.ToString()
        $event=[pscustomobject]@{Sequence=$sequence;ProcessId=$ExpectedProcessId;ThreadId=$snapshot.ThreadId;InvocationId=$snapshot.InvocationId;SiteAddress=$snapshot.SiteAddress}
        if($snapshot.SiteAddress -eq 0x4d86b0){
            if($seen.ContainsKey($key) -or $threads.ContainsKey($thread)){throw 'Snapshot entry lifetime ambiguous.'}
            $stack=Read-Ep11SnapshotBytes $ReadAdapter $snapshot.Esp 8
            $caller=[BitConverter]::ToUInt32($stack,0);$pathPointer=[BitConverter]::ToUInt32($stack,4)
            if($caller -ne 0x4d98ab){throw 'Snapshot caller outside scoped route.'}
            $capture=Read-Ep11SnapshotBytes $ReadAdapter $pathPointer $pathBytes.Length
            for($index=0;$index -lt $pathBytes.Length;$index++){if($capture[$index] -ne $pathBytes[$index]){throw 'Snapshot exact path capture differs.'}}
            $seen[$key]=$true;$threads[$thread]=$key;$active[$key]=@{Thread=$thread;Stage='Entered';Object=0L;Buffer=0L}
            $event|Add-Member Kind 'ReaderEnter';$event|Add-Member Path $ProbePath;$event|Add-Member CallerReturnAddress $caller
        }else{
            if(-not $active.ContainsKey($key) -or $active[$key].Thread -cne $thread){throw 'Snapshot has no same-thread invocation.'}
            $state=$active[$key]
            switch($snapshot.SiteAddress){
                0x4d86f2 {
                    if($state.Stage -cne 'Entered' -or $objects.ContainsKey($snapshot.Eax.ToString())){throw 'Snapshot object stage/lifetime ambiguous.'}
                    $capture=Read-Ep11SnapshotBytes $ReadAdapter $snapshot.Eax 4
                    $vtable=[BitConverter]::ToUInt32($capture,0)
                    if($vtable -notin @(0xbfa208,0xbfa270)){throw 'Snapshot reader contract unknown.'}
                    $state.Object=[long]$snapshot.Eax;$state.Stage='Opened';$objects[$state.Object.ToString()]=$key
                    $event|Add-Member Kind 'OpenReturn';$event|Add-Member ObjectAddress $state.Object;$event|Add-Member ReaderVtable $vtable
                }
                0x4d8725 {
                    if($state.Stage -cne 'Opened' -or $snapshot.Esi -ne $state.Object -or $snapshot.Ebx -ne 1 -or $snapshot.Eax -ne 1){throw 'Snapshot read registers differ.'}
                    Assert-Ep11ReadPointer $snapshot.Edi 2
                    foreach($buffer in $buffers.Keys){if($snapshot.Edi -lt [long]$buffer+2 -and [long]$snapshot.Edi+2 -gt [long]$buffer){throw 'Snapshot active buffer overlaps.'}}
                    $capture=Read-Ep11SnapshotBytes $ReadAdapter $snapshot.Edi 1
                    if($capture[0] -ne 10){throw 'Snapshot read byte differs.'}
                    $state.Buffer=[long]$snapshot.Edi;$state.Stage='Read';$buffers[$state.Buffer.ToString()]=$key
                    $event|Add-Member Kind 'ReadReturn';$event|Add-Member ObjectAddress $state.Object;$event|Add-Member BufferAddress $state.Buffer
                    $event|Add-Member RequestedCount $snapshot.Ebx;$event|Add-Member ReturnedCount $snapshot.Eax
                }
                0x4d9040 {
                    if($state.Stage -cne 'Read'){throw 'Snapshot parser stage differs.'}
                    Assert-Ep11ReadPointer $snapshot.Esp 8
                    $stack=Read-Ep11SnapshotBytes $ReadAdapter ([long]$snapshot.Esp+4) 4
                    $buffer=[BitConverter]::ToUInt32($stack,0)
                    if($buffer -ne $state.Buffer){throw 'Snapshot parser buffer differs.'}
                    $capture=Read-Ep11SnapshotBytes $ReadAdapter $buffer 2
                    if($capture[0] -ne 10 -or $capture[1] -ne 0){throw 'Snapshot parser content differs.'}
                    $state.Stage='ParsedInput';$event|Add-Member Kind 'ParserEnter';$event|Add-Member BufferAddress $buffer
                }
                0x4d98ab {
                    if($state.Stage -cne 'ParsedInput'){throw 'Snapshot return precedes matched parser.'}
                    $event|Add-Member Kind 'ReaderExit'
                    $objects.Remove($state.Object.ToString());$buffers.Remove($state.Buffer.ToString());$threads.Remove($thread);$active.Remove($key)
                }
                default {throw 'Snapshot site outside scoped route.'}
            }
            if($null -ne $capture -and $event.Kind -cne 'ReaderExit'){
                $event|Add-Member CaptureSucceeded $true;$event|Add-Member CapturedCount $capture.Length;$event|Add-Member Bytes ([byte[]]$capture.Clone())
            }
        }
        $Session.Events.Add($event);$Session.Sequence=$sequence
    }catch{
        # Reborn: never repair a partial capture or continue reading after a refused snapshot.
        $Session.Status='Faulted';throw
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: close only complete chains, validate all captured events, and export private copies without performing more reads. #>
#-------------------------------------------------------------------------------------------------
function Complete-Ep11ConfigSnapshotSession($Session) {
    if($null -eq $Session -or $Session.Status -cne 'Open'){throw 'Snapshot session is not open.'}
    try{
        # Reborn: a requested debugger context must be consumed before finalizing its captured evidence.
        if($null -ne $Session.PendingContext){throw 'Snapshot session has a pending context response.'}
        if($Session.Active.Count -ne 0){throw 'Snapshot chain incomplete.'}
        $null=Get-Ep11ConfigReadCorrelation $Session.Events.ToArray() $Session.ExpectedProcessId $Session.ProbePath $Session.ModuleBase
        $Session.Status='Closed'
        foreach($event in $Session.Events){
            $copy=$event.PSObject.Copy()
            if($event.PSObject.Properties.Name -contains 'Bytes'){$copy.Bytes=[byte[]]$event.Bytes.Clone()}
            $copy
        }
    }catch{$Session.Status='Faulted';throw}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: retain batch compatibility for detached fixtures only, using the same incremental decoder rather than a second ABI implementation. #>
#-------------------------------------------------------------------------------------------------
function ConvertTo-Ep11ConfigReadEvents([object[]]$Snapshots,[int]$ExpectedProcessId,[string]$ProbePath,[scriptblock]$ReadAdapter,[long]$ModuleBase=0x400000) {
    if($Snapshots.Count -lt 5 -or $Snapshots.Count -gt 320){throw 'Snapshot batch outside bounds.'}
    $session=New-Ep11ConfigSnapshotSession $ExpectedProcessId $ProbePath $ReadAdapter $ModuleBase
    foreach($snapshot in $Snapshots){Add-Ep11ConfigSnapshot $session $snapshot}
    Complete-Ep11ConfigSnapshotSession $session
}
