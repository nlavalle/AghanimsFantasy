<!-- eslint-disable vue/valid-v-slot -->
<template>
    <v-col>
        <StatsFilterBar :role-filter="roleFilter" :team-filter="teamFilter" :teams="teamsList" :search="leagueFilter"
            @update:role-filter="roleFilter = $event" @update:team-filter="teamFilter = $event"
            @update:search="leagueFilter = $event" />
        <v-row v-if="display.mobile.value" dense>
            <v-tabs v-model="leagueTab" density="compact">
                <v-tab value="kda" min-width="70px" width="70px">K/D/A</v-tab>
                <v-tab value="farm" min-width="70px" width="70px">Farm</v-tab>
                <v-tab value="support" min-width="80px" width="80px">Supp.</v-tab>
                <v-tab value="damageHealing" min-width="100px" width="100px">Dmg/Heal</v-tab>
            </v-tabs>
        </v-row>
        <div class="table-scroll-wrap" ref="tableRoot">
            <v-data-table class="league-table" :items="leagueMetadataStatsIndexed" :headers="displayedLeagueColumns"
                density="compact" :items-per-page="itemsPerPage" v-model:page="page"
                :style="{ 'font-size': !display.mobile.value ? '0.8rem' : '0.7rem' }">

                <template #headers="{ columns, isSorted, getSortIcon, toggleSort }">
                    <tr v-if="!display.mobile.value" class="group-band-row">
                        <th colspan="1" class="group-fixed" />
                        <th colspan="3" class="group-kda">K / D / A</th>
                        <th colspan="4" class="group-farm">Farm</th>
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

                <template v-slot:item.leaguePlayer="{ value }">
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

                <template v-slot:item.totalGoldPerMin="{ value }">{{ value.toFixed(0) }}</template>
                <template v-slot:item.totalXpPerMin="{ value }">{{ value.toFixed(0) }}</template>
                <template v-slot:item.totalHeroDamage="{ value }">{{ (value / 1000).toFixed(1) + 'k' }}</template>
                <template v-slot:item.totalTowerDamage="{ value }">{{ (value / 1000).toFixed(1) + 'k' }}</template>
                <template v-slot:item.totalHeroHealing="{ value }">{{ (value / 1000).toFixed(1) + 'k' }}</template>
                <template v-slot:item.totalStunDuration="{ value }">{{ value.toFixed(1) }}</template>

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
import type { LeagueMetadata } from '@/types/LeagueMetadata';
import { useDebouncedRef } from '@/services/debounce'
import type { FantasyLeague } from '@/types/FantasyLeague';
import { useDisplay } from 'vuetify';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import StatsFilterBar from '@/components/Stats/StatsFilterBar.vue';
import { useDragToScroll } from '@/services/useDragToScroll';
import { getPositionIcon } from '@/services/iconService';

const display = useDisplay()
const fantasyLeagueStore = useFantasyLeagueStore()

const leagueFilter = useDebouncedRef('');
const roleFilter = ref<number[]>([]);
const teamFilter = ref<number[]>([]);

const selectedFantasyLeague = defineModel<FantasyLeague>('selectedFantasyLeague');

const page = ref(1)
const itemsPerPage = 15;
const pageCount = computed(() => Math.ceil(leagueMetadataStatsIndexed.value.length / itemsPerPage))

const teamsList = computed(() => {
    const teams = fantasyLeagueStore.leagueMetadataStats.map(item => item.fantasyPlayer.team)
    return [...new Map(teams.map(item => [item.id, item])).values()]
})

onMounted(() => {
    if (selectedFantasyLeague.value) {
        fantasyLeagueStore.fetchFantasyLeagueMetadataStats();
    }
});

watch(selectedFantasyLeague, () => {
    if (selectedFantasyLeague.value) {
        fantasyLeagueStore.fetchFantasyLeagueMetadataStats();
    }
});

const leagueTab = ref('kda');

