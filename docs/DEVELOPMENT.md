# Development Guide

This guide covers development workflows, project structure, and common tasks for the C-Ollama Chat application.

## Project Structure

```
c-ollama-chat/
├── README.md                          # Main project documentation
├── run-with-env.ps1                   # Environment variable runner script
├── .env.template                      # Environment variables template
├── docs/                              # Documentation
│   ├── AUTH0_SETUP.md                # Auth0 configuration guide
│   ├── ENVIRONMENT_SETUP.md          # Environment variables setup
│   └── DEVELOPMENT.md                # This file
├── client/                           # React frontend
│   ├── src/
│   │   ├── components/               # React components
│   │   │   ├── Auth0ProviderWithConfig.jsx
│   │   │   ├── AuthButton.jsx
│   │   │   ├── ChatInterface.jsx
│   │   │   ├── LoginButton.jsx
│   │   │   ├── LogoutButton.jsx
│   │   │   └── UserProfile.jsx
│   │   ├── hooks/                    # Custom React hooks
│   │   │   └── useApiAuth.js
│   │   ├── services/                 # API and auth services
│   │   │   ├── apiService.js
│   │   │   └── authService.js
│   │   └── main.jsx                  # React app entry point
│   ├── package.json                  # Node.js dependencies
│   └── vite.config.js                # Vite build configuration
└── src/                              # .NET backend
    ├── Program.cs                    # Application entry point
    ├── appsettings.json              # Configuration (with env var placeholders)
    ├── Controllers/                  # API controllers
    │   ├── ChatController.cs         # Chat API endpoints
    │   ├── ConfigController.cs       # Configuration endpoints
    │   └── SpaController.cs          # SPA serving
    ├── Services/                     # Business logic services
    │   └── OllamaService.cs          # LLM integration
    ├── Config/                       # Configuration classes
    │   ├── EnvironmentVariableExpander.cs
    │   └── SecurityConfig.cs
    └── Auth0/                        # Auth0 integration
        ├── Auth0Extensions.cs
        └── Auth0Options.cs
```

## Development Workflow

### 1. Initial Setup

```powershell
# Clone and setup
git clone <repository-url>
cd c-ollama-chat

# Set up environment variables (see ENVIRONMENT_SETUP.md)
$env:AUTH0_DOMAIN="your-domain.auth0.com"
$env:AUTH0_CLIENT_ID="your-client-id"
# ... other variables

# Install client dependencies
cd client
npm install
cd ..
```

### 2. Development Commands

#### Start Development Server
```powershell
# Start both backend and frontend
.\run-with-env.ps1
```

#### Build Frontend Only
```powershell
cd client
npm run build
```

#### Run Backend Only
```powershell
cd src
dotnet run
```

#### Run Tests
```powershell
cd src
dotnet test
```

### 3. Frontend Development

The React frontend uses:
- **Vite** for build tooling and hot reload
- **Tailwind CSS** for styling
- **Auth0 React SDK** for authentication
- **Custom hooks** for API integration

#### Key Components:
- `Auth0ProviderWithConfig` - Loads Auth0 config from backend
- `AuthButton` - Handles login/logout state
- `ChatInterface` - Main chat UI with authentication guards
- `UserProfile` - Displays user information

#### Adding New Components:
1. Create component in `client/src/components/`
2. Export from component file
3. Import where needed
4. Follow existing patterns for Auth0 integration

### 4. Backend Development

The .NET backend provides:
- **RESTful APIs** for chat functionality
- **Auth0 JWT validation** for authentication
- **Ollama integration** for LLM communication
- **SPA serving** for the React frontend

#### Key Controllers:
- `ChatController` - Chat message endpoints
- `ConfigController` - Configuration endpoints (Auth0, models)
- `SpaController` - Serves React SPA

#### Adding New Endpoints:
1. Create controller in `src/Controllers/`
2. Add `[ApiController]` and `[Route("api/[controller]")]` attributes
3. Use `[Authorize]` for protected endpoints
4. Follow existing patterns for error handling

### 5. Authentication Flow

1. **Frontend loads** → Fetches Auth0 config from `/api/config/auth`
2. **User clicks login** → Redirects to Auth0 Universal Login
3. **Auth0 redirects back** → Frontend receives tokens
4. **API calls** → Include JWT Bearer token in Authorization header
5. **Backend validates** → JWT signature and claims

### 6. Common Tasks

#### Update Auth0 Configuration
1. Modify environment variables
2. Restart application with `.\start.ps1`

#### Add New Chat Models
1. Update `ConfigController.GetAvailableModels()`
2. Ensure Ollama has the model installed

#### Modify UI Styling
1. Edit Tailwind classes in React components
2. Custom colors defined in `tailwind.config.js`

#### Debug API Issues
1. Check browser Network tab for API calls
2. Check console logs for errors
3. Verify Auth0 configuration at `/api/config/auth`

## Technology Stack

### Frontend
- **React 18** - UI framework
- **Vite** - Build tool and dev server
- **Tailwind CSS** - Utility-first CSS framework
- **Auth0 React SDK** - Authentication

### Backend
- **.NET 9** - Web framework
- **ASP.NET Core** - Web API
- **JWT Bearer Authentication** - Token validation
- **Auth0** - Identity provider integration

### External Services
- **Auth0** - Authentication and authorization
- **Ollama** - Local LLM inference

## Best Practices

### Security
- ✅ Environment variables for secrets
- ✅ JWT token validation
- ✅ CORS configuration
- ✅ No secrets in source code

### Code Organization
- ✅ Separation of concerns
- ✅ Consistent naming conventions
- ✅ Clear component hierarchy
- ✅ Service layer abstraction

### Development
- ✅ Hot reload for frontend changes
- ✅ Environment-specific configuration
- ✅ Clear error messages
- ✅ Comprehensive documentation
