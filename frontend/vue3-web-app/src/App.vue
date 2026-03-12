<template>
  <v-app>
    <Navbar />
    <v-main>
      <FantasyNavbar />
      <RouterView v-slot="{ Component }">
        <Transition name="fade" mode="out-in">
          <component :is="Component" />
        </Transition>
      </RouterView>
    </v-main>
    <Footer />
  </v-app>
</template>

<script setup lang="ts">
import router from '@/router';
import { RouterView, useRoute } from 'vue-router'
import Navbar from '@/components/TheNavbar.vue'
import FantasyNavbar from '@/components/Fantasy/FantasyNavbar.vue'
import Footer from '@/components/TheFooter.vue'
import { VApp, VMain } from 'vuetify/components'
import { onBeforeMount, watch } from 'vue'
import { useFantasyLeagueStore } from './stores/fantasyLeague'
import { useAuthStore } from './stores/auth'

const authStore = useAuthStore();
const fantasyLeagueStore = useFantasyLeagueStore();
const route = useRoute();

onBeforeMount(() => {
  router.isReady()
    .then(() => {
      authStore.checkAuthenticatedAsync().then(
        () => {
          fantasyLeagueStore.fetchLeagues()
            .then(() => fantasyLeagueStore.fetchFantasyLeagues())
            .then(() => fantasyLeagueStore.setSelectedFantasyLeagueId(Number(route.query.fantasyLeagueId)))
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
