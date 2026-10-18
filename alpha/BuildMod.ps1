# Reborn: convenient stable SDK command; delegate to the configured PowerShell 7 backend rather than the caller's legacy host.
[CmdletBinding()]
param([string]$ModName='RebornTest',[switch]$ValidateOnly)
$ErrorActionPreference='Stop';$settings=@{}
# Reborn: internal maintenance without an explicit selection must target the developer slot, never the user's mod.
if([string]::IsNullOrWhiteSpace($ModName)){$ModName='RebornTest'}
foreach($line in Get-Content -LiteralPath (Join-Path $PSScriptRoot 'uprising-alpha.paths')){if($line -and -not $line.StartsWith('#')){$pair=$line.Split('=',2);$settings[$pair[0]]=$pair[1]}}
$arguments=@('-NoProfile','-File',$settings.Driver,'-ModName',$ModName);if($ValidateOnly){$arguments+='-ValidateOnly'}
& $settings.PowerShell7 @arguments
exit $LASTEXITCODE
