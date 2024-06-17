import type { FantasyLeague } from '@/types/FantasyLeague'
import { defineStore } from 'pinia'

export const useFantasyLeagueStore = defineStore({
  id: 'league',
  state: () => ({
    leagues: null as FantasyLeague[] | null,
    selectedLeague: null as FantasyLeague | null
  }),

  actions: {
    setLeagues(leagues: FantasyLeague[]) {
      this.leagues = leagues
    },

    clearLeagues() {
      this.leagues = null
    },

    setSelectedLeague(league: FantasyLeague) {
      this.selectedLeague = league
    },

    clearSelectedLeague() {
      this.selectedLeague = null
    }
  },

  getters: {
    activeLeagues(): FantasyLeague[] {
      return this.leagues?.filter((league) => league.isActive) ?? []
    },
    allLeagues(): FantasyLeague[] {
      return this.leagues ?? []
    }
  }
})
