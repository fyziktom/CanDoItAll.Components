# SB08 — Behavior expansion profiles

## Purpose

Make implicit event expansion explicit, versioned and inspectable.

## Scope

Repositories:

- `fyziktom/CanDoItAll.Components` / branch `webgl-engine`
- `fyziktom/CanDoItAll.Economy` / branch `main`

## Implementation tasks

- Add expansion profile IDs such as none, simple-need, trade-policy-v1.
- Scenario manifest must declare expansion profile.
- Event stream must carry expansion provenance for each generated event.
- Add tests comparing expansion disabled vs enabled.

## Required proof

- Scenario without required expansion profile fails strict mode.
- Expansion provenance includes parent event id and profile id.
- Behavior expansion can be disabled for pure event-stream experiments.

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
