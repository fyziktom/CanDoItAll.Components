# SB14 — Performance budgets and large-run stress gates

## Repository scope

Both

## Goal

Make performance results actionable and prevent slow/oversized runs from being misclassified as ready.

## Tasks

- Convert warning-only thresholds into readiness-affecting budgets for research mode.
- Add configurable performance profiles: small, medium, large, stress.
- Track materialization, projection, metric evaluation, snapshot build, serialization, browser load, batch settle time, memory deltas.
- Fail or mark not-comparable when budgets are exceeded.
- Add profiler-friendly logs and top bottleneck summary.

## Acceptance criteria

- Large run outputs include hard budget status.
- Budget failures do not look like economic-model failures.
- Performance proof includes browser idle/settled timing where browser is involved.

## Required proof artifacts

- `proof/SB14/transcripts/performance-budget-tests.txt`
- `proof/SB14/artifacts/performance-budget-report.json`

## Gate

Performance noise must be classified before interpreting economics.

## Implementation notes

- Prefer failing-first tests before changing implementation.
- Keep Components domain-neutral.
- Keep Economy economic semantics outside Components.
- Do not close this subbundle with screenshots only.
- Update traceability and proof manifest before moving on.
