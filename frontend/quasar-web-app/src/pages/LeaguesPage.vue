<template>
    <div class="flex-container">
        <div class="row">
            <div class="col left-fixed">
                <!-- <q-select v-model="selectedLeague" :options="leagues" option-label="name" option-value="id" dark
                    label-color="white" label="League" @update:model-value="selectLeague" /> -->
                <q-select v-model="selectedSeriesType" :options="seriesTypeOptions" option-label="name" option-value="id"
                    dark label-color="white" label="Series Type" />
            </div>
            <div class="col col-shrink">
                <award-card-component class="award" :name="(playerMostKills.accountName ?? '')" award-title="Most Kills"
                    :description="(playerMostKills.totalKills ?? 0) + ' Kills'" />
            </div>
            <div class="col col-shrink">
                <award-card-component class="award" :name="(playerMostDeaths.accountName ?? '')" award-title="Most Deaths"
                    :description="(playerMostDeaths.totalDeaths ?? 0) + ' Deaths'" />
            </div>
            <div class="col col-shrink">
                <award-card-component class="award" :name="(playerMostAssists.accountName ?? '')" award-title="Most Assists"
                    :description="(playerMostAssists.totalAssists ?? 0) + ' Assists'" />
            </div>
            <div class="col col-shrink">
                <award-card-component class="award" :name="(playerMostLastHits.accountName ?? '')"
                    award-title="Most Last Hits" :description="(playerMostLastHits.totalLastHits ?? 0) + ' Last Hits'" />
            </div>
            <div class="col col-shrink">
                <award-card-component class="award" :name="(playerMostDenies.accountName ?? '')" award-title="Most Denies"
                    :description="(playerMostDenies.totalDenies ?? 0) + ' Denies'" />
            </div>

            <div class="col col-shrink">
                <award-card-component class="award" :name="(playerMostNetworth.accountName ?? '')"
                    award-title="Most Net Worth"
                    :description="(playerMostNetworth.totalNetworth ?? 0) + ' Total Net Worth'" />
            </div>
        </div>
        <div class="row">
            <div class="col col-grow">
                <q-table dark :rows="matchHistoryView" :visible-columns="matchHistoryColumns">
                </q-table>
            </div>
        </div>
    </div>
</template>
  
<script>
import { ref } from 'vue';
import { localApiService } from 'src/services/localApiService';
import { useLeagueStore } from 'src/stores/league';
import AwardCardComponent from 'components/AwardCardComponent.vue';

