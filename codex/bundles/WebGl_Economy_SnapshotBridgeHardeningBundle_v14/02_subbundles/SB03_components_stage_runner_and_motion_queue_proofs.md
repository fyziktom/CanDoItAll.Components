# SB03 - Components stage runner and motion queue proofs

Goal:
- Prove ordered staged visual actions work reliably.

Tasks:
1. Add tests for stage wait timing:
   - first stage applies immediately,
   - wait blocks second stage,
   - next stage applies after time advance.
2. Add tests for per-object motion queue:
   - append queues after active motion,
   - active completion activates next motion with updated start transform,
   - cancel active motion,
   - cancel queued motion,
   - object removal clears active and queued motion.
3. Add diagnostics:
   - active motion count,
   - queued motion count,
   - max queue length,
   - current stage id,
   - queued stage count,
   - stage wait seconds.
4. Ensure stage runner does not continue after dispose/import reset.

Acceptance:
- Shared-resource sequence can be represented as ordered stages without motion races.

## Status

Completed.

## Prerequisites

SB02 runtime audit and scheduler/domain guard.

## Validation Depth

Add or verify WebGlLib runtime tests for staged waits, queue append/activation, active and queued cancellation, object removal, dispose/import reset, and diagnostics fields.

## Progression Gate

SB04/SB05 may proceed only after stage and motion queue tests prove ordered stage playback cannot be faked by immediate execution or stale queued motion state.
