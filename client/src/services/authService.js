// Auth0 configuration service
export const authService = {
    // Fetch Auth0 configuration from the backend
    async getAuth0Config() {
        try {
            const response = await fetch('/api/config/auth');
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const config = await response.json();
            return {
                domain: config.domain,
                clientId: config.clientId,
                authorizationParams: {
                    redirect_uri: window.location.origin,
                    audience: config.audience,
                    scope: config.scope
                }
            };
        } catch (error) {
            console.error('Failed to fetch Auth0 config:', error);      // Return default config for development
            return {
                domain: 'your-auth0-domain.auth0.com',
                clientId: 'your-auth0-client-id',
                authorizationParams: {
                    redirect_uri: window.location.origin,
                    audience: 'https://your-api-identifier',
                    scope: 'openid profile email'
                }
            };
        }
    }
};
