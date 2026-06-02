# Target Architecture

## Layering target

```text
CanDoItAll.Components.WebGlLib
  - Scene DTOs
  - Scene documents
  - Asset catalog and fallback rendering
  - WebGlSceneView
  - Browser runtime and diagnostics
  - No run documents
  - No domain semantics

CanDoItAll.Components.WebGlRunLib
  - Generic run documents
  - Timelines, frames, action stages
  - Generic planner/compiler
  - Browser apply adapter
  - Provenance envelope
  - No economy/ledger/market/production-line semantics

CanDoItAll.Economy.Simulation.Abstractions
  - Generic Economy simulation scenario/input/snapshot contracts
  - No WebGL/Blazor/Components dependency

CanDoItAll.Economy.Simulation.Visualization
  - Economy visual DTOs
  - No Components dependency

CanDoItAll.Economy.Simulation.WebGlBridge
  - Economy visual DTO -> WebGlRunDocument/WebGlSceneModel mapping
  - Economy-specific validation

CanDoItAll.Economy.SimulationSandbox
  - Scenario catalogs
  - Session services
  - Snapshot/export/import
  - Runtime pack loading

CanDoItAll.Economy.Components / Node
  - UI and host registration
```

## Safety principles

- Public runtime APIs should be safe even if the caller forgot to run a validator.
- Validators remain required, but APIs must still fail loudly on unsafe input.
- Scenario catalogs should expose portable scenario ids and manifests, not only absolute paths.
- Browser reset/import must preserve scene document runtime options or explicitly report why it cannot.
- `source.*` provenance is traceability only. It must not drive generic runtime decisions.
- Large-scene performance limits should be explicit budgets with diagnostics and tests.
