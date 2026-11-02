param([string]$OutputDirectory,[string]$FxcPath)
$ErrorActionPreference='Stop'
# Reborn: build only the independent shader utility; do not rebuild/synchronize the asset compiler or modify user mods.
$repo=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
if(-not $OutputDirectory){$OutputDirectory=Join-Path $repo 'alpha\ShaderCompiler'}
$OutputDirectory=[IO.Path]::GetFullPath($OutputDirectory)
[IO.Directory]::CreateDirectory($OutputDirectory)|Out-Null
$csc=Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe'
$arguments=@('/nologo','/target:winexe','/platform:x86','/optimize+','/r:System.Windows.Forms.dll','/r:System.Drawing.dll','/r:System.Core.dll',('/out:'+(Join-Path $OutputDirectory 'RebornShaderCompiler.exe')),(Join-Path $PSScriptRoot 'ShaderCompiler.cs'),(Join-Path $PSScriptRoot 'EffectValidator.cs'))
& $csc @arguments
if($LASTEXITCODE -ne 0){throw 'Shader utility compilation failed.'}
# Reborn: optionally supply the user's existing legacy compiler; do not download or silently substitute a newer FXC.
if($FxcPath){$FxcPath=[IO.Path]::GetFullPath($FxcPath);if(-not [IO.File]::Exists($FxcPath)){throw 'The specified legacy FXC does not exist.'};Copy-Item -LiteralPath $FxcPath -Destination (Join-Path $OutputDirectory 'fxc.exe') -Force}
Write-Output ('Reborn: shader utility built: '+(Join-Path $OutputDirectory 'RebornShaderCompiler.exe'))
