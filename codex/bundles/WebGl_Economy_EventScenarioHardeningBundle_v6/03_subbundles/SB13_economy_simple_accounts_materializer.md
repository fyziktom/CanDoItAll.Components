# SB13 - Economy: SimpleAccounts scenario materializer

Refactor `SimpleSimulation.cs` into smaller files:

- `SimpleSimulationBackend.cs`
- `SimpleSimulationScenarioCatalog.cs`
- `SimpleScenarioDefinitionMaterializer.cs`
- `SimpleFrameDeltaBuilder.cs`
- `SharedWellScenarioDefinitionFactory.cs`
- `EntrepreneurScenarioDefinitionFactory.cs`

Move hardcoded frame data into scenario definition factories.

Materializer requirements:

- consume `SimulationScenarioDefinition`;
- emit frames, deltas, and events;
- preserve existing deterministic tests;
- no ledger or WebGL dependencies.

Performance:

- avoid rebuilding all frame structures from scratch where a delta is sufficient;
- support materializing requested step range only.
