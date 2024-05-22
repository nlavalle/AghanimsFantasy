<!-- eslint-disable vue/valid-v-slot -->
<template>
  <v-container>
    <v-row>
      <v-col>
        <v-row>
          <v-tabs v-model="statsTab">
            <v-tab value="fantasy">Fantasy</v-tab>
            <v-tab value="league">League</v-tab>
          </v-tabs>
        </v-row>
      </v-col>
      <v-col>
        <v-row>
          <v-select class="league-selector" label="League" v-model="selectedLeague" :items="availableLeagues"
            item-title="name" item-value="id" variant="underlined" return-object>
          </v-select>
        </v-row>
      </v-col>
    </v-row>
    <v-row>
      <v-tabs-window v-model="statsTab" style="width:100%">
        <v-tabs-window-item value="fantasy">
          <v-col>
            <v-row v-if="selectedLeague">
              <FantasyDataTable class="fantasy-table" v-model:selectedLeague="selectedLeague">
              </FantasyDataTable>
            </v-row>
          </v-col>
        </v-tabs-window-item>
        <v-tabs-window-item value="league">
          <span>League data</span>
        </v-tabs-window-item>
      </v-tabs-window>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { VContainer, VRow, VCol, VTabs, VTab, VTabsWindow, VTabsWindowItem, VTextField, VBtn, VSelect } from 'vuetify/components';
import { localApiService } from '@/services/localApiService';
import { useLeagueStore } from '@/stores/league';
import FantasyDataTable from '@/components/Stats/FantasyDataTable.vue';

const statsTab = ref('fantasy')
const leagueStore = useLeagueStore();

const fantasyFilter = ref('');

const selectedLeague = ref();

const availableLeagues = computed(() => {
  return leagueStore.activeLeagues
})

onMounted(() => {
  localApiService.getLeagues().then((result: any) => {
    leagueStore.setLeagues(result)
  })
})

</script>

<style scoped>
.fantasy-table {
  font-size: 0.8rem;
  font-family: Avenir, Helvetica, Arial, sans-serif;
}

table th+th {
  border-left: 1px solid #dddddd;
}

table td+td {
  border-left: 1px solid #dddddd;
}
</style>
