import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { fileURLToPath, URL } from 'node:url'

// Storefront (trang khách hàng) là site mặc định ở cổng 3000.
// Đường dẫn /quan-tri được proxy sang app admin (cổng nội bộ 3100).
export default defineConfig({
  plugins: [react()],
  resolve: { alias: { '@': fileURLToPath(new URL('./src', import.meta.url)) } },
  server: {
    port: 3000,
    host: true,
    strictPort: true,
    proxy: {
      '/quan-tri': {
        target: 'http://localhost:3100',
        changeOrigin: true,
        ws: true,
      },
    },
  },
})
