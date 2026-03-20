<!-- eslint-disable vue/valid-v-slot -->
<template>
    <v-col>
        <StatsFilterBar :role-filter="roleFilter" :team-filter="teamFilter" :teams="teamsList" :search="matchFilter"
            @update:role-filter="roleFilter = $event" @update:team-filter="teamFilter = $event"
            @update:search="matchFilter = $event" />
        <v-row v-if="display.mobile.value" dense>
            <v-tabs v-model="fantasyTab" density="compact">
                <v-tab value="kda" min-width="70px" width="70px">K/D/A</v-tab>
                <v-tab value="farm" min-width="70px" width="70px">Farm</v-tab>
                <v-tab value="support" min-width="80px" width="80px">Supp.</v-tab>
                <v-tab value="damageHealing" min-width="100px" width="100px">Dmg/Heal</v-tab>
            </v-tabs>
        </v-row>
        <div class="table-scroll-wrap" ref="tableRoot">
            <v-data-table :class="['match-table', { 'is-grouped': !draftFiltered }]"
                :items="playerFantasyMatchStatsIndexed" :headers="displayedFantasyColumns"
                :group-by="draftFiltered ? undefined : groupBy" items-per-page="110" density="compact"
                hide-default-footer :style="{ 'font-size': !display.mobile.value ? '0.8rem' : '0.7rem' }">

                <template #headers="{ columns, isSorted, getSortIcon, toggleSort }">
                    <tr v-if="!display.mobile.value" class="group-band-row">
                        <!-- When grouped, Vuetify injects a data-table-group col as th:nth-child(1) -->
                        <th :colspan="draftFiltered ? 2 : 3" class="group-fixed" />
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

                <template v-if="!draftFiltered" v-slot:group-header="{ item, columns, toggleGroup, isGroupOpen }">
                    <tr class="match-group-header">
                        <td :colspan="columns.length">
                            <VBtn :icon="isGroupOpen(item) ? '$expand' : '$next'" size="small" variant="text"
                                @click="toggleGroup(item)" />
                            Match - {{ item.value }} - {{ getMatchTeams(item.value) }}
                        </td>
                    </tr>
                </template>

                <template v-slot:header.data-table-group>
                    <span></span>
                </template>

                <template v-slot:item.fantasyPlayer="{ value }">
                    <div class="player-cell">
                        <img v-if="!display.mobile.value" :src="value.playerPicture" class="player-avatar" />
                        <div class="player-info">
                            <span class="player-name">{{ value.playerName }}</span>
                            <span class="player-meta">
                                {{ value.teamName }}
                                <img :src="getPositionIcon(value.teamPosition)" class="player-pos-icon" />
                                <span v-if="draftFiltered">· Match {{ value.matchId }}</span>
                            </span>
                            <img :src="getHeroIcon(value.heroName)" class="hero-icon" />
                        </div>
                    </div>
                </template>

                <template v-slot:item.totalPoints="{ value }">
                    <b>{{ value }}</b>
                </template>
                <template v-slot:item.kills="{ value }">
                    <b>{{ value.killPoints }}</b><br>({{ value.kills }})
                </template>
                <template v-slot:item.deaths="{ value }">
                    <b>{{ value.deathPoints }}</b><br>({{ value.deaths }})
                </template>
                <template v-slot:item.assists="{ value }">
                    <b>{{ value.assistPoints }}</b><br>({{ value.assists }})
                </template>
                <template v-slot:item.lastHits="{ value }">
                    <b>{{ value.lastHitsPoints }}</b><br>({{ value.lastHits }})
                </template>
                <template v-slot:item.goldPerMin="{ value }">
                    <b>{{ value.goldPerMinPoints }}</b><br>({{ value.goldPerMin }})
                </template>
                <template v-slot:item.xpPerMin="{ value }">
                    <b>{{ value.xpPerMinPoints }}</b><br>({{ value.xpPerMin }})
                </template>
                <template v-slot:item.supportGoldSpent="{ value }">
                    <b>{{ value.supportGoldSpentPoints }}</b><br>({{ (value.supportGoldSpent / 1000).toFixed(1) + 'k'
                    }})
                </template>
                <template v-slot:item.obsPlaced="{ value }">
                    <b>{{ value.observerWardsPlacedPoints }}</b><br>({{ value.observerWardsPlaced }})
                </template>
                <template v-slot:item.sentriesPlaced="{ value }">
                    <b>{{ value.sentryWardsPlacedPoints }}</b><br>({{ value.sentryWardsPlaced }})
                </template>
                <template v-slot:item.wardsDewarded="{ value }">
                    <b>{{ value.wardsDewardedPoints }}</b><br>({{ value.wardsDewarded }})
                </template>
                <template v-slot:item.campsStacked="{ value }">
                    <b>{{ value.campsStackedPoints }}</b><br>({{ value.campsStacked }})
                </template>
                <template v-slot:item.heroDamage="{ value }">
                    <b>{{ value.heroDamagePoints }}</b><br>({{ value.heroDamage }})
                </template>
                <template v-slot:item.towerDamage="{ value }">
                    <b>{{ value.towerDamagePoints }}</b><br>({{ value.towerDamage }})
                </template>
                <template v-slot:item.heroHealing="{ value }">
                    <b>{{ value.heroHealingPoints }}</b><br>({{ value.heroHealing }})
                </template>
                <template v-slot:item.stunDuration="{ value }">
                    <b>{{ value.stunDurationPoints }}</b><br>({{ value.stunDuration }})
                </template>
            </v-data-table>
        </div>
    </v-col>
