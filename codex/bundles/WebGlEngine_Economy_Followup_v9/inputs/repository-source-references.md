# Repository evidence references

This bundle is based on current pushed code inspected through GitHub connector.

## Components
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/40-webgl-scene-runtime-idle.js` — runtime idle API and blockers.
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/39-webgl-scene-runtime-stop.js` — stop/cancel runtime activity.
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/26-webgl-scene-command-batch.js` — command lifecycle and applyCommandBatchAndWait.
- `src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor` — C# wrappers for apply/wait/stop.
- `src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs` — pause/playback UI, observer proof and diagnostics.
- `src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunDocumentValidator.cs` — generic boundary/provenance/domain term guard.

## Economy
- `src/CanDoItAll.Economy.SimulationSandbox/EconomyExperimentReadiness.cs` — readiness bands, research statuses, performance budgets and headless runner.
- `src/CanDoItAll.Economy.SimulationSandbox/EconomyExperimentDesignHarness.cs` — design matrix, factor materialization and deterministic summaries.
- `src/CanDoItAll.Economy.Simulation.Abstractions/Experiment/SimulationExperimentPolicies.cs` — ResearchStrict policy, behavior profiles, metric/invariant registries.
- `src/CanDoItAll.Economy.Simulation.Abstractions/Scenario/SimulationScenarioValidation.cs` — strict scenario validation including expansion profile and store policy references.
- `src/CanDoItAll.Economy.Simulation.SimpleAccounts/SimpleSimulationStateTransitionEngine.cs` — strict transition options and policy threading.
- `src/CanDoItAll.Economy.SimulationSandbox/FileSystemEconomySimulationScenarioCatalog.cs` — scenario manifest pack/file hash validation.
- `docs/simulation/experiment-readiness.md` — current readiness documentation and troubleshooting.
