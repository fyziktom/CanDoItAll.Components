# SB09 — Scenario pack hash hardening

## Purpose

Make scenario packs tamper-evident at per-file level.

## Scope

Repositories:

- `fyziktom/CanDoItAll.Components` / branch `webgl-engine`
- `fyziktom/CanDoItAll.Economy` / branch `main`

## Implementation tasks

- Extend manifest with per-file sha256 hashes and declared pack hash.
- Verify every declared file hash and detect extra files based on manifest policy.
- Add migration helper to generate/update manifests.
- Add tamper tests for changed file, missing file, extra file and stale pack hash.

## Required proof

- Tampered scenario pack fails.
- Valid scenario pack passes with exact hash.
- Pack hash is included in all run/readiness artifacts.

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
