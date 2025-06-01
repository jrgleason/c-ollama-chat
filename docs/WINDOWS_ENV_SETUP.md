# Windows Environment Variables Setup

This application uses Windows user environment variables instead of .env files for better security and persistence.

## Required Environment Variables

The following Auth0 environment variables must be set:

- `AUTH0_DOMAIN` - Your Auth0 domain (e.g., `jackiergleason.auth0.com`)
- `AUTH0_CLIENT_ID` - Your Auth0 application client ID
- `AUTH0_AUDIENCE` - Your Auth0 API identifier (e.g., `https://thejackiegleason.com`)
- `AUTH0_SCOPE` - Space-separated scopes (e.g., `openid profile email add:documents site:admin`)

## Setting Environment Variables

### Option 1: Using PowerShell (Recommended)

Run PowerShell as Administrator or regular user and execute:

```powershell
[Environment]::SetEnvironmentVariable("AUTH0_DOMAIN", "jackiergleason.auth0.com", "User")
[Environment]::SetEnvironmentVariable("AUTH0_CLIENT_ID", "your-actual-client-id", "User")
[Environment]::SetEnvironmentVariable("AUTH0_AUDIENCE", "https://thejackiegleason.com", "User")
[Environment]::SetEnvironmentVariable("AUTH0_SCOPE", "openid profile email add:documents site:admin", "User")
```

### Option 2: Using Windows System Properties

1. Press `Win + R`, type `sysdm.cpl`, and press Enter
2. Click the "Advanced" tab
3. Click "Environment Variables..."
4. Under "User variables", click "New..."
5. Add each variable:
   - Variable name: `AUTH0_DOMAIN`
   - Variable value: `jackiergleason.auth0.com`
6. Repeat for all four variables

### Option 3: Using Windows Settings

1. Open Windows Settings (`Win + I`)
2. Go to System → About → Advanced system settings
3. Click "Environment Variables..."
4. Follow the same steps as Option 2

## Verifying Setup

After setting the variables, **restart your terminal** and run:

```powershell
.\start.ps1
```

The script will check all required variables and show their status.

## Security Benefits

- ✅ **Persistent**: Variables survive system restarts
- ✅ **Secure**: No credentials in source code or .env files
- ✅ **User-specific**: Variables are isolated per Windows user
- ✅ **No accidental commits**: No risk of committing secrets to git

## Troubleshooting

### Variables Not Found
- Make sure you restarted your terminal after setting variables
- Check variables are set with: `[Environment]::GetEnvironmentVariable("AUTH0_DOMAIN", "User")`

### Permission Issues
- You may need to run PowerShell as Administrator for some commands
- User-level variables should work for most scenarios

### Application Won't Start
- Verify all four variables are set correctly
- Check the Auth0 dashboard for correct values
- Ensure no typos in variable names or values
