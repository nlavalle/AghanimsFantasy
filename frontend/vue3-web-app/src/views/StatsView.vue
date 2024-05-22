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
            item-title="name" item-value="id" variant="underlined" @update:modelValue="updateSelectedLeague"
            return-object>
          </v-select>
        </v-row>
      </v-col>
    </v-row>
    <v-row>
      <v-tabs-window v-model="statsTab" style="width:100%">
        <v-tabs-window-item value="fantasy">
          <v-row>
            <v-col class="v-col-2">
              <v-text-field v-model="fantasyFilter" label="Search" clearable>
              </v-text-field>
            </v-col>
            <v-col calss="v-col-1">
              <v-btn>Clear Players</v-btn>
              <!-- <v-btn v-if="selectedFantasyPlayer.length > 0" flat icon="highlight_off" size="14px" padding="md xs"
                @click="clearSelectedFantasyPlayers" /> -->
            </v-col>
          </v-row>
          <v-row>
            <v-data-table class="fantasy-table" :items="playerFantasyStatsIndexed" :headers="displayedFantasyColumns"
              density="compact">
              <template v-slot:item.fantasyPlayer="{ value }">
                <v-row class="mt-1 mb-1 pa-1">
                  <v-col class="mr-2" style="max-width:60px;width:60px;">
                    <v-row>
                      <img height="60px" width="60px" :src="value.playerPicture" />
                    </v-row>
                  </v-col>
                  <v-col class="mt-1" style="width:150px">
                    <v-row>
                      <b>{{ value.playerName }}</b>
                    </v-row>
                    <v-row>
                      {{ value.teamName }}
                      <img :src=getPositionIcon(value.teamPosition) height="15px" width="15px" />
                    </v-row>
                    <v-row>
                      {{ value.totalMatches }} games
                    </v-row>
                  </v-col>
                </v-row>
              </template>
              <template v-slot:item.totalKills="{ value }">
                <b>{{ value.killPoints }}</b>
                <br>
                ({{ value.kills }})
              </template>
              <template v-slot:item.totalDeaths="{ value }">
                <b>{{ value.deathPoints }}</b>
                <br>
                ({{ value.deaths }})
              </template>
              <template v-slot:item.totalAssists="{ value }">
                <b>{{ value.assistPoints }}</b>
                <br>
                ({{ value.assists }})
              </template>
              <template v-slot:item.totalLastHits="{ value }">
                <b>{{ value.lastHitsPoints }}</b>
                <br>
                ({{ value.lastHits }})
              </template>
              <template v-slot:item.totalGoldPerMin="{ value }">
                <b>{{ value.goldPerMinPoints }}</b>
                <br>
                ({{ value.goldPerMin }})
              </template>
              <template v-slot:item.totalXpPerMin="{ value }">
                <b>{{ value.xpPerMinPoints }}</b>
                <br>
                ({{ value.xpPerMin }})
              </template>
              <template v-slot:item.totalSupportGoldSpent="{ value }">
                <b>{{ value.supportGoldSpentPoints }}</b>
                <br>
                ({{ value.supportGoldSpent }})
              </template>
              <template v-slot:item.totalObsPlaced="{ value }">
                <b>{{ value.observerWardsPlacedPoints }}</b>
                <br>
                ({{ value.observerWardsPlaced }})
              </template>
              <template v-slot:item.totalSentriesPlaced="{ value }">
                <b>{{ value.sentryWardsPlacedPoints }}</b>
                <br>
                ({{ value.sentryWardsPlaced }})
              </template>
              <template v-slot:item.totalWardsDewarded="{ value }">
                <b>{{ value.wardsDewardedPoints }}</b>
                <br>
                ({{ value.wardsDewarded }})
              </template>
              <template v-slot:item.totalCampsStacked="{ value }">
                <b>{{ value.campsStackedPoints }}</b>
                <br>
                ({{ value.campsStacked }})
              </template>
              <template v-slot:item.totalHeroDamage="{ value }">
                <b>{{ value.heroDamagePoints }}</b>
                <br>
                ({{ value.heroDamage }})
              </template>
              <template v-slot:item.totalTowerDamage="{ value }">
                <b>{{ value.towerDamagePoints }}</b>
                <br>
                ({{ value.towerDamage }})
              </template>
              <template v-slot:item.totalHeroHealing="{ value }">
                <b>{{ value.heroHealingPoints }}</b>
                <br>
                ({{ value.heroHealing }})
              </template>
              <template v-slot:item.totalStunDuration="{ value }">
                <b>{{ value.stunDurationPoints }}</b>
                <br>
                ({{ value.stunDuration }})
              </template>
            </v-data-table>
          </v-row>
        </v-tabs-window-item>
        <v-tabs-window-item value="league">
          <span>League data</span>
        </v-tabs-window-item>
      </v-tabs-window>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue';
