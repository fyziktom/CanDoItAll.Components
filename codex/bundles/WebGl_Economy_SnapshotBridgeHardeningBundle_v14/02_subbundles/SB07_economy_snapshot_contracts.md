# SB07 - Economy simulation snapshot contracts

Goal:
- Add first-class simulation snapshots.

Tasks:
1. Add snapshot models under `Simulation.Abstractions/Snapshot`.
2. Snapshot must include:
   - run identity,
   - scenario id,
   - step index,
   - frame,
   - last delta,
   - applied/pending event refs or events,
   - metrics,
   - invariant evaluations,
   - provenance hashes,
   - optional visual state.
3. Add deterministic hash for snapshot.
4. Add serializer.

Acceptance:
- Snapshot is renderer-neutral.
- Snapshot can be produced without WebGL.

## Status

Completed.

## Prerequisites

SB01 branch/source inventory and baseline Economy state.

## Validation Depth

Add or verify renderer-neutral snapshot contracts, deterministic hashing, serializer behavior, and tests that snapshot creation does not require WebGL references.

## Progression Gate

SB08/SB09/SB12 may proceed only after snapshot contracts are stable, hashable, serializable, and independent of Components/WebGL.
