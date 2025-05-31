// filepath: c:\Users\jacki\Code\c-ollama-chat\client\src\main.jsx
import React from 'react'
import ReactDOM from 'react-dom/client'
import './style.css'
import { ChatInterface } from './components/ChatInterface'
import { Auth0ProviderWithConfig } from './components/Auth0ProviderWithConfig'
import { AuthButton } from './components/AuthButton'

function App() {
  return (
    <div className="min-h-screen bg-gradient-to-b from-gray-900 to-gray-800 text-white p-8">
      <header className="max-w-4xl mx-auto mb-6">
        <div className="flex justify-between items-center">
          <h1 className="text-3xl font-bold text-primary-400">C-Ollama Chat</h1>
          <div className="flex space-x-4">
            <a href="https://github.com/yourusername/c-ollama-chat" target="_blank" rel="noreferrer" className="text-sm bg-gray-700 px-3 py-1 rounded-md hover:bg-gray-600 transition-colors">
              GitHub
            </a>
            <AuthButton />
          </div>
        </div>
      </header>
      
      <main className="max-w-4xl mx-auto bg-gray-800 rounded-lg shadow-xl overflow-hidden h-[calc(100vh-12rem)]">
        <ChatInterface />
      </main>
      
      <footer className="max-w-4xl mx-auto mt-6 text-center text-sm text-gray-500">
        <p>Powered by Ollama & ASP.NET Core</p>
      </footer>
    </div>
  );
}

ReactDOM.createRoot(document.getElementById('app')).render(
  <React.StrictMode>
    <Auth0ProviderWithConfig>
      <App />
    </Auth0ProviderWithConfig>
  </React.StrictMode>
)
