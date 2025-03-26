<template>
    <div class="ml-5 mb-5">
        <v-row class="mt-1 justify-center">
            <h3>
                Winnings Breakdown
            </h3>
        </v-row>
        <v-row class="mt-0">
            <v-data-table class="winnings-breakdown-table" :items="statistics" :items-per-page="5"
                :headers="statsHeaders" hide-default-footer density="compact">
            </v-data-table>
        </v-row>
    </div>
</template>

<script setup lang="ts">
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import { VDataTable, VRow } from 'vuetify/components'

const fantasyLeagueStore = useFantasyLeagueStore();

const getQuintileRange = (quintile: number) => {
    let quintileSize = fantasyLeagueStore.fantasyPlayerPoints.length / 5;
    let quintileLast = Math.ceil(quintile * quintileSize);
    let quintileFirst = quintileLast - quintileSize + 1;

    return `${quintileFirst} - ${quintileLast}`;
}

const statsHeaders = [
    {
        title: 'Rank',
        value: 'name',
        width: '50%'
    },
    {
        title: 'Range',
        value: 'value',
        width: '30%'
    },
    {
        title: 'Winnings',
        value: 'available',
        width: '20%'
    },
];

const statistics = [
    {
        name: 'Top 20%',
        value: getQuintileRange(1),
        available: '240g'
    },
    {
        name: 'Top 40%',
        value: getQuintileRange(2),
        available: '180g'
    },
    {
        name: 'Top 60%',
        value: getQuintileRange(3),
        available: '120g'
    },
    {
        name: 'Bottom 40%',
        value: getQuintileRange(4),
        available: '60g'
    },
    {
        name: 'Bottom 20%',
        value: getQuintileRange(5),
        available: '0g'
    },
]

</script>

<style scoped>
.winnings-breakdown-table {
    margin-top: 10px;
    width: 400px;
    border: 2px solid var(--aghanims-fantasy-main-2);
    border-radius: 10px;
}

h3 {
    color: var(--aghanims-fantasy-white);
}

.v-data-table ::v-deep(header) {
    background-color: var(--aghanims-fantasy-main-2);
}

.v-data-table ::v-deep(thead) {
    background-color: var(--aghanims-fantasy-main-2);
}

.v-data-table ::v-deep(td) {
    border-right: 1px solid var(--aghanims-fantasy-main-2);
}
</style>