import React, { useState, useEffect, useRef } from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import { apiService } from '../services/apiService';
import { useApiAuth } from '../hooks/useApiAuth';

export function ChatInterface() {
  const [messages, setMessages] = useState([]);
  const [inputMessage, setInputMessage] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [models, setModels] = useState([]);
  const [selectedModel, setSelectedModel] = useState('llama3');
  const [debugInfo, setDebugInfo] = useState('Debug info will appear here...');
  const [showDebug, setShowDebug] = useState(true);
  
  const messagesEndRef = useRef(null);
  const { isAuthenticated, isLoading: auth0Loading, getAccessTokenSilently, user } = useAuth0();
  useApiAuth(); // Configure apiService with Auth0
  // Helper function to log both to console and in-app debug
  const debugLog = (message, data = null) => {
    console.log(message, data);
    const timestamp = new Date().toLocaleTimeString();
    setDebugInfo(prev => `[${timestamp}] ${message}${data ? ': ' + JSON.stringify(data, null, 2) : ''}\n${prev}`);
  };

  // Initialize debug on mount
  useEffect(() => {
    debugLog('🚀 ChatInterface component mounted');
    debugLog('🔍 Initial auth state', { isAuthenticated, auth0Loading });
  }, []);

  // Scroll to bottom of messages
  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);  // Load available models on component mount
  useEffect(() => {
    debugLog('🔄 useEffect for models triggered', { auth0Loading, isAuthenticated });
    
    const fetchModels = async () => {
      try {
        debugLog('📡 Fetching models...');
        // Get available models (public endpoint)
        const modelData = await apiService.getAvailableModels();
        debugLog('📦 Model data received', modelData);
        
        if (Array.isArray(modelData)) {
          setModels(modelData);
          if (modelData.length > 0) {
            setSelectedModel(modelData[0]);
            debugLog('✅ Models loaded successfully', { count: modelData.length, first: modelData[0] });
          }
        }
      } catch (error) {
        debugLog('❌ Failed to load models', error.message);
        console.error('Failed to load models:', error);
      }
    };

    // Only fetch models after Auth0 has finished loading
    if (!auth0Loading) {
      debugLog('🚀 Auth0 loading complete, fetching models...');
      fetchModels();
    }
  }, [auth0Loading]);  const handleSendMessage = async (e) => {
    e.preventDefault();
    if (!inputMessage.trim() || isLoading) return;
    
    debugLog('🎯 handleSendMessage triggered');
    debugLog('💬 Input message', inputMessage);
    debugLog('🤖 Selected model', selectedModel);
    debugLog('🔐 Is authenticated', isAuthenticated);
    debugLog('👤 User info', user);
    
    // Test if we can get access token
    try {
      const token = await getAccessTokenSilently();
      debugLog('🎫 Got access token', { length: token?.length, preview: token?.substring(0, 20) + '...' });
    } catch (tokenError) {
      debugLog('❌ Failed to get access token', tokenError.message);
    }
    
    const userMessage = inputMessage;
    setMessages(prev => [...prev, { role: 'user', content: userMessage }]);
    setInputMessage('');
    setIsLoading(true);
      try {
      debugLog('📞 Calling apiService.sendChatMessage...');
      const response = await apiService.sendChatMessage(userMessage, selectedModel);
      debugLog('🎉 Chat response received', response);
      debugLog('🔑 Response keys', Object.keys(response || {}));
      debugLog('🔍 Response type', typeof response);
      debugLog('🔍 Response is array', Array.isArray(response));
      debugLog('🔍 Response stringified', JSON.stringify(response, null, 2));
      
      // Handle both potential property names (camelCase from backend)
      const responseText = response?.response || response?.Response || response?.text || response?.content || 'No response content';
      debugLog('📝 Using response text', responseText);
      debugLog('📝 Response text type', typeof responseText);
      
      if (!responseText || responseText === 'No response content') {
        debugLog('⚠️ Empty or missing response text');
        debugLog('🔍 Full response object detailed', {
          response,
          responseKeys: Object.keys(response || {}),
          responseValues: Object.values(response || {}),
          stringified: JSON.stringify(response, null, 2)
        });
      }
      
      setMessages(prev => [...prev, { role: 'assistant', content: responseText }]);
      debugLog('✅ Message added to state', { responseText: responseText.substring(0, 100) + '...' });
    } catch (error) {
      debugLog('💥 Error in handleSendMessage', error.message);
      console.error('💥 Error in handleSendMessage:', error);
      setMessages(prev => [...prev, { 
        role: 'system', 
        content: `Error: ${error.message || 'Failed to get response'}`
      }]);
    } finally {
      setIsLoading(false);
      debugLog('🏁 handleSendMessage completed');
    }
  };  return (
    <div className="flex flex-col h-full">      {auth0Loading ? (        <div className="flex-1 flex items-center justify-center">
          <div className="text-center">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-brand mx-auto"></div>
            <p className="mt-4 text-muted">Loading authentication...</p>
          </div>
        </div>
      ) : !isAuthenticated ? (
        <div className="flex-1 flex items-center justify-center">
          <div className="text-center max-w-md">
            <div className="text-brand text-6xl mb-4">🔒</div>
            <h2 className="text-xl font-bold text-primary mb-2">Authentication Required</h2>
            <p className="text-muted mb-4">
              Please log in to start chatting with your local LLM.
            </p>
            <p className="text-sm text-lighter">
              Your conversations are private and secure.
            </p>
          </div>
        </div>
      ) : (<>
          <div className="flex-1 overflow-y-auto p-4 space-y-4">            {messages.length === 0 ? (
              <div className="text-center py-8 text-muted">
                <p className="text-xl mb-2">Welcome to C-Ollama Chat</p>
                <p>Start a conversation with your local LLM powered by Ollama</p>
              </div>
            ) : (
              messages.map((msg, index) => (
                <div 
                  key={index} 
                  className={`flex mb-4 ${
                    msg.role === 'user' ? 'justify-end' : 
                    msg.role === 'system' ? 'justify-center' : 'justify-start'
                  }`}
                >                  <div 
                    className={`max-w-3xl p-4 rounded-2xl shadow-lg ${
                      msg.role === 'user' 
                        ? 'bg-brand text-white rounded-br-md' 
                        : msg.role === 'system'
                          ? 'bg-danger text-white rounded-md'
                          : 'bg-surface text-primary rounded-bl-md border-l-4 border-brand'
                    }`}
                  >
                    {msg.role !== 'system' && (
                      <div className={`text-xs mb-2 font-medium ${
                        msg.role === 'user' ? 'text-brand-light' : 'text-muted'
                      }`}>
                        {msg.role === 'user' ? 'You' : 'Assistant'}
                      </div>
                    )}
                    <div className="whitespace-pre-wrap break-words leading-relaxed">
                      {msg.content}
                    </div>
                  </div>
                </div>
              ))
            )}            {isLoading && (
              <div className="flex justify-start mb-4">                <div className="bg-surface p-4 rounded-2xl rounded-bl-md shadow-lg border-l-4 border-brand max-w-3xl">
                  <div className="text-xs mb-2 font-medium text-muted">
                    Assistant
                  </div>
                  <div className="flex items-center space-x-2">
                    <div className="w-2 h-2 bg-brand rounded-full animate-pulse"></div>
                    <div className="w-2 h-2 bg-brand rounded-full animate-pulse" style={{ animationDelay: '0.2s' }}></div>
                    <div className="w-2 h-2 bg-brand rounded-full animate-pulse" style={{ animationDelay: '0.4s' }}></div>
                    <span className="text-muted text-sm ml-2">Thinking...</span>
                  </div>
                </div>
              </div>
            )}
            <div ref={messagesEndRef} />
          </div>          <form onSubmit={handleSendMessage} className="p-4 border-t border-surface bg-surface-bg">
            <div className="flex gap-3">
              <select
                className="bg-surface text-primary rounded-lg px-3 py-2 border border-surface focus:border-brand focus:outline-none min-w-32"
                value={selectedModel}
                onChange={(e) => setSelectedModel(e.target.value)}
                disabled={isLoading}
              >
                {models.length > 0 ? 
                  models.map(model => (
                    <option key={model} value={model} className="bg-surface">{model}</option>
                  )) : 
                  <option value="llama3" className="bg-surface">llama3</option>
                }
              </select>
              
              <input
                type="text"
                className="flex-1 bg-surface text-primary rounded-lg px-4 py-2 border border-surface placeholder-muted focus:border-brand focus:outline-none"
                placeholder="Type a message..."
                value={inputMessage}
                onChange={(e) => setInputMessage(e.target.value)}
                disabled={isLoading}
              />
              
              <button
                type="submit"
                className="bg-brand hover:bg-brand-hover disabled:bg-surface rounded-lg px-6 py-2 text-primary font-medium transition-colors"
                disabled={!inputMessage.trim() || isLoading}
              >
                Send
              </button>
            </div>
          </form></>
      )}      {/* Debug Panel */}
      {showDebug && (
        <div className="fixed bottom-4 right-4 w-96 max-h-64 bg-surface-dark border border-surface rounded-lg shadow-2xl z-50">          <div className="flex justify-between items-center p-3 border-b border-surface">
            <h3 className="text-sm font-bold text-warning">🐛 Debug Log</h3>
            <button 
              onClick={() => setShowDebug(false)}
              className="text-muted hover:text-primary text-lg leading-none"
            >
              ✕
            </button>
          </div>
          <div className="p-3">            <pre className="text-xs text-secondary overflow-y-auto max-h-40 whitespace-pre-wrap">
              {debugInfo}
            </pre>
            <button 
              onClick={() => setDebugInfo('Debug cleared...')}
              className="mt-3 text-xs bg-surface hover:bg-surface-light px-3 py-1 rounded text-secondary transition-colors"
            >
              Clear
            </button>
          </div>
        </div>
      )}
      
      {/* Debug Toggle Button */}
      {!showDebug && (
        <button 
          onClick={() => setShowDebug(true)}
          className="fixed bottom-4 right-4 bg-warning hover:bg-warning-hover text-primary p-3 rounded-full text-lg z-50 shadow-lg transition-colors"
          title="Show Debug"
        >
          🐛
        </button>
      )}
    </div>
  );
}
