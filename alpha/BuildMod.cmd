@echo off
REM Reborn: build one named mod through the configured runtime; bypass applies only to this process, never machine policy.
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0BuildMod.ps1" -ModName "%~1"
exit /b %errorlevel%
