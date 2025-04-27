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
import { faSquare } from '@fortawesome/free-regular-svg-icons';
import {
  faChevronLeft, faChevronRight, faChevronDown, faCaretDown, faArrowUp, faArrowDown,
  faStepForward, faStepBackward, faLock, faLockOpen,
  faMagnifyingGlass, faCheckSquare, faTimesCircle
} from '@fortawesome/free-solid-svg-icons';
import { faDiscord, faGithub } from '@fortawesome/free-brands-svg-icons';
// import { far } from '@fortawesome/free-regular-svg-icons';

// gtag.js for analytics
import { configure } from 'vue-gtag';

configure({
  tagId: import.meta.env.VITE_GA_MEASUREMENT_ID
})

library.add(
  // Regular
  faSquare,
  // Solid
  faChevronLeft, faChevronRight, faChevronDown, faCaretDown, faArrowUp, faArrowDown,
  faStepForward, faStepBackward, faLock, faLockOpen,
  faMagnifyingGlass, faCheckSquare, faTimesCircle,
  // Brand
  faDiscord, faGithub
);

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
