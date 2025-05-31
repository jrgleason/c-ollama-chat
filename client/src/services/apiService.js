// Service for interacting with the C# backend API
export class ApiService {
  constructor(baseUrl = '') {
    this.baseUrl = baseUrl || window.location.origin;
    this.token = null;
    this.getAccessTokenSilently = null; // Will be set by Auth0
  }

  // Set the Auth0 token getter function
  setAuth0TokenGetter(getAccessTokenSilently) {
    this.getAccessTokenSilently = getAccessTokenSilently;
  }

  // Set the JWT token for authenticated requests (fallback)
  setAuthToken(token) {
    this.token = token;
  }
  // Get the default headers for API requests
  async getHeaders() {
    const headers = {
      'Content-Type': 'application/json',
    };
    
    try {
      // Try to get Auth0 token first
      if (this.getAccessTokenSilently) {
        const token = await this.getAccessTokenSilently();
        if (token) {
          headers['Authorization'] = `Bearer ${token}`;
          return headers;
        }
      }
      
      // Fallback to stored token
      if (this.token) {
        headers['Authorization'] = `Bearer ${this.token}`;
      }
    } catch (error) {
      console.warn('Failed to get access token:', error);
      // Continue without token for public endpoints
    }
    
    return headers;
  }

  // Handle API responses and errors
  async handleResponse(response) {
    if (!response.ok) {
      const errorData = await response.json().catch(() => null);
      throw new Error(errorData?.message || `API error: ${response.status}`);
    }
    
    return response.json();
  }  // Send a message to the chat API
  async sendChatMessage(message, modelName = 'llama3') {
    try {
      const response = await fetch(`${this.baseUrl}/api/chat/message`, {
        method: 'POST',
        headers: await this.getHeaders(),
        body: JSON.stringify({
          Text: message,
          Model: modelName
        })
      });
      
      return await this.handleResponse(response);
    } catch (error) {
      console.error('Chat API error:', error);
      throw error;
    }
  }  // Get available chat models
  async getAvailableModels() {
    try {
      const url = `${this.baseUrl}/api/config/models`;
      console.log('Fetching models from:', url);
      
      const response = await fetch(url, {
        headers: await this.getHeaders(),
        // Add cache control to avoid browser caching
        cache: 'no-cache'
      });
      
      console.log('Response status:', response.status);
      const contentType = response.headers.get('content-type');
      console.log('Response content type:', contentType);
      
      // Handle non-JSON responses
      if (!contentType || !contentType.includes('application/json')) {
        let text;
        try {
          text = await response.text();
          console.error('Received non-JSON response:', text.substring(0, 200) + '...');
        } catch (e) {
          console.error('Failed to read response text', e);
        }
        // Return default models instead of throwing
        console.warn('Using default models due to non-JSON response');
        return ['llama3', 'mistral'];
      }
      
      const data = await response.json();
      console.log('Models data:', data);
      return data;
    } catch (error) {
      console.error('Get models error:', error);
      // Return a default model list to prevent UI breakage
      return ['llama3', 'mistral'];
    }
  }

  // For development: get a test JWT token
  async getDevToken(username = 'user', isAdmin = false) {
    if (process.env.NODE_ENV !== 'development') {
      console.warn('Dev tokens should only be used in development mode');
      return null;
    }
    
    try {
      const response = await fetch(`${this.baseUrl}/api/dev/token`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          username,
          isAdmin
        })
      });
      
      const data = await this.handleResponse(response);
      if (data.token) {
        this.setAuthToken(data.token);
      }
      return data;
    } catch (error) {
      console.error('Dev token error:', error);
      throw error;
    }
  }
}

// Create and export a singleton instance
export const apiService = new ApiService();