const COL_S = { width: '75px', align: 'center' } as const
const COL_M = { width: '90px', align: 'center' } as const
const COL_L = { width: '100px', align: 'center' } as const

const commonLeagueColumns = [
    {
        key: 'leaguePlayer',
        title: 'Player',
        align: 'left',
        value: (row: any) => ({
            playerName: row.fantasyPlayer.dotaAccount.name,
            playerPicture: row.fantasyPlayer.dotaAccount.steamProfilePicture,
            teamName: row.fantasyPlayer.team.name,
            teamPosition: row.fantasyPlayer.teamPosition,
            totalMatches: row.matchesPlayed
        }),
        width: !display.mobile.value ? '220px' : '120px',
        sortable: true,
        sort: (a: any, b: any) => a.playerName > b.playerName
    },
];
const kdaLeagueColumns = [
    { key: 'totalKills', title: !display.mobile.value ? 'Kills' : 'K', ...COL_S, value: (row: any) => row.kills, sortable: true, sort: (a: number, b: number) => b - a },
    { key: 'totalDeaths', title: !display.mobile.value ? 'Deaths' : 'D', ...COL_S, value: (row: any) => row.deaths, sortable: true, sort: (a: number, b: number) => b - a },
    { key: 'totalAssists', title: !display.mobile.value ? 'Assists' : 'A', ...COL_S, value: (row: any) => row.assists, sortable: true, sort: (a: number, b: number) => b - a },
];
const farmLeagueColumns = [
    { key: 'totalLastHits', title: !display.mobile.value ? 'Last Hits' : 'LH', ...COL_M, value: (row: any) => row.lastHits, sortable: true, sort: (a: number, b: number) => b - a },
    { key: 'totalDenies', title: !display.mobile.value ? 'Denies' : 'DN', ...COL_S, value: (row: any) => row.denies, sortable: true, sort: (a: number, b: number) => b - a },
    { key: 'totalGoldPerMin', title: !display.mobile.value ? 'Avg GPM' : 'G', ...COL_M, value: (row: any) => row.goldPerMinAverage, sortable: true, sort: (a: number, b: number) => b - a },
    { key: 'totalXpPerMin', title: !display.mobile.value ? 'Avg XPM' : 'XP', ...COL_M, value: (row: any) => row.xpPerMinAverage, sortable: true, sort: (a: number, b: number) => b - a },
];
const supportLeagueColumns = [
    { key: 'totalSupportGoldSpent', title: !display.mobile.value ? 'Supp Gold' : 'SG', ...COL_M, value: (row: any) => row.supportGoldSpent ?? 0, sortable: true, sort: (a: number, b: number) => b - a },
    { key: 'totalObsPlaced', title: !display.mobile.value ? 'Obs' : 'OB', ...COL_S, value: (row: any) => row.observerWardsPlaced ?? 0, sortable: true, sort: (a: number, b: number) => b - a },
    { key: 'totalSentriesPlaced', title: !display.mobile.value ? 'Sentries' : 'SN', ...COL_S, value: (row: any) => row.sentryWardsPlaced ?? 0, sortable: true, sort: (a: number, b: number) => b - a },
    { key: 'totalWardsDewarded', title: !display.mobile.value ? 'Dewards' : 'DW', ...COL_S, value: (row: any) => row.wardsDewarded ?? 0, sortable: true, sort: (a: number, b: number) => b - a },
    { key: 'totalCampsStacked', title: !display.mobile.value ? 'Camps' : 'C', ...COL_S, value: (row: any) => row.campsStacked ?? 0, sortable: true, sort: (a: number, b: number) => b - a },
];
const damageHealingLeagueColumns = [
    { key: 'totalHeroDamage', title: !display.mobile.value ? 'Hero Dmg' : 'HD', ...COL_L, value: (row: any) => row.heroDamage ?? 0, sortable: true, sort: (a: number, b: number) => b - a },
    { key: 'totalTowerDamage', title: !display.mobile.value ? 'Tower Dmg' : 'TD', ...COL_L, value: (row: any) => row.towerDamage ?? 0, sortable: true, sort: (a: number, b: number) => b - a },
    { key: 'totalHeroHealing', title: !display.mobile.value ? 'Hero Heal' : 'HH', ...COL_L, value: (row: any) => row.heroHealing ?? 0, sortable: true, sort: (a: number, b: number) => b - a },
    { key: 'totalStunDuration', title: !display.mobile.value ? 'Stuns' : 'SD', ...COL_M, value: (row: any) => row.stunDuration ?? 0, sortable: true, sort: (a: number, b: number) => b - a },
];

