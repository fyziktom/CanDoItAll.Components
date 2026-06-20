# SB14 — Final cross-repo red-team closure

## Purpose

Prove the system can run meaningful economic experiments without simulator noise.

## Scope

Repositories:

- `fyziktom/CanDoItAll.Components` / branch `webgl-engine`
- `fyziktom/CanDoItAll.Economy` / branch `main`

## Implementation tasks

- Run all Components WebGlLib/WebGlRunLib tests.
- Run all Economy simulation/sandbox/WebGlBridge tests.
- Run golden oracle suite and headless experiment runner.
- Run browser pause/idle proof.
- Run package-mode proof with isolated caches.
- Review all proof artifacts for emptiness and semantic adequacy.

## Required proof

- Final report closes R01-R14.
- No critical known simulator-noise risk remains unclassified.
- Remaining risks are documented as backlog, not hidden assumptions.

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
