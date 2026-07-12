@echo off
rem Starts the Arcane Vault API + Web app together and opens the browser.
rem Double-click this file, or run it from a terminal. Close the two
rem console windows it opens to stop the servers.

cd /d "%~dp0"

echo Starting ArcaneVault.Api on http://localhost:5287 ...
start "ArcaneVault.Api" dotnet run --project ArcaneVault.Api --urls http://localhost:5287

echo Starting ArcaneVault.Web on http://localhost:5068 ...
start "ArcaneVault.Web" dotnet run --project ArcaneVault.Web --urls http://localhost:5068

echo Waiting for the site to come up ...
timeout /t 8 /nobreak >nul

start http://localhost:5068
echo Done. Two server windows are now running - close them to stop the app.
