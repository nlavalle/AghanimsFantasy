<!-- eslint-disable vue/valid-v-slot -->
<template>
  <v-container>
    <v-row>
      <v-col>
        <v-row>
          <v-tabs v-model="statsTab">
            <v-tab value="fantasy">Fantasy</v-tab>
            <v-tab value="league">League</v-tab>
            <v-tab value="match">Matches</v-tab>
          </v-tabs>
        </v-row>
      </v-col>
      <v-col>
        <v-row>
          <v-select label="League" v-model="selectedLeague" :items="availableLeagues" item-title="name" item-value="id"
            variant="underlined" return-object>
            <template v-slot:item="{ props, item }">
              <v-list-item v-bind="props" class="league-selector" :title="item.raw.name"></v-list-item>
            </template>
          </v-select>
        </v-row>
      </v-col>
    </v-row>
    <v-row>
      <v-tabs-window v-model="statsTab" style="width:100%">
        <v-tabs-window-item value="fantasy">
          <v-col>
            <v-row v-if="selectedLeague">
              <FantasyDataTable v-model:selectedLeague="selectedLeague">
              </FantasyDataTable>
            </v-row>
          </v-col>
        </v-tabs-window-item>
        <v-tabs-window-item value="league">
          <v-col>
            <v-row v-if="selectedLeague">
              <LeagueDataTable v-model:selectedLeague="selectedLeague">
              </LeagueDataTable>
            </v-row>
          </v-col>
        </v-tabs-window-item>
        <v-tabs-window-item value="match">
          <v-col>
            <v-row v-if="selectedLeague">
              <MatchDataTable v-model:selectedLeague="selectedLeague" v-model:draftFiltered="draftFiltered">
              </MatchDataTable>
            </v-row>
          </v-col>
        </v-tabs-window-item>
      </v-tabs-window>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { VContainer, VRow, VCol, VTabs, VTab, VTabsWindow, VTabsWindowItem, VListItem, VSelect } from 'vuetify/components';
import { localApiService } from '@/services/localApiService';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import FantasyDataTable from '@/components/Stats/FantasyDataTable.vue';
import LeagueDataTable from '@/components/Stats/LeagueDataTable.vue';
import MatchDataTable from '@/components/Stats/MatchDataTable.vue';
import type { FantasyLeague } from '@/types/FantasyLeague';

const statsTab = ref('fantasy')
const leagueStore = useFantasyLeagueStore();
const draftFiltered = false;

const selectedLeague = ref<FantasyLeague>();

const availableLeagues = computed(() => {
  return leagueStore.activeLeagues
})

onMounted(() => {
  if(leagueStore.selectedLeague) {
    selectedLeague.value = leagueStore.selectedLeague;
  } else {
    localApiService.getFantasyLeagues().then((result: any) => {
    leagueStore.setLeagues(result);
    selectedLeague.value = leagueStore.defaultLeague;
    leagueStore.setSelectedLeague(selectedLeague.value)
  })
  }
})

</script>

<style scoped>
.league-selector :deep(.v-list-item-title) {
  font-size: 0.8rem;
}
</style>