const stringifyNested = (obj: Object | string | null): any => {
    if (typeof obj !== 'object' || obj === null) return String(obj);
    return Object.values(obj).map(val => stringifyNested(val)).join(' ');
};

const leagueMetadataStatsIndexed = computed(() => {
    return fantasyLeagueStore.leagueMetadataStats
        .map((player: LeagueMetadata, index) => ({ ...player, position: index + 1 }))
        .filter(item => !leagueFilter.value ||
            Object.values(item.fantasyPlayer).some(val => stringifyNested(val).toLowerCase().includes(leagueFilter.value.toLowerCase())))
        .filter(item => roleFilter.value.length === 0 || roleFilter.value.some(role => role == item.fantasyPlayer.teamPosition))
        .filter(item => teamFilter.value.length === 0 || teamFilter.value.some(team => team == item.fantasyPlayer.teamId));
});

const displayedLeagueColumns = computed<any>(() => {
    if (!display.mobile.value) {
        return [...commonLeagueColumns, ...kdaLeagueColumns, ...farmLeagueColumns, ...supportLeagueColumns, ...damageHealingLeagueColumns];
    }
    switch (leagueTab.value) {
        case 'kda': return [...commonLeagueColumns, ...kdaLeagueColumns];
        case 'farm': return [...commonLeagueColumns, ...farmLeagueColumns];
        case 'support': return [...commonLeagueColumns, ...supportLeagueColumns];
        case 'damageHealing': return [...commonLeagueColumns, ...damageHealingLeagueColumns];
        default: return [...commonLeagueColumns];
    }
})

const KDA_KEYS = new Set(['totalKills', 'totalDeaths', 'totalAssists'])
const FARM_KEYS = new Set(['totalLastHits', 'totalDenies', 'totalGoldPerMin', 'totalXpPerMin'])
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

.group-fixed { background: var(--rune-bg-surface); }

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
.col-header.sortable { cursor: pointer; }
.col-header.sortable:hover { opacity: 0.8; }

.col-fixed { color: color-mix(in srgb, var(--rune-gold-dark) 80%, transparent); }
.col-kda   { color: color-mix(in srgb, var(--rune-gold-dark) 60%, transparent); }
.col-farm  { color: color-mix(in srgb, var(--rune-green) 60%, transparent); }
.col-support { color: color-mix(in srgb, var(--rune-blue-light) 60%, transparent); }
.col-damage  { color: color-mix(in srgb, var(--rune-red) 60%, transparent); }

.sort-icon { margin-left: 4px; }

/* Scroll container */
.table-scroll-wrap {
    width: 100%;
    overflow-x: auto;
    cursor: grab;
}
.table-scroll-wrap:active { cursor: grabbing; }

.league-table { min-width: 1500px; }

.league-table :deep(.v-table__wrapper) { overflow: visible; }

/* Sticky first column (player) */
.league-table {
    --frozen-shadow: 4px 0 12px color-mix(in srgb, var(--ot-bg-deep) 60%, transparent);
}

