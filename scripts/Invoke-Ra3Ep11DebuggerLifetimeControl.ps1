# Reborn: calibrate detach and debugger-loss behavior on a newly compiled x86 helper, never a game or existing process.
[CmdletBinding()]
param([switch]$Run,[ValidateSet('Detach','DebuggerLoss')][string]$Mode='Detach')
$ErrorActionPreference='Stop'
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
$directory=(New-Item -ItemType Directory -Path (Join-Path $repo ('artifacts/RebornDebuggerLifetime-'+[guid]::NewGuid().ToString('N')))).FullName
$executable=Join-Path $directory 'RebornDebuggerLifetimeControl.exe'
# Reborn: bound compilation independently; only the newly created compiler process can be terminated.
$compile=Start-Process -FilePath $compiler -ArgumentList ('/nologo /target:exe /platform:x86 /optimize+ /out:"'+$executable+'" "'+$source+'"') -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $directory 'compiler.txt') -RedirectStandardError (Join-Path $directory 'compiler-error.txt')
try{
    if(-not $compile.WaitForExit(10000)){$compile.Kill();throw 'Owned compiler exceeded deadline.'}
    if($compile.ExitCode -ne 0){throw 'Owned helper compilation failed.'}
}finally{$compile.Dispose()}
$command=if($Mode -ceq 'Detach'){'qd'}else{'g'}
# Reborn: -pd requests detach on debugger exit; do not follow children, attach, open a server or use external symbols.
$arguments='-pd -G -noshell -nosqm -sins -y "'+$directory+'" -c "'+$command+'" "'+$executable+'" "'+$directory+'"'
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
    if($Mode -ceq 'DebuggerLoss'){
        if($debug.HasExited -or $helper.HasExited){throw 'Debugger-loss case was not live.'}
        # Reborn: simulate loss of this single owned debugger only; -pd is tested, not assumed to protect arbitrary targets.
        $debug.Kill()
    }
    if(-not $debug.WaitForExit(5000)){throw 'Debugger did not exit within bound.'}
    # Reborn: retain bounded logs as diagnostics, not executable instructions or proof of target behavior.
    if((Get-Item -LiteralPath (Join-Path $directory 'debugger.txt')).Length -gt 65536 -or (Get-Item -LiteralPath (Join-Path $directory 'debugger-error.txt')).Length -gt 8192){throw 'Debugger output exceeds bound.'}
    $survived=-not $helper.HasExited
    if(-not $survived -or -not $helper.WaitForExit(8000)){throw 'Helper did not survive debugger exit and finish naturally.'}
    $completed=Join-Path $directory 'completed.txt'
    if($helper.ExitCode -ne 0 -or -not (Test-Path -LiteralPath $completed) -or [IO.File]::ReadAllText($completed) -cne 'REBORN_NATURAL_COMPLETION'){throw 'Natural completion evidence missing.'}
    [pscustomobject]@{Mode=$Mode;HelperIdentity=$identity;DebuggerExitCode=$debug.ExitCode;SurvivedDebuggerExit=$survived;NaturalCompletionValidated=$true;GameExecuted=$false;GameDetachValidated=$false;OutputDirectory=$directory;DebuggerSha256=(Get-FileHash -LiteralPath $debugger).Hash}
}finally{
    # Reborn: on observation failure terminate only our debugger; leave the self-expiring helper untouched and report no proof.
    if(-not $debug.HasExited){$debug.Kill();$null=$debug.WaitForExit(5000)}
    $debug.Dispose()
    if($null -ne $helper){$helper.Dispose()}
}
