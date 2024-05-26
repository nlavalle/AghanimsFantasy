import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { compression } from 'vite-plugin-compression2'
import vuetify from 'vite-plugin-vuetify'
import VueDevTools from 'vite-plugin-vue-devtools'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    compression(),
    vue(),
    vuetify(),
    VueDevTools(),
  ],
  build: {
    rollupOptions: {
      output: {
        manualChunks: {
          'playerStats': [
            '@/components/Fantasy/CreateDraft/PlayerStats.vue',
            '@/components/Fantasy/PlayerStats/PlayerRadarChart.vue',
            '@/components/Fantasy/PlayerStats/PlayerStatsBio.vue',
            '@/components/Fantasy/PlayerStats/PlayerTopHeroes.vue'
          ],
          'statsPage': [
            '@/views/StatsView.vue',
            '@/components/Stats/FantasyDataTable.vue',
            '@/components/Stats/LeagueDataTable.vue',
          ]
        }
      }
    }
  },
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  server: {
    proxy: {
      '/api': {
        target: 'https://localhost:5001',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/api/, '/api'),
        secure: false,
        headers:
        {
          'Access-Control-Allow-Origin': '*',
          'Access-Control-Allow-Methods': 'GET, POST, PUT, DELETE, OPTIONS',
          'Access-Control-Allow-Headers': 'X-Requested-With, content-type, Authorization'
        }
      }
    },
  },
})
