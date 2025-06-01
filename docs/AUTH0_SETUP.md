# Auth0 Setup Guide

This guide will help you configure Auth0 authentication for the C-Ollama Chat application.

## Prerequisites

1. An Auth0 account (free tier available at [auth0.com](https://auth0.com))
2. The application running locally

## Step 1: Create Auth0 Application

1. Log in to your [Auth0 Dashboard](https://manage.auth0.com/)
2. Navigate to **Applications** > **Applications**
3. Click **Create Application**
4. Choose:
   - **Name**: `C-Ollama Chat`
   - **Type**: `Single Page Web Applications`
5. Click **Create**

## Step 2: Configure Application Settings

In your Auth0 application settings, configure the following URLs. The application automatically uses `window.location.origin` for redirects, so you need to add the URLs for each environment you'll run the app in:

### Allowed Callback URLs
```
http://localhost:8019
https://yourdomain.com
```

### Allowed Logout URLs
```
http://localhost:8019
https://yourdomain.com
```

### Allowed Web Origins
```
http://localhost:8019
https://yourdomain.com
```

### Allowed Origins (CORS)
```
http://localhost:8019
https://yourdomain.com
```

## Step 3: Configure Auth0 Credentials (Environment Variables)

**Important**: Never commit Auth0 credentials to version control. Use environment variables instead.

### Option A: Using System Environment Variables (Recommended)

Set these environment variables in PowerShell:
```powershell
$env:AUTH0_DOMAIN="jackiergleason.auth0.com"
$env:AUTH0_CLIENT_ID="your-client-id"  
$env:AUTH0_AUDIENCE="https://thejackiegleason.com"
$env:AUTH0_SCOPE="openid profile email add:documents site:admin"
```

Or set them permanently in Windows:
```powershell
[Environment]::SetEnvironmentVariable("AUTH0_DOMAIN", "jackiergleason.auth0.com", "User")
[Environment]::SetEnvironmentVariable("AUTH0_CLIENT_ID", "your-client-id", "User")
[Environment]::SetEnvironmentVariable("AUTH0_AUDIENCE", "https://thejackiegleason.com", "User")
[Environment]::SetEnvironmentVariable("AUTH0_SCOPE", "openid profile email add:documents site:admin", "User")
```

Then run the application:
```powershell
.\run-with-env.ps1
```

### Option B: Using .env file (Fallback)

1. **Copy the template**:
   ```bash
   cp .env.template .env
   ```

2. **Edit .env file** with your Auth0 credentials:   ```env
   AUTH0_DOMAIN=jackiergleason.auth0.com
   AUTH0_CLIENT_ID=your-client-id
   AUTH0_CLIENT_SECRET=your-client-secret
   AUTH0_AUDIENCE=https://thejackiegleason.com
   AUTH0_SCOPE=openid profile email add:documents site:admin
   ```

3. **Run with .env file as fallback**:
   ```bash
   .\run-with-env.ps1 -UseEnvFile
   ```

### Option C: Using .NET User Secrets (Alternative)

```bash
cd src
dotnet user-secrets set "Auth0:Domain" "jackiergleason.auth0.com"
dotnet user-secrets set "Auth0:ClientId" "your-client-id"
dotnet user-secrets set "Auth0:ClientSecret" "your-client-secret"
dotnet user-secrets set "Auth0:Audience" "https://your-api-identifier"
```

### Where to find these values:

- **Domain**: Found in your Auth0 application settings (e.g., `dev-abc123.us.auth0.com`)
- **ClientId**: Found in your Auth0 application settings under "Basic Information"
- **ClientSecret**: Found in your Auth0 application settings under "Basic Information"
- **Audience**: You'll need to create an API in Auth0 (see next step)

## Step 4: Create Auth0 API (Optional but Recommended)

1. In Auth0 Dashboard, go to **Applications** > **APIs**
2. Click **Create API**
3. Set:
   - **Name**: `C-Ollama Chat API`
   - **Identifier**: `https://c-ollama-chat-api` (use this as your Audience)
   - **Signing Algorithm**: `RS256`
4. Click **Create**

## Step 5: Test the Integration

1. Restart your application: `dotnet run` in the `src` folder
2. Navigate to `http://localhost:8019`
3. Click the **Log In** button
4. You should be redirected to Auth0's Universal Login
5. After successful login, you should be redirected back to the application

## Troubleshooting

### Common Issues:

1. **Configuration Error Screen**: Check that your Auth0 domain and client ID are correctly set
2. **Login Redirect Issues**: Verify that your callback URLs match exactly in Auth0 settings
3. **CORS Errors**: Ensure `http://localhost:8019` is added to Allowed Origins in Auth0

### Checking Configuration:

You can verify your Auth0 configuration is being loaded by visiting:
```
http://localhost:8019/api/config/auth
```

This should return your Auth0 configuration (without sensitive data).

## Security Notes

- **Never commit real Auth0 credentials to version control**
- Use environment variables or user secrets for production
- The ClientSecret is not used in SPA applications but is included for potential server-side flows

## Production Deployment

For production, update the URLs in Auth0 settings to match your production domain:
- Replace `http://localhost:8019` with `https://yourdomain.com`
- Update the `appsettings.json` accordingly
