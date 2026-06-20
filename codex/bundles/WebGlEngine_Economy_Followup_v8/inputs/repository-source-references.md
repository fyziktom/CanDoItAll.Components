# Repository source references

## Components current-state references

- `CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/39-webgl-scene-runtime-stop.js`
  - `stopRuntimeActivity` increments `runtimeStopGeneration`, cancels command stages, clears motions, and writes stop diagnostics.
- `CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/40-webgl-scene-runtime-idle.js`
  - `waitForRuntimeIdle` polls blockers: active/queued motions, queued/active command stages, barriers, pending asset disposal, render-loop state, and continuous render mode.
- `CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/26-webgl-scene-command-batch.js`
  - `applyCommandBatch` can return scheduled work before runtime is idle; `applyCommandBatchAndWait` waits and annotates lifecycle/idle metadata.
- `CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor`
  - C# exposes `ApplyCommandBatchAndWaitAsync`, `StopRuntimeActivityAsync(waitForIdle: ...)`, and `WaitForRuntimeIdleAsync`.
- `CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs`
  - Demo now has generation-based play cancellation, runtime stop, idle capture, and late drain.
- `CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs`
  - Browser playback adapter supports idle wait policy, cancellation, failure snapshots, and multi-frame playback.

## Economy current-state references

- `CanDoItAll.Economy.SimulationSandbox/EconomyExperimentReadiness.cs`
  - Readiness report v2, status, gates, confidence levels, warning budgets, headless runner, performance budgets, and run manifests.
- `CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationExperimentPolicies.cs`
  - `ResearchStrict`, explicit metric/invariant registries, behavior expansion profiles.
- `CanDoItAll.Economy.Simulation.Abstractions/Scenario/SimulationScenarioValidation.cs`
  - Strict mode requires explicit expansion profile and validates store-resolution policies.
- `CanDoItAll.Economy.Simulation.SimpleAccounts/SimpleSimulationStateTransitionEngine.cs`
  - `SimpleSimulationTransitionOptions.ForPolicy` maps research policy into strict transition behavior.
- `CanDoItAll.Economy.Simulation.SimpleAccounts/SimpleSimulationStateTransitionEngine.Mutations.cs`
  - Store resolution and transfer logic; ambiguity and rejected-flow behavior are critical research-noise sources.
- `CanDoItAll.Economy.SimulationSandbox/FileSystemEconomySimulationScenarioCatalog.cs`
  - Scenario manifest validation includes required files, pack hash, file hashes, and extra-file policy.
- `CanDoItAll.Economy.SimulationSandbox/EconomyExperimentDesignHarness.cs`
  - Design matrix currently records factor levels and configuration hashes but needs proof that factor levels actually mutate scenario inputs.
- `tests/CanDoItAll.Economy.Tests/SimulationEconomicTrustHardeningTests.cs`
  - Existing tests cover strict mode, store resolution, metric/invariant registry, behavior profiles, golden oracle cases, and readiness statuses.
