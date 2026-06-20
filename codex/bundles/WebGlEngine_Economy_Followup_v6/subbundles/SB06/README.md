# SB06 — Golden economic oracle suite

## Purpose

Create known-answer scenarios that prove model primitives before experiments are trusted.

## Scope

Repositories:

- `fyziktom/CanDoItAll.Components` / branch `webgl-engine`
- `fyziktom/CanDoItAll.Economy` / branch `main`

## Implementation tasks

- Add small scenarios with exact expected frame-by-frame stores, flows, issues and metrics.
- Assert deterministic frame hashes for each golden scenario.
- Include negative tests for rejected flows, missing references and ambiguity.
- Run oracles headlessly, without WebGL.

## Required proof

- At least 10 golden scenarios pass.
- Each golden scenario has expected final stores and flow counts.
- At least 4 negative oracle scenarios fail for the expected reason.

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
