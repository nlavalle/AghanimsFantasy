// Shared test constants

// Frozen epoch used across all fantasy E2E tests: 2025-06-01 12:00:00 UTC
// cy.clock() is called with this value so all timestamp comparisons are deterministic.
export const FROZEN_NOW_MS = 1748779200000
export const FROZEN_NOW    = FROZEN_NOW_MS / 1000
