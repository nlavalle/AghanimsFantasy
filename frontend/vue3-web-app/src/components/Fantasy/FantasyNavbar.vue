<template>
  <div class="league-select bg-primary">
    <v-container class="ma-0" style="height:2.4rem;max-width:600px">
      <v-row>
        <v-col style="max-width: 80px;" align-self="center">
          <v-row class="pa-2 mb-1 league-selector-label" justify="end">
            <span>League:</span>
          </v-row>
        </v-col>
        <v-col>
          <v-row>
            <v-select label="League" v-model="leagueStore.selectedLeague" :items="leagueOptions" item-title="name"
              @update:model-value="updateSelectedLeague" density="compact" single-line variant="underlined"
              return-object>
              <template v-slot:item="{ props, item }">
                <v-list-item v-bind="props" class="league-selector" :title="item.raw.name"
                  :variant="leagueStore.isLeagueActive(item.raw) ? 'text' : 'plain'">
                  <template v-slot:append>
                    <v-icon v-if="!leagueStore.isLeagueOpen(item.raw)" icon="fa-solid fa-lock" size="x-small"></v-icon>
                  </template>
                </v-list-item>
              </template>
            </v-select>
          </v-row>
        </v-col>
      </v-row>
    </v-container>
  </div>
  <div class="bg-secondary">
    <v-tabs v-model="leagueStore.selectedFantasyLeague">
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
import { VContainer, VRow, VCol, VSelect, VListItem, VIcon, VTab, VTabs } from 'vuetify/components'

const authStore = useAuthStore()
const leagueStore = useFantasyLeagueStore()

const leagueOptions = computed(() => {
  return leagueStore.activeLeagues.sort((a, b) => b.id - a.id)
})

const fantasyLeagueOptions = computed(() => {
  return leagueStore.activeFantasyLeagues
    .filter(fantasyLeague => fantasyLeague.leagueId == leagueStore.selectedLeague.id)
    .sort((a, b) => a.id - b.id)
})

onMounted(() => {
  authStore.checkAuthenticatedAsync()
    .then(() => leagueStore.fetchLeagues())
    .then(() => leagueStore.fetchFantasyLeagues())
    .then(() => {
      if (authStore.authenticated) {
        leagueStore.fetchFantasyDraftPoints()
      }
    });
})

watch(() => authStore.authenticated, () => {
  leagueStore.fetchLeagues()
    .then(() => leagueStore.fetchFantasyLeagues())
    .then(() => leagueStore.fetchFantasyDraftPoints());
})

function updateSelectedLeague() {
  if (authStore.authenticated) {
    leagueStore.fetchFantasyDraftPoints()
  }
  leagueStore.selectedFantasyLeague = leagueStore.defaultFantasyLeague;
}

</script>

<style scoped>
.league-select {
  height: 2.5rem;
}

.league-selector-label {
  text-transform: uppercase;
  font-family: "Roboto", sans-serif;
  font-size: 0.8rem;
}

.league-selector :deep(.v-list-item-title) {
  font-size: 0.8rem;
}
</style>