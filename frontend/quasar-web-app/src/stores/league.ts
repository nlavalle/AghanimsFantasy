import { defineStore } from 'pinia';

interface League {
  id: number,
  name: string,
  isActive: boolean,
}

export const useLeagueStore = defineStore({
  id: 'league',
  state: () => ({
    leagues: null as League[] | null,
    selectedLeague: null as League | null
  }),

  actions: {
    setLeagues(leagues: League[]) {
      this.leagues = leagues;
    },

    clearLeagues() {
      this.leagues = null;
    },

    setSelectedLeague(league: League) {
      this.selectedLeague = league;
    },

    clearSelectedLeague() {
      this.selectedLeague = null;
    },
  },

  getters: {
    activeLeagues(): League[] {
      return this.leagues?.filter(league => league.isActive) ?? []
    }
  }
});
