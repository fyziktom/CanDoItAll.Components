# SB07 - Snapshot builder, analyzer and diff hardening

## Goal
Make simulation snapshots production-grade analysis artifacts.

## Required actions

1. Add `ISimulationRunSnapshotBuilder`.
2. Add `SimulationRunSnapshotBuildRequest` with:
   - selected frame,
   - previous delta,
   - applied events,
   - pending events,
   - metrics,
   - invariants,
   - optional visual state input.
3. Add `ISimulationSnapshotAnalyzer` and generic analyzers:
   - admin burden,
   - issue pressure,
   - store/resource concentration,
   - relationship stress,
   - pending-event pressure,
   - visual-stage pressure.
4. Extend snapshot diff to include:
   - relationships,
   - visual state,
   - provenance hashes,
   - metadata changes.
5. Split hashes into:
   - data hash,
   - visual state hash,
   - full snapshot hash.

## Acceptance criteria

- Snapshot analysis code is reusable, not test-local.
- Snapshot diff catches relationship and visual state changes.
- Snapshot export/import roundtrip validates hash.
