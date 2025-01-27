import { defineStore } from 'pinia'
import type { FantasyLeague } from '@/types/FantasyLeague'
import type { League } from '@/types/League'
import { localApiService } from '@/services/localApiService'
import type { FantasyDraftPoints, FantasyPlayerPoints } from '@/components/Fantasy/fantasyDraft'

export const useFantasyLeagueStore = defineStore({
  id: 'league',
  state: () => ({
    leagues: [] as League[],
    fantasyLeagues: [] as FantasyLeague[],
    selectedLeague: {
      id: 0,
      isActive: false,
      name: ''
    } as League,
    selectedFantasyLeague: {
      id: 0,
      leagueId: 0,
      isActive: false,
      name: '',
      fantasyDraftLocked: 0,
      leagueStartTime: 0,
      leagueEndTime: 0
    } as FantasyLeague,
    fantasyDraftPoints: [] as FantasyDraftPoints[],
    fantasyPlayerPoints: [] as FantasyPlayerPoints[]
  }),

  actions: {
    fetchLeagues() {
      return localApiService.getLeagues().then((leagueResult: any) => {
        this.setLeagues(leagueResult);
        if (this.selectedLeague.id == 0) this.setSelectedLeague(this.defaultLeague);
      })
    },

    fetchFantasyLeagues(selectFantasyLeagueId: number | undefined) {
      if (this.leagues.length == 0) this.fetchLeagues();
      return localApiService.getFantasyLeagues()
        .then((fantasyLeagueResult: any) => {
          this.setFantasyLeagues(fantasyLeagueResult);
          if (selectFantasyLeagueId) {
            let fantasyLeagueLookup = this.fantasyLeagues.find(fl => fl.id == selectFantasyLeagueId)
            if (fantasyLeagueLookup) {
              let leagueLookup = this.leagues.find(l => l.id == fantasyLeagueLookup.leagueId)
              if (leagueLookup) {
                this.setSelectedLeague(leagueLookup);
                this.setSelectedFantasyLeague(fantasyLeagueLookup);
              }
            }
          } else if (this.selectedFantasyLeague.id == 0) {
            this.setSelectedFantasyLeague(this.defaultFantasyLeague);
          }
        })
        .then(() => {
          this.fetchFantasyPlayerPoints();
        })
    },

    fetchFantasyDraftPoints() {
      if (this.selectedLeague) {
        return localApiService.getUserDraftPoints(this.selectedLeague.id).then((draftResult: FantasyDraftPoints[]) => {
          this.fantasyDraftPoints = draftResult;
        })
      }
    },

    fetchFantasyPlayerPoints() {
      if (this.selectedFantasyLeague.id) {
        return localApiService.getPlayerFantasyStats(this.selectedFantasyLeague.id)
          .then((result: any) => {
            this.fantasyPlayerPoints = result;
          });
      }
    },

    setLeagues(leagues: League[]) {
      this.leagues = leagues
    },

    setFantasyLeagues(fantasyLeagues: FantasyLeague[]) {
      this.fantasyLeagues = fantasyLeagues
    },

    clearLeagues() {
      this.leagues = []
    },

    clearFantasyLeagues() {
      this.fantasyLeagues = []
    },

    setSelectedLeague(league: League) {
      this.selectedLeague = league
    },

    setSelectedFantasyLeague(fantasyLeagues: FantasyLeague) {
      this.selectedFantasyLeague = fantasyLeagues
    },

    clearSelectedLeague() {
      this.selectedLeague = {
        id: 0,
        isActive: false,
        name: ''
      }
    },

    clearSelectedFantasyLeague() {
      this.selectedFantasyLeague = {
        id: 0,
        leagueId: 0,
        isActive: false,
        name: '',
        fantasyDraftLocked: 0,
        leagueStartTime: 0,
        leagueEndTime: 0
      }
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
        .filter(fantasyLeague => fantasyLeague.leagueId == this.selectedLeague?.id);
      return filteredLeagues
        .reduce((max, current) => {
          // Scenarios
          if (
            // if any are active take active fantasy league
            new Date() >= new Date(current.leagueStartTime * 1000) &&
            new Date() < new Date(current.leagueEndTime * 1000)
          ) {
            return current
          } else if (
            // if none are active but the fantasy hasn't started take the earliest
            new Date() < new Date(current.leagueStartTime * 1000) &&
            new Date() < new Date(max.leagueStartTime * 1000)
          ) {
            return current.fantasyDraftLocked < max.fantasyDraftLocked ? current : max
          } else if (
            // if none are active but the fantasy has finished take the latest
            new Date() > new Date(current.leagueEndTime * 1000) &&
            new Date() > new Date(max.leagueEndTime * 1000)
          ) {
            return current.fantasyDraftLocked > max.fantasyDraftLocked ? current : max
          } else {
            // idk why this should trigger
            return max
          }
        }, filteredLeagues[0])
    },
    selectedFantasyDraftPoints(): FantasyDraftPoints | undefined {
      return this.fantasyDraftPoints.find(fdp => fdp.fantasyDraft.fantasyLeagueId == this.selectedFantasyLeague.id)
    }
  }
})
