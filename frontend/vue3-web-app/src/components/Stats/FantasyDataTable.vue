<!-- eslint-disable vue/valid-v-slot -->
<template>
    <v-col>
        <v-row v-if="!isDesktop" dense>
            <v-tabs v-model="fantasyTab" density="compact">
                <v-tab value="kda" min-width="70px" width="70px">K/D/A</v-tab>
                <v-tab value="farm" min-width="70px" width="70px">Farm</v-tab>
                <v-tab value="support" min-width="80px" width="80px">Supp.</v-tab>
                <v-tab value="damageHealing" min-width="100px" width="100px">Dmg/Heal</v-tab>
            </v-tabs>
        </v-row>
        <v-row>
            <v-data-table class="fantasy-table" :items="playerFantasyStatsIndexed" :headers="displayedFantasyColumns"
                density="compact" :items-per-page="itemsPerPage"
                :style="{ 'font-size': isDesktop ? '0.8rem' : '0.7rem' }">
                <template v-slot:item.fantasyPlayer="{ value }">
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
import { ref, defineModel, onMounted, watch, computed } from 'vue';
import { VRow, VCol, VDataTable, VPagination, VTabs, VTab } from 'vuetify/components';
import { localApiService } from '@/services/localApiService';
import type { League } from '@/stores/league';

const fantasyFilter = ref('');

const selectedLeague = defineModel<League>('selectedLeague');

const page = ref(1)
const itemsPerPage = 100;
const pageCount = computed(() => {
    return Math.ceil(playerFantasyStats.value.length / itemsPerPage);
})

onMounted(() => {
    if (selectedLeague.value) {
        localApiService.getPlayerFantasyStats(selectedLeague.value.id)
            .then(result => playerFantasyStats.value = result);
    }
});

watch(selectedLeague, () => {
    if (selectedLeague.value) {
        localApiService.getPlayerFantasyStats(selectedLeague.value.id)
            .then(result => playerFantasyStats.value = result);
    }
});

const fantasyTab = ref('kda');

const isDesktop = ref(window.outerWidth >= 600);

// const showFantasyFilters = ref(false);

