<template>
  <v-container class="d-flex justify-evenly">
    <v-row v-if="authenticated">
      <leaderboard-component class="leaderboardComponent" leaderboardTitle="Fantasy Leaderboard"
        headerName="Draft Player" headerValue="Points" :authenticatedUser="user" :boardData="fantasyLeaderboardData" />
    </v-row>
    <v-row v-else>
      <span class="not-authenticated"> Not Authenticated </span>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue'
import { VContainer, VRow } from 'vuetify/components'
import { localApiService } from '@/services/localApiService'
import { useAuthStore, type User } from '@/stores/auth'
import { useFantasyLeagueStore } from '@/stores/fantasyLeague'
import LeaderboardComponent from '@/components/Fantasy/LeaderboardComponent.vue'
import type { LeaderboardItem } from '@/types/LeaderboardItem'

const authStore = useAuthStore()
const leagueStore = useFantasyLeagueStore()

const fantasyLeaderboard = ref([])

interface Leaderboard {
  fantasyDraft: {
    id: number,
  },
  isTeam: boolean,
  discordName: string,
  teamId: number,
  draftTotalFantasyPoints: number
}

onMounted(() => {
  if (authStore.authenticated && leagueStore.selectedLeague) {
    localApiService
      .getTopTenDrafts(leagueStore.selectedLeague.id)
      .then((result) => (fantasyLeaderboard.value = result))
  }
})

const fantasyLeaderboardData = computed(() => {
  if (!fantasyLeaderboard.value || Object.keys(fantasyLeaderboard.value).length === 0) {
    return []
  }
  return fantasyLeaderboard.value.map((leaderboard: Leaderboard) => ({
    id: leaderboard.fantasyDraft.id,
    isTeam: leaderboard.isTeam,
    teamId: leaderboard.teamId,
    description: leaderboard.discordName,
    value: leaderboard.draftTotalFantasyPoints
  } as LeaderboardItem))
})

const authenticated = computed(() => {
  return authStore.authenticated
})

const user = computed(() => {
  return authStore.user as User
})

watch(
  () => authStore.authenticated,
  (newValue) => {
    if (newValue) {
      if (authStore.authenticated && leagueStore.selectedLeague) {
        localApiService
          .getTopTenDrafts(leagueStore.selectedLeague.id)
          .then((result) => (fantasyLeaderboard.value = result))
      }
    }
  }
)

watch(
  () => leagueStore.selectedLeague,
  (newValue) => {
    if (newValue) {
      if (authStore.authenticated && leagueStore.selectedLeague) {
        localApiService
          .getTopTenDrafts(leagueStore.selectedLeague.id)
          .then((result) => (fantasyLeaderboard.value = result))
      }
    }
  }
)
</script>

<style scoped>
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