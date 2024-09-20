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
          <v-select label="League" v-model="selectedLeague" :items="leagueOptions" item-title="name"
            @update:model-value="updateSelectedLeague" density="compact" single-line variant="underlined" return-object>
            <template v-slot:item="{ props, item }">
              <v-list-item v-bind="props" class="league-selector" :title="item.raw.name"
                :variant="leagueStore.isLeagueActive(item.raw) ? 'text' : 'plain'">
                <template v-slot:append>
                  <v-icon v-if="!leagueStore.isLeagueOpen(item.raw)" icon="fa-solid fa-lock"
                    size="x-small"></v-icon>
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
    <v-tabs v-model="selectedFantasyLeague">
      <v-tab class="pa-0 ma-0" style="min-width: 0px" />
      <v-tab v-for="fantasyLeague in fantasyLeagueOptions" 
        :value="fantasyLeague"
        :variant="leagueStore.isDraftActive(fantasyLeague.leagueEndTime) ? 'text' : 'plain'"
        @click="updateSelectedFantasyLeague">
          {{fantasyLeague.name}}
          <v-icon v-if="!leagueStore.isDraftOpen(fantasyLeague.fantasyDraftLocked)" class="ml-1" icon="fa-solid fa-lock"
            size="x-small">
          </v-icon>
      </v-tab>
    </v-tabs>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useFantasyLeagueStore } from '@/stores/fantasyLeague'
import { fantasyDraftState } from '@/components/Fantasy/fantasyDraft';
import { localApiService } from '@/services/localApiService'
import { VContainer, VRow, VCol, VSelect, VListItem, VIcon, VTab, VTabs } from 'vuetify/components'
import type { FantasyLeague } from '@/types/FantasyLeague';
import type { League } from '@/types/League';

const leagueStore = useFantasyLeagueStore()
const { clearFantasyDraftPicks } = fantasyDraftState();
const selectedLeague = ref<League>({
  id: 0,
  isActive: false,
  name: ''
})
const selectedFantasyLeague = ref<FantasyLeague>({
  id: 0,
  leagueId: 0,
  isActive: false,
  name: '',
  fantasyDraftLocked: 0,
  leagueStartTime: 0,
  leagueEndTime: 0
})

const leagueOptions = computed(() => {
  return leagueStore.activeLeagues.sort((a, b) => a.id - b.id)
})

const fantasyLeagueOptions = computed(() => {
  return leagueStore.activeFantasyLeagues
    .filter(fantasyLeague => fantasyLeague.leagueId == selectedLeague.value.id)
    .sort((a, b) => a.id - b.id)
})

onMounted(() => {
  localApiService.getLeagues().then((leagueResult: any) => {
    leagueStore.setLeagues(leagueResult);
      //default to most recent league
      selectedLeague.value = leagueStore.defaultLeague;
      leagueStore.setSelectedLeague(selectedLeague.value);

      localApiService.getFantasyLeagues().then((fantasyLeagueResult: any) => {
        leagueStore.setFantasyLeagues(fantasyLeagueResult);
        selectedFantasyLeague.value = leagueStore.defaultFantasyLeague;
        leagueStore.setSelectedFantasyLeague(selectedFantasyLeague.value);
    })
    clearFantasyDraftPicks()
  })
})

function updateSelectedLeague() {
  leagueStore.setSelectedLeague(selectedLeague.value)
  clearFantasyDraftPicks()
}

function updateSelectedFantasyLeague() {
  leagueStore.setSelectedFantasyLeague(selectedFantasyLeague.value)
  clearFantasyDraftPicks()
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