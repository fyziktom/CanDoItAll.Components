# SB10 - Economy backend registry and ledger readiness probe

## Goal

Ensure sandbox can later run ledger-backed simulations without mixing simple-account and ledger logic.

## Tasks

- Keep `IEconomySimulationBackendRegistry`.
- Add a descriptor-only ledger backend readiness test.
- Ensure SimulationSandbox does not hard-code SimpleAccounts beyond default registration.
- If a ledger backend is requested but not registered, error must be clear.

## Acceptance

- Fake backend test remains.
- Missing backend test is explicit.
- Ledger descriptor test does not require full ledger simulation UI.
