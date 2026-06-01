import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { fileURLToPath, URL } from 'node:url'

// Admin chạy dưới đường dẫn con /quan-tri (được storefront cổng 3000 proxy sang).
// Cổng nội bộ 3100; HMR kết nối thẳng về 3100 để không phải proxy websocket.
export default defineConfig({
  base: '/quan-tri/',
  plugins: [react()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    port: 3100,
    host: true,
    strictPort: true,
    hmr: { clientPort: 3100 },
  },
})
