<!-- eslint-disable vue/valid-v-slot -->
<template>
    <v-col>
        <StatsFilterBar :role-filter="roleFilter" :team-filter="teamFilter" :teams="teamsList" :search="fantasyFilter"
            @update:role-filter="roleFilter = $event" @update:team-filter="teamFilter = $event"
            @update:search="fantasyFilter = $event" />
        <v-row v-if="display.mobile.value" dense>
            <v-tabs v-model="fantasyTab" density="compact">
                <v-tab value="kda" min-width="70px" width="70px">K/D/A</v-tab>
                <v-tab value="farm" min-width="70px" width="70px">Farm</v-tab>
                <v-tab value="support" min-width="80px" width="80px">Supp.</v-tab>
                <v-tab value="damageHealing" min-width="100px" width="100px">Dmg/Heal</v-tab>
            </v-tabs>
        </v-row>
        <div class="table-scroll-wrap" ref="tableRoot">
            <v-data-table class="fantasy-table" :items="playerFantasyStatsIndexed" :headers="displayedFantasyColumns"
                density="compact" :items-per-page="itemsPerPage" v-model:page="page"
                :style="{ 'font-size': !display.mobile.value ? '0.8rem' : '0.7rem' }">

                <template #headers="{ columns, isSorted, getSortIcon, toggleSort }">
                    <tr v-if="!display.mobile.value" class="group-band-row">
                        <th colspan="4" class="group-fixed" />
                        <th colspan="3" class="group-kda">K / D / A</th>
                        <th colspan="3" class="group-farm">Farm</th>
                        <th colspan="5" class="group-support">Support</th>
                        <th colspan="4" class="group-damage">Damage</th>
                    </tr>
                    <tr class="col-header-row">
                        <th v-for="col in columns" :key="col.key ?? ''"
                            :class="['col-header', colGroupClass(col.key ?? ''), { sortable: col.sortable }]"
                            @click="col.sortable ? toggleSort(col) : undefined">
                            <span>{{ col.title }}</span>
                            <v-icon v-if="col.sortable && isSorted(col)" :icon="getSortIcon(col)" size="x-small"
                                class="sort-icon" />
                        </th>
                    </tr>
                </template>
                <template v-slot:item.fantasyPlayer="{ value }">
                    <div class="player-cell">
                        <img v-if="!display.mobile.value" :src="value.playerPicture" class="player-avatar" />
                        <div class="player-info">
                            <span class="player-name">{{ value.playerName }}</span>
                            <span class="player-meta">
                                {{ value.teamName }}
                                <img :src="getPositionIcon(value.teamPosition)" class="player-pos-icon" />
                                · {{ value.totalMatches }} games
                            </span>
                        </div>
                    </div>
                </template>

                <template v-slot:item.totalPoints="{ value }">
                    <span class="total-pts">{{ value }}</span>
                </template>
                <template v-slot:item.pointsPerMatch="{ value }">
                    <span class="avg-pts">{{ value }}</span>
                </template>
                <template v-slot:item.totalKills="{ value }">
                    <StatCell :pts="value.killPoints" :raw="`(${value.kills})`"
                        :tier="getTier(value.killPoints, 'totalKills')" />
                </template>
                <template v-slot:item.totalDeaths="{ value }">
                    <StatCell :pts="value.deathPoints" :raw="`(${value.deaths})`"
                        :tier="getTier(value.deathPoints, 'totalDeaths')" />
                </template>
                <template v-slot:item.totalAssists="{ value }">
                    <StatCell :pts="value.assistPoints" :raw="`(${value.assists})`"
                        :tier="getTier(value.assistPoints, 'totalAssists')" />
                </template>
                <template v-slot:item.totalLastHits="{ value }">
                    <StatCell :pts="value.lastHitsPoints" :raw="`(${value.lastHits})`"
                        :tier="getTier(value.lastHitsPoints, 'totalLastHits')" />
                </template>
                <template v-slot:item.totalGoldPerMin="{ value }">
                    <StatCell :pts="value.goldPerMinPoints" :raw="`(${value.goldPerMin})`"
                        :tier="getTier(value.goldPerMinPoints, 'totalGoldPerMin')" />
                </template>
                <template v-slot:item.totalXpPerMin="{ value }">
                    <StatCell :pts="value.xpPerMinPoints" :raw="`(${value.xpPerMin})`"
                        :tier="getTier(value.xpPerMinPoints, 'totalXpPerMin')" />
                </template>
                <template v-slot:item.totalSupportGoldSpent="{ value }">
                    <StatCell :pts="value.supportGoldSpentPoints"
                        :raw="`(${(value.supportGoldSpent / 1000).toFixed(1)}k)`"
                        :tier="getTier(value.supportGoldSpentPoints, 'totalSupportGoldSpent')" />
                </template>
                <template v-slot:item.totalObsPlaced="{ value }">
                    <StatCell :pts="value.observerWardsPlacedPoints" :raw="`(${value.observerWardsPlaced})`"
                        :tier="getTier(value.observerWardsPlacedPoints, 'totalObsPlaced')" />
                </template>
                <template v-slot:item.totalSentriesPlaced="{ value }">
                    <StatCell :pts="value.sentryWardsPlacedPoints" :raw="`(${value.sentryWardsPlaced})`"
                        :tier="getTier(value.sentryWardsPlacedPoints, 'totalSentriesPlaced')" />
                </template>
                <template v-slot:item.totalWardsDewarded="{ value }">
                    <StatCell :pts="value.wardsDewardedPoints" :raw="`(${value.wardsDewarded})`"
                        :tier="getTier(value.wardsDewardedPoints, 'totalWardsDewarded')" />
                </template>
                <template v-slot:item.totalCampsStacked="{ value }">
                    <StatCell :pts="value.campsStackedPoints" :raw="`(${value.campsStacked})`"
                        :tier="getTier(value.campsStackedPoints, 'totalCampsStacked')" />
                </template>
                <template v-slot:item.totalHeroDamage="{ value }">
                    <StatCell :pts="value.heroDamagePoints" :raw="`(${value.heroDamage})`"
                        :tier="getTier(value.heroDamagePoints, 'totalHeroDamage')" />
                </template>
                <template v-slot:item.totalTowerDamage="{ value }">
                    <StatCell :pts="value.towerDamagePoints" :raw="`(${value.towerDamage})`"
                        :tier="getTier(value.towerDamagePoints, 'totalTowerDamage')" />
                </template>
                <template v-slot:item.totalHeroHealing="{ value }">
                    <StatCell :pts="value.heroHealingPoints" :raw="`(${value.heroHealing})`"
                        :tier="getTier(value.heroHealingPoints, 'totalHeroHealing')" />
                </template>
                <template v-slot:item.totalStunDuration="{ value }">
                    <StatCell :pts="value.stunDurationPoints" :raw="`(${value.stunDuration})`"
                        :tier="getTier(value.stunDurationPoints, 'totalStunDuration')" />
                </template>
                <template v-slot:bottom>
                    <div class="text-center pt-2">
                        <v-pagination v-model="page" :length="pageCount"></v-pagination>
                    </div>
                </template>
            </v-data-table>
        </div>
    </v-col>

