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
            <v-tab value="fantasyLeagueWeight">Fantasy League Weight</v-tab>
            <v-tab value="fantasyPlayer">Fantasy Players</v-tab>
            <v-tab value="account">Accounts</v-tab>
            <v-tab value="team">Teams</v-tab>
          </v-tabs>
        </v-row>
      </v-col>
    </v-row>
    <v-row v-if="isMounted">
      <v-tabs-window v-model="statsTab" style="width:100%">
        <v-tabs-window-item value="league">
          <CrudTable :table-columns="columns" :table-items="items" :default-item-specified="defaultItem"
            @save="saveItem" @edit="editItem" @delete="deleteItem" />
        </v-tabs-window-item>
        <v-tabs-window-item value="fantasyLeague">
          <CrudTable :table-columns="columns" :table-items="items" :default-item-specified="defaultItem"
            @save="saveItem" @edit="editItem" @delete="deleteItem" />
        </v-tabs-window-item>
        <v-tabs-window-item value="fantasyLeagueWeight">
          <CrudTable :table-columns="columns" :table-items="items" :default-item-specified="defaultItem"
            @save="saveItem" @edit="editItem" @delete="deleteItem" />
        </v-tabs-window-item>
        <v-tabs-window-item value="fantasyPlayer">
          <FantasyPlayerCrud />
        </v-tabs-window-item>
        <v-tabs-window-item value="account">
          <CrudTable :table-columns="columns" :table-items="items" :default-item-specified="defaultItem"
            @save="saveItem" @edit="editItem" @delete="deleteItem" />
        </v-tabs-window-item>
        <v-tabs-window-item value="team">
          <CrudTable :table-columns="columns" :table-items="items" :default-item-specified="defaultItem"
            @save="saveItem" @edit="editItem" @delete="deleteItem" />
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
import type { FantasyLeague } from '@/types/FantasyLeague';
import FantasyPlayerCrud from '@/components/Admin/FantasyPlayerCrud.vue';
import type { DotaAccount, DotaTeam } from '@/types/Dota';

const leagueStore = useFantasyLeagueStore();

const statsTab = ref('fantasy')
const leagueData = ref(leagueStore.allLeagues);
const fantasyLeagueData = ref<FantasyLeague[]>([]);
const fantasyLeagueWeightData = ref([]);
const accountData = ref<DotaAccount[]>([]);
const teamData = ref<DotaTeam[]>([]);

const isMounted = ref(false);

onMounted(() => {
  localApiService.getLeagues('true').then((result: any) => {
    leagueStore.setLeagues(result);
    leagueData.value = leagueStore.allLeagues;
  })
  localApiService.getFantasyLeagues().then((result: any) => {
    fantasyLeagueData.value = result;
  })
  localApiService.getFantasyLeagueWeights().then((result: any) => {
    fantasyLeagueWeightData.value = result;
  })
  localApiService.getAccounts().then((result: any) => {
    accountData.value = result;
  })
  localApiService.getTeams().then((result: any) => {
    teamData.value = result;
  })
  isMounted.value = true;
})

