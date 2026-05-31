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

## Status
- Completed.

## Prerequisites
- SB03 stage-runner lifecycle proof is complete.

## Exact Source References
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/29-webgl-scene-motion-queues.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/31-webgl-scene-motion-cancellation.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/14-webgl-scene-motion.js`
- `repo://tests/CanDoItAll.Components.WebGlLib.Tests`

## Dependency Impact
- Critical foundation for physically understandable visual action sequences and bridge proof.

## Validation Depth
- Requires positive A/B/C/home queue proof, negative missing-object and zero-duration cases, queue policy assertions, and anti-stub audit.

## Acceptance Checklist
- Queue policy is explicit for append, replace, cancel-and-replace, and reject-if-active.
- Queue diagnostics expose active and queued motion IDs.
- Clearing an object clears both active and queued motion state.

## Proof Required
- `bundle://proof/SB04/manifest.md`
- `bundle://proof/SB04/semantic-invariants.md`
- Test transcripts and source assertions.

## Browser Validation Logging
- Browser validation is not required unless rendered scene behavior changes without equivalent runtime tests.

## Progression Gate
- SB05 and Economy bridge work may depend on queue semantics after deterministic queue proof is recorded.

## Suggested Agent Prompt
- Harden and prove motion queue ordering, cancellation, diagnostics, and edge cases with deterministic runtime tests.
