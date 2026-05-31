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
