# SB12 — Statistical experiment harness and noise isolation

## Repository scope

Economy

## Goal

Support robust economic investigation with repeated runs, factor sweeps, and seed matrices.

## Tasks

- Add experiment design matrix model: factors, levels, seeds, repetitions.
- Support deterministic and stochastic runs separately.
- Compute variance/summary metrics over repeated runs.
- Detect non-determinism in deterministic mode.
- Label stochastic conclusions with confidence/variance metadata.

## Acceptance criteria

- A deterministic run repeated N times has identical hash chains.
- A stochastic run records seeds and variance.
- Scenario comparisons can be made without hidden config drift.

## Required proof artifacts

- `proof/SB12/transcripts/experiment-design-harness-tests.txt`
- `proof/SB12/artifacts/design-matrix-summary.json`

## Gate

Scenario comparison requires explicit design matrix or single-run exploratory label.

## Implementation notes

- Prefer failing-first tests before changing implementation.
- Keep Components domain-neutral.
- Keep Economy economic semantics outside Components.
- Do not close this subbundle with screenshots only.
- Update traceability and proof manifest before moving on.
