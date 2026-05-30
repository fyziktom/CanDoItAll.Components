# SB09 - Economy scenario definition loader

## Goal

Move from hardcoded materialized frames to loadable scenario definitions.

## Required contracts

Add to `Simulation.Abstractions`:

```text
SimulationScenarioDefinition
SimulationScenarioActorDefinition
SimulationScenarioResourceDefinition
SimulationScenarioLocationDefinition
SimulationScenarioObjectDefinition
SimulationScenarioRuleDefinition
SimulationScenarioEventDefinition
SimulationScenarioSchedule
SimulationScenarioLoader
SimulationScenarioDefinitionValidator
```

## Required behavior

- Load from JSON string/file.
- Validate:
  - duplicate ids
  - dangling actor/resource/location/object ids
  - invalid negative capacities
  - invalid event time/step
  - unknown event kinds
  - missing required target for event kind
- Normalize deterministic ordering.
- Compute deterministic hash independent of formatting and non-semantic UI metadata.

## Scenario examples

Add JSON definitions for:
- shared well community
- small entrepreneur community

Do not remove current hardcoded factories until the loader path is validated.
