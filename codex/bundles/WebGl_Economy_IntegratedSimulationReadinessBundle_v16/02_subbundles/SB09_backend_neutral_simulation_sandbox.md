# SB09 - Backend-neutral Economy SimulationSandbox

## Goal
Avoid locking the joined workflow to SimpleAccounts.

## Required actions

1. Add backend selector based on run plan / backend id.
2. Add backend registry abstraction.
3. Keep SimpleAccounts as default backend for tests.
4. Ensure Ledger backend can be registered later without changing the orchestration workflow.
5. Keep WebGL bridge independent of backends.

## Acceptance criteria

- Sandbox workflow can run with an injected `ISimulationBackend`.
- SimpleAccounts-specific code is not hardcoded inside orchestration except default registration.
