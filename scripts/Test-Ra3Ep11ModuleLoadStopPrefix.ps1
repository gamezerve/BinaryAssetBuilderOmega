# Reborn: detached module-line classification and fragmented bridge controls; no debugger, game, commands or process-memory reads.
[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
$null=. (Join-Path $PSScriptRoot 'Test-Ra3Ep11NativeOutputBridge.ps1')
$nonce='0123456789abcdef0123456789abcdef'
$stop='REBORN_NATIVE_STOP '+$nonce+' 0 10 20 4d86b0'
$module="ModLoad: 778f0000 77aaf000   ntdll.dll`r`n"
$prefix="0:000> 0:000> ModLoad: 75350000 753a5000   C:\WINDOWS\SysWOW64\MSCOREE.DLL`r`n"+$module
$positives=0;$refusals=0
$frame=Get-Ep11NativeOutputFrame ($prefix+$stop+"`r`n0:000> ") 'AwaitStop' $nonce -AllowModuleLoadPrefix
if($frame.ModuleLoads.Count -ne 2 -or $frame.ModuleLoads[0].Start -ne 0x75350000 -or $frame.ModuleLoads[0].IdentityValidated -or
   $frame.Text.Contains('ModLoad') -or -not $frame.Text.Contains($stop)){throw 'Module-only prefix classification failed.'};$positives++
foreach($name in @('image00680000','C:\Windows\Microsoft.NET\Framework\v4.0.30319\clr.dll')){
    $frame=Get-Ep11NativeOutputFrame ("ModLoad: 00680000 00688000   "+$name+"`n"+$stop+"`n0:000> ") 'AwaitStop' $nonce -AllowModuleLoadPrefix
    if($frame.ModuleLoads.Count -ne 1 -or $frame.ModuleLoads[0].PrintedName -cne $name){throw 'Calibrated image-name shape failed.'};$positives++
}
# Reborn: even a complete module line and stop record cannot authorize a response until the final prompt space arrives.
if($null -ne (Get-Ep11NativeOutputFrame ($prefix+$stop+"`n0:000>") 'AwaitStop' $nonce -AllowModuleLoadPrefix)){throw 'Partial terminal prompt accepted.'};$positives++
# Reborn: informational records with a complete prompt are not a substitute for a stop marker.
if($null -ne (Get-Ep11NativeOutputFrame ($prefix+"0:000> ") 'AwaitStop' $nonce -AllowModuleLoadPrefix)){throw 'Module-only output manufactured a stop.'};$positives++
$faults=@(
    ($prefix.Replace('753a5000','75350000')+$stop+"`n0:000> "),
    ($prefix.Replace('75350000','00000000')+$stop+"`n0:000> "),
    ($prefix.Replace('MSCOREE.DLL','bad;g.dll')+$stop+"`n0:000> "),
    ($prefix.Replace('MSCOREE.DLL',('a'*261)+'.dll')+$stop+"`n0:000> "),
    ($prefix.Replace('MSCOREE.DLL',"bad`t.dll")+$stop+"`n0:000> "),
    ($prefix.Replace('0:000> 0:000> ','0:000> 0:000> 0:000> ')+$stop+"`n0:000> "),
    ($stop+"`n"+$module+"0:000> "),
    (($module*33)+$stop+"`n0:000> "),
    ($module+$stop.Replace($nonce,'0'*32)+"`n0:000> "),
    ($module+$stop+"`n"+$stop+"`n0:000> "),
    ($module+"unexpected`n"+$stop+"`n0:000> "),
    ($module+"(d220.89f4): Unknown exception - code 04242420 (first chance)`n"+$stop+"`n0:000> "),
    ($module+$stop+"`neax=ffffffff`n0:000> ")
)
# Reborn: require independent fault cases so PowerShell expression/array precedence cannot collapse coverage into one oversized string.
if($faults.Count -ne 13){throw 'Module-prefix fault fixture count differs.'}
foreach($text in $faults){
    $refused=$false;try{$null=Get-Ep11NativeOutputFrame $text 'AwaitStop' $nonce -AllowModuleLoadPrefix}catch{$refused=$true}
    if(-not $refused){throw 'Malformed or unclassified prefix accepted.'};$refusals++
}
# Reborn: opt-in cannot weaken context/clear windows, nor does it change strict default framing.
foreach($phase in @('AwaitContext','AwaitClear','AwaitStop')){
    $refused=$false
    try{$null=Get-Ep11NativeOutputFrame ($prefix+$stop+"`n0:000> ") $phase $nonce -AllowModuleLoadPrefix:($phase -cne 'AwaitStop')}catch{$refused=$true}
    if(-not $refused){throw 'Module classification escaped its opt-in stop scope.'};$refusals++
}
$fixture=New-Ep11BridgeFixture;$controller=$fixture.Controller
$window=New-Ep11NativeOutputWindow $controller $fixture.Pump -AllowModuleLoadPrefix
$text=$prefix+(New-Ep11NativeStopFixture $controller.Observer $fixture.Snapshot)+"`n0:000> "
$request=Send-Ep11BridgeFragments $fixture $window $text 1001
if($window.Status -cne 'Consumed' -or $controller.Status -cne 'AwaitContext' -or $window.ModuleLoads.Count -ne 2 -or $script:filterTrace.Reads -ne 0){
    throw 'Fragmented module prefix bypassed stop-before-context boundary.'
};$positives++
$refused=$false;try{$null=New-Ep11NativeOutputWindow $controller $fixture.Pump -AllowModuleLoadPrefix}catch{$refused=$true}
if(-not $refused -or $controller.OutputWindowActive){throw 'Context window accepted module-prefix opt-in.'};$refusals++
# Reborn: a startup exception faults the bridge before context/capture and cannot be retried as a safe stopped event.
$fixture=New-Ep11BridgeFixture;$controller=$fixture.Controller
$window=New-Ep11NativeOutputWindow $controller $fixture.Pump -AllowModuleLoadPrefix
$text=$module+"(d220.89f4): Unknown exception - code 04242420 (first chance)`n"+(New-Ep11NativeStopFixture $controller.Observer $fixture.Snapshot)+"`n0:000> "
$fixture.Stdout.Supply($text);$null=Update-Ep11NativeOutputWindow $window $fixture.Identity $null 1001
if($window.Status -cne 'Faulted' -or $controller.Status -cne 'Unresolved' -or $script:filterTrace.Reads -ne 0){throw 'Unknown event bypassed unresolved boundary.'};$refusals++
$refused=$false;try{$null=Update-Ep11NativeOutputWindow $window $fixture.Identity $null 1002}catch{$refused=$true}
if(-not $refused){throw 'Unknown event window replayed.'};$refusals++
# Reborn: a syntactically framed module prefix never weakens the observer's independently admitted PID or fixed-site validation.
foreach($fieldIndex in @(3,5)){
    $fixture=New-Ep11BridgeFixture;$controller=$fixture.Controller
    $window=New-Ep11NativeOutputWindow $controller $fixture.Pump -AllowModuleLoadPrefix
    $parts=(New-Ep11NativeStopFixture $controller.Observer $fixture.Snapshot) -split ' '
    $parts[$fieldIndex]=if($fieldIndex -eq 3){'0'}else{'4d86b1'}
    $fixture.Stdout.Supply($module+($parts -join ' ')+"`n0:000> ")
    $null=Update-Ep11NativeOutputWindow $window $fixture.Identity $null 1001
    if($controller.Status -cne 'Unresolved' -or $script:filterTrace.Reads -ne 0){throw 'Module prefix bypassed admitted stop identity.'};$refusals++
}
[pscustomobject]@{DetachedPositives=$positives;Refusals=$refusals;ModuleIdentityProven=$false;GameExecuted=$false;DebuggerExecuted=$false;NativeRunEnabled=$false}
