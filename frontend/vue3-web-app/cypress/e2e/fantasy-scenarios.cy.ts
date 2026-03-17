// Fantasy page — league lifecycle scenarios
// Covers what a user sees at each stage of a fantasy league's life.
// Draft mechanic rules live in fantasy-draft-rules.cy.ts.
import { FROZEN_NOW_MS, FROZEN_NOW } from '../support/constants'

describe('Fantasy page — league lifecycle scenarios', () => {
  beforeEach(() => {
    cy.clock(FROZEN_NOW_MS, ['Date'])
    cy.mockAuthenticated(true)
  })

  // -------------------------------------------------------------------------
  // Scenario 1: pre-tournament, draft open
  // -------------------------------------------------------------------------
  describe('Scenario 1: pre-tournament, draft open', () => {
    beforeEach(() => {
      cy.mockFantasyScenario('pre-draft', FROZEN_NOW)
      cy.visit('/fantasy')
    })

    it('pick bar is visible', () => {
      cy.get('.new-pick-bar').should('be.visible')
    })

    it('save button is enabled', () => {
      cy.contains('button', 'Save Draft').should('not.be.disabled')
    })

    it('draft timer card is present', () => {
      cy.get('.new-pick-bar .timer-card').should('exist')
    })

    it('player pool renders players', () => {
      cy.contains('.card-name', 'Player One').should('be.visible')
    })
  })

  // -------------------------------------------------------------------------
  // Scenario 2: draft open, tournament already live
  // -------------------------------------------------------------------------
  describe('Scenario 2: draft open, tournament live', () => {
    beforeEach(() => {
      cy.mockFantasyScenario('draft-open', FROZEN_NOW)
      cy.visit('/fantasy')
    })

    it('save button is enabled', () => {
      cy.contains('button', 'Save Draft').should('not.be.disabled')
    })

    it('player pool renders players', () => {
      cy.contains('.card-name', 'Player One').should('be.visible')
    })
  })

  // -------------------------------------------------------------------------
  // Scenario 3: draft locked, round live
  // -------------------------------------------------------------------------
  describe('Scenario 3: draft locked, round live', () => {
    beforeEach(() => {
      cy.mockFantasyScenario('locked-active', FROZEN_NOW)
      cy.visit('/fantasy')
    })

    it('save button is disabled', () => {
      cy.contains('button', 'Save Draft').should('be.disabled')
    })

    it('player pool is still visible for browsing', () => {
      cy.contains('.card-name', 'Player One').should('be.visible')
    })

    it('clicking a player still shows their stats panel', () => {
      cy.contains('.card-name', 'Player One').click()
      cy.get('.detail-panel').should('contain', 'Player One')
    })
  })

  describe('Scenario 3b: draft locked, user has saved draft', () => {
    beforeEach(() => {
      cy.mockFantasyScenarioWithSavedDraft('locked-active', FROZEN_NOW)
      cy.visit('/fantasy')
    })

    it('save button is disabled', () => {
      cy.contains('button', 'Save Draft').should('be.disabled')
    })
  })

  // -------------------------------------------------------------------------
  // Scenario 4: tournament concluded
  // -------------------------------------------------------------------------
  describe('Scenario 4: tournament concluded, no saved draft', () => {
    beforeEach(() => {
      cy.mockFantasyScenario('concluded', FROZEN_NOW)
      cy.visit('/fantasy')
    })

    it('save button is disabled', () => {
      cy.contains('button', 'Save Draft').should('be.disabled')
    })

    it('player pool is still visible', () => {
      cy.contains('.card-name', 'Player One').should('be.visible')
    })
  })

  describe('Scenario 4b: tournament concluded, user has saved draft', () => {
    beforeEach(() => {
      cy.mockFantasyScenarioWithSavedDraft('concluded', FROZEN_NOW)
      cy.visit('/fantasy')
    })

    it('save button is disabled', () => {
      cy.contains('button', 'Save Draft').should('be.disabled')
    })
  })
})
