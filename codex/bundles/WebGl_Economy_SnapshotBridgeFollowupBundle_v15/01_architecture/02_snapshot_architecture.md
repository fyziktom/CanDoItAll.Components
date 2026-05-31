# Snapshot architecture

## Purpose

A snapshot is the answer to:

> I paused the simulation because the situation looks bad. Give me the exact data state, visual state, pending actions, metrics, invariants, and provenance so I can analyze why.

## Snapshot layers

### Data snapshot

This is canonical and should be usable without WebGL.

Includes:

- run id
- scenario id
- step index
- simulated time
- current frame
- last delta
- applied events
- pending events
- metrics
- invariants
- provenance hashes
- deterministic data hash

### Visual attachment

Optional. This connects the data state to visualization without making WebGL required.

Includes:

- visual frame id
- playback frame id/index
- node-object id mapping
- active/pending action ids
- active/pending stage ids
- runtime diagnostics
- visual attachment hash

### Runtime attachment

Optional and transient.

Includes:

- WebGL runtime diagnostics
- queue depths
- current stage id
- pending command stage count
- active motion count
- object positions if exported from runtime

## Required hardening

1. Add separate hashes:
   - `DataHash`
   - `VisualStateHash`
   - `FullSnapshotHash`
2. Add `ISimulationSnapshotBuilder`.
3. Add `ISimulationSnapshotAnalyzer`.
4. Add `SimulationSnapshotAnalysisReport`.
5. Add optional file-based snapshot store.
6. Add snapshot diff for visual state and provenance.
7. Add tests for tamper detection and pure-data hash stability when runtime diagnostics change.
