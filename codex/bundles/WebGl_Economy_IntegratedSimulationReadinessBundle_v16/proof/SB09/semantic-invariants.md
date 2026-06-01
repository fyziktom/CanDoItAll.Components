# SB09 Semantic Invariants

| Invariant ID | Assertion |
|---|---|
| SB09-backend-registry | SimulationSandbox selects registered backends by run-plan backend id while preserving explicit injected backend behavior. |
| SB09-backend-neutral-contract | Sandbox projection exposes backend result data instead of a SimpleAccounts-specific scenario contract. |

## Shallow-pass trap

A shallow pass could keep always selecting SimpleAccounts or expose SimpleAccounts from the public workflow result.

## Adversarial negative proof

`economy-sandbox-workflow-tests.txt` includes an injected fake backend and a registry selection test.

## Semantic positive proof

`economy-sandbox-workflow-tests.txt` passes for shared and constrained probes, fake backend injection, backend id propagation, and visual snapshots.

## Anti-stub audit

The workflow materializes frames through `IEconomySimulationBackend` and builds visual/snapshot outputs through pipeline interfaces.

