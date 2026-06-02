# SB02 WebGlRun fail-closed runner semantics

## Objective

Implement the next hardening step for the WebGL/Economy engine foundation.

## Scope

- Harden `WebGlRunDocumentRunner.ApplyCurrentFrameAsync` so errors from `WebGlRunFrameApplyResult.FromFrame` are merged and stop execution.
- Add tests where `WebGlRunFrameExecutionValidator` passes but `FromFrame` returns an error; runner must not call the applier.
- Ensure `WebGlRunBrowserApplyAdapter` result diagnostics explicitly distinguish `PreApplyValidationFailed`, `ResetFailed`, and `BatchFailed`.
- Add source assertions for fail-closed branches.

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

- Failing-first runner test.
- Passing runner fail-closed test.
- Fake applier proof that no apply call occurs after frame conversion errors.
- WebGlRunLib full tests pass.

## Refactor gate

Before moving to the next subbundle, record:
- changed files;
- test/build/audit commands;
- proof artifact paths;
- open risks;
- whether public API changed and how users migrate.
