<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated class="header text-white" height-hint="40">
      <q-toolbar>
        <LeagueSelector />

        <q-toolbar-title class="title">
          <q-avatar>
            <img src="https://cdn.quasar.dev/logo-v2/svg/logo-mono-white.svg">
          </q-avatar>
          Aghanim's Fantasy
        </q-toolbar-title>

        <q-space />
        <LoginDiscord />
      </q-toolbar>
      <q-separator color="white" />
      <q-tabs align="left">
        <q-route-tab to="/about" label="About" />
        <q-route-tab to="/league" label="Leagues" />
        <q-route-tab to="/fantasy" label="Fantasy" />
      </q-tabs>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import LoginDiscord from 'src/components/LoginDiscord.vue'
import LeagueSelector from 'src/components/LeagueSelector.vue'

export default defineComponent({
  name: 'MainLayout',
  components: {
    LoginDiscord,
    LeagueSelector,
  }
});
</script>

<style>
.debug {
  border: 1px solid red;
  padding: 10px;
}

.header {
  background-color: #3a404d;
  /* z-index: 9000; */
}

.title {
  position: absolute;
  left: 50%;
  transform: translateX(-50%);
}
</style>