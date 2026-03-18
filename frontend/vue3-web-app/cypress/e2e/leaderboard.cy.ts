// Leaderboard page — round pills, per-round points, all-rounds totals, hover tooltip
//
// Fixture: leaderboard/league-leaderboard-two-rounds.json
//   Round 1 (FL id=1): UserA=80.0, UserB=60.0
//   Round 2 (FL id=2): UserA=45.0, UserB=70.0
//   All-rounds totals: UserA=125.0, UserB=130.0  (UserB leads all-rounds despite losing R1)
//
// Round 1 pick breakdown for UserA: Pick1=50.0, Pick2=30.0
import { FROZEN_NOW_MS, FROZEN_NOW } from '../support/constants'

// ---------------------------------------------------------------------------
// Shared setup: one open (unlocked) round — draft still accepting picks
// ---------------------------------------------------------------------------
function setupOpenRoundLeaderboard(now: number) {
  const PAST   = now - 7 * 86400
  const FUTURE = now + 7 * 86400

  const league = { league_id: 10, name: 'TI 2025', is_active: true, tier: 1, region: 1, start_timestamp: PAST, end_timestamp: FUTURE }
  const round1 = { id: 1, leagueId: 10, name: 'Round 1', isActive: true, fantasyDraftLocked: FUTURE, leagueStartTime: FUTURE, leagueEndTime: FUTURE + 86400 }

  cy.intercept('GET', '/api/league*', { body: [league] }).as('getLeagues')
  cy.intercept('GET', '/api/FantasyLeague', { body: [round1] }).as('getFantasyLeagues')
  cy.intercept('GET', '/api/fantasyplayer/fantasyleague/**', { body: [] }).as('getPlayers')
  cy.intercept('GET', '/api/fantasydraft/**/drafts/points', { body: [] }).as('getDraftPoints')
  cy.intercept('GET', '/api/fantasyleague/*/players/points', { body: [] }).as('getPlayerStats')
  cy.intercept('GET', '/api/league/10/leaderboard', { fixture: 'leaderboard/league-leaderboard-two-rounds.json' }).as('getLeaderboard')
  cy.intercept('GET', '/api/fantasyleague/*/drafters/stats', { body: {} }).as('getLeaderboardStats')
}

// ---------------------------------------------------------------------------
// Shared setup: two-round concluded league so both rounds have data
// ---------------------------------------------------------------------------
function setupTwoRoundLeaderboard(now: number) {
  const PAST = now - 14 * 86400
  const MID  = now - 7 * 86400
  const RECENT = now - 86400

  const league = { league_id: 10, name: 'TI 2025', is_active: true, tier: 1, region: 1, start_timestamp: PAST, end_timestamp: RECENT }

  const round1 = { id: 1, leagueId: 10, name: 'Round 1', isActive: true, fantasyDraftLocked: PAST, leagueStartTime: PAST, leagueEndTime: MID }
  const round2 = { id: 2, leagueId: 10, name: 'Round 2', isActive: true, fantasyDraftLocked: MID,  leagueStartTime: MID,  leagueEndTime: RECENT }

  cy.intercept('GET', '/api/league*', { body: [league] }).as('getLeagues')
  cy.intercept('GET', '/api/FantasyLeague', { body: [round1, round2] }).as('getFantasyLeagues')
  cy.intercept('GET', '/api/fantasyplayer/fantasyleague/**', { body: [] }).as('getPlayers')
  cy.intercept('GET', '/api/fantasydraft/**/drafts/points', { body: [] }).as('getDraftPoints')
  cy.intercept('GET', '/api/fantasyleague/*/players/points', { body: [] }).as('getPlayerStats')
  cy.intercept('GET', '/api/league/10/leaderboard', { fixture: 'leaderboard/league-leaderboard-two-rounds.json' }).as('getLeaderboard')
  cy.intercept('GET', '/api/fantasyleague/*/drafters/stats', { body: {} }).as('getLeaderboardStats')
}

