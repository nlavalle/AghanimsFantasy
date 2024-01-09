<template>
  <q-layout view="hHh lpR fFf" id="app">
    <q-page-container>
      <router-view />
    </q-page-container>

  </q-layout>
</template>

<script lang="ts">
import { defineComponent } from 'vue';

export default defineComponent({
  name: 'App'
});
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 20px;
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
  background: radial-gradient(ellipse farthest-corner at center top, #373c4b 0%, #192038 100%);
  /* background-color:#192038; */
  color: #fff;
}
</style>