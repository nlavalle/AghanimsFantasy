<!-- eslint-disable vue/valid-v-slot -->
<template>
    <v-col>
        <v-row class="search-input">
            <v-col style="min-width: 200px">
                <v-text-field v-model="leagueFilter" label="Search" prepend-inner-icon="fa-magnifying-glass"
                    variant="solo-filled" clearable flat hide-details single-line />
            </v-col>
            <v-col style="min-width: 200px">
                <v-select v-model="roleFilter" label="Filter Role" :items="roleList" item-title="name" item-value="id"
                    multiple clearable hide-details single-line />
            </v-col>
            <v-col style="min-width: 200px">
                <v-select v-model="teamFilter" label="Filter Team" :items="teamsList" item-title="name" item-value="id"
                    multiple clearable hide-details single-line />
            </v-col>
        </v-row>
        <v-row v-if="display.mobile.value" dense>
            <v-tabs v-model="leagueTab" density="compact">
                <v-tab value="kda" min-width="70px" width="70px">K/D/A</v-tab>
                <v-tab value="farm" min-width="70px" width="70px">Farm</v-tab>
                <v-tab value="support" min-width="80px" width="80px">Supp.</v-tab>
                <v-tab value="damageHealing" min-width="100px" width="100px">Dmg/Heal</v-tab>
            </v-tabs>
        </v-row>
        <v-row>
            <v-data-table class="league-table" :items="leagueMetadataStatsIndexed" :headers="displayedLeagueColumns"
                density="compact" :items-per-page="itemsPerPage" v-model:page="page"
                :style="{ 'font-size': !display.mobile.value ? '0.8rem' : '0.7rem' }">
                <template v-slot:item.leaguePlayer="{ value }">
                    <v-row v-if="!display.mobile.value" class="ma-1 pa-1">
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
                <template v-slot:item.totalGoldPerMin="{ value }">
                    {{ (value).toFixed(2) }}
                </template>
                <template v-slot:item.totalXpPerMin="{ value }">
                    {{ (value).toFixed(2) }}
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
import { ref, onMounted, watch, computed } from 'vue';
import { VRow, VCol, VDataTable, VPagination, VTabs, VTab, VTextField, VSelect } from 'vuetify/components';
import { localApiService } from '@/services/localApiService';
import type { LeagueMetadata } from '@/types/LeagueMetadata';
import { useDebouncedRef } from '@/services/debounce'
import type { FantasyLeague } from '@/types/FantasyLeague';
import { useDisplay } from 'vuetify';

const display = useDisplay()

const leagueFilter = useDebouncedRef('');
const roleFilter = ref([]);
const teamFilter = ref([]);

const roleList = [
    {
        id: 1,
        name: 'Carry',
    },
    {
        id: 2,
        name: 'Mid',
    },
    {
        id: 3,
        name: 'Offlane',
    },
    {
        id: 4,
        name: 'Soft Support',
    },
    {
        id: 5,
        name: 'Hard Support',
    },
]

const teamsList = computed(() => {
    // We want the distinct teams
    var teams = leagueMetadataStats.value.map(item => item.fantasyPlayer.team)
    return [...new Map(teams.map(item => [item['id'], item])).values()]
})

const selectedFantasyLeague = defineModel<FantasyLeague>('selectedFantasyLeague');

const page = ref(1)
const itemsPerPage = 15;
const pageCount = computed(() => {
    return Math.ceil(leagueMetadataStatsIndexed.value.length / itemsPerPage);
})

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

const leagueMetadataStats = ref<LeagueMetadata[]>([]);

