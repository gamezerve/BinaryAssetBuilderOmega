# Reborn: capture one positive and one differently named negative helper read in a unique owned WPR instance; never execute a game or cancel someone else's trace.
[CmdletBinding()]
param([switch]$Record,[switch]$SelfTest,[switch]$ValidateHelpers)
$ErrorActionPreference='Stop'
if(([int]$Record.IsPresent+[int]$SelfTest.IsPresent+[int]$ValidateHelpers.IsPresent)-gt 1){throw 'Detached tests, helper-only validation and live recording must be separate invocations.'}
$repoRoot=Split-Path -Parent $PSScriptRoot
$fixture=Join-Path $repoRoot 'fixtures/ra3ep11/phase-a/config-read-only.cfg'
$source=Join-Path $repoRoot 'fixtures/ra3ep11/phase-a/ConfigReadControl.cs'
$profile=Join-Path $repoRoot 'fixtures/ra3ep11/phase-a/config-read-control.wprp'
$wpr=Join-Path ([Environment]::GetFolderPath([Environment+SpecialFolder]::Windows)) 'System32/wpr.exe'
$compiler=Join-Path ([Environment]::GetFolderPath([Environment+SpecialFolder]::Windows)) 'Microsoft.NET/Framework/v4.0.30319/csc.exe'
foreach($path in @($repoRoot,$fixture,$source,$profile)){
    # Reborn: reject reparse ancestry for all explicit inputs and the output parent before creating an owned run directory.
    $item=Get-Item -LiteralPath $path -Force
    for($ancestor=$item;$null-ne $ancestor;){
        if(($ancestor.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Reparse control path not admitted.'}
        $ancestor=if($ancestor-is [IO.FileInfo]){$ancestor.Directory}else{$ancestor.Parent}
    }
}
if((Get-Item -LiteralPath $fixture).Length-ne 1 -or (Get-FileHash -LiteralPath $fixture -Algorithm SHA256).Hash-cne '01BA4719C80B6FE911B091A7C05124B64EEECE964E09C058EF8F9805DACA546B'){throw 'Control fixture identity differs.'}
if((Get-Item -LiteralPath $source).Length-gt 8192 -or (Get-Item -LiteralPath $profile).Length-gt 8192){throw 'Control input size exceeds policy.'}
#-------------------------------------------------------------------------------------------------
<# Reborn: admit only the reviewed control source and exact capture scope, with DTD/external resolution disabled. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11ControlInputs([string]$SourceText,[string]$ProfileText) {
    if($SourceText.Length-gt 8192 -or $ProfileText.Length-gt 8192){throw 'Control text bound exceeded.'}
    $sourceHash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($SourceText.Replace("`r`n","`n"))))
    if($sourceHash-cne '195B7403B1D92BB7B9A1D18C135BC1B21381E9C6CC46281FCBB00FA415BC6B4C'){throw 'Unreviewed control helper source.'}
    $settings=[Xml.XmlReaderSettings]::new();$settings.DtdProcessing=[Xml.DtdProcessing]::Prohibit;$settings.XmlResolver=$null;$settings.IgnoreWhitespace=$true;$settings.IgnoreComments=$true;$settings.MaxCharactersInDocument=8192
    $input=[IO.StringReader]::new($ProfileText);$reader=[Xml.XmlReader]::Create($input,$settings)
    try{$document=[Xml.XmlDocument]::new();$document.XmlResolver=$null;$document.Load($reader)}finally{$reader.Dispose();$input.Dispose()}
    $scopeHash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($document.DocumentElement.OuterXml)))
    if($scopeHash-cne '607C3DD144805B3FAC0F3C7C2FA5F59DAFF85DF361A2374EE25177769C274FD6'){throw 'Unreviewed control trace scope.'}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: run only an owned compiled helper with a five-second deadline and hidden window; kill only that owned child on timeout. #>
