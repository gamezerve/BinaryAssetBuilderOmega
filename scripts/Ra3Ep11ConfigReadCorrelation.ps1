# Reborn: correlate bounded supplied engine-reader records only; no debugger, process access, kernel-read or game-execution claim.
. (Join-Path $PSScriptRoot 'Ra3Ep11ConfigNamePolicy.ps1')

#-------------------------------------------------------------------------------------------------
<# Reborn: require non-null bounded x86 object/buffer pointers before linking any supplied observations. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11ReadPointer([long]$Pointer,[int]$Length) {
    if($Pointer -le 0 -or $Length -lt 1 -or $Pointer+$Length -gt 0x100000000L){throw 'Engine correlation pointer outside x86 range.'}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: require actual successful byte capture, not requested size or a positive engine count alone. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11ReadCapture($Record,[byte[]]$Expected) {
    if($Record.CaptureSucceeded -isnot [bool] -or -not $Record.CaptureSucceeded -or $Record.CapturedCount -ne $Expected.Length -or
       $Record.Bytes -isnot [byte[]] -or $Record.Bytes.Length -ne $Expected.Length){throw 'Engine correlation byte capture incomplete.'}
    for($index=0;$index -lt $Expected.Length;$index++){if($Record.Bytes[$index] -ne $Expected[$index]){throw 'Engine correlation captured content differs.'}}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: bind exact-probe entry, reader, actual read buffer, parser input and return per invocation/thread; refuse ambiguous lifetimes. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11ConfigReadCorrelation([object[]]$Events,[int]$ExpectedProcessId,[string]$ProbePath,[long]$ModuleBase=0x400000) {
    if($ExpectedProcessId -le 0 -or $ModuleBase -ne 0x400000 -or $Events.Count -lt 5 -or $Events.Count -gt 320 -or
       -not [IO.Path]::IsPathFullyQualified($ProbePath) -or $ProbePath.Length -gt 240 -or $ProbePath -match '[^\x20-\x7E]|[";]'){throw 'Engine correlation scope outside bounds.'}
    Assert-Ep11ConfigNamePolicy $ProbePath
    $active=@{};$seen=@{};$threads=@{};$objects=@{};$buffers=@{};$completed=[Collections.Generic.List[object]]::new();$sequence=0
    foreach($event in $Events){
        $sequence++
        if($event.Sequence -ne $sequence -or $event.ProcessId -ne $ExpectedProcessId -or $event.ThreadId -le 0 -or
           $event.ThreadId -gt [uint32]::MaxValue -or $event.InvocationId -le 0 -or $event.InvocationId -gt [uint32]::MaxValue){throw 'Engine correlation ordering/process identity differs.'}
        $key=$event.InvocationId.ToString();$thread=$event.ThreadId.ToString()
        if($event.Kind -ceq 'ReaderEnter'){
            if($seen.ContainsKey($key) -or $threads.ContainsKey($thread) -or $event.SiteAddress -ne 0x4d86b0 -or
               $event.Path -ine $ProbePath -or $event.CallerReturnAddress -ne 0x4d98ab){throw 'Engine reader entry scope/overlap differs.'}
            $seen[$key]=$true;$threads[$thread]=$key
            $active[$key]=@{Stage='Entered';Thread=$thread;Object=0L;Vtable=0L;Buffer=0L}
            continue
        }
        if(-not $active.ContainsKey($key) -or $active[$key].Thread -cne $thread){throw 'Engine event belongs to no active same-thread invocation.'}
        $state=$active[$key]
        switch -CaseSensitive ($event.Kind){
            'OpenReturn' {
                if($state.Stage -cne 'Entered' -or $event.SiteAddress -ne 0x4d86f2){throw 'Unexpected reader open return.'}
                Assert-Ep11ReadPointer $event.ObjectAddress 4
                if($event.ReaderVtable -notin @(0xbfa208,0xbfa270)){throw 'Unknown concrete reader contract.'}
                Assert-Ep11ReadCapture $event ([BitConverter]::GetBytes([uint32]$event.ReaderVtable))
                $object=$event.ObjectAddress.ToString()
                if($objects.ContainsKey($object)){throw 'Concurrent reader object lifetime ambiguous.'}
                $objects[$object]=$key;$state.Object=[long]$event.ObjectAddress;$state.Vtable=[long]$event.ReaderVtable;$state.Stage='Opened'
            }
            'ReadReturn' {
                if($state.Stage -cne 'Opened' -or $event.SiteAddress -ne 0x4d8725 -or $event.ObjectAddress -ne $state.Object -or
                   $event.RequestedCount -ne 1 -or $event.ReturnedCount -ne 1){throw 'Engine actual read/object contract differs.'}
                Assert-Ep11ReadPointer $event.BufferAddress 2
                Assert-Ep11ReadCapture $event ([byte[]]@(10))
                foreach($buffer in $buffers.Keys){if([long]$event.BufferAddress -lt [long]$buffer+2 -and [long]$event.BufferAddress+2 -gt [long]$buffer){throw 'Concurrent parser buffer lifetime overlaps.'}}
                $buffers[$event.BufferAddress.ToString()]=$key;$state.Buffer=[long]$event.BufferAddress;$state.Stage='Read'
            }
            'ParserEnter' {
                if($state.Stage -cne 'Read' -or $event.SiteAddress -ne 0x4d9040 -or $event.BufferAddress -ne $state.Buffer){throw 'Parser input not bound to actual reader buffer.'}
                Assert-Ep11ReadCapture $event ([byte[]]@(10,0));$state.Stage='ParsedInput'
            }
            'ReaderExit' {
                if($state.Stage -cne 'ParsedInput' -or $event.SiteAddress -ne 0x4d98ab){throw 'Reader returned without matched parser input.'}
                $completed.Add([pscustomobject]@{InvocationId=$event.InvocationId;ThreadId=$event.ThreadId;ReaderObject=$state.Object;ReaderVtable=$state.Vtable;ParserBuffer=$state.Buffer;ActualReadCount=1;ParserBytesMatched=$true})
                $objects.Remove($state.Object.ToString());$buffers.Remove($state.Buffer.ToString());$threads.Remove($thread);$active.Remove($key)
            }
            default {throw 'Unknown scoped engine correlation event.'}
        }
    }
    if($active.Count -ne 0 -or $completed.Count -eq 0){throw 'Engine correlation ended with incomplete invocation.'}
    [pscustomobject]@{SuppliedEngineChainValidated=$true;CompletedInvocationCount=$completed.Count;Invocations=$completed.ToArray();ExactProbePath=$ProbePath;ProcessId=$ExpectedProcessId;WindowsFileReadProven=$false;LiveGameReadProven=$false;ParserDirectivesConsumedProven=$false;ModPackageLoaded=$false;TargetExecuted=$false;CollectorReady=$false}
}
