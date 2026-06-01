# SB03 — Components per-object motion queue semantics

## Goal

Ensure queued object motion is deterministic, sequential, cancellable, and analyzable.

## Required behavior

- `queueMode=append` must run motions sequentially for the same object.
- queued motion start transform must be recalculated when activated;
- cancellation must remove active and queued motions;
- clear-all must report affected objects from active and queued motions;
- diagnostics must expose active count, queued count, max queue length, completed count, cancelled count.

## Required tests

- two moves for the same object reach the second target only after the first completes;
- two different objects can move in parallel;
- cancelling queued motion does not cancel active motion unless explicitly requested;
- clear object cancels both active and queued motions;
- deterministic motion IDs remain stable in deterministic mode.

## Closure proof

- WebGL runtime audit transcript
- focused motion queue tests
