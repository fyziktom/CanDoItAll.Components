# SB07 — Snapshot Analysis And Diff Completion

## Goal

Make snapshot analysis reusable outside tests.

## Required actions

- Promote analysis logic into production services if any remains test-local.
- Add generic analysis facets:
  - active issues
  - admin/work burden
  - resource concentration
  - resource shortage/surplus
  - relationship stress
  - pending event pressure
  - visual stage pressure
- Ensure `SimulationSnapshotDiff` compares relationships and visual state in addition to actors/stores/flows/issues/events/metrics.
- Add file-backed snapshot export/read/list/delete tests.
- Add snapshot analysis artifact generation used by the real probes.

## Acceptance

A snapshot at a problematic step must explain the state without relying on browser internals.
