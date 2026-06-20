# SB11 — Performance budgets and noise isolation

## Purpose

Separate model performance from visualization performance and establish hard budgets.

## Scope

Repositories:

- `fyziktom/CanDoItAll.Components` / branch `webgl-engine`
- `fyziktom/CanDoItAll.Economy` / branch `main`

## Implementation tasks

- Add deterministic headless budgets as hard gates.
- Keep browser/WebGL budgets separate and label them visual-only.
- Measure CPU time, allocations, frame count, event count, object count and artifact size.
- Add regression baseline files with explicit update workflow.

## Required proof

- Headless budgets fail on intentional O(n^2) probe.
- Visual budgets report warnings without invalidating economic model.
- Budget report is included in readiness output.

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
