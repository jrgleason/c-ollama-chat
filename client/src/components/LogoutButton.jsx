import React from 'react';
import {useAuth0} from '@auth0/auth0-react';

export function LogoutButton() {
    const {logout, isLoading} = useAuth0();

    return (<button
            onClick={() => logout({
                logoutParams: {
                    returnTo: window.location.origin
                }
            })}
            disabled={isLoading}
            className="text-sm bg-danger text-primary px-3 py-1 rounded-md hover:bg-danger-hover transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
        >
            {isLoading ? 'Loading...' : 'Log Out'}
        </button>
    );
}
