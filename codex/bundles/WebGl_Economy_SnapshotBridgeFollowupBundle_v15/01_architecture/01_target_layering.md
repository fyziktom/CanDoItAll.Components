# Target layering

## Components repository

```text
CanDoItAll.Components.WebGlLib
  - generic scene DTOs
  - WebGL JS runtime
  - scene patches
  - command batches
  - motion queues
  - stage runner
  - diagnostics/proof snapshots
  - no Economy references

CanDoItAll.Components.WebGlRunLib
  - generic run/playback/action contracts
  - action planner
  - action plan to command batch compiler
  - run document
  - frame/stage contracts
  - no Economy references
```

## Economy repository

```text
CanDoItAll.Economy.Simulation.Abstractions
  - scenario definitions
  - events
  - experiment input packs
  - frames/deltas
  - metrics/invariants
  - snapshots
  - no WebGL / Components references

CanDoItAll.Economy.Simulation.SimpleAccounts
  - one simulation backend/transition engine

CanDoItAll.Economy.Simulation.Ledger
  - ledger-backed adapter

CanDoItAll.Economy.Simulation.Visualization
  - Economy visual nodes/actions/frames
  - no WebGL / Components references

CanDoItAll.Economy.Simulation.WebGlBridge
  - only place mapping Economy visual frames/actions to WebGlRunLib/WebGlLib concepts
  - may reference Components WebGlRunLib and WebGlLib
  - must not reference SimpleAccounts or Ledger backends

CanDoItAll.Economy.SimulationSandbox
  - connected workflow for input pack -> backend -> visual frames -> WebGL bridge -> snapshot
  - may reference SimpleAccounts initially, but should be backend-neutral
```

## Rule

The bridge direction is one way:

```text
Economy -> Components
```

Never:

```text
Components -> Economy
```

## Execution Closure

The completed bundle preserves this layering. SB01/SB02 prove the cross-repository boundary and runtime audit baseline, SB06/SB07/SB11 prove renderer-neutral bridge and mapping structure, and SB12 proves sandbox orchestration through backend-neutral seams while keeping SimpleAccounts as an adapter-wired backend path.
