import React from 'react';
import { useAuth0 } from '@auth0/auth0-react';

export function LoginButton() {
  const { loginWithRedirect, isLoading } = useAuth0();

  return (
    <button
      onClick={() => loginWithRedirect()}
      disabled={isLoading}
      className="text-sm bg-primary-600 px-3 py-1 rounded-md hover:bg-primary-500 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
    >
      {isLoading ? 'Loading...' : 'Log In'}
    </button>
  );
}
