<!-- eslint-disable vue/valid-v-slot -->
<template>
    <v-data-table class="league-table" :items="leagueMetadataStatsIndexed" :headers="displayedLeagueColumns"
        density="compact" :items-per-page="itemsPerPage">
        <template v-slot:item.leaguePlayer="{ value }">
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
        <template v-slot:item.totalHeroDamage="{ value }">
            {{ value.toLocaleString() }}
        </template>
        <template v-slot:item.totalTowerDamage="{ value }">
            {{ value.toLocaleString() }}
        </template>
        <template v-slot:item.totalHeroHealing="{ value }">
            {{ value.toLocaleString() }}
        </template>
        <template v-slot:item.totalStunDuration="{ value }">
            {{ value.toFixed(1) }}
        </template>
        <template v-slot:bottom>
            <div class="text-center pt-2">
                <v-pagination v-model="page" :length="pageCount"></v-pagination>
            </div>
        </template>
    </v-data-table>
</template>

<script setup lang="ts">
import { ref, defineModel, onMounted, watch, computed } from 'vue';
import { VRow, VCol, VDataTable, VPagination } from 'vuetify/components';
import { localApiService } from '@/services/localApiService';
import type { League } from '@/stores/league';

const isDesktop = ref(window.outerWidth >= 600);

const selectedLeague = defineModel<League>('selectedLeague');

const page = ref(1)
const itemsPerPage = 100;
const pageCount = computed(() => {
    return Math.ceil(leagueMetadataStats.value.length / itemsPerPage);
})

const leagueFilter = ref('');
// const compareOn = ref(false);

const leagueTab = ref('kda');
// const leagueCompareTab = ref('avg');

// const leaguePagination = ref({
//     sortBy: 'desc',
//     descending: false,
//     page: 1,
//     rowsPerPage: 15
// })

// const selectedLeaguePlayer = ref([]);
// const compareLeaguePlayers = ref([]);

const leagueMetadataStats = ref([]);

const commonLeagueColumns = [
    {
        key: 'leaguePlayer',
        title: 'Player/Team',
        align: 'left',
        value: (row: any) => {
            return {
                playerName: row.player.dotaAccount.name,
                playerPicture: row.player.dotaAccount.steamProfilePicture,
                teamName: row.player.team.name,
                teamPosition: row.player.teamPosition,
                totalMatches: row.matchesPlayed
            };
        },
        style: 'width: 400px',
        sortable: true,
    },
];
const kdaLeagueColumns = [
    {
        key: 'totalKills',
        title: isDesktop.value ? 'Kills' : 'K',
        align: 'left',
        value: (row: any) => row.kills,
        sortable: true,
        sort: (a: number, b: number) => a - b
    },
    {
        key: 'totalDeaths',
        title: isDesktop.value ? 'Deaths' : 'D',
        align: 'left',
        value: (row: any) => row.deaths,
        sortable: true,
        sort: (a: number, b: number) => a - b
    },
    {
        key: 'totalAssists',
        title: isDesktop.value ? 'Assists' : 'A',
        align: 'left',
        value: (row: any) => row.assists,
        sortable: true,
        sort: (a: number, b: number) => a - b
    },
];
const farmLeagueColumns = [
    {
        key: 'totalLastHits',
        title: isDesktop.value ? 'Last Hits' : 'LH',
        align: 'left',
        value: (row: any) => row.lastHits,
        sortable: true,
        sort: (a: number, b: number) => a - b
    },
    {
        key: 'totalDenies',
        title: isDesktop.value ? 'Denies' : 'DN',
        align: 'left',
        value: (row: any) => row.denies,
        sortable: true,
        sort: (a: number, b: number) => a - b
    },
    {
        key: 'totalGoldPerMin',
        title: isDesktop.value ? 'Avg GPM' : 'G',
        align: 'left',
        value: (row: any) => row.goldPerMin,
        sortable: true,
        sort: (a: number, b: number) => a - b
    },
    {
        key: 'totalXpPerMin',
        title: isDesktop.value ? 'Avg XPM' : 'XP',
        align: 'left',
        value: (row: any) => row.xpPerMin,
        sortable: true,
        sort: (a: number, b: number) => a - b
    },
];
const supportLeagueColumns = [
    {
        key: 'totalSupportGoldSpent',
        title: isDesktop.value ? 'Supp. Gold Spent' : 'SG',
        align: 'left',
        value: (row: any) => row.supportGoldSpent ?? 0,
        sortable: true,
        sort: (a: number, b: number) => a - b
    },
    {
        key: 'totalObsPlaced',
        title: isDesktop.value ? 'Obs Placed' : 'OB',
        align: 'left',
        value: (row: any) => row.observerWardsPlaced ?? 0,
        sortable: true,
        sort: (a: number, b: number) => a - b
    },
    {
        key: 'totalSentriesPlaced',
        title: isDesktop.value ? 'Sentries Placed' : 'SN',
        align: 'left',
        value: (row: any) => row.sentryWardsPlaced ?? 0,
        sortable: true,
        sort: (a: number, b: number) => a - b
    },
    {
        key: 'totalWardsDewarded',
        title: isDesktop.value ? 'Dewards' : 'DW',
        align: 'left',
        value: (row: any) => row.wardsDewarded ?? 0,
        sortable: true,
        sort: (a: number, b: number) => a - b
    },
    {
        key: 'totalCampsStacked',
        title: isDesktop.value ? 'Camps Stacked' : 'C',
        align: 'left',
        value: (row: any) => row.campsStacked ?? 0,
        sortable: true,
        sort: (a: number, b: number) => a - b
    },
];
const damageHealingLeagueColumns = [
    {
        key: 'totalHeroDamage',
        title: isDesktop.value ? 'Hero Dmg' : 'HD',
        align: 'left',
        value: (row: any) => row.heroDamage ?? 0,
        sortable: true,
        sort: (a: number, b: number) => a - b
    },
    {
        key: 'totalTowerDamage',
        title: isDesktop.value ? 'Tower Dmg' : 'TD',
        align: 'left',
        value: (row: any) => row.towerDamage ?? 0,
        sortable: true,
        sort: (a: number, b: number) => a - b
    },
    {
        key: 'totalHeroHealing',
        title: isDesktop.value ? 'Hero Healing' : 'HH',
        align: 'left',
        value: (row: any) => row.heroHealing ?? 0,
        sortable: true,
        sort: (a: number, b: number) => a - b
    },
    {
        key: 'totalStunDuration',
        title: isDesktop.value ? 'Stun Dur.' : 'SD',
        align: 'left',
        value: (row: any) => row.stunDuration ?? 0,
        sortable: true,
        sort: (a: number, b: number) => a - b
    },
];

