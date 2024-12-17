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
    </v-row>
    <v-row>
      <v-tabs-window v-model="statsTab" style="width:100%">
        <v-tabs-window-item value="fantasy">
          <v-col>
            <v-row v-if="leagueStore.selectedFantasyLeague">
              <FantasyDataTable v-model:selectedFantasyLeague="leagueStore.selectedFantasyLeague">
              </FantasyDataTable>
            </v-row>
          </v-col>
        </v-tabs-window-item>
        <v-tabs-window-item value="league">
          <v-col>
            <v-row v-if="leagueStore.selectedFantasyLeague">
              <LeagueDataTable v-model:selectedFantasyLeague="leagueStore.selectedFantasyLeague">
              </LeagueDataTable>
            </v-row>
          </v-col>
        </v-tabs-window-item>
        <v-tabs-window-item value="match">
          <v-col>
            <v-row v-if="leagueStore.selectedFantasyLeague">
              <MatchDataTable v-model:selectedFantasyLeague="leagueStore.selectedFantasyLeague"
                v-model:draftFiltered="draftFiltered">
              </MatchDataTable>
            </v-row>
          </v-col>
        </v-tabs-window-item>
      </v-tabs-window>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { VContainer, VRow, VCol, VTabs, VTab, VTabsWindow, VTabsWindowItem } from 'vuetify/components';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import FantasyDataTable from '@/components/Stats/FantasyDataTable.vue';
import LeagueDataTable from '@/components/Stats/LeagueDataTable.vue';
import MatchDataTable from '@/components/Stats/MatchDataTable.vue';

const statsTab = ref('fantasy')
const leagueStore = useFantasyLeagueStore();
const draftFiltered = false;

onMounted(() => {
  if (leagueStore.selectedFantasyLeague.id == 0) {
    leagueStore.fetchFantasyLeagues(undefined)
  }
})

</script>

<style scoped>
.league-selector :deep(.v-list-item-title) {
  font-size: 0.8rem;
}
</style>