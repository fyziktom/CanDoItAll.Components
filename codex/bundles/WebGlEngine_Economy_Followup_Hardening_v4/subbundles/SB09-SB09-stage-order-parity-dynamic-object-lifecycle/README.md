# SB09 Stage-order parity and dynamic-object lifecycle

## Objective

Implement the next hardening step for the WebGL/Economy engine foundation.

## Scope

- Create a shared `WebGlRunStageOrderingPolicy` and use it in validation, FromFrame, Economy bridge validation, runner diagnostics, and tests.
- Add tests with out-of-order stage list where add-object occurs before motion in playback order.
- Add tests where input order differs from playback order and validator outcome matches runtime application outcome.
- Clarify same-stage add-and-motion policy.

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

- Failing-first order mismatch test.
- Passing shared-order tests.
- Dynamic object lifecycle tests.
- Source assertion that all relevant code paths use the shared ordering policy.

## Refactor gate

Before moving to the next subbundle, record:
- changed files;
- test/build/audit commands;
- proof artifact paths;
- open risks;
- whether public API changed and how users migrate.

## Execution status

Status: completed.

- Changed files: see `../../proof/SB09/changed-file-hashes.md`.
- Test/build/audit commands: see `../../proof/SB09/transcripts/`.
- Public API changed: additive `WebGlRunStageOrderingPolicy` shared ordering helper.
- Open risks: no SB09 blocker.
