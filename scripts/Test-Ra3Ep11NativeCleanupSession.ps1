# Reborn: test five-site cleanup with admission-captured disk bytes and private lifetime records; no debugger/game/process API.
[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
$null=. (Join-Path $PSScriptRoot 'Test-Ra3Ep11NativeObservationSession.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep11NativeCleanupSession.ps1')

#-------------------------------------------------------------------------------------------------
<# Reborn: create a known supplied stop and copied admission identity, including a separate stopped-state assertion. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11CleanupFixture([switch]$ObservationFault) {
    $observer=New-Ep11NativeFixture;$snapshot=(New-Ep11SnapshotFixture)[0]
    $null=Start-Ep11NativeObservationStop $observer (New-Ep11NativeStopFixture $observer $snapshot)
    if($ObservationFault){$observer.Status='Faulted';$observer.Decoder.Status='Faulted'}
    $identity=$nativeIdentity.PSObject.Copy();$identity|Add-Member TargetStopped $true
    $identity|Add-Member TargetThreadId $snapshot.ThreadId;$identity|Add-Member TargetSiteAddress $snapshot.SiteAddress
    $cleanup=New-Ep11NativeCleanupSession $observer $identity
    $post=$identity.PSObject.Copy();$post.DebuggerHasExited=$true;$post|Add-Member DebuggerExitCode ([int]0)
    @{Observer=$observer;Session=$cleanup;Before=$identity;After=$post;Text=("REBORN_CLEAR "+$cleanup.Nonce+"`n0:000> ")}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: return one address-bound copied original byte with counted synthetic reads and optional fault injection. #>
#-------------------------------------------------------------------------------------------------
function New-Ep11CleanupReader($Fixture,[string]$Mode='Valid',[int]$FaultSite=0) {
    $sites=@($Fixture.Session.Sites.Clone());$bytes=[byte[]]$Fixture.Session.OriginalBytes.Clone();$trace=$script:cleanupTrace
    {
        param([long]$Address,[int]$Count)
        $trace.Reads++;$index=[array]::IndexOf($sites,$Address)
        if($index -lt 0 -or $Count -ne 1){throw 'Cleanup requested an unexpected site/count.'}
        $record=[pscustomobject]@{Address=$Address;Success=$true;ActualCount=1;Bytes=[byte[]]@($bytes[$index])}
        if($index -eq $FaultSite){
            switch($Mode){
                'Byte' {$record.Bytes[0]=$record.Bytes[0] -bxor 1}
                'Trap' {$record.Bytes[0]=0xcc}
                'Short' {$record.ActualCount=0}
                'Address' {$record.Address++}
                'Failed' {$record.Success=$false}
                'StatusType' {$record.Success=1}
                'Length' {$record.Bytes=[byte[]]@(1,2)}
                'Missing' {return}
                'Duplicate' {$record;$record;return}
            }
        }
        $record
    }.GetNewClosure()
}
$positives=0;$refusals=0
foreach($observationFault in @($false,$true)){
    $fixture=New-Ep11CleanupFixture -ObservationFault:$observationFault;$script:cleanupTrace=@{Reads=0};$reader=New-Ep11CleanupReader $fixture
    Add-Ep11NativeCleanupClear $fixture.Session $fixture.Before $fixture.Text $reader
    $result=Complete-Ep11NativeCleanupSession $fixture.Session $fixture.After $reader
    if(-not $result.SuppliedCleanupValidated -or $script:cleanupTrace.Reads -ne 10 -or $result.LiveCleanupProven -or $result.CommandsSent -or
       ($observationFault -and $result.OriginalObservationStatus -cne 'Faulted') -or -not $fixture.Observer.NeedsBreakpointCleanup){throw 'Supplied cleanup positive boundary failed.'};$positives++
}
foreach($phase in @('Clear','Detach')){
    foreach($mode in @('Pid','Creation','Debugger','Base','Path','Hash','TargetExited','DebuggerLifetime','StopThread','StopSite','StoppedFlag','Nonce','Partial','Nonempty','DuplicateMarker','Error','ExitCode')){
        if($phase -ceq 'Detach' -and $mode -in @('StopThread','StopSite','StoppedFlag','Nonce','Partial','Nonempty','DuplicateMarker','Error')){continue}
        if($phase -ceq 'Clear' -and $mode -ceq 'ExitCode'){continue}
        $fixture=New-Ep11CleanupFixture;$script:cleanupTrace=@{Reads=0};$reader=New-Ep11CleanupReader $fixture
        if($phase -ceq 'Detach'){Add-Ep11NativeCleanupClear $fixture.Session $fixture.Before $fixture.Text $reader;$script:cleanupTrace.Reads=0}
        $identity=if($phase -ceq 'Clear'){$fixture.Before.PSObject.Copy()}else{$fixture.After.PSObject.Copy()};$text=$fixture.Text
        switch($mode){
            'Pid' {$identity.ProcessId++}
            'Creation' {$identity.CreationUtcTicks++}
            'Debugger' {$identity.DebuggerProcessId++}
            'Base' {$identity.ModuleBase++}
            'Path' {$identity.ActualExecutablePath+='other'}
            'Hash' {$identity.DiskImageSha256='0'*64}
            'TargetExited' {$identity.TargetHasExited=$true}
            'DebuggerLifetime' {$identity.DebuggerHasExited=-not $identity.DebuggerHasExited}
            'StopThread' {$identity.TargetThreadId++}
            'StopSite' {$identity.TargetSiteAddress++}
            'StoppedFlag' {$identity.TargetStopped=$false}
            'Nonce' {$text=$text.Replace($fixture.Session.Nonce,'0'*32)}
            'Partial' {$text="REBORN_CLEAR "+$fixture.Session.Nonce}
            'Nonempty' {$text+="`n 4 e 004d98ab 0001 (0001)"}
            'DuplicateMarker' {$text+="`nREBORN_CLEAR "+$fixture.Session.Nonce+"`n"}
            'Error' {$text+="`nSyntax error"}
            'ExitCode' {$identity.DebuggerExitCode=-1}
        }
        $refused=$false;try{
            if($phase -ceq 'Clear'){Add-Ep11NativeCleanupClear $fixture.Session $identity $text $reader}else{$null=Complete-Ep11NativeCleanupSession $fixture.Session $identity $reader}
        }catch{$refused=$true}
        if(-not $refused -or $script:cleanupTrace.Reads -ne 0 -or $fixture.Session.Status -cne 'Faulted' -or -not $fixture.Session.NeedsUserAction){throw ('Cleanup envelope fault admitted/read: '+$phase+'/'+$mode)};$refusals++
    }
    foreach($mode in @('Byte','Trap','Short','Address','Failed','StatusType','Length','Missing','Duplicate')){
        foreach($site in 0..4){
            $fixture=New-Ep11CleanupFixture;$script:cleanupTrace=@{Reads=0}
            if($phase -ceq 'Detach'){Add-Ep11NativeCleanupClear $fixture.Session $fixture.Before $fixture.Text (New-Ep11CleanupReader $fixture);$script:cleanupTrace.Reads=0}
            $reader=New-Ep11CleanupReader $fixture $mode $site;$refused=$false
            try{if($phase -ceq 'Clear'){Add-Ep11NativeCleanupClear $fixture.Session $fixture.Before $fixture.Text $reader}else{$null=Complete-Ep11NativeCleanupSession $fixture.Session $fixture.After $reader}}catch{$refused=$true}
            if(-not $refused -or $script:cleanupTrace.Reads -ne $site+1 -or $fixture.Session.Status -cne 'Faulted' -or -not $fixture.Session.NeedsUserAction){throw 'Cleanup transfer fault continued after failed site.'}
            $retry=$false;try{Add-Ep11NativeCleanupClear $fixture.Session $fixture.Before $fixture.Text $reader}catch{$retry=$true}
            if(-not $retry -or $script:cleanupTrace.Reads -ne $site+1){throw 'Faulted cleanup allowed another read.'};$refusals+=2
        }
    }
}
# Reborn: unknown stops cannot open cleanup; post-detach evidence cannot precede a validated empty listing and all five bytes.
$observer=New-Ep11NativeFixture;$identity=$nativeIdentity.PSObject.Copy();$identity|Add-Member TargetStopped $true
$refused=$false;try{$null=New-Ep11NativeCleanupSession $observer $identity}catch{$refused=$true};if(-not $refused){throw 'Unknown stop admitted to cleanup.'};$refusals++
$fixture=New-Ep11CleanupFixture;$script:cleanupTrace=@{Reads=0}
$refused=$false;try{$null=Complete-Ep11NativeCleanupSession $fixture.Session $fixture.After (New-Ep11CleanupReader $fixture)}catch{$refused=$true}
if(-not $refused -or $script:cleanupTrace.Reads -ne 0){throw 'Detach bypassed clear validation.'};$refusals++
[pscustomobject]@{DetachedPositives=$positives;RefusalAndRetryChecks=$refusals;SitesCheckedBeforeAndAfterDetach=5;GameExecuted=$false;DebuggerExecuted=$false;ProcessQueried=$false;LiveCleanupProven=$false;NativeRunEnabled=$false}
