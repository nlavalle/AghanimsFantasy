// https://on.cypress.io/api

describe('Auth', () => {
  it('logged out displays login button', () => {
    cy.mockAuthenticated(false);
    cy.visit('/about');
    cy.contains('Login').should('be.visible')
  })

  it('logged in displays logout button', () => {
    cy.mockAuthenticated(true);
    cy.visit('/about');
    cy.contains('Logout').should('be.visible')
  })
})
