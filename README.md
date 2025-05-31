# C-Ollama Chat

A modern chat application with React frontend and .NET backend, featuring local LLM integration via Ollama and Auth0 authentication.

## ✨ Features

- 🔐 **Auth0 Authentication** - Secure login with Auth0 integration
- 🤖 **Local LLM Integration** - Chat with local models via Ollama
- ⚛️ **Modern React Frontend** - Built with Vite and Tailwind CSS
- 🔧 **ASP.NET Core Backend** - RESTful API with JWT authentication
- 🎨 **Beautiful UI** - Dark theme with custom color palette
- 🔒 **Environment Variables** - Secure credential management
- 📱 **Responsive Design** - Works on desktop and mobile

## 🚀 Quick Start

### Prerequisites

- .NET 9.0 SDK
- Node.js 18+ and npm
- Ollama running locally
- Auth0 account (free tier available)

### Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd c-ollama-chat
   ```

2. **Configure Auth0** (see [Auth0 Setup Guide](docs/AUTH0_SETUP.md))
   ```powershell
   $env:AUTH0_DOMAIN="your-domain.auth0.com"
   $env:AUTH0_CLIENT_ID="your-client-id"
   $env:AUTH0_AUDIENCE="https://thejackiegleason.com"
   $env:AUTH0_SCOPE="openid profile email add:documents site:admin"
   ```

3. **Run the application**
   ```powershell
   .\start.ps1
   ```

4. **Open your browser** → http://localhost:8019

## 📚 Documentation

- **[Auth0 Setup Guide](docs/AUTH0_SETUP.md)** - Complete Auth0 configuration
- **[Windows Environment Setup](docs/WINDOWS_ENV_SETUP.md)** - Windows environment variables setup
- **[Environment Setup](docs/ENVIRONMENT_SETUP.md)** - Alternative environment configuration
- **[Development Guide](docs/DEVELOPMENT.md)** - Development workflow and tips
- **[Development Guide](docs/DEVELOPMENT.md)** - Development workflows and project structure

## 🛠️ Technology Stack

### Frontend
- **React 18** with Vite
- **Tailwind CSS** for styling
- **Auth0 React SDK** for authentication

### Backend  
- **.NET 9** with ASP.NET Core
- **JWT Bearer Authentication**
- **Ollama Integration** for LLM access

### External Services
- **Auth0** for authentication
- **Ollama** for local LLM inference

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