</template>

<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue';
import { VCol, VDataTable, VTabs, VTab, VIcon, VRow, VBtn } from 'vuetify/components';
import type { FantasyLeague } from '@/types/FantasyLeague';
import type { FantasyPlayerMatchPoints } from '../Fantasy/fantasyDraft';
import { useDebouncedRef } from '@/services/debounce'
import { useDisplay } from 'vuetify';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import StatsFilterBar from '@/components/Stats/StatsFilterBar.vue';
import { useDragToScroll } from '@/services/useDragToScroll';
import { getPositionIcon, getHeroIcon } from '@/services/iconService';

const display = useDisplay()
const fantasyLeagueStore = useFantasyLeagueStore()

const matchFilter = useDebouncedRef('');
const roleFilter = ref<number[]>([]);
const teamFilter = ref<number[]>([]);
const groupBy = <VDataTable['groupBy']>[{ key: 'fantasyMatchPlayer.fantasyMatchId', order: 'desc' }];

const selectedFantasyLeague = defineModel<FantasyLeague>('selectedFantasyLeague');
const draftFiltered = defineModel<boolean>('draftFiltered');


const fantasyMatchStats = computed(() =>
    draftFiltered.value ? fantasyLeagueStore.draftFantasyMatchStats : fantasyLeagueStore.playerFantasyMatchStats
)

const teamsList = computed(() => {
    const teams = fantasyMatchStats.value.map(item => item.fantasyPlayer.team)
    return [...new Map(teams.map(item => [item.id, item])).values()]
})

onMounted(() => {
    if (selectedFantasyLeague.value && !draftFiltered.value) {
        fantasyLeagueStore.fetchPlayerFantasyMatchStats();
    } else if (selectedFantasyLeague.value && draftFiltered.value) {
        fantasyLeagueStore.fetchDraftFantasyMatchStats();
    }
});

watch(selectedFantasyLeague, () => {
    if (selectedFantasyLeague.value && !draftFiltered.value) {
        fantasyLeagueStore.fetchPlayerFantasyMatchStats();
    } else if (selectedFantasyLeague.value && draftFiltered.value) {
        fantasyLeagueStore.fetchDraftFantasyMatchStats();
    }
});

const fantasyTab = ref('kda');

const COL_S = { width: '75px', align: 'center' } as const
const COL_M = { width: '90px', align: 'center' } as const
const COL_L = { width: '100px', align: 'center' } as const

