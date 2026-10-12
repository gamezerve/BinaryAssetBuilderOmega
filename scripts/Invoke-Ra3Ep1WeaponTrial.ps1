# Reborn: one separately authorized fixed AK47 Damage=50 launch; no debugger, stock writes, retries or process termination.
[CmdletBinding()]
param([switch]$Run)
$ErrorActionPreference='Stop'
if($PSVersionTable.PSVersion.Major -lt 7){throw 'PowerShell 7 is required.'}
$repo=Split-Path -Parent $PSScriptRoot
$trial=Join-Path $repo 'artifacts/RebornWpnPkg-e0111e2d26b0465da770e53bdd2671c2/weapon-2cfd7a4a'
$root='D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising'
$config=Join-Path $trial 'weaponprobe_1.0.cfg'
$archive=Join-Path $trial 'RebornWeaponProbe.big'
$expectedHash='932415FE98AC7C527604A01923AC9D7910E6C6116C6F59A42C51912C0DCEBC5E'
$handles=[Collections.Generic.List[IDisposable]]::new()

#-------------------------------------------------------------------------------------------------
<# Reborn: freeze only the exact candidate files with read-share handles after rejecting redirected ancestry. #>
#-------------------------------------------------------------------------------------------------
function Open-RebornWeaponFile([string]$Path) {
    if(-not [IO.Path]::IsPathFullyQualified($Path)){throw 'Weapon trial requires absolute paths.'}
    for($item=Get-Item -LiteralPath $Path;$null-ne $item;$item=if($item.PSIsContainer){$item.Parent}else{$item.Directory}){
        if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected weapon trial path refused.'}
    }
    $stream=[IO.File]::Open($Path,[IO.FileMode]::Open,[IO.FileAccess]::Read,[IO.FileShare]::Read)
    $handles.Add($stream)
    return $stream
}

#-------------------------------------------------------------------------------------------------
<# Reborn: query only named Uprising processes; any existing instance blocks the single start and is never modified. #>
#-------------------------------------------------------------------------------------------------
function Get-RebornWeaponProcesses {
    @(Get-CimInstance Win32_Process -Filter "Name = 'RA3EP1.exe' OR Name = 'ra3ep1_1.0.game' OR Name = 'ra3ep1_1.1.game'")
}

try {
    # Reborn: retain the reviewed launcher/game/SKU/campaign identity and loose-override checks without relaxing the baseline's inert-config gate.
    & (Join-Path $PSScriptRoot 'Invoke-Ra3Ep11Baseline.ps1')
    . (Join-Path $PSScriptRoot 'Ra3Ep11ConfigNamePolicy.ps1')
    Assert-Ep11ConfigNamePolicy $config
    if($config.Length-gt 240 -or $config-match '[^\x20-\x7E]|[";]'){throw 'Weapon config path exceeds conservative policy.'}
    $fragment=[Text.Encoding]::ASCII.GetBytes("add-big RebornWeaponProbe.big`n")
    foreach($path in @($config,(Join-Path $trial 'weaponprobe_1.0.skudef'))){
        $stream=Open-RebornWeaponFile $path
        if($stream.Length-ne $fragment.Length){throw 'Weapon fragment length differs.'}
        $bytes=[byte[]]::new($fragment.Length);$stream.ReadExactly($bytes)
        if([Convert]::ToHexString($bytes)-cne [Convert]::ToHexString($fragment)){throw 'Weapon fragment bytes differ.'}
    }
    $big=Open-RebornWeaponFile $archive
    if($big.Length-ne 8730 -or [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($big))-cne $expectedHash){throw 'Pinned weapon archive differs.'}
    $sku=Join-Path $root 'RA3EP1_english_1.1.SkuDef'
    foreach($line in Get-Content -LiteralPath $sku){if($line.StartsWith('add-big ')){
        if(-not (Test-Path -LiteralPath (Join-Path $root $line.Substring(8)) -PathType Leaf)){throw 'Stock archive missing.'}
    }}
    if(@(Get-RebornWeaponProcesses).Count-ne 0){throw 'Existing Uprising process detected; no start.'}
    if(-not $Run){'Weapon trial preflight: PASS; pinned AK47 Damage=50 candidate; game not started.';return}
    # Reborn: validate output ancestry and retain a unique launch report; never overwrite a prior trial report.
    $artifactRoot=Join-Path $repo 'artifacts'
    for($item=Get-Item -LiteralPath $artifactRoot;$null-ne $item;$item=$item.Parent){
        if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected artifact root refused.'}
    }
    $output=Join-Path $artifactRoot ('WeaponLaunch-'+[guid]::NewGuid().ToString('N'))
    $null=New-Item -ItemType Directory -Path $output
    $observations=[Collections.Generic.List[object]]::new()
    $started=$false;$owned=$null;$launcherId=$null;$failure=$null
    try {
        if(@(Get-RebornWeaponProcesses).Count-ne 0){throw 'Uprising appeared before launch; no start.'}
        # Reborn: sole launch preserves stock cwd and uses separately quoted fixed config arguments; no automatic retry exists.
        $info=[Diagnostics.ProcessStartInfo]::new()
        $info.FileName=Join-Path $root 'RA3EP1.exe';$info.WorkingDirectory=$root;$info.UseShellExecute=$false
        $info.ArgumentList.Add('-modconfig');$info.ArgumentList.Add($config)
        $owned=[Diagnostics.Process]::Start($info);$started=$true;$launcherId=$owned.Id
        $creation=$owned.StartTime.ToUniversalTime();$clock=[Diagnostics.Stopwatch]::StartNew()
        while($clock.Elapsed.TotalSeconds-lt 30){
            foreach($process in @(Get-RebornWeaponProcesses)){
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
        # Reborn: command-line observations never certify config reads or weapon override; retain failure evidence without killing the game.
        $report=[pscustomobject]@{Asset='WeaponTemplate:SovietAntiInfantryInfantryAK47';Damage=50;ArchiveSha256=$expectedHash
            GameExecuted=$started;LauncherProcessId=$launcherId;Failure=$failure;ConfigPath=$config;ArchivePath=$archive;WorkingDirectory=$root
            Observations=$observations.ToArray();ConfigConsumptionProven=$false;ManifestLoadProven=$false;OverrideAppliedInGame=$false;MainMenuReachedProven=$false
            InstalledFilesModified=$false;GameStoppedByObserver=$false;ProcessSnapshotAtomic=$false;PidReuseFullyExcluded=$false;OutputDirectory=$output}
        [IO.File]::WriteAllText((Join-Path $output 'launch.json'),($report|ConvertTo-Json -Depth 6))
        if($null-ne $owned){$owned.Dispose()}
    }
    $report|Select-Object GameExecuted,LauncherProcessId,Failure,@{Name='ObservationCount';Expression={$_.Observations.Count}},
        @{Name='MatchingArgumentCount';Expression={@($_.Observations|Where-Object VisibleConfigArgumentMatches).Count}},
        ConfigConsumptionProven,OverrideAppliedInGame,OutputDirectory|ConvertTo-Json
    if($null-ne $failure){throw "Weapon observation failed; do not retry: $failure"}
}finally{
    # Reborn: release only owned read-share locks; never terminate or manipulate an existing or newly launched game.
    foreach($handle in $handles){$handle.Dispose()}
}
