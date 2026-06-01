# SB11 - Backend selector and ledger readiness

The sandbox now has a backend selector. Harden it:

- ensure backend selection is deterministic,
- ensure missing backend fails with structured diagnostics, not exception-only,
- add fake backend test and ledger-descriptor-only readiness test,
- do not let SimpleAccounts-specific types leak into generic sandbox contracts.

## Status

Completed.

## Goal

Harden backend selection and ledger readiness diagnostics while keeping sandbox contracts backend-neutral.

## Prerequisites

- SB08 runner path must identify backend selection in the sandbox workflow.

## Owned Requirements

- R11 Backend Selector And Ledger Readiness.

## Dependency Impact

Feeds SB12 readiness report with deterministic backend and ledger-readiness conclusions.

## Validation Depth

Unit/integration tests for deterministic selection, missing backend diagnostics, fake backend, and ledger descriptor readiness.

## Proof Required

- Economy tests for backend selector behavior.
- Source assertions for structured diagnostics and absence of SimpleAccounts-specific generic contract leakage.
- Proof manifest.

Proof captured in `bundle://proof/SB11/manifest.md`.

## Progression Gate

Pass only when missing backend failures are structured and deterministic, and generic sandbox contracts remain backend-neutral.

Gate result: Passed. Backend selection now exposes structured `TrySelect` diagnostics, deterministic same-priority hint ordering, ledger descriptor-only readiness, workflow diagnostics without exception-only failure, and backend-neutral sandbox contracts.
