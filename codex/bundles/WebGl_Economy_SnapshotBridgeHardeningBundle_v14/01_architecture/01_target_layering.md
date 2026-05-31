# Target Layering

## Components repo

```text
CanDoItAll.Components.WebGlLib
  Generic WebGL scene rendering, assets, patches, motion, selection, runtime diagnostics.

CanDoItAll.Components.WebGlRunLib
  Generic run documents, timelines, run frames, action planning, stage batching, replay helpers.
  No Economy references.
```

## Economy repo

```text
CanDoItAll.Economy.Simulation.Abstractions
  Experiment input packs, scenario definitions, typed events, frames/deltas, snapshots, metrics/invariants.

CanDoItAll.Economy.Simulation.SimpleAccounts
  Lightweight state transition backend.

CanDoItAll.Economy.Simulation.Ledger
  Ledger-backed simulation adapter.

CanDoItAll.Economy.Simulation.Visualization
  Economy visual frames/actions/symbols, renderer-neutral.

CanDoItAll.Economy.Simulation.WebGlBridge
  The only layer that references Components.WebGlRunLib.
  Converts renderer-neutral economy visual outputs into generic WebGL run documents.

CanDoItAll.Economy.SimulationSandbox
  Future app/demo composition layer that wires input packs -> backend -> visualization -> WebGL bridge -> UI.
```

## Forbidden dependencies

- Components -> Economy: forbidden.
- Simulation.Abstractions -> Components/WebGL: forbidden.
- Simulation.Visualization -> Components/WebGL: forbidden.
- Simulation.WebGlBridge -> SimpleAccounts or Ledger: forbidden.
- SimpleAccounts -> WebGL: forbidden.
- Ledger -> WebGL: forbidden.
