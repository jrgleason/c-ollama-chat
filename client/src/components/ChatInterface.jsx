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
  
  const messagesEndRef = useRef(null);
  const { isAuthenticated, isLoading: auth0Loading } = useAuth0();
  useApiAuth(); // Configure apiService with Auth0

  // Scroll to bottom of messages
  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);
  // Load available models on component mount
  useEffect(() => {
    const fetchModels = async () => {
      try {
        // Get available models (public endpoint)
        const modelData = await apiService.getAvailableModels();
        if (Array.isArray(modelData)) {
          setModels(modelData);
          if (modelData.length > 0) {
            setSelectedModel(modelData[0]);
          }
        }
      } catch (error) {
        console.error('Failed to load models:', error);
      }
    };

    // Only fetch models after Auth0 has finished loading
    if (!auth0Loading) {
      fetchModels();
    }
  }, [auth0Loading]);

  const handleSendMessage = async (e) => {
    e.preventDefault();
    if (!inputMessage.trim() || isLoading) return;
    
    const userMessage = inputMessage;
    setMessages(prev => [...prev, { role: 'user', content: userMessage }]);
    setInputMessage('');
    setIsLoading(true);

    try {
      const response = await apiService.sendChatMessage(userMessage, selectedModel);
      setMessages(prev => [...prev, { role: 'assistant', content: response.message }]);
    } catch (error) {
      setMessages(prev => [...prev, { 
        role: 'system', 
        content: `Error: ${error.message || 'Failed to get response'}`
      }]);
    } finally {
      setIsLoading(false);
    }
  };
  return (
    <div className="flex flex-col h-full">
      {auth0Loading ? (
        <div className="flex-1 flex items-center justify-center">
          <div className="text-center">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-400 mx-auto"></div>
            <p className="mt-4 text-gray-300">Loading authentication...</p>
          </div>
        </div>
      ) : !isAuthenticated ? (
        <div className="flex-1 flex items-center justify-center">
          <div className="text-center max-w-md">
            <div className="text-primary-400 text-6xl mb-4">🔒</div>
            <h2 className="text-xl font-bold mb-2">Authentication Required</h2>
            <p className="text-gray-300 mb-4">
              Please log in to start chatting with your local LLM.
            </p>
            <p className="text-sm text-gray-400">
              Your conversations are private and secure.
            </p>
          </div>
        </div>
      ) : (
        <>
          <div className="flex-1 overflow-y-auto p-4 space-y-4">
            {messages.length === 0 ? (
              <div className="text-center py-8 text-gray-400">
                <p className="text-xl mb-2">Welcome to C-Ollama Chat</p>
                <p>Start a conversation with your local LLM powered by Ollama</p>
              </div>
            ) : (
              messages.map((msg, index) => (
                <div 
                  key={index} 
                  className={`p-3 rounded-lg max-w-[85%] ${
                    msg.role === 'user' 
                      ? 'bg-primary-600 ml-auto text-white' 
                      : msg.role === 'system'
                        ? 'bg-accent-500 text-white mx-auto'
                        : 'bg-secondary-500 mr-auto text-white'
                  }`}
                >
                  {msg.content}
                </div>
              ))
            )}
            {isLoading && (
              <div className="bg-gray-700 p-3 rounded-lg mr-auto flex items-center space-x-2">
                <div className="w-2 h-2 bg-gray-400 rounded-full animate-pulse"></div>
                <div className="w-2 h-2 bg-gray-400 rounded-full animate-pulse" style={{ animationDelay: '0.2s' }}></div>
                <div className="w-2 h-2 bg-gray-400 rounded-full animate-pulse" style={{ animationDelay: '0.4s' }}></div>
              </div>
            )}
            <div ref={messagesEndRef} />
          </div>
          
          <form onSubmit={handleSendMessage} className="p-4 border-t border-gray-700">
            <div className="flex space-x-2">
              <select
                className="bg-gray-700 text-white rounded-lg px-3 py-2"
                value={selectedModel}
                onChange={(e) => setSelectedModel(e.target.value)}
                disabled={isLoading}
              >
                {models.length > 0 ? 
                  models.map(model => (
                    <option key={model} value={model}>{model}</option>
                  )) : 
                  <option value="llama3">llama3</option>
                }
              </select>
              
              <input
                type="text"
                className="flex-1 bg-gray-700 rounded-lg px-4 py-2 text-white"
                placeholder="Type a message..."
                value={inputMessage}
                onChange={(e) => setInputMessage(e.target.value)}
                disabled={isLoading}
              />
              
              <button
                type="submit"
                className="bg-primary-500 hover:bg-primary-600 rounded-lg px-4 py-2 text-white disabled:opacity-50"
                disabled={!inputMessage.trim() || isLoading}
              >
                Send
              </button>
            </div>
          </form>
        </>
      )}
    </div>
  );
}
