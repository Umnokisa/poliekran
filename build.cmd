@echo off
setlocal
cd /d "%~dp0"

set "VER=%~1"
if "%VER%"=="" for /f "usebackq delims=" %%v in (`powershell -NoProfile -Command "([xml](Get-Content src\Poliekran.csproj)).Project.PropertyGroup.Version | Where-Object { $_ } | Select-Object -First 1"`) do set "VER=%%v"
echo %VER%| findstr /r "^[0-9][0-9]*\.[0-9][0-9]*\.[0-9][0-9]*$" >nul || (echo Version must look like 1.2.3, got "%VER%" & exit /b 1)

dotnet build src -c Release -o out -p:Version=%VER% || exit /b 1

set MAKENSIS=makensis
where makensis >nul 2>nul || set MAKENSIS="%ProgramFiles(x86)%\NSIS\makensis.exe"

if not exist dist mkdir dist
%MAKENSIS% /V2 /DVER=%VER% installer\setup.nsi || exit /b 1

echo.
echo Done: dist\Poliekran-%VER%-setup.exe
