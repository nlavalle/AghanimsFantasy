import { defineStore } from 'pinia'
import type { FantasyLeague } from '@/types/FantasyLeague'
import type { League } from '@/types/League'
import { localApiService } from '@/services/localApiService'
import type { FantasyDraftPoints, FantasyPlayerMatchPoints, FantasyPlayerPoints } from '@/components/Fantasy/fantasyDraft'
import type { FantasyPlayerStats } from '@/types/FantasyPlayerStats'
import type { LeagueMetadata } from '@/types/LeagueMetadata'
import { isDraftOpen, isDraftActive, isDraftFinished, isLeagueActive, isLeagueFinished } from '@/stores/fantasyLeagueUtils'

export const useFantasyLeagueStore = defineStore({
  id: 'league',
  state: () => ({
    fantasyLeaguePollInterval: null as ReturnType<typeof setInterval> | null,
    fantasyLeaguePollFailureCount: 0,
    fantasyViewPollInterval: null as ReturnType<typeof setInterval> | null,
    fantasyViewPollFailureCount: 0,
    leagues: [] as League[],
    fantasyLeagues: [] as FantasyLeague[],
    selectedDraftFantasyLeagueId: null as number | null,
    selectedLeague: {
      league_id: 0,
      is_active: false,
      name: ''
    } as League,
    fantasyPlayersStats: [] as FantasyPlayerStats[],
    fantasyDraftPoints: [] as FantasyDraftPoints[],
    fantasyPlayerPoints: [] as FantasyPlayerPoints[],
    leagueMetadataStats: [] as LeagueMetadata[],
    playerFantasyMatchStats: [] as FantasyPlayerMatchPoints[],
    draftFantasyMatchStats: [] as FantasyPlayerMatchPoints[]
  }),

  actions: {
    startFantasyLeaguePolling() {
      this.fantasyLeaguePollInterval = setInterval(() =>
        this.fetchLeagues()
          .then(() => this.fetchFantasyLeagues())
          .then(() => this.fantasyLeaguePollFailureCount = 0)
          .catch(() => {
            this.fantasyLeaguePollFailureCount++
            if (this.fantasyLeaguePollFailureCount > 3) {
              console.log("Too many failures encountered, stopping fantasy league polling")
              this.stopFantasyLeaguePolling()
            }
          }),
        30000)
    },

    stopFantasyLeaguePolling() {
      if (this.fantasyLeaguePollInterval) {
        clearInterval(this.fantasyLeaguePollInterval);
        this.fantasyLeaguePollInterval = null;
      }
    },

    startFantasyViewPolling() {
      this.fantasyViewPollInterval = setInterval(() =>
        this.fetchFantasyPlayerViewModels()
          ?.then(() => this.fetchFantasyPlayerPoints())
          .then(() => this.fantasyViewPollFailureCount = 0)
          .catch(() => {
            this.fantasyViewPollFailureCount++
            if (this.fantasyViewPollFailureCount > 3) {
              console.log("Too many failures encountered, stopping fantasy view polling")
              this.stopFantasyViewPolling()
            }
          }),
        30000)
    },

    stopFantasyViewPolling() {
      if (this.fantasyViewPollInterval) {
        clearInterval(this.fantasyViewPollInterval);
        this.fantasyViewPollInterval = null;
      }
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
      const fl = this.currentFantasyLeague
      if (fl?.id) {
        return localApiService.getFantasyPlayerViewModels(fl.id)
          .then((result: any) => {
            this.fantasyPlayersStats = result;
          })
      }
    },

    fetchFantasyPlayerPoints() {
      const fl = this.currentFantasyLeague
      if (fl?.id) {
        return localApiService.getPlayerFantasyStats(fl.id)
          .then((result: any) => {
            this.fantasyPlayerPoints = result;
          });
      }
    },

    fetchFantasyLeagueMetadataStats() {
      const fl = this.currentFantasyLeague
      if (fl?.id) {
        return localApiService.getFantasyLeagueMetadataStats(fl.id)
          .then((result: any) => {
            this.leagueMetadataStats = result;
          });
      }
    },

    fetchPlayerFantasyMatchStats() {
      const fl = this.currentFantasyLeague
      if (fl?.id) {
        return localApiService.getPlayerFantasyMatchStats(fl.id)
          .then((result: any) => {
            this.playerFantasyMatchStats = result;
          });
      }
    },

    fetchDraftFantasyMatchStats() {
      const fl = this.currentFantasyLeague
      if (fl?.id) {
        return localApiService.getDraftPlayerFantasyMatchStats(fl.id)
          .then((result: any) => {
            this.draftFantasyMatchStats = result;
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
      this.selectedDraftFantasyLeagueId = null
    },

    setSelectedDraftFantasyLeague(id: number | null) {
      this.selectedDraftFantasyLeagueId = id
    },

    setSelectedLeagueById(leagueId: number) {
      if (leagueId) {
        const league = this.leagues.find(l => l.league_id === leagueId)
        if (league) this.setSelectedLeague(league)
      } else {
        const defaultL = this.defaultSelectedLeague
        if (defaultL) this.setSelectedLeague(defaultL)
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

    // Validation methods — pure functions delegated to fantasyLeagueUtils.ts
    isDraftOpen,
    isDraftActive,
    isDraftFinished,
    isLeagueActive,
    isLeagueFinished,
    isLeagueOpen(league: League) {
      return this.activeFantasyLeagues
        .filter(fantasyLeague => fantasyLeague.leagueId == league.league_id)
        .some(fantasyLeague => isDraftOpen(fantasyLeague));
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

    // currentFantasyLeague: derived from selectedLeague, applies priority rules scoped to that league.
    // If selectedDraftFantasyLeagueId is set (user manually picked a round), that round wins.
    currentFantasyLeague(): FantasyLeague | undefined {
      if (this.selectedDraftFantasyLeagueId !== null) {
        return this.fantasyLeagues.find(fl => fl.id === this.selectedDraftFantasyLeagueId)
      }

      const fls = this.activeFantasyLeagues.filter(fl => fl.leagueId === this.selectedLeague?.league_id)

      const openDrafts = fls.filter(fl => isDraftOpen(fl))
      if (openDrafts.length > 0) {
        return openDrafts.reduce((min, fl) => fl.fantasyDraftLocked < min.fantasyDraftLocked ? fl : min)
      }

      const liveRounds = fls.filter(fl => isDraftActive(fl))
      if (liveRounds.length > 0) {
        return liveRounds.reduce((latest, fl) => fl.leagueStartTime > latest.leagueStartTime ? fl : latest)
      }

      const finished = fls.filter(fl => isDraftFinished(fl))
      if (finished.length > 0) {
        return finished.reduce((latest, fl) => fl.leagueEndTime > latest.leagueEndTime ? fl : latest)
      }

      return fls[0]
    },

    // defaultSelectedLeague: global priority rules across all tournaments to pick the initial league on load.
    // Finds the most urgent FantasyLeague globally, then returns its parent League.
    defaultSelectedLeague(): League | undefined {
      const all = this.activeFantasyLeagues

      let bestFL: FantasyLeague | undefined
      const openDrafts = all.filter(fl => isDraftOpen(fl))
      if (openDrafts.length > 0) {
        bestFL = openDrafts.reduce((min, fl) => fl.fantasyDraftLocked < min.fantasyDraftLocked ? fl : min)
      } else {
        const liveRounds = all.filter(fl => isDraftActive(fl))
        if (liveRounds.length > 0) {
          bestFL = liveRounds.reduce((latest, fl) => fl.leagueStartTime > latest.leagueStartTime ? fl : latest)
        } else {
          const finished = all.filter(fl => isDraftFinished(fl))
          if (finished.length > 0) {
            bestFL = finished.reduce((latest, fl) => fl.leagueEndTime > latest.leagueEndTime ? fl : latest)
          } else {
            bestFL = all[0]
          }
        }
      }

      if (!bestFL) return this.activeLeagues[0]
      return this.leagues.find(l => l.league_id === bestFL!.leagueId) ?? this.activeLeagues[0]
    },

    // defaultFantasyLeague: kept for backwards compatibility during migration.
    // Delegates to defaultSelectedLeague logic but returns the FL, not the League.
    // TODO: remove once all callers have migrated to currentFantasyLeague.
    defaultFantasyLeague(): FantasyLeague | undefined {
      const league = this.defaultSelectedLeague
      if (!league) return undefined
      const fls = this.activeFantasyLeagues.filter(fl => fl.leagueId === league.league_id)
      return fls[0]
    },

    viewMode(): 'draft' | 'live' | 'review' {
      const fl = this.currentFantasyLeague
      if (!fl?.id) return 'review'
      if (isDraftOpen(fl)) return 'draft'
      if (isDraftActive(fl)) return 'live'
      return 'review'
    },

    // otherUrgentLeagues: leagues (tournaments) with a more urgent state than the currently selected one.
    // Used by the alert banner to prompt the user to switch leagues.
    otherUrgentLeagues(): League[] {
      const urgencyScore = (league: League): number => {
        const fls = this.activeFantasyLeagues.filter(fl => fl.leagueId === league.league_id)
        if (fls.some(fl => isDraftOpen(fl))) return 2
        if (fls.some(fl => isDraftActive(fl))) return 1
        return 0
      }
      const currentScore = urgencyScore(this.selectedLeague)
      return this.activeLeagues.filter(l =>
        l.league_id !== this.selectedLeague?.league_id && urgencyScore(l) >= currentScore && urgencyScore(l) > 0
      )
    },

    selectedFantasyDraftPoints(): FantasyDraftPoints | undefined {
      return this.fantasyDraftPoints.find(fdp => fdp.fantasyDraft.fantasyLeagueId == this.currentFantasyLeague?.id)
    }
  }
})
