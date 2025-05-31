# Forward system environment variables and run the application
# Usage: .\run-with-env.ps1

param(
    [switch]$UseEnvFile = $false,
    [string]$EnvFile = ".env"
)

# Check for required Auth0 environment variables
$requiredVars = @("AUTH0_DOMAIN", "AUTH0_CLIENT_ID", "AUTH0_CLIENT_SECRET", "AUTH0_AUDIENCE")
$missingVars = @()

Write-Host "Checking Auth0 environment variables..." -ForegroundColor Green

foreach ($var in $requiredVars) {
    $value = [Environment]::GetEnvironmentVariable($var)
    if ([string]::IsNullOrEmpty($value)) {
        $missingVars += $var
    } else {
        # Show partial value for security (first 8 chars + ...)
        $displayValue = if ($value.Length -gt 8) { $value.Substring(0, 8) + "..." } else { $value }
        Write-Host "  ✓ $var = $displayValue" -ForegroundColor Green
    }
}

if ($missingVars.Count -gt 0) {
    if ($UseEnvFile -and (Test-Path $EnvFile)) {
        Write-Host "Loading missing variables from $EnvFile..." -ForegroundColor Yellow
        
        Get-Content $EnvFile | ForEach-Object {
            if ($_ -match "^\s*([^#][^=]*)\s*=\s*(.*)\s*$") {
                $name = $matches[1].Trim()
                $value = $matches[2].Trim()
                
                # Remove quotes if present
                if ($value -match '^"(.*)"$' -or $value -match "^'(.*)'$") {
                    $value = $matches[1]
                }
                
                if ($missingVars -contains $name) {
                    Write-Host "  Loading $name from .env file" -ForegroundColor Cyan
                    [Environment]::SetEnvironmentVariable($name, $value, "Process")
                }
            }
        }
    } else {
        Write-Warning "Missing environment variables: $($missingVars -join ', ')"
        Write-Host ""
        Write-Host "Please set them using one of these methods:" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Option 1 - Set system environment variables:" -ForegroundColor Cyan
        foreach ($var in $missingVars) {
            Write-Host "  `$env:$var = 'your-value-here'" -ForegroundColor Gray
        }
        Write-Host ""        Write-Host "Option 2 - Use .env file (copy .env.template to .env and configure):" -ForegroundColor Cyan
        Write-Host "  .\run-with-env.ps1 -UseEnvFile" -ForegroundColor Gray
        Write-Host ""
        Write-Host "For detailed setup instructions, see:" -ForegroundColor Yellow
        Write-Host "  docs\AUTH0_SETUP.md" -ForegroundColor Gray
        Write-Host "  docs\ENVIRONMENT_SETUP.md" -ForegroundColor Gray
        Write-Host ""
        exit 1
    }
}

# Start the application
Write-Host "Starting application..." -ForegroundColor Green
Set-Location "src"
dotnet run
