/// <reference types="cypress" />
// ***********************************************
// This example commands.ts shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
//
//
// -- This is a parent command --
// Cypress.Commands.add('login', (email, password) => { ... })
//
//
// -- This is a child command --
// Cypress.Commands.add('drag', { prevSubject: 'element'}, (subject, options) => { ... })
//
//
// -- This is a dual command --
// Cypress.Commands.add('dismiss', { prevSubject: 'optional'}, (subject, options) => { ... })
//
//
// -- This will overwrite an existing command --
// Cypress.Commands.overwrite('visit', (originalFn, url, options) => { ... })

Cypress.Commands.add('mockAuthenticated', (isAuthenticated = false) => {
    const authenticationFixture = isAuthenticated
        ? 'auth/authenticatedTrue.json'
        : 'auth/authenticatedFalse.json';

    cy.intercept('GET', '/api/auth/authenticated', { fixture: authenticationFixture }).as('authenticatedApiCall');

    if (isAuthenticated) {
        // authenticated = true triggers loadUser() which fires these four calls in sequence.
        // All must be stubbed or the app will throw 500s trying to reach the backend.
        cy.intercept('GET', '/api/auth/authorization', {
            body: { id: 'test-user-id', displayName: 'Test User', discordHandle: null, roles: [], prizes: [] }
        }).as('authorizationApiCall');
        cy.intercept('GET', '/identity/manage/info', {
            body: { email: 'test@example.com', isEmailConfirmed: true }
        }).as('manageInfoApiCall');
        cy.intercept('GET', '/api/auth/external-logins', { body: [] }).as('externalLoginsApiCall');
        cy.intercept('GET', '/api/discord/balance', { body: 0 }).as('balanceApiCall');
    }
});

// ---------------------------------------------------------------------------
// mockFantasyScenario
//
// Stubs the league, fantasyLeague, player pool, and draft-points API calls
// for a given lifecycle scenario. Uses a frozen NOW (passed in as a Unix
// epoch seconds integer) so all timestamp comparisons are deterministic.
//
// Scenarios:
//   'pre-draft'      — draft open, tournament not yet started
//   'draft-open'     — draft open, tournament already live
//   'locked-active'  — draft locked, tournament live
//   'concluded'      — everything in the past
// ---------------------------------------------------------------------------

type FantasyScenario = 'pre-draft' | 'draft-open' | 'locked-active' | 'concluded'

Cypress.Commands.add('mockFantasyScenario', (scenario: FantasyScenario, now: number) => {
    const PAST = now - 7 * 86400
    const RECENT = now - 86400
    const SOON = now + 86400
    const FUTURE = now + 7 * 86400

    const league = {
        league_id: 10,
        name: 'TI 2025',
        is_active: true,
        tier: 1,
        region: 1,
        start_timestamp: PAST,
        end_timestamp: FUTURE
    }

    type FantasyLeague = {
        id: number
        leagueId: number
        name: string
        isActive: boolean
        fantasyDraftLocked: number
        leagueStartTime: number
        leagueEndTime: number
    }

    let fantasyLeague: FantasyLeague

    if (scenario === 'pre-draft') {
        // Draft window open, tournament hasn't started yet
        fantasyLeague = { id: 1, leagueId: 10, name: 'Round 1', isActive: true, fantasyDraftLocked: FUTURE, leagueStartTime: FUTURE, leagueEndTime: FUTURE + 86400 }
    } else if (scenario === 'draft-open') {
        // Draft window open, tournament is already running
        fantasyLeague = { id: 1, leagueId: 10, name: 'Round 1', isActive: true, fantasyDraftLocked: SOON, leagueStartTime: RECENT, leagueEndTime: FUTURE }
    } else if (scenario === 'locked-active') {
        // Draft locked, round is live
        fantasyLeague = { id: 1, leagueId: 10, name: 'Round 1', isActive: true, fantasyDraftLocked: PAST, leagueStartTime: RECENT, leagueEndTime: FUTURE }
    } else {
        // concluded — everything in the past
        fantasyLeague = { id: 1, leagueId: 10, name: 'Round 1', isActive: true, fantasyDraftLocked: PAST, leagueStartTime: PAST, leagueEndTime: RECENT }
        league.end_timestamp = RECENT
    }

    cy.intercept('GET', '/api/league*', { body: [league] }).as('getLeagues')
    cy.intercept('GET', '/api/FantasyLeague', { body: [fantasyLeague] }).as('getFantasyLeagues')
    cy.intercept('GET', `/api/fantasyplayer/fantasyleague/${fantasyLeague.id}`, { fixture: 'fantasy/players.json' }).as('getPlayers')
    cy.intercept('GET', `/api/fantasydraft/${league.league_id}/drafts/points`, { fixture: 'fantasy/draft-points-empty.json' }).as('getDraftPoints')
    cy.intercept('GET', `/api/fantasyleague/${fantasyLeague.id}/players/points`, { fixture: 'fantasy/player-points.json' }).as('getPlayerStats')
    cy.intercept('GET', `/api/fantasyleague/${fantasyLeague.id}/drafters/top10`, { body: [] }).as('getLeaderboard')
    cy.intercept('GET', `/api/fantasyleague/${fantasyLeague.id}/drafters/stats`, { body: {} }).as('getLeaderboardStats')
});

