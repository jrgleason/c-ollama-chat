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

In your Auth0 application settings, configure the following:

### Allowed Callback URLs
```
http://localhost:5013
```

### Allowed Logout URLs
```
http://localhost:5013
```

### Allowed Web Origins
```
http://localhost:5013
```

### Allowed Origins (CORS)
```
http://localhost:5013
```

## Step 3: Update appsettings.json

Copy your Auth0 application details to `src/appsettings.json`:

```json
{
  "Auth0": {
    "Domain": "your-tenant.auth0.com",
    "ClientId": "your-client-id-from-auth0",
    "ClientSecret": "your-client-secret-from-auth0",
    "Audience": "https://your-api-identifier",
    "RedirectUri": "http://localhost:5013",
    "LogoutUri": "http://localhost:5013"
  }
}
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
2. Navigate to `http://localhost:5013`
3. Click the **Log In** button
4. You should be redirected to Auth0's Universal Login
5. After successful login, you should be redirected back to the application

## Troubleshooting

### Common Issues:

1. **Configuration Error Screen**: Check that your Auth0 domain and client ID are correctly set
2. **Login Redirect Issues**: Verify that your callback URLs match exactly in Auth0 settings
3. **CORS Errors**: Ensure `http://localhost:5013` is added to Allowed Origins in Auth0

### Checking Configuration:

You can verify your Auth0 configuration is being loaded by visiting:
```
http://localhost:5013/api/config/auth
```

This should return your Auth0 configuration (without sensitive data).

## Security Notes

- **Never commit real Auth0 credentials to version control**
- Use environment variables or user secrets for production
- The ClientSecret is not used in SPA applications but is included for potential server-side flows

## Production Deployment

For production, update the URLs in Auth0 settings to match your production domain:
- Replace `http://localhost:5013` with `https://yourdomain.com`
- Update the `appsettings.json` accordingly
