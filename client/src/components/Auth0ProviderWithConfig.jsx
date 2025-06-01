import React, {useEffect, useState} from 'react';
import {Auth0Provider} from '@auth0/auth0-react';
import {authService} from '../services/authService';

export function Auth0ProviderWithConfig({children}) {
    const [auth0Config, setAuth0Config] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const loadAuth0Config = async () => {
            try {
                const config = await authService.getAuth0Config();
                setAuth0Config(config);
            } catch (err) {
                console.error('Failed to load Auth0 config:', err);
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };

        loadAuth0Config();
    }, []);
    if (loading) {
        return (
            <div
                className="min-h-screen bg-gradient-to-b from-[--surface-dark] to-[--surface-bg] text-white flex items-center justify-center">
                <div className="text-center">
                    <div
                        className="animate-spin rounded-full h-12 w-12 border-b-2 border-[--brand-light] mx-auto"></div>
                    <p className="mt-4 text-[--text-secondary]">Loading authentication...</p>
                </div>
            </div>
        );
    }

    if (error || !auth0Config) {
        return (
            <div
                className="min-h-screen bg-gradient-to-b from-[--surface-dark] to-[--surface-bg] text-white flex items-center justify-center">
                <div className="text-center max-w-md">
                    <div className="text-red-400 text-6xl mb-4">⚠️</div>
                    <h2 className="text-xl font-bold mb-2">Authentication Configuration Error</h2>
                    <p className="text-[--text-secondary] mb-4">
                        Failed to load authentication configuration. Please check your Auth0 settings.
                    </p>
                    <button
                        onClick={() => window.location.reload()}
                        className="bg-[--brand] px-4 py-2 rounded-md hover:bg-[--brand-hover] transition-colors"
                    >
                        Retry
                    </button>
                </div>
            </div>
        );
    }

    return (
        <Auth0Provider
            domain={auth0Config.domain}
            clientId={auth0Config.clientId}
            authorizationParams={auth0Config.authorizationParams}
            useRefreshTokens={true}
            cacheLocation="localstorage"
        >
            {children}
        </Auth0Provider>
    );
}
