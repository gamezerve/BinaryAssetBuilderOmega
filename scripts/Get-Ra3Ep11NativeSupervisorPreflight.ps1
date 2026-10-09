# Reborn: connect the native factory plan to initial-break decision tests without any game/debugger/process execution path.
[CmdletBinding()]
param([switch]$SelfTest,[switch]$AsJson,[switch]$Run)
$ErrorActionPreference='Stop'
if($Run){throw 'Native supervisor execution is disabled; this preflight cannot launch or attach to a game.'}
$supervisorSelfTest=$SelfTest;$supervisorAsJson=$AsJson
$image='D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game'
$plan=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11NativeAdmissionPlan.ps1') -ImagePath $image
. (Join-Path $PSScriptRoot 'Ra3Ep11SupervisorAdmissionGate.ps1')
$SelfTest=$supervisorSelfTest;$AsJson=$supervisorAsJson
$result=[pscustomobject]@{ImageSha256=$plan.ImageSha256;NativePlanConnected=$true;AdmissionRanges=$plan.Ranges.Count;AdmissionBytes=($plan.Ranges|Measure-Object Length -Sum).Sum;DetachedDecisionValidated=$false;IdentityRefusals=0;ReadRefusals=0;RangeRefusals=0;NativeSupervisorRunEnabled=$false;GameExecuted=$false;DebuggerExecuted=$false;ProcessQueried=$false;LiveBytesValidated=$false;ConfigReadProven=$false;GameRecipeReady=$false}
if($SelfTest){
    # Reborn: a synthetic identity record and disk snapshots exercise the real shared decision without claiming OS identity or live reads.
    $context=[pscustomobject]@{ProcessId=16;DebuggerProcessId=32;SnapshotCount=1;SnapshotProcessId=16;ParentProcessId=32;TargetHasExited=$false;DebuggerHasExited=$false;ExpectedExecutablePath=$image;ActualExecutablePath=$image;SnapshotExecutablePath=$image;CreationUtcTicks=20000000L;DebuggerCreationUtcTicks=10000000L;SnapshotCreationUtcTicks=20000000L;ObservedUtcTicks=30000000L;DiskImageSha256=$plan.ImageSha256;ModuleBase=0x400000L;ModuleSize=13754368}
    $initial="REBORN_OWNER 10`nREBORN_ADMISSION_WAIT"
    $diskRanges=@{};$memory=[IO.MemoryStream]::new($modBytes,$false);$pe=[Reflection.PortableExecutable.PEReader]::new($memory)
    try{foreach($range in $plan.Ranges){$mapped=Get-Ep11NativeRawRange $pe $modBytes $range.Rva $range.Length;$diskRanges[$range.Name]=$mapped.Bytes}}finally{$pe.Dispose();$memory.Dispose()}
    $script:rebornAdmissionReadCalls=0
    $adapter={param($range)
        # Reborn: return a private disk copy only; callback counts prove ordering, not actual memory access.
        $script:rebornAdmissionReadCalls++
        [pscustomobject]@{Name=$range.Name;Address=$range.Address;Succeeded=$true;ActualCount=$range.Length;Bytes=[byte[]]$diskRanges[$range.Name].Clone()}
    }
    $decision=Get-Ep11SupervisorAdmissionDecision $plan $context $initial $adapter
    if(-not $decision.CanIssueObservationContinue -or $decision.ValidatedRangeCount -ne 23 -or $script:rebornAdmissionReadCalls -ne 23){throw 'Detached shared native decision failed.'}
    foreach($mode in @('Owner','Debugger','Parent','SnapshotPid','SnapshotCount','TargetExited','DebuggerExited','ActualPath','SnapshotPath','CreationWindow','CreationSnapshot','ObservedTime','DiskHash','Base','ModuleSize','ExpectedPath','Transcript','Profile')){
        $fault=$context.PSObject.Copy();$faultPlan=$plan.PSObject.Copy();$text=$initial
        switch($mode){
            'Owner' {$fault.ProcessId=17}
            'Debugger' {$fault.DebuggerProcessId=33}
            'Parent' {$fault.ParentProcessId=33}
            'SnapshotPid' {$fault.SnapshotProcessId=17}
            'SnapshotCount' {$fault.SnapshotCount=2}
            'TargetExited' {$fault.TargetHasExited=$true}
            'DebuggerExited' {$fault.DebuggerHasExited=$true}
            'ActualPath' {$fault.ActualExecutablePath=$image+'.other'}
            'SnapshotPath' {$fault.SnapshotExecutablePath=$image+'.other'}
            'CreationWindow' {$fault.CreationUtcTicks=1}
            'CreationSnapshot' {$fault.SnapshotCreationUtcTicks+=10001}
            'ObservedTime' {$fault.ObservedUtcTicks=1}
            'DiskHash' {$fault.DiskImageSha256='0'*64}
            'Base' {$fault.ModuleBase=0x500000}
            'ModuleSize' {$fault.ModuleSize++}
            'ExpectedPath' {$fault.ExpectedExecutablePath='C:\other\ra3ep1_1.1.game';$fault.ActualExecutablePath=$fault.ExpectedExecutablePath;$fault.SnapshotExecutablePath=$fault.ExpectedExecutablePath}
            'Transcript' {$text+="`nREBORN_OWNER 10"}
            'Profile' {$faultPlan.Kind='Unknown'}
        }
        $script:rebornAdmissionReadCalls=0;$refused=$false
        try{$null=Get-Ep11SupervisorAdmissionDecision $faultPlan $fault $text $adapter}catch{$refused=$true}
        if(-not $refused -or $script:rebornAdmissionReadCalls -ne 0){throw 'Identity refusal occurred after a read or was admitted.'};$result.IdentityRefusals++
    }
    foreach($mode in @('Byte','Count','Status','Address','Name','Length','Ambiguous','Empty')){
        $script:rebornAdmissionReadCalls=0
        $faultAdapter={param($range)
            # Reborn: injected read faults affect private records, never the game image or live memory.
            $script:rebornAdmissionReadCalls++
            $record=[pscustomobject]@{Name=$range.Name;Address=$range.Address;Succeeded=$true;ActualCount=$range.Length;Bytes=[byte[]]$diskRanges[$range.Name].Clone()}
            switch($mode){
                'Byte' {$record.Bytes[0]=$record.Bytes[0] -bxor 1}
                'Count' {$record.ActualCount--}
                'Status' {$record.Succeeded=$false}
                'Address' {$record.Address++}
                'Name' {$record.Name='Other'}
                'Length' {$record.Bytes=[byte[]]::new(0)}
                'Ambiguous' {$record;$record;return}
                'Empty' {return}
            }
            $record
        }
        $refused=$false;try{$null=Get-Ep11SupervisorAdmissionDecision $plan $context $initial $faultAdapter}catch{$refused=$true}
        if(-not $refused -or $script:rebornAdmissionReadCalls -ne 1){throw 'Read fault did not stop at its first failed range.'};$result.ReadRefusals++
    }
    foreach($mode in @('Oversized','ZeroLength','Duplicate','Misaddressed','InvalidHash','OutsideImage','Budget')){
        $faultPlan=$plan.PSObject.Copy();$faultPlan.Ranges=@($plan.Ranges|ForEach-Object {$_.PSObject.Copy()})
        switch($mode){
            'Oversized' {$faultPlan.Ranges[0].Length=4097}
            'ZeroLength' {$faultPlan.Ranges[0].Length=0}
            'Duplicate' {$faultPlan.Ranges[1]=$faultPlan.Ranges[0]}
            'Misaddressed' {$faultPlan.Ranges[0].Address++}
            'InvalidHash' {$faultPlan.Ranges[0].ExpectedSha256='invalid'}
            'OutsideImage' {$faultPlan.Ranges[0].Rva=13754368}
            'Budget' {foreach($range in $faultPlan.Ranges){$range.Length=4096}}
        }
        $script:rebornAdmissionReadCalls=0;$refused=$false
        try{$null=Get-Ep11SupervisorAdmissionDecision $faultPlan $context $initial $adapter}catch{$refused=$true}
        if(-not $refused -or $script:rebornAdmissionReadCalls -ne 0){throw 'Range preflight fault occurred after read or was admitted.'};$result.RangeRefusals++
    }
    # Reborn: repeat the valid decision after all mutations to prove private fault copies did not poison the trusted baseline.
    $script:rebornAdmissionReadCalls=0
    $decision=Get-Ep11SupervisorAdmissionDecision $plan $context $initial $adapter
    if(-not $decision.CanIssueObservationContinue -or $decision.ValidatedRangeCount -ne 23 -or $script:rebornAdmissionReadCalls -ne 23){throw 'Detached baseline changed after fault injection.'}
    $result.DetachedDecisionValidated=$true
}
if($AsJson){$result|ConvertTo-Json -Depth 4}else{$result}
