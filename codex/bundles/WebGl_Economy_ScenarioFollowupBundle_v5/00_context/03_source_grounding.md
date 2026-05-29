# Source-grounded observations

## Components

- The `webgl-engine` branch solution contains `CanDoItAll.Components.WebGlRunLib`.
- `WebGlRunContracts.cs` is a good initial contract layer but currently remains mostly DTOs/interfaces.
- `RunPlayback.razor.cs` still contains playback loop logic, so generic playback orchestration should move into `WebGlRunLib`.
- `21-webgl-scene-asset-cache.js` provides a cache and disposal helper; lifecycle must explicitly own and dispose it.
- `23-webgl-scene-indexes.js` is now available for scene indexes, but patch operations must keep indexes synchronized.
- `WebGlSceneDocumentSerializer.cs` has grown large and should be split before adding scenario/run serialization complexity.

## Economy

- `CanDoItAll.Economy.Simulation.Abstractions` exists and currently has no project dependencies.
- `CanDoItAll.Economy.Simulation.SimpleAccounts` references only `Simulation.Abstractions`.
- `CanDoItAll.Economy.Simulation.Visualization` references only `Simulation.Abstractions`.
- `CanDoItAll.Economy.Simulation.Ledger` references `Simulation.Abstractions`, `Ledger`, `BusinessObjects`, and `Sdk`.
- `SimulationContracts.cs` currently includes scenario manifest, run identity, clock, frames, deltas, actors, stores, flows, relationships, issues, backend interfaces, and deterministic hashing.
- `SimpleSimulation.cs` includes two hardcoded proof scenarios. This is useful evidence, but the next step must introduce loadable scenario definitions and event streams.
- `EconomyVisualizationContracts.cs` maps frames to nodes/links/symbols but lacks temporal visual action contracts.

## Architectural conclusion

The repositories are moving in the correct direction. The next wave should not connect them directly. It should make the generic run/action layer and the economy scenario/event/visual-action layer mature enough that a future bridge can be small, explicit, and safe.
