# SB08 Semantic Invariants

## Invariant SB08-SNAPSHOT-STORE-001

Raw notes:
- RN-008: "Support pause/export/analyze by saving simulation snapshots, exporting/importing JSON, diffing snapshots, and evaluating metrics/invariants from snapshot data."
- RN-011: "Snapshot persistence must not create renderer or backend coupling in simulation abstractions."

Expected behavior:
- A materialized run can be paused at step 2 and converted into a full snapshot with frame, delta, applied/pending events, metrics, invariants, and provenance hashes.
- JSON export/import preserves the deterministic snapshot hash.
- In-memory persistence stores canonical serialized snapshots and returns validated copies, not shared mutable instances.
- Metrics and invariants can be evaluated from the snapshot frame after import.
- Diffs expose meaningful frame/store changes instead of relying only on top-level hash drift.

Shallow-pass trap:
- A store can appear to work by keeping the same object reference in memory. That would not prove JSON export/import, hash validation, or analysis survival after a pause/export boundary.

Adversarial negative proof:
- `SnapshotStore_ExportsRoundTripsDiffsAndSupportsAnalysisFromStepTwoSnapshot` modifies an imported step-2 snapshot resource store, refreshes its hash, and asserts `SimulationSnapshotDiff.Compare` reports both `frameHash` and the changed store path.
- The same test re-reads the snapshot through `InMemorySimulationSnapshotStore`, proving the stored copy came through canonical JSON validation rather than object identity.
- Transcript: `bundle://proof/SB08/transcripts/simulation-snapshot-store-tests.txt`.

Semantic positive proof:
- The test materializes a real `SimpleSimulationStateTransitionEngine` run, creates a step-2 snapshot, exports/imports JSON, stores/loads it, compares deterministic hashes, and evaluates `SimulationFrameMetricEvaluator` plus `SimulationInvariantEvaluator` from the imported snapshot frame.
- `bundle://proof/SB08/transcripts/simulation-boundary-audit.txt` proves the store/export/diff implementation remains inside simulation abstraction boundaries.

Anti-stub audit:
- `bundle://proof/SB08/source-assertions/anti-stub-scan.txt` shows no production/test TODO, NotImplemented, placeholder, fake, stub, empty-return, or null-return matches in SB08 scope.

Changed source files:
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Snapshot/SimulationSnapshotStore.cs`.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Snapshot/SimulationSnapshotJsonExport.cs`.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Snapshot/SimulationSnapshotDiff.cs`.
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationSnapshotStoreTests.cs`.

Downstream dependency check:
- SB09, SB13, and SB14 can rely on snapshot persistence/export round-trips preserving deterministic hash and analysis data.
