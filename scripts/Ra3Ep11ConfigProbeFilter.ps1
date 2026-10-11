# Reborn: classify only reviewed same-process reader contexts using bounded captures; never claim an ignored call was read or parsed.

#-------------------------------------------------------------------------------------------------
<# Reborn: select the exact probe entry, or ignore known untracked contexts without decoding unrelated reader objects/buffers. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11ConfigProbeSelection($Session,$Record) {
    if($Session.Status -cne 'Open' -or -not $Session.FilterProbeEntries){throw 'Probe filter session not enabled/open.'}
    foreach($field in @('ProcessId','ThreadId','SiteAddress','Eax','Ebx','Esi','Edi','Esp')){
        $value=$Record.$field
        if(($value -isnot [int] -and $value -isnot [uint32] -and $value -isnot [long]) -or
           $value -lt 0 -or $value -gt [uint32]::MaxValue){throw 'Probe filter context is not an x86 integer record.'}
    }
    if($Record.ProcessId -ne $Session.ExpectedProcessId -or $Record.ThreadId -eq 0 -or
       $Record.SiteAddress -notin @(0x4d86b0,0x4d86f2,0x4d8725,0x4d9040,0x4d98ab)){throw 'Probe filter context outside scope.'}
    $thread=$Record.ThreadId.ToString()
    if($Record.SiteAddress -ne 0x4d86b0){
        if($Session.Threads.ContainsKey($thread)){return 'TrackedEvent'}
        return 'UntrackedEvent'
    }
    if($Session.Threads.ContainsKey($thread)){throw 'Probe filter cannot disambiguate nested same-thread readers.'}
    $stack=Read-Ep11SnapshotBytes $Session.ReadAdapter $Record.Esp 8
    if([BitConverter]::ToUInt32($stack,0) -ne 0x4d98ab){return 'CallerMismatch'}
    $pointer=[long][BitConverter]::ToUInt32($stack,4)
    # Reborn: compare one byte at a time through the first mismatch/NUL, never dump or decode an unrelated config path.
    for($index=0;$index -lt $Session.PathBytes.Length;$index++){
        $capture=Read-Ep11SnapshotBytes $Session.ReadAdapter ($pointer+$index) 1
        if($capture[0] -ne $Session.PathBytes[$index]){return 'PathMismatch'}
    }
    return 'ProbeEntry'
}
