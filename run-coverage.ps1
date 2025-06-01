# Script to run tests and generate coverage reports for both .NET and JS projects

Write-Host "Starting coverage generation..."
Write-Host "------------------------------------"

# --- JavaScript (client) coverage ---
Write-Host "[JS] Navigating to client directory..."
Push-Location -Path "./client"

Write-Host "[JS] Running npm run coverage..."
npm run coverage
if ($LASTEXITCODE -ne 0) {
    Write-Error "[JS] Coverage generation failed."
} else {
    Write-Host "[JS] JavaScript coverage report generated in client/coverage/"
}

Pop-Location
Write-Host "[JS] Returned to root directory."
Write-Host "------------------------------------"
Write-Host ""

# --- .NET (src) coverage ---
Write-Host "[.NET] Generating .NET coverage report (src)..."
# Assuming the script is run from the root of the c-ollama-chat directory
dotnet test "./src.Tests/src.Tests.csproj" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
if ($LASTEXITCODE -ne 0) {
    Write-Error "[.NET] Coverage generation failed."
} else {
    Write-Host "[.NET] .NET coverage report (opencover.xml) should be in src.Tests/TestResults/ somewhere"
}
Write-Host "------------------------------------"
Write-Host ""

Write-Host "All coverage report generation attempts finished."
