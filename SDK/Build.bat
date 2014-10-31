@echo off

rem /prod: Product
rem /ref: Reference help project
rem /build: Main project build

setlocal
set Product=
set RefProjOption=
set ProjectOption=
set DiagOption=
:options
if "%1"=="/prod" set Product=%2
if "%1"=="/ref" set RefProjOption=/p:RefProj=%2
if "%1"=="/build" set ProjectOption=/p:ProjName=%2
if "%1"=="/diag" set DiagOption=/Verbosity:diag
if not "%1"=="/diag" shift
shift
if not "%1"=="" goto options
if "%Product%"=="" goto help
if "%Product%"=="/p:Product=" goto help

SET LogFileOption=
if "%NOLOGFILE%"=="" SET LogFileOption=/l:FileLogger,Microsoft.Build.Engine;logfile=Logs\%Product%.log;encoding=utf-8

del /q Logs\Error.%Product%.log > nul
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe SDK.Help.proj /p:Product=%Product% %ProjectOption% %RefProjOption% %LogFileOption% /p:Configuration=Release %DiagOption%
if "%NOLOGFILE%"=="" if not %errorlevel%==0 move Logs\%Product%.log Logs\Error.%Product%.log > nul

goto exit
:help
echo Usage: Build.bat /prod Product [/ref Reference] [/build: Build] [/diag]
:exit
endlocal
