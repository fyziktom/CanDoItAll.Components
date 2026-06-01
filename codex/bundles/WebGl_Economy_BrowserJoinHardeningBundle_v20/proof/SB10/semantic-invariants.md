# SB10 Semantic Invariants

## Registry Boundary

- `IEconomySimulationBackendRegistry` remains the extension point for executable sandbox backends.
- Default SimpleAccounts registration is isolated to the default selector path.
- Workflow projection must use backend-selector diagnostics instead of throwing for missing backends.

## Ledger Readiness

- A ledger backend request can be represented as a descriptor-only readiness state.
- Descriptor-only readiness must not imply an executable ledger backend exists.
- Missing ledger backend requests must include both the descriptor warning and a clear missing-backend error.

## Boundary

- Lower-level simulation abstractions remain free of Components/WebGL references.
- Ledger readiness tests do not require ledger browser UI or full ledger simulation execution.
