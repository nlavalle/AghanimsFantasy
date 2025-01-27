<template>
  <div class="admin">
    <h3>This is your private fantasy league admin page</h3>
  </div>
  <v-container>
    <v-row>
      <v-col>
        <v-row>
          <v-tabs v-model="statsTab">
            <v-tab value="privateFantasyDiscordUsers">Discord Users</v-tab>
            <v-tab value="fantasyLeagueWeight">Fantasy League Weight</v-tab>
          </v-tabs>
        </v-row>
      </v-col>
    </v-row>
    <v-row v-if="isMounted">
      <v-tabs-window v-model="statsTab" style="width:100%">
        <v-tabs-window-item value="privateFantasyDiscordUsers">
          <CrudTable :table-columns="columns" :table-items="items" :default-item-specified="defaultItem"
            @save="saveItem" @edit="editItem" @delete="deleteItem" />
        </v-tabs-window-item>
        <v-tabs-window-item value="fantasyLeagueWeight">
          <CrudTable :table-columns="columns" :table-items="items" :default-item-specified="defaultItem"
            @edit="editItem" />
        </v-tabs-window-item>
      </v-tabs-window>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { VContainer, VRow, VCol, VTabs, VTab, VTabsWindow, VTabsWindowItem } from 'vuetify/components';
import { localApiService } from '@/services/localApiService';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import CrudTable from '@/components/Admin/CrudTable.vue'

const leagueStore = useFantasyLeagueStore();

const statsTab = ref('fantasy')
const privateFantasyDiscordUserData = ref([]);
const fantasyLeagueWeightData = ref([]);

const isMounted = ref(false);

onMounted(() => {
  localApiService.getPrivateFantasyPlayers(leagueStore.selectedFantasyLeague.id).then((result: any) => {
    privateFantasyDiscordUserData.value = result;
  })
  localApiService.getPrivateFantasyLeagueWeights().then((result: any) => {
    fantasyLeagueWeightData.value = result;
  })
  isMounted.value = true;
})

const saveItem = (item: any) => {
  switch (statsTab.value) {
    case "privateFantasyDiscordUsers":
      // localApiService.postFantasyLeague(item)?.then(() => {
      //   localApiService.getFantasyLeagues().then((result: any) => {
      //     fantasyLeagueData.value = result;
      //   })
      // })
      break;
  }
}

const editItem = (item: any) => {
  switch (statsTab.value) {
    case "privateFantasyDiscordUsers":
      // localApiService.putLeague(item)?.then(() => {
      //   localApiService.getLeagues('true').then((result: any) => {
      //     leagueStore.setLeagues(result);
      //     leagueData.value = leagueStore.allLeagues;
      //   })
      // })
      break;
    case "fantasyLeagueWeight":
      localApiService.putPrivateFantasyLeagueWeight(item)?.then(() => {
        localApiService.getPrivateFantasyLeagueWeights().then((result: any) => {
          fantasyLeagueWeightData.value = result;
        })
      })
      break;
  }
}

const deleteItem = (item: any) => {
  switch (statsTab.value) {
    case "privateFantasyDiscordUsers":
    // localApiService.deleteLeague(item)?.then(() => {
    //   localApiService.getLeagues('true').then((result: any) => {
    //     leagueStore.setLeagues(result);
    //     leagueData.value = leagueStore.allLeagues;
    //   })
    // });
    // break;
    case "fantasyLeagueWeight":
      localApiService.deleteFantasyLeagueWeight(item)?.then(() => {
        localApiService.getFantasyLeagueWeights().then((result: any) => {
          fantasyLeagueWeightData.value = result;
        })
      });
      break;
  }
}

const columns = computed(() => {
  switch (statsTab.value) {
    case "privateFantasyDiscordUsers":
      return privateFantasyDiscordUserColumns;
    case "fantasyLeagueWeight":
      return fantasyLeagueWeightColumns;
  }
  return [];
});

const items = computed(() => {
  switch (statsTab.value) {
    case "privateFantasyDiscordUsers":
      return privateFantasyDiscordUserData.value
    case "fantasyLeagueWeight":
      return fantasyLeagueWeightData.value
  }
  return [];
})

const defaultItem = computed(() => {
  switch (statsTab.value) {
    case "privateFantasyDiscordUsers":
      privateFantasyDiscordUserDefaultItemSpecified.fantasyLeagueId = leagueStore.selectedFantasyLeague.id
      return privateFantasyDiscordUserDefaultItemSpecified
    case "fantasyLeagueWeight":
      return fantasyLeagueWeightDefaultItemSpecified
  }
  return undefined;
})

const privateFantasyDiscordUserDefaultItemSpecified = {
  fantasyLeagueId: 0,
  discordUserId: 0,
  isAdmin: false
};

const fantasyLeagueWeightDefaultItemSpecified = {
  fantasyLeagueId: 0,
  killsWeight: 0.3,
  deathsWeight: -0.3,
  assistsWeight: 0.15,
  lastHitsWeight: 0.003,
  goldPerMinWeight: 0.002,
  xpPerMinWeight: 0.002,
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
  campsStackedWeight: 0.5,
  rampagesWeight: 0,
  tripleKillsWeight: 0,
  aegisSnatchedWeight: 0,
  rapiersPurchasedWeight: 0,
  couriersKilledWeight: 0.2,
  networthRankWeight: 0,
  supportGoldSpentWeight: 0,
  observerWardsPlacedWeight: 0.15,
  sentryWardsPlacedWeight: 0,
  wardsDewardedWeight: 0.15,
  stunDurationWeight: 0.025
}

const privateFantasyDiscordUserColumns = [
  {
    title: 'Discord ID',
    value: 'discordUserId'
  },
  {
    title: 'Discord Name',
    value: 'discordUser.username'
  },
  {
    title: 'Fantasy League',
    value: 'fantasyLeagueId',
  },
  {
    title: 'Is Admin',
    value: 'isAdmin',
  },
];

const fantasyLeagueWeightColumns = [
  {
    title: 'Fantasy Weight ID',
    value: 'id'
  },
  {
    title: 'Fantasy League ID',
    value: 'fantasyLeagueId'
  },
  {
    title: 'Fantasy League ID',
    value: 'fantasyLeague.name'
  },
  {
    title: 'Kills',
    value: 'killsWeight'
  },
  {
    title: 'Deaths',
    value: 'deathsWeight'
  },
  {
    title: 'Assists',
    value: 'assistsWeight'
  },
  {
    title: 'LastHits',
    value: 'lastHitsWeight'
  },
  {
    title: 'GoldPerMin',
    value: 'goldPerMinWeight'
  },
  {
    title: 'XpPerMin',
    value: 'xpPerMinWeight'
  },
  {
    title: 'campsStacked',
    value: 'campsStackedWeight'
  },
  {
    title: 'observerWards',
    value: 'observerWardsPlacedWeight'
  },
  {
    title: 'stunDuration',
    value: 'stunDurationWeight'
  },
];

</script>

<style></style>
