# SB14 - Performance and scalability proofs

## Goal
Find bottlenecks before demo work.

## Required probes

- 100 actors.
- 20 shared/spatial resources.
- 500 simulation events.
- 1000 visual actions.
- 1000 WebGL stages.
- 200 snapshots or snapshot diffs.

## Watch points

- Event normalization cost.
- Store lookup and mutation cost.
- Visual action normalization and nested sequence flattening.
- Bridge mapping cost.
- Snapshot serialization and diff cost.
- JS runtime stage queue and motion queue overhead.

## Acceptance criteria

- Produce JSON performance report.
- Add regression thresholds but avoid brittle machine-specific exact timings.