.league-table :deep(tr.group-band-row > th:nth-child(1)) {
    position: sticky; left: 0; z-index: 3;
    background-color: var(--rune-bg-surface);
}
.league-table :deep(tr.col-header-row > th:nth-child(1)) {
    position: sticky; left: 0; z-index: 3;
    background-color: var(--rune-bg-surface);
    text-align: left;
    box-shadow: var(--frozen-shadow);
}
.league-table :deep(.v-table__wrapper > table > tbody > tr > td:nth-child(1)) {
    position: sticky; left: 0; z-index: 2;
    box-shadow: var(--frozen-shadow);
}

/* Row styling */
.league-table :deep(.v-table__wrapper > table) {
    border-collapse: collapse;
    border-spacing: 0;
}
.league-table :deep(tbody tr) { height: 60px; }
.league-table :deep(tbody td) { border-bottom: 1px solid var(--ot-border-dim) !important; }
.league-table :deep(thead th) { border-bottom: 1px solid var(--ot-border-dim) !important; }
.league-table :deep(tbody tr:nth-child(odd) td) { background-color: var(--ot-bg-mid); }
.league-table :deep(tbody tr:nth-child(even) td) { background-color: var(--sg-bg-mid); }
.league-table :deep(tbody tr:hover td) { background-color: color-mix(in srgb, var(--rune-gold) 4%, var(--ot-bg-mid)); }
.league-table :deep(tbody tr:nth-child(odd) td:nth-child(1)) { background-color: var(--ot-bg-mid); }
.league-table :deep(tbody tr:nth-child(even) td:nth-child(1)) { background-color: var(--sg-bg-mid); }
.league-table :deep(tbody tr:hover td:nth-child(1)) { background-color: color-mix(in srgb, var(--rune-gold) 4%, var(--ot-bg-mid)); }

/* Group separator borders */
.league-table :deep(tr > :is(th, td):nth-child(2)) { border-left: 2px solid color-mix(in srgb, var(--rune-gold-dark) 40%, transparent) !important; }
.league-table :deep(tr > :is(th, td):nth-child(5)) { border-left: 2px solid color-mix(in srgb, var(--rune-green) 40%, transparent) !important; }
.league-table :deep(tr > :is(th, td):nth-child(9)) { border-left: 2px solid color-mix(in srgb, var(--rune-blue-light) 40%, transparent) !important; }
.league-table :deep(tr > :is(th, td):nth-child(14)) { border-left: 2px solid color-mix(in srgb, var(--rune-red) 40%, transparent) !important; }
.league-table :deep(tr.group-band-row > th:nth-child(2)) { border-left: 2px solid color-mix(in srgb, var(--rune-gold-dark) 40%, transparent) !important; }
.league-table :deep(tr.group-band-row > th:nth-child(3)) { border-left: 2px solid color-mix(in srgb, var(--rune-green) 40%, transparent) !important; }
.league-table :deep(tr.group-band-row > th:nth-child(4)) { border-left: 2px solid color-mix(in srgb, var(--rune-blue-light) 40%, transparent) !important; }
.league-table :deep(tr.group-band-row > th:nth-child(5)) { border-left: 2px solid color-mix(in srgb, var(--rune-red) 40%, transparent) !important; }

/* Player cell */
.player-cell { display: flex; align-items: center; gap: 10px; padding: 6px 0; }
.player-avatar { width: 48px; height: 48px; border-radius: 50%; object-fit: cover; flex-shrink: 0; background: color-mix(in srgb, var(--rune-blue) 15%, var(--ot-bg-mid)); }
.player-info { display: flex; flex-direction: column; gap: 2px; min-width: 0; }
.player-name { font-family: var(--font-heading); font-size: var(--text-md); font-weight: 700; color: var(--ot-text); white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
.player-meta { display: flex; align-items: center; gap: 3px; font-family: var(--font-body); font-size: var(--text-sm); color: var(--ot-text-dim); white-space: nowrap; }
.player-pos-icon { width: 13px; height: 13px; object-fit: contain; }
</style>
