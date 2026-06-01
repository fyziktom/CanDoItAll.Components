# SB07 Source Assertions

- Snapshot builder now emits separate `snapshot.data`, `snapshot.visualState`, and `snapshot.full` provenance hashes.
- Snapshot diff compares relationships, visual state, provenance hashes, and metadata in addition to frame stores, flows, issues, metrics, invariants, and events.
- Runtime diagnostics do not perturb the stable data hash, while full snapshot hash still changes when visual/runtime state changes.

