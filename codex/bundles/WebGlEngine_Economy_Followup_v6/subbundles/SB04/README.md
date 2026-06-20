# SB04 — Strict experiment mode

## Purpose

Prevent warning-level semantic issues from contaminating economic conclusions.

## Scope

Repositories:

- `fyziktom/CanDoItAll.Components` / branch `webgl-engine`
- `fyziktom/CanDoItAll.Economy` / branch `main`

## Implementation tasks

- Add `SimulationExperimentMode` and strict options to sandbox/runner.
- Map warning categories to fail/pass/allowed lists.
- Make unknown events, missing references, insufficient stock and rejected flows hard errors in strict mode.
- Expose mode in scenario manifest and readiness report.

## Required proof

- Strict mode rejects a scenario with unknown event kind.
- Strict mode rejects insufficient stock unless explicitly allowed.
- Exploratory mode still allows existing demos with warnings.

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
