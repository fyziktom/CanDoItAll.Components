# SB08 — Snapshot analyzer and diff hardening

## Goal

Promote snapshot analysis from test-local code into reusable production services.

## Required additions

- `ISimulationSnapshotBuilder`
- `ISimulationSnapshotAnalyzer`
- `ISimulationSnapshotAnalysisFacet`
- `FileSimulationSnapshotStore`
- snapshot descriptor with hashes
- `SimulationSnapshotDiff` support for relationships and visual state
- separate:
  - `DataHash`
  - `VisualStateHash`
  - `FullSnapshotHash`

## Generic analysis facets

- store/resource summary;
- active issues;
- admin/workload pressure;
- relationship stress;
- top holder concentration;
- pending events;
- active/pending visual stages.

Do not hardcode water/well/farmer/land terms in generic analyzers. Metrics and resource ids come from input definitions.

## Closure proof

- snapshot roundtrip test;
- snapshot diff test showing relationship + visual-state difference;
- analysis test answering a generic “why does this look bad?” question.
