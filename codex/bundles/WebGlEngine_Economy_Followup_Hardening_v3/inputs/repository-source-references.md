# Repository Source References

These references are exact starting points for Codex. Use current branch heads, not stale local copies.

## Components

- `repo://CanDoItAll.Components/README.md`
  - WebGlRunLib is now documented as a generic run/playback/action/stage layer above WebGlLib.
  - Package output is still versioned together and documented as `0.1.0`.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/README.md`
  - Documents `WebGlRunDocument`, `WebGlRunTimeline`, `WebGlRunFrame`, `WebGlRunActionStage`, validators, browser apply adapter, and `/run-playback`.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunDocumentValidator.cs`
  - Validates schema/timeline, rejects mixed direct+staged commands, allows `source.*` provenance keys, and blocks domain terms.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunFrameCommandPolicy.cs`
  - Defines the current policy: frames cannot mix direct frame-level commands with stages.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunFrameApplyResult.cs`
  - Still maps direct `ScenePatches` and `Motions` only when the frame has no stages.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs`
  - Imports initial scene and then applies frame command batch; currently needs reset fail-fast review.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/13-webgl-scene-patching.js`
  - Patch validation, patch classification, incremental patch behavior, and revision commit.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/17-webgl-scene-resources.js`
  - Resource ownership and texture disposal policy.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/21-webgl-scene-asset-cache.js`
  - State-local asset cache behavior and disposal.
- `repo://CanDoItAll.Components/package.json`
  - Runtime audit scripts: `webgllib:audit-scene-runtime`, `webgllib:audit-scene-runtime-imports`, `webgllib:audit-boundary`, `webglrunlib:audit-boundary`, resource ownership test.

## Economy

- `repo://CanDoItAll.Economy/README.md`
  - Simulation package map and WebGL integration documentation were added.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Components/Components/SimulationSandbox/EconomySimulationSandboxPage.razor`
  - Runtime page injects `IEconomySimulationSandboxSessionService` and `IEconomySimulationScenarioCatalog`.
  - It loads a default scenario but does not yet expose a real scenario picker or invalid-state UX.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/EconomyNodeServiceRegistration.cs`
  - Registers the sandbox session service and a file-system scenario catalog for Node.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/CanDoItAll.Economy.Node.csproj`
  - Copies runtime scenario files to output/publish under `SimulationScenarios/EconomySimulationSandbox`.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomySimulationSandboxContracts.cs`
  - Catalog, session service, backend, pipeline, and export contracts.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/FileSystemEconomySimulationScenarioCatalog.cs`
  - Safe file-system catalog implementation for runtime scenario packs.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/EconomySimulationSandboxSessionService.cs`
  - Session load/project/step/seek/snapshot/analyze/export/import. Still path-centric and contains sync-over-async snapshot calls.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRunValidator.cs`
  - Economy-specific WebGL run validator with source metadata checks and dynamic object tracking.
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomySimulationSandboxComponentTests.cs`
  - BUnit test manually registers services and catalog; this reveals missing reusable registration surface for non-Node consumers.
