<!-- eslint-disable vue/valid-v-slot -->
<template>
    <v-col>
        <v-row class="search-input">
            <v-col style="min-width: 200px">
                <v-text-field v-model="fantasyFilter" label="Search" prepend-inner-icon="fa-magnifying-glass"
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
        <v-row v-if="!display.mobile.value" dense>
            <v-tabs v-model="fantasyTab" density="compact">
                <v-tab value="kda" min-width="70px" width="70px">K/D/A</v-tab>
                <v-tab value="farm" min-width="70px" width="70px">Farm</v-tab>
                <v-tab value="support" min-width="80px" width="80px">Supp.</v-tab>
                <v-tab value="damageHealing" min-width="100px" width="100px">Dmg/Heal</v-tab>
            </v-tabs>
        </v-row>
        <v-row>
            <v-data-table class="fantasy-table" :items="playerFantasyStatsIndexed" :headers="displayedFantasyColumns"
                density="compact" :items-per-page="itemsPerPage" v-model:page="page"
                :style="{ 'font-size': !display.mobile.value ? '0.8rem' : '0.7rem' }">
                <template v-slot:item.fantasyPlayer="{ value }">
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
                    <v-row v-else class="ma-0 pa-0" style="width:120px">
                        <v-col class="mt-1">
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

                <template v-slot:item.totalPoints="{ value }">
                    <b>{{ value }}</b>
                </template>
                <template v-slot:item.pointsPerMatch="{ value }">
                    <b>{{ value }}</b>
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
                    ({{ (value.supportGoldSpent / 1000).toFixed(1) + 'k' }})
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
import type { FantasyPlayerPoints } from '../Fantasy/fantasyDraft';
import { useDebouncedRef } from '@/services/debounce'
import type { FantasyLeague } from '@/types/FantasyLeague';
import { useDisplay } from 'vuetify';

const display = useDisplay()

const fantasyFilter = useDebouncedRef('');
const roleFilter = ref([]);
const teamFilter = ref([]);

const selectedFantasyLeague = defineModel<FantasyLeague>('selectedFantasyLeague');

const page = ref(1)
const itemsPerPage = 15;
const pageCount = computed(() => {
    return Math.ceil(playerFantasyStatsIndexed.value.length / itemsPerPage);
})

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
    var teams = playerFantasyStats.value.map(item => item.fantasyPlayer.team)
    return [...new Map(teams.map(item => [item.id, item])).values()]
})

onMounted(() => {
    if (selectedFantasyLeague.value && selectedFantasyLeague.value.id != 0) {
        localApiService.getPlayerFantasyStats(selectedFantasyLeague.value.id)
            .then(result => playerFantasyStats.value = result);
    }
});

watch(selectedFantasyLeague, () => {
    if (selectedFantasyLeague.value && selectedFantasyLeague.value.id != 0) {
        localApiService.getPlayerFantasyStats(selectedFantasyLeague.value.id)
            .then(result => playerFantasyStats.value = result);
    }
});

const fantasyTab = ref('kda');

// const showFantasyFilters = ref(false);

const playerFantasyStats = ref<FantasyPlayerPoints[]>([]);

