<template>
  <v-container class="ma-0" style="height:2.4rem">
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
              <v-list-item v-bind="props" class="league-selector" :title="item.raw.name"></v-list-item>
            </template>
          </v-select>
        </v-row>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useFantasyLeagueStore } from '@/stores/fantasyLeague'
import { localApiService } from '@/services/localApiService'
import { VContainer, VRow, VCol, VSelect, VListItem } from 'vuetify/components'
import type { FantasyLeague } from '@/types/FantasyLeague';

const leagueStore = useFantasyLeagueStore()
const selectedLeague = ref<FantasyLeague>({
  id: 0,
  leagueId: 0,
  isActive: false,
  name: '',
  fantasyDraftLocked: 0,
  leagueStartTime: 0,
  leagueEndTime: 0
})

const leagueOptions = computed(() => {
  return leagueStore.activeLeagues
})

onMounted(() => {
  localApiService.getFantasyLeagues().then((result: any) => {
    leagueStore.setLeagues(result)
    //default to most recent league
    selectedLeague.value = leagueStore.activeLeagues.reduce((max, current) => {
      return current.id > max.id ? current : max
    }, leagueStore.activeLeagues[0])
    leagueStore.setSelectedLeague(selectedLeague.value)
  })
})

function updateSelectedLeague() {
  leagueStore.setSelectedLeague(selectedLeague.value)
}
</script>

<style scoped>
.league-selector-label {
  text-transform: uppercase;
  font-family: "Roboto", sans-serif;
  font-size: 0.8rem;
}

.league-selector :deep(.v-list-item-title) {
  font-size: 0.8rem;
}
</style>
