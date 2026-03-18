<template>
  <div>
    <div v-if="!isMounted" class="d-flex justify-center align-center" style="min-height: 200px;">
      <v-progress-circular color="primary" indeterminate />
    </div>
    <v-container v-if="isMounted">
      <v-row>
        <v-col>
          <v-row class="align-center">
            <v-tabs v-model="statsTab">
              <v-tab value="fantasy">Fantasy</v-tab>
              <v-tab value="league">League</v-tab>
              <v-tab value="match">Matches</v-tab>
              <!-- <v-tab value="topeight">Top 8</v-tab> -->
            </v-tabs>
            <v-btn icon="fa-refresh" @click="refreshStats" :loading="!loaded" :disabled="!loaded" size="small"
              variant="outlined" aria-label="Refresh stats data">
            </v-btn>
          </v-row>
        </v-col>
      </v-row>
      <v-row class="section-gap">
        <v-tabs-window v-model="statsTab" style="width:100%" transition="fade-transition"
          reverse-transition="fade-transition">
          <v-tabs-window-item value="fantasy">
            <v-col>
              <v-row v-if="leagueStore.currentFantasyLeague">
                <FantasyDataTable :selectedFantasyLeague="leagueStore.currentFantasyLeague">
                </FantasyDataTable>
              </v-row>
            </v-col>
          </v-tabs-window-item>
          <v-tabs-window-item value="league">
            <v-col>
              <v-row v-if="leagueStore.currentFantasyLeague">
                <LeagueDataTable :selectedFantasyLeague="leagueStore.currentFantasyLeague">
                </LeagueDataTable>
              </v-row>
            </v-col>
          </v-tabs-window-item>
          <v-tabs-window-item value="match">
            <v-col>
              <v-row v-if="leagueStore.currentFantasyLeague">
                <MatchDataTable :selectedFantasyLeague="leagueStore.currentFantasyLeague"
                  v-model:draftFiltered="draftFiltered">
                </MatchDataTable>
              </v-row>
            </v-col>
          </v-tabs-window-item>
          <!-- <v-tabs-window-item value="topeight">
          <v-col>
            <v-row v-if="leagueStore.currentFantasyLeague">
              <Top8DataTable :selectedFantasyLeague="leagueStore.currentFantasyLeague"
                v-model:draftFiltered="draftFiltered">
              </Top8DataTable>
            </v-row>
          </v-col>
        </v-tabs-window-item> -->
        </v-tabs-window>
      </v-row>
    </v-container>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { VContainer, VRow, VCol, VTabs, VTab, VTabsWindow, VTabsWindowItem, VProgressCircular, VBtn } from 'vuetify/components';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import FantasyDataTable from '@/components/Stats/FantasyDataTable.vue';
import LeagueDataTable from '@/components/Stats/LeagueDataTable.vue';
import MatchDataTable from '@/components/Stats/MatchDataTable.vue';
// import Top8DataTable from '@/components/Stats/Top8DataTable.vue'; // NADCL special

const statsTab = ref('fantasy')
const leagueStore = useFantasyLeagueStore();
const draftFiltered = ref(false);

const isMounted = ref(false);
const loaded = ref(true);

onMounted(() => {
  if (leagueStore.currentFantasyLeague?.id == 0) {
    leagueStore.fetchFantasyLeagues().then(() => isMounted.value = true)
  } else {
    isMounted.value = true
  }
})

const refreshStats = () => {
  loaded.value = false
  Promise.all([
    leagueStore.fetchFantasyPlayerPoints(),
    leagueStore.fetchFantasyLeagueMetadataStats(),
    leagueStore.fetchPlayerFantasyMatchStats(),
  ])
    ?.then(() => {
      loaded.value = true
    })
    .catch(() => {
      loaded.value = true
    })
}

</script>

<style scoped>
.league-selector :deep(.v-list-item-title) {
  font-size: 0.8rem;
}
</style>