import { VContainer, VRow, VCol, VTabs, VTab, VTabsWindow, VTabsWindowItem, VTextField, VBtn, VDataTable, VSelect } from 'vuetify/components';
import { localApiService } from '@/services/localApiService';
import { useLeagueStore } from '@/stores/league';

const statsTab = ref('fantasy')
const leagueStore = useLeagueStore();

const fantasyTab = ref('kda');
const fantasyFilter = ref('');
const leagueTab = ref('kda');
const leagueCompareTab = ref('avg');
const leagueFilter = ref('');
const compareOn = ref(false);
const leaguePagination = ref({
  sortBy: 'desc',
  descending: false,
  page: 1,
  rowsPerPage: 15
})

const selectedLeague = ref();
const availableLeagues = ref(leagueStore.activeLeagues);


const isDesktop = ref(window.outerWidth >= 600);

const showFantasyFilters = ref(false);

const playerFantasyStats = ref([]);
const fantasyLeagueMetadataStats = ref([]);
const commonFantasyColumns = [
  {
    key: 'fantasyPlayerRank',
    title: '',
    align: 'center',
    value: row => row.position,
    style: 'width: 15px',
    sortable: false
  },
  {
    key: 'fantasyPlayer',
    title: 'Player/Team/Games',
    align: 'left',
    value: row => {
      return {
        playerName: row.fantasyPlayer.dotaAccount.name,
        playerPicture: row.fantasyPlayer.dotaAccount.steamProfilePicture,
        teamName: row.fantasyPlayer.team.name,
        teamPosition: row.fantasyPlayer.teamPosition,
        totalMatches: row.totalMatches
      };
    },
    style: 'width: 400px',
    sortable: false
  },
  {
    key: 'totalPoints',
    title: isDesktop.value ? 'Total Points' : 'Pts',
    align: 'left',
    value: row => row.totalMatchFantasyPoints.toFixed(1),
    format: val => `${val.toLocaleString()}`,
    headerStyle: 'font-weight: bold',
    style: 'font-weight: bold',
    sortable: true,
    sort: (a, b) => a - b
  },
];
const kdaFantasyColumns = [
  {
    key: 'totalKills',
    title: isDesktop.value ? 'Kills' : 'K',
    align: 'left',
    value: row => {
      return {
        kills: row.totalKills,
        killPoints: row.totalKillsPoints.toFixed(1)
      };
    },
    sortable: true,
    sort: (a, b) => a.killPoints - b.killPoints
  },
  {
    key: 'totalDeaths',
    title: isDesktop.value ? 'Deaths' : 'D',
    align: 'left',
    value: row => {
      return {
        deaths: row.totalDeaths,
        deathPoints: row.totalDeathsPoints.toFixed(1)
      };
    },
    sortable: true,
    sort: (a, b) => a.deathPoints - b.deathPoints
  },
  {
    key: 'totalAssists',
    title: isDesktop.value ? 'Assists' : 'A',
    align: 'left',
    value: row => {
      return {
        assists: row.totalAssists,
        assistPoints: row.totalAssistsPoints.toFixed(1)
      };
    },
    sortable: true,
    sort: (a, b) => a.assistPoints - b.assistPoints
  },
];
const farmFantasyColumns = [
  {
    key: 'totalLastHits',
    title: isDesktop.value ? 'Last Hits' : 'LH',
    align: 'left',
    value: row => {
      return {
        lastHits: row.totalLastHits.toLocaleString(),
        lastHitsPoints: row.totalLastHitsPoints.toFixed(1).toLocaleString()
      };
    },
    sortable: true,
    sort: (a, b) => a.lastHitsPoints - b.lastHitsPoints
  },
  {
    key: 'totalGoldPerMin',
    title: isDesktop.value ? 'Avg GPM' : 'G',
    align: 'left',
    value: row => {
      return {
        goldPerMin: row.avgGoldPerMin.toFixed(0).toLocaleString(),
        goldPerMinPoints: row.totalGoldPerMinPoints.toFixed(1).toLocaleString()
      };
    },
    sortable: true,
    sort: (a, b) => a.goldPerMinPoints - b.goldPerMinPoints
  },
  {
    key: 'totalXpPerMin',
    title: isDesktop.value ? 'Avg XPM' : 'XP',
    align: 'left',
    value: row => {
      return {
        xpPerMin: row.avgXpPerMin.toFixed(0).toLocaleString(),
        xpPerMinPoints: row.totalXpPerMinPoints.toFixed(1).toLocaleString()
      };
    },
    sortable: true,
    sort: (a, b) => a.xpPerMinPoints - b.xpPerMinPoints
  },
];
const supportFantasyColumns = [
  {
    key: 'totalSupportGoldSpent',
    title: isDesktop.value ? 'Supp. Gold Spent' : 'SG',
    align: 'left',
    value: row => {
      return {
        supportGoldSpent: row.totalSupportGoldSpent.toFixed(0).toLocaleString() ?? 0,
        supportGoldSpentPoints: row.totalSupportGoldSpentPoints.toFixed(1).toLocaleString()
      };
    },
    sortable: true,
    sort: (a, b) => a.supportGoldSpentPoints - b.supportGoldSpentPoints
  },
  {
    key: 'totalObsPlaced',
    title: isDesktop.value ? 'Obs Placed' : 'OB',
    align: 'left',
    value: row => {
      return {
        observerWardsPlaced: row.totalObserverWardsPlaced.toFixed(0).toLocaleString() ?? 0,
        observerWardsPlacedPoints: row.totalObserverWardsPlacedPoints.toFixed(1).toLocaleString()
      };
    },
    sortable: true,
    sort: (a, b) => a.observerWardsPlacedPoints - b.observerWardsPlacedPoints
  },
  {
    key: 'totalSentriesPlaced',
    title: isDesktop.value ? 'Sentries Placed' : 'SN',
    align: 'left',
    value: row => {
      return {
        sentryWardsPlaced: row.totalSentryWardsPlaced.toFixed(0).toLocaleString() ?? 0,
        sentryWardsPlacedPoints: row.totalSentryWardsPlacedPoints.toFixed(1).toLocaleString()
      };
    },
    sortable: true,
    sort: (a, b) => a.sentryWardsPlacedPoints - b.sentryWardsPlacedPoints
  },
  {
    key: 'totalWardsDewarded',
    title: isDesktop.value ? 'Dewards' : 'DW',
    align: 'left',
    value: row => {
      return {
        wardsDewarded: row.totalWardsDewarded.toFixed(0).toLocaleString() ?? 0,
        wardsDewardedPoints: row.totalWardsDewardedPoints.toFixed(1).toLocaleString()
      };
    },
    sortable: true,
    sort: (a, b) => a.wardsDewardedPoints - b.wardsDewardedPoints
  },
  {
    key: 'totalCampsStacked',
    title: isDesktop.value ? 'Camps Stacked' : 'C',
    align: 'left',
    value: row => {
      return {
        campsStacked: row.totalCampsStacked.toFixed(0).toLocaleString() ?? 0,
        campsStackedPoints: row.totalCampsStackedPoints.toFixed(1).toLocaleString()
      };
    },
    sortable: true,
    sort: (a, b) => a.campsStackedPoints - b.campsStackedPoints
  },
];
const damageHealingFantasyColumns = [
  {
    key: 'totalHeroDamage',
    title: isDesktop.value ? 'Hero Dmg' : 'HD',
    align: 'left',
    value: row => {
      return {
        heroDamage: row.totalHeroDamage.toLocaleString() ?? 0,
        heroDamagePoints: row.totalHeroDamagePoints.toFixed(1).toLocaleString()
      };
    },
    sortable: true,
    sort: (a, b) => a.heroDamagePoints - b.heroDamagePoints
  },
  {
    key: 'totalTowerDamage',
    title: isDesktop.value ? 'Tower Dmg' : 'TD',
    align: 'left',
    value: row => {
      return {
        towerDamage: row.totalTowerDamage.toLocaleString() ?? 0,
        towerDamagePoints: row.totalTowerDamagePoints.toFixed(1).toLocaleString()
      };
    },
    sortable: true,
    sort: (a, b) => a.towerDamagePoints - b.towerDamagePoints
  },
  {
    key: 'totalHeroHealing',
    title: isDesktop.value ? 'Hero Healing' : 'HH',
    align: 'left',
    value: row => {
      return {
        heroHealing: row.totalHeroHealing.toLocaleString() ?? 0,
        heroHealingPoints: row.totalHeroHealingPoints.toFixed(1).toLocaleString()
      };
    },
    sortable: true,
    sort: (a, b) => a.heroHealingPoints - b.heroHealingPoints
  },
  {
    key: 'totalStunDuration',
    title: isDesktop.value ? 'Stun Dur.' : 'SD',
    align: 'left',
    value: row => {
      return {
        stunDuration: row.totalStunDuration.toFixed(1).toLocaleString() ?? 0,
        stunDurationPoints: row.totalStunDurationPoints.toFixed(1).toLocaleString()
      };
    },
    sortable: true,
    sort: (a, b) => a.stunDurationPoints - b.stunDurationPoints
  },
];

