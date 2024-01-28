<template>
    <div class="flex-container">
        <div v-if="authenticated" class="row text-white">
            <div class="row justify-evenly">
                <leaderboard-component class="leaderboardComponent" leaderboardTitle="Fantasy Leaderboard" headerName="Draft Player"
                    headerValue="Points" :authenticatedUser="user" :boardData="fantasyLeaderboardData" />
            </div>
        </div>
        <div v-else class="row text-white">
            <span class="not-authenticated">
                Not Authenticated
            </span>
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

        const fantasyLeaderboardData = computed(() => {
            if (!fantasyLeaderboard.value) {
                return [];
            }
            return fantasyLeaderboard.value.map((leaderboard) => ({
                id: leaderboard.fantasyDraft.id,
                isTeam: leaderboard.isTeam,
                teamId: leaderboard.teamId,
                description: leaderboard.discordName,
                value: leaderboard.draftTotalFantasyPoints,
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
    },
    computed: {
        authenticated() { return this.authStore.authenticated; },
        user() { return this.authStore.user ?? {}; }
    },
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

.leaderboardComponent {
    max-width: 700px;
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