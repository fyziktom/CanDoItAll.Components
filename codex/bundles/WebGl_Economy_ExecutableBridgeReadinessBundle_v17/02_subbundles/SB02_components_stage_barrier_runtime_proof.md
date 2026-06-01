# SB02 — Components stage barrier runtime proof

## Goal

Harden and prove generic stage barrier behavior in WebGL runtime.

## Required behavior

Prove these barrier policies:

- `wait-seconds`
- `wait-for-active-motions`
- `wait-for-object-motions`
- `wait-for-render-idle`
- `wait-for-event`

## Required tests

Add tests/fixtures proving:

1. Stage B does not apply while stage A motion is active.
2. `wait-for-object-motions` only blocks on the listed object IDs.
3. `wait-for-event` does not keep continuous render spinning.
4. Manual signal resumes the stage runner.
5. Failed delayed stage appears in diagnostics and journal.
6. Bounded journal trims older entries without losing current failure state.

## Refactoring guard

Keep JS files under existing audit thresholds. If a file grows near the warning threshold, split it.

## Closure proof

- JS runtime audit transcript
- unit/headless runtime test transcript
- source assertions for barrier policies
