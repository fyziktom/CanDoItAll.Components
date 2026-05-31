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

## Status
- Completed.

## Prerequisites
- SB01 branch and boundary baseline is complete.

## Exact Source References
- `C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.Abstractions\Snapshot\SimulationRunSnapshot.cs`
- `C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.Abstractions\Snapshot\SimulationRunSnapshotSerializer.cs`
- `C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\SimulationSnapshotTests.cs`

## Dependency Impact
- Critical foundation for SB09-SB10, SB13-SB14, and SB12 snapshot pipeline.

## Validation Depth
- Requires production builder service proof, shared-resource and finite-resource positive cases, runtime-diagnostics hash-stability negative case, and anti-stub audit.

## Acceptance Checklist
- Builder contracts exist in production code.
- Builder accepts data, visual, and runtime inputs without requiring WebGL.
- Data hash is stable when only runtime diagnostics change.

## Proof Required
- `bundle://proof/SB08/manifest.md`
- `bundle://proof/SB08/semantic-invariants.md`
- Economy test transcript and source assertions.

## Browser Validation Logging
- Browser validation is not required for snapshot builder services.

## Progression Gate
- SB09, SB10, SB12, SB13, and SB14 may proceed after reusable builder behavior is proven.

## Suggested Agent Prompt
- Promote snapshot creation into production builder services and prove deterministic hash behavior across shared and finite resource probes.
