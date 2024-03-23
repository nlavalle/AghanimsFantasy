<template>
  <q-layout>
    <q-header elevated class="header text-white" height-hint="40">
      <q-toolbar>
        <LeagueSelector :class="{ 'league-selector-wide': isWideScreen, 'league-selector-narrow': !isWideScreen }" />

        <q-toolbar-title v-if="isWideScreen" class="title">
          <q-avatar>
            <q-img src="~assets/BannerAvatar.png" height="38px" width="38px"/>
          </q-avatar>
          Aghanim's Fantasy
        </q-toolbar-title>
        <q-toolbar-title v-else class="title">
          <q-avatar>
            <q-img src="~assets/BannerAvatar.png" height="38px" width="38px"/>
          </q-avatar>
        </q-toolbar-title>

        <q-space />
        <LoginDiscord class="login-discord" />
      </q-toolbar>
      <q-separator color="white" />
      <q-tabs align="left">
        <q-route-tab to="/about" label="About" />
        <!-- <q-route-tab to="/league" label="Leagues" /> -->
        <q-route-tab to="/fantasy" label="Fantasy" />
        <q-route-tab to="/stats" label="Stats" />
        <q-route-tab to="/leaderboard" label="Leaderboard" />
        <q-route-tab to="/highlights" label="Highlights" />
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

.login-discord {
  max-width: 150px;
}

.league-selector-wide {
  min-width: 120px;
  max-width: 300px;
}

.league-selector-narrow {
  min-width: 120px;
  max-width: 150px;
}

.header {
  background-color: var(--nadcl-main-4);
  /* z-index: 9000; */
}

.title {
  position: absolute;
  left: 50%;
  transform: translateX(-50%);
}
</style>