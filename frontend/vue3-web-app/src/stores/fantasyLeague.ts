import type { FantasyLeague } from '@/types/FantasyLeague'
import type { League } from '@/types/League'
import { defineStore } from 'pinia'

export const useFantasyLeagueStore = defineStore({
  id: 'league',
  state: () => ({
    leagues: null as League[] | null,
    fantasyLeagues: null as FantasyLeague[] | null,
    selectedLeague: undefined as League | undefined,
    selectedFantasyLeague: undefined as FantasyLeague | undefined
  }),

  actions: {
    setLeagues(leagues: League[]) {
      this.leagues = leagues
    },

    setFantasyLeagues(fantasyLeagues: FantasyLeague[]) {
      this.fantasyLeagues = fantasyLeagues
    },

    clearLeagues() {
      this.leagues = null
    },

    clearFantasyLeagues() {
      this.fantasyLeagues = null
    },

    setSelectedLeague(league: League) {
      this.selectedLeague = league
    },

    setSelectedFantasyLeague(fantasyLeagues: FantasyLeague) {
      this.selectedFantasyLeague = fantasyLeagues
    },

    clearSelectedLeague() {
      this.selectedLeague = undefined
    },

    clearSelectedFantasyLeague() {
      this.selectedFantasyLeague = undefined
    },

    // Validation methods
    isDraftOpen(draftLockEpochTimestamp: number) {
      return new Date() < new Date(draftLockEpochTimestamp * 1000);
    },
    isDraftActive(leagueEndTimestamp: number) {
      return new Date() < new Date(leagueEndTimestamp * 1000);
    },
    isLeagueActive(league: League) {
      return this.activeFantasyLeagues
        .filter(fantasyLeague => fantasyLeague.leagueId == league.id)
        .some(fantasyLeague => this.isDraftActive(fantasyLeague.leagueEndTime));
    },
    isLeagueOpen(league: League) {
      return this.activeFantasyLeagues
        .filter(fantasyLeague => fantasyLeague.leagueId == league.id)
        .some(fantasyLeague => this.isDraftOpen(fantasyLeague.fantasyDraftLocked));
    }
  },

  getters: {
    activeLeagues(): League[] {
      return this.leagues?.filter((league) => league.isActive) ?? []
    },
    activeFantasyLeagues(): FantasyLeague[] {
      return this.fantasyLeagues?.filter((fantasyLeague) => fantasyLeague.isActive) ?? []
    },
    allLeagues(): League[] {
      return this.leagues ?? []
    },
    allFantasyLeagues(): FantasyLeague[] {
      return this.fantasyLeagues ?? []
    },
    defaultLeague(): League {
      return this.activeLeagues.reduce((max, current) => {
        return current.id > max.id ? current : max
      }, this.activeLeagues[0])
    },
    defaultFantasyLeague(): FantasyLeague {
      let filteredLeagues = this.activeFantasyLeagues
        .filter(fantasyLeague => fantasyLeague.leagueId == this.selectedLeague?.id ?? true);
      return filteredLeagues
        .reduce((max, current) => {
          return current.fantasyDraftLocked > max.fantasyDraftLocked ? current : max
        }, filteredLeagues[0])
    }
  }
})