const commonLeagueColumns = [
  {
    name: 'leaguePlayer',
    label: 'Player/Team',
    align: 'left',
    field: row => row.player.dotaAccount.name.toLowerCase(),
    style: 'width: 400px',
    sortable: true,
    sort: (a, b) => {
      if (a > b) return 1;
      if (a < b) return -1;
    }
  },
];
const kdaLeagueColumns = [
  {
    name: 'totalKills',
    label: isDesktop.value ? 'Kills' : 'K',
    align: 'left',
    field: row => row.kills,
    sortable: true,
    sort: (a, b) => a - b
  },
  {
    name: 'totalDeaths',
    label: isDesktop.value ? 'Deaths' : 'D',
    align: 'left',
    field: row => row.deaths,
    sortable: true,
    sort: (a, b) => a - b
  },
  {
    name: 'totalAssists',
    label: isDesktop.value ? 'Assists' : 'A',
    align: 'left',
    field: row => row.assists,
    sortable: true,
    sort: (a, b) => a - b
  },
];
const farmLeagueColumns = [
  {
    name: 'totalLastHits',
    label: isDesktop.value ? 'Last Hits' : 'LH',
    align: 'left',
    field: row => row.lastHits,
    sortable: true,
    sort: (a, b) => a - b
  },
  {
    name: 'totalDenies',
    label: isDesktop.value ? 'Denies' : 'DN',
    align: 'left',
    field: row => row.denies,
    sortable: true,
    sort: (a, b) => a - b
  },
  {
    name: 'totalGoldPerMin',
    label: isDesktop.value ? 'Avg GPM' : 'G',
    align: 'left',
    field: row => row.goldPerMin,
    sortable: true,
    sort: (a, b) => a - b
  },
  {
    name: 'totalXpPerMin',
    label: isDesktop.value ? 'Avg XPM' : 'XP',
    align: 'left',
    field: row => row.xpPerMin,
    sortable: true,
    sort: (a, b) => a - b
  },
];
const supportLeagueColumns = [
  {
    name: 'totalSupportGoldSpent',
    label: isDesktop.value ? 'Supp. Gold Spent' : 'SG',
    align: 'left',
    field: row => {
      return {
        supportGoldSpent: row.metadataPlayer?.supportGoldSpent ?? 0,
      };
    },
    sortable: true,
    sort: (a, b) => a.supportGoldSpent - b.supportGoldSpent
  },
  {
    name: 'totalObsPlaced',
    label: isDesktop.value ? 'Obs Placed' : 'OB',
    align: 'left',
    field: row => row.observerWardsPlaced ?? 0,
    sortable: true,
    sort: (a, b) => a - b
  },
  {
    name: 'totalSentriesPlaced',
    label: isDesktop.value ? 'Sentries Placed' : 'SN',
    align: 'left',
    field: row => row.sentryWardsPlaced ?? 0,
    sortable: true,
    sort: (a, b) => a - b
  },
  {
    name: 'totalWardsDewarded',
    label: isDesktop.value ? 'Dewards' : 'DW',
    align: 'left',
    field: row => {
      return {
        wardsDewarded: row.metadataPlayer?.wardsDewarded ?? 0
      };
    },
    sortable: true,
    sort: (a, b) => a.wardsDewarded - b.wardsDewarded
  },
  {
    name: 'totalCampsStacked',
    label: isDesktop.value ? 'Camps Stacked' : 'C',
    align: 'left',
    field: row => row.campsStacked ?? 0,
    sortable: true,
    sort: (a, b) => a - b
  },
];
const damageHealingLeagueColumns = [
  {
    name: 'totalHeroDamage',
    label: isDesktop.value ? 'Hero Dmg' : 'HD',
    align: 'left',
    field: row => row.heroDamage ?? 0,
    sortable: true,
    sort: (a, b) => a - b
  },
  {
    name: 'totalTowerDamage',
    label: isDesktop.value ? 'Tower Dmg' : 'TD',
    align: 'left',
    field: row => row.towerDamage ?? 0,
    sortable: true,
    sort: (a, b) => a - b
  },
  {
    name: 'totalHeroHealing',
    label: isDesktop.value ? 'Hero Healing' : 'HH',
    align: 'left',
    field: row => row.heroHealing ?? 0,
    sortable: true,
    sort: (a, b) => a - b
  },
  {
    name: 'totalStunDuration',
    label: isDesktop.value ? 'Stun Dur.' : 'SD',
    align: 'left',
    field: row => row.stunDuration ?? 0,
    sortable: true,
    sort: (a, b) => a - b
  },
];

