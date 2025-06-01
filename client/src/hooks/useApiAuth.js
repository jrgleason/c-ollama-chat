import { useEffect } from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import { apiService } from '../services/apiService';

// Hook to configure apiService with Auth0 token
export function useApiAuth() {
  const { getAccessTokenSilently, isAuthenticated } = useAuth0();

  useEffect(() => {
    if (isAuthenticated) {
      // Configure apiService to use Auth0 tokens
      apiService.setAuth0TokenGetter(getAccessTokenSilently);
    }
  }, [isAuthenticated, getAccessTokenSilently]);

  return { isAuthenticated };
}
