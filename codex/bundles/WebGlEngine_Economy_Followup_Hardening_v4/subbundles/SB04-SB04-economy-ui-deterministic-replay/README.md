# SB04 Economy UI deterministic replay

## Objective

Implement the next hardening step for the WebGL/Economy engine foundation.

## Scope

- Refactor `EconomySimulationSandboxPage` to use `WebGlRunDocumentRunner` or an equivalent deterministic playback service.
- `Step`, `First`, `Last`, and future seek operations must apply the required sequence from initial scene to target frame, or prove frames are absolute.
- Add UI diagnostics for `appliedFrameIndexes`, `requiresSceneReset`, `runnerState`, and failure reason.
- Ensure `BrowserSceneWasReset` cannot become a stale local boolean that disagrees with runner state.

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

- Failing-first browser/component test for seek-to-last with a delta frame.
- Passing component test proving multiple frames are applied for seek-to-last.
- Node route browser proof for Step, Last, First, Apply, Snapshot.
- Console review and runtime diagnostics JSON.

## Refactor gate

Before moving to the next subbundle, record:
- changed files;
- test/build/audit commands;
- proof artifact paths;
- open risks;
- whether public API changed and how users migrate.