const commonFantasyColumns = [
    {
        key: 'fantasyPlayer',
        title: 'Player',
        align: 'left',
        value: (row: any) => ({
            playerName: row.fantasyPlayer.dotaAccount.name,
            playerPicture: row.fantasyPlayer.dotaAccount.steamProfilePicture,
            teamName: row.fantasyPlayer.team.name,
            teamPosition: row.fantasyPlayer.teamPosition,
            heroName: row.fantasyMatchPlayer.hero.name,
            matchId: row.fantasyMatchPlayer.matchId
        }),
        width: !display.mobile.value ? '240px' : '120px',
        sortable: true,
        sort: (a: any, b: any) => a.playerName > b.playerName
    },
    {
        key: 'totalPoints',
        title: !display.mobile.value ? 'Total Pts' : 'Pts',
        align: 'center',
        value: (row: any) => row.totalMatchFantasyPoints.toFixed(1),
        width: '80px',
        sortable: true,
        sort: (a: number, b: number) => b - a
    },
];
const kdaFantasyColumns = [
    { key: 'kills', title: !display.mobile.value ? 'Kills' : 'K', ...COL_S, value: (row: any) => ({ kills: row.kills, killPoints: row.killsPoints.toFixed(1) }), sortable: true, sort: (a: any, b: any) => b.killPoints - a.killPoints },
    { key: 'deaths', title: !display.mobile.value ? 'Deaths' : 'D', ...COL_S, value: (row: any) => ({ deaths: row.deaths, deathPoints: row.deathsPoints.toFixed(1) }), sortable: true, sort: (a: any, b: any) => b.deathPoints - a.deathPoint },
    { key: 'assists', title: !display.mobile.value ? 'Assists' : 'A', ...COL_S, value: (row: any) => ({ assists: row.assists, assistPoints: row.assistsPoints.toFixed(1) }), sortable: true, sort: (a: any, b: any) => b.assistPoints - a.assistPoints },
];
const farmFantasyColumns = [
    { key: 'lastHits', title: !display.mobile.value ? 'Last Hits' : 'LH', ...COL_M, value: (row: any) => ({ lastHits: row.lastHits.toLocaleString(), lastHitsPoints: row.lastHitsPoints.toFixed(1) }), sortable: true, sort: (a: any, b: any) => b.lastHitsPoints - a.lastHitsPoints },
    { key: 'goldPerMin', title: !display.mobile.value ? 'GPM' : 'G', ...COL_S, value: (row: any) => ({ goldPerMin: row.goldPerMin.toFixed(0), goldPerMinPoints: row.goldPerMinPoints.toFixed(1) }), sortable: true, sort: (a: any, b: any) => b.goldPerMinPoints - a.goldPerMinPoints },
    { key: 'xpPerMin', title: !display.mobile.value ? 'XPM' : 'XP', ...COL_S, value: (row: any) => ({ xpPerMin: row.xpPerMin.toFixed(0), xpPerMinPoints: row.xpPerMinPoints.toFixed(1) }), sortable: true, sort: (a: any, b: any) => b.xpPerMinPoints - a.xpPerMinPoints },
];
const supportFantasyColumns = [
    { key: 'supportGoldSpent', title: !display.mobile.value ? 'Supp. Gold' : 'SG', ...COL_M, value: (row: any) => ({ supportGoldSpent: row.supportGoldSpent ?? 0, supportGoldSpentPoints: row.supportGoldSpentPoints.toFixed(1) }), sortable: true, sort: (a: any, b: any) => b.supportGoldSpentPoints - a.supportGoldSpentPoints },
    { key: 'obsPlaced', title: !display.mobile.value ? 'Obs' : 'OB', ...COL_S, value: (row: any) => ({ observerWardsPlaced: row.observerWardsPlaced ?? 0, observerWardsPlacedPoints: row.observerWardsPlacedPoints.toFixed(1) }), sortable: true, sort: (a: any, b: any) => b.observerWardsPlacedPoints - a.observerWardsPlacedPoints },
    { key: 'sentriesPlaced', title: !display.mobile.value ? 'Sentries' : 'SN', ...COL_S, value: (row: any) => ({ sentryWardsPlaced: row.sentryWardsPlaced ?? 0, sentryWardsPlacedPoints: row.sentryWardsPlacedPoints.toFixed(1) }), sortable: true, sort: (a: any, b: any) => b.sentryWardsPlacedPoints - a.sentryWardsPlacedPoints },
    { key: 'wardsDewarded', title: !display.mobile.value ? 'Dewards' : 'DW', ...COL_S, value: (row: any) => ({ wardsDewarded: row.wardsDewarded ?? 0, wardsDewardedPoints: row.wardsDewardedPoints.toFixed(1) }), sortable: true, sort: (a: any, b: any) => b.wardsDewardedPoints - a.wardsDewardedPoints },
    { key: 'campsStacked', title: !display.mobile.value ? 'Camps' : 'C', ...COL_S, value: (row: any) => ({ campsStacked: row.campsStacked ?? 0, campsStackedPoints: row.campsStackedPoints.toFixed(1) }), sortable: true, sort: (a: any, b: any) => b.campsStackedPoints - a.campsStackedPoints },
];
const damageHealingFantasyColumns = [
    { key: 'heroDamage', title: !display.mobile.value ? 'Hero Dmg' : 'HD', ...COL_L, value: (row: any) => ({ heroDamage: (row.heroDamage / 1000).toFixed(1) + 'k', heroDamagePoints: row.heroDamagePoints.toFixed(1) }), sortable: true, sort: (a: any, b: any) => b.heroDamagePoints - a.heroDamagePoints },
    { key: 'towerDamage', title: !display.mobile.value ? 'Tower Dmg' : 'TD', ...COL_L, value: (row: any) => ({ towerDamage: (row.towerDamage / 1000).toFixed(1) + 'k', towerDamagePoints: row.towerDamagePoints.toFixed(1) }), sortable: true, sort: (a: any, b: any) => b.towerDamagePoints - a.towerDamagePoints },
    { key: 'heroHealing', title: !display.mobile.value ? 'Hero Heal' : 'HH', ...COL_L, value: (row: any) => ({ heroHealing: (row.heroHealing / 1000).toFixed(1) + 'k', heroHealingPoints: row.heroHealingPoints.toFixed(1) }), sortable: true, sort: (a: any, b: any) => b.heroHealingPoints - a.heroHealingPoints },
    { key: 'stunDuration', title: !display.mobile.value ? 'Stun Dur.' : 'SD', ...COL_M, value: (row: any) => ({ stunDuration: row.stunDuration.toFixed(1), stunDurationPoints: row.stunDurationPoints.toFixed(1) }), sortable: true, sort: (a: any, b: any) => b.stunDurationPoints - a.stunDurationPoints },
];

