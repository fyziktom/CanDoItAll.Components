# Review findings: CanDoItAll.Economy / current `main`

## What is good

The repo now contains the intended preparation projects:

- `CanDoItAll.Economy.Simulation.Abstractions`
- `CanDoItAll.Economy.Simulation.SimpleAccounts`
- `CanDoItAll.Economy.Simulation.Ledger`
- `CanDoItAll.Economy.Simulation.Visualization`

The solution includes all four new projects.

The new project graph is mostly right:

- `Simulation.Abstractions` has no project dependencies.
- `Simulation.SimpleAccounts` references only `Simulation.Abstractions`.
- `Simulation.Visualization` references only `Simulation.Abstractions`.
- `Simulation.Ledger` references `Simulation.Abstractions`, `Economy.Ledger`, `Economy.BusinessObjects`, and `Economy.Sdk`.

This is consistent with the documented boundary in `docs/simulation/architecture-boundaries.md`.

## Remaining risks

### E-SIM-001: Large files were introduced

The current implementation has several files that will become difficult to maintain:

- `SimulationContracts.cs` is a large mixed contract/hash/backend file.
- `SimpleSimulation.cs` combines models, backend, scenario factory, shared-well seed, entrepreneur seed, frame builders, store builders, and delta builders.
- `EconomyVisualizationContracts.cs` combines contracts, mapper, category normalization, layout hints, symbol mapping, and link mapping.
- `LedgerSimulationAdapter.cs` combines descriptors, source DTOs, projector, backend, evidence mapper, and issue mapper.

Expected fix:
Split these into cohesive files before adding more scenario/event complexity.

### E-SIM-002: Scenarios are hardcoded frames, not loadable scenario definitions

`SimpleSimulationScenarioFactory` currently returns ready-made frames/deltas. This is useful proof data, but it does not yet solve how users define scenarios, store them, load them, validate them, and initiate simulation from them.

Expected fix:
Add `SimulationScenarioDefinition` contracts that describe:

- actors;
- resources;
- locations;
- initial stores;
- relationships;
- rules;
- event schedule;
- random seed;
- backend kind;
- validation constraints.

The simple backend should materialize frames from definitions.

### E-SIM-003: Simulation events are missing

Frames contain resource stores, flows, relationships, and issues, but no explicit event/action stream. For visualization playback, we need explicit semantic events such as:

- actor uses resource;
- actor travels to target;
- actor returns home;
- actor buys/sells/transfers resource;
- actor changes work/admin state;
- actor violates rule;
- institution enforces rule;
- relationship changes.

Expected fix:
Add `SimulationEvent` / `SimulationEventEffect` / `SimulationScheduledEvent` to `Simulation.Abstractions`, and include them in frames/deltas.

### E-SIM-004: Visualization frame lacks action semantics

`EconomyVisualFrame` currently emits nodes, links, layers, symbols and delta IDs. It does not describe temporal visual actions such as move-to-target, return-home, pose change, show symbol, or resource transfer pulse.

Expected fix:
Add `EconomyVisualAction` contracts in `Simulation.Visualization`. They must be economy-side neutral visual intentions and must not reference WebGL DTOs.

### E-SIM-005: Layout is deterministic but still too simplistic

Current visual layout uses index-based coordinates. For shared-well community, distance matters because closer actors can stockpile and resell water. Layout should support:

- fixed location anchors from scenario definition;
- deterministic seeded random placement;
- resource location nodes;
- home/work/market anchors;
- distance/cost metadata.

### E-SIM-006: Simple and ledger backend isolation must be preserved

Do not allow convenience shortcuts where simple-account code references ledger types or ledger-backed code references simple-account seed types. Shared definitions must move upward into `Simulation.Abstractions`.

### E-SIM-007: Existing `CanDoItAll.Economy.Simulator` must not be broken

The repo already has a richer simulator around projects/runs/lifecycle/API/UI. The new `Simulation.*` projects are preparation layers. Do not prematurely merge them into the existing simulator app. Add adapters only after contracts stabilize.
