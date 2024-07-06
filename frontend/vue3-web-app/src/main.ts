import '@/assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from '@/App.vue'
import router from '@/router'

import { createVuetify } from 'vuetify'
import 'vuetify/styles' // Ensure you are using css-loader
import { AghanimsFantasyDarkTheme } from './style/vuetify-themes'

// Fontawesome icons
import { aliases, fa } from 'vuetify/iconsets/fa-svg'
import { library } from '@fortawesome/fontawesome-svg-core';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faChevronLeft, faChevronRight, faCaretDown, faArrowUp, faStepForward, faStepBackward } from '@fortawesome/free-solid-svg-icons';
// import { far } from '@fortawesome/free-regular-svg-icons';

library.add(faChevronLeft, faChevronRight, faCaretDown, faArrowUp, faStepForward, faStepBackward);

const vuetify = createVuetify({
  theme: {
    defaultTheme: 'AghanimsFantasyDarkTheme',
    themes: {
      AghanimsFantasyDarkTheme
    }
  },
  icons: {
    defaultSet: 'fa',
    aliases,
    sets: {
      fa
    }
  }
})

const app = createApp(App)

app.use(createPinia())
app.use(router)

app.use(vuetify)

app.component('font-awesome-icon', FontAwesomeIcon);

app.mount('#app')