const selectedFantasyColumns = ref(commonFantasyColumns.map(column => column.name));
const selectedLeaguePlayer = ref([]);
const compareLeaguePlayers = ref([]);

const selectedFantasyPlayer = ref([]);
const compareFantasyPlayers = ref([]);

// onMounted(() => {
//   if (leagueStore.selectedLeague) {
//     localApiService.getPlayerFantasyStats(leagueStore.selectedLeague.id)
//       .then(result => playerFantasyStats.value = result);
//     localApiService.getFantasyLeagueMetadataStats(leagueStore.selectedLeague.id)
//       .then(result => fantasyLeagueMetadataStats.value = result);
//   }
// });

// Define a function to stringify nested objects recursively
const stringifyNested = (obj) => {
  if (typeof obj !== 'object' || obj === null) {
    return String(obj);
  }
  return Object.values(obj)
    .map(val => stringifyNested(val))
    .join(' ');
};

const updateSelectedLeague = () => {
  if (selectedLeague.value) {
    localApiService.getPlayerFantasyStats(selectedLeague.value.id)
      .then(result => playerFantasyStats.value = result);
    localApiService.getFantasyLeagueMetadataStats(selectedLeague.value.id)
      .then(result => fantasyLeagueMetadataStats.value = result);
  }
}