// ---------------------------------------------------------------------------
// mockFantasyScenarioWithSavedDraft
//
// Same as mockFantasyScenario but returns a saved draft for the user.
// ---------------------------------------------------------------------------
Cypress.Commands.add('mockFantasyScenarioWithSavedDraft', (scenario: FantasyScenario, now: number) => {
    cy.mockFantasyScenario(scenario, now)
    cy.intercept('GET', '/api/fantasydraft/10/drafts/points', { fixture: 'fantasy/draft-points-saved.json' }).as('getDraftPoints')
});

// ---------------------------------------------------------------------------
// mockConcurrentLeagues
//
// Sets up two leagues simultaneously, each with their own fantasy league.
// leagueAScenario applies to league 10, leagueBScenario to league 20.
// ---------------------------------------------------------------------------
type ConcurrentSetup = {
    leagueAScenario: FantasyScenario
    leagueBScenario: FantasyScenario
}

Cypress.Commands.add('mockConcurrentLeagues', ({ leagueAScenario, leagueBScenario }: ConcurrentSetup, now: number) => {
    const PAST = now - 7 * 86400
    const RECENT = now - 86400
    const SOON = now + 86400
    const FUTURE = now + 7 * 86400

    const leagueA = { league_id: 10, name: 'TI 2025', is_active: true, tier: 1, region: 1, start_timestamp: PAST, end_timestamp: FUTURE }
    const leagueB = { league_id: 20, name: 'ESL One 2025', is_active: true, tier: 1, region: 2, start_timestamp: PAST, end_timestamp: FUTURE }

    const makeFL = (id: number, leagueId: number, scenario: FantasyScenario) => {
        if (scenario === 'pre-draft') return { id, leagueId, name: `Round 1`, isActive: true, fantasyDraftLocked: FUTURE, leagueStartTime: FUTURE, leagueEndTime: FUTURE + 86400 }
        if (scenario === 'draft-open') return { id, leagueId, name: `Round 1`, isActive: true, fantasyDraftLocked: SOON, leagueStartTime: RECENT, leagueEndTime: FUTURE }
        if (scenario === 'locked-active') return { id, leagueId, name: `Round 1`, isActive: true, fantasyDraftLocked: PAST, leagueStartTime: RECENT, leagueEndTime: FUTURE }
        /* concluded */                   return { id, leagueId, name: `Round 1`, isActive: true, fantasyDraftLocked: PAST, leagueStartTime: PAST, leagueEndTime: RECENT }
    }

    const flA = makeFL(1, 10, leagueAScenario)
    const flB = makeFL(2, 20, leagueBScenario)

    cy.intercept('GET', '/api/league*', { body: [leagueA, leagueB] }).as('getLeagues')
    cy.intercept('GET', '/api/FantasyLeague', { body: [flA, flB] }).as('getFantasyLeagues')
    cy.intercept('GET', '/api/fantasyplayer/fantasyleague/**', { fixture: 'fantasy/players.json' }).as('getPlayers')
    cy.intercept('GET', '/api/fantasydraft/**/drafts/points', { fixture: 'fantasy/draft-points-empty.json' }).as('getDraftPoints')
    cy.intercept('GET', '/api/fantasyleague/*/players/points', { fixture: 'fantasy/player-points.json' }).as('getPlayerStats')
    cy.intercept('GET', '/api/fantasyleague/*/drafters/top10', { body: [] }).as('getLeaderboard')
    cy.intercept('GET', '/api/fantasyleague/*/drafters/stats', { body: {} }).as('getLeaderboardStats')
});

declare global {
    namespace Cypress {
        interface Chainable {
            mockAuthenticated(isAuthenticated: true | false): Chainable<void>
            mockFantasyScenario(scenario: FantasyScenario, now: number): Chainable<void>
            mockFantasyScenarioWithSavedDraft(scenario: FantasyScenario, now: number): Chainable<void>
            mockConcurrentLeagues(setup: ConcurrentSetup, now: number): Chainable<void>
        }
    }
}

export { }
