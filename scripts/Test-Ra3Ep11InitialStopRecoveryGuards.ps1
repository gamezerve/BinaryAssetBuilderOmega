# Reborn: refuse invalid initial-stop injection combinations before helper/debugger creation; this test never executes a valid Run control.
[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
$control=Join-Path $PSScriptRoot 'Invoke-Ra3Ep11DebuggerAdmissionControl.ps1'
$expected='Initial-stop failure injection requires the separate admitted helper control.'
$cases=@(
    @{SelfTest=$true},
    @{Mode='RejectLiveBytes'},
    @{Mode='Timeout'},
    @{CalibrateBreakpoint=$true},
    @{CalibrateContext=$true},
    @{CalibrateDisplayProfile=$true},
    @{BreakpointDisposition='TimeoutDetachArmed'}
)
foreach($case in $cases){
    # Reborn: Run is deliberately supplied only with an invalid combination; require a known pre-action guard, not any incidental failure.
    $failure=$null
    try{& $control -Run -InjectObservationFailure InitialStopRefusal @case}catch{$failure=$_.Exception.Message}
    $allowed=@($expected,'Context calibration requires the separate Admit helper control.',
        'Display calibration requires separate helper clear/resume control.',
        'Armed breakpoint disposition requires helper breakpoint calibration.')
    if($failure -cnotin $allowed){throw 'Initial-stop invalid combination did not reach a recognized pre-action guard.'}
}
# Reborn: the historic unsafe armed-loss replay remains refused before any executable/path preparation.
$failure=$null
try{& $control -Run -CalibrateBreakpoint -BreakpointDisposition DebuggerLossArmed}catch{$failure=$_.Exception.Message}
if($failure -cne 'Armed debugger-loss execution disabled: byte restoration did not prove helper completion.'){
    throw 'Armed-loss replay guard differs.'
}
'Initial-stop recovery guards: PASS; eight pre-action refusals; no helper/debugger executed.'
