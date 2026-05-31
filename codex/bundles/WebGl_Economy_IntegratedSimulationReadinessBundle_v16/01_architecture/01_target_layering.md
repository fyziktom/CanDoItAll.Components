# Target layering

```text
CanDoItAll.Components
  CanDoItAll.Components.WebGlLib
    Generic WebGL scene runtime, JS modules, scene documents, patches, motions, command batches.

  CanDoItAll.Components.WebGlRunLib
    Generic run/playback/action/stage abstractions over WebGlLib.
    No Economy references, no example terms.

CanDoItAll.Economy
  CanDoItAll.Economy.Simulation.Abstractions
    Backend-neutral simulation contracts, experiment input packs, scenarios, events, frames, deltas, snapshots, metrics, invariants.
    No WebGL/Components references.

  CanDoItAll.Economy.Simulation.SimpleAccounts
    Lightweight deterministic backend.

  CanDoItAll.Economy.Simulation.Ledger
    Ledger-backed projection/adapter.

  CanDoItAll.Economy.Simulation.Visualization
    Economy visual frame/action intentions, still renderer-neutral.

  CanDoItAll.Economy.Simulation.WebGlBridge
    The only allowed layer that bridges Economy visualization to Components WebGlRunLib/WebGlLib.

  CanDoItAll.Economy.SimulationSandbox
    Economy-side orchestration sandbox for joined simulation + visualization experiments.
```

## Allowed references

- Components.WebGlRunLib -> Components.WebGlLib
- Economy.Simulation.Visualization -> Economy.Simulation.Abstractions
- Economy.Simulation.WebGlBridge -> Economy.Simulation.Abstractions + Economy.Simulation.Visualization + Components.WebGlRunLib / WebGlLib
- Economy.SimulationSandbox -> Economy.Simulation.Abstractions + selected backends + Visualization + WebGlBridge

## Forbidden references

- Components -> Economy
- Economy.Simulation.Abstractions -> Components / WebGL / SimpleAccounts / Ledger
- Economy.Simulation.Visualization -> Components / WebGL / SimpleAccounts / Ledger
- Economy.Simulation.WebGlBridge -> SimpleAccounts / Ledger backend projects
