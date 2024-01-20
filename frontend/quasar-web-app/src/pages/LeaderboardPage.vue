<template>
    <div class="flex-container">
        <div class="row">
            <leaderboard-component leaderboardTitle="Fantasy Leaderboard" headerValue="Total Fantasy Points" :boardData="fantasyLeaderboardData" />
        </div>
    </div>
</template>
  
<script>
import { ref, onMounted, watch, computed } from 'vue';
import { localApiService } from 'src/services/localApiService';
import { useAuthStore } from 'stores/auth';
import { useLeagueStore } from 'src/stores/league';
import LeaderboardComponent from 'src/components/LeaderboardComponent.vue';

export default {
    name: 'LeaderboardPage',
    components: {
        LeaderboardComponent,
    },
    setup() {
        const authStore = useAuthStore();
        const leagueStore = useLeagueStore();

        const fantasyLeaderboard = ref([]);

        onMounted(() => {
            if (authStore.authenticated && leagueStore.selectedLeague) {
                localApiService.getTopTenDrafts(leagueStore.selectedLeague.id).then(result => fantasyLeaderboard.value = result);

            }
        });

        // Define a computed property to generate a grouped list of players per team
        const fantasyTeamPlayers = computed(() => {
            return Array.from(new Set(fantasyPlayers.value.map(opt => opt.team.name))).map(teamName => {
                return {
                    label: teamName,
                    options: fantasyPlayers.value
                        .filter(opt => opt.team.name === teamName) // Filter team
                        .filter(opt => !selectedPlayerIds.value.some((sel) => sel == opt.id)) // Filter selected players
                        .map(player => (
                            {
                                id: player.id,
                                name: player.dotaAccount.name
                            }
                        )),
                };
            })
        });

        const fantasyLeaderboardData = computed(() => {
            if (!fantasyLeaderboard.value) {
                return [];
            }
            console.log(fantasyLeaderboard.value)
            return fantasyLeaderboard.value.map((leaderboard) => ({
                id: leaderboard.fantasyDraft.id,
                value: leaderboard.totalMatchFantasyPoints,
            }));
        });

        watch(() => authStore.authenticated, (newValue) => {
            if (newValue) {
                if (authStore.authenticated && leagueStore.selectedLeague) {
                    localApiService.getTopTenDrafts(leagueStore.selectedLeague.id).then(result => fantasyLeaderboard.value = result);
                }
            }
        });

        watch(() => leagueStore.selectedLeague, (newValue) => {
            if (newValue) {
                if (authStore.authenticated && leagueStore.selectedLeague) {
                    localApiService.getTopTenDrafts(leagueStore.selectedLeague.id).then(result => fantasyLeaderboard.value = result);
                }
            }
        });

        return {
            authStore,
            leagueStore,
            fantasyLeaderboard,
            fantasyLeaderboardData
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