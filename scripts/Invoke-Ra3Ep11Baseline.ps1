# Reborn: one explicitly requested normal launcher start; bounded process observation only, without debugger, mod arguments, retries or process termination.
[CmdletBinding()]
param([switch]$Run)
$ErrorActionPreference='Stop'
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
if(-not $Run){Write-Output 'Baseline preflight: PASS; reviewed identities and no existing Uprising process; game not executed.';return}
$artifactRoot=Join-Path (Split-Path $PSScriptRoot -Parent) 'artifacts'
# Reborn: create only a unique repository-local result directory after validating its ancestry.
for($ancestor=Get-Item -LiteralPath $artifactRoot;$null-ne $ancestor;$ancestor=$ancestor.Parent){
    if(($ancestor.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Artifact reparse ancestry refused.'}
}
$output=Join-Path $artifactRoot ('Baseline-'+[Guid]::NewGuid().ToString('N'))
$null=New-Item -ItemType Directory -Path $output
$started=$false;$launcherId=$null;$observations=[Collections.Generic.List[object]]::new();$failure=$null
try{
    # Reborn: repeat the exclusion immediately before the sole start; no parent cwd/PATH changes or mod/runver arguments.
    if(@(Get-Ep11BaselineProcesses).Count-ne 0){throw 'Uprising appeared before launch; refusing start.'}
    $startUtc=[DateTime]::UtcNow
    $info=[Diagnostics.ProcessStartInfo]::new()
    $info.FileName=$launcher;$info.WorkingDirectory=$root;$info.UseShellExecute=$false
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
    $report=[pscustomobject]@{LauncherPath=$launcher;RequestedArguments=@();GameExecuted=$started;LauncherProcessId=$launcherId;Failure=$failure;Observations=$observations.ToArray();ProcessSnapshotAtomic=$false;PidReuseFullyExcluded=$false;IndirectDescendantsObserved=$false;ConfigConsumedProven=$false;ModPackageLoaded=$false;MainMenuReachedProven=$false;GameStoppedByObserver=$false;OutputDirectory=$output}
    [IO.File]::WriteAllText((Join-Path $output 'baseline.json'),(ConvertTo-Json -InputObject $report -Depth 6))
    if($null-ne $owned){$owned.Dispose()}
}
# Reborn: keep full snapshots local while displaying only the bounded outcome, not repeated process command lines.
$report|Select-Object LauncherPath,GameExecuted,LauncherProcessId,Failure,@{Name='ObservationCount';Expression={$_.Observations.Count}},ConfigConsumedProven,ModPackageLoaded,MainMenuReachedProven,GameStoppedByObserver,OutputDirectory|ConvertTo-Json -Depth 3
if($null-ne $failure){throw "Baseline observation failed; do not relaunch automatically: $failure"}