</template>

<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue';
import { VCol, VDataTable, VPagination, VTabs, VTab, VIcon, VRow } from 'vuetify/components';
import StatCell from '@/components/Stats/StatCell.vue';
import type { FantasyPlayerPoints } from '../Fantasy/fantasyDraft';
import { useDebouncedRef } from '@/services/debounce'
import type { FantasyLeague } from '@/types/FantasyLeague';
import { useDisplay } from 'vuetify';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import StatsFilterBar from '@/components/Stats/StatsFilterBar.vue';
import { useDragToScroll } from '@/services/useDragToScroll';
import { useStatTiers } from '@/services/useStatTiers';
import { getPositionIcon } from '@/services/iconService';

const display = useDisplay()

const fantasyLeagueStore = useFantasyLeagueStore()

const fantasyFilter = useDebouncedRef('');
const roleFilter = ref<number[]>([]);
const teamFilter = ref<number[]>([]);

const selectedFantasyLeague = defineModel<FantasyLeague>('selectedFantasyLeague');

const page = ref(1)
const itemsPerPage = 15;
const pageCount = computed(() => {
    return Math.ceil(playerFantasyStatsIndexed.value.length / itemsPerPage);
})

const teamsList = computed(() => {
    // We want the distinct teams
    var teams = fantasyLeagueStore.fantasyPlayerPoints.map(item => item.fantasyPlayer.team)
    return [...new Map(teams.map(item => [item.id, item])).values()]
})

