@where msbuild.exe 1>nul 2>nul
@IF ERRORLEVEL 1 GOTO :VsDevCmd
@GOTO :build

:VsDevCmd
:VsDevCmd14
@IF NOT EXIST "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\VsDevCmd.bat" @GOTO :VsDevCmd13
@CALL "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\VsDevCmd.bat"
@GOTO :build

:VsDevCmd13
@IF NOT EXIST "C:\Program Files (x86)\Microsoft Visual Studio 13.0\Common7\Tools\VsDevCmd.bat" @GOTO :VsDevCmd12
@CALL "C:\Program Files (x86)\Microsoft Visual Studio 13.0\Common7\Tools\VsDevCmd.bat"
@GOTO :build

:VsDevCmd12
@IF NOT EXIST "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\VsDevCmd.bat" @GOTO :build
@CALL "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\VsDevCmd.bat"
@GOTO :build

:build
@set p1=C:\Windows\SysNative\WindowsPowerShell\v1.0\PowerShell.exe
@set ps=C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe
@if EXIST %p1% @SET ps=%p1%
@%ps% -NoExit -ExecutionPolicy Unrestricted -Command ". '%~dp0build.ps1';%1;%2;%3;%4;Build-Help"
