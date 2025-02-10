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

const leagueSchedules = ref<League[]>([]);

onMounted(() => {
  router.isReady()
    .then(() => localApiService.getLeagueSchedules().then((result: any) => {
      leagueSchedules.value = result.sort((a: League, b: League) => a.start_timestamp - b.start_timestamp);
    }));
})

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
    width: '60%'
  },
  {
    title: 'Dates',
    value: 'dates',
    width: '25%'
  },
  {
    title: 'Region',
    value: 'region',
    width: '15%'
  },
];

</script>

<style scoped>
.attentionTitle {
  color: var(--aghanims-fantasy-main-2)
}

.schedule-table {
  max-width: 600px;
}
</style>