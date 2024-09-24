<!-- eslint-disable vue/valid-v-slot -->
<template>
    <v-col>
        <v-row class="search-input">
            <v-col style="min-width: 200px">
                <v-text-field v-model="matchFilter" label="Search" prepend-inner-icon="fa-magnifying-glass"
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
        <v-row v-if="!isDesktop" dense>
            <v-tabs v-model="fantasyTab" density="compact">
                <v-tab value="kda" min-width="70px" width="70px">K/D/A</v-tab>
                <v-tab value="farm" min-width="70px" width="70px">Farm</v-tab>
                <v-tab value="support" min-width="80px" width="80px">Supp.</v-tab>
                <v-tab value="damageHealing" min-width="100px" width="100px">Dmg/Heal</v-tab>
            </v-tabs>
        </v-row>
        <v-row>
            <v-data-table class="match-table" :items="playerFantasyMatchStatsIndexed" :headers="displayedFantasyColumns"
                :group-by="draftFiltered ? undefined : groupBy" items-per-page="110" density="compact"
                hide-default-footer :style="{ 'font-size': isDesktop ? '0.8rem' : '0.7rem' }">
                <template v-if="!draftFiltered" v-slot:group-header="{ item, columns, toggleGroup, isGroupOpen }">
                    <tr>
                        <td :colspan="columns.length">
                            <VBtn :icon="isGroupOpen(item) ? '$expand' : '$next'" size="small" variant="text"
                                @click="toggleGroup(item)"></VBtn>
                            Match - {{ item.value }} - {{ getMatchTeams(item.value) }}
                        </td>
                    </tr>
                </template>
                <template v-slot:header.data-table-group>
                    <span></span>
                </template>
                <template v-slot:item.fantasyPlayer="{ value }">
                    <v-row v-if="isDesktop" class="ma-1 pa-1">
                        <v-col class="mr-2" style="max-width:70px;width:70px;">
                            <v-row>
                                <img height="70px" width="70px" :src="value.playerPicture" />
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
                            <v-row v-if="draftFiltered">
                                <span>Match {{ value.matchId }}</span>
                            </v-row>
                            <v-row>
                                <img :style="{ width: '60px', height: '30px' }" :src="getHeroIcon(value.heroName)" />
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
                                <img :style="{ width: '60px', height: '30px' }" :src="getHeroIcon(value.heroName)" />
                            </v-row>
                        </v-col>
                    </v-row>
                </template>

                <template v-slot:item.totalPoints="{ value }">
                    <b>{{ value }}</b>
                </template>
                <template v-slot:item.kills="{ value }">
                    <b>{{ value.killPoints }}</b>
                    <br>
                    ({{ value.kills }})
                </template>
                <template v-slot:item.deaths="{ value }">
                    <b>{{ value.deathPoints }}</b>
                    <br>
                    ({{ value.deaths }})
                </template>
                <template v-slot:item.assists="{ value }">
                    <b>{{ value.assistPoints }}</b>
                    <br>
                    ({{ value.assists }})
                </template>
                <template v-slot:item.lastHits="{ value }">
                    <b>{{ value.lastHitsPoints }}</b>
                    <br>
                    ({{ value.lastHits }})
                </template>
                <template v-slot:item.goldPerMin="{ value }">
                    <b>{{ value.goldPerMinPoints }}</b>
                    <br>
                    ({{ value.goldPerMin }})
                </template>
                <template v-slot:item.xpPerMin="{ value }">
                    <b>{{ value.xpPerMinPoints }}</b>
                    <br>
                    ({{ value.xpPerMin }})
                </template>
                <template v-slot:item.supportGoldSpent="{ value }">
                    <b>{{ value.supportGoldSpentPoints }}</b>
                    <br>
                    ({{ (value.supportGoldSpent / 1000).toFixed(1) + 'k' }})
                </template>
                <template v-slot:item.obsPlaced="{ value }">
                    <b>{{ value.observerWardsPlacedPoints }}</b>
                    <br>
                    ({{ value.observerWardsPlaced }})
                </template>
                <template v-slot:item.sentriesPlaced="{ value }">
                    <b>{{ value.sentryWardsPlacedPoints }}</b>
                    <br>
                    ({{ value.sentryWardsPlaced }})
                </template>
                <template v-slot:item.wardsDewarded="{ value }">
                    <b>{{ value.wardsDewardedPoints }}</b>
                    <br>
                    ({{ value.wardsDewarded }})
                </template>
                <template v-slot:item.campsStacked="{ value }">
                    <b>{{ value.campsStackedPoints }}</b>
                    <br>
                    ({{ value.campsStacked }})
                </template>
                <template v-slot:item.heroDamage="{ value }">
                    <b>{{ value.heroDamagePoints }}</b>
                    <br>
                    ({{ value.heroDamage }})
                </template>
                <template v-slot:item.towerDamage="{ value }">
                    <b>{{ value.towerDamagePoints }}</b>
                    <br>
                    ({{ value.towerDamage }})
                </template>
                <template v-slot:item.heroHealing="{ value }">
                    <b>{{ value.heroHealingPoints }}</b>
                    <br>
                    ({{ value.heroHealing }})
                </template>
                <template v-slot:item.stunDuration="{ value }">
                    <b>{{ value.stunDurationPoints }}</b>
                    <br>
                    ({{ value.stunDuration }})
                </template>
            </v-data-table>
        </v-row>
    </v-col>

