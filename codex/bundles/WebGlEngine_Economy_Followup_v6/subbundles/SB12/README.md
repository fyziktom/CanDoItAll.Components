# SB12 — Headless experiment runner

## Purpose

Allow economic experiments to be run and compared without UI/WebGL.

## Scope

Repositories:

- `fyziktom/CanDoItAll.Components` / branch `webgl-engine`
- `fyziktom/CanDoItAll.Economy` / branch `main`

## Implementation tasks

- Add CLI/tool or service entry point for scenario pack -> run -> readiness report.
- Output JSON artifacts: event stream, frame hashes, metrics, invariants, readiness report.
- Support repeat run comparison and deterministic hash check.
- Support batch execution of multiple scenario packs.

## Required proof

- Two identical runs produce same run hash.
- A modified scenario produces a different pack hash and run hash.
- Batch runner summary marks confidence levels.

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
