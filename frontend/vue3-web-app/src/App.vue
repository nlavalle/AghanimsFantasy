<template>
  <v-app>
    <header class="bg-surface">
      <Navbar />
    </header>
    <v-main>
      <RouterView />
    </v-main>
    <footer>
      <Footer />
    </footer>
  </v-app>
</template>

<script setup lang="ts">
import router from '@/router';
import { RouterView, useRoute } from 'vue-router'
import Navbar from '@/components/TheNavbar.vue'
import Footer from '@/components/TheFooter.vue'
import { VApp, VMain } from 'vuetify/components'
import { onBeforeMount, watch } from 'vue'
import { useFantasyLeagueStore } from './stores/fantasyLeague'
import { useAuthStore } from './stores/auth'

const authStore = useAuthStore();
const fantasyLeagueStore = useFantasyLeagueStore();
const route = useRoute();

onBeforeMount(() => {
  // Get Leagues/Fantasy Leagues and begin polling for fantasy league updates
  router.isReady()
    .then(() => {
      authStore.checkAuthenticatedAsync().then(
        () => {
          fantasyLeagueStore.fetchLeagues()
            .then(() => fantasyLeagueStore.fetchFantasyLeagues())
            .then(() => fantasyLeagueStore.setSelectedFantasyLeagueId(Number(route.query.fantasyLeagueId)))
            .then(() => fantasyLeagueStore.startFantasyLeaguePolling())
        }
      );
    })
})

watch(() => authStore.authenticated, () => {
  if (fantasyLeagueStore.allLeagues.length != 0) {
    // If mounted hasn't fetched leagues yet then ignore this first watch

    // Refresh fantasy leagues because user may have more/less access now
    fantasyLeagueStore.fetchFantasyLeagues()
      .then(() => fantasyLeagueStore.setSelectedFantasyLeagueId(Number(route.query.fantasyLeagueId)));
  }
})

</script>
