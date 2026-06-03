# SB10 WebGlSceneView external import lifecycle

## Objective

Implement the next hardening step for the WebGL/Economy engine foundation.

## Scope

- Harden `WebGlSceneView.ImportSceneAsync`, `ImportSceneDetailedAsync`, and document import methods so external import updates or invalidates `appliedSceneKey` consistently.
- Document whether external import changes component parameter state, runtime state, or both.
- Prevent subsequent `OnAfterRenderAsync` from accidentally overwriting a runtime-imported scene with stale parameters.
- Add tests with fake JS runtime where document import is followed by parameter update.

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

- Failing-first stale scene key lifecycle test.
- Passing external import lifecycle test.
- Browser proof: import document, apply frame, trigger re-render, confirm scene remains correct.
- Documentation update.

## Refactor gate

Before moving to the next subbundle, record:
- changed files;
- test/build/audit commands;
- proof artifact paths;
- open risks;
- whether public API changed and how users migrate.

## Execution status

Status: completed.

- Changed files: see `../../proof/SB10/changed-file-hashes.md`.
- Test/build/audit commands: see `../../proof/SB10/transcripts/`.
- Browser proof: `../../proof/SB10/browser/run-playback-after-import-step-rerender.png`.
- Public API changed: no breaking API change; external imports now mark component lifecycle state consistently.
- Open risks: no SB10 blocker.
