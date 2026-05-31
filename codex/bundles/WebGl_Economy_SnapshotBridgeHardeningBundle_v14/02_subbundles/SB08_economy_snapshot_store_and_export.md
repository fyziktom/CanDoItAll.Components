# SB08 - Economy snapshot store and export

Goal:
- Support pause/export/analyze workflow.

Tasks:
1. Add `ISimulationSnapshotStore`.
2. Add in-memory implementation.
3. Add JSON export/import.
4. Add diff helper between snapshots.
5. Add test:
   - materialize a run,
   - create snapshot at step 2,
   - serialize,
   - deserialize,
   - compare deterministic hash,
   - evaluate metrics/invariants from snapshot.

Acceptance:
- User can pause a run and export a full data snapshot for analysis.

## Status

Completed.

## Prerequisites

SB07 snapshot contracts.

## Validation Depth

Add or verify store/export/import/diff tests that materialize a run, snapshot at step 2, JSON roundtrip, preserve deterministic hash, and evaluate metrics/invariants from snapshot data.

## Progression Gate

SB09/SB13/SB14 may proceed only after snapshot persistence and export proof shows pause/export/analyze data can survive a roundtrip without hash drift.
