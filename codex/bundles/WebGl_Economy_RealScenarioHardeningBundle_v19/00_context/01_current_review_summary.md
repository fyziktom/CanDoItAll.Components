# Current review summary

## Components

The WebGL runtime has moved in the right direction:

- `30-webgl-scene-stage-runner.js` now has a command stage runner with queue, active barriers, completed/failed/skipped stage ids, result log and command-stage journal integration.
- `32-webgl-scene-stage-barriers.js` now supports generic barrier policies:
  - `wait-seconds`
  - `wait-for-active-motions`
  - `wait-for-object-motions`
  - `wait-for-render-idle`
  - `wait-for-event`
- `14-webgl-scene-motion.js` uses `29-webgl-scene-motion-queues.js`, so per-object queued motion is no longer only a planned concept.
- `22-webgl-scene-scheduler.js` checks both active and queued motions and pending stage work before going idle.

This is a good foundation, but it still needs hardening before we can trust browser playback in a real scenario.

## Economy

The Economy side is now much closer to a joined pipeline:

- `EconomySimulationSandboxWorkflow` composes loader, backend selector, visualization pipeline, WebGL projection pipeline and snapshot pipeline.
- `EconomySimulationSandboxSessionService` exposes load/project/step/seek/pause/resume/snapshot/analyze plus safe operation wrappers.
- `EconomyWebGlRunProjector` projects visual frames into a `WebGlRunDocument`.
- `EconomyWebGlInitialSceneProjector` was split into smaller projector helpers.
- `EconomyWebGlActionStageProjector` produces executable stages with patches/motions or explicit wait stages.
- Snapshot contracts and stores exist and are now part of the sandbox flow.

The important remaining question is no longer "do we have the types?" but "can we safely execute the full loop and diagnose it when something goes wrong?"
