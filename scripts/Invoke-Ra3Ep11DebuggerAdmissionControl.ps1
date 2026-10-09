# Reborn: gate one newly compiled helper at CDB's initial break; rejection cleanup detaches/resumes that helper, never a game.
[CmdletBinding()]
param([switch]$Run,[switch]$SelfTest,[ValidateSet('Admit','RejectLiveBytes','Timeout')][string]$Mode='Admit')
$ErrorActionPreference='Stop'
. (Join-Path $PSScriptRoot 'Ra3Ep11DebuggerAdmissionPolicy.ps1')
if($SelfTest){
    if($Run){throw 'Run and detached tests must be separate.'}
    # Reborn: detached admission mutations do not start a debugger or read any process memory.
    $sample="REBORN_OWNER 10`nREBORN_ADMISSION_WAIT"
    if((Get-Ep11InitialAdmissionOwner $sample) -ne 16){throw 'Initial owner positive failed.'}
    Assert-Ep11AdmissionBytes $true 2 ([byte[]]@(255,37)) ([byte[]]@(255,37))
    $faults=@($sample.Replace('OWNER 10','OWNER 0'),($sample+"`nREBORN_OWNER 10"),$sample.Replace('REBORN_ADMISSION_WAIT',''),($sample+"`nREBORN_ADMISSION_WAIT"),($sample+"`nREBORN_UNKNOWN"),($sample+"`nMemory access error"),("REBORN_ADMISSION_WAIT`nREBORN_OWNER 10"),('x'*65537))
    foreach($fault in $faults){$refused=$false;try{$null=Get-Ep11InitialAdmissionOwner $fault}catch{$refused=$true};if(-not $refused){throw 'Initial record fault admitted.'}}
    foreach($fault in @(@($false,2,[byte[]]@(255,37),[byte[]]@(255,37)),@($true,1,[byte[]]@(255,37),[byte[]]@(255,37)),@($true,2,[byte[]]@(255,37),[byte[]]@(255,38)),@($true,2,[byte[]]@(255,37),[byte[]]@(255)))){
        $refused=$false;try{Assert-Ep11AdmissionBytes $fault[0] $fault[1] $fault[2] $fault[3]}catch{$refused=$true};if(-not $refused){throw 'Initial transfer fault admitted.'}
    }
    # Reborn: a private synthetic PE32 fixture verifies live-address mapping without assuming operand relocation at initial break.
    $peBytes=[byte[]]::new(512);$peBytes[0]=0x4d;$peBytes[1]=0x5a
    foreach($field in @(@(60,0x80),@(0x80,0x4550),@(0xa8,0x2000),@(0xb4,0x400000),@(0xd0,0x3000),@(0x184,0x2000),@(0x188,64),@(0x18c,448))){[BitConverter]::GetBytes([uint32]$field[1]).CopyTo($peBytes,$field[0])}
    foreach($field in @(@(0x84,0x14c),@(0x86,1),@(0x94,224),@(0x98,0x10b))){[BitConverter]::GetBytes([uint16]$field[1]).CopyTo($peBytes,$field[0])}
    $peBytes[448]=0xff;$peBytes[449]=0x25;[BitConverter]::GetBytes([uint32]0x402000).CopyTo($peBytes,450)
    foreach($base in @(0x400000L,0x600000L)){
        $entry=Get-Ep11InitialHelperEntry $peBytes $base
        if($entry.Address -ne $base+0x2000 -or [BitConverter]::ToUInt32($entry.ExpectedBytes,2) -ne 0x402000){throw 'Live entry mapping fixture failed.'}
    }
    foreach($offset in @(0,0x84,0x98,448)){
        $fault=[byte[]]$peBytes.Clone();$fault[$offset]=$fault[$offset] -bxor 1;$refused=$false
        try{$null=Get-Ep11InitialHelperEntry $fault 0x400000}catch{$refused=$true};if(-not $refused){throw 'Helper PE fault admitted.'}
    }
    'Initial admission detached tests: PASS; four positives and sixteen refusals; no process executed.';return
}
$repo=Split-Path -Parent $PSScriptRoot
$source=Join-Path $repo 'fixtures/ra3ep11/phase-a/DebuggerLifetimeControl.cs'
$debugger='C:\Program Files (x86)\Windows Kits\10\Debuggers\x86\cdb.exe'
$compiler=Join-Path ([Environment]::GetFolderPath([Environment+SpecialFolder]::Windows)) 'Microsoft.NET/Framework/v4.0.30319/csc.exe'
$paths=@($repo,$source,$debugger,$compiler)
if(Test-Path -LiteralPath (Join-Path $repo 'artifacts')){$paths+=Join-Path $repo 'artifacts'}
# Reborn: constrain fixed executables, helper source and fresh output ancestry; no external target argument is accepted.
foreach($path in $paths){
    for($item=Get-Item -LiteralPath $path -Force;$null -ne $item;){
        if(($item.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0){throw 'Redirected admission-control path refused.'}
        $item=if($item -is [IO.FileInfo]){$item.Directory}else{$item.Parent}
    }
}
if((Get-Item -LiteralPath $source).Length -gt 4096 -or [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes([IO.File]::ReadAllText($source).Replace("`r`n","`n")))) -cne '1EFA171DAE77F686E2E120EB6530B37F0D359E41C836F74A11C247CD5FC1FB46'){throw 'Unreviewed admission helper.'}
if((Get-FileHash -LiteralPath $debugger).Hash -cne 'E6771A874286D8A053C744D428D50A13091318DD1261F07B6C2C17BF42A486A5'){throw 'Debugger differs from calibrated build.'}
if(-not $Run){[pscustomobject]@{Mode=$Mode;RunRequested=$false;GameExecuted=$false;Scope='Fixed new helper, initial-break gate only'};return}
# Reborn: prepare bounded read-only native interop before any target exists.
Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
// Reborn: read only the retained newly owned helper handle; no memory writing or process creation API.
public static class RebornAdmissionMemory {
    //-------------------------------------------------------------------------------------------------
    /** Reborn: return Windows success and actual transfer count independently. */
    //-------------------------------------------------------------------------------------------------
    [DllImport("kernel32.dll", SetLastError=true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ReadProcessMemory(IntPtr process, IntPtr address, byte[] buffer, UIntPtr size, out UIntPtr actual);
}
'@
$directory=(New-Item -ItemType Directory -Path (Join-Path $repo ('artifacts/RebornDebuggerAdmission-'+[guid]::NewGuid().ToString('N')))).FullName
$executable=Join-Path $directory 'RebornDebuggerLifetimeControl.exe'
$compile=Start-Process -FilePath $compiler -ArgumentList ('/nologo /target:exe /platform:x86 /optimize+ /out:"'+$executable+'" "'+$source+'"') -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $directory 'compiler.txt') -RedirectStandardError (Join-Path $directory 'compiler-error.txt')
try{if(-not $compile.WaitForExit(10000)){$compile.Kill();throw 'Owned compiler timeout.'};if($compile.ExitCode -ne 0){throw 'Helper compilation failed.'}}finally{$compile.Dispose()}
$bytes=[IO.File]::ReadAllBytes($executable)
$commandPath=Join-Path $directory 'initial.txt'
# Reborn: no initial g/qd command; the supervisor alone decides whether to continue observation after identity and byte admission.
[IO.File]::WriteAllText($commandPath,'$$ Reborn: wait for supervisor admission at initial break.'+"`n"+'.printf "REBORN_OWNER %x", @$tpid; .echo'+"`n"+'.echo REBORN_ADMISSION_WAIT'+"`n")
$info=[Diagnostics.ProcessStartInfo]::new()
$info.FileName=$debugger;$info.WorkingDirectory=$directory;$info.UseShellExecute=$false;$info.CreateNoWindow=$true
$info.RedirectStandardInput=$true;$info.RedirectStandardOutput=$true;$info.RedirectStandardError=$true
foreach($argument in @('-pd','-G','-noshell','-nosqm','-sins','-y',$directory,'-cf',$commandPath,$executable,$directory)){$info.ArgumentList.Add($argument)}
$debug=[Diagnostics.Process]::new();$debug.StartInfo=$info
$helper=$null;$started=$false;$admitted=$false;$resumeIssued=$false;$cleanupIssued=$false;$failure=$null;$decision=$null;$entry=$null;$beforeDecision=$false;$observed=$null;$read=$false;$actual=[UIntPtr]::Zero;$owner=$null;$debuggerId=$null;$creationTicks=$null;$forcedExit=$false
$streams=@()
#-------------------------------------------------------------------------------------------------
<# Reborn: pump bounded redirected chunks without blocking on a prompt or needing a PowerShell runspace in callbacks. #>
#-------------------------------------------------------------------------------------------------
function Update-Ep11AdmissionOutput {
    foreach($stream in $streams){
        if($null -ne $stream.Task -and $stream.Task.IsCompleted){
            $count=$stream.Task.GetAwaiter().GetResult()
            if($count -eq 0){$stream.Task=$null;continue}
            $stream.Text+=[string]::new($stream.Buffer,0,$count)
            if($stream.Text.Length -gt $stream.Limit){throw 'Admission output bound exceeded.'}
            $stream.Task=$stream.Reader.ReadAsync($stream.Buffer,0,$stream.Buffer.Length)
        }
    }
}
try{
    $started=$debug.Start();if(-not $started){throw 'Debugger did not start.'};$debuggerId=$debug.Id
    foreach($spec in @(@($debug.StandardOutput,65536),@($debug.StandardError,8192))){
        $state=[pscustomobject]@{Reader=$spec[0];Limit=$spec[1];Text='';Buffer=[char[]]::new(1024);Task=$null}
        $state.Task=$state.Reader.ReadAsync($state.Buffer,0,$state.Buffer.Length);$streams+=$state
    }
    $deadline=[DateTime]::UtcNow.AddSeconds(10)
    while($streams[0].Text -notmatch '(?m)^REBORN_ADMISSION_WAIT\r?$'){
        Update-Ep11AdmissionOutput
        if($debug.HasExited -or [DateTime]::UtcNow -gt $deadline){throw 'Initial wait not reached within deadline.'}
        Start-Sleep -Milliseconds 50
    }
    $owner=Get-Ep11InitialAdmissionOwner $streams[0].Text
    if($streams[1].Text.Length -ne 0){throw 'Initial debugger stderr is not empty.'}
    $helper=[Diagnostics.Process]::GetProcessById($owner);$null=$helper.Handle
    $snapshot=@(Get-CimInstance Win32_Process -Filter ('ProcessId = '+$owner))
    if($helper.HasExited -or $snapshot.Count -ne 1 -or $snapshot[0].ParentProcessId -ne $debug.Id -or $snapshot[0].ExecutablePath -ine $executable -or $helper.MainModule.FileName -ine $executable -or $helper.StartTime.ToUniversalTime() -lt $debug.StartTime.ToUniversalTime()){throw 'Initial helper ownership mismatch.'}
    $creationTicks=$helper.StartTime.ToUniversalTime().Ticks
    $beforeDecision=-not (Test-Path -LiteralPath (Join-Path $directory 'started.txt'))
    if(-not $beforeDecision){throw 'Helper Main marker preceded admission.'}
    $entry=Get-Ep11InitialHelperEntry $bytes $helper.MainModule.BaseAddress.ToInt64()
    $observed=[byte[]]::new(6);$actual=[UIntPtr]::Zero
    $read=[RebornAdmissionMemory]::ReadProcessMemory($helper.Handle,[IntPtr]$entry.Address,$observed,[UIntPtr]6,[ref]$actual)
    # Reborn: validate the genuine baseline first so a failed OS read cannot masquerade as a successful injected refusal.
    Assert-Ep11AdmissionBytes $read $actual.ToUInt64() $entry.ExpectedBytes $observed
    $expected=[byte[]]$entry.ExpectedBytes.Clone()
    if($Mode -ceq 'RejectLiveBytes'){
        # Reborn: corrupt only a private expected-byte snapshot, never the helper's live memory or executable.
        $expected[0]=$expected[0] -bxor 1
        $refused=$false;try{Assert-Ep11AdmissionBytes $read $actual.ToUInt64() $expected $observed}catch{$refused=$true}
        if(-not $refused){throw 'Live-byte fault injection was admitted.'};$decision='ByteMismatchRefused'
    }else{
        Assert-Ep11AdmissionBytes $read $actual.ToUInt64() $expected $observed
        if($Mode -ceq 'Timeout'){
            # Reborn: withhold admission for a bounded interval while the helper remains at initial break.
            $hold=[DateTime]::UtcNow.AddSeconds(2)
            while([DateTime]::UtcNow -lt $hold){Update-Ep11AdmissionOutput;if($debug.HasExited -or (Test-Path -LiteralPath (Join-Path $directory 'started.txt'))){throw 'Helper escaped admission hold.'};Start-Sleep -Milliseconds 50}
            $decision='AdmissionWithheldTimeout'
        }else{$admitted=$true;$decision='IdentityAndEntryAdmitted'}
    }
    if($admitted){$debug.StandardInput.WriteLine('g');$resumeIssued=$true}else{
        # Reborn: rejected observation never gets g; helper-only cleanup detaches and therefore releases it to self-expire.
        $debug.StandardInput.WriteLine('qd');$cleanupIssued=$true
    }
    $debug.StandardInput.Flush()
    $deadline=[DateTime]::UtcNow.AddSeconds(10)
    while(-not $helper.HasExited -or -not $debug.HasExited){
        Update-Ep11AdmissionOutput
        if([DateTime]::UtcNow -gt $deadline){throw 'Helper/debugger completion deadline exceeded.'};Start-Sleep -Milliseconds 50
    }
    Update-Ep11AdmissionOutput
    $identity=[IO.File]::ReadAllText((Join-Path $directory 'started.txt'))
    if($identity -cne ($owner.ToString()+'|'+$helper.StartTime.ToUniversalTime().Ticks) -or $helper.ExitCode -ne 0 -or $debug.ExitCode -ne 0 -or $streams[1].Text.Length -ne 0 -or [IO.File]::ReadAllText((Join-Path $directory 'completed.txt')) -cne 'REBORN_NATURAL_COMPLETION'){throw 'Final owned helper completion differs.'}
}catch{$failure=$_.Exception.Message}finally{
    # Reborn: on unexpected failure end only our debugger; -pd can release the helper, so rejection is not execution containment.
    if($started -and -not $debug.HasExited){$debug.Kill();$forcedExit=$true;$null=$debug.WaitForExit(5000)}
    if($streams.Count -eq 2){[IO.File]::WriteAllText((Join-Path $directory 'stdout.txt'),$streams[0].Text);[IO.File]::WriteAllText((Join-Path $directory 'stderr.txt'),$streams[1].Text)}
    $result=[pscustomobject]@{Mode=$Mode;Decision=$decision;Failure=$failure;HelperProcessId=$owner;OwnedDebuggerProcessId=$debuggerId;HelperCreationUtcTicks=$creationTicks;CompiledHelperSha256=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bytes));InitialOwnershipAndBytesAdmitted=$admitted;ObservationResumeIssued=$resumeIssued;CleanupDetachIssued=$cleanupIssued;ForcedDebuggerExit=$forcedExit;MainMarkerAbsentBeforeDecision=$beforeDecision;Entry=$entry;ObservedEntryBytes=$observed;MemoryReadSucceeded=$read;ActualEntryBytesRead=$actual.ToUInt64();NaturalCompletionValidated=($null -eq $failure);RejectedHelperCanRunDuringCleanup=$true;GameExecuted=$false;GameRecipeReady=$false;OutputDirectory=$directory}
    [IO.File]::WriteAllText((Join-Path $directory 'result.json'),($result|ConvertTo-Json -Depth 4))
    $debug.Dispose();if($null -ne $helper){$helper.Dispose()}
}
if($null -ne $failure){throw $failure}
$result
