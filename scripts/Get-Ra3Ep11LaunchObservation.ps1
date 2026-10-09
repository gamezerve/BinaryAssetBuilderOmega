# Reborn: read a supplied running launcher PID and its immediate same-installation game children only; never start, stop or attach a debugger to a process.
[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$LauncherPath,[uint32]$LauncherProcessId,[switch]$SelfTest,[switch]$AsJson)
$ErrorActionPreference='Stop'

#-------------------------------------------------------------------------------------------------
<# Reborn: admit only immediate 1.0/1.1 game children of the supplied launcher in the exact installation, without parsing or endorsing their arguments. #>
#-------------------------------------------------------------------------------------------------
function Select-Ep11ObservedChild($Process,[uint32]$ParentId,[string]$Root) {
    if($null-eq $Process.ExecutablePath -or $Process.ParentProcessId-ne $ParentId){return $null}
    $expected=@([IO.Path]::Combine($Root,'Data','ra3ep1_1.0.game'),[IO.Path]::Combine($Root,'Data','ra3ep1_1.1.game'))
    if($Process.ExecutablePath-inotin $expected){return $null}
    return [pscustomobject]@{ProcessId=$Process.ProcessId;ParentProcessId=$Process.ParentProcessId;ExecutablePath=$Process.ExecutablePath;CommandLine=$Process.CommandLine;CreationDate=$Process.CreationDate}
}
if(-not [IO.Path]::IsPathFullyQualified($LauncherPath) -or [IO.Path]::GetFileName($LauncherPath)-ine 'RA3EP1.exe'){throw 'Observation requires an absolute RA3EP1.exe path.'}
$root=[IO.Path]::GetDirectoryName([IO.Path]::GetFullPath($LauncherPath))
if($SelfTest){
    # Reborn: detached observations never query WMI/CIM or execute any target; wrong parent, path and unavailable path must be refused.
    foreach($version in @('1.0','1.1')){
        $fixture=[pscustomobject]@{ProcessId=11;ParentProcessId=10;ExecutablePath=[IO.Path]::Combine($root,'Data',"ra3ep1_$version.game");CommandLine='detached fixture';CreationDate='fixture'}
        if($null-eq (Select-Ep11ObservedChild $fixture 10 $root)){throw 'Detached child observation rejected.'}
    }
    foreach($fixture in @(
        [pscustomobject]@{ParentProcessId=9;ExecutablePath=[IO.Path]::Combine($root,'Data','ra3ep1_1.1.game')},
        [pscustomobject]@{ParentProcessId=10;ExecutablePath=[IO.Path]::Combine($root,'Elsewhere','ra3ep1_1.1.game')},
        [pscustomobject]@{ParentProcessId=10;ExecutablePath=$null}
    )){if($null-ne (Select-Ep11ObservedChild $fixture 10 $root)){throw 'Out-of-scope observation admitted.'}}
    Write-Output 'EP1 launch observation detached tests: PASS; two admitted versions and three scope refusals; no process query or game execution.'
    return
}
if($LauncherProcessId-eq 0){throw 'Supply the PID of the already-running selected launcher; this command does not launch it.'}
$parent=@(Get-CimInstance -ClassName Win32_Process -Filter "ProcessId = $LauncherProcessId")
if($parent.Count-ne 1 -or $null-eq $parent[0].ExecutablePath -or -not $parent[0].ExecutablePath.Equals([IO.Path]::GetFullPath($LauncherPath),[StringComparison]::OrdinalIgnoreCase)){throw 'Running launcher identity/path unavailable or mismatched.'}
$children=[Collections.Generic.List[object]]::new()
foreach($candidate in @(Get-CimInstance -ClassName Win32_Process -Filter "ParentProcessId = $LauncherProcessId")){
    $selected=Select-Ep11ObservedChild $candidate $LauncherProcessId $root
    if($null-eq $selected){continue}
    # Reborn: reread only the selected child PID to detect exit, reuse or changed visible identity; this is not an atomic process-tree snapshot.
    $again=@(Get-CimInstance -ClassName Win32_Process -Filter "ProcessId = $($selected.ProcessId)")
    if($again.Count-ne 1){throw 'Selected child exited during observation.'}
    $repeat=Select-Ep11ObservedChild $again[0] $LauncherProcessId $root
    if($null-eq $repeat -or $repeat.CreationDate-ne $selected.CreationDate -or $repeat.CommandLine-cne $selected.CommandLine){throw 'Selected child observation changed.'}
    if($null-eq $selected.CommandLine){throw 'Selected child command line unavailable; do not infer forwarding.'}
    $children.Add($selected)
}
$parentAfter=@(Get-CimInstance -ClassName Win32_Process -Filter "ProcessId = $LauncherProcessId")
if($parentAfter.Count-ne 1 -or $parentAfter[0].CreationDate-ne $parent[0].CreationDate -or $parentAfter[0].ExecutablePath-ine $parent[0].ExecutablePath){throw 'Launcher exited or PID identity changed.'}
$report=[pscustomobject]@{ObservedAtUtc=[DateTime]::UtcNow.ToString('o');LauncherProcessId=$LauncherProcessId;LauncherPath=[IO.Path]::GetFullPath($LauncherPath);Children=$children.ToArray();ChildCount=$children.Count
    ReadOnly=$true;TargetStartedByObserver=$false;ProcessSnapshotAtomic=$false;IndirectDescendantsObserved=$false;ChildArgumentsParsed=$false;ConfigConsumptionObserved=$false;ModPackageLoaded=$false}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 6}else{$report}
