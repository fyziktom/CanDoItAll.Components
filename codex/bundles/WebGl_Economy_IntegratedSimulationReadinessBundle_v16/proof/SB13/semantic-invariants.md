# SB13 Semantic Invariants

| Invariant ID | Assertion |
|---|---|
| SB13-headless-join | The headless workflow joins input pack, backend, visualization, WebGL run document, and snapshots. |
| SB13-visual-analysis | Snapshot analysis can explain visual state from snapshot data without a final UI. |

## Shallow-pass trap

A shallow pass could run bridge projection only without snapshots or visual-state provenance.

## Adversarial negative proof

`economy-headless-bridge-e2e-tests.txt` checks invalid diagnostics, fake backend path, snapshot visual state, and visual analysis.

## Semantic positive proof

`economy-headless-bridge-e2e-tests.txt` passes with joined pipeline proof.

## Anti-stub audit

The proof uses the actual EconomySimulationSandbox workflow and snapshot analysis service.

