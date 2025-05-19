// https://on.cypress.io/api

describe('Fantasy Page', () => {
  beforeEach(() => {
    cy.visit('/fantasy')
  })

  it('displays fantasy tabs', () => {
    cy.contains('Current Draft').should('be.visible')
    cy.contains('Draft Players').should('be.visible')
    cy.contains('Leaderboard').should('be.visible')
  })

  it('Requires sign-in to view the Leaderboard tab', () => {
    cy.contains('Leaderboard').click()

    cy.contains('Please login to view the leaderboard').should('be.visible')
  })
})
