# Repository source references

## CanDoItAll.Components current branch

- `repo://CanDoItAll.Components/README.md` — package map includes `WebGlLib` and `WebGlRunLib`.
- `repo://CanDoItAll.Components/CanDoItAll.Components.slnx` — solution includes WebGlLib, WebGlRunLib, WebGlSandbox, and both test projects.
- `repo://CanDoItAll.Components/Directory.Build.props` — shared package version and package metadata.
- `repo://CanDoItAll.Components/docs/webgl/run-layer-boundary.md` — declared layer boundary.
- `repo://CanDoItAll.Components/package.json` — WebGL audit scripts.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/11-webgl-scene-graph.js` — scene rebuild and incremental transform runtime.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/13-webgl-scene-patching.js` — JS patch validation/classification/application.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/34-webgl-scene-revisions.js` — JS revision policy.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/35-webgl-scene-patch-validation.js` — JS patch preflight.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/36-webgl-scene-patch-classification.js` — JS incremental patch classifier.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/16-webgl-scene-models.js` — GLB instance/material clone handling.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/17-webgl-scene-resources.js` — resource ownership/disposal policy.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Patching/WebGlScenePatchReducer.cs` — C# patch reducer.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneRevisionPolicy.cs` — C# revision policy.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneDocumentNormalizer.cs` — scene document normalization.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeDiagnostics.cs` — C# diagnostic DTO parity surface.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/README.md` — run layer contracts and generic boundary.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunFrameApplyResult.cs` — frame-to-command-batch compilation for playback.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs` — browser apply adapter and scene reset path.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunDocumentValidator.cs` — generic run document validator and forbidden domain terms.

## CanDoItAll.Economy current branch

- `repo://CanDoItAll.Economy/README.md` — currently ledger-oriented and likely stale after simulation additions.
- `repo://CanDoItAll.Economy/CanDoItAll.Economy.slnx` — solution includes the new simulation family.
- `repo://CanDoItAll.Economy/NuGet.config` — package migration proof source.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/CanDoItAll.Economy.Simulation.Abstractions.csproj` — generic simulation abstractions.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Visualization/CanDoItAll.Economy.Simulation.Visualization.csproj` — generic visual DTO mapping layer.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.SimpleAccounts/CanDoItAll.Economy.Simulation.SimpleAccounts.csproj` — simple account backend examples.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/CanDoItAll.Economy.Simulation.WebGlBridge.csproj` — Economy bridge to Components WebGlRunLib.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRunProjector.cs` — bridge document projection.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlActionStageProjector.cs` — visual actions to WebGlRun stages.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRunValidator.cs` — bridge output validation.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Components/CanDoItAll.Economy.Components.csproj` — component package/project reference strategy.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Components/Components/SimulationSandbox/EconomySimulationSandboxPage.razor` — browser UI route internals.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/Components/Pages/SimulationSandbox.razor` — Node route for the sandbox.
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationSandboxSessionTests.cs` — headless session/export/import coverage.
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/shared-well/*` — current runtime fixture source used by the component.
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/farmer-land/*` — current fixture source used by tests.
