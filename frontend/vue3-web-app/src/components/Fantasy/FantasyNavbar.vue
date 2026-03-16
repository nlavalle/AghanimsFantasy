<template>
  <div class="league-select bg-primary">
    <v-tabs v-model="leagueStore.selectedLeague" selected-class="selected-tab">
      <v-tab v-for="league in leagueOptions" :value="league"
        :variant="!leagueStore.isLeagueFinished(league) ? 'text' : 'plain'">
        {{ league.name }}
        <v-icon v-if="!leagueStore.isLeagueOpen(league)" class="ml-1" icon="fa-solid fa-lock" size="x-small">
        </v-icon>
      </v-tab>
    </v-tabs>
  </div>
  <div class="bg-secondary">
    <v-tabs v-model="leagueStore.currentFantasyLeague" selected-class="selected-tab">
      <v-tab v-for="fantasyLeague in fantasyLeagueOptions" :value="fantasyLeague"
        :variant="!leagueStore.isDraftFinished(fantasyLeague) ? 'text' : 'plain'">
        {{ fantasyLeague.name }} ({{leagueStore.fantasyDraftPoints.find((draft: FantasyDraftPoints) =>
          draft?.fantasyDraft?.fantasyLeagueId
          ==
          fantasyLeague.id)?.fantasyPlayerPoints.length ?? 0}}/5)
        <v-icon v-if="!leagueStore.isDraftOpen(fantasyLeague)" class="ml-1" icon="fa-solid fa-lock" size="x-small">
        </v-icon>
      </v-tab>
    </v-tabs>
  </div>
</template>

<script setup lang="ts">
import { computed, onBeforeMount, watch } from 'vue';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import { useAuthStore } from '@/stores/auth';
import { VIcon, VTab, VTabs } from 'vuetify/components'
import { useRoute } from 'vue-router';
import router from '@/router';
import type { FantasyDraftPoints } from './fantasyDraft';

const authStore = useAuthStore()
const leagueStore = useFantasyLeagueStore()

const route = useRoute();

const leagueOptions = computed(() => {
  const league_ids = new Set(leagueStore.activeFantasyLeagues.map(fl => fl.leagueId))
  return leagueStore.activeLeagues
    .filter(league => league_ids.has(league.league_id))
    .sort((a, b) => b.start_timestamp - a.start_timestamp)
})

const fantasyLeagueOptions = computed(() => {
  if (!leagueStore.selectedLeague) return
  return leagueStore.activeFantasyLeagues
    .filter(fantasyLeague => fantasyLeague.leagueId == leagueStore.selectedLeague.league_id)
    .sort((a, b) => a.id - b.id)
})

onBeforeMount(() => {
  router.isReady()
    .then(() => {
      if (authStore.authenticated) {
        leagueStore.fetchFantasyDraftPoints()
      }
    });
})

watch(() => leagueStore.selectedLeague, () => {
  if (leagueStore.fantasyLeagues.length > 0) {
    // Don't fire this if fantasyLeagues hasn't been loaded yet
    if (!leagueStore.currentFantasyLeague || leagueStore.currentFantasyLeague.leagueId != leagueStore.selectedLeague.league_id) {
      // Want to respect router param selected Fantasy League if it's set and not override it
      leagueStore.currentFantasyLeague = leagueStore.currentFantasyLeague;
    }
    if (authStore.authenticated) {
      leagueStore.fetchFantasyDraftPoints()
    }
  }
})

watch(() => leagueStore.currentFantasyLeague, () => {
  if (leagueStore.currentFantasyLeague) {
    router.push({
      path: route.path,
      query: { fantasyLeagueId: leagueStore.currentFantasyLeague.id }
    })
  }
})

</script>

<style scoped>
.league-select {
  border-top: 1px solid rgba(255, 255, 255, 0.08);
}

.league-selector-label {
  text-transform: uppercase;
  font-family: var(--font-body);
  font-size: 0.8rem;
}

.league-selector :deep(.v-list-item-title) {
  font-size: 0.8rem;
}

.selected-tab {
  backdrop-filter: brightness(150%);
  transition: backdrop-filter 0.2s ease;
}
</style>
