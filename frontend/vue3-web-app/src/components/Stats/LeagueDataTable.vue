<!-- eslint-disable vue/valid-v-slot -->
<template>
    <v-col>
        <v-row v-if="!isDesktop" dense>
            <v-tabs v-model="leagueTab" density="compact">
                <v-tab value="kda" min-width="70px" width="70px">K/D/A</v-tab>
                <v-tab value="farm" min-width="70px" width="70px">Farm</v-tab>
                <v-tab value="support" min-width="80px" width="80px">Supp.</v-tab>
                <v-tab value="damageHealing" min-width="100px" width="100px">Dmg/Heal</v-tab>
            </v-tabs>
        </v-row>
        <v-row>
            <v-data-table class="league-table" :items="leagueMetadataStatsIndexed" :headers="displayedLeagueColumns"
                density="compact" :items-per-page="itemsPerPage"
                :style="{ 'font-size': isDesktop ? '0.8rem' : '0.7rem' }">
                <template v-slot:item.leaguePlayer="{ value }">
                    <v-row v-if="isDesktop" class="ma-1 pa-1">
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
                    <v-row v-else class="ma-0 pa-0">
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
                    {{ (value / 1000).toFixed(1) + 'k' }}
                </template>
                <template v-slot:item.totalTowerDamage="{ value }">
                    {{ (value / 1000).toFixed(1) + 'k' }}
                </template>
                <template v-slot:item.totalHeroHealing="{ value }">
                    {{ (value / 1000).toFixed(1) + 'k' }}
                </template>
                <template v-slot:item.totalStunDuration="{ value }">
                    {{ (value / 1000).toFixed(1) + 'k' }}
                </template>
                <template v-slot:bottom>
                    <div class="text-center pt-2">
                        <v-pagination v-model="page" :length="pageCount"></v-pagination>
                    </div>
                </template>
            </v-data-table>
        </v-row>
    </v-col>
</template>

<script setup lang="ts">
import { ref, defineModel, onMounted, watch, computed } from 'vue';
import { VRow, VCol, VDataTable, VPagination, VTabs, VTab } from 'vuetify/components';
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
        width: isDesktop.value ? '240px' : '200px',
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
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalDeaths',
        title: isDesktop.value ? 'Deaths' : 'D',
        align: 'left',
        value: (row: any) => row.deaths,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalAssists',
        title: isDesktop.value ? 'Assists' : 'A',
        align: 'left',
        value: (row: any) => row.assists,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
];
const farmLeagueColumns = [
    {
        key: 'totalLastHits',
        title: isDesktop.value ? 'Last Hits' : 'LH',
        align: 'left',
        value: (row: any) => row.lastHits,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalDenies',
        title: isDesktop.value ? 'Denies' : 'DN',
        align: 'left',
        value: (row: any) => row.denies,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalGoldPerMin',
        title: isDesktop.value ? 'Avg GPM' : 'G',
        align: 'left',
        value: (row: any) => row.goldPerMin,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalXpPerMin',
        title: isDesktop.value ? 'Avg XPM' : 'XP',
        align: 'left',
        value: (row: any) => row.xpPerMin,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
];
const supportLeagueColumns = [
    {
        key: 'totalSupportGoldSpent',
        title: isDesktop.value ? 'Supp. Gold Spent' : 'SG',
        align: 'left',
        value: (row: any) => row.supportGoldSpent ?? 0,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalObsPlaced',
        title: isDesktop.value ? 'Obs Placed' : 'OB',
        align: 'left',
        value: (row: any) => row.observerWardsPlaced ?? 0,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalSentriesPlaced',
        title: isDesktop.value ? 'Sentries Placed' : 'SN',
        align: 'left',
        value: (row: any) => row.sentryWardsPlaced ?? 0,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalWardsDewarded',
        title: isDesktop.value ? 'Dewards' : 'DW',
        align: 'left',
        value: (row: any) => row.wardsDewarded ?? 0,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalCampsStacked',
        title: isDesktop.value ? 'Camps Stacked' : 'C',
        align: 'left',
        value: (row: any) => row.campsStacked ?? 0,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
];
const damageHealingLeagueColumns = [
    {
        key: 'totalHeroDamage',
        title: isDesktop.value ? 'Hero Dmg' : 'HD',
        align: 'left',
        value: (row: any) => row.heroDamage ?? 0,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalTowerDamage',
        title: isDesktop.value ? 'Tower Dmg' : 'TD',
        align: 'left',
        value: (row: any) => row.towerDamage ?? 0,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalHeroHealing',
        title: isDesktop.value ? 'Hero Healing' : 'HH',
        align: 'left',
        value: (row: any) => row.heroHealing ?? 0,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalStunDuration',
        title: isDesktop.value ? 'Stun Dur.' : 'SD',
        align: 'left',
        value: (row: any) => row.stunDuration ?? 0,
        sortable: true,
        sort: (a: number, b: number) => b - a
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
    font-family: Avenir, Helvetica, Arial, sans-serif;
}

.v-data-table ::v-deep(th) {
    padding: 0 4px 0 0 !important;
    /* border-right: 1px solid #383838; */
}

.v-data-table ::v-deep(td) {
    padding: 0 4px 0 0 !important;
    /* border-right: 1px solid #383838; */
}
</style>