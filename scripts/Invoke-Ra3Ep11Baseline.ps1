# Reborn: one explicitly requested baseline or inert config launcher start; no debugger, mod package, retries or process termination.
[CmdletBinding()]
param([switch]$Run,[switch]$ConfigProbe,[switch]$SelfTest)
$ErrorActionPreference='Stop'
# Reborn: retain the historical long-name fixture for evidence, but never reuse it for game startup.
. (Join-Path $PSScriptRoot 'Ra3Ep11ConfigNamePolicy.ps1')
$probe=Join-Path (Split-Path $PSScriptRoot -Parent) 'fixtures\ra3ep11\phase-a\probe_1.0.cfg'
#-------------------------------------------------------------------------------------------------
<# Reborn: construct only an empty baseline request or the fixed single-LF config probe request; never accept arbitrary configs or directives. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11BaselineArguments([bool]$UseProbe,[string]$Path,[byte[]]$Bytes) {
    if(-not $UseProbe){return}
    if(-not [IO.Path]::IsPathFullyQualified($Path) -or $Path.Length-gt 240 -or $Path-match '[^\x20-\x7E]|[";]' -or $Bytes.Length-ne 1 -or $Bytes[0]-ne 10){throw 'Only the reviewed absolute ASCII path and single-LF probe are admitted.'}
    Assert-Ep11ConfigNamePolicy $Path
    '-modconfig';$Path
}
#-------------------------------------------------------------------------------------------------
<# Reborn: match the visible quoted probe or whitespace-free unquoted path only, not the native parser result or any config file read. #>
#-------------------------------------------------------------------------------------------------
function Test-Ep11VisibleConfigArgument([string]$CommandLine,[string]$Path) {
    if([string]::IsNullOrEmpty($CommandLine)){return $false}
    $escaped=[regex]::Escape($Path)
    $pathPattern='"'+$escaped+'"'
    if($Path-notmatch '\s'){ $pathPattern='(?:'+$pathPattern+'|'+$escaped+')' }
    return [regex]::IsMatch($CommandLine,'(?:^|\s)-modconfig\s+'+$pathPattern+'(?:\s|$)',[Text.RegularExpressions.RegexOptions]::CultureInvariant)
}
if($SelfTest){
    # Reborn: detached request/visible-argument fixtures never inspect the installation, query processes or launch a target.
    if($Run){throw 'SelfTest and Run cannot be combined.'}
    if(@(Get-Ep11BaselineArguments $false $null $null).Count-ne 0){throw 'Baseline argument fixture differs.'}
    $request=@(Get-Ep11BaselineArguments $true $probe ([byte[]]@(10)))
    if($request.Count-ne 2 -or $request[0]-cne '-modconfig' -or $request[1]-cne $probe){throw 'Probe argument fixture differs.'}
    foreach($fault in @(
        [pscustomobject]@{Path='relative.cfg';Bytes=[byte[]]@(10)},
        [pscustomobject]@{Path=$probe+'"';Bytes=[byte[]]@(10)},
        [pscustomobject]@{Path=$probe+';';Bytes=[byte[]]@(10)},
        [pscustomobject]@{Path=[IO.Path]::Combine([IO.Path]::GetDirectoryName($probe),'config-read-only.cfg');Bytes=[byte[]]@(10)},
        [pscustomobject]@{Path=$probe;Bytes=[byte[]]@(13,10)},
        [pscustomobject]@{Path=$probe;Bytes=[byte[]]@(0)},
        [pscustomobject]@{Path=$probe;Bytes=[byte[]]::new(0)}
    )){$rejected=$false;try{$null=@(Get-Ep11BaselineArguments $true $fault.Path $fault.Bytes)}catch{$rejected=$true};if(-not $rejected){throw 'Invalid probe request admitted.'}}
    if(-not (Test-Ep11VisibleConfigArgument ('game -modconfig "'+$probe+'" -config "sku"') $probe)){throw 'Visible argument positive rejected.'}
    if(-not (Test-Ep11VisibleConfigArgument ('game -modconfig '+$probe+' -config "sku"') $probe)){throw 'Unquoted whitespace-free argument rejected.'}
    $spaced='C:\detached fixture\probe.cfg'
    if(-not (Test-Ep11VisibleConfigArgument ('game -modconfig "'+$spaced+'"') $spaced) -or (Test-Ep11VisibleConfigArgument ('game -modconfig '+$spaced) $spaced)){throw 'Spaced argument boundary differs.'}
    foreach($line in @($null,'game -config "'+$probe+'"','game -modconfig "'+$probe+'.other"','game -modconfig "'+$probe+'"extra','game -modconfig '+$probe+'.other')){
        if(Test-Ep11VisibleConfigArgument $line $probe){throw 'Visible argument negative admitted.'}
    }
    Write-Output 'Baseline/config detached tests: PASS; two request positives, seven request refusals, three visible-argument positives and six negatives; no game or process query.'
    return
}
$requestedArguments=@()
$probeHandle=$null
if($ConfigProbe){
    # Reborn: reject probe reparse ancestry and hold a read-only, read-share handle throughout the experiment to deny ordinary replacement/writes.
    $probeItem=Get-Item -LiteralPath $probe
    for($ancestor=$probeItem;$null-ne $ancestor;$ancestor=if($ancestor.PSIsContainer){$ancestor.Parent}else{$ancestor.Directory}){
        if(($ancestor.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Probe reparse ancestry refused.'}
    }
    $probeHandle=[IO.File]::Open($probe,[IO.FileMode]::Open,[IO.FileAccess]::Read,[IO.FileShare]::Read)
    if($probeHandle.Length-ne 1 -or $probeHandle.ReadByte()-ne 10){$probeHandle.Dispose();throw 'Probe identity differs.'}
    $requestedArguments=@(Get-Ep11BaselineArguments $true $probe ([byte[]]@(10)))
}
$root='D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising'
$launcher=Join-Path $root 'RA3EP1.exe'
$pins=@{
    'RA3EP1.exe'='07694EBBCF21232B1A1B401C07ABC2CFB1EDB9F50FA8D0C36DA0F94820943A4A'
    'Data\ra3ep1_1.1.game'='B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B'
    'Data\ra3ep1_1.0.game'='ABA6A8825A744A78487837D78AD5992962257CE68404F639D600F11DEEA2580B'
    'RA3EP1_english_1.1.SkuDef'='416C0752EA6891F13916D60FB1C8C3B8E5F342F7E919214866FC5D6EE962D75E'
    'Data\MapsCampaign.big'='FF5DFFB04CCC2B080B6F49D11B588A3440A1CA56C7CD492BF745A959DA7FE44C'
}
# Reborn: reject reparse ancestry and changed reviewed files; do not replace or repair installation contents.
foreach($relative in $pins.Keys){
    $path=Join-Path $root $relative
    $item=Get-Item -LiteralPath $path
    for($ancestor=$item;$null-ne $ancestor;$ancestor=if($ancestor.PSIsContainer){$ancestor.Parent}else{$ancestor.Directory}){
        if(($ancestor.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Reparse ancestry refused.'}
    }
    if((Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash-cne $pins[$relative]){throw "Reviewed identity differs: $relative"}
}
foreach($relative in @('Data\maps','Data\mapmetadata.bin','Data\mapmetadata.imp','Data\mapmetadata.manifest','Data\mapmetadata.relo')){
    if(Test-Path -LiteralPath (Join-Path $root $relative)){throw "Known loose override returned: $relative"}
}
#-------------------------------------------------------------------------------------------------
<# Reborn: query only Uprising-named processes; unavailable identity or any existing instance prevents a new start. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11BaselineProcesses {
    @(Get-CimInstance -ClassName Win32_Process -Filter "Name = 'RA3EP1.exe' OR Name = 'ra3ep1_1.0.game' OR Name = 'ra3ep1_1.1.game'")
}
if(@(Get-Ep11BaselineProcesses).Count-ne 0){throw 'Existing Uprising process detected; it will not be touched.'}
if(-not $Run){if($null-ne $probeHandle){$probeHandle.Dispose()};Write-Output 'Baseline/config preflight: PASS; reviewed identities and no existing Uprising process; game not executed.';return}
$artifactRoot=Join-Path (Split-Path $PSScriptRoot -Parent) 'artifacts'
# Reborn: create only a unique repository-local result directory after validating its ancestry.
for($ancestor=Get-Item -LiteralPath $artifactRoot;$null-ne $ancestor;$ancestor=$ancestor.Parent){
    if(($ancestor.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Artifact reparse ancestry refused.'}
}
$output=Join-Path $artifactRoot ('Baseline-'+[Guid]::NewGuid().ToString('N'))
$null=New-Item -ItemType Directory -Path $output
$started=$false;$launcherId=$null;$observations=[Collections.Generic.List[object]]::new();$failure=$null
try{
    # Reborn: repeat the exclusion before the sole start; optional probe is the only modconfig argument, with no runver or parent cwd/PATH changes.
    if(@(Get-Ep11BaselineProcesses).Count-ne 0){throw 'Uprising appeared before launch; refusing start.'}
    $startUtc=[DateTime]::UtcNow
    $info=[Diagnostics.ProcessStartInfo]::new()
    $info.FileName=$launcher;$info.WorkingDirectory=$root;$info.UseShellExecute=$false
    foreach($argument in $requestedArguments){$info.ArgumentList.Add($argument)}
    $owned=[Diagnostics.Process]::Start($info)
    $started=$true;$launcherId=$owned.Id
    $ownedCreationUtc=$owned.StartTime.ToUniversalTime()
    $clock=[Diagnostics.Stopwatch]::StartNew()
    # Reborn: observe direct game-child parent IDs and creation times even if the launcher exits; snapshots are non-atomic and PID reuse is not fully excluded.
    while($clock.Elapsed.TotalSeconds-lt 30){
        foreach($candidate in @(Get-Ep11BaselineProcesses)){
            if($candidate.Name-ieq 'RA3EP1.exe'){continue}
            $expected=@((Join-Path $root 'Data\ra3ep1_1.0.game'),(Join-Path $root 'Data\ra3ep1_1.1.game'))
            if($candidate.ParentProcessId-ne $launcherId -or $candidate.ExecutablePath-inotin $expected -or $null-eq $candidate.CreationDate){continue}
            if($candidate.CreationDate.ToUniversalTime()-lt $ownedCreationUtc -or $candidate.CreationDate.ToUniversalTime()-gt [DateTime]::UtcNow){continue}
            $observations.Add([pscustomobject]@{ObservedAtUtc=[DateTime]::UtcNow.ToString('o');ProcessId=$candidate.ProcessId;ParentProcessId=$candidate.ParentProcessId;CreationUtc=$candidate.CreationDate.ToUniversalTime().ToString('o');ExecutablePath=$candidate.ExecutablePath;CommandLine=$candidate.CommandLine})
        }
        Start-Sleep -Milliseconds 500
    }
}catch{$failure=$_.Exception.Message}finally{
    # Reborn: save evidence even after post-start observation failure; never terminate the game or automatically retry.
    # Reborn: record exact visible argument matches separately from unproved native consumption; no command-line presence can certify a read.
    $matching=0
    if($ConfigProbe){foreach($observation in $observations){if(Test-Ep11VisibleConfigArgument $observation.CommandLine $probe){$matching++}}}
    $report=[pscustomobject]@{LauncherPath=$launcher;RequestedArguments=$requestedArguments;ConfigProbeRequested=[bool]$ConfigProbe;MatchingConfigArgumentSnapshotCount=$matching;GameExecuted=$started;LauncherProcessId=$launcherId;Failure=$failure;Observations=$observations.ToArray();ProcessSnapshotAtomic=$false;PidReuseFullyExcluded=$false;IndirectDescendantsObserved=$false;ConfigConsumedProven=$false;ModPackageLoaded=$false;MainMenuReachedProven=$false;GameStoppedByObserver=$false;OutputDirectory=$output}
    [IO.File]::WriteAllText((Join-Path $output 'baseline.json'),(ConvertTo-Json -InputObject $report -Depth 6))
    if($null-ne $owned){$owned.Dispose()}
    if($null-ne $probeHandle){$probeHandle.Dispose()}
}
# Reborn: keep full snapshots local while displaying only the bounded outcome, not repeated process command lines.
$report|Select-Object LauncherPath,GameExecuted,LauncherProcessId,Failure,ConfigProbeRequested,MatchingConfigArgumentSnapshotCount,@{Name='ObservationCount';Expression={$_.Observations.Count}},ConfigConsumedProven,ModPackageLoaded,MainMenuReachedProven,GameStoppedByObserver,OutputDirectory|ConvertTo-Json -Depth 3
if($null-ne $failure){throw "Baseline observation failed; do not relaunch automatically: $failure"}
