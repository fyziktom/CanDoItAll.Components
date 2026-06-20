# SB10 — Experiment readiness report and failure classification

## Purpose

Create the single gate that decides whether an economic result is usable.

## Scope

Repositories:

- `fyziktom/CanDoItAll.Components` / branch `webgl-engine`
- `fyziktom/CanDoItAll.Economy` / branch `main`

## Implementation tasks

- Implement `EconomyExperimentReadinessReport`.
- Classify failures into scenario, simulation, metric, projection, runtime, UI and performance.
- Add confidence levels L0-L5.
- Ensure runtime/browser failures do not masquerade as economic-model failures.

## Required proof

- A headless strict run can be L4 without browser.
- A browser failure degrades runtime band only.
- A semantic warning prevents L4/L5 confidence.

## Refactor gate

Before closing this subbundle, Codex must add a short self-review covering:

- API compatibility,
- generic/domain boundary,
- deterministic behavior,
- performance risk,
- proof adequacy,
- remaining open risks.

## Stop conditions

Do not continue to the next subbundle if a critical proof is browser-screenshot-only, placeholder-only, warning-only where a hard gate is required, or not tied to a source invariant.
