# Target layering and ownership

## Components repository

```text
CanDoItAll.Components.WebGlLib
  - Owns: scene model, scene document, asset catalog, WebGL/Three.js runtime, primitive/model loading, interaction, selection, drag, patch application, command batch execution, diagnostics, proof snapshots.
  - Must not own: run documents, economy, production line, simulator domain state, scenario persistence, business rules.

CanDoItAll.Components.WebGlRunLib
  - Owns: generic run document, timeline, frame, action, action stage, generic action planning/compilation, playback controller, browser apply adapter over WebGlSceneView.
  - Must not own: economy/ledger/market/account semantics, production line/work-order semantics, domain-specific scenario catalogs.

CanDoItAll.Components.WebGlSandbox
  - Owns: generic proof routes and visual browser regression surfaces.
```

## Economy repository

```text
CanDoItAll.Economy.Simulation.Abstractions
  - Owns: generic simulation scenario/run/frame/event/snapshot contracts.

CanDoItAll.Economy.Simulation.Visualization
  - Owns: economy-neutral-enough visual DTOs produced from simulation snapshots/events.

CanDoItAll.Economy.Simulation.SimpleAccounts
  - Owns: example/simple simulation backend and scenario factories.

CanDoItAll.Economy.Simulation.WebGlBridge
  - Owns: mapping from Economy visual DTOs to WebGlRunLib generic run documents.

CanDoItAll.Economy.SimulationSandbox
  - Owns: session service, scenario catalog integration, snapshot/analyze/export/import operations.

CanDoItAll.Economy.Components
  - Owns: browser UI components that consume the session service and Components packages.
```

## Hard rule

No dependency may point from Components packages into Economy. Economy may consume Components packages through project references during local development or package references during package proof.
