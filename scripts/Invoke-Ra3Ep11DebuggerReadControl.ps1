# Reborn: calibrate native read observation on one newly compiled, owned x64 helper only, never on a game.
[CmdletBinding()]
param([switch]$Run,[switch]$SelfTest)
$ErrorActionPreference='Stop'
if($Run -and $SelfTest){throw 'Run and detached tests must be separate.'}
$repo=Split-Path -Parent $PSScriptRoot
$fixture=Join-Path $repo 'fixtures/ra3ep11/phase-a/config-read-only.cfg'
$source=Join-Path $repo 'fixtures/ra3ep11/phase-a/ConfigReadControl.cs'
$commands=Join-Path $repo 'fixtures/ra3ep11/phase-a/config-read-debugger.txt'
$debugger='C:\Program Files (x86)\Windows Kits\10\Debuggers\x64\cdb.exe'
$compiler=Join-Path ([Environment]::GetFolderPath([Environment+SpecialFolder]::Windows)) 'Microsoft.NET/Framework/v4.0.30319/csc.exe'

#-------------------------------------------------------------------------------------------------
<# Reborn: admit exact synchronous open/read/return evidence and discard closed or replaced handle mappings. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11DebuggerReadProof([string]$Text,[string]$ExpectedPath) {
    if($Text.Length -gt 262144 -or $Text -match '(?i)syntax error|could not resolve|unable to|invalid switch|memory access error'){throw 'Debugger log exceeds bound or reports an observation error.'}
    $handles=@{};$open=$null;$read=$null;$proof=$null;$managed=$null;$readCount=0;$openCount=0
    foreach($line in ($Text -split '\r?\n')){
        if($line -match '^REBORN_OPEN_ENTRY ([0-9a-f]+) (.+)$'){
            if($null -ne $open){throw 'Overlapping open calls are not admitted.'}
            $open=@{Thread=$Matches[1];Path=$Matches[2]};$openCount++;continue
        }
        if($line -match '^REBORN_OPEN_RETURN ([0-9a-f]+) ([0-9a-f]+) ([0-9a-f`]+)$'){
            if($null -eq $open -or $open.Thread -cne $Matches[1]){throw 'Unpaired open return.'}
            $handle=$Matches[3].Replace('`','')
            if($Matches[2] -ceq '0'){$handles[$handle]=$open.Path}
            $open=$null;continue
        }
        if($line -match '^REBORN_CLOSE ([0-9a-f]+) ([0-9a-f`]+)$'){
            $handles.Remove($Matches[2].Replace('`',''));continue
        }
        if($line -match '^REBORN_READ_ENTRY ([0-9a-f]+) ([0-9a-f]+) ([0-9a-f`]+)$'){
            if($null -ne $read){throw 'Overlapping one-byte reads are not admitted.'}
            $handle=$Matches[3].Replace('`','');$readCount++
            if(-not $handles.ContainsKey($handle) -or $handles[$handle] -cne $ExpectedPath){throw 'Read handle does not map to the exact current probe path.'}
            $read=@{Pid=[Convert]::ToInt32($Matches[1],16);Thread=$Matches[2]};continue
        }
        if($line -match '^REBORN_READ_RETURN ([0-9a-f]+) ([0-9a-f]+) ([0-9a-f`]+) ([0-9a-f`]+) ([0-9a-f]+)$'){
            if($null -eq $read -or $read.Thread -cne $Matches[1] -or $Matches[2] -cne '0' -or [Convert]::ToUInt64($Matches[3].Replace('`',''),16) -ne 0 -or [Convert]::ToUInt64($Matches[4].Replace('`',''),16) -ne 1 -or $Matches[5] -cne 'a'){throw 'Read return is not same-thread synchronous success, one actual byte, value 0A.'}
            $proof=$read;$read=$null;continue
        }
        if($line -match '^(\d+)\|READ\|1\|10$'){
            if($null -ne $managed){throw 'Duplicate managed result.'};$managed=[int]$Matches[1];continue
        }
        if($line.StartsWith('REBORN_')){throw 'Unrecognized native observation record.'}
    }
    if($null -ne $open -or $null -ne $read -or $readCount -ne 1 -or $null -eq $proof -or $managed -ne $proof.Pid){throw 'Incomplete, duplicate or mismatched owned-helper evidence.'}
    [pscustomobject]@{HelperProcessId=$managed;ObservedOpenCount=$openCount;NativeReadCount=$readCount;NativeStatus=0;ActualBytesRead=1;ByteValue=10;OwnedHelperReadValidated=$true;GameExecuted=$false;GameReadValidated=$false;WprFilterValidated=$false}
}
if($SelfTest){
    # Reborn: detached mutations exercise identity, completion, handle lifetime and ambiguity refusals without execution.
    $sample=@('REBORN_OPEN_ENTRY 20 \??\C:\probe.cfg','REBORN_OPEN_RETURN 20 0 0000000000000040','REBORN_READ_ENTRY 10 20 0000000000000040','REBORN_READ_RETURN 20 0 0000000000000000 0000000000000001 a','16|READ|1|10') -join "`n"
    $expected='\??\C:\probe.cfg'
    $null=Get-Ep11DebuggerReadProof $sample $expected
    $faults=@($sample.Replace('probe.cfg','other.cfg'),$sample.Replace('RETURN 20 0 0000000000000000','RETURN 20 103 0000000000000000'),$sample.Replace('0000000000000001 a','0000000000000000 a'),$sample.Replace('0000000000000001 a','0000000000000001 b'),$sample.Replace('16|READ','17|READ'),$sample.Replace('READ_RETURN 20','READ_RETURN 21'),$sample.Replace('REBORN_READ_ENTRY',"REBORN_CLOSE 20 0000000000000040`nREBORN_READ_ENTRY"),$sample.Replace('REBORN_READ_RETURN','REMOVED_RETURN'),($sample+"`n"+$sample))
    foreach($fault in $faults){$rejected=$false;try{$null=Get-Ep11DebuggerReadProof $fault $expected}catch{$rejected=$true};if(-not $rejected){throw 'Detached debugger evidence fault admitted.'}}
    'EP1 debugger control: detached PASS; one valid case and nine refusal cases. No compiler, debugger, helper or game executed.';return
}
$controlPaths=@($repo,$fixture,$source,$commands,$compiler,$debugger)
# Reborn: an existing artifacts directory must not redirect the fresh output outside this checkout.
if(Test-Path -LiteralPath (Join-Path $repo 'artifacts')){$controlPaths+=Join-Path $repo 'artifacts'}
foreach($path in $controlPaths){
    # Reborn: refuse redirected inputs, executables and output ancestry.
    for($item=Get-Item -LiteralPath $path -Force;$null -ne $item;){
        if(($item.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0){throw 'Reparse debugger control path not admitted.'}
        $item=if($item -is [IO.FileInfo]){$item.Directory}else{$item.Parent}
    }
}
foreach($spec in @(@($source,'195B7403B1D92BB7B9A1D18C135BC1B21381E9C6CC46281FCBB00FA415BC6B4C'),@($commands,'950C4B5CBD3A1379BF5C73833EF6AACEA3CDF26AB2742645F3CA74C904681B4C'))){
    # Reborn: pin reviewed helper/commands independently of checkout line endings.
    if((Get-Item -LiteralPath $spec[0]).Length -gt 8192){throw 'Control input too large.'}
    $value=[IO.File]::ReadAllText($spec[0]).Replace("`r`n","`n")
    $hash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($value)))
    if($hash -cne $spec[1]){throw 'Unreviewed helper or debugger commands.'}
}
if((Get-Item -LiteralPath $fixture).Length -ne 1 -or (Get-FileHash -LiteralPath $fixture).Hash -cne '01BA4719C80B6FE911B091A7C05124B64EEECE964E09C058EF8F9805DACA546B'){throw 'Probe identity differs.'}
if(-not $Run){[pscustomobject]@{ControlInputsValidated=$true;DebuggerPath=$debugger;RunRequested=$false;GameExecuted=$false;Scope='One newly owned x64 helper; no attach, children, WPR or game'};return}
# Reborn: compile only the reviewed helper to a fresh ignored run directory, not an installed executable.
$runDirectory=New-Item -ItemType Directory -Path (Join-Path $repo ('artifacts/RebornDebuggerControl-'+[guid]::NewGuid().ToString('N')))
$executable=Join-Path $runDirectory.FullName 'RebornConfigReadControl.exe'
& $compiler /nologo /target:exe /platform:x64 /optimize+ ('/out:'+$executable) $source
if($LASTEXITCODE -ne 0){throw 'Owned helper compilation failed.'}
# Reborn: disable shell commands/SQM, ignore symbol environment, use only the empty local directory and launch exactly one owned target.
$arguments='-G -noshell -nosqm -sins -y "'+$runDirectory.FullName+'" -logo "'+$runDirectory.FullName+'\debugger.log" -cf "'+$commands+'" "'+$executable+'" "'+$fixture+'"'
$stdout=Join-Path $runDirectory.FullName 'stdout.txt';$stderr=Join-Path $runDirectory.FullName 'stderr.txt'
$process=Start-Process -FilePath $debugger -ArgumentList $arguments -WindowStyle Hidden -RedirectStandardOutput $stdout -RedirectStandardError $stderr -PassThru
try{
    # Reborn: bound the owned debugger lifetime and logs; termination never enumerates or kills unrelated processes.
    $deadline=[DateTime]::UtcNow.AddSeconds(20)
    while(-not $process.WaitForExit(100)){
        if([DateTime]::UtcNow -gt $deadline -or ((Get-Item -LiteralPath $stdout).Length -gt 262144) -or ((Get-Item -LiteralPath $stderr).Length -gt 8192)){$process.Kill();$process.WaitForExit(5000)|Out-Null;throw 'Owned debugger exceeded time/output bound.'}
    }
    $process.WaitForExit()
    if($process.ExitCode -ne 0 -or (Get-Item -LiteralPath $stdout).Length -gt 262144 -or (Get-Item -LiteralPath $stderr).Length -ne 0){throw 'Owned debugger exited unsuccessfully or emitted unexpected stderr.'}
    $result=Get-Ep11DebuggerReadProof ([IO.File]::ReadAllText($stdout)) ('\??\'+$fixture)
    $result|Add-Member -NotePropertyName OutputDirectory -NotePropertyValue $runDirectory.FullName
    $result|Add-Member -NotePropertyName LogSha256 -NotePropertyValue (Get-FileHash -LiteralPath $stdout).Hash
    $result
}finally{$process.Dispose()}
