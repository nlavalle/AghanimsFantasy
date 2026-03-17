// Fantasy page — concurrent leagues & alert banner scenarios
import { FROZEN_NOW_MS, FROZEN_NOW } from '../support/constants'

describe('Fantasy page — concurrent leagues & alert banner', () => {
  beforeEach(() => {
    cy.clock(FROZEN_NOW_MS, ['Date'])
    cy.mockAuthenticated(true)
  })

  // Helper: switch leagues via the navbar dropdown
  const switchLeague = (name: string) => {
    cy.get('.league-selector').click()
    cy.contains('.league-list-title', name).click()
  }

  // -------------------------------------------------------------------------
  // Scenario 5: TournB has open draft, TournA is live+locked.
  // defaultSelectedLeague should auto-select TournB (open draft priority).
  // -------------------------------------------------------------------------
  describe('Scenario 5: TournB has open draft, TournA is live', () => {
    beforeEach(() => {
      cy.mockConcurrentLeagues(
        { leagueAScenario: 'locked-active', leagueBScenario: 'draft-open' },
        FROZEN_NOW
      )
      cy.visit('/fantasy')
    })

    it('auto-selects ESL One 2025 (the league with an open draft) on load', () => {
      cy.get('.league-selector .league-name').should('contain', 'ESL One 2025')
    })

    it('no banner shown when user is already on the most urgent league', () => {
      cy.get('.alert-banner-stack').should('not.exist')
    })

    it('banner appears after switching to the less-urgent TournA', () => {
      switchLeague('TI 2025')
      cy.get('.alert-banner-stack').should('be.visible')
    })

    it('banner text mentions draft closing when the other league has an open draft', () => {
      switchLeague('TI 2025')
      cy.get('.alert-text').should('contain', 'draft closes in')
    })

    it('banner shows a Switch button for the other league', () => {
      switchLeague('TI 2025')
      cy.get('.alert-switch-btn').should('be.visible').and('contain', 'Switch')
    })
  })

  // -------------------------------------------------------------------------
  // Scenario 6: Both leagues live, no open drafts.
  // -------------------------------------------------------------------------
  describe('Scenario 6: both leagues live, no open drafts', () => {
    beforeEach(() => {
      cy.mockConcurrentLeagues(
        { leagueAScenario: 'locked-active', leagueBScenario: 'locked-active' },
        FROZEN_NOW
      )
      cy.visit('/fantasy')
    })

    it('banner appears for the non-selected live league', () => {
      cy.get('.alert-banner-stack').should('be.visible')
    })

    it('banner text mentions round is live since neither has an open draft', () => {
      cy.get('.alert-text').should('contain', 'round is live')
    })

    it('Switch button is present', () => {
      cy.get('.alert-switch-btn').should('be.visible')
    })
  })

  // -------------------------------------------------------------------------
  // Scenario 7: Both leagues concluded — no banner.
  // -------------------------------------------------------------------------
  describe('Scenario 7: both leagues concluded', () => {
    beforeEach(() => {
      cy.mockConcurrentLeagues(
        { leagueAScenario: 'concluded', leagueBScenario: 'concluded' },
        FROZEN_NOW
      )
      cy.visit('/fantasy')
    })

    it('no banner is shown', () => {
      cy.get('.alert-banner-stack').should('not.exist')
    })
  })

  // -------------------------------------------------------------------------
  // Scenario 8: Clicking Switch in the banner changes the selected league.
  // -------------------------------------------------------------------------
  describe('Scenario 8: clicking Switch in the banner switches leagues', () => {
    beforeEach(() => {
      cy.mockConcurrentLeagues(
        { leagueAScenario: 'locked-active', leagueBScenario: 'draft-open' },
        FROZEN_NOW
      )
      cy.visit('/fantasy')
      // Switch to TournA so the banner appears for TournB
      switchLeague('TI 2025')
    })

    it('clicking Switch selects the other league in the dropdown', () => {
      cy.get('.alert-switch-btn').click()
      cy.get('.league-selector .league-name').should('contain', 'ESL One 2025')
    })

    it('banner disappears after switching to the urgent league', () => {
      cy.get('.alert-switch-btn').click()
      cy.get('.alert-banner-stack').should('not.exist')
    })
  })
})
