<template>
  <q-layout id="app">
    <q-page-container>
      <router-view />
    </q-page-container>

  </q-layout>
</template>

<script lang="ts">
import { useQuasar } from 'quasar';
import { defineComponent } from 'vue';

export default defineComponent({
  name: 'App',
  setup() {
    const $q = useQuasar();
    $q.dark.set(true);
  }
});
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

.nav-container {
  margin-left: 10px;
}

/*-------------------- Body --------------------*/
*,
*::before,
*::after {
  box-sizing: border-box;
}

body {
  margin: 0;
  /* background: radial-gradient(ellipse farthest-corner at center top, #373c4b 0%, #192038 100%); */
  /* background-color: #192038; */
  color: #fff;
}

body.body--dark {
  margin: 0;
  background: var(--nadcl-main-3);
}

/*---------------- Color Palette ----------------*/
.text-nadcl-accent {
  color: var(--nadcl-accent) !important;
}
.bg-nadcl-accent {
  background: var(--nadcl-accent-dark) !important;
}
</style>