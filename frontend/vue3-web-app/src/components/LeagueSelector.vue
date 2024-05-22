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
          <v-select class="league-selector" label="League" v-model="selectedLeague" :items="leagueOptions"
            item-title="name" @update:model-value="updateSelectedLeague" density="compact" single-line
            variant="underlined" return-object>
          </v-select>
        </v-row>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useLeagueStore, type League } from '@/stores/league'
import { localApiService } from '@/services/localApiService'
import { VContainer, VRow, VCol, VSelect } from 'vuetify/components'

const leagueStore = useLeagueStore()
const selectedLeague = ref<League>({
  id: 0,
  isActive: false,
  name: '',
  fantasyDraftLocked: 0
})

const leagueOptions = computed(() => {
  return leagueStore.activeLeagues
})

onMounted(() => {
  localApiService.getLeagues().then((result: any) => {
    leagueStore.setLeagues(result)
    //default to most recent league
    selectedLeague.value = leagueStore.activeLeagues.reduce((max, current) => {
      return current.id > max.id ? current : max
    }, leagueStore.activeLeagues[0])
    leagueStore.setSelectedLeague(selectedLeague.value)
  })
})

function updateSelectedLeague() {
  console.log(selectedLeague.value)
  leagueStore.setSelectedLeague(selectedLeague.value)
}
</script>

<style scoped>
.league-selector-label {
  text-transform: uppercase;
  font-family: "Roboto", sans-serif;
  font-size: 0.8rem;
}

.icon {
  flex-shrink: 0;
  width: 25px;
  margin-right: 10px;
}
</style>
