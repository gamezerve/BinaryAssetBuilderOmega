# Reborn: pin the supplied real control trace's counterexample and repeat read-only decoding; never approve the rejected recording scope.
[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$TracePath,[Parameter(Mandatory=$true)][uint32]$PositiveProcessId,[Parameter(Mandatory=$true)][uint32]$NegativeProcessId,[Parameter(Mandatory=$true)][string]$CapturedProbePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11ReadControlEvidence.ps1'
$arguments=@{TracePath=$TracePath;PositiveProcessId=$PositiveProcessId;NegativeProcessId=$NegativeProcessId;CapturedProbePath=$CapturedProbePath}
$first=& $audit @arguments -SelfTest
$second=& $audit @arguments -AsJson|ConvertFrom-Json
if($first.TraceSha256-cne '6D2C1ECCE053540D5A5386D65EB6324DD495A1270F66F16ACD804D893C8E9C81' -or $first.TraceBytes-ne 2752512 -or
    $first.DecodedEventCount-ne 4287 -or $first.FileProviderEventCount-ne 4199 -or $first.OtherProcessCount-ne 24 -or
    $first.PositiveRequestCount-ne 1 -or $first.NegativeRequestCount-ne 1 -or -not $first.ProcessFilterCounterexampleObserved -or -not $first.RequestedCaptureScopeRejected -or $first.DetachedFixtureChecks-ne 5){throw 'Reviewed real control trace contract differs.'}
foreach($row in @($first.PositiveProbeReadRequests)+@($first.NegativeProbeReadRequests)){
    if($row.RequestedIOSize-cne '1' -or $row.ByteOffset-cne '0' -or $null-eq $row.CompletionCandidate -or $row.CompletionCandidate.Status-cne '0' -or $row.CompletionCandidate.ExtraInformation-cne '0x1'){throw 'Observed request/completion candidate differs.'}
}
foreach($gate in @('TraceStarted','GameExecuted','LostEventStatisticsValidated','CapturedDevicePathAliasIndependentlyValidated','CompletionCandidatesUniquelyTyped','ActualTransferSizeProven','ConfigConsumedByGameProven','ProductionBuildReady')){if($first.$gate){throw "Unresolved trace gate claimed: $gate"}}
foreach($property in $first.PSObject.Properties){
    if($property.Name-eq 'DetachedFixtureChecks'){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 8 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 8 -Compress)){throw "Repeat control evidence differs: $($property.Name)"}
}
Write-Output 'Real EP1 read-control evidence: PASS; fixed ETL identity, five detached checks and repeat decoding. Positive AND negative fixture reads are present, with adjacent status-zero completion candidates. Requested process-name capture scope is REJECTED; no game/config-consumption or loss-free correlation proof.'
