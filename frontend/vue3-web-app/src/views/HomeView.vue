<template>
  <v-container>
    <v-row>
      <h1><i>Welcome to Aghanim's Fantasy League for Dota 2</i></h1>
    </v-row>
    <v-row>
      <v-card class="ma-2 pa-1" variant="elevated" color="primary" to="/fantasy">
        <v-card-title>
          <h2>Start Drafting!</h2>
        </v-card-title>
      </v-card>
    </v-row>
    <v-row>
      <v-data-table class="schedule-table mt-5" :items="leagueSchedules" :items-per-page="10" :headers="scheduleHeaders"
        hide-default-footer density="compact">
        <template v-slot:top>
          <v-toolbar flat density="compact">
            <v-toolbar-title>Tournament Schedule</v-toolbar-title>
          </v-toolbar>
        </template>
        <template v-slot:item.dates="{ item }">
          <span>{{ new Date(item.start_timestamp * 1000).toLocaleDateString(undefined, {
            month: 'short',
            day: 'numeric',
          }) }} -
            {{ new Date(item.end_timestamp * 1000).toLocaleDateString(undefined, {
              month: 'short',
              day: 'numeric',
            }) }}
          </span>
        </template>
        <template v-slot:item.region="{ item }">
          <span>{{ regions[item.region] }}</span>
        </template>
        <template v-slot:item.fantasyOpen="{ item }">
          <span :class="fantasyStatuses[isFantasyOpen(item.league_id)].className">{{
            fantasyStatuses[isFantasyOpen(item.league_id)].text }}</span>
        </template>
      </v-data-table>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { VContainer, VRow, VCard, VCardTitle, VDataTable, VToolbar, VToolbarTitle } from 'vuetify/components';
import { localApiService } from '@/services/localApiService';
import type { League } from '@/types/League';
import { onMounted, ref } from 'vue';
import router from '@/router';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';

const leagueSchedules = ref<League[]>([]);

const fantasyLeagueStore = useFantasyLeagueStore();

const fantasyStatuses = {
  1: {
    text: 'Open!',
    className: 'status-open'
  },
  2: {
    text: 'Locked',
    className: 'status-locked'
  },
  3: {
    text: 'Coming Soon',
    className: 'status-coming-soon'
  }
}

onMounted(() => {
  router.isReady()
    .then(() => localApiService.getLeagueSchedules().then((result: any) => {
      leagueSchedules.value = result.sort((a: League, b: League) => a.start_timestamp - b.start_timestamp);
    }));
})

const isFantasyOpen = (leagueId: number) => {
  let currentDate = new Date();
  let fantasyLeagues = fantasyLeagueStore.activeFantasyLeagues.filter(fl => fl.leagueId == leagueId);
  if (fantasyLeagues.some(fl => new Date(fl.fantasyDraftLocked * 1000) > currentDate)) {
    // Fantasy draft exists and still open
    return 1
  } else if (fantasyLeagues.some(fl => new Date(fl.fantasyDraftLocked * 1000) < currentDate)) {
    // Fantasy draft exists but is closed
    return 2
  } else {
    // No fantasy draft exists at all
    return 3
  }
}

const regions = [
  "Global", //0
  "NA", //1
  "SA", //2
  "WEU", //3
  "EEU", //4
  "CN", //5
  "SEA" //6
]

const scheduleHeaders = [
  {
    title: 'Name',
    value: 'name',
    width: '40%'
  },
  {
    title: 'Dates',
    value: 'dates',
    width: '25%'
  },
  {
    title: 'Region',
    value: 'region',
    width: '10%'
  },
  {
    title: 'Fantasy Open?',
    value: 'fantasyOpen',
    width: '25%'
  }
];

</script>

<style scoped>
.attentionTitle {
  color: var(--aghanims-fantasy-main-2)
}

.schedule-table {
  max-width: 600px;
}

.status-open {
  color: var(--gradient-blue-1);
}

.status-locked {
  color: var(--aghanims-fantasy-main-1);
}

.status-coming-soon {
  color: var(--aghanims-fantasy-white);
}
</style>