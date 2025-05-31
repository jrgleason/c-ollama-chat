import React from 'react';
import { useAuth0 } from '@auth0/auth0-react';

export function UserProfile() {
  const { user, isAuthenticated, isLoading } = useAuth0();

  if (isLoading) {
    return (
      <div className="flex items-center space-x-2">
        <div className="animate-pulse bg-gray-600 rounded-full h-8 w-8"></div>
        <div className="animate-pulse bg-gray-600 rounded h-4 w-20"></div>
      </div>
    );
  }

  if (!isAuthenticated || !user) {
    return null;
  }

  return (
    <div className="flex items-center space-x-3">
      {user.picture && (
        <img
          src={user.picture}
          alt={user.name || 'User'}
          className="h-8 w-8 rounded-full border-2 border-primary-400"
        />
      )}
      <div className="text-sm">
        <p className="font-medium text-gray-200">
          {user.name || user.email}
        </p>
        {user.name && user.email && (
          <p className="text-xs text-gray-400">{user.email}</p>
        )}
      </div>
    </div>
  );
}
