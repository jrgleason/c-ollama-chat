# C-Ollama Chat
Simple C# Chat app with React frontend using Ollama to allow for local LLM integration with OAuth authentication.

## Features

- RESTful API built with ASP.NET Core
- OAuth/JWT authentication and authorization
- Auth0 integration support
- Integration with Ollama for local LLM access
- Role-based access control with admin privileges
- Modern React frontend with Tailwind CSS
- Development tools for testing

## Prerequisites

- .NET 9.0 SDK or later
- Node.js 18+ and npm
- Ollama running locally or on a remote server
- OAuth provider (for production)

## Getting Started

### Development

The easiest way to run the application in development mode is to use the provided script:

```powershell
# Run both frontend and backend in development mode
.\dev.ps1
```

Alternatively, you can run the frontend and backend separately:

```powershell
# Run the frontend
cd client
npm install
npm run dev

# In another terminal, run the backend
cd src
dotnet run
```

### Production Build

To build the application for production:

```powershell
# Build everything for production
.\build.ps1
```

This will:
1. Build the React frontend and output to the wwwroot folder
2. Build and publish the ASP.NET Core application
3. Output the final build to the ./publish folder

### Configuration

1. Update the OAuth settings in `appsettings.json`
2. Configure Ollama connection settings

For development, you can use the built-in test token endpoint:

```
GET /dev/token?userId=testuser&isAdmin=false
```

## API Endpoints

### Authentication Required
- `GET /chat` - Check if chat service is available
- `POST /chat/message` - Send a message to the LLM
- `GET /chat/models` - List available models
- `GET /config/user` - Get user configuration
- `POST /config/user` - Update user configuration

### Admin Required
- `GET /chat/admin` - Admin-only endpoint

## Configuration

OAuth and Ollama settings can be configured in `appsettings.json`.

### Auth0 Configuration

The application supports Auth0 authentication. To use Auth0:

1. Create an Auth0 account at [auth0.com](https://auth0.com)
2. Create a new Auth0 application (Regular Web Application)
3. Configure the callback URL to `https://your-domain/callback`
4. Configure the logout URL to `https://your-domain/`
5. Update the Auth0 settings in `appsettings.json`:
```json
"Auth0": {
  "Domain": "your-tenant.auth0.com",
  "ClientId": "your-client-id",
  "ClientSecret": "your-client-secret",
  "Audience": "your-api-audience"
}
```
6. Uncomment the Auth0 authentication configuration in `Program.cs`