#-------------------------------------------------------------------------------------------------
function Invoke-Ep11OwnedReadHelper([string]$Executable,[string]$Fixture,[string]$Log) {
    $child=Start-Process -FilePath $Executable -ArgumentList ('"'+$Fixture+'"') -WindowStyle Hidden -RedirectStandardOutput $Log -PassThru
    try{
        if(-not $child.WaitForExit(5000)){$child.Kill();$child.WaitForExit(5000)|Out-Null;throw 'Owned read helper timed out.'}
        $child.WaitForExit()
        if($child.ExitCode-ne 0 -or (Get-Item -LiteralPath $Log).Length-gt 128){throw 'Owned read helper failed or output exceeded bound.'}
        $result=(Get-Content -LiteralPath $Log -Raw).Trim()
        if($result-notmatch '^\d+\|READ\|1\|10$' -or [int]($result.Split('|')[0])-ne $child.Id){throw 'Owned helper result/PID differs.'}
        return $result
    }finally{$child.Dispose()}
}
$sourceText=Get-Content -LiteralPath $source -Raw;$profileText=Get-Content -LiteralPath $profile -Raw
Assert-Ep11ControlInputs $sourceText $profileText
if($SelfTest){
    # Reborn: refuse private scope/source faults without invoking a compiler, WPR or helper.
    foreach($fault in @(
        @($sourceText+' ',$profileText),
        @($sourceText,$profileText.Replace('ProcessExeFilter="RebornConfigReadControl.exe"','')),
        @($sourceText,$profileText.Replace('0x1D0','0xFFFFFFFFFFFFFFFF')),
        @($sourceText,'<!DOCTYPE x [<!ENTITY a "value">]><x>&a;</x>')
    )){$rejected=$false;try{Assert-Ep11ControlInputs $fault[0] $fault[1]}catch{$rejected=$true};if(-not $rejected){throw 'Control input fault admitted.'}}
    Write-Output 'EP1 non-game read control: detached PASS; reviewed source/scope and four private refusals. No compiler, helper or trace invoked.'
    return
}
$identity=[Security.Principal.WindowsIdentity]::GetCurrent()
try{$isAdministrator=([Security.Principal.WindowsPrincipal]::new($identity)).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)}finally{$identity.Dispose()}
if(-not $Record -and -not $ValidateHelpers){
    [pscustomobject]@{ControlInputsValidated=$true;WindowsAdministrator=$isAdministrator;RecordRequested=$false;TraceStarted=$false;HelperExecuted=$false;GameExecuted=$false;LiveFilterValidated=$false}
    return
}
if($Record -and -not $isAdministrator){throw 'Live control recording requires an elevated Windows PowerShell 7 session; sandbox approval alone is not administrator elevation. No trace/helper was started.'}
# Reborn: retain all generated binary/ETL outputs locally under the already ignored artifacts tree; no raw trace is staged or published.
$artifacts=Join-Path $repoRoot 'artifacts'
if(Test-Path -LiteralPath $artifacts){if(((Get-Item -LiteralPath $artifacts -Force).Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Reparse artifacts parent refused.'}}
$instance='RebornConfigControl-'+[Guid]::NewGuid().ToString('N')
$output=Join-Path $artifacts $instance
$null=New-Item -ItemType Directory -Path $output
$positive=Join-Path $output 'RebornConfigReadControl.exe';$negative=Join-Path $output 'RebornConfigReadNegative.exe';$trace=Join-Path $output 'control.etl'
foreach($exe in @($positive,$negative)){
    & $compiler /nologo /target:exe /platform:x64 /optimize+ ("/out:"+$exe) $source
    if($LASTEXITCODE-ne 0){throw 'Control compilation failed.'}
}
# Reborn: helper-only validation is useful without administrator rights, but cannot validate ETW filtering or read-event correlation.
if($ValidateHelpers){
    $negativeResult=Invoke-Ep11OwnedReadHelper $negative $fixture (Join-Path $output 'negative.stdout.txt')
    $positiveResult=Invoke-Ep11OwnedReadHelper $positive $fixture (Join-Path $output 'positive.stdout.txt')
    [pscustomobject]@{OutputDirectory=$output;PositiveResult=$positiveResult;NegativeResult=$negativeResult;HelperReadsValidated=$true;TraceStarted=$false;GameExecuted=$false;LiveFilterValidated=$false;SuccessfulReadCorrelationValidated=$false}
    return
}
Assert-Ep11ControlInputs (Get-Content -LiteralPath $source -Raw) (Get-Content -LiteralPath $profile -Raw)
& $wpr -profiles $profile|Out-Null;if($LASTEXITCODE-ne 0){throw 'Control profile refused by WPR.'}
$owned=$false;$timer=[Diagnostics.Stopwatch]::new()
try{
    # Reborn: instancename must be the last argument for every session operation; a failed start never authorizes cancellation.
    & $wpr -start ($profile+'!RebornConfigReadControl.Light') -instancename $instance
    if($LASTEXITCODE-ne 0){throw 'Owned control trace could not start.'}
    $owned=$true;$timer.Start()
    Write-Output "Owned recording instance: $instance"
    $negativeResult=Invoke-Ep11OwnedReadHelper $negative $fixture (Join-Path $output 'negative.stdout.txt')
    $positiveResult=Invoke-Ep11OwnedReadHelper $positive $fixture (Join-Path $output 'positive.stdout.txt')
    & $wpr -stop $trace 'Reborn one-byte config read calibration only' -skipPdbGen -instancename $instance
    if($LASTEXITCODE-ne 0){throw 'Owned control trace could not save.'}
    $owned=$false;$timer.Stop()
}finally{
    if($owned){& $wpr -cancel -instancename $instance;if($LASTEXITCODE-ne 0){Write-Warning "Cleanup failed for owned instance $instance; do not cancel an unnamed session."}}
}
[pscustomobject]@{OutputDirectory=$output;TracePath=$trace;InstanceName=$instance;PositiveResult=$positiveResult;NegativeResult=$negativeResult;RecordingElapsedMs=$timer.ElapsedMilliseconds;TraceBytes=(Get-Item -LiteralPath $trace).Length
    GameExecuted=$false;OtherWprInstanceCancelled=$false;LiveFilterValidated=$false;SuccessfulReadCorrelationValidated=$false}