</template>

<script setup lang="ts">
import { ref, defineModel, onMounted, watch, computed } from 'vue';
import { VRow, VCol, VDataTable, VTabs, VTab, VTextField, VSelect } from 'vuetify/components';
import { localApiService } from '@/services/localApiService';
import type { FantasyLeague } from '@/types/FantasyLeague';
import type { FantasyPlayerMatchPoints } from '../Fantasy/fantasyDraft';
import { useDebouncedRef } from '@/services/debounce'

const matchFilter = useDebouncedRef('');
const roleFilter = ref([]);
const teamFilter = ref([]);
const groupBy = <VDataTable['groupBy']>[
    {
        key: 'fantasyMatchPlayer.fantasyMatchId',
        order: 'desc'
    }
];

const selectedFantasyLeague = defineModel<FantasyLeague>('selectedFantasyLeague');
const draftFiltered = defineModel<boolean>('draftFiltered');

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
    var teams = playerFantasyMatchStats.value.map(item => ({id: item.fantasyPlayer.teamId, ...item.fantasyPlayer.team}))
    return [...new Map(teams.map(item => [item.id, item])).values()]
})

onMounted(() => {
    if (selectedFantasyLeague.value && !draftFiltered.value) {
        localApiService.getPlayerFantasyMatchStats(selectedFantasyLeague.value.id)
            .then(result => playerFantasyMatchStats.value = result);
    } else if (selectedFantasyLeague.value && draftFiltered.value) {
        localApiService.getDraftPlayerFantasyMatchStats(selectedFantasyLeague.value.id)
            .then(result => playerFantasyMatchStats.value = result);
    }
});

watch(selectedFantasyLeague, () => {
    if (selectedFantasyLeague.value && !draftFiltered.value) {
        localApiService.getPlayerFantasyMatchStats(selectedFantasyLeague.value.id)
            .then(result => playerFantasyMatchStats.value = result);
    } else if (selectedFantasyLeague.value && draftFiltered.value) {
        localApiService.getDraftPlayerFantasyMatchStats(selectedFantasyLeague.value.id)
            .then(result => playerFantasyMatchStats.value = result);
    }
});

const fantasyTab = ref('kda');

const isDesktop = ref(window.outerWidth >= 600);

// const showFantasyFilters = ref(false);

const playerFantasyMatchStats = ref<FantasyPlayerMatchPoints[]>([]);

