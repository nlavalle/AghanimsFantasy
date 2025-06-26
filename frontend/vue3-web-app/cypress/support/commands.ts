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
    let authenticationFixture;

    if (isAuthenticated) {
        authenticationFixture = 'auth/authenticatedTrue.json';
        cy.fixture(authenticationFixture);
    } else {
        authenticationFixture = 'auth/authenticatedFalse.json';
        cy.fixture(authenticationFixture);
    }

    // Intercept the authenticated GET request
    cy.intercept('GET', '/api/auth/authenticated', { fixture: authenticationFixture }).as('authenticatedApiCall');
});

declare global {
    namespace Cypress {
        interface Chainable {
            mockAuthenticated(isAuthenticated: true | false): Chainable<void>
        }
    }
}

export { }
