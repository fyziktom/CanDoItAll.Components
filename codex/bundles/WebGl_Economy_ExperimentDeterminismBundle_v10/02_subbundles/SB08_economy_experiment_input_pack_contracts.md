# SB08 — Economy: experiment input pack contracts

## Problem

Scenario definitions are not enough for reproducible economic experiments.

## Tasks

Add neutral contracts in `Simulation.Abstractions`:

- `SimulationExperimentDefinition`
- `SimulationExperimentInputPack`
- `SimulationInputDocumentRef`
- `SimulationInputHashManifest`
- `SimulationRunPlanDefinition`
- `SimulationTreatmentDefinition`
- `SimulationHypothesisDefinition`
- `SimulationMetricDefinition`
- `SimulationExpectedInvariantDefinition`

Input pack must reference:

- scenario definition
- placement/topology
- parameters
- institution/rules
- run plan
- visual mapping
- expected invariants

## Done criteria

- Experiment pack can be serialized to JSON.
- Deterministic hash includes input document hashes.
- Tests prove playback speed/UI settings are excluded, but placement/parameters/rules are included.
