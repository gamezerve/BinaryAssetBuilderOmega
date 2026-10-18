# Reborn: one separately authorized fixed Conscript BuildTime=1 launch; no debugger, stock writes, retries or process termination.
[CmdletBinding()]
param([switch]$Run,[switch]$SkuDef)
$ErrorActionPreference='Stop'
if($PSVersionTable.PSVersion.Major -lt 7){throw 'PowerShell 7 is required.'}
$repo=Split-Path -Parent $PSScriptRoot
# Reborn: the new experiment mounts an isolated SKUDef-only export; the previous CFG candidate is preserved unchanged.
$trial=Join-Path $repo $(if($SkuDef){'artifacts/Conscript-Sku-20261018-a'}else{'artifacts/Conscript-20261018-a'})
$root='D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising'
$config=Join-Path $trial $(if($SkuDef){'reborn_1.0.skudef'}else{'reborn_1.0.cfg'})
$archive=Join-Path $trial 'RebornConscriptProbe.big'
$expectedHash='3FCDFFB9909B39E9C3A6B75AC50791A19D3EFBAE5A83D6169C174197C50C8FB2'
$handles=[Collections.Generic.List[IDisposable]]::new()

#-------------------------------------------------------------------------------------------------
<# Reborn: freeze only the exact candidate files with read-share handles after rejecting redirected ancestry. #>
#-------------------------------------------------------------------------------------------------
function Open-RebornConscriptFile([string]$Path) {
    if(-not [IO.Path]::IsPathFullyQualified($Path)){throw 'Conscript trial requires absolute paths.'}
    for($item=Get-Item -LiteralPath $Path;$null-ne $item;$item=if($item.PSIsContainer){$item.Parent}else{$item.Directory}){
        if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected Conscript trial path refused.'}
    }
    $stream=[IO.File]::Open($Path,[IO.FileMode]::Open,[IO.FileAccess]::Read,[IO.FileShare]::Read)
    $handles.Add($stream)
    return $stream
}

#-------------------------------------------------------------------------------------------------
<# Reborn: query only named Uprising processes; any existing instance blocks the single start and is never modified. #>
#-------------------------------------------------------------------------------------------------
function Get-RebornConscriptProcesses {
    @(Get-CimInstance Win32_Process -Filter "Name = 'RA3EP1.exe' OR Name = 'ra3ep1_1.0.game' OR Name = 'ra3ep1_1.1.game'")
}

