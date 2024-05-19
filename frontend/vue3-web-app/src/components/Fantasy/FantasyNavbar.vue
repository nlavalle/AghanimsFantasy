<template>
  <div class="bg-primary">
    <v-tabs v-model="tabIndex">
      <LeagueSelector />
      <v-tab class="pa-0 ma-0" style="min-width: 0px" />
      <v-tab value="fantasy" to="/fantasy">Draft</v-tab>
      <v-tab value="leaderboard" to="/leaderboard">Leaderboard</v-tab>
      <!-- <v-tab value="highlights" to="/highlights">Highlights</v-tab> -->
    </v-tabs>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { VTab, VTabs } from 'vuetify/components'
import LeagueSelector from '@/components/LeagueSelector.vue'
import { collapsed } from './FantasyState';

const tabIndex = ref(0)

watch(collapsed, (newValue) => {
  // Reset the fantasy so it doesn't look like it's set if a user comes back to it
  if (newValue) {
    tabIndex.value = 0;
  }
});
</script>
