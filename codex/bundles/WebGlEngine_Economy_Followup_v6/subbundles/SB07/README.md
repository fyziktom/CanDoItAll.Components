# SB07 — Metric and invariant registry

## Purpose

Stop metrics and invariants from silently returning zero or fallback semantics.

## Scope

Repositories:

- `fyziktom/CanDoItAll.Components` / branch `webgl-engine`
- `fyziktom/CanDoItAll.Economy` / branch `main`

## Implementation tasks

- Create typed metric/invariant definitions or a registry with known kinds.
- Unknown metric kind must be an error in strict/research-grade mode.
- Unknown invariant kind must be an error unless explicitly mapped.
- Missing metric IDs must fail validation, not evaluate to zero.

## Required proof

- Unknown metric test fails before run.
- Unknown invariant test fails before run.
- Existing intended metrics still evaluate with explicit kinds.

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
