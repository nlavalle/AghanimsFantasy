import { defineStore } from 'pinia'
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import type { FantasyLeague } from '@/types/FantasyLeague'
import type { League } from '@/types/League'
import { localApiService } from '@/services/localApiService'
import type { FantasyDraftPoints, FantasyPlayer } from '@/components/Fantasy/fantasyDraft'
import type { Leaderboard, LeaderboardStats } from '@/types/Leaderboard';
import type { LeaderboardItem } from '@/types/LeaderboardItem';

export const useFantasyDraftStore = defineStore({
  id: 'fantasyDraft',
  state: () => ({
    leagueStore: useFantasyLeagueStore(),
    fantasyPlayers: [] as FantasyPlayer[],
    fantasyLeaderboardStats: {} as LeaderboardStats,
    fantasyLeaderboard: []
  }),

  actions: {
    fetchLeaderboard() {
      if (this.leagueStore.selectedFantasyLeague.id) {
        return localApiService.getTopTenDrafts(this.leagueStore.selectedFantasyLeague.id)
          .then((result) => (this.fantasyLeaderboard = result))
          .then(() => { this.fetchLeaderboardStats() })
      }
    },

    fetchLeaderboardStats() {
      if (this.leagueStore.selectedFantasyLeague.id) {
        return localApiService.getLeaderboardStats(this.leagueStore.selectedFantasyLeague.id)
          .then((result: any) => {
            this.fantasyLeaderboardStats = result;
          })
      }
    },

    saveFantasyDraft(draftPicks: FantasyPlayer[]) {
      return new Promise((resolve) => {
        localApiService.saveFantasyDraft(
          this.leagueStore.selectedFantasyLeague,
          draftPicks
        ).then((result: any) => {
          resolve(result)
        })
      })
    }
  },

  getters: {
    fantasyLeaderboardData(): LeaderboardItem[] {
      if (!this.fantasyLeaderboard || Object.keys(this.fantasyLeaderboard).length === 0) {
        return []
      }
      return this.fantasyLeaderboard.map((leaderboard: Leaderboard) => ({
        id: leaderboard.fantasyDraft.id,
        isTeam: leaderboard.isTeam,
        teamId: leaderboard.teamId,
        description: leaderboard.discordName,
        value: leaderboard.draftTotalFantasyPoints,
        fantasyDraft: leaderboard.fantasyDraft,
        playerPoints: [
          leaderboard.draftPickOnePoints,
          leaderboard.draftPickTwoPoints,
          leaderboard.draftPickThreePoints,
          leaderboard.draftPickFourPoints,
          leaderboard.draftPickFivePoints
        ]
      } as LeaderboardItem))
    }
  }
})
