# Current review summary

## What is now good

### Components

The WebGL runtime has moved in the right direction:

- `30-webgl-scene-stage-runner.js` now has a stage queue, active barriers, completed/failed/skipped stage IDs, a result log, and command-stage journal integration.
- `32-webgl-scene-stage-barriers.js` supports generic barrier policies: `wait-seconds`, `wait-for-active-motions`, `wait-for-object-motions`, `wait-for-render-idle`, and `wait-for-event`.
- `14-webgl-scene-motion.js` now uses `29-webgl-scene-motion-queues.js`, which gives us a real per-object motion queue foundation.
- `22-webgl-scene-scheduler.js` knows about pending stage work and queued motions.
- `tools/webgllib/audit-scene-runtime.cjs` monitors JS file lengths, unsafe DOM patterns, import cycles, domain neutrality, large-screen policy, and C# file sizes.

### Economy

The Economy repository now contains the connected side where it belongs:

- `CanDoItAll.Economy.Simulation.WebGlBridge` is in the Economy solution.
- `CanDoItAll.Economy.SimulationSandbox` is in the Economy solution.
- `EconomySimulationSandboxWorkflow` is now pipeline-oriented and backend-selectable instead of directly hardcoding the simple account engine in its main workflow.
- `EconomyWebGlRunProjector`, `EconomyWebGlInitialSceneProjector`, and `EconomyWebGlActionStageProjector` form a real projection chain from Economy visual frames/actions into WebGL run documents.
- `SimulationRunSnapshot`, serializer, diff, store, and visual-state attachment exist.

## Main remaining risks

### 1. Executable run proof is still not strong enough

The bridge produces `WebGlRunDocument`, initial scene, frames, stages, scene patches, and motions. But we still need a stronger proof that a `WebGlRunDocument` can be executed by the generic WebGL runtime through a stable API, not only generated and validated as data.

### 2. Stage barriers exist, but need behavioral proof

Barrier policies exist, but we need tests proving exact behavior:

- stage B does not start until stage A's object motions finish;
- a manual event barrier does not keep the render loop spinning;
- queued stage diagnostics remain queryable after delayed execution;
- failed delayed stages appear in the bounded journal and diagnostics.

### 3. Snapshot analysis exists, but is still too test-local

There is a good snapshot analysis probe, but it is still mostly test code. We need reusable services:

- `ISimulationRunSnapshotBuilder`
- `ISimulationSnapshotAnalyzer`
- file-backed snapshot store
- generic analysis facets
- visual-state diffing

### 4. Generic visual mapping still risks renderer leakage

`EconomyVisualMappingDefinition` lives in `Simulation.Abstractions` but contains WebGL-oriented validation concepts such as WebGL asset markers. The intent is renderer-neutral visual mapping, so either rename these checks to renderer-specific checks or move WebGL-specific checks into `Simulation.WebGlBridge`.

### 5. Projector/refactoring risk

`EconomyWebGlInitialSceneProjector` and `EconomyVisualMappingDefinition` are both broad files. They are under current line gates but still carry multiple responsibilities. They should be split before more behavior is added.

### 6. SimulationSandbox still needs an executable session concept

The workflow currently projects an experiment into backend frames, visual frames, WebGL document, and snapshots. The next step is a session API that can:

- load an input pack;
- choose backend;
- materialize frames;
- produce visual frames;
- produce a WebGL run document;
- expose current step/frame;
- pause/resume/step;
- create a snapshot at the current step;
- return diagnostics and analysis without UI assumptions.
