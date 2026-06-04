# SB15 — Final cross-repo red-team closure

## Repository scope

Both

## Goal

Validate that the simulator can separate economic failures from infrastructure/runtime/projection bugs.

## Tasks

- Run all focused Components and Economy tests.
- Run golden oracles, readiness report, headless runner, deterministic repeat tests, performance budgets, and browser pause/idle proof.
- Run anti-stub/proof-integrity scans.
- Red-team at least five failure modes: unknown event, ambiguous store, unknown metric, browser non-idle, broken scenario hash.
- Produce final readiness verdict.

## Acceptance criteria

- Every red-team failure is classified correctly.
- No simulator-noise failure is reported as an economic model conclusion.
- Final report states exactly what can and cannot be claimed.

## Required proof artifacts

- `proof/SB15/final-red-team-report.md`
- `proof/SB15/transcripts/final-cross-repo-tests.txt`
- `proof/SB15/artifacts/final-readiness-summary.json`

## Gate

Bundle complete only when research-readiness claims are honest and evidence-backed.

## Implementation notes

- Prefer failing-first tests before changing implementation.
- Keep Components domain-neutral.
- Keep Economy economic semantics outside Components.
- Do not close this subbundle with screenshots only.
- Update traceability and proof manifest before moving on.
