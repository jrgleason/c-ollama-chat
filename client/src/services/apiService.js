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
        console.log('🔍 handleResponse called with status:', response.status);
        console.log('🔍 Response headers:', Object.fromEntries(response.headers.entries()));
        let responseText = ''; // Declare responseText in the outer scope

        if (!response.ok) {
            console.error('❌ Response not OK, status:', response.status);
            const errorData = await response.json().catch(() => null);
            console.error('❌ Error data:', errorData);
            throw new Error(errorData?.message || `API error: ${response.status}`);
        }

        try {
            responseText = await response.text(); // Assign value here
            console.log('📄 Raw response text:', responseText);

            const parsedData = JSON.parse(responseText);
            console.log('📝 Parsed JSON data:', parsedData);
            console.log('📝 Parsed JSON keys:', Object.keys(parsedData));

            return parsedData;
        } catch (parseError) {
            console.error('❌ JSON parse error:', parseError);
            console.error('❌ Response text was:', responseText); // Now accessible
            throw new Error('Failed to parse JSON response');
        }
    }    // Send a message to the chat API
    async sendChatMessage(message, modelName = 'llama3') {
        console.log('🚀 apiService.sendChatMessage called with:', {message, modelName});

        try {
            const url = `${this.baseUrl}/api/chat/message`;
            console.log('📡 Making request to:', url);

            const headers = await this.getHeaders();
            console.log('📋 Request headers:', headers);

            const requestBody = {
                Text: message,
                Model: modelName
            };
            console.log('📦 Request body:', requestBody);

            const response = await fetch(url, {
                method: 'POST',
                headers: headers,
                body: JSON.stringify(requestBody)
            });

            console.log('📨 Raw response status:', response.status);
            console.log('📨 Raw response headers:', Object.fromEntries(response.headers.entries()));

            const result = await this.handleResponse(response);
            console.log('✅ apiService.sendChatMessage result:', result);
            return result;
        } catch (error) {
            console.error('❌ apiService.sendChatMessage error:', error);
            throw error;
        }
    }    // Send a streaming message to the chat API
    sendStreamingChatMessage(message, modelName = 'llama3') {
        console.log('🚀 apiService.sendStreamingChatMessage called with:', {message, modelName});

        const self = this;
        
        return (async function* () {
            let response;
            let reader;
            
            try {
                const url = `${self.baseUrl}/api/chat/stream`;
                console.log('📡 Making streaming request to:', url);

                const headers = await self.getHeaders();
                console.log('📋 Request headers:', headers);

                const requestBody = {
                    Text: message,
                    Model: modelName
                };
                console.log('📦 Request body:', requestBody);

                response = await fetch(url, {
                    method: 'POST',
                    headers: headers,
                    body: JSON.stringify(requestBody)
                });

                console.log('📨 Raw streaming response status:', response.status);

                if (!response.ok) {
                    const errorText = await response.text();
                    console.error('❌ HTTP error response:', errorText);
                    throw new Error(`HTTP error! status: ${response.status}, response: ${errorText}`);
                }

                if (!response.body) {
                    throw new Error('Response body is null');
                }

                reader = response.body.getReader();
                const decoder = new TextDecoder();
                let buffer = '';

                while (true) {
                    const { done, value } = await reader.read();
                    
                    if (done) {
                        console.log('📨 Streaming completed');
                        break;
                    }

                    buffer += decoder.decode(value, { stream: true });
                    const lines = buffer.split('\n');
                    
                    // Keep the last incomplete line in buffer
                    buffer = lines.pop() || '';

                    for (const line of lines) {
                        if (line.startsWith('data: ')) {
                            try {
                                const jsonData = line.slice(6).trim(); // Remove 'data: ' prefix
                                if (jsonData) {
                                    const data = JSON.parse(jsonData);
                                    console.log('📦 Streaming data received:', data);
                                    yield data;
                                    
                                    if (data.Done || data.Error) {
                                        console.log('✅ Streaming done flag received');
                                        return;
                                    }
                                }
                            } catch (parseError) {
                                console.warn('⚠️ Failed to parse streaming data:', line, parseError);
                            }
                        }
                    }
                }
            } catch (error) {
                console.error('❌ apiService.sendStreamingChatMessage error:', error);
                // Yield an error response instead of throwing
                yield {
                    Response: `Error: ${error.message}`,
                    Model: modelName,
                    Done: true,
                    Error: true
                };
            } finally {
                if (reader) {
                    try {
                        reader.releaseLock();
                    } catch (e) {
                        console.warn('⚠️ Failed to release reader lock:', e);
                    }
                }
            }
        })();    }

    // Get available chat models
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