const selectLeagueRow = (selectedRow) => {
  const index = selectedLeaguePlayer.value.findIndex(row => row.player === selectedRow.player);

  if (index !== -1) {
    selectedLeaguePlayer.value.splice(index, 1);
  } else {
    if (selectedLeaguePlayer.value.length < 2) {
      selectedLeaguePlayer.value.push(selectedRow);
    }
  }
};

const selectFantasyRow = (selectedRow) => {
  const index = selectedFantasyPlayer.value.findIndex(row => row.player === selectedRow.player);

  if (index !== -1) {
    selectedFantasyPlayer.value.splice(index, 1);
  } else {
    if (selectedFantasyPlayer.value.length < 2) {
      selectedFantasyPlayer.value.push(selectedRow);
    }
  }
};

const clearSelectedLeaguePlayers = () => {
  selectedLeaguePlayer.value = [];
};

const clearSelectedFantasyPlayers = () => {
  selectedFantasyPlayer.value = [];
};

const CompareLeaguePlayers = () => {
  compareOn.value = !compareOn.value;
  var leagueTable = document.getElementById('leagueTable');
  leagueTable.classList.toggle("collapsed");

  var leagueCompareTable = document.getElementById('leagueCompareTable');
  leagueCompareTable.classList.toggle("collapsed");
  compareLeaguePlayers.value = [...selectedLeaguePlayer.value];
  clearSelectedLeaguePlayers();
}