const stringifyNested = (obj: Object | string | null): any => {
    if (typeof obj !== 'object' || obj === null) return String(obj);
    return Object.values(obj).map(val => stringifyNested(val)).join(' ');
};

const getMatchTeams = (matchId: number): string => {
    const radiantTeam = fantasyMatchStats.value.find(m => m.fantasyMatchPlayer.fantasyMatchId == matchId && m.fantasyMatchPlayer.dotaTeamSide == false)?.fantasyMatchPlayer.teamFormatted ?? '';
    const direTeam = fantasyMatchStats.value.find(m => m.fantasyMatchPlayer.fantasyMatchId == matchId && m.fantasyMatchPlayer.dotaTeamSide == true)?.fantasyMatchPlayer.teamFormatted ?? '';
    return `${radiantTeam} (Radiant) vs ${direTeam} (Dire)`
}

const playerFantasyMatchStatsIndexed = computed(() => {
    return fantasyMatchStats.value
        .map((player: FantasyPlayerMatchPoints, index) => ({ ...player, position: index + 1 }))
        .filter(item => !matchFilter.value ||
            Object.values(item.fantasyPlayer).some(val => stringifyNested(val).toLowerCase().includes(matchFilter.value.toLowerCase())))
        .filter(item => roleFilter.value.length === 0 || roleFilter.value.some(role => role == item.fantasyPlayer.teamPosition))
        .filter(item => teamFilter.value.length === 0 || teamFilter.value.some(team => team == item.fantasyPlayer.teamId));
});

