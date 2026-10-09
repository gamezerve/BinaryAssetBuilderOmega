# Reborn: admit bounded initial debugger identity and optional breakpoint evidence for one newly owned helper only.

#-------------------------------------------------------------------------------------------------
<# Reborn: read a bounded generated debugger log while the detached helper still inherits its output handle. #>
#-------------------------------------------------------------------------------------------------
function Read-Ep11OwnedDebuggerLog([string]$Path) {
    $stream=[IO.File]::Open($Path,[IO.FileMode]::Open,[IO.FileAccess]::Read,[IO.FileShare]::ReadWrite)
    try{
        if($stream.Length -gt 65536){throw 'Owned debugger log exceeds bound.'}
        $bytes=[byte[]]::new([int]$stream.Length)
        $stream.ReadExactly($bytes)
        [Text.Encoding]::UTF8.GetString($bytes)
    }finally{$stream.Dispose()}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: reject mismatched, duplicate or missing initial target records before interpreting a cleanup observation. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11OwnedDebuggerEvidence([string]$Text,[int]$ExpectedProcessId,[bool]$RequireBreakpoint) {
    if($ExpectedProcessId -le 0 -or $Text.Length -gt 65536 -or $Text -match '(?i)syntax error|could not resolve|unable to|memory access error'){throw 'Invalid owned-debugger evidence envelope.'}
    $owner=$null;$site=$null;$hits=0
    foreach($line in ($Text -split '\r?\n')){
        if($line -match '^REBORN_OWNER ([0-9a-f]+)$'){
            if($null -ne $owner){throw 'Duplicate initial owner.'}
            $owner=[Convert]::ToInt32($Matches[1],16);continue
        }
        if($line -match '^REBORN_SITE ([0-9a-f]{8}) ([0-9a-f]{1,2})$'){
            if($null -eq $owner -or $null -ne $site){throw 'Ambiguous or out-of-order site.'}
            $address=[Convert]::ToUInt32($Matches[1],16);$value=[Convert]::ToByte($Matches[2],16)
            if($address -eq 0 -or $value -eq 0xcc){throw 'Null or pre-existing trap site.'}
            $site=[pscustomobject]@{Address=$address;OriginalByte=$value};continue
        }
        if($line -ceq 'REBORN_BP_HIT'){
            if($null -eq $site -or $hits -ne 0){throw 'Ambiguous breakpoint hit.'};$hits++;continue
        }
        if($line.StartsWith('REBORN_')){throw 'Unknown ownership record.'}
    }
    if($owner -ne $ExpectedProcessId){throw 'Initial debugger target differs from owned helper.'}
    if($RequireBreakpoint -and ($null -eq $site -or $hits -ne 1)){throw 'Incomplete breakpoint observation.'}
    if(-not $RequireBreakpoint -and ($null -ne $site -or $hits -ne 0)){throw 'Unexpected breakpoint observation.'}
    [pscustomobject]@{InitialTargetProcessId=$owner;BreakpointSite=$site;BreakpointHitCount=$hits}
}
