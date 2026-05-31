# SB11 — Metric and invariant pipeline

## Goal
Make experiment outcomes interpretable, not just visually interesting.

## Tasks
- Load expected invariants from input pack.
- Evaluate metrics from frames/deltas.
- Produce `SimulationExperimentResult` with:
  - input pack hash
  - frame hashes
  - metric results
  - invariant results
  - warnings/errors
- Support generic metrics:
  - top owner share by resource
  - concentration index
  - resource access fairness
  - total resource stock
  - rule violation count
  - admin burden
  - relationship/trust/conflict summary

## Tests
- shared-well readiness metrics run without UI.
- farmer-land concentration metrics run without scenario-specific code.
