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
import { faSquare, faCircleUser } from '@fortawesome/free-regular-svg-icons';
import {
  faChevronLeft, faChevronRight, faChevronDown, faCaretDown, faArrowUp, faArrowDown,
  faStepForward, faStepBackward, faLock, faLockOpen,
  faMagnifyingGlass, faCheckSquare, faTimesCircle,
  faEye, faEyeSlash
} from '@fortawesome/free-solid-svg-icons';
import { faDiscord, faGithub, faGoogle } from '@fortawesome/free-brands-svg-icons';
// import { far } from '@fortawesome/free-regular-svg-icons';

// gtag.js for analytics
import { configure } from 'vue-gtag';

configure({
  tagId: import.meta.env.VITE_GA_MEASUREMENT_ID
})

library.add(
  // Regular
  faSquare, faCircleUser,
  // Solid
  faChevronLeft, faChevronRight, faChevronDown, faCaretDown, faArrowUp, faArrowDown,
  faStepForward, faStepBackward, faLock, faLockOpen,
  faMagnifyingGlass, faCheckSquare, faTimesCircle,
  faEye, faEyeSlash,
  // Brand
  faDiscord, faGithub, faGoogle
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
