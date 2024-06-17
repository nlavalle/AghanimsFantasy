<template>
  <div class="admin">
    <h3>This is an admin page</h3>
  </div>
  <v-container>
    <v-row>
      <v-col>
        <v-row>
          <v-tabs v-model="statsTab">
            <v-tab value="league">League</v-tab>
            <v-tab value="fantasyLeague">Fantasy League</v-tab>
            <v-tab value="fantasyPlayer">Fantasy Players</v-tab>
          </v-tabs>
        </v-row>
      </v-col>
    </v-row>
    <v-row v-if="isMounted">
      <CrudTable :table-columns="columns" :table-items="items" :default-item-specified="defaultItem" @save="saveItem"
        @delete="deleteItem" />
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { VContainer, VRow, VCol, VTabs, VTab } from 'vuetify/components';
import { localApiService } from '@/services/localApiService';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import CrudTable from '@/components/Admin/CrudTable.vue'

const leagueStore = useFantasyLeagueStore();

const statsTab = ref('fantasy')
const leagueData = ref(leagueStore.allLeagues);
const fantasyLeagueData = ref([]);
const fantasyPlayerData = ref([]);

const isMounted = ref(false);

onMounted(() => {
  localApiService.getLeagues().then((result: any) => {
    leagueStore.setLeagues(result);
    leagueData.value = leagueStore.allLeagues;
  })
  localApiService.getFantasyLeagues().then((result: any) => {
    fantasyLeagueData.value = result;
  })
  localApiService.getFantasyPlayers().then((result: any) => {
    fantasyPlayerData.value = result;
  })
  isMounted.value = true;
})

const saveItem = (item: any) => {
  switch (statsTab.value) {
    case "league":
      localApiService.postLeague(item)?.then(() => {
        localApiService.getLeagues().then((result: any) => {
          leagueStore.setLeagues(result);
          leagueData.value = leagueStore.allLeagues;
        })
      })
      break;
    case "fantasyLeague":
      localApiService.postFantasyLeague(item)?.then(() => {
        localApiService.getFantasyLeagues().then((result: any) => {
          fantasyLeagueData.value = result;
        })
      })
      break;
    case "fantasyPlayer":
      localApiService.postFantasyPlayer(item)?.then(() => {
        localApiService.getFantasyPlayers().then((result: any) => {
          fantasyPlayerData.value = result;
        })
      })
      break;
  }
}

const deleteItem = (item: any) => {
  switch (statsTab.value) {
    case "league":
      localApiService.deleteLeague(item)?.then(() => {
        localApiService.getLeagues().then((result: any) => {
          leagueStore.setLeagues(result);
          leagueData.value = leagueStore.allLeagues;
        })
      });
      break;
    case "fantasyLeague":
      localApiService.deleteFantasyLeague(item)?.then(() => {
        localApiService.getFantasyLeagues().then((result: any) => {
          fantasyLeagueData.value = result;
        })
      });
      break;
    case "fantasyPlayer":
      localApiService.deleteFantasyPlayer(item)?.then(() => {
        localApiService.getFantasyPlayers().then((result: any) => {
          fantasyPlayerData.value = result;
        })
      });
      break;
  }

}

const columns = computed(() => {
  switch (statsTab.value) {
    case "league":
      return leagueColumns;
    case "fantasyLeague":
      return fantasyLeagueColumns;
    case "fantasyPlayer":
      return fantasyPlayerColumns;
  }
  return [];
});

const items = computed(() => {
  switch (statsTab.value) {
    case "league":
      return leagueData.value
    case "fantasyLeague":
      return fantasyLeagueData.value
    case "fantasyPlayer":
      return fantasyPlayerData.value
  }
  return [];
})

const defaultItem = computed(() => {
  switch (statsTab.value) {
    case "league":
      return leagueDefaultItemSpecified
    case "fantasyLeague":
      return fantasyLeagueDefaultItemSpecified
    case "fantasyPlayer":
      return fantasyPlayerDefaultItemSpecified
  }
  return undefined;
})

const leagueDefaultItemSpecified = {
  id: 0,
  name: "string",
  isActive: true
}

const fantasyLeagueDefaultItemSpecified = {
  id: 0,
  leagueId: 0,
  name: "string",
  isActive: true,
  fantasyDraftLocked: 0,
  leagueStartTime: 0,
  leagueEndTime: 0,
  fantasyLeagueWeight: {
    id: 0,
    fantasyLeagueId: 0,
    killsWeight: 0,
    deathsWeight: 0,
    assistsWeight: 0,
    lastHitsWeight: 0,
    goldPerMinWeight: 0,
    xpPerMinWeight: 0,
    networthWeight: 0,
    heroDamageWeight: 0,
    towerDamageWeight: 0,
    heroHealingWeight: 0,
    goldWeight: 0,
    scaledHeroDamageWeight: 0,
    scaledTowerDamageWeight: 0,
    scaledHeroHealingWeight: 0,
    fightScoreWeight: 0,
    farmScoreWeight: 0,
    supportScoreWeight: 0,
    pushScoreWeight: 0,
    heroXpWeight: 0,
    campsStackedWeight: 0,
    rampagesWeight: 0,
    tripleKillsWeight: 0,
    aegisSnatchedWeight: 0,
    rapiersPurchasedWeight: 0,
    couriersKilledWeight: 0,
    networthRankWeight: 0,
    supportGoldSpentWeight: 0,
    observerWardsPlacedWeight: 0,
    sentryWardsPlacedWeight: 0,
    wardsDewardedWeight: 0,
    stunDurationWeight: 0
  },
}

const fantasyPlayerDefaultItemSpecified = {
  id: 0,
  fantasyLeagueId: 0,
  teamId: 0,
  dotaAccountId: 0,
  teamPosition: 0
}

const leagueColumns = [
  {
    title: 'League Name',
    value: 'name',
  },
  {
    title: 'Active?',
    value: 'isActive',
  },
];

const fantasyLeagueColumns = [
  {
    title: 'League ID',
    value: 'leagueId',
  },
  {
    title: 'League Name',
    value: 'name',
  },
  {
    title: 'Active?',
    value: 'isActive',
  },
  {
    title: 'Fantasy Lock Time',
    value: 'fantasyDraftLocked',
  },
  {
    title: 'League Start Time',
    value: 'leagueStartTime',
  },
  {
    title: 'League End Time',
    value: 'leagueEndTime',
  },
];

const fantasyPlayerColumns = [
  {
    title: 'Fantasy Player Id',
    value: 'id',
  },
  {
    title: 'Fantasy League Id',
    value: 'fantasyLeagueId',
  },
  {
    title: 'Team Id',
    value: 'teamId',
  },
  {
    title: 'Dota Account ID',
    value: 'dotaAccountId',
  },
  {
    title: 'Team Position',
    value: 'teamPosition',
  },
];

</script>

<style></style>
