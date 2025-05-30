# Run frontend and backend in development mode
$ErrorActionPreference = "Stop"

Write-Host "Starting C-Ollama Chat development environment..." -ForegroundColor Cyan

# Start the React frontend in a new PowerShell window
$frontendPath = Join-Path $PSScriptRoot "client"
Start-Process powershell -ArgumentList "-Command", "Set-Location '$frontendPath'; npm run dev"

# Wait a moment to ensure the frontend starts properly
Start-Sleep -Seconds 2

# Start the backend
$backendPath = Join-Path $PSScriptRoot "src"
Set-Location $backendPath
Write-Host "Starting ASP.NET Core backend..." -ForegroundColor Green
dotnet run
