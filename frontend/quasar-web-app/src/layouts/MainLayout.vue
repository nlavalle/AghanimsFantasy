<template>
  <q-layout>
    <q-header elevated class="header text-white" height-hint="40">
      <q-toolbar>
        <LeagueSelector class="league-selector" />

        <q-toolbar-title v-if="isWideScreen" class="title">
          <q-avatar>
            <img src="~assets/BannerAvatar.png">
          </q-avatar>
          Aghanim's Fantasy
        </q-toolbar-title>
        <q-toolbar-title v-else class="title">
          <q-avatar>
            <img src="~assets/BannerAvatar.png">
          </q-avatar>
        </q-toolbar-title>

        <q-space />
        <LoginDiscord />
      </q-toolbar>
      <q-separator color="white" />
      <q-tabs align="left">
        <q-route-tab to="/about" label="About" />
        <!-- <q-route-tab to="/league" label="Leagues" /> -->
        <q-route-tab to="/fantasy" label="Fantasy" />
        <q-route-tab to="/stats" label="Stats" />
        <!-- <q-route-tab to="/leaderboard" label="Leaderboard" /> -->
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
  },
  computed: {
    isWideScreen() {
      return window.outerWidth >= 600;
    }
  }
});
</script>

<style>
.debug {
  border: 1px solid red;
}

.league-selector {
  min-width: 180px;
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