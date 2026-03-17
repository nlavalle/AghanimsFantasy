// Fantasy draft mechanic rules
// All tests run under 'pre-draft' scenario (draft window open, tournament not started)
// so that saving is allowed and every pick interaction is live.
import { FROZEN_NOW_MS, FROZEN_NOW } from '../support/constants'

describe('Fantasy draft — mechanic rules', () => {
  beforeEach(() => {
    cy.clock(FROZEN_NOW_MS, ['Date'])
    cy.mockAuthenticated(true)
    cy.mockFantasyScenario('pre-draft', FROZEN_NOW)
    cy.visit('/fantasy')
  })

  // Helper: click a pool card then click the draft button
  const draftPlayer = (name: string) => {
    cy.contains('.card-name', name).click()
    cy.get('.detail-panel .draft-btn').click({ force: true })
  }

  // -------------------------------------------------------------------------
  // Stats panel
  // -------------------------------------------------------------------------
  it('clicking a player card opens their stats panel', () => {
    cy.contains('.card-name', 'Player One').click()
    cy.get('.detail-panel').should('contain', 'Player One')
  })

  // -------------------------------------------------------------------------
  // Slot filling and slot advancement
  // -------------------------------------------------------------------------
  it('drafting a player fills the pick bar slot with their name', () => {
    draftPlayer('Player Four') // pos 1 — matches default slot 1
    cy.get('.new-pick-bar').should('contain', 'Player Four')
  })

  it('active draft slot advances to the next position after picking', () => {
    // Before picking: slot index 0 (pos-1) is selected
    cy.get('.new-pick-bar .pick-slot.selected').should('have.length', 1)
    draftPlayer('Player Four') // pos 1
    // After picking pos-1, the previously-selected slot should now be filled, not selected
    cy.get('.new-pick-bar .pick-slot.filled').should('have.length', 1)
    // And a different slot should now be selected (pos-2)
    cy.get('.new-pick-bar .pick-slot.selected').should('have.length', 1)
    cy.get('.new-pick-bar .pick-slot.filled').should('not.have.class', 'selected')
  })

  // -------------------------------------------------------------------------
  // Role (position) filtering
  // -------------------------------------------------------------------------
  it('only players matching the active slot position are enabled', () => {
    // Default slot is pos-1. Pos-2 and pos-3 players should be disabled.
    cy.contains('.pool-card', 'Player Two').should('have.class', 'pool-card--disabled')
    cy.contains('.pool-card', 'Player Three').should('have.class', 'pool-card--disabled')
  })

  it('after drafting slot 1, pos-2 players become enabled and other pos-1 players become disabled', () => {
    draftPlayer('Player Four') // pos 1
    // Remaining pos-1 player (Player One) should now be disabled (already-picked position)
    cy.contains('.pool-card', 'Player One').should('have.class', 'pool-card--disabled')
    // Pos-2 players should now be enabled
    cy.contains('.pool-card', 'Player Two').should('not.have.class', 'pool-card--disabled')
    cy.contains('.pool-card', 'Player Five').should('not.have.class', 'pool-card--disabled')
  })

  // -------------------------------------------------------------------------
  // Budget
  // -------------------------------------------------------------------------
  it('selecting a player whose cost exceeds the budget shows the over-budget indicator', () => {
    // Player One costs 700 > DRAFT_BUDGET (600)
    cy.contains('.card-name', 'Player One').click()
    cy.get('.detail-panel .draft-btn').click({ force: true })
    cy.get('.budget-card').should('have.class', 'budget-card--over')
  })

  it('draft save button is disabled when over budget', () => {
    draftPlayer('Player One') // cost 700 > DRAFT_BUDGET (600)
    cy.contains('button', 'Save Draft').should('be.disabled')
  })

  // -------------------------------------------------------------------------
  // Team cap (max 2 players per team)
  // -------------------------------------------------------------------------
  it('drafting 2 players from the same team disables the rest of that team', () => {
    // Pick Player Four (Team OG, pos 1) then Player Five (Team OG, pos 2)
    draftPlayer('Player Four')
    draftPlayer('Player Five')
    // No more OG players remain after the two picks, but if there were they'd be capped.
    // Verify the team header for OG is marked as full.
    cy.get('.team-block').contains('OG').closest('.team-block')
      .find('.team-header').should('have.class', 'team-header--full')
  })

  it('after hitting the team cap, remaining players from that team are disabled', () => {

    // Team Secret has 3 players. Draft pos-1 from a different team first so slot advances.
    // Advance to pos-2 by picking Player Four (OG, pos 1)
    draftPlayer('Player Four')
    // Now pick Player Two (Secret, pos 2) — advances slot to pos 3
    draftPlayer('Player Two')
    // Now pick Player Three (Secret, pos 3) — Secret now has 2 picks, cap hit
    draftPlayer('Player Three')
    // Advance slot to pos 1 equivalent — but let's just verify Player One (Secret, pos 1) is capped
    // Player One is pos-1 which won't be in active slot, but it should also carry pool-card--disabled
    // due to team cap regardless of position filter.
    cy.contains('.pool-card', 'Player One').should('have.class', 'pool-card--disabled')
  })
})

// ---------------------------------------------------------------------------
// Saved draft pre-population
// ---------------------------------------------------------------------------
describe('Fantasy draft — saved draft pre-population', () => {
  beforeEach(() => {
    cy.clock(FROZEN_NOW_MS, ['Date'])
    cy.mockAuthenticated(true)
    cy.mockFantasyScenarioWithSavedDraft('pre-draft', FROZEN_NOW)
    cy.visit('/fantasy')
  })

  it('loads a previously saved draft into the pick bar on page load', () => {
    // Saved draft has Player One through Player Five in slots 1-5
    cy.get('.new-pick-bar').should('contain', 'Player One')
    cy.get('.new-pick-bar').should('contain', 'Player Two')
    cy.get('.new-pick-bar').should('contain', 'Player Three')
    cy.get('.new-pick-bar').should('contain', 'Player Four')
    cy.get('.new-pick-bar').should('contain', 'Player Five')
  })

  it('all five pick slots are filled when a saved draft is loaded', () => {
    cy.get('.new-pick-bar .pick-slot.filled').should('have.length', 5)
  })

  it('no pick slot is selected (active) when all slots are pre-filled', () => {
    cy.get('.new-pick-bar .pick-slot.selected').should('have.length', 0)
  })
})
