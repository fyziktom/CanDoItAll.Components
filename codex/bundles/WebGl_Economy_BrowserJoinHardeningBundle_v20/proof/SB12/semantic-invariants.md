# SB12 Semantic Invariants

## Scope

- Performance proof is large-screen/desktop only.
- The gate measures moderate scenario sizes and does not imply mobile readiness.

## Metrics

- Record counts for actors, resource stores, visual actions, stage barriers, and snapshots.
- Record elapsed time for projection, export, and snapshot serialization.
- Record run document size and total exported artifact size.

## Thresholds

- Elapsed-time and size thresholds are warning-only in SB12.
- Correctness, deterministic hashes, and bounded-count assertions remain hard gates.
