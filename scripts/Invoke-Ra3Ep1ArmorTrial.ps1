# Reborn: one explicitly authorized armor candidate launch; no debugger, installation writes, retries or process termination.
[CmdletBinding()]
param([switch]$Run,[switch]$DefaultReader)
$ErrorActionPreference='Stop'
if($PSVersionTable.PSVersion.Major -lt 7){throw 'PowerShell 7 is required.'}
$repo=Split-Path -Parent $PSScriptRoot
# Reborn: select only one of two separately admitted fixed candidates, never a caller-supplied config or digest.
$trial=Join-Path $repo $(if($DefaultReader){'artifacts/RebornArmorDefault-abb50afbef8745af952b8cc879c43aa9'}else{'artifacts/RebornArmorTrial-0f2a34bd39dd4eef9946d7e3b818f57f'})
$expectedBytes=if($DefaultReader){3552}else{3650}
$expectedHash=if($DefaultReader){'DD7DE09E3A81B9714DC2861ADE0AA9ECDFE72A1851EDD1ED20BE2155EF31C2DC'}else{'3FE964292FA72A1DF961EC93D3CFDA884E1334F55ACB294FF75FE1E4144F3F85'}
$root='D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising'
$config=Join-Path $trial 'reborn_1.0.cfg'
$archive=Join-Path $trial 'RebornArmorProbe.big'
$handles=[Collections.Generic.List[IDisposable]]::new()
#-------------------------------------------------------------------------------------------------
<# Reborn: refuse redirected candidate paths before retaining read-share-only handles for the entire test. #>
#-------------------------------------------------------------------------------------------------
function Open-RebornFrozenFile([string]$Path) {
    for($item=Get-Item -LiteralPath $Path;$null-ne $item;$item=if($item.PSIsContainer){$item.Parent}else{$item.Directory}){
        if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected candidate refused.'}
    }
    $stream=[IO.File]::Open($Path,[IO.FileMode]::Open,[IO.FileAccess]::Read,[IO.FileShare]::Read)
    $handles.Add($stream)
    return $stream
}
#-------------------------------------------------------------------------------------------------
<# Reborn: query only named Uprising processes; do not kill or alter existing instances. #>
#-------------------------------------------------------------------------------------------------
function Get-RebornTrialProcesses {
    @(Get-CimInstance Win32_Process -Filter "Name = 'RA3EP1.exe' OR Name = 'ra3ep1_1.0.game' OR Name = 'ra3ep1_1.1.game'")
}
try {
    # Reborn: reuse the strict stock identity, campaign digest and process exclusion preflight without relaxing its inert-config gate.
    & (Join-Path $PSScriptRoot 'Invoke-Ra3Ep11Baseline.ps1')
    . (Join-Path $PSScriptRoot 'Ra3Ep11ConfigNamePolicy.ps1')
    Assert-Ep11ConfigNamePolicy $config
    $cfgHandle=Open-RebornFrozenFile $config
    $bigHandle=Open-RebornFrozenFile $archive
    $expected=[Text.Encoding]::ASCII.GetBytes($(if($DefaultReader){"add-big RebornArmorProbe.big`n"}else{"add-big RebornArmorProbe.big`nadd-manifest data\reborn_armor_probe.manifest`n"}))
    if($cfgHandle.Length-ne $expected.Length){throw 'Candidate config length differs.'}
    $actual=[byte[]]::new($expected.Length);$cfgHandle.ReadExactly($actual)
    if([Convert]::ToHexString($actual)-cne [Convert]::ToHexString($expected)){throw 'Candidate config bytes differ.'}
    if($bigHandle.Length-ne $expectedBytes -or [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bigHandle))-cne $expectedHash){throw 'Candidate archive digest differs.'}
    $sku=Join-Path $root 'RA3EP1_english_1.1.SkuDef'
    foreach($line in Get-Content -LiteralPath $sku){if($line.StartsWith('add-big ')){
        $file=Join-Path $root $line.Substring(8)
        if(-not (Test-Path -LiteralPath $file -PathType Leaf)){throw 'Stock archive missing.'}
    }}
    if(@(Get-RebornTrialProcesses).Count-ne 0){throw 'Existing Uprising process detected; no start.'}
    if(-not $Run){'Armor trial preflight: PASS; game not started.';return}
    $output=Join-Path $repo ('artifacts/ArmorLaunch-'+[guid]::NewGuid().ToString('N'))
    $null=New-Item -ItemType Directory -Path $output
    $observations=[Collections.Generic.List[object]]::new()
    $started=$false;$owned=$null;$failure=$null
    try {
        if(@(Get-RebornTrialProcesses).Count-ne 0){throw 'Uprising appeared before launch; no start.'}
        # Reborn: sole start uses argument-list quoting, stock cwd and the admitted fixed config; never retry.
        $info=[Diagnostics.ProcessStartInfo]::new()
        $info.FileName=Join-Path $root 'RA3EP1.exe';$info.WorkingDirectory=$root;$info.UseShellExecute=$false
        $info.ArgumentList.Add('-modconfig');$info.ArgumentList.Add($config)
        $owned=[Diagnostics.Process]::Start($info);$started=$true
        $ownedId=$owned.Id;$creation=$owned.StartTime.ToUniversalTime()
        $clock=[Diagnostics.Stopwatch]::StartNew()
        while($clock.Elapsed.TotalSeconds-lt 30){
            foreach($process in @(Get-RebornTrialProcesses)){
                if($process.ParentProcessId-ne $ownedId -or $process.ExecutablePath-ine (Join-Path $root 'Data\ra3ep1_1.1.game') -or $null-eq $process.CreationDate){continue}
                if($process.CreationDate.ToUniversalTime()-lt $creation -or $process.CreationDate.ToUniversalTime()-gt [DateTime]::UtcNow){continue}
                $pattern='(?:^|\s)-modconfig\s+(?:"'+[regex]::Escape($config)+'"|'+[regex]::Escape($config)+')(?:\s|$)'
                $observations.Add([pscustomobject]@{ObservedUtc=[DateTime]::UtcNow.ToString('o');ProcessId=$process.ProcessId;ParentProcessId=$process.ParentProcessId;CreationUtc=$process.CreationDate.ToUniversalTime().ToString('o');ExecutablePath=$process.ExecutablePath;CommandLine=$process.CommandLine;VisibleConfigArgumentMatches=($null-ne $process.CommandLine -and [regex]::IsMatch($process.CommandLine,$pattern))})
            }
            Start-Sleep -Milliseconds 500
        }
    }catch{$failure=$_.Exception.Message}finally{
        # Reborn: retain observations even on failure; command-line evidence is not a config read or loaded override.
        $report=[pscustomobject]@{GameExecuted=$started;ConfigPath=$config;ArchivePath=$archive;WorkingDirectory=$root;LauncherProcessId=$ownedId;Failure=$failure;Observations=$observations.ToArray();ConfigConsumptionProven=$false;ManifestLoadProven=$false;OverrideAppliedInGame=$false;MainMenuReachedProven=$false;InstalledFilesModified=$false;GameStoppedByObserver=$false;ProcessSnapshotAtomic=$false;PidReuseFullyExcluded=$false;OutputDirectory=$output}
        [IO.File]::WriteAllText((Join-Path $output 'launch.json'),($report|ConvertTo-Json -Depth 6))
        if($null-ne $owned){$owned.Dispose()}
    }
    $report|Select-Object GameExecuted,LauncherProcessId,Failure,@{Name='ObservationCount';Expression={$_.Observations.Count}},@{Name='MatchingArgumentCount';Expression={@($_.Observations|Where-Object VisibleConfigArgumentMatches).Count}},ConfigConsumptionProven,OverrideAppliedInGame,OutputDirectory|ConvertTo-Json
    if($null-ne $failure){throw "Trial observation failed; do not retry: $failure"}
}finally{
    # Reborn: release read-only locks without terminating any process.
    foreach($handle in $handles){$handle.Dispose()}
}
