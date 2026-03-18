// DraftRoundCard — round selector behaviour
// Uses a two-round league: Round 1 locked/concluded, Round 2 open (pre-draft).
// The app auto-selects Round 2 on load; the card lets the user switch to Round 1.
import { FROZEN_NOW_MS, FROZEN_NOW } from '../support/constants'

describe('DraftRoundCard — single round', () => {
  beforeEach(() => {
    cy.clock(FROZEN_NOW_MS, ['Date'])
    cy.mockAuthenticated(false)
    cy.mockFantasyScenario('pre-draft', FROZEN_NOW)
    cy.visit('/fantasy')
  })

  it('shows the correct current / total when there is only one round', () => {
    cy.get('.round-card').should('contain', '1')
    cy.get('.round-card .round-denom').should('contain', '/ 1')
  })

  it('does not open a dropdown when there is only one round', () => {
    cy.get('.round-card').click()
    cy.get('.round-dropdown').should('not.exist')
  })
})

describe('DraftRoundCard — multi-round league', () => {
  beforeEach(() => {
    cy.clock(FROZEN_NOW_MS, ['Date'])
    cy.mockAuthenticated(false)
    cy.mockMultiRoundScenario(FROZEN_NOW)
    cy.visit('/fantasy')
  })

  // -------------------------------------------------------------------------
  // Initial state
  // -------------------------------------------------------------------------
  it('auto-selects the open round on load (Round 2)', () => {
    // Round 2 is the open draft; priority logic should pick it as current.
    cy.get('.round-card .round-value').invoke('text').then(text => {
      expect(text.trim()).to.match(/^2/)
    })
  })

  it('shows the correct total round count', () => {
    cy.get('.round-card .round-denom').should('contain', '/ 2')
  })

  // -------------------------------------------------------------------------
  // Dropdown open / close
  // -------------------------------------------------------------------------
  it('clicking the round card opens the dropdown', () => {
    cy.get('.round-card').click()
    cy.get('.round-dropdown').should('be.visible')
  })

  it('dropdown lists all rounds', () => {
    cy.get('.round-card').click()
    cy.get('.round-option').should('have.length', 2)
    cy.get('.round-option').eq(0).should('contain', 'Round 1')
    cy.get('.round-option').eq(1).should('contain', 'Round 2')
  })

  it('the currently active round is marked as active in the dropdown', () => {
    cy.get('.round-card').click()
    cy.get('.round-option.round-option--active').should('contain', 'Round 2')
  })

  it('the locked round is marked as locked in the dropdown', () => {
    cy.get('.round-card').click()
    cy.get('.round-option').eq(0).should('have.class', 'round-option--locked')
    cy.get('.round-option').eq(0).should('contain', 'Locked')
  })

  it('the open round is marked as open in the dropdown', () => {
    cy.get('.round-card').click()
    cy.get('.round-option').eq(1).should('not.have.class', 'round-option--locked')
    cy.get('.round-option').eq(1).should('contain', 'Open')
  })

  it('clicking outside the dropdown closes it', () => {
    cy.get('.round-card').click()
    cy.get('.round-dropdown').should('be.visible')
    cy.get('body').click(0, 0)
    cy.get('.round-dropdown').should('not.exist')
  })

  // -------------------------------------------------------------------------
  // Round switching
  // -------------------------------------------------------------------------
  it('selecting a different round closes the dropdown', () => {
    cy.get('.round-card').click()
    cy.get('.round-option').eq(0).click()
    cy.get('.round-dropdown').should('not.exist')
  })

  it('selecting Round 1 updates the round card to show 1 / 2', () => {
    cy.get('.round-card').click()
    cy.get('.round-option').eq(0).click()
    cy.get('.round-card .round-value').invoke('text').then(text => {
      expect(text.trim()).to.match(/^1/)
    })
    cy.get('.round-card .round-denom').should('contain', '/ 2')
  })

  it('after switching to Round 1, the active marker moves to Round 1 on next open', () => {
    cy.get('.round-card').click()
    cy.get('.round-option').eq(0).click()
    cy.get('.round-card').click()
    cy.get('.round-option.round-option--active').should('contain', 'Round 1')
  })

  it('selecting Round 2 updates the round card to show 2 / 2', () => {
    // Switch away to Round 1 first so we can test switching back
    cy.get('.round-card').click()
    cy.get('.round-option').eq(0).click()
    // Now switch back to Round 2
    cy.get('.round-card').click()
    cy.get('.round-option').eq(1).click()
    cy.get('.round-card .round-value').invoke('text').then(text => {
      expect(text.trim()).to.match(/^2/)
    })
    cy.get('.round-card .round-denom').should('contain', '/ 2')
  })

  it('after switching back to Round 2, the active marker returns to Round 2', () => {
    cy.get('.round-card').click()
    cy.get('.round-option').eq(0).click()
    cy.get('.round-card').click()
    cy.get('.round-option').eq(1).click()
    cy.get('.round-card').click()
    cy.get('.round-option.round-option--active').should('contain', 'Round 2')
  })
})
