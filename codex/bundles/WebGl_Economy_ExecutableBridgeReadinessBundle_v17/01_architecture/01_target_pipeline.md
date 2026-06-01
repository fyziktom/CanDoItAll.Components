# Target pipeline

The intended long-term flow is:

```text
Experiment input pack JSON
  -> Experiment loader + strict hash validation
  -> Backend selector
  -> Simulation backend materialization
  -> Simulation frames / deltas / events
  -> Economy visual frame mapper
  -> Economy visual actions
  -> Economy WebGL bridge
  -> WebGlRunDocument + InitialScene + timeline stages
  -> Generic WebGL runtime execution
  -> Pause / snapshot / analyze / resume
```

## Repository boundary

```text
CanDoItAll.Components
  WebGlLib
    Generic scene engine, runtime, patches, motions, JS runtime
  WebGlRunLib
    Generic run/timeline/action/stage abstractions over WebGlLib
  WebGlSandbox
    Generic WebGL proofs only

CanDoItAll.Economy
  Simulation.Abstractions
    Scenario, events, frames, snapshots, experiment input packs, renderer-neutral visual contracts
  Simulation.SimpleAccounts
    Lightweight simulation backend
  Simulation.Ledger
    Ledger-backed adapter
  Simulation.Visualization
    Economy visual frame/action mapping, no WebGL dependency
  Simulation.WebGlBridge
    The only place that bridges Economy visual actions to Components.WebGlRunLib
  SimulationSandbox
    Economy-side orchestration and future demo/sandbox host
```

## Prohibited direction

`Components -> Economy` is forbidden.

`Simulation.Abstractions -> WebGl/Components` is forbidden.

`Simulation.Visualization -> WebGl/Components` is forbidden.

`Simulation.WebGlBridge -> SimpleAccounts/Ledger` is forbidden.

`SimulationSandbox -> SimpleAccounts` is acceptable only through a backend registry/default adapter boundary. The workflow should not hardcode simple accounts as the only possible path.
