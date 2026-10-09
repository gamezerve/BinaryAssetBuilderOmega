# Reborn: verify candidate scope/refusals/repeat metadata while keeping all live trace and game-load gates false.
[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$ProfilePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep11ConfigTraceProfile.ps1'
$first=& $audit -ProfilePath $ProfilePath -SelfTest
$second=& $audit -ProfilePath $ProfilePath -AsJson|ConvertFrom-Json
if(-not $first.ReadOnly -or -not $first.WprMetadataAccepted -or $first.RequestedProcessExeFilter-cne 'ra3ep1_1.1.game' -or
    $first.RequestedKeywordMask-cne '0x1D0' -or ($first.RequestedEventIds-join ',')-cne '10,11,12,15,24' -or
    $first.ConfiguredBufferProductKiB-ne 4096 -or $first.RequestedStacks -or $first.ScopeMutationsRejected-ne 8 -or $first.XmlPolicyRefusals-ne 2){throw 'Trace candidate contract differs.'}
foreach($gate in @('TraceStarted','TargetExecuted','RuntimeProcessFilterValidated','FileNameCorrelationValidated','SuccessfulReadCompletionValidated','ActualTransferSizeValidated','ConfigConsumedProven','ModPackageLoaded','ProductionBuildReady')){
    if($first.$gate){throw "Unvalidated trace gate claimed: $gate"}
}
foreach($property in $first.PSObject.Properties){
    if($property.Name-in @('ScopeMutationsRejected','XmlPolicyRefusals')){continue}
    if((ConvertTo-Json -InputObject $property.Value -Depth 6 -Compress)-cne (ConvertTo-Json -InputObject $second.($property.Name) -Depth 6 -Compress)){throw "Repeat trace field differs: $($property.Name)"}
}
Write-Output 'EP1 config trace candidate: PASS; WPR metadata acceptance, one process-name/provider/five-event request, 4 MiB configured buffer product, eight scope faults and two XML refusals plus repeat JSON. No trace/game started; live filtering, file/read/completion correlation and config consumption remain unvalidated.'
