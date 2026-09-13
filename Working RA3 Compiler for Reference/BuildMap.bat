::=========================================================::
::===== ABOUT =============================================::
::=========================================================::
::                                                         ::
:: Extended BuildMap.bat for compiling Red Alert 3 maps    ::
::                                                         ::
:: Author: Bibber                                          ::
:: eMail: m.bibber@live.com                                 ::
:: Homepage: http://bibber.bplaced.net                     ::
::                                                         ::
::=========================================================::
::===== USAGE =============================================::
::=========================================================::
::                                                         ::
:: BuildMap.bat [MapName [/cbc] [/csc] [/cbm] [/ma] [/nf]] ::
::                                                         ::
:: /cbc: Clear build cache                                 ::
:: /csc: Clear session cache                               ::
:: /cbm: Clear built map                                   ::
:: /ma: Merge assets                                       ::
:: /nf: Do not fix assets                                  ::
::                                                         ::
::=========================================================::
::===== PROGRAM ===========================================::
::=========================================================::

@echo off
set map=
set par=
set udl=
set sdk=
set man=
set pl=
set cbc=false
set csc=false
set cbm=false
set ma=false
set nf=false

echo.
set map=%~1

if not defined map (
	set /P map="Map: "
	if not defined map goto map_fail
	set /P par="Parameters (optional): "
	set man=true
) else echo Map: %map%

if defined man goto man_start
set par=%~2
if defined par (set pl=Parameters:) else set pl=
	
:par_start
set par=%~2
	
if defined par (
	if /I "%par%" == "/cbc" (
		set cbc=true
		set pl=%pl% /cbc
	)
	
	if /I "%par%" == "/csc" (
		set csc=true
		set pl=%pl% /csc
	)
	
	if /I "%par%" == "/cbm" (
		set cbm=true
		set pl=%pl% /cbm
	)
	
	if /I "%par%" == "/ma" (
		set ma=true
		set pl=%pl% /ma
	)
	
	if /I "%par%" == "/nf" (
		set nf=true
		set pl=%pl% /nf
	)
	
	shift
) else goto par_end
goto par_start
:par_end
	
if defined pl echo %pl%
goto man_end

:man_start
if defined par (
	for /f "tokens=1-5 delims= " %%A in ("%par%") do (
		if /I "%%A" == "/cbc" set cbc=true
		if /I "%%A" == "/csc" set csc=true
		if /I "%%A" == "/cbm" set cbm=true
		if /I "%%A" == "/ma" set ma=true
		if /I "%%A" == "/nf" set nf=true
		
		if /I "%%B" == "/cbc" set cbc=true
		if /I "%%B" == "/csc" set csc=true
		if /I "%%B" == "/cbm" set cbm=true
		if /I "%%B" == "/ma" set ma=true
		if /I "%%B" == "/nf" set nf=true
		
		if /I "%%C" == "/cbc" set cbc=true
		if /I "%%C" == "/csc" set csc=true
		if /I "%%C" == "/cbm" set cbm=true
		if /I "%%C" == "/ma" set ma=true
		if /I "%%C" == "/nf" set nf=true
		
		if /I "%%D" == "/cbc" set cbc=true
		if /I "%%D" == "/csc" set csc=true
		if /I "%%D" == "/cbm" set cbm=true
		if /I "%%D" == "/ma" set ma=true
		if /I "%%D" == "/nf" set nf=true
		
		if /I "%%E" == "/cbc" set cbc=true
		if /I "%%E" == "/csc" set csc=true
		if /I "%%E" == "/cbm" set cbm=true
		if /I "%%E" == "/ma" set ma=true
		if /I "%%E" == "/nf" set nf=true
	)
)
:man_end

if not defined udl for /F "tokens=2* delims=	 " %%A IN ('REG QUERY "HKLM\Software\Electronic Arts\Electronic Arts\Red Alert 3" /v "UserDataLeafName" 2^>nul') do call set udl="%%B"
if not defined udl for /F "tokens=2* delims=	 " %%A IN ('REG QUERY "HKLM\Software\Wow6432Node\Electronic Arts\Electronic Arts\Red Alert 3" /v "UserDataLeafName" 2^>nul') do call set udl="%%B"
if not defined udl set udl="Red Alert 3"
for %%A in (%udl%) do set udl=%%~A

if not defined sdk for /F "tokens=2* delims=	 " %%A IN ('REG QUERY "HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall\{F6A3F605-7B10-4939-8D3D-4594332C1649}" /v "InstallLocation" 2^>nul') do call set sdk="%%B\"
if not defined sdk for /F "tokens=2* delims=	 " %%A IN ('REG QUERY "HKLM\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{F6A3F605-7B10-4939-8D3D-4594332C1649}" /v "InstallLocation" 2^>nul') do call set sdk="%%B\"
if not defined sdk set sdk="%~dp0"
for %%A in (%sdk%) do set sdk=%%~A

set exc=buildmap_exclude.txt
cd /D "%sdk%"

