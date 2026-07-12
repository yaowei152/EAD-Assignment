@echo off
rem Starts the Arcane Vault API + Web app together and opens the browser
rem once the site is actually ready. Double-click this file to run it.
rem Close the two server windows it opens to stop the app.

cd /d "%~dp0"

echo Starting ArcaneVault.Api on http://localhost:5287 ...
start "ArcaneVault.Api" cmd /k dotnet run --project ArcaneVault.Api --urls http://localhost:5287

echo Starting ArcaneVault.Web on http://localhost:5068 ...
start "ArcaneVault.Web" cmd /k dotnet run --project ArcaneVault.Web --urls http://localhost:5068

echo.
echo Waiting for the site to come up (the first run builds the app and
echo can take a minute - keep this window open) ...

set tries=0
:waitloop
set /a tries+=1
if %tries% gtr 60 goto giveup
timeout /t 2 /nobreak >nul
netstat -an | findstr ":5068" | findstr "LISTENING" >nul
if errorlevel 1 goto waitloop

echo Site is up - opening your browser.
start http://localhost:5068
echo.
echo Done. Leave the two "ArcaneVault" windows open while you use the app;
echo close them when you're finished.
timeout /t 5 >nul
exit /b

:giveup
echo.
echo The site did not come up within 2 minutes. Check the two
echo "ArcaneVault" windows for a red error message.
pause
