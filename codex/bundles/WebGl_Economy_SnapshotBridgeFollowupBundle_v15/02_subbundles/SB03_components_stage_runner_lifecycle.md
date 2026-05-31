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

## Status
- Completed.

## Prerequisites
- SB02 runtime audit has passed or has explicit non-blocking warnings.

## Exact Source References
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/30-webgl-scene-stage-runner.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/29-webgl-scene-motion-queues.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/26-webgl-scene-command-batch.js`
- `repo://tests/CanDoItAll.Components.WebGlLib.Tests`

## Dependency Impact
- Critical foundation for ordered bridge playback and SB04-SB06 proof.

## Validation Depth
- Requires failing-first or adversarial proof for shallow timer-only staging, semantic positive proof for motion-completion barriers, and anti-stub audit.

## Acceptance Checklist
- Barrier policies exist and are exercised.
- Delayed stage diagnostics survive after the initial command result.
- Stage result log includes completed, failed, skipped, current, and queue snapshot information.

## Proof Required
- `bundle://proof/SB03/manifest.md`
- `bundle://proof/SB03/semantic-invariants.md`
- Test and runtime audit transcripts.
- Source assertions for stage barrier and diagnostics code.

## Browser Validation Logging
- Browser validation is not required unless the rendered runtime route changes; JS/runtime tests can close this subbundle.

## Progression Gate
- SB04 and SB05 may proceed when stage diagnostics prove all A/B/C/home stages.

## Suggested Agent Prompt
- Add or verify stage barriers and durable diagnostics, then prove ordered stage execution with negative and positive tests.
