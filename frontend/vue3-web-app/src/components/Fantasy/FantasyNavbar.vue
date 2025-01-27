<template>
  <div class="league-select bg-primary">
    <v-tabs v-model="leagueStore.selectedLeague" selected-class="selected-tab">
      <v-tab v-for="league in leagueOptions" :value="league"
        :variant="leagueStore.isLeagueActive(league) ? 'text' : 'plain'">
        {{ league.name }}
        <v-icon v-if="!leagueStore.isLeagueOpen(league)" class="ml-1" icon="fa-solid fa-lock" size="x-small">
        </v-icon>
      </v-tab>
    </v-tabs>
  </div>
  <div class="bg-secondary">
    <v-tabs v-model="leagueStore.selectedFantasyLeague" selected-class="selected-tab">
      <v-tab v-for="fantasyLeague in fantasyLeagueOptions" :value="fantasyLeague"
        :variant="leagueStore.isDraftActive(fantasyLeague.leagueEndTime) ? 'text' : 'plain'">
        {{ fantasyLeague.name }} ({{ leagueStore.fantasyDraftPoints.find(draft => draft?.fantasyDraft?.fantasyLeagueId
          ==
          fantasyLeague.id)?.fantasyPlayerPoints.length ?? 0 }}/5)
        <v-icon v-if="!leagueStore.isDraftOpen(fantasyLeague.fantasyDraftLocked)" class="ml-1" icon="fa-solid fa-lock"
          size="x-small">
        </v-icon>
      </v-tab>
    </v-tabs>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, watch } from 'vue';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import { useAuthStore } from '@/stores/auth';
import { VIcon, VTab, VTabs } from 'vuetify/components'
import { useRoute } from 'vue-router';
import router from '@/router';

const authStore = useAuthStore()
const leagueStore = useFantasyLeagueStore()

const route = useRoute();

const leagueOptions = computed(() => {
  return leagueStore.activeLeagues.sort((a, b) => b.id - a.id)
})

const fantasyLeagueOptions = computed(() => {
  return leagueStore.activeFantasyLeagues
    .filter(fantasyLeague => fantasyLeague.leagueId == leagueStore.selectedLeague.id)
    .sort((a, b) => a.id - b.id)
})

onMounted(() => {
  router.isReady()
    .then(() => authStore.checkAuthenticatedAsync())
    .then(() => leagueStore.fetchLeagues())
    .then(() => leagueStore.fetchFantasyLeagues(Number(route.query.fantasyLeagueId)))
    .then(() => {
      if (authStore.authenticated) {
        leagueStore.fetchFantasyDraftPoints()
      }

    });
})

watch(() => authStore.authenticated, () => {
  if (leagueStore.allLeagues.length != 0) {
    // If mounted hasn't fetched leagues yet then ignore this first watch
    leagueStore.fetchLeagues()
      .then(() => leagueStore.fetchFantasyLeagues(Number(route.query.fantasyLeagueId)))
      .then(() => leagueStore.fetchFantasyDraftPoints());
  }
})

watch(() => leagueStore.selectedLeague, () => {
  if (leagueStore.fantasyLeagues.length > 0) {
    // Don't fire this if fantasyLeagues hasn't been loaded yet
    leagueStore.selectedFantasyLeague = leagueStore.defaultFantasyLeague;
    if (authStore.authenticated) {
      leagueStore.fetchFantasyDraftPoints()
    }
  }
})

watch(() => leagueStore.selectedFantasyLeague, () => {
  router.push({
    path: route.path,
    query: { fantasyLeagueId: leagueStore.selectedFantasyLeague.id }
  })
})

</script>

<style scoped>
/* .league-select {
  height: 2.5rem;
} */

.league-selector-label {
  text-transform: uppercase;
  font-family: "Roboto", sans-serif;
  font-size: 0.8rem;
}

.league-selector :deep(.v-list-item-title) {
  font-size: 0.8rem;
}

.selected-tab {
  backdrop-filter: brightness(150%);
}
</style>