# Source references captured during review

## Components / WebGL runtime
- `src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs`
  - Play loop now uses `playbackGeneration`, a background `playbackTask`, cancellation token, and `StopRuntimeActivityAsync` on stop.
  - Critical lines reviewed: play loop, `StopPlaybackAsync`, `HandleMotionCompleted`, generation cancellation.
- `src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor`
  - Public runtime control APIs now include `CancelCommandStagesAsync` and `StopRuntimeActivityAsync`.
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/01-webgl-scene.js`
  - Exposes `cancelCommandStages` and `stopRuntimeActivity`.
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/39-webgl-scene-runtime-stop.js`
  - Clears active/queued motions and cancels command-stage runner work.
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/26-webgl-scene-command-batch.js`
  - `applyCommandBatch` queues stages and returns a batch result before all asynchronous stage barriers/motions necessarily complete.
- `src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs`
  - Adds `ApplyPlaybackAsync`, but per-frame application still uses command batch enqueue semantics.
- `src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunDocumentRunner.cs`
  - Runner now propagates `FromFrame` errors and aligns ordered stage ids with ordering policy.
- `src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunFrameExecutionValidator.cs`
  - Validator now uses `WebGlRunStageOrderingPolicy.OrderStages(frame)`.

## Economy simulation
- `src/CanDoItAll.Economy.Components/Components/SimulationSandbox/EconomySimulationSandboxPage.razor`
  - Scenario selector added, deterministic replay applies frames `<= currentFrame.Index` through `ApplyPlaybackAsync`.
- `src/CanDoItAll.Economy.SimulationSandbox/EconomySimulationSandboxContracts.cs`
  - Scenario source, manifest, async APIs and v2 session export fields added, but legacy path fields remain.
- `src/CanDoItAll.Economy.SimulationSandbox/FileSystemEconomySimulationScenarioCatalog.cs`
  - Manifest validation, required file checks, pack hash calculation, content hash validation and path safety checks added.
- `src/CanDoItAll.Economy.SimulationSandbox/EconomySimulationSandboxSessionService.cs`
  - Load/Project support scenario source, async export, session import with pack hash validation; sync APIs remain.
- `src/CanDoItAll.Economy.Simulation.SimpleAccounts/SimpleSimulationStateTransitionEngine.cs`
  - Default transition options make unknown events and insufficient stock warnings unless stricter options are supplied.
- `src/CanDoItAll.Economy.Simulation.SimpleAccounts/SimpleSimulationStateTransitionEngine.Mutations.cs`
  - Store resolution can fall back heuristically; duplicate actor/resource stores are collapsed to the first store.
- `src/CanDoItAll.Economy.Simulation.Abstractions/Events/SimulationEventKindRegistry.cs`
  - Event kind aliases and ordering policy exist.
- `src/CanDoItAll.Economy.Simulation.Abstractions/Events/SimulationEventStream.cs`
  - Behavior expander injects hard-coded sequences for need/use/trade/rule events.
- `src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationMetricAndInvariantEvaluation.cs`
  - Unknown metrics infer or default to `resource-total` / 0-like behavior.
- `src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationInvariantEvaluation.cs`
  - Unknown invariant kinds fall back to metric thresholds; missing metric values can become 0.
- `tests/CanDoItAll.Economy.Tests/EconomyPerformanceProbeTests.cs`
  - Current performance proof is broad and useful, but many thresholds are warning-only and do not establish scientific validity.
