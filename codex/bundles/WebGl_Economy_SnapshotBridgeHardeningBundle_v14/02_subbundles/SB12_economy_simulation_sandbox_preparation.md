# SB12 - Economy SimulationSandbox preparation

Goal:
- Prepare, but do not yet overbuild, the combined sandbox.

Tasks:
1. Add design or skeleton for `CanDoItAll.Economy.SimulationSandbox`.
2. Sandbox may reference:
   - Simulation.Abstractions,
   - SimpleAccounts/Ledger as backends,
   - Simulation.Visualization,
   - Simulation.WebGlBridge,
   - Components WebGl UI packages if needed.
3. Sandbox should load input pack -> run backend -> visual frame -> bridge -> WebGl view.
4. Add placeholder routes only if low-risk.
5. Do not add final shared-well demo yet unless explicitly requested.

Acceptance:
- Clear wiring plan exists in Economy repo.

## Status

Completed.

## Prerequisites

SB05 bridge projection proof and SB07 snapshot contracts.

## Validation Depth

Add or verify an Economy-side design or skeleton that wires input pack, backend, visualization, bridge, and WebGL view boundaries without building the final demo.

## Progression Gate

SB15 may proceed only after the skeleton/design compiles or is documented clearly, remains in Economy, and does not create final-demo scope.
