import React from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import { LoginButton } from './LoginButton';
import { LogoutButton } from './LogoutButton';
import { UserProfile } from './UserProfile';

export function AuthButton() {
  const { isAuthenticated, isLoading } = useAuth0();  if (isLoading) {
    return (
      <div className="flex items-center gap-2">
        <div className="animate-pulse bg-surface rounded h-8 w-16"></div>
      </div>
    );
  }

  if (isAuthenticated) {
    return (
      <div className="flex items-center gap-4">
        <UserProfile />
        <LogoutButton />
      </div>
    );
  }

  return <LoginButton />;
}
