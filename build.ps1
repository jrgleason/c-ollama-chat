# Build the application for production
$ErrorActionPreference = "Stop"

Write-Host "Building C-Ollama Chat for production..." -ForegroundColor Cyan

# First, build the React frontend
$frontendPath = Join-Path $PSScriptRoot "client"
Set-Location $frontendPath

Write-Host "Building React frontend..." -ForegroundColor Yellow
npm ci
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error installing frontend dependencies!" -ForegroundColor Red
    exit 1
}

npm run build
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error building frontend!" -ForegroundColor Red
    exit 1
}

# Build the backend
$backendPath = Join-Path $PSScriptRoot "src"
Set-Location $backendPath

Write-Host "Building ASP.NET Core backend..." -ForegroundColor Green
dotnet publish -c Release -o ../publish
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error building backend!" -ForegroundColor Red
    exit 1
}

Write-Host "Build completed successfully!" -ForegroundColor Cyan
Write-Host "The application has been published to: $PSScriptRoot\publish" -ForegroundColor White
