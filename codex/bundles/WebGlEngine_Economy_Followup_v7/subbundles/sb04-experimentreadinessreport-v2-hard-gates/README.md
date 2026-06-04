# SB04 — ExperimentReadinessReport v2 hard gates

## Repository scope

Economy

## Goal

Convert readiness reporting from advisory text into machine-readable, hard-gated experiment status.

## Tasks

- Add explicit statuses: engineering-demo, exploratory, headless-valid, oracle-valid, browser-observer-valid, research-ready, not-comparable, failed.
- Separate missing browser observer proof from missing economic correctness proof.
- Add per-gate pass/fail/warning counts.
- Report warnings budget and whether warnings are allowlisted.
- Emit a top-level `researchReady` boolean that is false unless all hard gates pass.

## Acceptance criteria

- Current scenarios produce a status that honestly reflects readiness.
- If browser proof is missing, report can still say headless-valid but not browser-observer-valid.
- No warning-only condition can result in research-ready.

## Required proof artifacts

- `proof/SB04/transcripts/readiness-v2-tests.txt`
- `proof/SB04/artifacts/readiness-v2-sample.json`

## Gate

Readiness report must be useful for deciding whether an experiment can support claims.

## Implementation notes

- Prefer failing-first tests before changing implementation.
- Keep Components domain-neutral.
- Keep Economy economic semantics outside Components.
- Do not close this subbundle with screenshots only.
- Update traceability and proof manifest before moving on.
