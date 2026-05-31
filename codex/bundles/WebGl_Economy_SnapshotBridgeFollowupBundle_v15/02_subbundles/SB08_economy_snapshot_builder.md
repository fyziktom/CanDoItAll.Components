# SB08 — Economy snapshot builder service

## Goal
Move snapshot creation out of tests into production reusable services.

## Required contracts
- `ISimulationSnapshotBuilder`
- `SimulationSnapshotBuildRequest`
- `SimulationSnapshotBuildResult`
- `SimulationSnapshotProvenanceBuilder`

## Inputs
- scenario
- run/scenario result
- current frame
- last delta
- pending events
- metrics/invariants
- optional visual frame
- optional WebGlRunDocument and runtime diagnostics

## Output
A `SimulationRunSnapshot` with deterministic hashes and warnings/errors.

## Validation
- Build snapshot at a shared-resource step.
- Build snapshot at a finite-resource market step.
- Verify data hash remains stable if runtime diagnostics change.
