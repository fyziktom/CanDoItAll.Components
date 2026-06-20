# SB05 — Store resolution policy

## Purpose

Remove hidden economic policy from store lookup heuristics.

## Scope

Repositories:

- `fyziktom/CanDoItAll.Components` / branch `webgl-engine`
- `fyziktom/CanDoItAll.Economy` / branch `main`

## Implementation tasks

- Introduce typed store resolution requests and policies.
- Detect duplicate actor/resource stores and ambiguous shared stores.
- Add explicit policies for actor-owned, location-owned, shared-pool, market-pool and storeId exact resolution.
- Disable heuristic fallback in strict mode.

## Required proof

- Ambiguous store fixture fails in strict mode.
- Same fixture passes when an explicit tie-breaker policy is declared.
- No existing valid scenario regresses without an explanation.

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
