# Current review findings

## Components repo

Observed current implementation:

- JS runtime audit now enforces runtime JS line limits, public façade limit, unsafe DOM pattern checks, import graph checks, domain-neutrality checks, branch-creation instruction checks, large-screen policy checks and C# file-size checks.
- `14-webgl-scene-motion.js` now delegates queue handling to `29-webgl-scene-motion-queues.js`.
- `26-webgl-scene-command-batch.js` now delegates stage queue handling to `30-webgl-scene-stage-runner.js`.
- Render loop calls `advanceCommandBatchStages` and `advanceMotions` every rendered frame.

Risks:

- Several JS runtime modules are still near the warning threshold. They are acceptable now, but should be split if they grow further.
- Stage runner currently handles `waitSeconds`, but needs richer barrier modes: wait for active motions, wait for motions on specific object ids, wait for render idle and wait for explicit event.
- Delayed stage results are not yet a first-class command journal that can be queried after the initial batch call returns.
- Motion queue is much better, but needs stress proof for multiple queued motions on many objects.

## Economy repo

Observed current implementation:

- `Simulation.WebGlBridge` exists in Economy and maps visual frames/actions to WebGL run documents.
- `SimulationSandbox` exists and currently orchestrates loader -> simple simulation -> visual mapping -> WebGL projection -> validation.
- `SimulationRunSnapshot`, serializer, diff, snapshot store and visual-state builder exist.
- Snapshot analysis probe exists and proves export/import and visual-state analysis on the shared-resource example.

Risks:

- `SimulationSandbox` is still tied to `SimpleSimulationStateTransitionEngine`; it should be backend-neutral via `ISimulationBackend` or a backend registry.
- `EconomyWebGlInitialSceneProjector.cs` and `EconomyVisualMappingDefinition.cs` are broad and need decomposition before more features accumulate.
- Snapshot analysis is mostly test-local; reusable analyzer services are not yet production contracts.
- `SimulationSnapshotDiff` should include relationships, visual state, provenance hash changes and metadata changes; current diff appears too narrow.
- `Simulation.WebGlBridge` uses WebGL types, which is expected, but must remain the only Economy layer allowed to do that.
