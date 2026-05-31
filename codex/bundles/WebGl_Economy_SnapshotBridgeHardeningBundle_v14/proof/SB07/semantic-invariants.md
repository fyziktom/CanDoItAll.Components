# SB07 Semantic Invariants

## Invariant SB07-SNAPSHOT-001

Raw notes:
- RN-007: "Introduce simulation snapshots with run identity, frame, delta, events, metrics, invariant evaluations, provenance hashes, optional visual state, deterministic hash, and JSON serializer."
- RN-011: "Snapshots must be renderer-neutral and cannot introduce Components/WebGL coupling into simulation abstractions."

Expected behavior:
- A snapshot can be produced, hashed, serialized, deserialized, and validated using only `Simulation.Abstractions`.
- The deterministic snapshot hash is based on canonical frame and delta content, not stale nested `DeterministicHash` strings.
- Optional visual state is named in renderer-neutral playback terms and is omitted from renderer-neutral snapshots.
- Hash validation rejects tampered serialized content.

Shallow-pass trap:
- A serializer-only snapshot can appear valid if it preserves a top-level hash while the hash function trusts stale nested frame or delta hashes. That would allow frame content tampering to pass validation.

Adversarial negative proof:
- `SnapshotSerializer_RoundTripsRendererNeutralSnapshotAndValidatesContentHash` mutates serialized resource quantity while preserving existing nested hash fields. `Deserialize(validateHash: true)` rejects the tampered JSON.
- The same test mutates a restored frame in memory and asserts `SimulationRunSnapshotHasher.CalculateHash` changes even when nested frame hash data remains stale.
- Transcript: `bundle://proof/SB07/transcripts/simulation-snapshot-tests.txt`.

Semantic positive proof:
- `SnapshotSerializer_RoundTripsRendererNeutralSnapshotAndValidatesContentHash` proves renderer-neutral snapshots serialize without `visualState`, round-trip successfully, and preserve deterministic hashes.
- `SnapshotHash_IncludesOptionalVisualStateWithoutMakingItRequired` proves optional visual playback state participates in the hash while a no-visual-state snapshot remains valid.
- `bundle://proof/SB07/transcripts/simulation-boundary-audit.txt` proves simulation abstraction code remains independent of Components/WebGL coupling.

Anti-stub audit:
- `bundle://proof/SB07/source-assertions/anti-stub-scan.txt` shows no production/test TODO, NotImplemented, placeholder, fake, stub, empty-return, or null-return matches in SB07 scope.

Changed source files:
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Snapshot/SimulationRunSnapshot.cs`.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Snapshot/SimulationRunSnapshotSerializer.cs`.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Hashing/SimulationDeterministicHash.cs`.
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.Abstractions/Hashing/SimulationDeterministicHash.Snapshot.cs`.
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationSnapshotTests.cs`.

Downstream dependency check:
- SB08, SB09, SB12, and SB14 can rely on stable renderer-neutral snapshot contracts and content-based snapshot hash validation.
