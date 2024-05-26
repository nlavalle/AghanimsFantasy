import '@/assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from '@/App.vue'
import router from '@/router'

import { createVuetify } from 'vuetify'
import 'vuetify/styles' // Ensure you are using css-loader
import { AghanimsFantasyDarkTheme } from './style/vuetify-themes'

const vuetify = createVuetify({
  theme: {
    defaultTheme: 'AghanimsFantasyDarkTheme',
    themes: {
      AghanimsFantasyDarkTheme
    }
  },
})

const app = createApp(App)

app.use(createPinia())
app.use(router)

app.use(vuetify)

app.mount('#app')