const commonFantasyColumns = [
    {
        key: 'fantasyPlayerRank',
        title: '',
        align: 'center',
        value: (row: any) => row.position,
        width: '15px',
        sortable: true
    },
    {
        key: 'fantasyPlayer',
        title: 'Player/Team/Games',
        align: 'left',
        value: (row: any) => {
            return {
                playerName: row.fantasyPlayer.dotaAccount.name,
                playerPicture: row.fantasyPlayer.dotaAccount.steamProfilePicture,
                teamName: row.fantasyPlayer.team.name,
                teamPosition: row.fantasyPlayer.teamPosition,
                totalMatches: row.totalMatches
            };
        },
        width: !display.mobile.value ? '240px' : '120px',
        sortable: true,
        sort: (a: any, b: any) => a.playerName > b.playerName
    },
    {
        key: 'totalPoints',
        title: !display.mobile.value ? 'Total Points' : 'Pts',
        align: 'left',
        value: (row: any) => row.totalMatchFantasyPoints.toFixed(1),
        format: (val: number) => `${val.toLocaleString()}`,
        width: '50px',
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'pointsPerMatch',
        title: !display.mobile.value ? 'Avg Points' : 'FP/M',
        align: 'left',
        value: (row: any) => row.totalMatches > 1 ? (row.totalMatchFantasyPoints.toFixed(1) / row.totalMatches).toFixed(1) : 0,
        format: (val: number) => `${val.toLocaleString()}`,
        width: '50px',
        sortable: true,
        sort: (a: number, b: number) => a - b
    },
];
const kdaFantasyColumns = [
    {
        key: 'totalKills',
        title: !display.mobile.value ? 'Kills' : 'K',
        align: 'left',
        value: (row: any) => {
            return {
                kills: row.totalKills,
                killPoints: row.totalKillsPoints.toFixed(1)
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.killPoints - a.killPoints
    },
    {
        key: 'totalDeaths',
        title: !display.mobile.value ? 'Deaths' : 'D',
        align: 'left',
        value: (row: any) => {
            return {
                deaths: row.totalDeaths,
                deathPoints: row.totalDeathsPoints.toFixed(1)
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.deathPoints - a.deathPoints
    },
    {
        key: 'totalAssists',
        title: !display.mobile.value ? 'Assists' : 'A',
        align: 'left',
        value: (row: any) => {
            return {
                assists: row.totalAssists,
                assistPoints: row.totalAssistsPoints.toFixed(1)
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.assistPoints - a.assistPoints
    },
];
const farmFantasyColumns = [
    {
        key: 'totalLastHits',
        title: !display.mobile.value ? 'Last Hits' : 'LH',
        align: 'left',
        value: (row: any) => {
            return {
                lastHits: row.totalLastHits.toLocaleString(),
                lastHitsPoints: row.totalLastHitsPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.lastHitsPoints - a.lastHitsPoints
    },
    {
        key: 'totalGoldPerMin',
        title: !display.mobile.value ? 'Avg GPM' : 'G',
        align: 'left',
        value: (row: any) => {
            return {
                goldPerMin: row.avgGoldPerMin.toFixed(0).toLocaleString(),
                goldPerMinPoints: row.totalGoldPerMinPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.goldPerMinPoints - a.goldPerMinPoints
    },
    {
        key: 'totalXpPerMin',
        title: !display.mobile.value ? 'Avg XPM' : 'XP',
        align: 'left',
        value: (row: any) => {
            return {
                xpPerMin: row.avgXpPerMin.toFixed(0).toLocaleString(),
                xpPerMinPoints: row.totalXpPerMinPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.xpPerMinPoints - a.xpPerMinPoints
    },
];
const supportFantasyColumns = [
    {
        key: 'totalSupportGoldSpent',
        title: !display.mobile.value ? 'Supp. Gold Spent' : 'SG',
        align: 'left',
        value: (row: any) => {
            return {
                supportGoldSpent: row.totalSupportGoldSpent.toFixed(0).toLocaleString() ?? 0,
                supportGoldSpentPoints: row.totalSupportGoldSpentPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.supportGoldSpentPoints - a.supportGoldSpentPoints
    },
    {
        key: 'totalObsPlaced',
        title: !display.mobile.value ? 'Obs Placed' : 'OB',
        align: 'left',
        value: (row: any) => {
            return {
                observerWardsPlaced: row.totalObserverWardsPlaced.toFixed(0).toLocaleString() ?? 0,
                observerWardsPlacedPoints: row.totalObserverWardsPlacedPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.observerWardsPlacedPoints - a.observerWardsPlacedPoints
    },
    {
        key: 'totalSentriesPlaced',
        title: !display.mobile.value ? 'Sentries Placed' : 'SN',
        align: 'left',
        value: (row: any) => {
            return {
                sentryWardsPlaced: row.totalSentryWardsPlaced.toFixed(0).toLocaleString() ?? 0,
                sentryWardsPlacedPoints: row.totalSentryWardsPlacedPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.sentryWardsPlacedPoints - a.sentryWardsPlacedPoints
    },
    {
        key: 'totalWardsDewarded',
        title: !display.mobile.value ? 'Dewards' : 'DW',
        align: 'left',
        value: (row: any) => {
            return {
                wardsDewarded: row.totalWardsDewarded.toFixed(0).toLocaleString() ?? 0,
                wardsDewardedPoints: row.totalWardsDewardedPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.wardsDewardedPoints - a.wardsDewardedPoints
    },
    {
        key: 'totalCampsStacked',
        title: !display.mobile.value ? 'Camps Stacked' : 'C',
        align: 'left',
        value: (row: any) => {
            return {
                campsStacked: row.totalCampsStacked.toFixed(0).toLocaleString() ?? 0,
                campsStackedPoints: row.totalCampsStackedPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.campsStackedPoints - a.campsStackedPoints
    },
];
const damageHealingFantasyColumns = [
    {
        key: 'totalHeroDamage',
        title: !display.mobile.value ? 'Hero Dmg' : 'HD',
        align: 'left',
        value: (row: any) => {
            return {
                heroDamage: (row.totalHeroDamage / 1000).toFixed(1) + 'k',
                heroDamagePoints: row.totalHeroDamagePoints.toFixed(1)
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.heroDamagePoints - a.heroDamagePoints
    },
    {
        key: 'totalTowerDamage',
        title: !display.mobile.value ? 'Tower Dmg' : 'TD',
        align: 'left',
        value: (row: any) => {
            return {
                towerDamage: (row.totalTowerDamage / 1000).toFixed(1) + 'k',
                towerDamagePoints: row.totalTowerDamagePoints.toFixed(1)
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.towerDamagePoints - a.towerDamagePoints
    },
    {
        key: 'totalHeroHealing',
        title: !display.mobile.value ? 'Hero Healing' : 'HH',
        align: 'left',
        value: (row: any) => {
            return {
                heroHealing: (row.totalHeroHealing / 1000).toFixed(1) + 'k',
                heroHealingPoints: row.totalHeroHealingPoints.toFixed(1)
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.heroHealingPoints - a.heroHealingPoints
    },
    {
        key: 'totalStunDuration',
        title: !display.mobile.value ? 'Stun Dur.' : 'SD',
        align: 'left',
        value: (row: any) => {
            return {
                stunDuration: row.totalStunDuration.toFixed(1).toLocaleString() ?? 0,
                stunDurationPoints: row.totalStunDurationPoints.toFixed(1)
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.stunDurationPoints - a.stunDurationPoints
    },
];

// const selectedFantasyColumns = ref(commonFantasyColumns.map(column => column.name));

// const selectedFantasyPlayer = ref([]);
// const compareFantasyPlayers = ref([]);

// Define a function to stringify nested objects recursively
const stringifyNested = (obj: Object | string | null): any => {
    if (typeof obj !== 'object' || obj === null) {
        return String(obj);
    }
    return Object.values(obj)
        .map(val => stringifyNested(val))
        .join(' ');
};

// const selectFantasyRow = (selectedRow) => {
//     const index = selectedFantasyPlayer.value.findIndex(row => row.player === selectedRow.player);

//     if (index !== -1) {
//         selectedFantasyPlayer.value.splice(index, 1);
//     } else {
//         if (selectedFantasyPlayer.value.length < 2) {
//             selectedFantasyPlayer.value.push(selectedRow);
//         }
//     }
// };

// const clearSelectedFantasyPlayers = () => {
//     selectedFantasyPlayer.value = [];
// };

// const CompareFantasyPlayers = () => {
//     compareOn.value = !compareOn.value;
//     var fantasyTable = document.getElementById('fantasyTable');
//     fantasyTable.classList.toggle("collapsed");

//     var fantasyCompareTable = document.getElementById('fantasyCompareTable');
//     fantasyCompareTable.classList.toggle("collapsed");
//     compareFantasyPlayers.value = [...selectedFantasyPlayer.value];
//     clearSelectedFantasyPlayers();
// }

const playerFantasyStatsIndexed = computed(() => {
    return playerFantasyStats.value
        .map((player: FantasyPlayerPoints, index) => ({
            ...player,
            position: index + 1
        }))
        .filter(item => {
            if (!fantasyFilter.value) {
                return true;
            } else {
                // Search filter
                return Object.values(item.fantasyPlayer).some(val => stringifyNested(val).toLowerCase().includes(fantasyFilter.value.toLowerCase()))
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

const displayedFantasyColumns = computed<any>(() => {
    if (!display.mobile.value) {
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

const getPositionIcon = (positionInt: number) => {
    return `icons/pos_${positionInt}.png`
}
</script>

<style scoped>
.search-input {
    padding: 1rem;
    padding-top: 0;
}

.fantasy-table {
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
