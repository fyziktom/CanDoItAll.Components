# SB10 - Economy scenario definition loading

Repository: `CanDoItAll.Economy`

## Goal

Move beyond hardcoded frames toward loadable scenario definitions.

## Add to Simulation.Abstractions

```text
SimulationScenarioDefinition
SimulationScenarioActorDefinition
SimulationScenarioResourceDefinition
SimulationScenarioLocationDefinition
SimulationScenarioStoreDefinition
SimulationScenarioRelationshipDefinition
SimulationScheduledEventDefinition
SimulationScenarioBackendHint
SimulationScenarioDefinitionValidationResult
ISimulationScenarioDefinitionStore
ISimulationScenarioDefinitionSerializer
ISimulationScenarioDefinitionValidator
```

## Requirements

- No WebGL references.
- Locations are generic coordinates, not renderer coordinates.
- Scenario definition can represent:
  - shared-well community;
  - small entrepreneur community;
  - future ledger-backed scenario.
- Deterministic hash excludes UI playback speed.
- Validation catches duplicate IDs, missing actor/resource/location references, invalid step indexes, and negative capacities unless explicitly allowed.

## Proof

Serialize and deserialize the shared-well and entrepreneur scenario definitions.
