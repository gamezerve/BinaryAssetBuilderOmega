# Reborn: exercise private synchronous and controlled pending readers, not a debugger/process or OS pipe.
[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
. (Join-Path $PSScriptRoot 'Ra3Ep11DebuggerOutputPump.ps1')
Add-Type -TypeDefinition @'
using System;
using System.IO;
using System.Threading.Tasks;
// Reborn: expose deterministic partial-read completion and faults without threads, native handles or process APIs.
public sealed class RebornPumpTestReader : TextReader {
    private char[] buffer; private int offset, capacity;
    private TaskCompletionSource<int> pending;
    public int ReadCalls { get; private set; }
    public bool Disposed { get; private set; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain one pending test buffer and refuse overlapping reads. */
    //-------------------------------------------------------------------------------------------------
    public override Task<int> ReadAsync(char[] target, int start, int count) {
        if (pending != null) throw new InvalidOperationException("Overlapping private read.");
        buffer=target; offset=start; capacity=count; ReadCalls++;
        pending=new TaskCompletionSource<int>(); return pending.Task;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: complete the pending read with exactly one supplied fragment, including empty EOF. */
    //-------------------------------------------------------------------------------------------------
    public void Supply(string text) {
        if (pending == null || text.Length > capacity) throw new InvalidOperationException("Invalid private fragment.");
        text.CopyTo(0,buffer,offset,text.Length); var completion=pending; pending=null; completion.SetResult(text.Length);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: report a private I/O fault through the existing pending task. */
    //-------------------------------------------------------------------------------------------------
    public void Fail() {
        var completion=pending; pending=null; completion.SetException(new IOException("Injected private read fault."));
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: record disposal only so tests can detect accidental ownership changes. */
    //-------------------------------------------------------------------------------------------------
    protected override void Dispose(bool disposing) { Disposed=true; base.Dispose(disposing); }
}
'@
$positives=0;$refusals=0
$stdout=[RebornPumpTestReader]::new();$stderr=[RebornPumpTestReader]::new();$pump=New-Ep11DebuggerOutputPump $stdout $stderr
# Reborn: repeated polling of pending tasks must not block or issue overlapping reads.
1..10|ForEach-Object {Update-Ep11DebuggerOutputPump $pump}
if($stdout.ReadCalls -ne 1 -or $stderr.ReadCalls -ne 1 -or $pump[0].Text.Length -ne 0){throw 'Pending pump started another read.'};$positives++
$expected=''
foreach($chunk in @('0:','000> RE','BORN_CONTEXT abc 10 20',"`r","`nREBORN_CONTEXT_END abc`n")){
    $expected+=$chunk;$stdout.Supply($chunk);Update-Ep11DebuggerOutputPump $pump
    if($pump[0].Text -cne $expected -or $pump[1].Text.Length -ne 0){throw 'Pump lost fragment/prompt/CRLF bytes.'}
}
$stderr.Supply('private warning');Update-Ep11DebuggerOutputPump $pump
if($pump[1].Text -cne 'private warning' -or $pump[0].Text -cne $expected){throw 'Pump mixed output streams.'};$positives++
$stdout.Supply('');$stderr.Supply('');Update-Ep11DebuggerOutputPump $pump
$calls=$stdout.ReadCalls+$stderr.ReadCalls;Update-Ep11DebuggerOutputPump $pump
if(-not $pump[0].Eof -or -not $pump[1].Eof -or $stdout.ReadCalls+$stderr.ReadCalls -ne $calls -or $stdout.Disposed -or $stderr.Disposed){throw 'EOF or reader ownership changed.'};$positives++
foreach($channel in 0..1){
    $limit=if($channel -eq 0){65536}else{8192}
    foreach($overflow in @($false,$true)){
        $texts=@('','');$texts[$channel]='x'*($limit+[int]$overflow)
        $pump=New-Ep11DebuggerOutputPump ([IO.StringReader]::new($texts[0])) ([IO.StringReader]::new($texts[1]));$refused=$false
        try{for($index=0;$index -lt 80;$index++){Update-Ep11DebuggerOutputPump $pump}}catch{$refused=$true}
        if($overflow){
            if(-not $refused -or -not $pump[$channel].Faulted -or $pump[$channel].Text.Length -gt $limit){throw 'Oversized output retained or admitted.'};$refusals++
            $retry=$false;try{Update-Ep11DebuggerOutputPump $pump}catch{$retry=$true};if(-not $retry){throw 'Faulted pump retried.'};$refusals++
        }else{if($refused -or $pump[$channel].Text.Length -ne $limit -or -not $pump[$channel].Eof){throw 'Exact output cap failed.'};$positives++}
    }
    $stdout=[RebornPumpTestReader]::new();$stderr=[RebornPumpTestReader]::new();$pump=New-Ep11DebuggerOutputPump $stdout $stderr
    if($channel -eq 0){$stdout.Fail()}else{$stderr.Fail()}
    $refused=$false;try{Update-Ep11DebuggerOutputPump $pump}catch{$refused=$true}
    if(-not $refused -or -not $pump[$channel].Faulted -or $stdout.Disposed -or $stderr.Disposed){throw 'Read fault hidden or readers disposed.'};$refusals++
    $retry=$false;try{Update-Ep11DebuggerOutputPump $pump}catch{$retry=$true};if(-not $retry){throw 'Read-fault pump retried.'};$refusals++
}
$reader=[IO.StringReader]::new('');$refused=$false;try{$null=New-Ep11DebuggerOutputPump $reader $reader}catch{$refused=$true};if(-not $refused){throw 'Same reader admitted twice.'};$refusals++
[pscustomobject]@{DetachedPositives=$positives;RefusalAndRetryChecks=$refusals;StreamsRemainCallerOwned=$true;GameExecuted=$false;DebuggerExecuted=$false}