// Define a function to stringify nested objects recursively
const stringifyNested = (obj: Object | string | null): any => {
    if (typeof obj !== 'object' || obj === null) {
        return String(obj);
    }
    return Object.values(obj)
        .map(val => stringifyNested(val))
        .join(' ');
};

// const selectLeagueRow = (selectedRow) => {
//     const index = selectedLeaguePlayer.value.findIndex(row => row.player === selectedRow.player);

//     if (index !== -1) {
//         selectedLeaguePlayer.value.splice(index, 1);
//     } else {
//         if (selectedLeaguePlayer.value.length < 2) {
//             selectedLeaguePlayer.value.push(selectedRow);
//         }
//     }
// };

// const clearSelectedLeaguePlayers = () => {
//     selectedLeaguePlayer.value = [];
// };

// const CompareLeaguePlayers = () => {
//     compareOn.value = !compareOn.value;
//     var leagueTable = document.getElementById('leagueTable');
//     leagueTable.classList.toggle("collapsed");

//     var leagueCompareTable = document.getElementById('leagueCompareTable');
//     leagueCompareTable.classList.toggle("collapsed");
//     compareLeaguePlayers.value = [...selectedLeaguePlayer.value];
//     clearSelectedLeaguePlayers();
// }

const leagueMetadataStatsIndexed = computed(() => {
    return leagueMetadataStats.value
        .map((player: object, index) => ({
            ...player,
            position: index + 1
        })).filter(item =>
            Object.values(item).some(val => stringifyNested(val).toLowerCase().includes(leagueFilter.value.toLowerCase())
            ));
});

onMounted(() => {
    if (selectedLeague.value) {
        localApiService.getFantasyLeagueMetadataStats(selectedLeague.value.id)
            .then(result => leagueMetadataStats.value = result);
    }
});

watch(selectedLeague, () => {
    if (selectedLeague.value) {
        localApiService.getFantasyLeagueMetadataStats(selectedLeague.value.id)
            .then(result => leagueMetadataStats.value = result);
    }
});

const displayedLeagueColumns = computed<any>(() => {
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
.league-table {
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