const CompareFantasyPlayers = () => {
  compareOn.value = !compareOn.value;
  var fantasyTable = document.getElementById('fantasyTable');
  fantasyTable.classList.toggle("collapsed");

  var fantasyCompareTable = document.getElementById('fantasyCompareTable');
  fantasyCompareTable.classList.toggle("collapsed");
  compareFantasyPlayers.value = [...selectedFantasyPlayer.value];
  clearSelectedFantasyPlayers();
}

const playerFantasyStatsIndexed = computed(() => {
  return playerFantasyStats.value
    .map((player, index) => ({
      ...player,
      position: index + 1
    })).filter(item =>
      Object.values(item).some(val => stringifyNested(val).toLowerCase().includes(fantasyFilter.value.toLowerCase())
      ));
});

const fantasyLeagueMetadataStatsIndexed = computed(() => {
  return fantasyLeagueMetadataStats.value
    .map((player, index) => ({
      ...player,
      position: index + 1
    })).filter(item =>
      Object.values(item).some(val => stringifyNested(val).toLowerCase().includes(leagueFilter.value.toLowerCase())
      ));
});

watch(() => leagueStore.selectedLeague, (newValue) => {
  if (newValue) {
    if (leagueStore.selectedLeague) {
      localApiService.getPlayerFantasyStats(leagueStore.selectedLeague.id)
        .then(result => playerFantasyStats.value = result);
      localApiService.getFantasyLeagueMetadataStats(leagueStore.selectedLeague.id)
        .then(result => fantasyLeagueMetadataStats.value = result);
    }
  }
});

const displayedFantasyColumns = computed(() => {
  if (isDesktop.value) {
    return [...commonFantasyColumns, ...kdaFantasyColumns, ...farmFantasyColumns, ...supportFantasyColumns, ...damageHealingFantasyColumns];
  }
  else {
    switch (fantasyTab.value) {
      case 'kda':
        return [...commonFantasyColumns, ...kdaFantasyColumns];
      case 'farm':
        return [...commonFantasyColumns, ...farmFantasyColumns];
      case 'support':
        return [...commonFantasyColumns, ...supportFantasyColumns];
      case 'damageHealing':
        return [...commonFantasyColumns, ...damageHealingFantasyColumns];
      default:
        return [...commonFantasyColumns];
    }
  }
})

const displayedLeagueColumns = computed(() => {
  if (isDesktop.value) {
    return [...commonLeagueColumns, ...kdaLeagueColumns, ...farmLeagueColumns, ...supportLeagueColumns, ...damageHealingLeagueColumns];
  }
  else {
    switch (leagueTab.value) {
      case 'kda':
        return [...commonLeagueColumns, ...kdaLeagueColumns];
      case 'farm':
        return [...commonLeagueColumns, ...farmLeagueColumns];
      case 'support':
        return [...commonLeagueColumns, ...supportLeagueColumns];
      case 'damageHealing':
        return [...commonLeagueColumns, ...damageHealingLeagueColumns];
      default:
        return [...commonLeagueColumns];
    }
  }
})

const getPositionIcon = (positionInt: number) => {
  return `icons/pos_${positionInt}.png`
}
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