onMounted(() => {
    if (selectedFantasyLeague.value && selectedFantasyLeague.value.id != 0) {
        if (!fantasyLeagueStore.fantasyPlayerPoints)
            fantasyLeagueStore.fetchFantasyPlayerPoints()
    }
});

watch(selectedFantasyLeague, () => {
    if (selectedFantasyLeague.value && selectedFantasyLeague.value.id != 0) {
        fantasyLeagueStore.fetchFantasyPlayerPoints()
    }
});

const fantasyTab = ref('kda');

// Column size tiers: S = small (1-2 digit counts), M = medium (3-4 digit), L = large (formatted values like 123.5k)
const COL_S = { width: '75px', align: 'center' } as const
const COL_M = { width: '90px', align: 'center' } as const
const COL_L = { width: '100px', align: 'center' } as const

const commonFantasyColumns = [
    {
        key: 'fantasyPlayerRank',
        title: '',
        align: 'center',
        value: (row: any) => row.position,
        width: '40px',
        sortable: true
    },
    {
        key: 'fantasyPlayer',
        title: 'Player',
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
        width: !display.mobile.value ? '220px' : '120px',
        sortable: true,
        sort: (a: any, b: any) => a.playerName > b.playerName
    },
    {
        key: 'totalPoints',
        title: 'TOT PTS',
        align: 'left',
        value: (row: any) => row.totalMatchFantasyPoints.toFixed(1),
        format: (val: number) => `${val.toLocaleString()}`,
        width: '80px',
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
    {
        key: 'pointsPerMatch',
        title: 'AVG',
        align: 'left',
        value: (row: any) => row.totalMatches > 0 ? (row.totalMatchFantasyPoints.toFixed(1) / row.totalMatches).toFixed(1) : 0,
        format: (val: number) => `${val.toLocaleString()}`,
        width: '70px',
        sortable: true,
        sort: (a: number, b: number) => a - b
    },
];
const kdaFantasyColumns = [
    {
        key: 'totalKills',
        title: !display.mobile.value ? 'Kills' : 'K',
        ...COL_S,
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
        ...COL_S,
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
        ...COL_S,
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
        ...COL_M,
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
        ...COL_M,
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
        ...COL_M,
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
        title: !display.mobile.value ? 'Supp Gold' : 'SG',
        ...COL_M,
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
        title: !display.mobile.value ? 'Obs' : 'OB',
        ...COL_S,
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
        title: !display.mobile.value ? 'Sentries' : 'SN',
        ...COL_S,
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
        ...COL_S,
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
        title: !display.mobile.value ? 'Camps' : 'C',
        ...COL_S,
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
        ...COL_L,
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
        ...COL_L,
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
        title: !display.mobile.value ? 'Hero Heal' : 'HH',
        ...COL_L,
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
        title: !display.mobile.value ? 'Stuns' : 'SD',
        ...COL_M,
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

// Define a function to stringify nested objects recursively
const stringifyNested = (obj: Object | string | null): any => {
    if (typeof obj !== 'object' || obj === null) {
        return String(obj);
    }
    return Object.values(obj)
        .map(val => stringifyNested(val))
        .join(' ');
};

const playerFantasyStatsIndexed = computed(() => {
    return fantasyLeagueStore.fantasyPlayerPoints
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

const { getTier } = useStatTiers(playerFantasyStatsIndexed)

const KDA_KEYS = new Set(['totalKills', 'totalDeaths', 'totalAssists'])
const FARM_KEYS = new Set(['totalLastHits', 'totalGoldPerMin', 'totalXpPerMin'])
const SUPPORT_KEYS = new Set(['totalSupportGoldSpent', 'totalObsPlaced', 'totalSentriesPlaced', 'totalWardsDewarded', 'totalCampsStacked'])
const DAMAGE_KEYS = new Set(['totalHeroDamage', 'totalTowerDamage', 'totalHeroHealing', 'totalStunDuration'])

const colGroupClass = (key: string): string => {
    if (KDA_KEYS.has(key)) return 'col-kda'
    if (FARM_KEYS.has(key)) return 'col-farm'
    if (SUPPORT_KEYS.has(key)) return 'col-support'
    if (DAMAGE_KEYS.has(key)) return 'col-damage'
    return 'col-fixed'
}

const { scrollEl: tableRoot } = useDragToScroll()
</script>

<style scoped>
@import '@/assets/data-tables.css';

/* Group band row */
.group-band-row th {
    height: 22px;
    font-family: var(--font-heading);
    font-size: var(--text-xs);
    font-weight: 700;
    letter-spacing: 4px;
    text-align: center !important;
    border-bottom: none;
}

.group-fixed {
    background: var(--rune-bg-surface);
}

.group-kda {
    background: color-mix(in srgb, var(--rune-gold-dark) 6%, transparent);
    color: color-mix(in srgb, var(--rune-gold-dark) 80%, transparent);
    border-bottom: 2px solid color-mix(in srgb, var(--rune-gold-dark) 20%, transparent);
}

.group-farm {
    background: color-mix(in srgb, var(--rune-green) 6%, transparent);
    color: color-mix(in srgb, var(--rune-green) 80%, transparent);
    border-bottom: 2px solid color-mix(in srgb, var(--rune-green) 20%, transparent);
}

.group-support {
    background: color-mix(in srgb, var(--rune-blue-light) 6%, transparent);
    color: color-mix(in srgb, var(--rune-blue-light) 80%, transparent);
    border-bottom: 2px solid color-mix(in srgb, var(--rune-blue-light) 20%, transparent);
}

.group-damage {
    background: color-mix(in srgb, var(--rune-red) 6%, transparent);
    color: color-mix(in srgb, var(--rune-red) 80%, transparent);
    border-bottom: 2px solid color-mix(in srgb, var(--rune-red) 20%, transparent);
}

/* Column header row */
.col-header-row .col-header {
    height: 32px;
    font-family: var(--font-heading);
    font-size: var(--text-xs);
    font-weight: 700;
    letter-spacing: 1px;
    text-align: center;
    white-space: nowrap;
    background: var(--rune-bg-surface);
    cursor: default;
}

.col-header.sortable {
    cursor: pointer;
}

.col-header.sortable:hover {
    opacity: 0.8;
}

.col-fixed {
    color: color-mix(in srgb, var(--rune-gold-dark) 80%, transparent);
}

.col-kda {
    color: color-mix(in srgb, var(--rune-gold-dark) 60%, transparent);
}

.col-farm {
    color: color-mix(in srgb, var(--rune-green) 60%, transparent);
}

.col-support {
    color: color-mix(in srgb, var(--rune-blue-light) 60%, transparent);
}

.col-damage {
    color: color-mix(in srgb, var(--rune-red) 60%, transparent);
}

.sort-icon {
    margin-left: 4px;
}

/* Scroll container */
.table-scroll-wrap {
    width: 100%;
    overflow-x: auto;
    cursor: grab;
}

.table-scroll-wrap:active {
    cursor: grabbing;
}

.fantasy-table {
    min-width: 1760px;
}

/*
 * Sticky column left offsets — update these if column widths change:
 *   col-1: rank (40px)
 *   col-2: player (226px = 40 + 226... actual user-adjusted value)
 *   col-3: TOT PTS (80px)
 *   col-4: AVG (70px) — last frozen, gets shadow
 */
.fantasy-table {
    --sticky-2: 40px;
    --sticky-3: 266px;
    --sticky-4: 346px;
    --frozen-shadow: 4px 0 12px color-mix(in srgb, var(--ot-bg-deep) 60%, transparent);
}

/* Disable internal scroll so .table-scroll-wrap is the scroll container */
.fantasy-table :deep(.v-table__wrapper) {
    overflow: visible;
}

/* Group band row: only the first th (colspan=4 frozen area) is sticky */
.fantasy-table :deep(tr.group-band-row > th:nth-child(1)) {
    position: sticky;
    left: 0;
    z-index: 3;
    background-color: var(--rune-bg-surface);
}

/* Column header row: freeze first 4 individual th cells */
.fantasy-table :deep(tr.col-header-row > th:nth-child(1)) {
    position: sticky;
    left: 0;
    z-index: 3;
    background-color: var(--rune-bg-surface);
}

.fantasy-table :deep(tr.col-header-row > th:nth-child(2)) {
    position: sticky;
    left: var(--sticky-2);
    z-index: 3;
    background-color: var(--rune-bg-surface);
    text-align: left;
}

.fantasy-table :deep(tr.col-header-row > th:nth-child(3)) {
    position: sticky;
    left: var(--sticky-3);
    z-index: 3;
    background-color: var(--rune-bg-surface);
    text-align: center;
}

.fantasy-table :deep(tr.col-header-row > th:nth-child(4)) {
    position: sticky;
    left: var(--sticky-4);
    z-index: 3;
    background-color: var(--rune-bg-surface);
    box-shadow: var(--frozen-shadow);
    text-align: center;
}

/* Body rows: freeze first 4 td cells */
.fantasy-table :deep(.v-table__wrapper > table > tbody > tr > td:nth-child(1)) {
    position: sticky;
    left: 0;
    z-index: 2;
}

.fantasy-table :deep(.v-table__wrapper > table > tbody > tr > td:nth-child(2)) {
    position: sticky;
    left: var(--sticky-2);
    z-index: 2;
}

.fantasy-table :deep(.v-table__wrapper > table > tbody > tr > td:nth-child(3)) {
    position: sticky;
    left: var(--sticky-3);
    z-index: 2;
    text-align: center;
}

.fantasy-table :deep(.v-table__wrapper > table > tbody > tr > td:nth-child(4)) {
    position: sticky;
    left: var(--sticky-4);
    z-index: 2;
    box-shadow: var(--frozen-shadow);
    text-align: center;
}

/* Collapse table borders to eliminate any spacing gaps between sticky cells */
.fantasy-table :deep(.v-table__wrapper > table) {
    border-collapse: collapse;
    border-spacing: 0;
}

/* Row styling */
.fantasy-table :deep(tbody tr) {
    height: 60px;
}

.fantasy-table :deep(tbody td) {
    border-bottom: 1px solid var(--ot-border-dim) !important;
}

/* Sticky cells: suppress the td border and use the row border only, prevents double-border darkening */
.fantasy-table :deep(tbody tr > td:nth-child(-n+4)) {
    border-bottom: 1px solid var(--ot-border-dim) !important;
}

/* Header cells: same solid border treatment */
.fantasy-table :deep(thead th) {
    border-bottom: 1px solid var(--ot-border-dim) !important;
}

/* Group separator borders — left edge of each group's first column */
.fantasy-table :deep(tr > :is(th, td):nth-child(5)) {
    border-left: 2px solid color-mix(in srgb, var(--rune-gold-dark) 40%, transparent) !important;
}
.fantasy-table :deep(tr > :is(th, td):nth-child(8)) {
    border-left: 2px solid color-mix(in srgb, var(--rune-green) 40%, transparent) !important;
}
.fantasy-table :deep(tr > :is(th, td):nth-child(11)) {
    border-left: 2px solid color-mix(in srgb, var(--rune-blue-light) 40%, transparent) !important;
}
.fantasy-table :deep(tr > :is(th, td):nth-child(16)) {
    border-left: 2px solid color-mix(in srgb, var(--rune-red) 40%, transparent) !important;
}

/* Also add matching left border to the group band header th cells */
.fantasy-table :deep(tr.group-band-row > th:nth-child(2)) {
    border-left: 2px solid color-mix(in srgb, var(--rune-gold-dark) 40%, transparent) !important;
}
.fantasy-table :deep(tr.group-band-row > th:nth-child(3)) {
    border-left: 2px solid color-mix(in srgb, var(--rune-green) 40%, transparent) !important;
}
.fantasy-table :deep(tr.group-band-row > th:nth-child(4)) {
    border-left: 2px solid color-mix(in srgb, var(--rune-blue-light) 40%, transparent) !important;
}
.fantasy-table :deep(tr.group-band-row > th:nth-child(5)) {
    border-left: 2px solid color-mix(in srgb, var(--rune-red) 40%, transparent) !important;
}

.fantasy-table :deep(tbody tr:nth-child(odd) td) {
    background-color: var(--ot-bg-mid);
}

.fantasy-table :deep(tbody tr:nth-child(even) td) {
    background-color: var(--sg-bg-mid);
}

.fantasy-table :deep(tbody tr:hover td) {
    background-color: color-mix(in srgb, var(--rune-gold) 4%, var(--ot-bg-mid));
}

/* Sticky body cells must have explicit opaque backgrounds matching their row */
.fantasy-table :deep(tbody tr:nth-child(odd) td:nth-child(-n+4)) {
    background-color: var(--ot-bg-mid);
}

.fantasy-table :deep(tbody tr:nth-child(even) td:nth-child(-n+4)) {
    background-color: var(--sg-bg-mid);
}

.fantasy-table :deep(tbody tr:hover td:nth-child(-n+4)) {
    background-color: color-mix(in srgb, var(--rune-gold) 4%, var(--ot-bg-mid));
}

/* Fixed summary columns */
.total-pts {
    font-family: var(--font-heading);
    font-size: var(--text-md);
    font-weight: 700;
    color: var(--sg-text);
}

.avg-pts {
    font-family: var(--font-body);
    font-size: var(--text-sm);
    color: var(--ot-text-dim);
}

/* Player cell */
.player-cell {
    display: flex;
    align-items: center;
    gap: 10px;
    padding: 6px 0;
}

.player-avatar {
    width: 48px;
    height: 48px;
    border-radius: 50%;
    object-fit: cover;
    flex-shrink: 0;
    background: color-mix(in srgb, var(--rune-blue) 15%, var(--ot-bg-mid));
}

.player-info {
    display: flex;
    flex-direction: column;
    gap: 2px;
    min-width: 0;
}

.player-name {
    font-family: var(--font-heading);
    font-size: var(--text-md);
    font-weight: 700;
    color: var(--ot-text);
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

.player-meta {
    display: flex;
    align-items: center;
    gap: 3px;
    font-family: var(--font-body);
    font-size: var(--text-sm);
    color: var(--ot-text-dim);
    white-space: nowrap;
}

.player-pos-icon {
    width: 13px;
    height: 13px;
    object-fit: contain;
}
</style>
