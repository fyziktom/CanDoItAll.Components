# SB11 - Backend selector and ledger readiness

The sandbox now has a backend selector. Harden it:

- ensure backend selection is deterministic,
- ensure missing backend fails with structured diagnostics, not exception-only,
- add fake backend test and ledger-descriptor-only readiness test,
- do not let SimpleAccounts-specific types leak into generic sandbox contracts.