// ---------------------------------------------------------------------------
// Round pills
// ---------------------------------------------------------------------------
describe('Leaderboard — round pills', () => {
  beforeEach(() => {
    cy.clock(FROZEN_NOW_MS, ['Date'])
    cy.mockAuthenticated(false)
    setupTwoRoundLeaderboard(FROZEN_NOW)
    cy.visit('/leaderboard')
  })

  it('shows an All Rounds pill', () => {
    cy.contains('.round-pill', 'All Rounds').should('exist')
  })

  it('shows a pill for each round', () => {
    cy.contains('.round-pill', 'Round 1').should('exist')
    cy.contains('.round-pill', 'Round 2').should('exist')
    // Exact check: only Round 1 and Round 2, not All Rounds
    cy.get('.round-pill').filter((_, el) => /^Round \d+$/.test(el.textContent?.trim() ?? '')).should('have.length', 2)
  })

  it('the most recent round pill is active by default', () => {
    cy.contains('.round-pill', 'Round 2').should('have.class', 'active')
    cy.contains('.round-pill', 'Round 1').should('not.have.class', 'active')
    cy.contains('.round-pill', 'All Rounds').should('not.have.class', 'active')
  })

  it('clicking All Rounds activates that pill', () => {
    cy.contains('.round-pill', 'All Rounds').click()
    cy.contains('.round-pill', 'All Rounds').should('have.class', 'active')
  })

  it('clicking Round 1 activates that pill and deactivates Round 2', () => {
    cy.contains('.round-pill', 'Round 1').click()
    cy.contains('.round-pill', 'Round 1').should('have.class', 'active')
    cy.contains('.round-pill', 'Round 2').should('not.have.class', 'active')
  })
})

// ---------------------------------------------------------------------------
// Per-round points column
// ---------------------------------------------------------------------------
describe('Leaderboard — per-round points', () => {
  beforeEach(() => {
    cy.clock(FROZEN_NOW_MS, ['Date'])
    cy.mockAuthenticated(false)
    setupTwoRoundLeaderboard(FROZEN_NOW)
    cy.visit('/leaderboard')
  })

  it('shows a Round Pts column when viewing a single round', () => {
    cy.get('.leaderboard-table th').should('contain', 'Round Pts')
  })

  it('Round 2 default view: UserB(70) ranks above UserA(45)', () => {
    cy.get('.drafter-row').eq(0).should('contain', 'UserB')
    cy.get('.drafter-row').eq(1).should('contain', 'UserA')
  })

  it('Round 2 default view: round pts come from draftTotalFantasyPoints (not a pick sub-field)', () => {
    // UserB round-2 total is 70.0; UserA is 45.0
    cy.get('.drafter-row').eq(0).find('.col-pts').first().should('contain', '70.0')
    cy.get('.drafter-row').eq(1).find('.col-pts').first().should('contain', '45.0')
  })

  it('switching to Round 1: UserA(80) ranks above UserB(60)', () => {
    cy.contains('.round-pill', 'Round 1').click()
    cy.get('.drafter-row').eq(0).should('contain', 'UserA')
    cy.get('.drafter-row').eq(1).should('contain', 'UserB')
  })

  it('Round 1 round pts match fixture values (80.0 and 60.0)', () => {
    cy.contains('.round-pill', 'Round 1').click()
    cy.get('.drafter-row').eq(0).find('.col-pts').first().should('contain', '80.0')
    cy.get('.drafter-row').eq(1).find('.col-pts').first().should('contain', '60.0')
  })
})

