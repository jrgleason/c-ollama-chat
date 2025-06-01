import { defineConfig } from 'vite'
import tailwindcss from '@tailwindcss/vite'
import react from '@vitejs/plugin-react'
import { resolve } from 'path'

export default defineConfig({
  plugins: [
    react(),
    tailwindcss(),
  ],
  build: {
    outDir: resolve(__dirname, '../src/wwwroot'),
    emptyOutDir: true,
    assetsDir: 'assets',
    sourcemap: true,
    minify: process.env.NODE_ENV === 'production',
  },
  server: {
    proxy: {
      '/api': {
        target: 'https://localhost:7231',
        secure: false,
        changeOrigin: true
      }
    }
  },
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: './src/test/setup.js', // if you have a setup file
    coverage: {
      provider: 'v8', // or 'istanbul'
      reporter: ['text', 'json', 'html'],
    },
  },
})
