# Current implementation review summary

## Components repo observations

The current `webgl-engine` branch has moved in the right direction:

- `CanDoItAll.Components.WebGlRunLib` exists in the solution and references `WebGlLib` only.
- `01-webgl-scene.js` is now a thin public facade over lifecycle, patching, motion, diagnostics, and command APIs.
- Scene runtime modules are split into lifecycle, graph, resources, model diagnostics, command results, asset cache, scheduler, indexes, and notifications.
- `WebGlSceneView` exposes import/export, patch, motion, diagnostics, and motion-completion APIs.

Remaining risks:

- `WebGlRunLib` is still mostly contracts. Real playback orchestration lives in the sandbox `RunPlayback.razor.cs` instead of the reusable run library.
- There is no generic visual-action/event pipeline yet. Current run frames are patch/motion driven, but a simulator needs to emit generic intentions like `actor goes to target`, `actor returns home`, `actor changes pose`, `show symbol`, or `perform work`.
- Target resolution is too low-level. A simulator should be able to send `sourceObjectId`, `targetObjectId`, `targetAnchor`, `returnAnchor`, not calculate WebGL coordinates itself.
- Asset cache disposal helpers exist, but lifecycle must explicitly dispose the asset cache; otherwise state-local GLB templates may survive longer than expected.
- Some JS files are near or above desired maintainability thresholds (`18-webgl-scene-model-diagnostics.js`, `11-webgl-scene-graph.js`, `13-webgl-scene-patching.js`).
- `WebGlSceneDocumentSerializer.cs` is long and has several responsibilities: normalization, hashing, validation, metadata filtering, sorting, and cloning.

## Economy repo observations

The current `main` branch has added good preparation projects:

- `CanDoItAll.Economy.Simulation.Abstractions`
- `CanDoItAll.Economy.Simulation.SimpleAccounts`
- `CanDoItAll.Economy.Simulation.Ledger`
- `CanDoItAll.Economy.Simulation.Visualization`

The project graph is directionally correct:

- `Simulation.Abstractions` has no project dependencies.
- `SimpleAccounts` references only `Simulation.Abstractions`.
- `Visualization` references only `Simulation.Abstractions`.
- `Ledger` references `Simulation.Abstractions`, `Economy.Ledger`, `BusinessObjects`, and `Sdk`.

Remaining risks:

- `SimulationContracts.cs` is too large and mixes scenario manifest, run identity, frame DTOs, backend contracts, and deterministic hashing.
- `SimpleSimulation.cs` is too large and hardcodes materialized frames instead of loading scenario definitions.
- Shared-well and entrepreneur scenarios exist, but they are not described as reusable scenario definitions with event templates and entity placement.
- `EconomyVisualizationContracts.cs` maps frames to visual nodes/links/symbols, but it does not yet express temporal visual intentions such as movement, pose, status animation, or resource-transfer animation.
- There is no common event vocabulary for backends. SimpleAccounts and Ledger should emit common simulation events, while preserving backend-specific evidence separately.

## Next objective

Prepare a generic pipeline:

`ScenarioDefinition -> SimulationEvents -> VisualIntentions -> WebGlRunActions -> WebGlLib commands`

without coupling Economy to WebGL and without baking the shared-well example into the engine.
