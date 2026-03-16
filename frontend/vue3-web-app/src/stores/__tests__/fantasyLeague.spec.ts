import { setActivePinia, createPinia } from 'pinia'
import { useFantasyLeagueStore } from '../fantasyLeague'
import { beforeEach, afterEach, describe, it, expect, vi } from 'vitest'

// Fixed "now": 2025-01-10T12:00:00Z
const NOW = 1736510400
const PAST = NOW - 7 * 86400    // 7 days ago
const RECENT = NOW - 86400       // 1 day ago
const SOON = NOW + 86400         // 1 day from now
const FUTURE = NOW + 7 * 86400  // 7 days from now

const makeFL = (id: number, leagueId: number, overrides: object = {}) => ({
  id,
  leagueId,
  name: `Round ${id}`,
  isActive: true,
  fantasyDraftLocked: 0,
  leagueStartTime: 0,
  leagueEndTime: 0,
  ...overrides
})

const makeLeague = (id: number, overrides: object = {}) => ({
  league_id: id,
  name: `Tournament ${id}`,
  is_active: true,
  tier: 1,
  region: 1,
  start_timestamp: 0,
  end_timestamp: 0,
  ...overrides
})

describe('fantasyLeague store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.useFakeTimers()
    vi.setSystemTime(new Date(NOW * 1000))
  })

  afterEach(() => {
    vi.useRealTimers()
  })

  // ---------------------------------------------------------------------------
  // 1. Primitive methods
  // ---------------------------------------------------------------------------

  describe('isDraftOpen', () => {
    it('returns true when fantasyDraftLocked is in the future', () => {
      const store = useFantasyLeagueStore()
      const fl = makeFL(1, 1, { fantasyDraftLocked: FUTURE })
      expect(store.isDraftOpen(fl as any)).toBe(true)
    })

    it('returns false when fantasyDraftLocked is in the past', () => {
      const store = useFantasyLeagueStore()
      const fl = makeFL(1, 1, { fantasyDraftLocked: PAST })
      expect(store.isDraftOpen(fl as any)).toBe(false)
    })

    it('returns false exactly at the lock time', () => {
      const store = useFantasyLeagueStore()
      const fl = makeFL(1, 1, { fantasyDraftLocked: NOW })
      expect(store.isDraftOpen(fl as any)).toBe(false)
    })
  })

  describe('isDraftActive', () => {
    it('returns true when now is between leagueStartTime and leagueEndTime', () => {
      const store = useFantasyLeagueStore()
      const fl = makeFL(1, 1, { leagueStartTime: PAST, leagueEndTime: FUTURE })
      expect(store.isDraftActive(fl as any)).toBe(true)
    })

    it('returns false when round has not started yet', () => {
      const store = useFantasyLeagueStore()
      const fl = makeFL(1, 1, { leagueStartTime: SOON, leagueEndTime: FUTURE })
      expect(store.isDraftActive(fl as any)).toBe(false)
    })

    it('returns false when round has ended', () => {
      const store = useFantasyLeagueStore()
      const fl = makeFL(1, 1, { leagueStartTime: PAST, leagueEndTime: RECENT })
      expect(store.isDraftActive(fl as any)).toBe(false)
    })
  })

  describe('isDraftFinished', () => {
    it('returns true when leagueEndTime is in the past', () => {
      const store = useFantasyLeagueStore()
      const fl = makeFL(1, 1, { leagueEndTime: PAST })
      expect(store.isDraftFinished(fl as any)).toBe(true)
    })

    it('returns false when leagueEndTime is in the future', () => {
      const store = useFantasyLeagueStore()
      const fl = makeFL(1, 1, { leagueEndTime: FUTURE })
      expect(store.isDraftFinished(fl as any)).toBe(false)
    })
  })

  describe('isLeagueOpen', () => {
    it('returns true when at least one fantasy league in that tournament has an open draft', () => {
      const store = useFantasyLeagueStore()
      const league = makeLeague(10)
      store.leagues = [league as any]
      store.fantasyLeagues = [
        makeFL(1, 10, { fantasyDraftLocked: PAST, isActive: true }) as any,
        makeFL(2, 10, { fantasyDraftLocked: FUTURE, isActive: true }) as any,
      ]
      expect(store.isLeagueOpen(league as any)).toBe(true)
    })

    it('returns false when all fantasy leagues for that tournament have closed drafts', () => {
      const store = useFantasyLeagueStore()
      const league = makeLeague(10)
      store.leagues = [league as any]
      store.fantasyLeagues = [
        makeFL(1, 10, { fantasyDraftLocked: PAST, isActive: true }) as any,
      ]
      expect(store.isLeagueOpen(league as any)).toBe(false)
    })
  })

  describe('isLeagueFinished', () => {
    it('returns true when league end_timestamp is in the past', () => {
      const store = useFantasyLeagueStore()
      const league = makeLeague(10, { end_timestamp: PAST })
      expect(store.isLeagueFinished(league as any)).toBe(true)
    })

    it('returns false when league end_timestamp is in the future', () => {
      const store = useFantasyLeagueStore()
      const league = makeLeague(10, { end_timestamp: FUTURE })
      expect(store.isLeagueFinished(league as any)).toBe(false)
    })
  })

  // ---------------------------------------------------------------------------
  // 2. defaultFantasyLeague — single tournament scenarios
  // ---------------------------------------------------------------------------

  describe('defaultFantasyLeague — single tournament', () => {
    it('Row 1: pre-tournament, 1 open draft — returns that round', () => {
      const store = useFantasyLeagueStore()
      const league = makeLeague(10, { start_timestamp: SOON, end_timestamp: FUTURE })
      const r1 = makeFL(1, 10, { fantasyDraftLocked: SOON, leagueStartTime: SOON, leagueEndTime: FUTURE })
      store.leagues = [league as any]
      store.fantasyLeagues = [r1 as any]
      store.selectedLeague = league as any
      expect(store.defaultFantasyLeague?.id).toBe(1)
    })

    it('Row 1b: pre-tournament, 3 open rounds — returns the one closing soonest', () => {
      const store = useFantasyLeagueStore()
      const league = makeLeague(10)
      const r1 = makeFL(1, 10, { fantasyDraftLocked: SOON, leagueStartTime: FUTURE, leagueEndTime: FUTURE })
      const r2 = makeFL(2, 10, { fantasyDraftLocked: SOON + 3600, leagueStartTime: FUTURE, leagueEndTime: FUTURE })
      const r3 = makeFL(3, 10, { fantasyDraftLocked: FUTURE, leagueStartTime: FUTURE, leagueEndTime: FUTURE })
      store.leagues = [league as any]
      store.fantasyLeagues = [r1, r2, r3] as any
      store.selectedLeague = league as any
      expect(store.defaultFantasyLeague?.id).toBe(1)
    })

    it('Row 3: draft closed, round live — returns that live round', () => {
      const store = useFantasyLeagueStore()
      const league = makeLeague(10)
      const r1 = makeFL(1, 10, { fantasyDraftLocked: PAST, leagueStartTime: PAST, leagueEndTime: FUTURE })
      store.leagues = [league as any]
      store.fantasyLeagues = [r1 as any]
      store.selectedLeague = league as any
      expect(store.defaultFantasyLeague?.id).toBe(1)
    })

    // TODO: fails — current logic returns R1 (live) instead of R2 (open draft)
    it('Row 5: R1 live + R2 draft open — should return R2 (open draft takes priority)', () => {
      const store = useFantasyLeagueStore()
      const league = makeLeague(10)
      const r1 = makeFL(1, 10, { fantasyDraftLocked: PAST, leagueStartTime: PAST, leagueEndTime: FUTURE })
      const r2 = makeFL(2, 10, { fantasyDraftLocked: FUTURE, leagueStartTime: FUTURE, leagueEndTime: FUTURE })
      store.leagues = [league as any]
      store.fantasyLeagues = [r1, r2] as any
      store.selectedLeague = league as any
      expect(store.defaultFantasyLeague?.id).toBe(2)
    })

    it('Row 7: single finished round — returns it', () => {
      const store = useFantasyLeagueStore()
      const league = makeLeague(10)
      const r1 = makeFL(1, 10, { fantasyDraftLocked: PAST, leagueStartTime: PAST, leagueEndTime: PAST })
      store.leagues = [league as any]
      store.fantasyLeagues = [r1 as any]
      store.selectedLeague = league as any
      expect(store.defaultFantasyLeague?.id).toBe(1)
    })

    it('Row 10: 3 finished rounds — returns most recently concluded (highest leagueEndTime)', () => {
      const store = useFantasyLeagueStore()
      const league = makeLeague(10)
      const r1 = makeFL(1, 10, { fantasyDraftLocked: PAST - 10, leagueStartTime: PAST - 20, leagueEndTime: PAST - 5 })
      const r2 = makeFL(2, 10, { fantasyDraftLocked: RECENT - 10, leagueStartTime: RECENT - 20, leagueEndTime: RECENT - 5 })
      const r3 = makeFL(3, 10, { fantasyDraftLocked: RECENT, leagueStartTime: RECENT - 10, leagueEndTime: RECENT })
      store.leagues = [league as any]
      store.fantasyLeagues = [r1, r2, r3] as any
      store.selectedLeague = league as any
      expect(store.defaultFantasyLeague?.id).toBe(3)
    })
  })

  // ---------------------------------------------------------------------------
  // 3. defaultFantasyLeague — multi-tournament scenarios
  // ---------------------------------------------------------------------------

  describe('defaultFantasyLeague — multi-tournament', () => {
    // TODO: fails — current logic filters by selectedLeague.league_id, ignoring TournB
    it('Row 11: 2 open drafts from different tournaments — returns the one closing soonest', () => {
      const store = useFantasyLeagueStore()
      const leagueA = makeLeague(10)
      const leagueB = makeLeague(20)
      const flA = makeFL(1, 10, { fantasyDraftLocked: SOON, leagueStartTime: FUTURE, leagueEndTime: FUTURE })
      const flB = makeFL(2, 20, { fantasyDraftLocked: FUTURE, leagueStartTime: FUTURE, leagueEndTime: FUTURE })
      store.leagues = [leagueA, leagueB] as any
      store.fantasyLeagues = [flA, flB] as any
      store.selectedLeague = leagueA as any
      expect(store.defaultFantasyLeague?.id).toBe(1)
    })

    // TODO: fails — current logic only sees TournA (selectedLeague), misses TournB open draft
    it('Row 12: TournB draft open + TournA live — should return TournB (open draft priority)', () => {
      const store = useFantasyLeagueStore()
      const leagueA = makeLeague(10)
      const leagueB = makeLeague(20)
      const flA = makeFL(1, 10, { fantasyDraftLocked: PAST, leagueStartTime: PAST, leagueEndTime: FUTURE })
      const flB = makeFL(2, 20, { fantasyDraftLocked: FUTURE, leagueStartTime: FUTURE, leagueEndTime: FUTURE })
      store.leagues = [leagueA, leagueB] as any
      store.fantasyLeagues = [flA, flB] as any
      store.selectedLeague = leagueA as any
      expect(store.defaultFantasyLeague?.id).toBe(2)
    })

    // TODO: fails — current logic only sees TournA, can't compare across tournaments
    it('Row 13: 2 concurrent live rounds — returns most recently started', () => {
      const store = useFantasyLeagueStore()
      const leagueA = makeLeague(10)
      const leagueB = makeLeague(20)
      // TournA-R3 started yesterday, TournB-R1 started today (more recent)
      const flA = makeFL(1, 10, { fantasyDraftLocked: PAST, leagueStartTime: RECENT, leagueEndTime: FUTURE })
      const flB = makeFL(2, 20, { fantasyDraftLocked: PAST, leagueStartTime: NOW - 3600, leagueEndTime: FUTURE })
      store.leagues = [leagueA, leagueB] as any
      store.fantasyLeagues = [flA, flB] as any
      store.selectedLeague = leagueA as any
      expect(store.defaultFantasyLeague?.id).toBe(2)
    })
  })
})
