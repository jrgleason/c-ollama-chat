# Simple startup script for c-ollama-chat with Auth0
# Uses Windows environment variables (no .env file needed)
# Usage: .\start.ps1

Write-Host "Starting c-ollama-chat application..." -ForegroundColor Green

# Store the original location
$originalLocation = Get-Location

# Ensure we return to the original location on exit
trap {
    Set-Location $originalLocation
    break
}

# Check Auth0 environment variables
Write-Host "Checking Auth0 environment variables..." -ForegroundColor Yellow
$auth0Vars = @("AUTH0_DOMAIN", "AUTH0_CLIENT_ID", "AUTH0_AUDIENCE", "AUTH0_SCOPE")
$allSet = $true

foreach ($var in $auth0Vars) {
    $value = [Environment]::GetEnvironmentVariable($var, "User")
    if (![string]::IsNullOrEmpty($value)) {
        $displayValue = if ($value.Length -gt 20) { $value.Substring(0, 20) + "..." } else { $value }
        Write-Host "  `u2713 $var = $displayValue" -ForegroundColor Green
        # Set the variable in the current process so dotnet can access it
        [Environment]::SetEnvironmentVariable($var, $value, "Process")
    } else {
        Write-Host "  `u2717 $var = (not set)" -ForegroundColor Red
        $allSet = $false
    }
}

if (!$allSet) {
    Write-Host "Please set missing Auth0 environment variables in Windows Environment Variables" -ForegroundColor Red
    Write-Host "See docs\AUTH0_SETUP.md for details" -ForegroundColor Yellow
    Set-Location $originalLocation
    exit 1
}

Write-Host "Starting application on http://localhost:8019..." -ForegroundColor Cyan
Set-Location "src"

try {
    dotnet run
}
finally {
    # Always return to the original location
    Set-Location $originalLocation
    Write-Host "Returned to original directory: $originalLocation" -ForegroundColor Green
}
