# Simulation Snapshot Gap

## User scenario

During a run, the user notices a bad outcome:
"Half of the village is sitting and filling out paperwork."

The user pauses the simulation and wants a snapshot that captures:
- current simulation step/time,
- current actors,
- current resource stores,
- current relationships,
- current issues,
- current events already applied,
- rules/policies currently active,
- metrics/invariants at that point,
- visual frame / WebGL object mapping,
- pending visual actions/motions/stages,
- provenance from input pack hashes,
- deterministic hash of the snapshot.

## Current assessment

The repositories have:
- simulation frames/deltas,
- WebGL run document/timeline/frame/stage,
- WebGL scene document export/import concepts,
- experiment input pack hashing,
- bridge metadata.

But a first-class `SimulationRunSnapshot` is not yet apparent.

## Missing model

Add a generic Economy-side snapshot model, not a WebGL-only snapshot:

`CanDoItAll.Economy.Simulation.Abstractions/Snapshot/`

Suggested contracts:
- `SimulationRunSnapshot`
- `SimulationRunSnapshotState`
- `SimulationRunSnapshotStore`
- `SimulationRunSnapshotSerializer`
- `SimulationRunSnapshotDiff`
- `SimulationSnapshotAnalysisRequest`
- `SimulationSnapshotMetricEvaluation`

Snapshot should be simulation-first and renderer-agnostic.
The WebGL state can be attached as an optional diagnostic/visual snapshot reference, not as the canonical state.

## Why this matters

Without snapshots, we can visualize a run but cannot pause, inspect, interpret, compare, or reproduce the exact state that led to a visible bad behavior.
