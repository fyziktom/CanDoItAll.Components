# SB04 — Components motion queue semantics

## Goal
Ensure ordered movement is deterministic and physically understandable.

## Required proof
- Object moves A -> B -> C -> home.
- Start position for B is end position of A.
- Start position for C is end position of B.
- Cancelling active motion cancels or preserves queued motions according to explicit policy.
- Clearing object motion clears active and queued motions.
- Queue diagnostics expose queue length and queued motion IDs.

## Required hardening
- Add queue snapshot to WebGL proof snapshot.
- Add optional `queuePolicy`: append, replace, cancel-and-replace, reject-if-active.
- Add tests for zero-duration and missing-object edge cases.
