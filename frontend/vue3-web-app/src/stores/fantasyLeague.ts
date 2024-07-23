import type { FantasyLeague } from '@/types/FantasyLeague'
import { defineStore } from 'pinia'

export const useFantasyLeagueStore = defineStore({
  id: 'league',
  state: () => ({
    leagues: null as FantasyLeague[] | null,
    selectedLeague: undefined as FantasyLeague | undefined
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
      this.selectedLeague = undefined
    }
  },

  getters: {
    activeLeagues(): FantasyLeague[] {
      return this.leagues?.filter((league) => league.isActive) ?? []
    },
    allLeagues(): FantasyLeague[] {
      return this.leagues ?? []
    },
    defaultLeague(): FantasyLeague {
      return this.activeLeagues.reduce((max, current) => {
        return current.id > max.id ? current : max
      }, this.activeLeagues[0])
    }
  }
})
