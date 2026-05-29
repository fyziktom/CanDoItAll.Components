# SB09 - Economy refactor large simulation files

Repository: `CanDoItAll.Economy`

## Problem

New simulation files are too broad for the next phase.

## Split targets

### Simulation.Abstractions

Split `SimulationContracts.cs` into:

```text
Scenario/SimulationScenarioManifest.cs
Scenario/SimulationScenarioDefinition.cs
Scenario/SimulationScenarioValidation.cs
Run/SimulationRunIdentity.cs
Run/SimulationClockState.cs
Frame/SimulationFrame.cs
Frame/SimulationFrameDelta.cs
Frame/SimulationEntities.cs
Events/SimulationEvent.cs
Backends/ISimulationBackend.cs
Hashing/SimulationDeterministicHash.cs
```

### Simulation.SimpleAccounts

Split `SimpleSimulation.cs` into:

```text
SimpleAccountModels.cs
SimpleSimulationBackend.cs
SimpleSimulationScenarioFactory.cs
SharedWellCommunityScenarioFactory.cs
SmallEntrepreneurCommunityScenarioFactory.cs
SimpleSimulationDeltaBuilder.cs
```

### Simulation.Visualization

Split `EconomyVisualizationContracts.cs` into:

```text
EconomyVisualFrame.cs
EconomyVisualEntities.cs
EconomyVisualAction.cs
EconomyVisualFrameMapper.cs
EconomyVisualCategoryPolicy.cs
EconomyVisualLayoutStrategy.cs
EconomyVisualSymbolPolicy.cs
```

### Simulation.Ledger

Split `LedgerSimulationAdapter.cs` into:

```text
LedgerScenarioForkDescriptor.cs
LedgerSnapshotSimulationSource.cs
LedgerProjectionFrameProjector.cs
LedgerSimulationBackend.cs
LedgerBusinessObjectEvidenceMapper.cs
LedgerSimulationIssueMapper.cs
```

## Tests

All existing `SimulationPreparationTests` must remain green after refactor.
