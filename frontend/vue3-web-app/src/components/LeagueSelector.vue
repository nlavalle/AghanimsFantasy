<template>
  <v-select class="league-selector" label="League" v-model="selectedLeague" :items="leagueOptions" item-title="name"
    item-value="id" @click="updateSelectedLeague" variant="underlined">
    <!-- <template v-slot:selection="{ item }">
            <span style="font-size: 10px;">{{ item.title }}</span>
        </template> -->
  </v-select>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useLeagueStore, type League } from '@/stores/league'
import { localApiService } from '@/services/localApiService'
import { VSelect } from 'vuetify/components'

const leagueStore = useLeagueStore()
const selectedLeague = ref<League>({
  id: 0,
  isActive: false,
  name: '',
  fantasyDraftLocked: new Date()
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
  leagueStore.setSelectedLeague(selectedLeague.value)
}
</script>

<style scoped>
.league-container {
  /* box-sizing: border-box; */
  /* border: 1px solid var(--nadcl-white); */
  /* border-radius: 5px; */
  /* margin: auto; */
  /* height: 40px; */
  flex-direction: column;
  justify-self: left;
}

.league-selector {
  max-width: 180px;
}

.icon {
  flex-shrink: 0;
  width: 25px;
  margin-right: 10px;
}
</style>
