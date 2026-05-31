# SB03 — Stage runner lifecycle hardening

## Goal
Make command stages reliable for ordered visual stories.

## Problems to solve
- `waitSeconds` is a timer, not motion completion semantics.
- Stages applied after the initial command result are not fully represented in the original batch result.
- Errors in delayed stages need durable diagnostics.

## Required
Add stage barrier policies:

- `time-delay`
- `wait-for-active-motions`
- `wait-for-object-motions`
- `wait-for-render-idle`
- `manual-step`

Add diagnostics:

- current stage id
- completed stages
- failed stages
- skipped stages
- last stage error
- stage result log
- stage queue snapshot

## Validation
- Stage A applies patch.
- Stage B waits for motion completion.
- Stage C applies pose/symbol.
- Stage D returns object home.
- Diagnostics show all stages.
