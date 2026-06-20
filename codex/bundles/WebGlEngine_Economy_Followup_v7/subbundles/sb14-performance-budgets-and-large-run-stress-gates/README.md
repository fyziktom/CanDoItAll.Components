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

## Closure

Status: completed

- Components now exposes generic `Small()`, `Medium()`, `Large()`, and `Stress()` runtime budget profiles while preserving existing scene-count profiles and keeping Components free of Economy-domain semantics.
- Economy now evaluates named performance profiles across materialization, projection, metric evaluation, snapshot build, serialization, browser load, and batch settle timing.
- Headless budget failures are readiness-affecting comparability failures through a separate `performance-budget` gate; they do not appear as scenario, simulation, or metric economic-model failures.
- Large-run proof, browser idle/settled timing, source assertions, anti-stub audit, changed-file hashes, and bundle validator output are recorded under `proof/SB14/`.
