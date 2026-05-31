# Snapshot analysis architecture

## Problem
The user needs to pause a run, create a snapshot, and answer questions such as:

> Why does this visual state look bad? Half of the village appears to be doing paperwork.

That requires a snapshot that is not only a WebGL screenshot. It must be a data snapshot with optional visual state attached.

## Required contracts

```text
SimulationRunSnapshot
  Data snapshot: frame, delta, applied/pending events, metrics, invariants, provenance hashes.

SimulationSnapshotVisualState
  Optional visual attachment: visual frame id, playback frame id, node/object mapping, active/pending actions/stages, runtime diagnostics.

ISimulationSnapshotBuilder
  Builds a snapshot from a simulation run state, selected frame and optional visual/run document state.

ISimulationSnapshotAnalyzer
  Produces analysis facts from a snapshot: admin burden, issue pressure, concentration, scarcity, relationship stress, visual stage pressure.

ISimulationSnapshotStore
  Stores descriptors and full snapshots.

ISimulationSnapshotDiff
  Compares snapshots, including relationships and visual state.
```

## Required hash model

Keep separate hashes:

- `DataHash`: frame + delta + events + stores + relationships + issues + metrics + invariants.
- `VisualStateHash`: node/object mapping + active/pending actions/stages + runtime diagnostics.
- `FullSnapshotHash`: data + visual + provenance + metadata.

This avoids false changes when only the visual attachment changes and allows data-only reproducibility checks.
