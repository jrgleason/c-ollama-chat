@echo off
REM Simple startup script for c-ollama-chat
echo Starting c-ollama-chat application...

REM Set environment variables if .env file exists
if exist .env (
    echo Loading environment variables from .env...
    for /f "usebackq tokens=1,2 delims==" %%a in (".env") do (
        if not "%%a"=="" if not "%%a"=="REM" if not "%%a"=="#" (
            set "%%a=%%b"
            echo   Loaded %%a
        )
    )
)

echo Starting application on http://localhost:8019...
cd src
dotnet run
