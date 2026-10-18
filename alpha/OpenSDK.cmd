@echo off
REM Reborn: open the one local SDK in its own working directory; no game launch or installation change.
pushd "%~dp0"
start "" "%~dp0EALAModStudio.exe"
popd
