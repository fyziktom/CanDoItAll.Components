# Economy review findings

## Current positive state

- `CanDoItAll.Economy.Simulation.Abstractions`, `Simulation.SimpleAccounts`, `Simulation.Ledger`, and `Simulation.Visualization` are present in the solution.
- Current project references are correctly isolated:
  - `Simulation.Abstractions` has no project references.
  - `Simulation.SimpleAccounts` references only `Simulation.Abstractions`.
  - `Simulation.Visualization` references only `Simulation.Abstractions`.
  - `Simulation.Ledger` references `Simulation.Abstractions`, `Economy.Ledger`, `Economy.BusinessObjects`, and `Economy.Sdk`.
- The architecture boundary document correctly states that Economy simulation prep should not reference WebGL/Components.
- Simple seed scenarios exist for a shared well community and a small entrepreneur community.
- Basic tests validate deterministic hashing, simple scenario frames/deltas, visualization mapping, and ledger adapter preparation.

## Main concerns

1. `SimulationContracts.cs` is a very large mixed contract file. It contains manifest, run identity, clock, frames, deltas, actors, stores, flows, relationships, issues, artifacts, backend interfaces, and deterministic hashing. Split it by responsibility.
2. `SimpleSimulation.cs` is also too large and mixes simple accounts models, backend, scenario factory, shared well seed, entrepreneur seed, delta builder, helpers, and flow/store creation.
3. The current simple scenarios are materialized as hardcoded frames/deltas. They are not yet loadable scenario definitions.
4. The current abstraction lacks explicit scenario events and action/intention events such as:
   - actor draws resource from source
   - actor walks to source
   - actor returns home
   - actor performs administration
   - actor sells stock to another actor
   - actor shows conflict/trust/scarcity/admin symbol
5. Current visualization DTOs map frames to nodes/links/symbols, but do not express temporal visual intentions.
6. Flow store ids in the simple scenarios look pattern-generated and may not correspond to actual `SimulationResourceStore.StoreId` values. Add validation for dangling store ids.
7. Some flow timestamps appear to be created with a static scenario start timestamp rather than the frame clock. Add timestamp validation.
8. Ledger delta currently returns all target stores/issues rather than a true minimal diff. Good as prep, but inefficient for large runs.

## Recommended action

Do not bridge to WebGL yet. First add loadable, deterministic, backend-neutral scenario definitions, a simulation event stream, and WebGL-independent visual intention DTOs. The future bridge will map those visual intentions into `WebGlRunAction` objects in a separate integration package.
