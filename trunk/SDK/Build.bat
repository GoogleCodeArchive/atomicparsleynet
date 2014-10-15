@echo off

rem %1: Product
rem %2: Reference help project
rem %3: Main project

if "%1"=="" goto exit
SET RefProjOption=
SET ProjectOption=
if not "%2"=="" SET RefProjOption=/p:RefProj=%2
if not "%3"=="" SET ProjectOption=/p:ProjName=%3
SET LogFileOption=
if "%NOLOGFILE%"=="" SET LogFileOption=/l:FileLogger,Microsoft.Build.Engine;logfile=Logs\%1.log;encoding=utf-8

del /q Logs\Error.%1.log > nul
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe SDK.Help.proj /p:Product=%1 %ProjectOption% %RefProjOption% %LogFileOption% /p:Configuration=Release
rem /Verbosity:diag
if "%NOLOGFILE%"=="" if not %errorlevel%==0 move Logs\%1.log Logs\Error.%1.log > nul

:exit
