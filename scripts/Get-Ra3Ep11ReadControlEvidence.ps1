# Reborn: inspect an explicit bounded saved ETL read-only; report only exact control-path requests and adjacent completion candidates, never unrelated file paths.
[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$TracePath,[Parameter(Mandatory=$true)][uint32]$PositiveProcessId,[Parameter(Mandatory=$true)][uint32]$NegativeProcessId,[Parameter(Mandatory=$true)][string]$CapturedProbePath,[switch]$SelfTest,[switch]$AsJson)
$ErrorActionPreference='Stop'
if(-not [IO.Path]::IsPathFullyQualified($TracePath) -or $PositiveProcessId-eq 0 -or $NegativeProcessId-eq 0 -or $PositiveProcessId-eq $NegativeProcessId -or -not $CapturedProbePath.StartsWith('\Device\',[StringComparison]::Ordinal) -or -not $CapturedProbePath.EndsWith('\config-read-only.cfg',[StringComparison]::Ordinal)){throw 'Trace/control identities outside policy.'}
$item=Get-Item -LiteralPath $TracePath -Force
if($item.PSIsContainer -or $item.Length-lt 1 -or $item.Length-gt 33554432){throw 'Control ETL exceeds 32 MiB policy.'}
for($ancestor=$item;$null-ne $ancestor;){if(($ancestor.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Reparse ETL refused.'};$ancestor=if($ancestor-is [IO.FileInfo]){$ancestor.Directory}else{$ancestor.Parent}}

#-------------------------------------------------------------------------------------------------
<# Reborn: map the latest observed create by PID/file-object to exact path; completion is only an adjacent selected-event candidate, not proof of every intervening operation. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11ControlReadCandidates([object[]]$Records,[uint32]$ProcessId,[string]$ExactPath) {
    $objects=@{};$rows=[Collections.Generic.List[object]]::new()
    for($index=0;$index-lt $Records.Count;$index++){
        $record=$Records[$index];if($record.Pid-ne $ProcessId){continue}
        if($record.Id-eq 12){$objects[$record.Fields.FileObject]=$record.Fields.FileName;continue}
        if($record.Id-ne 15 -or -not $record.Fields.FileObject -or $objects[$record.Fields.FileObject]-ine $ExactPath){continue}
        $candidate=$null
        for($next=$index+1;$next-lt $Records.Count;$next++){
            $later=$Records[$next];if($later.Pid-ne $ProcessId -or $later.Fields.Irp-cne $record.Fields.Irp -or $later.Id-notin @(12,15,24)){continue}
            if($later.Id-eq 24){$candidate=[pscustomobject]@{Ordinal=$later.Ordinal;Utc=$later.Utc;Status=$later.Fields.Status;ExtraInformation=$later.Fields.ExtraInformation}}
            break
        }
        $rows.Add([pscustomobject]@{Pid=$ProcessId;Ordinal=$record.Ordinal;Utc=$record.Utc;FileObject=$record.Fields.FileObject;Irp=$record.Fields.Irp;RequestedIOSize=$record.Fields.IOSize;ByteOffset=$record.Fields.ByteOffset;CompletionCandidate=$candidate})
    }
    return $rows.ToArray()
}
if($SelfTest){
    # Reborn: detached fixtures distinguish exact paths, replacement creates and missing/wrong completion IRPs without fabricating ETL records.
    $create=[pscustomobject]@{Pid=1;Id=12;Fields=@{FileObject='object';FileName=$CapturedProbePath;Irp='open'}}
    $read=[pscustomobject]@{Pid=1;Id=15;Ordinal=2;Utc='fixture';Fields=@{FileObject='object';Irp='read';IOSize='1';ByteOffset='0'}}
    $end=[pscustomobject]@{Pid=1;Id=24;Ordinal=3;Utc='fixture';Fields=@{Irp='read';Status='0';ExtraInformation='0x1'}}
    if(@(Get-Ep11ControlReadCandidates @($create,$read,$end) 1 $CapturedProbePath).Count-ne 1){throw 'Positive detached request fixture failed.'}
    if(@(Get-Ep11ControlReadCandidates @($create,$read,$end) 2 $CapturedProbePath).Count-ne 0 -or @(Get-Ep11ControlReadCandidates @($create,$read,$end) 1 ($CapturedProbePath+'other')).Count-ne 0){throw 'Detached PID/path boundary failed.'}
    $wrongEnd=[pscustomobject]@{Pid=1;Id=24;Fields=@{Irp='different'}}
    if($null-ne @(Get-Ep11ControlReadCandidates @($create,$read,$wrongEnd) 1 $CapturedProbePath)[0].CompletionCandidate){throw 'Wrong completion IRP admitted.'}
    $replace=[pscustomobject]@{Pid=1;Id=12;Fields=@{FileObject='object';FileName='different';Irp='open'}}
    if(@(Get-Ep11ControlReadCandidates @($create,$replace,$read,$end) 1 $CapturedProbePath).Count-ne 0){throw 'Stale create mapping admitted.'}
}
$handle=[IO.File]::Open($item.FullName,[IO.FileMode]::Open,[IO.FileAccess]::Read,[IO.FileShare]::Read)
try{
    $sha=[Security.Cryptography.SHA256]::Create();try{$hash=[Convert]::ToHexString($sha.ComputeHash($handle))}finally{$sha.Dispose()}
    $records=[Collections.Generic.List[object]]::new();$counts=@{};$total=0;$unselected=0;$otherPids=[Collections.Generic.HashSet[uint32]]::new()
    foreach($event in Get-WinEvent -Path $item.FullName -Oldest -ErrorAction Stop){
        try{
            $total++;if($total-gt 100000){throw 'Control trace event-count bound exceeded.'}
            if($event.ProviderName-cne 'Microsoft-Windows-Kernel-File'){$unselected++;continue}
            if($event.Id-notin @(10,11,12,15,24)){throw 'Unexpected file-provider event ID.'}
            $counts[$event.Id]=1+$counts[$event.Id]
            if($event.ProcessId-notin @($PositiveProcessId,$NegativeProcessId)){[void]$otherPids.Add([uint32]$event.ProcessId)}
            [xml]$xml=$event.ToXml();$fields=@{}
            foreach($data in $xml.Event.EventData.Data){if($fields.ContainsKey($data.Name)){throw 'Duplicate trace payload field.'};$fields[$data.Name]=$data.'#text'}
            # Reborn: retain an explicitly labeled UTC string so JSON readers cannot silently convert it to local DateTime values.
            $records.Add([pscustomobject]@{Id=$event.Id;Pid=$event.ProcessId;Ordinal=$total;Utc=($event.TimeCreated.ToUniversalTime().ToString('yyyy-MM-dd HH:mm:ss.fffffff')+' UTC');Fields=$fields})
        }finally{$event.Dispose()}
    }
    $positive=@(Get-Ep11ControlReadCandidates $records.ToArray() $PositiveProcessId $CapturedProbePath)
    $negative=@(Get-Ep11ControlReadCandidates $records.ToArray() $NegativeProcessId $CapturedProbePath)
    $handle.Position=0;$sha=[Security.Cryptography.SHA256]::Create();try{if([Convert]::ToHexString($sha.ComputeHash($handle))-cne $hash){throw 'ETL identity changed.'}}finally{$sha.Dispose()}
}finally{$handle.Dispose()}
$report=[pscustomobject]@{
    TraceBytes=$item.Length;TraceSha256=$hash;DecodedEventCount=$total;FileProviderEventCount=$records.Count;UnselectedProviderEventCount=$unselected;OtherProcessCount=$otherPids.Count
    FileEventCounts=@($counts.GetEnumerator()|Sort-Object Name|ForEach-Object{[pscustomobject]@{Id=$_.Name;Count=$_.Value}})
    PositiveProbeReadRequests=$positive;NegativeProbeReadRequests=$negative;PositiveRequestCount=$positive.Count;NegativeRequestCount=$negative.Count
    ProcessFilterCounterexampleObserved=($negative.Count-gt 0);RequestedCaptureScopeRejected=($negative.Count-gt 0 -or $otherPids.Count-gt 0)
    DetachedFixtureChecks=$(if($SelfTest){5}else{0});ReadOnly=$true;TraceStarted=$false;GameExecuted=$false
    LostEventStatisticsValidated=$false;CapturedDevicePathAliasIndependentlyValidated=$false;CompletionCandidatesUniquelyTyped=$false;ActualTransferSizeProven=$false;ConfigConsumedByGameProven=$false;ProductionBuildReady=$false
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 8}else{$report}
