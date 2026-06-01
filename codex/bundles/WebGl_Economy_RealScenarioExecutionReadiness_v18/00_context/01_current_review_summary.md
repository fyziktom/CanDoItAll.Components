# Current Review Summary

## Components repo

The current `webgl-engine` branch has made meaningful progress:

- WebGL runtime stage runner now supports queued stages, active barriers, completed/failed/skipped stage IDs, result logs, and command-stage journal diagnostics.
- Stage barriers include `wait-seconds`, `wait-for-active-motions`, `wait-for-object-motions`, `wait-for-render-idle`, and `wait-for-event`.
- Motion runtime now has a per-object motion queue helper.
- Scheduler now considers queued motions and pending stage work.
- JS runtime audit enforces file-size, public facade, unsafe DOM, import graph, domain-neutrality, branch instruction, and large-screen policy checks.

Remaining risks:

- Some JS modules are still over the warning line threshold and should either be split or explicitly justified.
- Stage execution has stronger pieces, but still needs more proof that a real WebGlRunDocument can drive runtime commands frame-by-frame.
- We need stronger command/stage journal tests that prove delayed stages and barrier completions remain traceable after the initial interop call returns.

## Economy repo

The current main branch has made meaningful progress:

- `CanDoItAll.Economy.Simulation.WebGlBridge` exists in Economy, not Components.
- `CanDoItAll.Economy.SimulationSandbox` exists and uses pipeline services.
- Sandbox workflow now uses a backend selector, visualization pipeline, WebGL projection pipeline, and snapshot pipeline.
- Shared-well and farmer-land fixtures are already used as generic probes.
- Snapshot model, serializer, diff, store, file store, builder, analysis service, and visual-state attachment exist.
- Visual mapping was split into smaller files and bridge projectors were split.

Remaining risks:

- The sandbox session is still mostly synchronous and precomputed; real interactive stepping/playback needs a stronger session lifecycle model.
- The WebGL bridge produces executable-looking stages, but we still need a stronger executable runtime proof, not only DTO validation.
- Snapshot analysis has been promoted, but we need reusable, queryable analysis facets and artifact exports for debugging real scenarios.
- Some tests are very large and should be split before they become hard to maintain.
