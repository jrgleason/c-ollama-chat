import { describe, it, expect } from 'vitest'
import { render } from '@testing-library/react'
import { AuthButton } from '../components/AuthButton'
import { Auth0Provider } from '@auth0/auth0-react'

// Mock Auth0Provider for testing
const mockAuth0Provider = ({ children }) => {
  return (
    <Auth0Provider 
      domain="test-domain" 
      clientId="test-client-id"
      authorizationParams={{ redirectUri: window.location.origin }}
    >
      {children}
    </Auth0Provider>
  )
}

describe('AuthButton', () => {
  it('renders without crashing', () => {
    render(
      <mockAuth0Provider>
        <AuthButton />
      </mockAuth0Provider>
    )
    expect(document.body).toBeTruthy()
  })
})
