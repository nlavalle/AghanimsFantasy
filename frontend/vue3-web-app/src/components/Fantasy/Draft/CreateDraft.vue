<template>
  <div>
    <!-- Desktop: player pool (stats panel is fixed in FantasyView) -->
    <div v-if="!display.mobile.value" class="draft-body">
      <PlayerPicksAvailable class="picks-available" />
    </div>

    <!-- Mobile: drawer for stats, full-width pool -->
    <div v-else style="margin-top:140px;">
      <v-navigation-drawer v-model="mobileDrawer" temporary location="right" :width="300">
        <PlayerStats @save-player="toggleDrawer" />
      </v-navigation-drawer>
      <PlayerPicksAvailable class="picks-available" @click="toggleDrawer" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue';
import { VNavigationDrawer } from 'vuetify/components';
import PlayerPicksAvailable from './PlayerPool/PlayerPicksAvailable.vue';
import PlayerStats from './PlayerPanel/PlayerStats.vue';
import { fantasyDraftState } from '@/components/Fantasy/fantasyDraft';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import { useDisplay } from 'vuetify';

const display = useDisplay()

const mobileDrawer = ref(false);

const leagueStore = useFantasyLeagueStore();
const { selectedPlayer, fantasyPlayerPointsAvailable, setFantasyPlayerPoints } = fantasyDraftState();

onMounted(() => {
  if (fantasyPlayerPointsAvailable.value.length === 0) {
    leagueStore.fetchFantasyPlayerPoints()?.then(() => setFantasyPlayerPoints(leagueStore.fantasyPlayerPoints))
  }
  if (leagueStore.fantasyPlayersStats.length === 0) {
    leagueStore.fetchFantasyPlayerViewModels();
  }
})

watch(() => leagueStore.selectedFantasyLeague, () => {
  selectedPlayer.value = undefined;
})

const toggleDrawer = () => {
  mobileDrawer.value = !mobileDrawer.value;
}
</script>

<style scoped>
.draft-body {
  display: flex;
  align-items: flex-start;
  gap: 16px;
  margin-top: 16px;
}
</style>