const commonFantasyColumns = [
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
                heroName: row.fantasyMatchPlayer.hero.name,
                matchId: row.fantasyMatchPlayer.matchId
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
        sort: (a: number, b: number) => b - a
    },
];
const kdaFantasyColumns = [
    {
        key: 'kills',
        title: isDesktop.value ? 'Kills' : 'K',
        align: 'left',
        value: (row: any) => {
            return {
                kills: row.kills,
                killPoints: row.killsPoints.toFixed(1)
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.killPoints - a.killPoints
    },
    {
        key: 'deaths',
        title: isDesktop.value ? 'Deaths' : 'D',
        align: 'left',
        value: (row: any) => {
            return {
                deaths: row.deaths,
                deathPoints: row.deathsPoints.toFixed(1)
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.deathPoints - a.deathPoints
    },
    {
        key: 'assists',
        title: isDesktop.value ? 'Assists' : 'A',
        align: 'left',
        value: (row: any) => {
            return {
                assists: row.assists,
                assistPoints: row.assistsPoints.toFixed(1)
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.assistPoints - a.assistPoints
    },
];
const farmFantasyColumns = [
    {
        key: 'lastHits',
        title: isDesktop.value ? 'Last Hits' : 'LH',
        align: 'left',
        value: (row: any) => {
            return {
                lastHits: row.lastHits.toLocaleString(),
                lastHitsPoints: row.lastHitsPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.lastHitsPoints - a.lastHitsPoints
    },
    {
        key: 'goldPerMin',
        title: isDesktop.value ? 'Avg GPM' : 'G',
        align: 'left',
        value: (row: any) => {
            return {
                goldPerMin: row.goldPerMin.toFixed(0).toLocaleString(),
                goldPerMinPoints: row.goldPerMinPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.goldPerMinPoints - a.goldPerMinPoints
    },
    {
        key: 'xpPerMin',
        title: isDesktop.value ? 'Avg XPM' : 'XP',
        align: 'left',
        value: (row: any) => {
            return {
                xpPerMin: row.xpPerMin.toFixed(0).toLocaleString(),
                xpPerMinPoints: row.xpPerMinPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.xpPerMinPoints - a.xpPerMinPoints
    },
];
const supportFantasyColumns = [
    {
        key: 'supportGoldSpent',
        title: isDesktop.value ? 'Supp. Gold Spent' : 'SG',
        align: 'left',
        value: (row: any) => {
            return {
                supportGoldSpent: row.supportGoldSpent.toFixed(0).toLocaleString() ?? 0,
                supportGoldSpentPoints: row.supportGoldSpentPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.supportGoldSpentPoints - a.supportGoldSpentPoints
    },
    {
        key: 'obsPlaced',
        title: isDesktop.value ? 'Obs Placed' : 'OB',
        align: 'left',
        value: (row: any) => {
            return {
                observerWardsPlaced: row.observerWardsPlaced.toFixed(0).toLocaleString() ?? 0,
                observerWardsPlacedPoints: row.observerWardsPlacedPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.observerWardsPlacedPoints - a.observerWardsPlacedPoints
    },
    {
        key: 'sentriesPlaced',
        title: isDesktop.value ? 'Sentries Placed' : 'SN',
        align: 'left',
        value: (row: any) => {
            return {
                sentryWardsPlaced: row.sentryWardsPlaced.toFixed(0).toLocaleString() ?? 0,
                sentryWardsPlacedPoints: row.sentryWardsPlacedPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.sentryWardsPlacedPoints - a.sentryWardsPlacedPoints
    },
    {
        key: 'wardsDewarded',
        title: isDesktop.value ? 'Dewards' : 'DW',
        align: 'left',
        value: (row: any) => {
            return {
                wardsDewarded: row.wardsDewarded.toFixed(0).toLocaleString() ?? 0,
                wardsDewardedPoints: row.wardsDewardedPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.wardsDewardedPoints - a.wardsDewardedPoints
    },
    {
        key: 'campsStacked',
        title: isDesktop.value ? 'Camps Stacked' : 'C',
        align: 'left',
        value: (row: any) => {
            return {
                campsStacked: row.campsStacked.toFixed(0).toLocaleString() ?? 0,
                campsStackedPoints: row.campsStackedPoints.toFixed(1).toLocaleString()
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.campsStackedPoints - a.campsStackedPoints
    },
];
const damageHealingFantasyColumns = [
    {
        key: 'heroDamage',
        title: isDesktop.value ? 'Hero Dmg' : 'HD',
        align: 'left',
        value: (row: any) => {
            return {
                heroDamage: (row.heroDamage / 1000).toFixed(1) + 'k',
                heroDamagePoints: row.heroDamagePoints.toFixed(1)
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.heroDamagePoints - a.heroDamagePoints
    },
    {
        key: 'towerDamage',
        title: isDesktop.value ? 'Tower Dmg' : 'TD',
        align: 'left',
        value: (row: any) => {
            return {
                towerDamage: (row.towerDamage / 1000).toFixed(1) + 'k',
                towerDamagePoints: row.towerDamagePoints.toFixed(1)
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.towerDamagePoints - a.towerDamagePoints
    },
    {
        key: 'heroHealing',
        title: isDesktop.value ? 'Hero Healing' : 'HH',
        align: 'left',
        value: (row: any) => {
            return {
                heroHealing: (row.heroHealing / 1000).toFixed(1) + 'k',
                heroHealingPoints: row.heroHealingPoints.toFixed(1)
            };
        },
        sortable: true,
        sort: (a: any, b: any) => b.heroHealingPoints - a.heroHealingPoints
    },
    {
        key: 'stunDuration',
        title: isDesktop.value ? 'Stun Dur.' : 'SD',
        align: 'left',
        value: (row: any) => {
            return {
                stunDuration: row.stunDuration.toFixed(1).toLocaleString() ?? 0,
                stunDurationPoints: row.stunDurationPoints.toFixed(1)
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

const getMatchTeams = (matchId: number): string => {
    var radiantTeam = playerFantasyMatchStats.value.filter(match => match.fantasyMatchPlayer.fantasyMatchId == matchId && match.fantasyMatchPlayer.dotaTeamSide == false)[0]?.fantasyMatchPlayer.teamFormatted ?? '';
    var direTeam = playerFantasyMatchStats.value.filter(match => match.fantasyMatchPlayer.fantasyMatchId == matchId && match.fantasyMatchPlayer.dotaTeamSide == true)[0]?.fantasyMatchPlayer.teamFormatted ?? '';
    return `${radiantTeam} (Radiant) vs ${direTeam} (Dire)`
}

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

const playerFantasyMatchStatsIndexed = computed(() => {
    return playerFantasyMatchStats.value
        .map((player: FantasyPlayerMatchPoints, index) => ({
            ...player,
            position: index + 1
        }))
        .filter(item => {
            if (!matchFilter.value) {
                return true;
            } else {
                // Search filter
                return Object.values(item.fantasyPlayer).some(val => stringifyNested(val).toLowerCase().includes(matchFilter.value.toLowerCase()))
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

const getHeroIcon = (heroIconString: string) => {
    if (heroIconString == '') return undefined;
    var formattedString = heroIconString.replace('npc_dota_hero_', '');
    return `https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/${formattedString}.png`
}

</script>

<style scoped>
.search-input {
    padding: 1rem;
    padding-top: 0;
}

.match-table {
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
