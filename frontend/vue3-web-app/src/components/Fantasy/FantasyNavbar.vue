<template>
  <div class="bg-primary">
    <v-tabs v-model="tabIndex">
      <v-tab class="pa-0 ma-0" style="min-width: 0px" />
      <v-tab value="fantasy" to="/fantasy">Draft</v-tab>
      <v-tab value="leaderboard" to="/leaderboard">Leaderboard</v-tab>
      <!-- <v-tab value="highlights" to="/highlights">Highlights</v-tab> -->
    </v-tabs>
  </div>
  <div class="bg-secondary">
    <LeagueSelector style="max-width:600px" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { VTab, VTabs } from 'vuetify/components'
import LeagueSelector from '@/components/LeagueSelector.vue'

const tabIndex = ref(1)
</script>

<style scoped>
.league-select {
  height: 3rem;
}
</style>