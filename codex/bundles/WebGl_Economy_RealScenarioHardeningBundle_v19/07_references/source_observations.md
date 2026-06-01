# Source observations used for this bundle

Components:

- `30-webgl-scene-stage-runner.js` now includes queue, active barriers, completed/failed/skipped ids, result log and journal integration.
- `32-webgl-scene-stage-barriers.js` includes wait policies for seconds, motions, render idle and events.
- `14-webgl-scene-motion.js` now delegates queue operations to `29-webgl-scene-motion-queues.js`.
- `22-webgl-scene-scheduler.js` checks queued motions and pending stage work.

Economy:

- `EconomySimulationSandboxWorkflow.cs` composes loader, backend selector, visualization, WebGL projection and snapshots.
- `EconomySimulationSandboxPipelines.cs` contains backend registry and pipeline implementations.
- `EconomySimulationSandboxSessionService.cs` exposes session navigation, pause/resume, snapshot and analysis.
- `EconomyWebGlActionStageProjector.cs` projects visual actions into run stages with patches/motions.
- `EconomyWebGlRunValidator.cs` validates stage command content and source metadata.
- Snapshot contracts, serializer, diff, store and analyzers exist in Simulation.Abstractions.
