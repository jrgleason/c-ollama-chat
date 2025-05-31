# Environment Variables Setup

This document explains how to configure environment variables for the C-Ollama Chat application.

## Required Environment Variables

The application requires the following Auth0 environment variables:

- `AUTH0_DOMAIN` - Your Auth0 domain (e.g., `jackiergleason.auth0.com`)
- `AUTH0_CLIENT_ID` - Your Auth0 application client ID
- `AUTH0_AUDIENCE` - Your Auth0 API identifier (e.g., `https://thejackiegleason.com`)
- `AUTH0_SCOPE` - Space-separated list of scopes (e.g., `openid profile email add:documents site:admin`)

## Setup Methods

### Method 1: System Environment Variables (Recommended)

#### Temporary (Current Session Only)
```powershell
$env:AUTH0_DOMAIN="your-domain.auth0.com"
$env:AUTH0_CLIENT_ID="your-client-id"
$env:AUTH0_AUDIENCE="https://your-api-identifier"
$env:AUTH0_SCOPE="openid profile email add:documents site:admin"
```

#### Permanent (Persists Across Sessions)
```powershell
[Environment]::SetEnvironmentVariable("AUTH0_DOMAIN", "your-domain.auth0.com", "User")
[Environment]::SetEnvironmentVariable("AUTH0_CLIENT_ID", "your-client-id", "User")
[Environment]::SetEnvironmentVariable("AUTH0_AUDIENCE", "https://your-api-identifier", "User")
[Environment]::SetEnvironmentVariable("AUTH0_SCOPE", "openid profile email add:documents site:admin", "User")
```

### Method 2: .env File (Fallback)

1. Copy the template:
   ```powershell
   Copy-Item ".env.template" ".env"
   ```

2. Edit `.env` with your values:
   ```env
   AUTH0_DOMAIN=your-domain.auth0.com
   AUTH0_CLIENT_ID=your-client-id
   AUTH0_AUDIENCE=https://your-api-identifier
   AUTH0_SCOPE=openid profile email add:documents site:admin
   ```

3. Run with .env fallback:
   ```powershell
   .\run-with-env.ps1 -UseEnvFile
   ```

### Method 3: .NET User Secrets

```powershell
cd src
dotnet user-secrets set "Auth0:Domain" "your-domain.auth0.com"
dotnet user-secrets set "Auth0:ClientId" "your-client-id"
dotnet user-secrets set "Auth0:Audience" "https://your-api-identifier"
dotnet user-secrets set "Auth0:Scope" "openid profile email add:documents site:admin"
```

## Running the Application

After setting up your environment variables:

```powershell
.\start.ps1
```

The script will:
- ✅ Check for required environment variables
- ✅ Show which variables are set (with partial values for security)
- ✅ Provide helpful error messages if variables are missing
- ✅ Start the application with proper configuration

## Security Notes

- **Never commit `.env` files** to version control
- **Use system environment variables** for production deployments
- **The `.env` file is gitignored** to prevent accidental commits
- **Partial values are shown** in logs for security
