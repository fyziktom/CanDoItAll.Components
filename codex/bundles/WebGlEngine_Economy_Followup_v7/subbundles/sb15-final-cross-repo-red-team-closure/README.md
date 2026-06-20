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

## Closure

Status: completed

- Focused Components and Economy final tests passed: 58 WebGlLib tests, 62 WebGlRunLib tests, and 32 Economy hardening/probe/CLI tests.
- Red-team failure modes are classified correctly: unknown event, ambiguous store, unknown metric, browser non-idle, and broken scenario hash all block economic conclusions through the proper simulation, registry, runtime/observer, or comparability path.
- Browser proof was refreshed under SB15 for pause/idle and performance budgets; active runtime work paused to idle, late-drain state remained paused, and browser load/batch settle stayed within budget.
- Final readiness is conditionally research-ready only under strict gates; ungated demo/exploratory runs and over-budget/non-comparable runs are not economic evidence.
