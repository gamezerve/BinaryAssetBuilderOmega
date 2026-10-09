# Reborn: validate the exact scoped WPR candidate and ask WPR for metadata only; never start, stop, cancel or save a trace.
[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$ProfilePath,[switch]$SelfTest,[switch]$AsJson)
$ErrorActionPreference='Stop'

#-------------------------------------------------------------------------------------------------
<# Reborn: read at most 16 KiB from an explicit non-reparse profile path while denying writers. #>
#-------------------------------------------------------------------------------------------------
function Read-Ep11TraceProfile([string]$Path) {
    if(-not [IO.Path]::IsPathFullyQualified($Path)){throw 'Trace profile path must be absolute.'}
    $item=Get-Item -LiteralPath $Path -Force
    if($item.PSIsContainer -or $item.Length-lt 1 -or $item.Length-gt 16384){throw 'Trace profile file bound exceeded.'}
    for($ancestor=$item;$null-ne $ancestor;){
        if(($ancestor.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Reparse trace profile is not admitted.'}
        $ancestor=if($ancestor-is [IO.FileInfo]){$ancestor.Directory}else{$ancestor.Parent}
    }
    $stream=[IO.File]::Open($item.FullName,[IO.FileMode]::Open,[IO.FileAccess]::Read,[IO.FileShare]::Read)
    try{if($stream.Length-lt 1 -or $stream.Length-gt 16384){throw 'Trace profile length changed beyond policy.'};$bytes=[byte[]]::new([int]$stream.Length);$stream.ReadExactly($bytes,0,$bytes.Length);return ,$bytes}finally{$stream.Dispose()}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: pin canonical scope/settings XML with DTD and external resolution disabled; formatting changes cannot broaden capture silently. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11TraceProfile([string]$Text) {
    if($Text.Length-gt 16384){throw 'Trace profile XML bound exceeded.'}
    $settings=[Xml.XmlReaderSettings]::new();$settings.DtdProcessing=[Xml.DtdProcessing]::Prohibit;$settings.XmlResolver=$null;$settings.IgnoreWhitespace=$true;$settings.IgnoreComments=$true;$settings.MaxCharactersInDocument=16384
    $input=[IO.StringReader]::new($Text);$reader=[Xml.XmlReader]::Create($input,$settings)
    try{$document=[Xml.XmlDocument]::new();$document.XmlResolver=$null;$document.Load($reader)}finally{$reader.Dispose();$input.Dispose()}
    $canonical=[Text.Encoding]::UTF8.GetBytes($document.DocumentElement.OuterXml)
    $hash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($canonical))
    if($hash-cne '330D94E9E661765D6AB06D1DDFE6D32FD4203980D12E62EAE307C3F8C1A8BEE2'){throw 'Unreviewed trace profile scope/settings.'}
    return $hash
}
$profileBytes=Read-Ep11TraceProfile $ProfilePath
$profileText=[Text.UTF8Encoding]::new($false,$true).GetString($profileBytes)
$scopeHash=Assert-Ep11TraceProfile $profileText
if($SelfTest){
    # Reborn: detached mutations target process/event filters, provider breadth, stacks, buffering and logging mode; none are passed to WPR.
    foreach($pair in @(
        @('ProcessExeFilter="ra3ep1_1.1.game"',''),@('FilterIn="true"','FilterIn="false"'),
        @('EventId Value="15"','EventId Value="16"'),@('Value="0x1D0"','Value="0xFFFFFFFFFFFFFFFF"'),
        @('Stack="false"','Stack="true"'),@('<Buffers Value="64"','<Buffers Value="4096"'),
        @('LoggingMode="Memory"','LoggingMode="File"'),@('<Profiles>','<Profiles><SystemProvider Id="Unscoped" />')
    )){$fault=$profileText.Replace($pair[0],$pair[1]);if($fault-ceq $profileText){throw 'Trace fault did not change its target.'};$rejected=$false
        try{Assert-Ep11TraceProfile $fault|Out-Null}catch{$rejected=$true};if(-not $rejected){throw 'Broadened trace profile admitted.'}
    }
    foreach($fault in @('<!DOCTYPE x [<!ENTITY a "value">]><x>&a;</x>',('x'*16385))){$rejected=$false;try{Assert-Ep11TraceProfile $fault|Out-Null}catch{$rejected=$true};if(-not $rejected){throw 'Unsafe trace XML admitted.'}}
}
$fullProfile=[IO.Path]::GetFullPath($ProfilePath)
# Reborn: hard-code the Windows metadata-only entry point; no start/stop/cancel/export command is supported here.
$wprPath=Join-Path ([Environment]::GetFolderPath([Environment+SpecialFolder]::Windows)) 'System32/wpr.exe'
$list=(& $wprPath -profiles $fullProfile 2>&1|Out-String);if($LASTEXITCODE-ne 0){throw 'WPR refused the candidate profile.'}
$details=(& $wprPath -profiledetails ($fullProfile+'!Ra3Ep11ConfigRead.Light') 2>&1|Out-String);if($LASTEXITCODE-ne 0){throw 'WPR refused candidate profile details.'}
if($list.Length-gt 32768 -or $details.Length-gt 32768){throw 'WPR metadata output bound exceeded.'}
$after=Read-Ep11TraceProfile $ProfilePath
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($after))-cne [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($profileBytes))){throw 'Trace profile changed during review.'}
$report=[pscustomobject]@{
    ProfilePath=$fullProfile;ScopeCanonicalSha256=$scopeHash;WprMetadataAccepted=$true
    RequestedProvider='Microsoft-Windows-Kernel-File';RequestedProviderGuid='edd08927-9cc4-4e65-b970-c2560fb5c289';RequestedProcessExeFilter='ra3ep1_1.1.game';RequestedEventIds=@(10,11,12,15,24);RequestedKeywordMask='0x1D0'
    RequestedBufferSizeKiB=64;RequestedBuffers=64;ConfiguredBufferProductKiB=4096;RequestedLoggingMode='Memory';RequestedStacks=$false
    ScopeMutationsRejected=$(if($SelfTest){8}else{0});XmlPolicyRefusals=$(if($SelfTest){2}else{0})
    ReadOnly=$true;TraceStarted=$false;TargetExecuted=$false;RuntimeProcessFilterValidated=$false;FileNameCorrelationValidated=$false;SuccessfulReadCompletionValidated=$false;ActualTransferSizeValidated=$false;ConfigConsumedProven=$false;ModPackageLoaded=$false;ProductionBuildReady=$false
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 6}else{$report}
