# SB09 — Economy shared simulation abstractions

## Goal

Create the shared layer used by both simple-account and ledger-backed simulation engines.

## Project

Create:

`src/CanDoItAll.Economy.Simulation.Abstractions`

References:
- `CanDoItAll.Economy.Core` only if needed.

## Contracts

Add:

- `SimulationScenarioManifest`
- `SimulationRunIdentity`
- `SimulationClockState`
- `SimulationStepIdentity`
- `SimulationFrame`
- `SimulationFrameDelta`
- `SimulationActor`
- `SimulationResourceStore`
- `SimulationResourceFlow`
- `SimulationRelationship`
- `SimulationRuleRef`
- `SimulationIssue`
- `SimulationArtifactRef`
- `SimulationBackendCapabilities`
- `ISimulationBackend`
- `ISimulationFrameProjector`

## Determinism

Add deterministic hash helper for:
- manifest;
- frame;
- frame delta.

No UI playback speed in output hash.

## Validation

- unit tests for deterministic hash stability;
- dependency scan passes.