export default {
    name: 'LeaguesPage',
    components: {
        AwardCardComponent,
    },
    setup() {
        const leagueStore = useLeagueStore();

        // Data variables
        const leagues = ref([]);
        const players = ref([]);
        const teams = ref([]);
        const heroes = ref([]);
        const accounts = ref([]);
        const leagueMatchHistory = ref([]);

        return {
            leagueStore,

            leagues,
            players,
            teams,
            heroes,
            accounts,
            leagueMatchHistory,

            // Render variables
            // selectedLeague: ref(null),
            selectedSeriesType: ref({
                id: "-1",
                name: "All",
            }),
            seriesTypeOptions: [
                {
                    id: "-1",
                    name: "All",
                },
                {
                    id: "0",
                    name: "BO1",
                },
                {
                    id: "1",
                    name: "BO3",
                }
            ],
            matchHistoryColumns: [
                'matchId',
                'seriesId',
                'seriesType',
                'startTime',
                'radiantTeam',
                'direTeam',
                'lobbyType'
            ],
        }
    },
    async beforeMount() {
        try {
            this.leagues = await localApiService.getLeagues();
            this.teams = await localApiService.getTeams();
            this.heroes = await localApiService.getHeroes();
            this.accounts = await localApiService.getAccounts();
            this.loadLeagueData();
        } catch (error) {
            console.error('Error in component:', error);
        }
    },
    computed: {
        playerKillsRollup() {
            const aggregatedScores = this.players
                .reduce((result, player) => {
                    const accountId = player.accountId;

                    if (!result[accountId]) {
                        // If player ID doesn't exist in the result, create an entry with the initial score
                        result[accountId] = {
                            accountId,
                            accountName: this.accounts.find(acc => acc.id === accountId)?.name,
                            totalKills: player.kills,
                            totalDeaths: player.deaths,
                            totalAssists: player.assists,
                            totalLastHits: player.lastHits,
                            totalDenies: player.denies,
                            totalNetworth: player.networth,
                            totalGames: 1
                        };
                    } else {
                        // If player ID already exists, add the score to the existing total
                        result[accountId].totalKills += player.kills;
                        result[accountId].totalDeaths += player.deaths;
                        result[accountId].totalAssists += player.assists;
                        result[accountId].totalLastHits += player.lastHits;
                        result[accountId].totalDenies += player.denies;
                        result[accountId].totalNetworth += player.networth;
                        result[accountId].totalGames += 1;
                    }
                    return result;
                }, {});

            // Convert the object values back to an array
            const aggregatedPlayers = Object.values(aggregatedScores).sort((a, b) => b.totalKills - a.totalKills);

            return aggregatedPlayers;
        },
        playerMostKills() {
            let reduced = {};
            if (this.playerKillsRollup.length > 0) {
                reduced = this.playerKillsRollup.reduce(function (highest, player) {
                    return ((player.totalGames >= 3 && player.totalKills > highest.totalKills)) ? player : highest;
                });
            }
            return reduced;
        },
        playerMostDeaths() {
            let reduced = {};
            if (this.playerKillsRollup.length > 0) {
                reduced = this.playerKillsRollup.reduce(function (highest, player) {
                    return ((player.totalGames >= 3 && player.totalDeaths > highest.totalDeaths)) ? player : highest;
                });
            }
            return reduced;
        },
        playerMostAssists() {
            let reduced = {};
            if (this.playerKillsRollup.length > 0) {
                reduced = this.playerKillsRollup.reduce(function (highest, player) {
                    return ((player.totalGames >= 3 && player.totalAssists > highest.totalAssists)) ? player : highest;
                });
            }
            return reduced;
        },
        playerMostLastHits() {
            let reduced = {};
            if (this.playerKillsRollup.length > 0) {
                reduced = this.playerKillsRollup.reduce(function (highest, player) {
                    return ((player.totalGames >= 3 && player.totalLastHits > highest.totalLastHits)) ? player : highest;
                });
            }
            return reduced;
        },
        playerMostDenies() {
            let reduced = {};
            if (this.playerKillsRollup.length > 0) {
                reduced = this.playerKillsRollup.reduce(function (highest, player) {
                    return ((player.totalGames >= 3 && player.totalDenies > highest.totalDenies)) ? player : highest;
                });
            }
            return reduced;
        },
        playerMostNetworth() {
            let reduced = {};
            if (this.playerKillsRollup.length > 0) {
                reduced = this.playerKillsRollup.reduce(function (highest, player) {
                    return ((player.totalGames >= 3 && player.totalNetworth > highest.totalNetworth)) ? player : highest;
                });
            }
            return reduced;
        },
        matchHistoryView() {
            return this.leagueMatchHistory
                .filter(row => row.seriesType == this.selectedSeriesType.id || this.selectedSeriesType.id == -1 || this.selectedSeriesType == null)
                .map(match => {
                    return {
                        matchId: match.matchId,
                        seriesId: match.seriesId,
                        seriesType: match.seriesType,
                        startTime: new Date(match.startTime * 1000).toLocaleString(),
                        radiantTeam: this.teams.find(team => team.id === match.radiantTeamId)?.name,
                        direTeam: this.teams.find(team => team.id === match.direTeamId)?.name,
                    }
                });
        }
    },
    methods: {
        loadLeagueData() {
            if (this.leagueStore.selectedLeague) {
                localApiService.getLeagueMatchHistory(this.leagueStore.selectedLeague.id)
                    .then(data => {
                        this.leagueMatchHistory = data;
                    })
                    .catch(error => {
                        console.error('Error in component:', error);
                    });
                localApiService.getLeaguePlayerData(this.leagueStore.selectedLeague.id)
                    .then(data => {
                        this.players = data;
                    })
                    .catch(error => {
                        console.error('Error in component:', error);
                    });
            }

        }
    }
}
</script>
  
  <!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.debug {
    border: 1px solid red;
    padding: 10px;
}

.award {
    width: 220px;
    height: 200px;
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