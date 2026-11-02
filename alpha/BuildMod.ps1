# Reborn: convenient stable SDK command; delegate to the configured PowerShell 7 backend rather than the caller's legacy host.
[CmdletBinding()]
param([string]$ModName='RebornTest',[switch]$ValidateOnly)
$ErrorActionPreference='Stop'
# Reborn: this retained maintenance helper uses the same bundled runtime and relative paths as Studio.
. (Join-Path $PSScriptRoot 'scripts/Ra3Ep1PortablePaths.ps1')
$settings=Read-RebornSdkPaths $PSScriptRoot
# Reborn: internal maintenance without an explicit selection must target the developer slot, never the user's mod.
if([string]::IsNullOrWhiteSpace($ModName)){$ModName='RebornTest'}
$arguments=@('-NoProfile','-File',$settings.Driver,'-ModName',$ModName);if($ValidateOnly){$arguments+='-ValidateOnly'}
& $settings.PowerShell7 @arguments
exit $LASTEXITCODE