const displayedFantasyColumns = computed<any>(() => {
    if (!display.mobile.value) {
        return [...commonFantasyColumns, ...kdaFantasyColumns, ...farmFantasyColumns, ...supportFantasyColumns, ...damageHealingFantasyColumns];
    }
    switch (fantasyTab.value) {
        case 'kda': return [...commonFantasyColumns, ...kdaFantasyColumns];
        case 'farm': return [...commonFantasyColumns, ...farmFantasyColumns];
        case 'support': return [...commonFantasyColumns, ...supportFantasyColumns];
        case 'damageHealing': return [...commonFantasyColumns, ...damageHealingFantasyColumns];
        default: return [...commonFantasyColumns];
    }
})

const KDA_KEYS = new Set(['kills', 'deaths', 'assists'])
const FARM_KEYS = new Set(['lastHits', 'goldPerMin', 'xpPerMin'])
const SUPPORT_KEYS = new Set(['supportGoldSpent', 'obsPlaced', 'sentriesPlaced', 'wardsDewarded', 'campsStacked'])
const DAMAGE_KEYS = new Set(['heroDamage', 'towerDamage', 'heroHealing', 'stunDuration'])

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
.col-header-row {
    border-bottom: 2px solid var(--ot-border-dim) !important;
}

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

.match-table {
    min-width: 1500px;
}

.match-table :deep(.v-table__wrapper) {
    overflow: visible;
}

/* Sticky columns (player + total pts) */
.match-table {
    --sticky-2: 200px;
    /* width of player column */
    --frozen-shadow: 4px 0 12px color-mix(in srgb, var(--ot-bg-deep) 60%, transparent);
}

/*
 * Zero out the data-table-group column Vuetify injects into the col-header-row
 * (th:nth-child(1)) and body data rows (td:nth-child(1)).
 * group-band-row is fully custom — it has NO data-table-group th, so don't touch it.
 * Exclude .match-group-header rows — their single td is colspan=all, not a group col.
 */
.match-table :deep(tr.col-header-row > th:nth-child(1)),
.match-table :deep(tbody tr:not(.match-group-header) > td:nth-child(1)) {
    width: 0 !important;
    min-width: 0 !important;
    max-width: 0 !important;
    padding: 0 !important;
    overflow: hidden;
}

/* group-band-row: th:1 is our group-fixed, sticky at left:0 */
.match-table :deep(tr.group-band-row > th:nth-child(1)) {
    position: sticky;
    left: 0;
    z-index: 3;
    background-color: var(--rune-bg-surface);
    border-left: none !important;
    box-shadow: var(--frozen-shadow);
}

/* col-header-row: th:1 is zeroed data-table-group; player=th:2, totalPts=th:3 */
.match-table :deep(tr.col-header-row > th:nth-child(2)) {
    position: sticky;
    left: 0;
    z-index: 3;
    background-color: var(--rune-bg-surface);
    text-align: left;
    border-left: none !important;
    /* Enforce fixed width so --sticky-2 offset is always accurate regardless of collapse state */
    width: var(--sticky-2) !important;
    min-width: var(--sticky-2) !important;
}

.match-table :deep(tr.col-header-row > th:nth-child(3)) {
    position: sticky;
    left: var(--sticky-2);
    z-index: 3;
    background-color: var(--rune-bg-surface);
    border-left: none !important;
    box-shadow: var(--frozen-shadow);
}

/* Body data rows: td:1 is zeroed data-table-group; player=td:2, totalPts=td:3 */
.match-table :deep(tbody tr:not(.match-group-header) > td:nth-child(2)) {
    position: sticky;
    left: 0;
    z-index: 2;
    width: var(--sticky-2) !important;
    min-width: var(--sticky-2) !important;
}

.match-table :deep(tbody tr:not(.match-group-header) > td:nth-child(3)) {
    position: sticky;
    left: var(--sticky-2);
    z-index: 2;
    box-shadow: var(--frozen-shadow);
}

/* Frozen col backgrounds (cols 1-3 = zeroed group + player + totalPts) */
.match-table :deep(tbody tr:not(.match-group-header):nth-child(odd) td:nth-child(-n+3)) {
    background-color: var(--ot-bg-mid);
}

.match-table :deep(tbody tr:not(.match-group-header):nth-child(even) td:nth-child(-n+3)) {
    background-color: var(--sg-bg-mid);
}

