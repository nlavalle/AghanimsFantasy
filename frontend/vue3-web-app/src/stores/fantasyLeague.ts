import { defineStore } from 'pinia'
import type { FantasyLeague } from '@/types/FantasyLeague'
import type { League } from '@/types/League'
import { localApiService } from '@/services/localApiService'
import type { FantasyDraftPoints, FantasyPlayerPoints } from '@/components/Fantasy/fantasyDraft'
import type { FantasyPlayerStats } from '@/types/FantasyPlayerStats'

export const useFantasyLeagueStore = defineStore({
  id: 'league',
  state: () => ({
    fantasyLeaguePollInterval: 0,
    fantasyLeaguePollFailureCount: 0,
    fantasyViewPollInterval: 0,
    fantasyViewPollFailureCount: 0,
    leagues: [] as League[],
    fantasyLeagues: [] as FantasyLeague[],
    selectedLeague: {
      league_id: 0,
      is_active: false,
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
    fantasyPlayersStats: [] as FantasyPlayerStats[],
    fantasyDraftPoints: [] as FantasyDraftPoints[],
    fantasyPlayerPoints: [] as FantasyPlayerPoints[]
  }),

  actions: {
    startFantasyLeaguePolling() {
      this.fantasyLeaguePollInterval = setInterval(() =>
        this.fetchLeagues()
          .then(() => this.fetchFantasyLeagues())
          .then(() => this.fantasyLeaguePollFailureCount = 0) // reset on success
          .catch(() => {
            this.fantasyLeaguePollFailureCount++
            if (this.fantasyLeaguePollFailureCount > 3) {
              console.log("Too many failures encountered, stopping fantasy league polling")
              this.stopFantasyLeaguePolling()
            }
          }),
        30000) // poll every 30sec
    },

    stopFantasyLeaguePolling() {
      clearInterval(this.fantasyLeaguePollInterval);
    },

    startFantasyViewPolling() {
      this.fantasyViewPollInterval = setInterval(() =>
        this.fetchFantasyPlayerViewModels()
          ?.then(() => this.fetchFantasyPlayerPoints())
          .then(() => this.fantasyViewPollFailureCount = 0) // reset on success
          .catch(() => {
            this.fantasyViewPollFailureCount++
            if (this.fantasyViewPollFailureCount > 3) {
              console.log("Too many failures encountered, stopping fantasy view polling")
              this.stopFantasyViewPolling()
            }
          }),
        30000) // poll every 30sec
    },

    stopFantasyViewPolling() {
      clearInterval(this.fantasyViewPollInterval);
    },

    fetchLeagues() {
      return localApiService.getLeagues().then((leagueResult: any) => {
        this.setLeagues(leagueResult);
      })
    },

    fetchFantasyLeagues() {
      return localApiService.getFantasyLeagues()
        .then((fantasyLeagueResult: any) => {
          this.setFantasyLeagues(fantasyLeagueResult);
        })
    },

    fetchFantasyDraftPoints() {
      if (this.selectedLeague) {
        return localApiService.getUserDraftPoints(this.selectedLeague.league_id).then((draftResult: FantasyDraftPoints[]) => {
          this.fantasyDraftPoints = draftResult;
        })
      }
    },

    fetchFantasyPlayerViewModels() {
      if (this.selectedFantasyLeague.id) {
        return localApiService.getFantasyPlayerViewModels(this.selectedFantasyLeague.id)
          .then((result: any) => {
            this.fantasyPlayersStats = result;
          })
      }
    },

    fetchFantasyPlayerPoints() {
      if (!this.selectedFantasyLeague) return
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

    setSelectedFantasyLeagueId(selectFantasyLeagueId: number) {
      if (selectFantasyLeagueId) {
        let fantasyLeagueLookup = this.fantasyLeagues.find(fl => fl.id == selectFantasyLeagueId)
        if (fantasyLeagueLookup) {
          let leagueLookup = this.leagues.find(l => l.league_id == fantasyLeagueLookup.leagueId)
          if (leagueLookup) {
            this.setSelectedLeague(leagueLookup);
            this.setSelectedFantasyLeague(fantasyLeagueLookup);
          }
        }
      } else if (this.selectedFantasyLeague.id == 0) {
        this.setSelectedLeague(this.defaultLeague);
        this.setSelectedFantasyLeague(this.defaultFantasyLeague);
      }
    },

    clearSelectedLeague() {
      this.selectedLeague = {
        league_id: 0,
        is_active: false,
        name: '',
        region: 0,
        tier: 0,
        start_timestamp: 0,
        end_timestamp: 0
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
    isDraftOpen(fantasyLeague: FantasyLeague) {
      return new Date() < new Date(fantasyLeague.fantasyDraftLocked * 1000);
    },
    isDraftActive(fantasyLeague: FantasyLeague) {
      return new Date() >= new Date(fantasyLeague.leagueStartTime * 1000) &&
        new Date() <= new Date(fantasyLeague.leagueEndTime * 1000);
    },
    isDraftFinished(fantasyLeague: FantasyLeague) {
      return new Date() > new Date(fantasyLeague.leagueEndTime * 1000);
    },
    isLeagueActive(league: League) {
      return new Date() >= new Date(league.start_timestamp * 1000) &&
        new Date() <= new Date(league.end_timestamp * 1000);
    },
    isLeagueOpen(league: League) {
      return this.activeFantasyLeagues
        .filter(fantasyLeague => fantasyLeague.leagueId == league.league_id)
        .some(fantasyLeague => this.isDraftOpen(fantasyLeague));
    },
    isLeagueFinished(league: League) {
      return new Date() > new Date(league.end_timestamp * 1000);
    }
  },

  getters: {
    activeLeagues(): League[] {
      return this.leagues?.filter((league) => league.is_active) ?? []
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
        return current.start_timestamp > max.start_timestamp ? current : max
      }, this.activeLeagues[0])
    },
    defaultFantasyLeague(): FantasyLeague {
      let filteredLeagues = this.activeFantasyLeagues
        .filter(fantasyLeague => fantasyLeague.leagueId == this.selectedLeague?.league_id);
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
