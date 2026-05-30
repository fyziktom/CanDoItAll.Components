# SB08 - Economy Simulation.Abstractions file refactor

## Problem

`SimulationContracts.cs` is too large and mixes many responsibilities.

## Required split

```text
Identity/
  SimulationRunIdentity.cs
  SimulationStepIdentity.cs
  SimulationClockState.cs

Scenario/
  SimulationScenarioManifest.cs
  SimulationScenarioDefinition.cs
  SimulationRuleRef.cs
  SimulationArtifactRef.cs

Frame/
  SimulationFrame.cs
  SimulationFrameDelta.cs
  SimulationActor.cs
  SimulationResourceStore.cs
  SimulationResourceFlow.cs
  SimulationRelationship.cs
  SimulationIssue.cs

Events/
  SimulationEvent.cs
  SimulationEventKind.cs
  SimulationEventStream.cs
  SimulationEventTarget.cs

Backend/
  ISimulationBackend.cs
  ISimulationFrameProjector.cs
  SimulationBackendCapabilities.cs

Hashing/
  SimulationDeterministicHash.cs

Validation/
  SimulationScenarioValidationResult.cs
  SimulationScenarioValidator.cs
```

## Rules

- `Simulation.Abstractions` must keep zero project references.
- Do not introduce WebGL, Components, Ledger, SDK, or BusinessObjects references.
- Keep deterministic hashing stable or document the intentional change.
