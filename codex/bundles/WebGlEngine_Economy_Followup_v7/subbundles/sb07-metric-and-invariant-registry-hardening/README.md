# SB07 — Metric and invariant registry hardening

## Repository scope

Economy

## Goal

Prevent unknown or misspelled metrics/invariants from silently passing with default values.

## Tasks

- Create explicit metric/invariant registries with schemas.
- Fail unknown metric/invariant kinds in research strict mode.
- Validate required metadata keys and units.
- Add deterministic precision/rounding policy.
- Add oracle tests for resource totals, HHI, top-owner share, access cost, depletion, admin burden, transfer volume.

## Acceptance criteria

- Unknown metric/invariant kinds are errors in research mode.
- Every metric result includes kind, unit, precision, source frame hash, and evaluator version.
- Oracle metric tests pass with exact expected values.

## Required proof artifacts

- `proof/SB07/transcripts/metric-invariant-registry-tests.txt`
- `proof/SB07/artifacts/metric-oracle-results.json`

## Gate

No readiness report may pass if metrics/invariants use fallback behavior.

## Implementation notes

- Prefer failing-first tests before changing implementation.
- Keep Components domain-neutral.
- Keep Economy economic semantics outside Components.
- Do not close this subbundle with screenshots only.
- Update traceability and proof manifest before moving on.
