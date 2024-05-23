import { describe, it, expect } from 'vitest'

import { mount } from '@vue/test-utils'
import AboutView from '../../views/AboutView.vue'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'

const vuetify = createVuetify({
  components,
  directives,
})

global.ResizeObserver = require('resize-observer-polyfill')

describe('AboutView', () => {
  it('renders properly', () => {
    const wrapper = mount({
      template: '<v-layout><about-view></about-view></v-layout>'
    }, {
      props: {},
      global: {
        components: {
          AboutView,
        },
        plugins: [vuetify],
      }
    })
    expect(wrapper.text()).toContain('Dota 2 Fantasy Draft')
  })
})
