<template>
    <div class="flex-container">
        <div class="row">
            <q-table class="fantasy-stats-table" title="Fantasy Player Stats" :columns="columns"
                :rows="playerFantasyStatsIndexed" virtual-scroll :rows-per-page-options="[0]" />
        </div>
    </div>
</template>
  
<script>
import { ref, onMounted, watch, computed } from 'vue';
import { localApiService } from 'src/services/localApiService';
import { useAuthStore } from 'stores/auth';
import { useLeagueStore } from 'src/stores/league';

export default {
    name: 'FantasyStatsPage',
    setup() {
        const authStore = useAuthStore();
        const leagueStore = useLeagueStore();

        const playerFantasyStats = ref([]);
        const columns = [
            {
                name: 'fantasyPlayerPosition',
                label: '#',
                align: 'left',
                field: row => row.position,
                format: val => `${val}`,
                sortable: true
            },
            {
                name: 'fantasyPlayer',
                label: 'Player',
                align: 'left',
                field: row => row.fantasyPlayer.dotaAccount.name,
                format: val => `${val}`,
                style: 'width: 200px; max-width: 200px',
                sortable: true
            },
            {
                name: 'fantasyPlayerTeam',
                label: 'Team',
                align: 'left',
                field: row => row.fantasyPlayer.team.name,
                format: val => `${val}`,
                style: 'width: 150px; max-width: 150px',
                sortable: true
            },
            {
                name: 'totalMatches',
                label: 'Matches',
                align: 'right',
                field: row => row.totalMatches,
                sortable: true
            },
            {
                name: 'totalKills',
                label: 'Kills (pts)',
                align: 'left',
                field: row => row,
                format: val => val.totalKills + ` (${val.totalKillsPoints.toFixed(1)})`,
                sortable: true,
                sort: (a, b) => a.totalKills - b.totalKills
            },
            {
                name: 'totalDeaths',
                label: 'Deaths (pts)',
                align: 'left',
                field: row => row,
                format: val => val.totalDeaths + ` (${val.totalDeathsPoints.toFixed(1)})`,
                sortable: true,
                sort: (a, b) => a.totalDeaths - b.totalDeaths
            },
            {
                name: 'totalAssists',
                label: 'Assists (pts)',
                align: 'left',
                field: row => row,
                format: val => val.totalAssists + ` (${val.totalAssistsPoints.toFixed(1)})`,
                sortable: true,
                sort: (a, b) => a.totalAssists - b.totalAssists
            },
            {
                name: 'totalLastHits',
                label: 'LastHits (pts)',
                align: 'left',
                field: row => row,
                format: val => val.totalLastHits.toLocaleString() + ` (${val.totalLastHitsPoints.toFixed(1).toLocaleString()})`,
                sortable: true,
                sort: (a, b) => a.totalLastHits - b.totalLastHits
            },
            {
                name: 'totalGoldPerMin',
                label: 'GoldPerMin (pts)',
                align: 'left',
                field: row => row,
                format: val => val.avgGoldPerMin.toLocaleString() + ` (${val.totalGoldPerMinPoints.toFixed(1).toLocaleString()})`,
                sortable: true,
                sort: (a, b) => a.avgGoldPerMin - b.avgGoldPerMin
            },
            {
                name: 'totalXpPerMin',
                label: 'XpPerMin (pts)',
                align: 'left',
                field: row => row,
                format: val => val.avgXpPerMin.toLocaleString() + ` (${val.totalXpPerMinPoints.toFixed(1).toLocaleString()})`,
                sortable: true,
                sort: (a, b) => a.avgXpPerMin - b.avgXpPerMin
            },
            {
                name: 'totalPoints',
                label: 'Total Points',
                align: 'right',
                field: row => row.totalMatchFantasyPoints,
                format: val => `${val.toLocaleString()}`,
                headerStyle: 'font-weight: bold',
                style: 'font-weight: bold',
                sortable: true
            },
        ];

        onMounted(() => {
            if (leagueStore.selectedLeague) {
                localApiService.getPlayerFantasyStats(leagueStore.selectedLeague.id)
                                .then(result => playerFantasyStats.value = result);

            }
        });

        const playerFantasyStatsIndexed = computed(() => {
            return playerFantasyStats.value.map((player, index) => ({ 
                ...player, 
                position: index + 1 
            }));
        })

        watch(() => leagueStore.selectedLeague, (newValue) => {
            if (newValue) {
                if (leagueStore.selectedLeague) {
                    localApiService.getPlayerFantasyStats(leagueStore.selectedLeague.id)
                                    .then(result => playerFantasyStats.value = result);
                }
            }
        });

        return {
            authStore,
            leagueStore,
            playerFantasyStats,
            playerFantasyStatsIndexed,
            columns
        }
    }
}
</script>
  

<style lang="sass">
.fantasy-stats-table
  /* height or max-height is important */

  .q-table__top,
  .q-table__bottom,
  thead tr:first-child th
    /* bg color is important for th; just specify one */
    background-color: #1D1D1D

  thead tr th
    position: sticky
    z-index: 1
  thead tr:first-child th
    top: 0

  /* this is when the loading indicator appears */
  &.q-table--loading thead tr:last-child th
    /* height of all previous header rows */
    top: 48px

  /* prevent scrolling behind sticky top row on focus */
  tbody
    /* height of all previous header rows */
    scroll-margin-top: 48px
</style>
  <!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.debug {
    border: 1px solid red;
    padding: 10px;
}

thead th tr {
    position: sticky;
}

.fantasy-stats-table {
    width: 1400px;
    height: 800px;
}

.left-fixed {
    flex: 0 0 300px;
}

.flex-container {
    display: flex;
    flex-flow: row wrap;
    max-width: 100%;
    padding: 20px;
}

.flex-break {
    flex: 1 0 100% !important
}

.row {
    width: 100%;
}

.row .flex-break {
    height: 0 !important
}

.column .flex-break {
    width: 0 !important
}
</style>