const saveItem = (item: any) => {
  switch (statsTab.value) {
    case "league":
      localApiService.postLeague(item)?.then(() => {
        localApiService.getLeagues('true').then((result: any) => {
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
    case "fantasyLeagueWeight":
      localApiService.postFantasyLeagueWeight(item)?.then(() => {
        localApiService.getFantasyLeagueWeights().then((result: any) => {
          fantasyLeagueWeightData.value = result;
        })
      })
      break;
    case "account":
      localApiService.postAccount(item)?.then(() => {
        localApiService.getAccounts().then((result: any) => {
          accountData.value = result;
        })
      })
      break;
    case "team":
      localApiService.postTeam(item)?.then(() => {
        localApiService.getTeams().then((result: any) => {
          teamData.value = result;
        })
      })
      break;
  }
}

const editItem = (item: any) => {
  switch (statsTab.value) {
    case "league":
      localApiService.putLeague(item)?.then(() => {
        localApiService.getLeagues('true').then((result: any) => {
          leagueStore.setLeagues(result);
          leagueData.value = leagueStore.allLeagues;
        })
      })
      break;
    case "fantasyLeague":
      localApiService.putFantasyLeague(item)?.then(() => {
        localApiService.getFantasyLeagues().then((result: any) => {
          fantasyLeagueData.value = result;
        })
      })
      break;
    case "fantasyLeagueWeight":
      localApiService.putFantasyLeagueWeight(item)?.then(() => {
        localApiService.getFantasyLeagueWeights().then((result: any) => {
          fantasyLeagueWeightData.value = result;
        })
      })
      break;
    case "account":
      localApiService.putAccount(item)?.then(() => {
        localApiService.getAccounts().then((result: any) => {
          accountData.value = result;
        })
      })
      break;
    case "team":
      localApiService.putTeam(item)?.then(() => {
        localApiService.getTeams().then((result: any) => {
          teamData.value = result;
        })
      })
      break;
  }
}

const deleteItem = (item: any) => {
  switch (statsTab.value) {
    case "league":
      localApiService.deleteLeague(item)?.then(() => {
        localApiService.getLeagues('true').then((result: any) => {
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
    case "fantasyLeagueWeight":
      localApiService.deleteFantasyLeagueWeight(item)?.then(() => {
        localApiService.getFantasyLeagueWeights().then((result: any) => {
          fantasyLeagueWeightData.value = result;
        })
      });
      break;
    case "account":
      localApiService.deleteAccount(item)?.then(() => {
        localApiService.getAccounts().then((result: any) => {
          accountData.value = result;
        })
      });
      break;
    case "team":
      localApiService.deleteTeam(item)?.then(() => {
        localApiService.getTeams().then((result: any) => {
          teamData.value = result;
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
    case "fantasyLeagueWeight":
      return fantasyLeagueWeightColumns;
    case "account":
      return accountColumns;
    case "team":
      return teamColumns;
  }
  return [];
});

const items = computed(() => {
  switch (statsTab.value) {
    case "league":
      return leagueData.value
    case "fantasyLeague":
      return fantasyLeagueData.value.map(fantasyLeague => {
        return {
          ...fantasyLeague,
          fantasyDraftLockedFormatted: new Date(fantasyLeague.fantasyDraftLocked * 1000).toLocaleString(),
          leagueStartTimeFormatted: new Date(fantasyLeague.leagueStartTime * 1000).toLocaleString(),
          leagueEndTimeFormatted: new Date(fantasyLeague.leagueEndTime * 1000).toLocaleString(),
        }
      })
    case "fantasyLeagueWeight":
      return fantasyLeagueWeightData.value
    case "account":
      return accountData.value
    case "team":
      return teamData.value
  }
  return [];
})

const defaultItem = computed(() => {
  switch (statsTab.value) {
    case "league":
      return leagueDefaultItemSpecified
    case "fantasyLeague":
      return fantasyLeagueDefaultItemSpecified
    case "fantasyLeagueWeight":
      return fantasyLeagueWeightDefaultItemSpecified
    case "account":
      return accountDefaultItemSpecified
    case "team":
      return teamDefaultItemSpecified
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
  isPrivate: false,
  fantasyDraftLocked: 0,
  leagueStartTime: 0,
  leagueEndTime: 0
}

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

const accountDefaultItemSpecified = {
  id: 0,
  name: "string",
  steamProfilePicture: "https://"
}

const teamDefaultItemSpecified = {
  id: 0,
  name: "string",
  tag: "string",
  abbreviation: "string",
  time_created: 0,
  logo: "string",
  logo_sponsor: "string",
  country_code: "string",
  url: "string",
  games_played: 0,
  player_0_account_id: 0,
  player_1_account_id: 0,
  player_2_account_id: 0,
  player_3_account_id: 0,
  player_4_account_id: 0,
  player_5_account_id: 0,
  admin_account_id: 0
}

const leagueColumns = [
  {
    title: 'ID',
    value: 'id'
  },
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
    title: 'Fantasy League ID',
    value: 'id'
  },
  {
    title: 'Fantasy League Name',
    value: 'name',
  },
  {
    title: 'Active?',
    value: 'isActive',
  },
  {
    title: 'Private?',
    value: 'isPrivate',
  },
  {
    title: 'Fantasy Lock Time Raw',
    value: 'fantasyDraftLocked',
  },
  {
    title: 'Fantasy Lock Time',
    value: 'fantasyDraftLockedFormatted',
  },
  {
    title: 'League Start Time Raw',
    value: 'leagueStartTime',
  },
  {
    title: 'League Start Time',
    value: 'leagueStartTimeFormatted',
  },
  {
    title: 'League End Time Raw',
    value: 'leagueEndTime',
  },
  {
    title: 'League End Time',
    value: 'leagueEndTimeFormatted',
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

const accountColumns = [
  {
    title: 'ID',
    value: 'id'
  },
  {
    title: 'Name',
    value: 'name'
  },
  {
    title: 'Steam Profile Picture',
    value: 'steamProfilePicture'
  }
];

const teamColumns = [
  {
    title: 'ID',
    value: 'id'
  },
  {
    title: 'Name',
    value: 'name'
  },
  {
    title: 'Time Created',
    value: 'time_created'
  }
];

</script>

<style></style>
