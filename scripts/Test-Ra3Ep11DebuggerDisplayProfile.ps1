# Reborn: validate supplied English display reports and helper-stop frames only; no debugger or target operation.
[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
. (Join-Path $PSScriptRoot 'Ra3Ep11DebuggerDisplayProfile.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep11NativeOutputBridge.ps1')
$request=New-Ep11DebuggerDisplayRequest;$nonce=$request.Nonce
$text="Allow the following information to be displayed at the prompt:`n(Other settings can affect whether the information is actually displayed)`n  (none)`nDo not allow the following information to be displayed at the prompt:`n reg - Register state`n dis - Disassembly of current instruction`n ea - Effective address`n src - Source info`n sym - Symbol`nREBORN_DISPLAY "+$nonce+"`n0:000> "
Assert-Ep11DebuggerDisplayProfile $text $nonce
$positives=1;$refusals=0
# Reborn: replay the actual first CDB report shape without treating its initial rejected run as successful calibration.
Assert-Ep11DebuggerDisplayProfile ('0:000> '+$text.Replace('(none)','None')) $nonce;$positives++
foreach($mode in @('Nonce','Missing','Duplicate','Allowed','Unknown','Partial','Oversized','Error','AckBeforeList','DuplicateAck')){
    $fault=switch($mode){
        'Nonce' {$text.Replace($nonce,'0'*32)}
        'Missing' {$text.Replace(" sym - Symbol`n",'')}
        'Duplicate' {$text.Replace(" sym - Symbol`n"," sym - Symbol`n sym - Symbol`n")}
        'Allowed' {$text.Replace("  (none)`n"," reg - Register state`n")}
        'Unknown' {$text.Replace(" src - Source info`n"," unknown - Source info`n")}
        'Partial' {$text.TrimEnd(' ')}
        'Oversized' {$text+('x'*4096)}
        'Error' {$text.Replace("  (none)`n","Syntax error`n")}
        'AckBeforeList' {("REBORN_DISPLAY "+$nonce+"`n"+$text.Replace(("REBORN_DISPLAY "+$nonce+"`n"),''))}
        'DuplicateAck' {$text.Replace(("REBORN_DISPLAY "+$nonce+"`n"),("REBORN_DISPLAY "+$nonce+"`nREBORN_DISPLAY "+$nonce+"`n"))}
    }
    $refused=$false;try{Assert-Ep11DebuggerDisplayProfile $fault $nonce}catch{$refused=$true};if(-not $refused){throw ('Display report fault admitted: '+$mode)};$refusals++
}
$sample="REBORN_BREAK "+$nonce+" 10 20 77969ee0`n0:000> "
$frame=Get-Ep11NativeOutputFrame $sample 'AwaitHelperStop' $nonce
$null=Get-Ep11HelperBreakpointHit $frame.Text $nonce 16 0x77969ee0;$positives++
$frameFaults=@(
    $sample.Replace($nonce,('0'*32)),
    $sample.Replace("`n0:000> ","`neax=ffffffff`n0:000> "),
    $sample.Replace("`n0:000> ",("`nREBORN_BREAK "+$nonce+" 10 20 77969ee0`n0:000> "))
)
foreach($fault in $frameFaults){
    $refused=$false;try{$null=Get-Ep11NativeOutputFrame $fault 'AwaitHelperStop' $nonce}catch{$refused=$true};if(-not $refused){throw 'Helper display frame fault admitted.'};$refusals++
}
if($request.Command -cne ('.prompt_allow -reg -dis -ea -src -sym; .echo REBORN_DISPLAY '+$nonce)){throw 'Display request command differs.'}
[pscustomobject]@{DetachedPositives=$positives;Refusals=$refusals;GameExecuted=$false;DebuggerExecuted=$false;NativeRunEnabled=$false}
