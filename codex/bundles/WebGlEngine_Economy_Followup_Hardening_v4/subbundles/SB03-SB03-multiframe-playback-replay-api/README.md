# SB03 Multi-frame playback and replay API

## Objective

Implement the next hardening step for the WebGL/Economy engine foundation.

## Scope

- Replace or harden `ApplyAsync(WebGlRunPlaybackResult)` so it cannot silently apply only `CurrentFrame` or `FramesToApply[^1]` for multi-frame playback.
- Add explicit multi-frame browser apply API that applies reset once and then frames in deterministic order.
- Return per-frame results and stop on first failed frame.
- Keep single-frame `ApplyAsync(WebGlRunFrameApplyResult)` intact for direct single-frame use.
- Add docs explaining when to use runner vs adapter.

## Out of scope

- Do not add domain semantics into Components packages.
- Do not rewrite unrelated systems.
- Do not close the subbundle with screenshots only.
- Do not accept empty required proof artifacts.

## Implementation guidance

- Start with a failing-first test or audit where applicable.
- Make the smallest cohesive refactor that fixes the root cause.
- Add source assertions that prove the intended path is used.
- Keep API compatibility where safe; otherwise document the migration.
- Ensure all source-code comments are in English.

## Required proof

- Failing-first test showing old last-frame-only behavior.
- Passing multi-frame adapter/runner test.
- Browser or fake runtime proof with two or more frames applied in order.
- API compatibility note.

## Refactor gate

Before moving to the next subbundle, record:
- changed files;
- test/build/audit commands;
- proof artifact paths;
- open risks;
- whether public API changed and how users migrate.
