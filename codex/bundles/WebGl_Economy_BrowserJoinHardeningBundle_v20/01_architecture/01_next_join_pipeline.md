# Target next join pipeline

## Repository boundary

```text
CanDoItAll.Components
  WebGlLib      -> generic scene runtime
  WebGlRunLib   -> generic run/playback contracts and browser-agnostic control
  WebGlSandbox  -> generic WebGL proofs only

CanDoItAll.Economy
  Simulation.Abstractions
  Simulation.SimpleAccounts
  Simulation.Ledger
  Simulation.Visualization
  Simulation.WebGlBridge
  SimulationSandbox
```

## Required runtime chain

```text
EconomySimulationSandboxSession
  -> CurrentRunFrame
  -> WebGlRunFrameApplyResult.FromFrame(...)
  -> Economy/Blazor desktop sandbox page
  -> generic WebGlRun browser adapter
  -> WebGlSceneView / JS runtime
  -> command batch / stages / patches / motions
  -> runtime diagnostics
  -> snapshot visual runtime attachment
```

## Keep genericity

The engine must not know about wells, farmers, land, tax, or water.
Those must remain experiment/scenario data and visual mapping data.

The generic concepts are:

- actor / entity
- resource
- resource store
- location / object / institution
- event
- visual action
- run stage
- motion
- scene patch
- snapshot
- metric / invariant
- diagnostic
