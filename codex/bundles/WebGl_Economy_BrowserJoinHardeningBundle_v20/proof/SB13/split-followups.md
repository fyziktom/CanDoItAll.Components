# SB13 Split Follow-Ups

Status: Follow-ups recorded; no SB13 source refactor required for closure.

## Thresholds Used

- JS runtime module warning threshold: 220 lines, from `tools/webgllib/audit-scene-runtime.cjs`.
- Bundle-relevant broad C# test threshold: 700 lines.
- Production C#/Razor review threshold: 800 lines unless generated or unrelated to this bundle.

## JS Runtime Modules Over 220 Lines

- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/13-webgl-scene-patching.js` - split patch normalization from patch application.
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/02-webgl-scene-core.js` - split state initialization from runtime diagnostics defaults.
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/28-webgl-scene-command-batch-normalizer.js` - split stage normalization helpers from batch flattening.
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/10-webgl-scene-lifecycle.js` - split boot/dispose wiring from runtime host wiring.
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/14-webgl-scene-motion.js` - split motion command creation from animation ticking.
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/30-webgl-scene-stage-runner.js` - split runner state from stage execution transitions.
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/32-webgl-scene-stage-barriers.js` - split policy resolution from asynchronous wait helpers.
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/11-webgl-scene-graph.js` - split object graph CRUD from link/symbol refresh coordination.
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/18-webgl-scene-model-diagnostics.js` - split model inspection from diagnostic formatting.
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/03-webgl-scene-assets.js` - split asset loading from cache coordination.
- `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/04-webgl-scene-symbols.js` - split symbol CRUD from symbol material/render helpers.

## Bundle-Relevant Broad Tests

- `C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyPerformanceProbeTests.cs` - split synthetic scenario builders, measurement/report helpers, and assertion probes into dedicated helper files.
- `C:/repositories/CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationPreparationTests.cs` - split loader/normalizer coverage from ledger descriptor and input-pack coverage.
- `C:/repositories/CanDoItAll.Components/tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunActionPlannerTests.cs` - split provenance/domain-vocabulary tests from action planning tests.

## Existing Large Files Outside This Bundle

- `C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulator/VefFinancialPlanScenarioData.g.cs` is generated and should stay generated.
- `C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Simulator/SimulationRunLifecycleServices.cs`, `C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Persistence.EFCore/EfLedgerStore.cs`, and `C:/repositories/CanDoItAll.Economy/src/CanDoItAll.Economy.Node/EconomyNodeHost.cs` exceed production review thresholds but are outside the WebGL/Economy browser-join change surface.
- `C:/repositories/CanDoItAll.Components/src/CanDoItAll.Components.CanvasLib/Components/Workbench/CanvasWorkbench.razor` and sandbox demo pages exceed review thresholds but are unrelated to this bundle.
