# Reborn: calibrate detach and debugger-loss behavior on a newly compiled x86 helper, never a game or existing process.
[CmdletBinding()]
param([switch]$Run,[switch]$SelfTest,[ValidateSet('Detach','DebuggerLoss','BreakpointDetach')][string]$Mode='Detach')
$ErrorActionPreference='Stop'
. (Join-Path $PSScriptRoot 'Ra3Ep11DebuggerOwnershipPolicy.ps1')
if($SelfTest){
    if($Run){throw 'Detached tests and execution must be separate.'}
    # Reborn: inject only detached text faults; no compiler, process query, debugger or memory read occurs.
    $sample="REBORN_OWNER 10`nREBORN_SITE 77661234 b8`nREBORN_BP_HIT"
    $null=Get-Ep11OwnedDebuggerEvidence $sample 16 $true
    $null=Get-Ep11OwnedDebuggerEvidence 'REBORN_OWNER 10' 16 $false
    $faults=@($sample.Replace('OWNER 10','OWNER 11'),$sample.Replace('OWNER 10',"OWNER 10`nREBORN_OWNER 10"),$sample.Replace('77661234','00000000'),$sample.Replace('b8','cc'),$sample.Replace('REBORN_BP_HIT',''),($sample+"`nREBORN_BP_HIT"),$sample.Replace('REBORN_SITE','REBORN_UNKNOWN'),("REBORN_BP_HIT`n"+$sample),('x'*65537),($sample+"`nMemory access error"),$sample.Replace("REBORN_OWNER 10`n",''))
    foreach($fault in $faults){$refused=$false;try{$null=Get-Ep11OwnedDebuggerEvidence $fault 16 $true}catch{$refused=$true};if(-not $refused){throw 'Ownership fault admitted.'}}
    $refused=$false;try{$null=Get-Ep11OwnedDebuggerEvidence $sample 16 $false}catch{$refused=$true};if(-not $refused){throw 'Unexpected breakpoint admitted.'}
    'Owned-debugger detached tests: PASS; two positives and twelve refusals; no target executed.';return
}
$repo=Split-Path -Parent $PSScriptRoot
$source=Join-Path $repo 'fixtures/ra3ep11/phase-a/DebuggerLifetimeControl.cs'
$debugger='C:\Program Files (x86)\Windows Kits\10\Debuggers\x86\cdb.exe'
$compiler=Join-Path ([Environment]::GetFolderPath([Environment+SpecialFolder]::Windows)) 'Microsoft.NET/Framework/v4.0.30319/csc.exe'
# Reborn: reject redirected fixed inputs and output ancestry before creating a fresh run directory.
$paths=@($repo,$source,$debugger,$compiler)
if(Test-Path -LiteralPath (Join-Path $repo 'artifacts')){$paths+=Join-Path $repo 'artifacts'}
foreach($path in $paths){
    for($item=Get-Item -LiteralPath $path -Force;$null -ne $item;){
        if(($item.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0){throw 'Redirected lifetime-control path refused.'}
        $item=if($item -is [IO.FileInfo]){$item.Directory}else{$item.Parent}
    }
}
if((Get-Item -LiteralPath $source).Length -gt 4096){throw 'Helper source exceeds bound.'}
# Reborn: pin the reviewed source independently of checkout line endings and the exact locally calibrated debugger.
$normalized=[IO.File]::ReadAllText($source).Replace("`r`n","`n")
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($normalized))) -cne '1EFA171DAE77F686E2E120EB6530B37F0D359E41C836F74A11C247CD5FC1FB46'){throw 'Unreviewed lifetime helper.'}
if((Get-FileHash -LiteralPath $debugger).Hash -cne 'E6771A874286D8A053C744D428D50A13091318DD1261F07B6C2C17BF42A486A5'){throw 'Debugger identity differs from calibrated build.'}
if(-not $Run){[pscustomobject]@{Mode=$Mode;RunRequested=$false;GameExecuted=$false;Scope='Fresh x86 self-expiring helper only'};return}
if($Mode -ceq 'BreakpointDetach'){
    # Reborn: prepare read-only interop before starting the five-second helper so compilation cannot consume its observation window.
    Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
// Reborn: bounded read-only interop for the newly owned helper's breakpoint site.
public static class RebornLifetimeMemory {
    //-------------------------------------------------------------------------------------------------
    /** Reborn: use the Windows read result and actual transfer count, never infer success from requested size. */
    //-------------------------------------------------------------------------------------------------
    [DllImport("kernel32.dll", SetLastError=true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ReadProcessMemory(IntPtr process, IntPtr address, byte[] buffer, UIntPtr size, out UIntPtr actual);
}
'@
}
$directory=(New-Item -ItemType Directory -Path (Join-Path $repo ('artifacts/RebornDebuggerLifetime-'+[guid]::NewGuid().ToString('N')))).FullName
$executable=Join-Path $directory 'RebornDebuggerLifetimeControl.exe'
# Reborn: bound compilation independently; only the newly created compiler process can be terminated.
$compile=Start-Process -FilePath $compiler -ArgumentList ('/nologo /target:exe /platform:x86 /optimize+ /out:"'+$executable+'" "'+$source+'"') -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $directory 'compiler.txt') -RedirectStandardError (Join-Path $directory 'compiler-error.txt')
try{
    if(-not $compile.WaitForExit(10000)){$compile.Kill();throw 'Owned compiler exceeded deadline.'}
    if($compile.ExitCode -ne 0){throw 'Owned helper compilation failed.'}
}finally{$compile.Dispose()}
# Reborn: capture the initial target PID before resuming; fixed generated commands cannot attach or select another target.
$commandText='$$ Reborn: initial ownership and optional helper-only breakpoint detach.'+"`n"+'.printf "REBORN_OWNER %x", @$tpid; .echo'+"`n"
if($Mode -ceq 'BreakpointDetach'){
    $commandText+='.printf "REBORN_SITE %p %x", ntdll!NtDelayExecution, by(ntdll!NtDelayExecution); .echo'+"`n"
    $commandText+='bp ntdll!NtDelayExecution ".echo REBORN_BP_HIT; qd"'+"`ng`n"
}else{$commandText+=$(if($Mode -ceq 'Detach'){"qd`n"}else{"g`n"})}
$commandPath=Join-Path $directory 'commands.txt'
[IO.File]::WriteAllText($commandPath,$commandText)
# Reborn: -pd requests detach on debugger exit; do not follow children, attach, open a server or use external symbols.
$arguments='-pd -G -noshell -nosqm -sins -y "'+$directory+'" -cf "'+$commandPath+'" "'+$executable+'" "'+$directory+'"'
$debug=Start-Process -FilePath $debugger -ArgumentList $arguments -WorkingDirectory $directory -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $directory 'debugger.txt') -RedirectStandardError (Join-Path $directory 'debugger-error.txt')
$helper=$null
try{
    $deadline=[DateTime]::UtcNow.AddSeconds(15)
    $started=Join-Path $directory 'started.txt'
    while(-not (Test-Path -LiteralPath $started)){
        if([DateTime]::UtcNow -gt $deadline){throw 'No owned helper identity within deadline.'}
        Start-Sleep -Milliseconds 100
    }
    # Reborn: retain an OS process handle and verify PID plus creation identity; never kill this helper or enumerate by process name.
    $identity=[IO.File]::ReadAllText($started)
    if($identity -notmatch '^(\d+)\|(\d+)$'){throw 'Malformed helper identity.'}
    $helper=[Diagnostics.Process]::GetProcessById([int]$Matches[1])
    $null=$helper.Handle
    if($helper.StartTime.ToUniversalTime().Ticks -ne [long]$Matches[2] -or $helper.MainModule.FileName -ine $executable){throw 'Helper process identity mismatch.'}
    # Reborn: bind the still-owned OS handle to CDB's direct child identity and creation window, not an arbitrary existing PID.
    $snapshot=@(Get-CimInstance Win32_Process -Filter ('ProcessId = '+$helper.Id))
    if($helper.HasExited -or $snapshot.Count -ne 1 -or $snapshot[0].ParentProcessId -ne $debug.Id -or $snapshot[0].ExecutablePath -ine $executable -or $helper.StartTime.ToUniversalTime() -lt $debug.StartTime.ToUniversalTime()){throw 'Debugger child ownership differs.'}
    if($Mode -ceq 'DebuggerLoss'){
        if($debug.HasExited -or $helper.HasExited){throw 'Debugger-loss case was not live.'}
        # Reborn: simulate loss of this single owned debugger only; -pd is tested, not assumed to protect arbitrary targets.
        $debug.Kill()
    }
    if(-not $debug.WaitForExit(5000)){throw 'Debugger did not exit within bound.'}
    # Reborn: do not wait unboundedly for inherited output handles; incomplete redirected evidence is refused by the bounded parser.
    # Reborn: retain bounded logs as diagnostics, not executable instructions or proof of target behavior.
    if((Get-Item -LiteralPath (Join-Path $directory 'debugger.txt')).Length -gt 65536 -or (Get-Item -LiteralPath (Join-Path $directory 'debugger-error.txt')).Length -gt 8192){throw 'Debugger output exceeds bound.'}
    $survived=-not $helper.HasExited
    $evidence=Get-Ep11OwnedDebuggerEvidence (Read-Ep11OwnedDebuggerLog (Join-Path $directory 'debugger.txt')) $helper.Id ($Mode -ceq 'BreakpointDetach')
    $restored=$false
    if($Mode -ceq 'BreakpointDetach'){
        # Reborn: inspect one byte through the retained owned-helper handle after detach; do not modify memory or accept an external PID.
        $buffer=[byte[]]::new(1);$actual=[UIntPtr]::Zero
        if(-not [RebornLifetimeMemory]::ReadProcessMemory($helper.Handle,[IntPtr][long]$evidence.BreakpointSite.Address,$buffer,[UIntPtr]1,[ref]$actual) -or $actual.ToUInt64() -ne 1 -or $buffer[0] -ne $evidence.BreakpointSite.OriginalByte){throw 'Detached breakpoint byte was not restored.'}
        $restored=$true
    }
    if(-not $survived -or -not $helper.WaitForExit(8000)){throw 'Helper did not survive debugger exit and finish naturally.'}
    $completed=Join-Path $directory 'completed.txt'
    if($helper.ExitCode -ne 0 -or -not (Test-Path -LiteralPath $completed) -or [IO.File]::ReadAllText($completed) -cne 'REBORN_NATURAL_COMPLETION'){throw 'Natural completion evidence missing.'}
    # Reborn: persist helper-only proof separately from raw logs; initial ownership is checked after resume, not an early game admission gate.
    $result=[pscustomobject]@{Mode=$Mode;HelperIdentity=$identity;InitialTargetProcessId=$evidence.InitialTargetProcessId;DirectChildOwnershipValidated=$true;OwnershipValidatedBeforeResume=$false;BreakpointCleanupTested=($Mode -ceq 'BreakpointDetach');BreakpointHitCount=$evidence.BreakpointHitCount;BreakpointSite=$evidence.BreakpointSite;BreakpointByteRestored=$restored;DebuggerExitCode=$debug.ExitCode;SurvivedDebuggerExit=$survived;NaturalCompletionValidated=$true;GameExecuted=$false;GameDetachValidated=$false;OutputDirectory=$directory;DebuggerSha256=(Get-FileHash -LiteralPath $debugger).Hash}
    [IO.File]::WriteAllText((Join-Path $directory 'result.json'),($result|ConvertTo-Json -Depth 4))
    $result
}finally{
    # Reborn: on observation failure terminate only our debugger; leave the self-expiring helper untouched and report no proof.
    if(-not $debug.HasExited){$debug.Kill();$null=$debug.WaitForExit(5000)}
    $debug.Dispose()
    if($null -ne $helper){$helper.Dispose()}
}
