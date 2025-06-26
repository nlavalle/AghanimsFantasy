// https://on.cypress.io/api

describe('Prizes', () => {
  it('logged out does not display prizes', () => {
    cy.mockAuthenticated(false);
    cy.visit('/about');
    cy.contains('Prizes').should('not.be.visible')
  })

  it('logged in displays prizes', () => {
    cy.mockAuthenticated(true);
    cy.visit('/about');
    cy.contains('Prizes').should('be.visible')
  })

  it('logged in navigates to prizes', () => {
    cy.mockAuthenticated(true);
    cy.visit('/prizes');
    cy.contains('Prizes').should('be.visible')
  })
})