// ---------------------------------------------------------------------------
// All-rounds total column
// ---------------------------------------------------------------------------
describe('Leaderboard — all-rounds totals', () => {
  beforeEach(() => {
    cy.clock(FROZEN_NOW_MS, ['Date'])
    cy.mockAuthenticated(false)
    setupTwoRoundLeaderboard(FROZEN_NOW)
    cy.visit('/leaderboard')
  })

  it('Total Pts column is always present', () => {
    cy.get('.leaderboard-table th').should('contain', 'Total Pts')
  })

  it('per-round view: Total Pts sums both rounds for each user', () => {
    // UserA: 80 + 45 = 125.0, UserB: 60 + 70 = 130.0
    // Default is Round 2; UserB is row 0, UserA is row 1
    cy.get('.drafter-row').eq(0).find('.col-pts').last().should('contain', '130.0')
    cy.get('.drafter-row').eq(1).find('.col-pts').last().should('contain', '125.0')
  })

  it('All Rounds view: hides Round Pts column, shows only Total Pts', () => {
    cy.contains('.round-pill', 'All Rounds').click()
    cy.get('.leaderboard-table th').should('not.contain', 'Round Pts')
    cy.get('.leaderboard-table th').should('contain', 'Total Pts')
  })

  it('All Rounds view: UserB(130) ranks above UserA(125)', () => {
    cy.contains('.round-pill', 'All Rounds').click()
    cy.get('.drafter-row').eq(0).should('contain', 'UserB')
    cy.get('.drafter-row').eq(1).should('contain', 'UserA')
  })

  it('All Rounds view: total pts match summed round values', () => {
    cy.contains('.round-pill', 'All Rounds').click()
    cy.get('.drafter-row').eq(0).find('.col-pts').should('contain', '130.0')
    cy.get('.drafter-row').eq(1).find('.col-pts').should('contain', '125.0')
  })
})

// ---------------------------------------------------------------------------
// Hover tooltip — pick breakdown
// ---------------------------------------------------------------------------
describe('Leaderboard — hover tooltip', () => {
  beforeEach(() => {
    cy.clock(FROZEN_NOW_MS, ['Date'])
    cy.mockAuthenticated(false)
    setupTwoRoundLeaderboard(FROZEN_NOW)
    cy.visit('/leaderboard')
    // Switch to Round 1 so UserA is on top (easier to assert pick names)
    cy.contains('.round-pill', 'Round 1').click()
  })

  it('hovering a row shows the pick tooltip', () => {
    cy.get('.drafter-row').eq(0).trigger('mouseenter')
    cy.get('.pick-tooltip-fixed').should('exist')
  })

  it('tooltip shows pro player names from draftPickPlayers', () => {
    cy.get('.drafter-row').eq(0).trigger('mouseenter')
    cy.get('.pick-tooltip-fixed').should('contain', 'Pro Alpha')
    cy.get('.pick-tooltip-fixed').should('contain', 'Pro Beta')
  })

  it('tooltip shows per-pick points from the draftPickN fields', () => {
    // UserA Round 1: Pick1=50.0, Pick2=30.0
    cy.get('.drafter-row').eq(0).trigger('mouseenter')
    cy.get('.pick-pts').should('contain', '50.0')
    cy.get('.pick-pts').should('contain', '30.0')
  })

  it('tooltip disappears on mouseleave', () => {
    cy.get('.drafter-row').eq(0).trigger('mouseenter')
    cy.get('.pick-tooltip-fixed').should('exist')
    cy.get('.drafter-row').eq(0).trigger('mouseleave')
    cy.get('.pick-tooltip-fixed').should('not.exist')
  })

  it('All Rounds tooltip shows per-round breakdown instead of per-pick', () => {
    cy.contains('.round-pill', 'All Rounds').click()
    // UserB is row 0 in all-rounds (130 > 125)
    cy.get('.drafter-row').eq(0).trigger('mouseenter')
    cy.get('.pick-tooltip-fixed').should('contain', 'Round 1')
    cy.get('.pick-tooltip-fixed').should('contain', 'Round 2')
  })
})

// ---------------------------------------------------------------------------
// Tooltip hidden when draft window is still open (picks not yet locked)
// ---------------------------------------------------------------------------
describe('Leaderboard — tooltip hidden for unlocked rounds', () => {
  beforeEach(() => {
    cy.clock(FROZEN_NOW_MS, ['Date'])
    cy.mockAuthenticated(false)
    setupOpenRoundLeaderboard(FROZEN_NOW)
    cy.visit('/leaderboard')
  })

  it('hovering a row does not show the tooltip when the draft is still open', () => {
    cy.get('.drafter-row').eq(0).trigger('mouseenter')
    cy.get('.pick-tooltip-fixed').should('not.exist')
  })

  it('tooltip stays hidden after mouseleave when draft is open', () => {
    cy.get('.drafter-row').eq(0).trigger('mouseenter')
    cy.get('.drafter-row').eq(0).trigger('mouseleave')
    cy.get('.pick-tooltip-fixed').should('not.exist')
  })
})
