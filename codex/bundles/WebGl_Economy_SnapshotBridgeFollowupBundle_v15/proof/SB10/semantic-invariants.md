# Semantic invariants SB10

Status: Completed.

## Snapshot store durability invariant

Snapshots saved through `FileSimulationSnapshotStore` are durable JSON payloads that can be listed by descriptor, loaded by snapshot id, deleted by snapshot id, and rejected when the serialized payload no longer matches its embedded content hash.

## Query/index invariant

Descriptor indexes expose stable filters for run id, scenario id, and step index. `SimulationSnapshotStoreTests.FileSnapshotStore_SavesIndexesLoadsDeletesAndDetectsTampering` saves 100 snapshots, lists all 100, lists the 50 matching run id `run.bulk.a`, and finds the single scenario/step descriptor for step 42.

## Hash-validation invariant

Every in-memory and file-backed load path uses `SimulationRunSnapshotSerializer.Deserialize(..., validateHash: true)`. The SB10 tamper test edits the stored JSON hash and asserts that a later load throws, proving corrupt snapshot payloads are not silently accepted.

## Extension invariant

`ISimulationSnapshotPayloadCodec` isolates the payload encoding contract from storage, so future compression can be plugged in without changing the descriptor indexing or content-hash validation semantics.

## Completed Validator Tokens

Invariant ID: SB10-snapshot-store-hardening

Shallow-pass trap: SB10 rejects an in-memory-only store as durable snapshot proof.

Adversarial negative proof: failing-first snapshot-store hardening evidence records the missing async/file-backed store contract before implementation.

Semantic positive proof: focused snapshot store tests save, list, query, load, delete, and tamper-check 100 file-backed snapshots.

Anti-stub audit: SB10 source assertions document the scan results and expected missing-id default behavior.
