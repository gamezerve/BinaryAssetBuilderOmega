# Reborn: bounded asynchronous readers for already-owned redirected streams; no process launch, stream disposal, commands or waits.

#-------------------------------------------------------------------------------------------------
<# Reborn: begin one pending read per caller-owned stream with fixed stdout/stderr caps and independent buffers. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11DebuggerOutputPump([IO.TextReader]$Stdout,[IO.TextReader]$Stderr) {
    if($null -eq $Stdout -or $null -eq $Stderr -or [object]::ReferenceEquals($Stdout,$Stderr)){throw 'Output pump requires two distinct readers.'}
    $streams=@()
    foreach($spec in @(@($Stdout,65536),@($Stderr,8192))){
        $state=[pscustomobject]@{Reader=$spec[0];Limit=$spec[1];Text='';Buffer=[char[]]::new(1024);Task=$null;Eof=$false;Faulted=$false;Failure=$null;TotalRead=0L}
        $state.Task=$state.Reader.ReadAsync($state.Buffer,0,$state.Buffer.Length);$streams+=$state
    }
    return ,$streams
}

#-------------------------------------------------------------------------------------------------
<# Reborn: consume only completed reads, check the cap before retaining text and permanently refuse failed streams without closing them. #>
#-------------------------------------------------------------------------------------------------
function Update-Ep11DebuggerOutputPump([object[]]$Streams) {
    if($Streams.Count -ne 2 -or $Streams[0].Limit -ne 65536 -or $Streams[1].Limit -ne 8192){throw 'Output pump layout differs.'}
    if(@($Streams|Where-Object {$_.Faulted}).Count -gt 0){throw 'Output pump already faulted.'}
    foreach($stream in $Streams){
        if($null -eq $stream.Task -or -not $stream.Task.IsCompleted){continue}
        try{
            $count=$stream.Task.GetAwaiter().GetResult();$stream.Task=$null
            if($count -lt 0 -or $count -gt $stream.Buffer.Length){throw 'Output reader returned invalid count.'}
            if($count -eq 0){$stream.Eof=$true;continue}
            $stream.TotalRead+=$count
            if($stream.Text.Length -gt $stream.Limit-$count){throw 'Output pump bound exceeded.'}
            $stream.Text+=[string]::new($stream.Buffer,0,$count)
            $stream.Task=$stream.Reader.ReadAsync($stream.Buffer,0,$stream.Buffer.Length)
        }catch{$stream.Faulted=$true;$stream.Failure=$_.Exception.Message;throw}
    }
}
