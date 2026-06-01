# SB10 - Economy backend registry and ledger readiness probe

## Status

Completed. Closure gate passed.

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

## Prerequisites

- SB08/SB09 service shape known.

## Owned Requirements

- R10 Backend registry and ledger readiness.

## Dependency Impact

Backend registry proof prevents the sandbox from hard-coding SimpleAccounts in a way that blocks later ledger-backed scenarios.

## Validation Depth

Tests must cover fake backend, missing backend diagnostics, and descriptor-only ledger readiness without implementing ledger UI.

## Proof Required

- Backend registry test transcript.
- Boundary/source assertions.
- `bundle://proof/SB10/manifest.md`

## Browser Validation Logging

N/A. Ledger UI is out of scope.

## Progression Gate

Pass only when registry behavior is deterministic and no lower-level simulation abstraction references Components/WebGL.