if exist "%appdata%\%udl%\maps\%map%" (
	if /I %cbc%==true (
		echo.
		echo Clearing build cache
		if exist "%sdk%builtmods\cache" rd "%sdk%builtmods\cache" /S /Q
	)
	
	if /I %csc%==true (
		echo.
		echo Clearing session cache
		if exist "%sdk%builtmods\builtmods" rd "%sdk%builtmods\builtmods" /S /Q
		for /R "%sdk%builtmods\mods\maps\%map%" %%I in ("*.asset") do del "%%I" /F /Q
		if exist "%sdk%builtmods\binaryassetbuilder.sessioncache.xml" del "%sdk%builtmods\binaryassetbuilder.sessioncache.xml" /F /Q
		if exist "%sdk%builtmods\binaryassetbuilder.sessioncache.xml.old" del "%sdk%builtmods\binaryassetbuilder.sessioncache.xml.old" /F /Q
		if exist "%sdk%builtmods\binaryassetbuilder.sessioncache.xml.deflate" del "%sdk%builtmods\binaryassetbuilder.sessioncache.xml.deflate" /F /Q
		if exist "%sdk%builtmods\binaryassetbuilder.sessioncache.xml.deflate.old" del "%sdk%builtmods\binaryassetbuilder.sessioncache.xml.deflate.old" /F /Q
		if exist "%sdk%builtmods\stringhashes.xml" del "%sdk%builtmods\stringhashes.xml" /F /Q
	)

	if /I %cbm%==true (
		echo.
		echo Clearing built map
		for /R "%sdk%builtmods\mods\maps\%map%" %%I in ("*.*") do if not "%%~xI" == ".asset" del "%%I" /F /Q
	)
	
	echo.
	echo Building map
	if exist "%appdata%\%udl%\maps\%map%\map.bin" del "%appdata%\%udl%\maps\%map%\map.bin" /F /Q
	if exist "%appdata%\%udl%\maps\%map%\map.imp" del "%appdata%\%udl%\maps\%map%\map.imp" /F /Q
	if exist "%appdata%\%udl%\maps\%map%\map.manifest" del "%appdata%\%udl%\maps\%map%\map.manifest" /F /Q
	if exist "%appdata%\%udl%\maps\%map%\map.relo" del "%appdata%\%udl%\maps\%map%\map.relo" /F /Q
	if exist "%appdata%\%udl%\maps\%map%\map.version" del "%appdata%\%udl%\maps\%map%\map.version" /F /Q
	if exist "%appdata%\%udl%\maps\%map%\map" rd "%appdata%\%udl%\maps\%map%\map" /S /Q
	xcopy "%appdata%\%udl%\maps\%map%\*.*" "%sdk%mods\maps\%map%" /Q /Y /I /R /S
	"%sdk%tools\binaryassetbuilder.exe" "%sdk%mods\maps\%map%\map.xml" /od:"%sdk%builtmods" /iod:"%sdk%builtmods" /ls:true /osh:false /pc:true /slowclean:true /ss:true /art:".;.\mods\maps\%map%\Art;.\Mods;.\Art" /audio:".;.\mods\maps\%map%\Audio;.\Mods;.\Audio" /data:".;.\mods\maps\%map%\Data;.\Mods;.\SageXml"

	if exist "%sdk%builtmods\mods\maps\%map%\map.manifest" (
		if /I %ma%==true if exist "%sdk%tools\assetmerger.exe" (
			echo.
			echo Merging assets
			"%sdk%tools\assetmerger.exe" "%sdk%builtmods\mods\maps\%map%\map.manifest" "%sdk%mods\maps\%map%\assets"
			if /I %nf%==true if exist "%sdk%tools\streamsorter.exe" "%sdk%tools\streamsorter.exe" "%sdk%builtmods\mods\maps\%map%\map.manifest"
		)

		if /I %nf%==false if exist "%sdk%tools\hashfix.exe" if exist "%sdk%tools\mapassetresolver.exe" if exist "%sdk%tools\referencefix.exe" (
			echo.
			echo Fixing assets
			"%sdk%tools\hashfix.exe" "%sdk%builtmods\mods\maps\%map%\map.manifest" "%sdk%builtmods\sagexml\worldbuilder.manifest"
			copy "%sdk%builtmods\sagexml\worldbuilder.manifest" "%sdk%builtmods\mods\maps\%map%\worldbuilder.manifest" /Y
			"%sdk%tools\mapassetresolver.exe" -m "%sdk%builtmods\mods\maps\%map%\map.manifest" -s "map"
			if exist "%sdk%builtmods\mods\maps\%map%\worldbuilder.manifest" del "%sdk%builtmods\mods\maps\%map%\worldbuilder.manifest" /F /Q
			"%sdk%tools\referencefix.exe" "%sdk%builtmods\mods\maps\%map%\map.manifest" /a "2" "worldbuilder.11.manifest" /d "worldbuilder.manifest"
		)

		echo .asset>%exc%
		xcopy "%sdk%builtmods\mods\maps\%map%\*.*" "%appdata%\%udl%\maps\%map%" /Q /Y /I /R /S /EXCLUDE:%exc%
		if exist "%exc%" del "%exc%" /F /Q
		echo.
		echo Map successfully compiled
	) else (
		echo.
		echo Map not compiled
	)

	if exist "%sdk%mods\maps" rd "%sdk%mods\maps" /S /Q
) else (
	:map_fail
	echo.
	echo Map not found
)

echo.
pause
echo.

::=========================================================::
::=========================================================::
::=========================================================::