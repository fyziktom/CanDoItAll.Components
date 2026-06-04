# SB05 — Strict simulation mode as first-class policy

## Repository scope

Economy

## Goal

Make simulator-noise conditions fail explicitly in research mode.

## Tasks

- Add `EconomyExperimentPolicy.ResearchStrict` or equivalent.
- Enable fail-on-unknown-event, fail-on-insufficient-stock, fail-on-missing-reference, fail-on-ambiguous-store, fail-on-unknown-metric, fail-on-unknown-invariant.
- Add warnings budget enforcement.
- Ensure demo/permissive mode remains available but marked not research-ready.
- Thread policy through scenario runner, readiness report, and performance probes.

## Acceptance criteria

- Research strict mode fails when a scenario contains unknown handlers, ambiguous stores, unresolved mappings, or unknown metric/invariant kinds.
- Existing sample scenarios either pass strict mode or report concrete remaining failures.

## Required proof artifacts

- `proof/SB05/transcripts/research-strict-mode-tests.txt`
- `proof/SB05/artifacts/strict-mode-failure-examples.json`

## Gate

Strict mode must be the default for readiness reports used in research claims.

## Implementation notes

- Prefer failing-first tests before changing implementation.
- Keep Components domain-neutral.
- Keep Economy economic semantics outside Components.
- Do not close this subbundle with screenshots only.
- Update traceability and proof manifest before moving on.
