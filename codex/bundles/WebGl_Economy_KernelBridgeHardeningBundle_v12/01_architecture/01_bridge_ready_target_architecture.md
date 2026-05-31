# Bridge-Ready Target Architecture

## Components repo

```text
CanDoItAll.Components.WebGlLib
  - scene renderer
  - patch runtime
  - motion runtime
  - asset/model runtime
  - command batch executor
  - no Economy references

CanDoItAll.Components.WebGlRunLib
  - run document
  - frames/timeline
  - action planning
  - target/anchor resolver
  - visual state catalog
  - stage-aware command batches
  - playback controller
  - no Economy references

CanDoItAll.Components.WebGlSandbox
  - generic desktop-only proofs
  - no Economy references
```

## Economy repo

```text
CanDoItAll.Economy.Simulation.Abstractions
  - experiment input pack contracts
  - scenario/placement/parameter/rule/invariant contracts
  - event stream contracts
  - frame/delta contracts
  - metric/invariant contracts
  - no Components/WebGL references

CanDoItAll.Economy.Simulation.SimpleAccounts
  - simple transition backend
  - event handler registry
  - state transition engine
  - simple account/store model
  - no WebGL references

CanDoItAll.Economy.Simulation.Ledger
  - ledger-backed simulation adapter
  - no SimpleAccounts references

CanDoItAll.Economy.Simulation.Visualization
  - economy visual DTOs and visual-action mapper
  - no WebGL references

Future:
CanDoItAll.Economy.Simulation.WebGlBridge
  - references Economy.Visualization + Components.WebGlRunLib
  - maps economy visual frames/actions to WebGL run frames
  - this is the only project allowed to bridge Economy to WebGL
```

## Bridge boundary

The bridge must be explicit and isolated. Do not let `CanDoItAll.Components.*` reference Economy. Do not let low-level Economy abstractions reference WebGL. If a WebGL-specific mapping is needed, create a dedicated adapter project above both.
