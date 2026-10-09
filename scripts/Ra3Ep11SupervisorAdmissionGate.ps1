# Reborn: shared initial-break decision over trusted in-process identity and read adapters; this module cannot start or resume a process.
. (Join-Path $PSScriptRoot 'Ra3Ep11DebuggerAdmissionPolicy.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep11NativeAdmissionPolicy.ps1')

#-------------------------------------------------------------------------------------------------
<# Reborn: require matching retained-handle and exact-PID snapshot identity before invoking any memory-read adapter. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11SupervisorAdmissionIdentity($Plan,$Context,[string]$InitialText) {
    $owner=Get-Ep11InitialAdmissionOwner $InitialText
    if($Context.ProcessId -le 0 -or $owner -ne $Context.ProcessId -or $Context.DebuggerProcessId -le 0 -or
       $Context.ProcessId -eq $Context.DebuggerProcessId -or $Context.SnapshotCount -ne 1 -or
       $Context.SnapshotProcessId -ne $Context.ProcessId -or $Context.ParentProcessId -ne $Context.DebuggerProcessId){throw 'Supervisor target/parent identity differs.'}
    foreach($flag in @('TargetHasExited','DebuggerHasExited')){
        if($Context.$flag -isnot [bool] -or $Context.$flag){throw 'Supervisor process not live or unavailable.'}
    }
    if(-not [IO.Path]::IsPathFullyQualified($Context.ExpectedExecutablePath) -or
       $Context.ActualExecutablePath -ine $Context.ExpectedExecutablePath -or $Context.SnapshotExecutablePath -ine $Context.ExpectedExecutablePath){throw 'Supervisor executable path differs.'}
    if($Context.CreationUtcTicks -le 0 -or $Context.DebuggerCreationUtcTicks -le 0 -or
       $Context.CreationUtcTicks -lt $Context.DebuggerCreationUtcTicks -or $Context.ObservedUtcTicks -lt $Context.CreationUtcTicks -or
       $Context.SnapshotCreationUtcTicks -le 0 -or [Math]::Abs([decimal]$Context.SnapshotCreationUtcTicks-$Context.CreationUtcTicks) -gt 10000){throw 'Supervisor creation identity/window differs.'}
    if($Plan.ImageSha256 -cnotmatch '^[A-F0-9]{64}$' -or $Context.DiskImageSha256 -cne $Plan.ImageSha256 -or
       $Context.ModuleBase -ne $Plan.PreferredBase -or $Context.ModuleSize -ne $Plan.SizeOfImage){throw 'Supervisor image identity/base/size differs.'}
    if($Plan.Kind -ceq 'NativeEp11'){
        $expected='D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game'
        if($Plan.ImageSha256 -cne 'B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B' -or
           $Context.ExpectedExecutablePath -ine $expected -or $Plan.PreferredBase -ne 0x400000 -or $Plan.SizeOfImage -ne 13754368){throw 'Native supervisor profile differs.'}
    }elseif($Plan.Kind -ceq 'OwnedClrHelper'){
        # Reborn: helper admission is limited to the small fixed helper, not a profile escape for the native game.
        if([IO.Path]::GetFileName($Context.ExpectedExecutablePath) -cne 'RebornDebuggerLifetimeControl.exe' -or $Plan.SizeOfImage -gt 65536){throw 'Owned helper supervisor profile differs.'}
    }else{throw 'Unknown supervisor admission profile.'}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: validate all bounded reads before returning a continuation decision; supplied evidence is not itself an OS ownership proof. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11SupervisorAdmissionDecision($Plan,$Context,[string]$InitialText,[scriptblock]$ReadRange) {
    Assert-Ep11SupervisorAdmissionIdentity $Plan $Context $InitialText
    if($null -eq $ReadRange -or $Plan.Ranges.Count -lt 1 -or $Plan.Ranges.Count -gt 32){throw 'Supervisor range count/adapter differs.'}
    $budget=0;$seen=@{}
    foreach($range in $Plan.Ranges){
        if([string]::IsNullOrEmpty($range.Name) -or $seen.ContainsKey($range.Name) -or $range.Length -lt 1 -or $range.Length -gt 4096 -or
           $range.Rva -le 0 -or [long]$range.Rva+$range.Length -gt $Plan.SizeOfImage -or
           $range.Address -ne [long]$Plan.PreferredBase+$range.Rva -or $range.ExpectedSha256 -cnotmatch '^[A-F0-9]{64}$'){throw 'Supervisor range shape differs.'}
        $seen[$range.Name]=$true;$budget+=$range.Length
        if($budget -gt 8192){throw 'Supervisor total read budget exceeded.'}
    }
    $observations=[Collections.Generic.List[object]]::new()
    foreach($range in $Plan.Ranges){
        $read=@(& $ReadRange $range)
        if($read.Count -ne 1){throw 'Memory adapter returned missing or ambiguous evidence.'}
        $record=$read[0]
        if($record.Name -cne $range.Name -or $record.Address -ne $range.Address -or $record.Succeeded -isnot [bool] -or
           -not $record.Succeeded -or $record.ActualCount -ne $range.Length -or $record.Bytes -isnot [byte[]] -or $record.Bytes.Length -ne $range.Length){throw 'Supervisor read incomplete or misbound.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($record.Bytes)) -cne $range.ExpectedSha256){throw 'Supervisor code/table bytes differ.'}
        $observations.Add($record)
    }
    if($Plan.Kind -ceq 'NativeEp11'){Assert-Ep11NativeAdmissionObservations $Plan $observations.ToArray() $Context.ModuleBase}
    [pscustomobject]@{CanIssueObservationContinue=$true;SuppliedIdentityValidated=$true;SuppliedReadEvidenceValidated=$true;ValidatedRangeCount=$observations.Count;ValidatedBytes=$budget;Profile=$Plan.Kind;DecisionOnly=$true;ContinuationCommandSent=$false}
}
