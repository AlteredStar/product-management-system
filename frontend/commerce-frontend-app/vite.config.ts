import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  server: {
    proxy: {
      '^/api': {                   // Match any path starting with /api
        target: 'https://localhost:5045', // Your backend's actual local URL
        secure: false
      }
    }
  },
  plugins: [react()],
})
