# Current review findings

## Components repository

The WebGL runtime is now much healthier than earlier versions:

- The JS runtime has an audit script enforcing line-count thresholds, unsafe-pattern checks, import graph checks, domain neutrality, asset includes, branch instruction checks, large-screen policy checks, and C# file-size checks.
- Stage runner support exists in `30-webgl-scene-stage-runner.js`.
- Per-object motion queues exist in `29-webgl-scene-motion-queues.js`.
- The render loop advances both command batch stages and motions.

Remaining concerns:

1. Stage runner has `waitSeconds`, but the next phase needs **motion-completion stage barriers**, not only fixed time waits.
2. Delayed stage execution happens after the original command result is returned, so proof/diagnostics for delayed stage children must be persisted in runtime diagnostics or a stage result log.
3. Motion queue behavior needs explicit tests for A → B → C → home with recalculated start positions.
4. JS modules are currently under control, but the audit script itself is large. The runtime is the priority, but the audit script should eventually be split into checks.
5. The allowed large C# command batch file remains a split candidate.

## Economy repository

Economy now contains the right projects:

- `Simulation.Abstractions`
- `Simulation.SimpleAccounts`
- `Simulation.Ledger`
- `Simulation.Visualization`
- `Simulation.WebGlBridge`
- `SimulationSandbox`

Positive findings:

1. The connected bridge is in Economy, not Components.
2. Snapshot contracts exist.
3. Snapshot serializer, diff, in-memory store, and visual state attachment exist.
4. SimulationSandbox exists and can project an input pack through simulation, visualization, and WebGL bridge.
5. Strict input pack validation has improved.

Remaining concerns:

1. `EconomyWebGlInitialSceneProjector.cs` is large and mixes layer, node, link, symbol, visual state catalog, metadata, and diagnostic projection.
2. Snapshot analysis still lives mainly in tests. It should become a reusable analyzer service.
3. `SimulationSandbox` currently depends directly on `SimpleSimulationStateTransitionEngine`; it needs a generic backend/orchestrator interface before ledger-backed or alternative simulations.
4. `EconomyVisualMappingDefinition.cs` is large and sits in `Simulation.Abstractions`; it contains mapping concerns that may deserve a separate `Simulation.Visualization.Abstractions` or split files.
5. Fixture/probe tests contain domain terms by design, but generic code must remain clean.
