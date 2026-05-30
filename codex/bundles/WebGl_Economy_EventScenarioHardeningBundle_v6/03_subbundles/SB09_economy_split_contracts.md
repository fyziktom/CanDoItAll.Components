# SB09 - Economy: split SimulationContracts.cs

Current risk: `SimulationContracts.cs` is too large and mixes multiple responsibilities.

Split into files under `Simulation.Abstractions`:

- `SimulationScenarioManifest.cs`
- `SimulationRunIdentity.cs`
- `SimulationClock.cs`
- `SimulationFrame.cs`
- `SimulationFrameDelta.cs`
- `SimulationActors.cs`
- `SimulationResources.cs`
- `SimulationEvents.cs` (new)
- `SimulationBackendContracts.cs`
- `SimulationDeterministicHash.cs`

No behavior changes in this subbundle except adding new event DTOs.

Validation:

- all existing `SimulationPreparationTests` pass;
- public namespace remains `CanDoItAll.Economy.Simulation.Abstractions`.
