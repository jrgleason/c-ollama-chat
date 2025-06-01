# Forward system environment variables and run the application
# Usage: .\run-with-env.ps1

param(
    [switch]$UseEnvFile = $false,
    [string]$EnvFile = ".env"
)

# Check for required Auth0 environment variables (no client secret needed for SPA)
$requiredVars = @("AUTH0_DOMAIN", "AUTH0_CLIENT_ID", "AUTH0_AUDIENCE", "AUTH0_SCOPE")
$missingVars = @()

Write-Host "Checking Auth0 environment variables..." -ForegroundColor Green
Write-Host "DEBUG: Checking system environment variables first..." -ForegroundColor Magenta

foreach ($var in $requiredVars) {
    # Try system environment variables first, including user registry
    $userValue = [Environment]::GetEnvironmentVariable($var, "User")
    $machineValue = [Environment]::GetEnvironmentVariable($var, "Machine")
    $processValue = [Environment]::GetEnvironmentVariable($var, "Process")
    
    # Also try registry directly for user environment variables
    if ([string]::IsNullOrEmpty($userValue)) {
        try {
            $userValue = Get-ItemProperty -Path "HKCU:\Environment" -Name $var -ErrorAction SilentlyContinue | Select-Object -ExpandProperty $var
        } catch {
            # Ignore registry errors
        }
    }
    
    Write-Host "DEBUG: $var - User: '$userValue', Machine: '$machineValue', Process: '$processValue'" -ForegroundColor Magenta
    
    $value = $userValue
    if ([string]::IsNullOrEmpty($value)) {
        $value = $machineValue
    }
    if ([string]::IsNullOrEmpty($value)) {
        $value = $processValue
    }
    
    if ([string]::IsNullOrEmpty($value)) {
        $missingVars += $var
        Write-Host "  ✗ $var = (not found in system environment)" -ForegroundColor Red
    } else {
        # Show partial value for security (first 8 chars + ...)
        $displayValue = if ($value.Length -gt 8) { $value.Substring(0, 8) + "..." } else { $value }
        Write-Host "  ✓ $var = $displayValue" -ForegroundColor Green
        # Set in process environment for the app
        [Environment]::SetEnvironmentVariable($var, $value, "Process")
    }
}

if ($missingVars.Count -gt 0) {    if (Test-Path $EnvFile) {
        Write-Host "Loading missing variables from $EnvFile..." -ForegroundColor Yellow
        Write-Host "DEBUG: Reading .env file content..." -ForegroundColor Magenta
        
        Get-Content $EnvFile | ForEach-Object {
            $line = $_.Trim()
            Write-Host "DEBUG: Processing line: '$line'" -ForegroundColor Magenta
            
            if ($line -and !$line.StartsWith("#") -and $line.Contains("=")) {
                $parts = $line.Split("=", 2)
                if ($parts.Length -eq 2) {
                    $name = $parts[0].Trim()
                    $value = $parts[1].Trim()
                    
                    Write-Host "DEBUG: Found $name = '$value'" -ForegroundColor Magenta
                    
                    # Remove quotes if present
                    if ($value.StartsWith('"') -and $value.EndsWith('"')) {
                        $value = $value.Substring(1, $value.Length - 2)
                    }
                    if ($value.StartsWith("'") -and $value.EndsWith("'")) {
                        $value = $value.Substring(1, $value.Length - 2)
                    }
                    
                    # Skip placeholder values
                    if ($value -notlike "*your-*-here*" -and $value -ne "your-api-identifier") {
                        if ($missingVars -contains $name) {
                            Write-Host "  Loading $name from .env file (value: $($value.Substring(0, [Math]::Min(8, $value.Length)))...)" -ForegroundColor Cyan
                                                        [Environment]::SetEnvironmentVariable($name, $value, "Process")
                            # Remove from missing vars list
                            $missingVars = $missingVars | Where-Object { $_ -ne $name }
                        }
                    } else {
                        Write-Host "DEBUG: Skipping placeholder value for ${name}: '${value}'" -ForegroundColor Magenta
                    }
                }
            }
        }
    }
    
    if ($missingVars.Count -gt 0) {
        Write-Warning "Missing environment variables: $($missingVars -join ', ')"
        Write-Host ""
        Write-Host "Please set them using:" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Example:" -ForegroundColor Cyan
        foreach ($var in $missingVars) {
            Write-Host "`$env:$var = 'your-value-here'" -ForegroundColor Gray
        }
        Write-Host ""
        Write-Host "For detailed setup instructions, see:" -ForegroundColor Yellow
        Write-Host "  docs\AUTH0_SETUP.md" -ForegroundColor Gray
        Write-Host ""
        exit 1
    }
}

# Start the application
Write-Host "Starting application..." -ForegroundColor Green
Write-Host "DEBUG: Final environment variables being passed to app:" -ForegroundColor Magenta

foreach ($var in $requiredVars) {
    $finalValue = [Environment]::GetEnvironmentVariable($var, "Process")
    if (![string]::IsNullOrEmpty($finalValue)) {
        $displayValue = if ($finalValue.Length -gt 8) { $finalValue.Substring(0, 8) + "..." } else { $finalValue }
        Write-Host "DEBUG: $var = $displayValue" -ForegroundColor Magenta
    } else {
        Write-Host "DEBUG: $var = (NOT SET)" -ForegroundColor Red
    }
}

Set-Location "src"
dotnet run