const playerFantasyStats = ref([]);

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
        width: isDesktop.value ? '240px' : '120px',
        sortable: true,
        sort: (a: any, b: any) => a.playerName > b.playerName
    },
    {
        key: 'totalPoints',
        title: isDesktop.value ? 'Total Points' : 'Pts',
        align: 'left',
        value: (row: any) => row.totalMatchFantasyPoints.toFixed(1),
        format: (val: number) => `${val.toLocaleString()}`,
        width: '50px',
        sortable: true,
        sort: (a: number, b: number) => a - b
    },
];
const kdaFantasyColumns = [
    {
        key: 'totalKills',
        title: isDesktop.value ? 'Kills' : 'K',
        align: 'left',
        value: (row: any) => {
            return {
                kills: row.totalKills,
                killPoints: row.totalKillsPoints.toFixed(1)
            };
        },
        sortable: true,
        sort: (a: any, b: any) => a.killPoints - b.killPoints
    },
    {
        key: 'totalDeaths',
        title: isDesktop.value ? 'Deaths' : 'D',
        align: 'left',
        value: (row: any) => {
            return {
                deaths: row.totalDeaths,
                deathPoints: row.totalDeathsPoints.toFixed(1)
            };
        },
        sortable: true,
        sort: (a: any, b: any) => a.deathPoints - b.deathPoints
    },
    {
        key: 'totalAssists',
        title: isDesktop.value ? 'Assists' : 'A',
        align: 'left',
        value: (row: any) => {
            return {
                assists: row.totalAssists,
                assistPoints: row.totalAssistsPoints.toFixed(1)
            };
        },
        sortable: true,
        sort: (a: any, b: any) => a.assistPoints - b.assistPoints
    },
];
const farmFantasyColumns = [
    {
        key: 'totalLastHits',
        title: isDesktop.value ? 'Last Hits' : 'LH',
        align: 'left',
        value: (row: any) => {
            return {
                lastHits: row.totalLastHits.toLocaleString(),
                lastHitsPoints: row.totalLastHitsPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => a.lastHitsPoints - b.lastHitsPoints
    },
    {
        key: 'totalGoldPerMin',
        title: isDesktop.value ? 'Avg GPM' : 'G',
        align: 'left',
        value: (row: any) => {
            return {
                goldPerMin: row.avgGoldPerMin.toFixed(0).toLocaleString(),
                goldPerMinPoints: row.totalGoldPerMinPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => a.goldPerMinPoints - b.goldPerMinPoints
    },
    {
        key: 'totalXpPerMin',
        title: isDesktop.value ? 'Avg XPM' : 'XP',
        align: 'left',
        value: (row: any) => {
            return {
                xpPerMin: row.avgXpPerMin.toFixed(0).toLocaleString(),
                xpPerMinPoints: row.totalXpPerMinPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => a.xpPerMinPoints - b.xpPerMinPoints
    },
];
const supportFantasyColumns = [
    {
        key: 'totalSupportGoldSpent',
        title: isDesktop.value ? 'Supp. Gold Spent' : 'SG',
        align: 'left',
        value: (row: any) => {
            return {
                supportGoldSpent: row.totalSupportGoldSpent.toFixed(0).toLocaleString() ?? 0,
                supportGoldSpentPoints: row.totalSupportGoldSpentPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => a.supportGoldSpentPoints - b.supportGoldSpentPoints
    },
    {
        key: 'totalObsPlaced',
        title: isDesktop.value ? 'Obs Placed' : 'OB',
        align: 'left',
        value: (row: any) => {
            return {
                observerWardsPlaced: row.totalObserverWardsPlaced.toFixed(0).toLocaleString() ?? 0,
                observerWardsPlacedPoints: row.totalObserverWardsPlacedPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => a.observerWardsPlacedPoints - b.observerWardsPlacedPoints
    },
    {
        key: 'totalSentriesPlaced',
        title: isDesktop.value ? 'Sentries Placed' : 'SN',
        align: 'left',
        value: (row: any) => {
            return {
                sentryWardsPlaced: row.totalSentryWardsPlaced.toFixed(0).toLocaleString() ?? 0,
                sentryWardsPlacedPoints: row.totalSentryWardsPlacedPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => a.sentryWardsPlacedPoints - b.sentryWardsPlacedPoints
    },
    {
        key: 'totalWardsDewarded',
        title: isDesktop.value ? 'Dewards' : 'DW',
        align: 'left',
        value: (row: any) => {
            return {
                wardsDewarded: row.totalWardsDewarded.toFixed(0).toLocaleString() ?? 0,
                wardsDewardedPoints: row.totalWardsDewardedPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => a.wardsDewardedPoints - b.wardsDewardedPoints
    },
    {
        key: 'totalCampsStacked',
        title: isDesktop.value ? 'Camps Stacked' : 'C',
        align: 'left',
        value: (row: any) => {
            return {
                campsStacked: row.totalCampsStacked.toFixed(0).toLocaleString() ?? 0,
                campsStackedPoints: row.totalCampsStackedPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => a.campsStackedPoints - b.campsStackedPoints
    },
];
const damageHealingFantasyColumns = [
    {
        key: 'totalHeroDamage',
        title: isDesktop.value ? 'Hero Dmg' : 'HD',
        align: 'left',
        value: (row: any) => {
            return {
                heroDamage: (row.totalHeroDamage / 1000).toFixed(1) + 'k' ?? '0',
                heroDamagePoints: row.totalHeroDamagePoints.toFixed(1)
            };
        },
        sortable: true,
        sort: (a: any, b: any) => a.heroDamagePoints - b.heroDamagePoints
    },
    {
        key: 'totalTowerDamage',
        title: isDesktop.value ? 'Tower Dmg' : 'TD',
        align: 'left',
        value: (row: any) => {
            return {
                towerDamage: (row.totalTowerDamage / 1000).toFixed(1) + 'k' ?? '0',
                towerDamagePoints: row.totalTowerDamagePoints.toFixed(1)
            };
        },
        sortable: true,
        sort: (a: any, b: any) => a.towerDamagePoints - b.towerDamagePoints
    },
    {
        key: 'totalHeroHealing',
        title: isDesktop.value ? 'Hero Healing' : 'HH',
        align: 'left',
        value: (row: any) => {
            return {
                heroHealing: (row.totalHeroHealing / 1000).toFixed(1) + 'k' ?? '0',
                heroHealingPoints: row.totalHeroHealingPoints.toFixed(1)
            };
        },
        sortable: true,
        sort: (a: any, b: any) => a.heroHealingPoints - b.heroHealingPoints
    },
    {
        key: 'totalStunDuration',
        title: isDesktop.value ? 'Stun Dur.' : 'SD',
        align: 'left',
        value: (row: any) => {
            return {
                stunDuration: row.totalStunDuration.toFixed(1).toLocaleString() ?? 0,
                stunDurationPoints: row.totalStunDurationPoints.toFixed(1)
            };
        },
        sortable: true,
        sort: (a: any, b: any) => a.stunDurationPoints - b.stunDurationPoints
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
        .map((player: Object, index) => ({
            ...player,
            position: index + 1
        })).filter(item =>
            Object.values(item).some(val => stringifyNested(val).toLowerCase().includes(fantasyFilter.value.toLowerCase())
            ));
});

const displayedFantasyColumns = computed<any>(() => {
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

const getPositionIcon = (positionInt: number) => {
    return `icons/pos_${positionInt}.png`
}
</script>

<style scoped>
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
