<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated class="header text-white" height-hint="40">
      <q-toolbar>
        <!-- <q-btn
          flat
          dense
          round
          icon="menu"
          aria-label="Menu"
          @click="toggleLeftDrawer"
        /> -->

        <q-toolbar-title>
          <q-avatar>
            <img src="https://cdn.quasar.dev/logo-v2/svg/logo-mono-white.svg">
          </q-avatar>
          Aghanim's Fantasy
        </q-toolbar-title>

        <div>Quasar v{{ $q.version }}</div>
      </q-toolbar>
      <q-separator color="white"/>
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
// import EssentialLink from 'components/EssentialLink.vue';

export default defineComponent({
  name: 'MainLayout',
});
</script>

<style>
.header {
  background-color: #3a404d;
}
</style>