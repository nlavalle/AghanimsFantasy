<!-- eslint-disable vue/valid-v-slot -->
<template>
  <div class="about">
    <div>
      <p class="text-white">
        Dota 2 Fantasy Draft, click the fantasy tab to get started.
        <br />
        Points are calculated using the pattern below trying to follow the TI fantasy scoring.
        Certain statistics require parsing the .dem replay files to measure, so the unavailable
        metrics won't be involved in the calculation until that is added to the API service that
        fetches match data (hopefully by Season 7).
      </p>
    </div>
    <div>
      <v-data-table class="about-table" :items="statistics" :headers="statsHeaders" hide-default-footer
        density="compact">
        <template v-slot:item.value="{ item }">
          <span :style="getPointsPer(item)">{{ item.value }}</span>
        </template>
        <template v-slot:item.available="{ item }">
          <span :style="getAvailability(item)">{{ item.available }}</span>
        </template>
      </v-data-table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { VDataTable } from 'vuetify/components'

const statsHeaders = [
  {
    title: 'Name',
    value: 'name',
    width: '40%'
  },
  {
    title: 'Points Per',
    value: 'value',
    width: '30%'
  },
  {
    title: 'Available?',
    value: 'available',
    width: '30%'
  },
];

const statistics = [
  {
    name: 'Kill',
    value: '0.3',
    available: 'Yes'
  },
  {
    name: 'Death',
    value: '-0.3',
    available: 'Yes'
  },
  {
    name: 'Last hit',
    value: '0.003',
    available: 'Yes'
  },
  {
    name: 'Gold per minute',
    value: '0.002',
    available: 'Yes'
  },
  {
    name: 'XP per minute',
    value: '0.002',
    available: 'Yes'
  },
  {
    name: 'Wards Planted',
    value: '0.15',
    available: 'Yes'
  },
  {
    name: 'Camps Stacked',
    value: '0.5',
    available: 'Yes'
  },
  {
    name: 'Stuns',
    value: '0.05',
    available: 'Yes'
  },
  {
    name: 'Tower Kill',
    value: '1',
    available: 'No'
  },
  {
    name: 'Roshan Kill',
    value: '1',
    available: 'No'
  },
  {
    name: 'Team Fight',
    value: '3',
    available: 'No'
  },
  {
    name: 'Runes Grabbed',
    value: '0.25',
    available: 'No'
  },
  {
    name: 'First Blood',
    value: '4.0',
    available: 'No'
  }
]

const getPointsPer = (field: { available: string; name: string }) => {
  return field.available == 'Yes'
    ? field.name == 'Death'
      ? 'color: red'
      : 'color: white'
    : 'color: grey'
}

const getAvailability = (field: { available: string }) => {
  return field.available == 'Yes' ? 'color: white' : 'color: grey'
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.about-table {
  margin: 20px;
  max-width: 375px;
  border: 2px solid var(--aghanims-fantasy-main-2);
  border-radius: 5px;
}

.v-data-table ::v-deep(thead) {
  background-color: var(--aghanims-fantasy-main-2);
}

.v-data-table ::v-deep(td) {
  border-right: 1px solid var(--aghanims-fantasy-main-2);
}
</style>
