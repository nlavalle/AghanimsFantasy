<template>
    <div class="flex-container">
        <div class="row">
            <q-table class="fantasy-stats-table" dark style="height: 800px" title="Fantasy Player Stats" :columns="columns"
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
                style: 'width: 300px',
                sortable: true
            },
            {
                name: 'fantasyPlayerTeam',
                label: 'Team',
                align: 'left',
                field: row => row.fantasyPlayer.team.name,
                format: val => `${val}`,
                sortable: true
            },
            {
                name: 'totalMatches',
                label: 'Matches',
                align: 'right',
                field: row => row.totalMatches,
                format: val => `${val}`,
                sortable: true
            },
            {
                name: 'totalKills',
                label: 'Kills (pts)',
                align: 'left',
                field: row => row.totalKills + ` (${row.totalKillsPoints})`,
                format: val => `${val}`,
                sortable: true
            },
            {
                name: 'totalDeaths',
                label: 'Deaths (pts)',
                align: 'left',
                field: row => row.totalDeaths + ` (${row.totalDeathsPoints})`,
                format: val => `${val}`,
                sortable: true
            },
            {
                name: 'totalAssists',
                label: 'Assists (pts)',
                align: 'left',
                field: row => row.totalAssists + ` (${row.totalAssistsPoints})`,
                format: val => `${val}`,
                sortable: true
            },
            {
                name: 'totalLastHits',
                label: 'LastHits (pts)',
                align: 'left',
                field: row => row.totalLastHits.toLocaleString() + ` (${row.totalLastHitsPoints.toLocaleString()})`,
                format: val => `${val}`,
                sortable: true
            },
            {
                name: 'totalGoldPerMin',
                label: 'GoldPerMin (pts)',
                align: 'left',
                field: row => row.avgGoldPerMin.toLocaleString() + ` (${row.totalGoldPerMinPoints.toLocaleString()})`,
                format: val => `${val}`,
                sortable: true
            },
            {
                name: 'totalXpPerMin',
                label: 'XpPerMin (pts)',
                align: 'left',
                field: row => row.avgXpPerMin.toLocaleString() + ` (${row.totalXpPerMinPoints.toLocaleString()})`,
                format: val => `${val}`,
                sortable: true
            },
            {
                name: 'totalPoints',
                label: 'Total Points',
                align: 'right',
                field: row => row.totalMatchFantasyPoints,
                format: val => `${val.toLocaleString()}`,
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