const commonLeagueColumns = [
    {
        key: 'leaguePlayer',
        title: 'Player/Team',
        align: 'left',
        value: (row: any) => {
            return {
                playerName: row.fantasyPlayer.dotaAccount.name,
                playerPicture: row.fantasyPlayer.dotaAccount.steamProfilePicture,
                teamName: row.fantasyPlayer.team.name,
                teamPosition: row.fantasyPlayer.teamPosition,
                totalMatches: row.matchesPlayed
            };
        },
        width: !display.mobile.value ? '240px' : '200px',
        sortable: true,
        sort: (a: any, b: any) => a.playerName > b.playerName
    },
];
const kdaLeagueColumns = [
    {
        key: 'totalKills',
        title: !display.mobile.value ? 'Kills' : 'K',
        align: 'left',
        value: (row: any) => row.kills,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalDeaths',
        title: !display.mobile.value ? 'Deaths' : 'D',
        align: 'left',
        value: (row: any) => row.deaths,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalAssists',
        title: !display.mobile.value ? 'Assists' : 'A',
        align: 'left',
        value: (row: any) => row.assists,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
];
const farmLeagueColumns = [
    {
        key: 'totalLastHits',
        title: !display.mobile.value ? 'Last Hits' : 'LH',
        align: 'left',
        value: (row: any) => row.lastHits,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalDenies',
        title: !display.mobile.value ? 'Denies' : 'DN',
        align: 'left',
        value: (row: any) => row.denies,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalGoldPerMin',
        title: !display.mobile.value ? 'Avg GPM' : 'G',
        align: 'left',
        value: (row: any) => row.goldPerMinAverage,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalXpPerMin',
        title: !display.mobile.value ? 'Avg XPM' : 'XP',
        align: 'left',
        value: (row: any) => row.xpPerMinAverage,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
];
const supportLeagueColumns = [
    {
        key: 'totalSupportGoldSpent',
        title: !display.mobile.value ? 'Supp. Gold Spent' : 'SG',
        align: 'left',
        value: (row: any) => row.supportGoldSpent ?? 0,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalObsPlaced',
        title: !display.mobile.value ? 'Obs Placed' : 'OB',
        align: 'left',
        value: (row: any) => row.observerWardsPlaced ?? 0,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalSentriesPlaced',
        title: !display.mobile.value ? 'Sentries Placed' : 'SN',
        align: 'left',
        value: (row: any) => row.sentryWardsPlaced ?? 0,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalWardsDewarded',
        title: !display.mobile.value ? 'Dewards' : 'DW',
        align: 'left',
        value: (row: any) => row.wardsDewarded ?? 0,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalCampsStacked',
        title: !display.mobile.value ? 'Camps Stacked' : 'C',
        align: 'left',
        value: (row: any) => row.campsStacked ?? 0,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
];
const damageHealingLeagueColumns = [
    {
        key: 'totalHeroDamage',
        title: !display.mobile.value ? 'Hero Dmg' : 'HD',
        align: 'left',
        value: (row: any) => row.heroDamage ?? 0,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalTowerDamage',
        title: !display.mobile.value ? 'Tower Dmg' : 'TD',
        align: 'left',
        value: (row: any) => row.towerDamage ?? 0,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalHeroHealing',
        title: !display.mobile.value ? 'Hero Healing' : 'HH',
        align: 'left',
        value: (row: any) => row.heroHealing ?? 0,
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'totalStunDuration',
        title: !display.mobile.value ? 'Stun Dur.' : 'SD',
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
        .map((player: LeagueMetadata, index) => ({
            ...player,
            position: index + 1
        }))
        .filter(item => {
            if (!leagueFilter.value) {
                return true;
            } else {
                // Search filter
                return Object.values(item.fantasyPlayer).some(val => stringifyNested(val).toLowerCase().includes(leagueFilter.value.toLowerCase()))
            }
        }
        )
        .filter(item => {
            // Role filter
            if (roleFilter.value.length == 0) {
                return true;
            }
            else {
                return roleFilter.value.some(role => role == item.fantasyPlayer.teamPosition)
            }
        }
        )
        .filter(item => {
            // Team filter
            if (teamFilter.value.length == 0) {
                return true;
            }
            else {
                return teamFilter.value.some(team => team == item.fantasyPlayer.teamId)
            }
        }
        )
        ;
});

onMounted(() => {
    if (selectedFantasyLeague.value) {
        localApiService.getFantasyLeagueMetadataStats(selectedFantasyLeague.value.id)
            .then(result => leagueMetadataStats.value = result);
    }
});

watch(selectedFantasyLeague, () => {
    if (selectedFantasyLeague.value) {
        localApiService.getFantasyLeagueMetadataStats(selectedFantasyLeague.value.id)
            .then(result => leagueMetadataStats.value = result);
    }
});

const displayedLeagueColumns = computed<any>(() => {
    if (!display.mobile.value) {
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
.search-input {
    padding: 1rem;
    padding-top: 0;
}

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