try {
    # Reborn: revalidate the pinned short export without creating files or accepting arbitrary package identities.
    $exportCheck=& (Join-Path $PSScriptRoot 'New-Ra3Ep1ConscriptTrial.ps1') -OutputDirectory $trial -CheckOnly -SkuDefOnly:$SkuDef
    if(-not $exportCheck.ExportValidated){throw 'Conscript short export check failed.'}
    # Reborn: retain the reviewed launcher/game/SKU/campaign identity and loose-override checks without relaxing the baseline's inert-config gate.
    & (Join-Path $PSScriptRoot 'Invoke-Ra3Ep11Baseline.ps1')
    . (Join-Path $PSScriptRoot 'Ra3Ep11ConfigNamePolicy.ps1')
    Assert-Ep11ConfigNamePolicy $config
    if($config.Length-gt 240 -or $config-match '[^\x20-\x7E]|[";]'){throw 'Conscript config path exceeds conservative policy.'}
    $fragment=[Text.Encoding]::ASCII.GetBytes("add-big RebornConscriptProbe.big`n")
    foreach($path in @($config,(Join-Path $trial 'reborn_1.0.skudef'))|Select-Object -Unique){
        $stream=Open-RebornConscriptFile $path
        if($stream.Length-ne $fragment.Length){throw 'Conscript fragment length differs.'}
        $bytes=[byte[]]::new($fragment.Length);$stream.ReadExactly($bytes)
        if([Convert]::ToHexString($bytes)-cne [Convert]::ToHexString($fragment)){throw 'Conscript fragment bytes differ.'}
    }
    $big=Open-RebornConscriptFile $archive
    if($big.Length-lt 1 -or $big.Length-gt 1048576 -or [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($big))-cne $expectedHash){throw 'Pinned Conscript archive differs.'}
    $sku=Join-Path $root 'RA3EP1_english_1.1.SkuDef'
    foreach($line in Get-Content -LiteralPath $sku){if($line.StartsWith('add-big ')){
        if(-not (Test-Path -LiteralPath (Join-Path $root $line.Substring(8)) -PathType Leaf)){throw 'Stock archive missing.'}
    }}
    if(@(Get-RebornConscriptProcesses).Count-ne 0){throw 'Existing Uprising process detected; no start.'}
    if(-not $Run){'Conscript trial preflight: PASS; pinned Conscript BuildTime=1 candidate; game not started.';return}
    # Reborn: validate output ancestry and retain a unique launch report; never overwrite a prior trial report.
    $artifactRoot=Join-Path $repo 'artifacts'
    for($item=Get-Item -LiteralPath $artifactRoot;$null-ne $item;$item=$item.Parent){
        if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected artifact root refused.'}
    }
    $output=Join-Path $artifactRoot ('ConscriptLaunch-'+[guid]::NewGuid().ToString('N'))
    $null=New-Item -ItemType Directory -Path $output
    $observations=[Collections.Generic.List[object]]::new()
    $started=$false;$owned=$null;$launcherId=$null;$failure=$null
    try {
        if(@(Get-RebornConscriptProcesses).Count-ne 0){throw 'Uprising appeared before launch; no start.'}
        # Reborn: sole launch preserves stock cwd and uses separately quoted fixed config arguments; no automatic retry exists.
        $info=[Diagnostics.ProcessStartInfo]::new()
        $info.FileName=Join-Path $root 'RA3EP1.exe';$info.WorkingDirectory=$root;$info.UseShellExecute=$false
        $info.ArgumentList.Add('-modconfig');$info.ArgumentList.Add($config)
        $owned=[Diagnostics.Process]::Start($info);$started=$true;$launcherId=$owned.Id
        $creation=$owned.StartTime.ToUniversalTime();$clock=[Diagnostics.Stopwatch]::StartNew()
        while($clock.Elapsed.TotalSeconds-lt 30){
            foreach($process in @(Get-RebornConscriptProcesses)){
                if($process.ParentProcessId-ne $launcherId -or $process.ExecutablePath-ine (Join-Path $root 'Data\ra3ep1_1.1.game') -or $null-eq $process.CreationDate){continue}
                if($process.CreationDate.ToUniversalTime()-lt $creation -or $process.CreationDate.ToUniversalTime()-gt [DateTime]::UtcNow){continue}
                $pattern='(?:^|\s)-modconfig\s+(?:"'+[regex]::Escape($config)+'"|'+[regex]::Escape($config)+')(?:\s|$)'
                $observations.Add([pscustomobject]@{ObservedUtc=[DateTime]::UtcNow.ToString('o');ProcessId=$process.ProcessId;ParentProcessId=$process.ParentProcessId
                    ExecutablePath=$process.ExecutablePath;CreationUtc=$process.CreationDate.ToUniversalTime().ToString('o');CommandLine=$process.CommandLine
                    VisibleConfigArgumentMatches=($null-ne $process.CommandLine -and [regex]::IsMatch($process.CommandLine,$pattern))})
            }
            Start-Sleep -Milliseconds 500
        }
    }catch{$failure=$_.Exception.Message}finally{
        # Reborn: command-line observations never certify config reads or Conscript override; retain failure evidence without killing the game.
        $report=[pscustomobject]@{Asset='GameObject:SovietAntiInfantryInfantry';BuildTime=1;ArchiveSha256=$expectedHash;SkuDefExperiment=[bool]$SkuDef;CfgCounterpartPresent=(Test-Path -LiteralPath (Join-Path $trial 'reborn_1.0.cfg'))
            GameExecuted=$started;LauncherProcessId=$launcherId;Failure=$failure;ConfigPath=$config;ArchivePath=$archive;WorkingDirectory=$root
            Observations=$observations.ToArray();ConfigConsumptionProven=$false;ManifestLoadProven=$false;OverrideAppliedInGame=$false;MainMenuReachedProven=$false
            InstalledFilesModified=$false;GameStoppedByObserver=$false;ProcessSnapshotAtomic=$false;PidReuseFullyExcluded=$false;OutputDirectory=$output}
        [IO.File]::WriteAllText((Join-Path $output 'launch.json'),($report|ConvertTo-Json -Depth 6))
        if($null-ne $owned){$owned.Dispose()}
    }
    $report|Select-Object GameExecuted,LauncherProcessId,Failure,@{Name='ObservationCount';Expression={$_.Observations.Count}},
        @{Name='MatchingArgumentCount';Expression={@($_.Observations|Where-Object VisibleConfigArgumentMatches).Count}},
        ConfigConsumptionProven,OverrideAppliedInGame,OutputDirectory|ConvertTo-Json
    if($null-ne $failure){throw "Conscript observation failed; do not retry: $failure"}
}finally{
    # Reborn: release only owned read-share locks; never terminate or manipulate an existing or newly launched game.
    foreach($handle in $handles){$handle.Dispose()}
}
