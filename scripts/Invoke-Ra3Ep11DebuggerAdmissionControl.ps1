# Reborn: gate one newly compiled helper at CDB's initial break; rejection cleanup detaches/resumes that helper, never a game.
[CmdletBinding()]
param([switch]$Run,[switch]$SelfTest,[ValidateSet('Admit','RejectLiveBytes','Timeout')][string]$Mode='Admit',[switch]$CalibrateContext,[switch]$CalibrateBreakpoint,[switch]$CalibrateDisplayProfile,
    [ValidateSet('ClearAndResume','TimeoutDetachArmed','DebuggerLossArmed')][string]$BreakpointDisposition='ClearAndResume',
    [ValidateSet('None','CaptureRefusal','ObservationTimeout')][string]$InjectObservationFailure='None')
$ErrorActionPreference='Stop'
. (Join-Path $PSScriptRoot 'Ra3Ep11DebuggerAdmissionPolicy.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep11SupervisorAdmissionGate.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep11ConfigDebuggerTransport.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep11DebuggerOutputPump.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep11DebuggerDisplayProfile.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep11NativeOutputBridge.ps1')
# Reborn: quiet stop calibration is confined to the fixed helper's normal clear/resume path; native launch and fault/loss modes are not enabled.
if($CalibrateDisplayProfile -and ($SelfTest -or -not $CalibrateBreakpoint -or $BreakpointDisposition -cne 'ClearAndResume' -or $InjectObservationFailure -cne 'None')){throw 'Display calibration requires separate helper clear/resume control.'}
# Reborn: context calibration is an explicit helper-only admission mode, never a native-game launch option.
if($CalibrateContext -and ($SelfTest -or $Mode -cne 'Admit')){throw 'Context calibration requires the separate Admit helper control.'}
# Reborn: keep breakpoint calibration separate from initial-context and refusal controls; all use the same fixed new helper only.
if($CalibrateBreakpoint -and ($SelfTest -or $CalibrateContext -or $Mode -cne 'Admit')){throw 'Breakpoint calibration requires the separate Admit helper control.'}
# Reborn: never enable an armed-breakpoint loss control outside the fixed admitted helper calibration.
if($BreakpointDisposition -cne 'ClearAndResume' -and -not $CalibrateBreakpoint){throw 'Armed breakpoint disposition requires helper breakpoint calibration.'}
# Reborn: actual armed debugger loss left the helper incomplete despite byte restoration; block replay before any file/process action.
if($Run -and $BreakpointDisposition -ceq 'DebuggerLossArmed'){throw 'Armed debugger-loss execution disabled: byte restoration did not prove helper completion.'}
# Reborn: private failure injection is restricted to the same separately admitted helper breakpoint control.
if($InjectObservationFailure -cne 'None' -and ($SelfTest -or -not $CalibrateBreakpoint -or $BreakpointDisposition -cne 'ClearAndResume')){throw 'Failure injection requires the separate clear/resume helper breakpoint control.'}
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
if(-not $Run){[pscustomobject]@{Mode=$Mode;CalibrateContextRequested=[bool]$CalibrateContext;CalibrateBreakpointRequested=[bool]$CalibrateBreakpoint;DisplayProfileCalibrationRequested=[bool]$CalibrateDisplayProfile;BreakpointDisposition=$BreakpointDisposition;ArmedDebuggerLossRunEnabled=$false;RunRequested=$false;GameExecuted=$false;Scope='Fixed new helper, initial-break gate only'};return}
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
$helper=$null;$started=$false;$admitted=$false;$resumeIssued=$false;$cleanupIssued=$false;$failure=$null;$decision=$null;$entry=$null;$beforeDecision=$false;$observed=$null;$read=$false;$actual=[UIntPtr]::Zero;$owner=$null;$debuggerId=$null;$creationTicks=$null;$forcedExit=$false;$sharedDecision=$null;$sharedFaultRefused=$false
$streams=@()
# Reborn: retain actual helper context results separately from native config consumption evidence.
$contextRecords=@();$contextRoundTripValidated=$false;$staleContextRefused=$false;$stackContextValidated=$false
# Reborn: track actual breakpoint transport and byte restoration without claiming game cleanup or config consumption.
$breakpointHitValidated=$false;$breakpointRestored=$false;$breakpointContext=$null
$breakpointSite=$null;$breakpointOriginalByte=$null
$armedBreakpointValidated=$false;$debuggerLossInjected=$false;$survivedDebuggerExit=$false;$timeoutHoldMs=0
# Reborn: distinguish an issued breakpoint command from a verified stopped event; unknown armed states must never use the old forced-kill fallback.
$breakpointCommandIssued=$false;$atOwnedBreakpoint=$false;$recoveryAttempted=$false;$recoveryValidated=$false;$recoveryFailure=$null;$injectedFailureObserved=$false
# Reborn: separate actual display/framing calibration from native stop acceptance and startup diagnostic classification.
$displayProfileValidated=$false;$quietStopFramed=$false;$quietContextFramed=$false;$quietClearFramed=$false;$startupCharsBeforeHelperMarker=0
#-------------------------------------------------------------------------------------------------
<# Reborn: pump bounded redirected chunks without blocking on a prompt or needing a PowerShell runspace in callbacks. #>
#-------------------------------------------------------------------------------------------------
function Update-Ep11AdmissionOutput {
    Update-Ep11DebuggerOutputPump $streams
}
try{
    $started=$debug.Start();if(-not $started){throw 'Debugger did not start.'};$debuggerId=$debug.Id
    $streams=New-Ep11DebuggerOutputPump $debug.StandardOutput $debug.StandardError
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
    # Reborn: connect the calibrated supervisor to the same admission decision used by the native preflight, before any continuation command.
    $helperHash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bytes))
    $helperPlan=[pscustomobject]@{Kind='OwnedClrHelper';ImageSha256=$helperHash;PreferredBase=$entry.LiveBase;SizeOfImage=$entry.SizeOfImage;Ranges=@([pscustomobject]@{Name='HelperInitialEntry';Rva=$entry.EntryRva;Address=$entry.Address;Length=6;ExpectedSha256=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($entry.ExpectedBytes))})}
    $context=[pscustomobject]@{ProcessId=$helper.Id;DebuggerProcessId=$debug.Id;SnapshotCount=$snapshot.Count;SnapshotProcessId=$snapshot[0].ProcessId;ParentProcessId=$snapshot[0].ParentProcessId;TargetHasExited=$helper.HasExited;DebuggerHasExited=$debug.HasExited;ExpectedExecutablePath=$executable;ActualExecutablePath=$helper.MainModule.FileName;SnapshotExecutablePath=$snapshot[0].ExecutablePath;CreationUtcTicks=$creationTicks;DebuggerCreationUtcTicks=$debug.StartTime.ToUniversalTime().Ticks;SnapshotCreationUtcTicks=$snapshot[0].CreationDate.ToUniversalTime().Ticks;ObservedUtcTicks=[DateTime]::UtcNow.Ticks;DiskImageSha256=$helperHash;ModuleBase=$entry.LiveBase;ModuleSize=$helper.MainModule.ModuleMemorySize}
    $adapter={param($range)
        # Reborn: this callback reads only through the retained owned-helper handle, not a caller-supplied PID.
        $buffer=[byte[]]::new($range.Length);$transferred=[UIntPtr]::Zero
        $success=[RebornAdmissionMemory]::ReadProcessMemory($helper.Handle,[IntPtr]$range.Address,$buffer,[UIntPtr]$range.Length,[ref]$transferred)
        [pscustomobject]@{Name=$range.Name;Address=$range.Address;Succeeded=$success;ActualCount=$transferred.ToUInt64();Bytes=$buffer}
    }
    $sharedDecision=Get-Ep11SupervisorAdmissionDecision $helperPlan $context $streams[0].Text $adapter
    if(-not $sharedDecision.CanIssueObservationContinue){throw 'Shared supervisor admission did not permit observation.'}
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
        # Reborn: also reject the private expected-byte fault through the shared gate; unrelated OS/identity failures cannot count as this success.
        $faultPlan=$helperPlan.PSObject.Copy();$faultPlan.Ranges=@($helperPlan.Ranges[0].PSObject.Copy())
        $faultPlan.Ranges[0].ExpectedSha256=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($expected))
        try{$null=Get-Ep11SupervisorAdmissionDecision $faultPlan $context $streams[0].Text $adapter}catch{
            if($_.Exception.Message -cne 'Supervisor code/table bytes differ.'){throw};$sharedFaultRefused=$true
        }
        if(-not $sharedFaultRefused){throw 'Shared gate admitted injected helper byte fault.'}
    }else{
        Assert-Ep11AdmissionBytes $read $actual.ToUInt64() $expected $observed
        if($Mode -ceq 'Timeout'){
            # Reborn: withhold admission for a bounded interval while the helper remains at initial break.
            $hold=[DateTime]::UtcNow.AddSeconds(2)
            while([DateTime]::UtcNow -lt $hold){Update-Ep11AdmissionOutput;if($debug.HasExited -or (Test-Path -LiteralPath (Join-Path $directory 'started.txt'))){throw 'Helper escaped admission hold.'};Start-Sleep -Milliseconds 50}
            $decision='AdmissionWithheldTimeout'
        }else{$admitted=$true;$decision='IdentityAndEntryAdmitted'}
    }
    if($admitted -and $CalibrateContext){
        # Reborn: request registers twice at the admitted initial break; no breakpoint, context write or continuation occurs between captures.
        $contextSession=New-Ep11ConfigSnapshotSession $owner (Join-Path $repo 'fixtures/ra3ep11/phase-a/probe_1.0.cfg') {throw 'Context-only calibration must not invoke the config memory adapter.'}
        $lastWindow=$null
        foreach($round in @(1,2)){
            $request=New-Ep11ConfigContextRequest $contextSession
            Update-Ep11AdmissionOutput;$offset=$streams[0].Text.Length
            $debug.StandardInput.WriteLine($request.Command+'; .echo REBORN_CONTEXT_END '+$request.Nonce);$debug.StandardInput.Flush()
            $contextDeadline=[DateTime]::UtcNow.AddSeconds(5)
            do{
                Update-Ep11AdmissionOutput;$response=$streams[0].Text.Substring($offset)
                if($response.Length -gt 4096 -or $debug.HasExited -or $helper.HasExited -or [DateTime]::UtcNow -gt $contextDeadline -or $streams[1].Text.Length -ne 0){throw 'Owned helper context response failed or exceeded bounds.'}
                if($response -notmatch ('(?m)^REBORN_CONTEXT_END '+$request.Nonce+'\r?\n')){Start-Sleep -Milliseconds 25}
            }while($response -notmatch ('(?m)^REBORN_CONTEXT_END '+$request.Nonce+'\r?\n'))
            $window=Get-Ep11CompletedContextWindow $response $request.Nonce
            $record=Get-Ep11ConfigContextRecord $window $request.Nonce
            if($record.ProcessId -ne $owner -or $record.Esp -le 0 -or $record.SiteAddress -le 0 -or (Test-Path -LiteralPath (Join-Path $directory 'started.txt'))){throw 'Owned helper context identity/hold mismatch.'}
            if($round -eq 2){
                foreach($field in @('ProcessId','ThreadId','SiteAddress','Eax','Ebx','Esi','Edi','Esp')){if($record.$field -ne $contextRecords[0].$field){throw 'Helper context changed without continuation.'}}
                try{$null=Get-Ep11ConfigContextRecord $lastWindow $request.Nonce}catch{
                    if($_.Exception.Message -cne 'Context nonce or register format differs.'){throw};$staleContextRefused=$true
                }
                if(-not $staleContextRefused){throw 'Actual stale helper context response admitted.'}
            }
            # Reborn: bind returned ESP to a real read through the retained owned helper handle, not a synthetic memory callback.
            $stack=[byte[]]::new(4);$stackActual=[UIntPtr]::Zero
            $stackSuccess=[RebornAdmissionMemory]::ReadProcessMemory($helper.Handle,[IntPtr]$record.Esp,$stack,[UIntPtr]4,[ref]$stackActual)
            if(-not $stackSuccess -or $stackActual.ToUInt64() -ne 4){throw 'Owned helper context stack capture incomplete.'}
            if($round -eq 2 -and [Convert]::ToHexString($stack) -cne $contextRecords[0].StackHex){throw 'Helper stack changed without continuation.'}
            $record|Add-Member StackHex ([Convert]::ToHexString($stack));$contextRecords+=$record
            $lastWindow=$window;$contextSession.PendingContext=$null
        }
        $contextSession.Status='Closed';$contextRoundTripValidated=$true;$stackContextValidated=$true
    }
    if($admitted -and $CalibrateBreakpoint){
        # Reborn: query the already-calibrated helper delay site after admission; CLR startup need not execute the PE entry thunk.
        $siteNonce=[guid]::NewGuid().ToString('N');Update-Ep11AdmissionOutput;$siteOffset=$streams[0].Text.Length
        $debug.StandardInput.WriteLine('.printf "REBORN_SITE '+$siteNonce+' %x %x", ntdll!NtDelayExecution, by(ntdll!NtDelayExecution); .echo');$debug.StandardInput.Flush()
        $siteDeadline=[DateTime]::UtcNow.AddSeconds(5)
        do{
            Update-Ep11AdmissionOutput;$siteText=$streams[0].Text.Substring($siteOffset)
            if($siteText.Length -gt 4096 -or $debug.HasExited -or $helper.HasExited -or [DateTime]::UtcNow -gt $siteDeadline -or $streams[1].Text.Length -ne 0){throw 'Helper delay-site query failed.'}
            $siteDone=$siteText -cmatch ('(?m)^REBORN_SITE '+$siteNonce+' [0-9a-f]+ [0-9a-f]+\r?\n')
            # Reborn: display calibration starts at a complete prompt boundary, never in a trailing fragment of the previous site response.
            if($CalibrateDisplayProfile){$siteDone=$siteDone -and $siteText -cmatch '(?m)^[0-9a-fA-F]{1,8}:[0-9a-fA-F]{1,8}> \z'}
            if(-not $siteDone){Start-Sleep -Milliseconds 25}
        }while(-not $siteDone)
        $site=Get-Ep11HelperBreakpointSite $siteText $siteNonce
        $siteByte=[byte[]]::new(1);$siteActual=[UIntPtr]::Zero
        $siteSuccess=[RebornAdmissionMemory]::ReadProcessMemory($helper.Handle,[IntPtr]$site.Address,$siteByte,[UIntPtr]1,[ref]$siteActual)
        Assert-Ep11AdmissionBytes $siteSuccess $siteActual.ToUInt64() ([byte[]]@($site.OriginalByte)) $siteByte
        $breakpointSite=$site.Address;$breakpointOriginalByte=$site.OriginalByte
        if($CalibrateDisplayProfile){
            # Reborn: configure only debugger display after ownership/byte admission and before installing the helper breakpoint.
            $displayRequest=New-Ep11DebuggerDisplayRequest;Update-Ep11AdmissionOutput;$displayOffset=$streams[0].Text.Length
            $debug.StandardInput.WriteLine($displayRequest.Command);$debug.StandardInput.Flush();$displayDeadline=[DateTime]::UtcNow.AddSeconds(5)
            do{
                Update-Ep11AdmissionOutput;$displayText=$streams[0].Text.Substring($displayOffset)
                if($displayText.Length -gt 4096 -or $debug.HasExited -or $helper.HasExited -or [DateTime]::UtcNow -gt $displayDeadline -or $streams[1].Text.Length -ne 0){throw 'Display profile acknowledgement failed.'}
                $displayDone=$displayText -cmatch ('(?m)^REBORN_DISPLAY '+$displayRequest.Nonce+'\r?\n') -and $displayText -cmatch '(?m)^[0-9a-fA-F]{1,8}:[0-9a-fA-F]{1,8}> \z'
                if(-not $displayDone){Start-Sleep -Milliseconds 25}
            }while(-not $displayDone)
            Assert-Ep11DebuggerDisplayProfile $displayText $displayRequest.Nonce;$displayProfileValidated=$true
        }
        # Reborn: install only the owned helper's one observed delay-site breakpoint, without automatic continuation in its command.
        $breakNonce=[guid]::NewGuid().ToString('N');$siteHex='{0:x}' -f $breakpointSite
        Update-Ep11AdmissionOutput;$breakOffset=$streams[0].Text.Length
        $breakCommand='bp0 '+$siteHex+' ".printf \"REBORN_BREAK '+$breakNonce+' %x %x %x\", @$tpid, @$tid, @eip; .echo"'
        # Reborn: mark possible arming before writing; a partial/broken pipe must not enter the unarmed forced-exit fallback.
        $breakpointCommandIssued=$true;$debug.StandardInput.WriteLine($breakCommand)
        $debug.StandardInput.WriteLine('g');$debug.StandardInput.Flush();$resumeIssued=$true
        $breakDeadline=[DateTime]::UtcNow.AddSeconds(10)
        do{
            Update-Ep11AdmissionOutput;$breakText=$streams[0].Text.Substring($breakOffset)
            if($debug.HasExited -or $helper.HasExited -or [DateTime]::UtcNow -gt $breakDeadline -or $streams[1].Text.Length -ne 0){throw 'Owned helper breakpoint not reached within bound.'}
            if($breakText -notmatch ('(?m)^REBORN_BREAK '+$breakNonce+' [0-9a-f]+ [0-9a-f]+ [0-9a-f]+\r?\n')){Start-Sleep -Milliseconds 25}
        }while($breakText -notmatch ('(?m)^REBORN_BREAK '+$breakNonce+' [0-9a-f]+ [0-9a-f]+ [0-9a-f]+\r?\n'))
        $hit=Get-Ep11HelperBreakpointHit $breakText $breakNonce $owner $breakpointSite
        $atOwnedBreakpoint=$true
        if($CalibrateDisplayProfile){
            # Reborn: calibrate only the suffix beginning at the already ownership-validated helper marker; startup diagnostics remain separately unresolved for native use.
            $helperMarker=[regex]::Match($breakText,'(?m)^REBORN_BREAK '+$breakNonce+' ')
            $startupCharsBeforeHelperMarker=$helperMarker.Index;$quietDeadline=[DateTime]::UtcNow.AddSeconds(5)
            do{
                Update-Ep11AdmissionOutput;$quietText=$streams[0].Text.Substring($breakOffset+$helperMarker.Index)
                if($debug.HasExited -or $helper.HasExited -or [DateTime]::UtcNow -gt $quietDeadline -or $streams[1].Text.Length -ne 0){throw 'Quiet helper stop framing failed.'}
                $quietFrame=Get-Ep11NativeOutputFrame $quietText 'AwaitHelperStop' $breakNonce
                if($null -eq $quietFrame){Start-Sleep -Milliseconds 25}
            }while($null -eq $quietFrame)
            $null=Get-Ep11HelperBreakpointHit $quietFrame.Text $breakNonce $owner $breakpointSite;$quietStopFramed=$true
        }
        $contextSession=New-Ep11ConfigSnapshotSession $owner (Join-Path $repo 'fixtures/ra3ep11/phase-a/probe_1.0.cfg') {throw 'Helper breakpoint must not invoke native config decoding.'}
        $request=New-Ep11ConfigContextRequest $contextSession
        Update-Ep11AdmissionOutput;$offset=$streams[0].Text.Length
        $debug.StandardInput.WriteLine($request.Command+'; .echo REBORN_CONTEXT_END '+$request.Nonce);$debug.StandardInput.Flush()
        $contextDeadline=[DateTime]::UtcNow.AddSeconds(5)
        do{
            Update-Ep11AdmissionOutput;$response=$streams[0].Text.Substring($offset)
            if($response.Length -gt 4096 -or $debug.HasExited -or $helper.HasExited -or [DateTime]::UtcNow -gt $contextDeadline -or $streams[1].Text.Length -ne 0){throw 'Breakpoint context capture failed.'}
            $contextDone=$response -cmatch ('(?m)^REBORN_CONTEXT_END '+$request.Nonce+'\r?\n')
            if($contextDone -and $CalibrateDisplayProfile){$contextFrame=Get-Ep11NativeOutputFrame $response 'AwaitContext' $request.Nonce;$contextDone=$null -ne $contextFrame}
            if(-not $contextDone){Start-Sleep -Milliseconds 25}
        }while(-not $contextDone)
        $window=Get-Ep11CompletedContextWindow $(if($CalibrateDisplayProfile){$contextFrame.Text}else{$response}) $request.Nonce
        if($CalibrateDisplayProfile){$quietContextFramed=$true}
        $breakpointContext=Get-Ep11ConfigContextRecord $window $request.Nonce
        if($breakpointContext.ProcessId -ne $owner -or $breakpointContext.ThreadId -ne $hit.ThreadId -or $breakpointContext.SiteAddress -ne $breakpointSite){throw 'Breakpoint context does not match owned hit.'}
        $contextSession.PendingContext=$null;$contextSession.Status='Closed';$breakpointHitValidated=$true
        if($InjectObservationFailure -cne 'None'){
            # Reborn: inject only a supervisor failure after a real owned stop, never corrupt target registers, memory or input files.
            if($InjectObservationFailure -ceq 'ObservationTimeout'){
                $injectedDeadline=[DateTime]::UtcNow.AddSeconds(1)
                while([DateTime]::UtcNow -lt $injectedDeadline){Update-Ep11AdmissionOutput;if($debug.HasExited -or $helper.HasExited){throw 'Helper lost during injected timeout hold.'};Start-Sleep -Milliseconds 25}
            }
            $injectedFailureObserved=$true;throw ('Reborn injected observation failure: '+$InjectObservationFailure)
        }
        if($BreakpointDisposition -cne 'ClearAndResume'){
            # Reborn: establish an enabled breakpoint in CDB's table before testing loss, rather than assuming it remains armed after the hit.
            $armedNonce=[guid]::NewGuid().ToString('N');Update-Ep11AdmissionOutput;$armedOffset=$streams[0].Text.Length
            $debug.StandardInput.WriteLine('bl; .echo REBORN_ARMED '+$armedNonce);$debug.StandardInput.Flush()
            $armedDeadline=[DateTime]::UtcNow.AddSeconds(5)
            do{
                Update-Ep11AdmissionOutput;$armedText=$streams[0].Text.Substring($armedOffset)
                if($armedText.Length -gt 4096 -or $debug.HasExited -or $helper.HasExited -or [DateTime]::UtcNow -gt $armedDeadline -or $streams[1].Text.Length -ne 0){throw 'Armed breakpoint listing failed.'}
                if($armedText -notmatch ('(?m)^REBORN_ARMED '+$armedNonce+'\r?\n')){Start-Sleep -Milliseconds 25}
            }while($armedText -notmatch ('(?m)^REBORN_ARMED '+$armedNonce+'\r?\n'))
            Assert-Ep11HelperBreakpointArmed $armedText $armedNonce $breakpointSite
            $armedBreakpointValidated=$true
            if($BreakpointDisposition -ceq 'TimeoutDetachArmed'){
                # Reborn: model a bounded observation timeout by withholding continuation, then detach without first clearing the armed breakpoint.
                $holdStart=[DateTime]::UtcNow;$holdDeadline=$holdStart.AddSeconds(2)
                while([DateTime]::UtcNow -lt $holdDeadline){
                    Update-Ep11AdmissionOutput
                    if($debug.HasExited -or $helper.HasExited -or (Test-Path -LiteralPath (Join-Path $directory 'completed.txt'))){throw 'Helper escaped armed timeout hold.'}
                    Start-Sleep -Milliseconds 25
                }
                $timeoutHoldMs=[int]([DateTime]::UtcNow-$holdStart).TotalMilliseconds
                $debug.StandardInput.WriteLine('qd');$debug.StandardInput.Flush();$cleanupIssued=$true
            }else{
                # Reborn: intentionally terminate only the retained newly owned debugger; never kill or enumerate helpers/games by name.
                $debug.Kill();$forcedExit=$true;$debuggerLossInjected=$true
            }
            $exitDeadline=[DateTime]::UtcNow.AddSeconds(5)
            while(-not $debug.HasExited){Update-Ep11AdmissionOutput;if([DateTime]::UtcNow -gt $exitDeadline){throw 'Owned debugger exit deadline exceeded.'};Start-Sleep -Milliseconds 25}
            $survivedDebuggerExit=-not $helper.HasExited
            if(-not $survivedDebuggerExit){throw 'Helper did not survive armed debugger exit.'}
            # Reborn: inspect the restored byte through the retained helper handle after debugger exit, while this helper is still alive.
            $restored=[byte[]]::new(1);$restoredActual=[UIntPtr]::Zero
            $restoredSuccess=[RebornAdmissionMemory]::ReadProcessMemory($helper.Handle,[IntPtr]$breakpointSite,$restored,[UIntPtr]1,[ref]$restoredActual)
            Assert-Ep11AdmissionBytes $restoredSuccess $restoredActual.ToUInt64() ([byte[]]@($breakpointOriginalByte)) $restored
            $breakpointRestored=$true
        }else{
        # Reborn: clear exactly our breakpoint and await its acknowledgement while stopped, then independently verify its original code byte.
        $clearNonce=[guid]::NewGuid().ToString('N');Update-Ep11AdmissionOutput;$clearOffset=$streams[0].Text.Length
        $debug.StandardInput.WriteLine('bc 0; bl; .echo REBORN_CLEAR '+$clearNonce);$debug.StandardInput.Flush()
        $clearDeadline=[DateTime]::UtcNow.AddSeconds(5)
        do{
            Update-Ep11AdmissionOutput;$clearText=$streams[0].Text.Substring($clearOffset)
            if($clearText.Length -gt 4096 -or $debug.HasExited -or $helper.HasExited -or [DateTime]::UtcNow -gt $clearDeadline -or $streams[1].Text.Length -ne 0){throw 'Breakpoint clear acknowledgement failed.'}
            $clearDone=$clearText -cmatch ('(?m)^REBORN_CLEAR '+$clearNonce+'\r?\n')
            if($clearDone -and $CalibrateDisplayProfile){$clearFrame=Get-Ep11NativeOutputFrame $clearText 'AwaitClear' $clearNonce;$clearDone=$null -ne $clearFrame}
            if(-not $clearDone){Start-Sleep -Milliseconds 25}
        }while(-not $clearDone)
        Assert-Ep11HelperBreakpointCleared $(if($CalibrateDisplayProfile){$clearFrame.Text}else{$clearText}) $clearNonce
        if($CalibrateDisplayProfile){$quietClearFramed=$true}
        $breakpointCommandIssued=$false
        $restored=[byte[]]::new(1);$restoredActual=[UIntPtr]::Zero
        $restoredSuccess=[RebornAdmissionMemory]::ReadProcessMemory($helper.Handle,[IntPtr]$breakpointSite,$restored,[UIntPtr]1,[ref]$restoredActual)
        Assert-Ep11AdmissionBytes $restoredSuccess $restoredActual.ToUInt64() ([byte[]]@($breakpointOriginalByte)) $restored
        $breakpointRestored=$true
        $debug.StandardInput.WriteLine('g');$atOwnedBreakpoint=$false
        }
    }elseif($admitted){$debug.StandardInput.WriteLine('g');$resumeIssued=$true}else{
        # Reborn: rejected observation never gets g; helper-only cleanup detaches and therefore releases it to self-expire.
        $debug.StandardInput.WriteLine('qd');$cleanupIssued=$true
    }
    if(-not $debug.HasExited){$debug.StandardInput.Flush()}
    $deadline=[DateTime]::UtcNow.AddSeconds(10)
    while(-not $helper.HasExited -or -not $debug.HasExited){
        Update-Ep11AdmissionOutput
        if([DateTime]::UtcNow -gt $deadline){throw 'Helper/debugger completion deadline exceeded.'};Start-Sleep -Milliseconds 50
    }
    Update-Ep11AdmissionOutput
    $identity=[IO.File]::ReadAllText((Join-Path $directory 'started.txt'))
    if($identity -cne ($owner.ToString()+'|'+$helper.StartTime.ToUniversalTime().Ticks) -or $helper.ExitCode -ne 0 -or
       (-not $debuggerLossInjected -and $debug.ExitCode -ne 0) -or $streams[1].Text.Length -ne 0 -or
       [IO.File]::ReadAllText((Join-Path $directory 'completed.txt')) -cne 'REBORN_NATURAL_COMPLETION'){throw 'Final owned helper completion differs.'}
}catch{$failure=$_.Exception.Message}finally{
    if($started -and -not $debug.HasExited){
        if($breakpointCommandIssued){
            # Reborn: recover only a verified owned stopped helper cooperatively; armed unknown states cannot be made safe by killing the debugger.
            $recoveryAttempted=$true
            try{
                if(-not $admitted -or -not $atOwnedBreakpoint -or $null -eq $helper -or $helper.HasExited){throw 'No verified owned stop for cooperative breakpoint recovery.'}
                $recoveryNonce=[guid]::NewGuid().ToString('N');Update-Ep11AdmissionOutput;$recoveryOffset=$streams[0].Text.Length
                $debug.StandardInput.WriteLine('bc 0; bl; .echo REBORN_CLEAR '+$recoveryNonce);$debug.StandardInput.Flush()
                $recoveryDeadline=[DateTime]::UtcNow.AddSeconds(5)
                do{
                    Update-Ep11AdmissionOutput;$recoveryText=$streams[0].Text.Substring($recoveryOffset)
                    if($recoveryText.Length -gt 4096 -or $debug.HasExited -or $helper.HasExited -or [DateTime]::UtcNow -gt $recoveryDeadline -or $streams[1].Text.Length -ne 0){throw 'Cooperative clear acknowledgement failed.'}
                    if($recoveryText -notmatch ('(?m)^REBORN_CLEAR '+$recoveryNonce+'\r?\n')){Start-Sleep -Milliseconds 25}
                }while($recoveryText -notmatch ('(?m)^REBORN_CLEAR '+$recoveryNonce+'\r?\n'))
                Assert-Ep11HelperBreakpointCleared $recoveryText $recoveryNonce;$breakpointCommandIssued=$false
                $recoveryByte=[byte[]]::new(1);$recoveryActual=[UIntPtr]::Zero
                $recoveryRead=[RebornAdmissionMemory]::ReadProcessMemory($helper.Handle,[IntPtr]$breakpointSite,$recoveryByte,[UIntPtr]1,[ref]$recoveryActual)
                Assert-Ep11AdmissionBytes $recoveryRead $recoveryActual.ToUInt64() ([byte[]]@($breakpointOriginalByte)) $recoveryByte
                $debug.StandardInput.WriteLine('qd');$debug.StandardInput.Flush();$cleanupIssued=$true
                $recoveryDeadline=[DateTime]::UtcNow.AddSeconds(5)
                while(-not $debug.HasExited){Update-Ep11AdmissionOutput;if([DateTime]::UtcNow -gt $recoveryDeadline){throw 'Cooperative detach did not finish.'};Start-Sleep -Milliseconds 25}
                if($helper.HasExited){throw 'Helper did not survive cooperative detach for byte validation.'}
                # Reborn: repeat the actual byte check after debugger exit, not just while its breakpoint could be temporarily restored.
                $recoveryActual=[UIntPtr]::Zero
                $recoveryRead=[RebornAdmissionMemory]::ReadProcessMemory($helper.Handle,[IntPtr]$breakpointSite,$recoveryByte,[UIntPtr]1,[ref]$recoveryActual)
                Assert-Ep11AdmissionBytes $recoveryRead $recoveryActual.ToUInt64() ([byte[]]@($breakpointOriginalByte)) $recoveryByte
                $breakpointRestored=$true;$survivedDebuggerExit=$true;$recoveryDeadline=[DateTime]::UtcNow.AddSeconds(8)
                while(-not $helper.HasExited){Update-Ep11AdmissionOutput;if([DateTime]::UtcNow -gt $recoveryDeadline){throw 'Recovered helper did not complete naturally.'};Start-Sleep -Milliseconds 25}
                Update-Ep11AdmissionOutput
                if($helper.ExitCode -ne 0 -or $debug.ExitCode -ne 0 -or $streams[1].Text.Length -ne 0 -or
                   [IO.File]::ReadAllText((Join-Path $directory 'started.txt')) -cne ($owner.ToString()+'|'+$creationTicks) -or
                   [IO.File]::ReadAllText((Join-Path $directory 'completed.txt')) -cne 'REBORN_NATURAL_COMPLETION'){throw 'Recovered helper completion evidence differs.'}
                $recoveryValidated=$true
            }catch{$recoveryFailure=$_.Exception.Message}
        }else{
            # Reborn: retain the legacy helper-only fallback solely when no installed software breakpoint remains in our tracked state.
            $debug.Kill();$forcedExit=$true;$null=$debug.WaitForExit(5000)
        }
    }
    if($streams.Count -eq 2){[IO.File]::WriteAllText((Join-Path $directory 'stdout.txt'),$streams[0].Text);[IO.File]::WriteAllText((Join-Path $directory 'stderr.txt'),$streams[1].Text)}
    $result=[pscustomobject]@{Mode=$Mode;Decision=$decision;Failure=$failure;HelperProcessId=$owner;OwnedDebuggerProcessId=$debuggerId;HelperCreationUtcTicks=$creationTicks;CompiledHelperSha256=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bytes));SharedAdmissionDecision=$sharedDecision;SharedGateInjectedByteFaultRefused=$sharedFaultRefused;InitialOwnershipAndBytesAdmitted=$admitted;ObservationResumeIssued=$resumeIssued;CleanupDetachIssued=$cleanupIssued;ForcedDebuggerExit=$forcedExit;MainMarkerAbsentBeforeDecision=$beforeDecision;Entry=$entry;ObservedEntryBytes=$observed;MemoryReadSucceeded=$read;ActualEntryBytesRead=$actual.ToUInt64();NaturalCompletionValidated=($null -eq $failure);RejectedHelperCanRunDuringCleanup=$true;GameExecuted=$false;GameRecipeReady=$false;OutputDirectory=$directory}
    # Reborn: helper-only context proof does not validate any native config observation site or loaded mod.
    $result|Add-Member ContextCalibrationRequested ([bool]$CalibrateContext)
    $result|Add-Member ContextRoundTripValidated $contextRoundTripValidated
    $result|Add-Member ContextRecords $contextRecords
    $result|Add-Member ActualStaleContextRefused $staleContextRefused
    $result|Add-Member OwnedContextStackReadValidated $stackContextValidated
    $result|Add-Member NativeConfigReadProven $false
    $result|Add-Member BreakpointCalibrationRequested ([bool]$CalibrateBreakpoint)
    $result|Add-Member OwnedBreakpointHitValidated $breakpointHitValidated
    $result|Add-Member BreakpointContext $breakpointContext
    $result|Add-Member BreakpointOriginalByteRestored $breakpointRestored
    $result|Add-Member BreakpointSite $breakpointSite
    $result|Add-Member BreakpointOriginalByte $breakpointOriginalByte
    # Reborn: distinguish intentional helper-only debugger termination from successful natural debugger exit.
    $result|Add-Member BreakpointDisposition $BreakpointDisposition
    $result|Add-Member ArmedBreakpointTableValidated $armedBreakpointValidated
    $result|Add-Member IntentionalOwnedDebuggerLoss $debuggerLossInjected
    $result|Add-Member SurvivedArmedDebuggerExit $survivedDebuggerExit
    $result|Add-Member ArmedTimeoutHoldMs $timeoutHoldMs
    $result|Add-Member OwnedDebuggerExitCode $(if($debug.HasExited){$debug.ExitCode}else{$null})
    # Reborn: an observation failure remains a failure even when cooperative helper cleanup succeeds.
    $result|Add-Member InjectObservationFailure $InjectObservationFailure
    $result|Add-Member InjectedFailureObserved $injectedFailureObserved
    $result|Add-Member CooperativeRecoveryAttempted $recoveryAttempted
    $result|Add-Member CooperativeRecoveryValidated $recoveryValidated
    $result|Add-Member RecoveryFailure $recoveryFailure
    $result|Add-Member RecoveryNeedsUserAction ($null -ne $recoveryFailure)
    # Reborn: helper-only display/framing proof does not validate native startup events, five-site detach or config consumption.
    $result|Add-Member DisplayProfileCalibrationRequested ([bool]$CalibrateDisplayProfile)
    $result|Add-Member DisplayProfileReportValidated $displayProfileValidated
    $result|Add-Member QuietHelperStopFramed $quietStopFramed
    $result|Add-Member QuietHelperContextFramed $quietContextFramed
    $result|Add-Member QuietHelperClearFramed $quietClearFramed
    $result|Add-Member StartupCharactersBeforeHelperMarker $startupCharsBeforeHelperMarker
    [IO.File]::WriteAllText((Join-Path $directory 'result.json'),($result|ConvertTo-Json -Depth 4))
    $debug.Dispose();if($null -ne $helper){$helper.Dispose()}
}
if($null -ne $failure -and -not ($injectedFailureObserved -and $failure -ceq ('Reborn injected observation failure: '+$InjectObservationFailure) -and $recoveryValidated)){throw $failure}
$result
