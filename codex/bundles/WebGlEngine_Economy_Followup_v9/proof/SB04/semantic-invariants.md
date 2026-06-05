# SB04 semantic invariants

Status: completed

## Invariants

- Non-waiting `applyCommandBatch` must return `lifecycleState=scheduled` and `settled=false` while motion, command-stage, or barrier work remains.
- Wait-capable `applyCommandBatchAndWait` must return `lifecycleState=settled` and `settled=true` only after runtime idle succeeds.
- The C# browser apply adapter defaults to the wait-capable command-batch path when no explicit runtime-idle policy is configured.
- Explicit runtime-idle policies preserve non-waiting command application and perform their configured idle waits separately.
- C# browser apply snapshots expose command lifecycle and runtime idle blocker diagnostics so observer proof cannot flatten scheduled work into success.

## Proof hooks

- `bundle://proof/SB04/transcripts/webglrunlib-tests.txt`
- `bundle://proof/SB04/browser/command-batch-lifecycle-assertions.json`
- `bundle://proof/SB04/transcripts/source-assertions.txt`
