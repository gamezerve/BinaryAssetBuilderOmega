# Reborn: convenient stable SDK command; delegate to the configured PowerShell 7 backend rather than the caller's legacy host.
[CmdletBinding()]
param([string]$ModName='Reborn',[switch]$ValidateOnly)
$ErrorActionPreference='Stop';$settings=@{}
if([string]::IsNullOrWhiteSpace($ModName)){$ModName='Reborn'}
foreach($line in Get-Content -LiteralPath (Join-Path $PSScriptRoot 'uprising-alpha.paths')){if($line -and -not $line.StartsWith('#')){$pair=$line.Split('=',2);$settings[$pair[0]]=$pair[1]}}
$arguments=@('-NoProfile','-File',$settings.Driver,'-ModName',$ModName);if($ValidateOnly){$arguments+='-ValidateOnly'}
& $settings.PowerShell7 @arguments
exit $LASTEXITCODE