.match-table :deep(tbody tr:not(.match-group-header):hover td:nth-child(-n+3)) {
    background-color: color-mix(in srgb, var(--rune-gold) 4%, var(--ot-bg-mid));
}

/* Row styling */
.match-table :deep(.v-table__wrapper > table) {
    border-collapse: collapse;
    border-spacing: 0;
}

.match-table :deep(tbody tr) {
    height: 70px;
}

.match-table :deep(tbody td) {
    border-bottom: 1px solid var(--ot-border-dim) !important;
}

.match-table :deep(thead th) {
    border-bottom: 1px solid var(--ot-border-dim) !important;
}

.match-table :deep(tbody tr:nth-child(odd) td) {
    background-color: var(--ot-bg-mid);
}

.match-table :deep(tbody tr:nth-child(even) td) {
    background-color: var(--sg-bg-mid);
}

.match-table :deep(tbody tr:hover td) {
    background-color: color-mix(in srgb, var(--rune-gold) 4%, var(--ot-bg-mid));
}


/* Match group header row */
.match-table :deep(tr.match-group-header td) {
    font-family: var(--font-heading);
    font-size: var(--text-sm);
    font-weight: 700;
    background-color: color-mix(in srgb, var(--rune-blue) 10%, var(--ot-bg-deep));
    border-bottom: 1px solid var(--ot-border-dim) !important;
    padding: 8px 12px;
}

/* Group separator borders — td:1 is zeroed when grouped so offsets match not-grouped */
.match-table :deep(tbody tr > td:nth-child(4)) {
    border-left: 2px solid color-mix(in srgb, var(--rune-gold-dark) 40%, transparent) !important;
}

.match-table :deep(tbody tr > td:nth-child(7)) {
    border-left: 2px solid color-mix(in srgb, var(--rune-green) 40%, transparent) !important;
}

.match-table :deep(tbody tr > td:nth-child(10)) {
    border-left: 2px solid color-mix(in srgb, var(--rune-blue-light) 40%, transparent) !important;
}

.match-table :deep(tbody tr > td:nth-child(15)) {
    border-left: 2px solid color-mix(in srgb, var(--rune-red) 40%, transparent) !important;
}

/*
 * col-header-row: th:1 is zeroed, so KDA=th:4, Farm=th:7, Support=th:10, Damage=th:15
 * group-band-row: th:1=group-fixed, th:2=KDA, th:3=Farm, th:4=Support, th:5=Damage
 */
.match-table :deep(thead tr.col-header-row > th:nth-child(4)) {
    border-left: 2px solid color-mix(in srgb, var(--rune-gold-dark) 40%, transparent) !important;
}

.match-table :deep(thead tr.col-header-row > th:nth-child(7)) {
    border-left: 2px solid color-mix(in srgb, var(--rune-green) 40%, transparent) !important;
}

.match-table :deep(thead tr.col-header-row > th:nth-child(10)) {
    border-left: 2px solid color-mix(in srgb, var(--rune-blue-light) 40%, transparent) !important;
}

.match-table :deep(thead tr.col-header-row > th:nth-child(15)) {
    border-left: 2px solid color-mix(in srgb, var(--rune-red) 40%, transparent) !important;
}

.match-table :deep(tr.group-band-row > th:nth-child(2)) {
    border-left: 2px solid color-mix(in srgb, var(--rune-gold-dark) 40%, transparent) !important;
}

.match-table :deep(tr.group-band-row > th:nth-child(3)) {
    border-left: 2px solid color-mix(in srgb, var(--rune-green) 40%, transparent) !important;
}

.match-table :deep(tr.group-band-row > th:nth-child(4)) {
    border-left: 2px solid color-mix(in srgb, var(--rune-blue-light) 40%, transparent) !important;
}

.match-table :deep(tr.group-band-row > th:nth-child(5)) {
    border-left: 2px solid color-mix(in srgb, var(--rune-red) 40%, transparent) !important;
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

.hero-icon {
    width: 64px;
    height: 36px;
    object-fit: cover;
    border-radius: 2px;
    margin-top: 2px;
}
</style>
