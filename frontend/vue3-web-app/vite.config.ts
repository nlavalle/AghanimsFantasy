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
          // Core framework — changes rarely, long cache life
          'vendor-vue': ['vue', 'vue-router', 'pinia'],

          // Vuetify is massive — isolate it so app code changes don't bust this cache
          'vendor-vuetify': ['vuetify'],

          // FontAwesome SVG icons — large icon sets, very stable
          'vendor-icons': [
            '@fortawesome/fontawesome-svg-core',
            '@fortawesome/free-solid-svg-icons',
            '@fortawesome/free-regular-svg-icons',
            '@fortawesome/free-brands-svg-icons',
            '@fortawesome/vue-fontawesome',
          ],

          // Chart.js only used on Stats — don't pay for it on every page
          'vendor-charts': ['chart.js', 'vue-chartjs'],

          // Stats page: heavy data tables + chart components, route-gated
          'stats': [
            '@/views/StatsView.vue',
            '@/components/Stats/FantasyDataTable.vue',
            '@/components/Stats/LeagueDataTable.vue',
            '@/components/Stats/MatchDataTable.vue',
            '@/components/Stats/Top8DataTable.vue',
          ],

          // Fantasy draft page: large component tree, only needed on /fantasy
          'fantasy': [
            '@/views/FantasyView.vue',
            '@/components/Fantasy/Draft/PlayerPool/PlayerPicksAvailable.vue',
            '@/components/Fantasy/Draft/PlayerPool/PlayerPoolCard.vue',
            '@/components/Fantasy/Draft/PlayerPool/TeamBlock.vue',
            '@/components/Fantasy/Draft/PickBar/CreateDraftPicks.vue',
            '@/components/Fantasy/Draft/PlayerPanel/PlayerStats.vue',
            '@/components/Fantasy/PlayerStats/PlayerRadarChart.vue',
            '@/components/Fantasy/PlayerStats/PlayerStatsBio.vue',
            '@/components/Fantasy/PlayerStats/PlayerTopHeroes.vue',
            '@/components/Fantasy/PlayerStats/FantasyBarChart.vue',
          ],

          // Leaderboard page
          'leaderboard': [
            '@/views/LeaderboardView.vue',
            '@/components/Leaderboard/LeaderboardTable.vue',
            '@/components/Leaderboard/LeaderboardControlBar.vue',
            '@/components/Leaderboard/LeaderboardPageHeader.vue',
            '@/components/Leaderboard/LeaderboardStatsBar.vue',
          ],

          // Admin — requiresAdmin guard, rarely visited
          'admin': [
            '@/views/AdminView.vue',
            '@/views/PrivateFantasyAdminView.vue',
            '@/components/Admin/CrudTable.vue',
            '@/components/Admin/FantasyPlayerCrud.vue',
          ],
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
      },
      '/identity': {
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
      },
      '/swagger': {
        target: 'https://localhost:5001',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/swagger/, '/swagger'),
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
