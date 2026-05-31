# SB10 — Snapshot store hardening

## Goal
Make snapshots useful beyond in-memory tests.

## Required
- async snapshot store interface
- file-backed JSON snapshot store
- descriptor indexes by run/scenario/step
- content hash verification on load
- optional compression extension point
- delete/list/query tests

## Validation
- Save 100 snapshots.
- List by run id.
- Load by snapshot id.
- Detect tampering.

## Status
- Completed.

## Prerequisites
- SB08 snapshot builder service proof is complete.

## Exact Source References
- `C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.Abstractions\Snapshot\SimulationSnapshotStore.cs`
- `C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.Abstractions\Snapshot\SimulationSnapshotJsonExport.cs`
- `C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\SimulationSnapshotStoreTests.cs`

## Dependency Impact
- Critical persistence foundation for pause/export/analyze workflows and performance proof.

## Validation Depth
- Requires async interface, file-backed store, index/query/delete proof, 100-snapshot scale proof, tamper negative proof, and anti-stub audit.

## Acceptance Checklist
- File-backed JSON store is production code.
- Descriptor indexes support run/scenario/step queries.
- Loading verifies content hash and detects tampering.

## Proof Required
- `bundle://proof/SB10/manifest.md`
- `bundle://proof/SB10/semantic-invariants.md`
- Economy test transcript and source assertions.

## Browser Validation Logging
- Browser validation is not required for snapshot store services.

## Progression Gate
- SB15 may rely on snapshot persistence only after store proof is recorded.

## Suggested Agent Prompt
- Harden snapshot persistence with async file-backed storage, descriptor queries, content hash validation, and tamper tests.
