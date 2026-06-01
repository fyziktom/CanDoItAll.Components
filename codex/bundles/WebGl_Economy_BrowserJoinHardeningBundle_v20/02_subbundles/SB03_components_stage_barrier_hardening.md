# SB03 - Components stage barrier hardening

## Status

Completed. Entry and closure gates passed.

## Goal

Make stage barriers robust enough for visual action sequences.

## Tasks

- Add tests for:
  - wait-for-active-motions,
  - wait-for-object-motions,
  - wait-for-render-idle,
  - wait-for-event/manual-step.
- Verify that queued stages do not execute before required motion completion.
- Verify journal entries are bounded and preserve sequence order.
- Add failure behavior for unknown barrier policies:
  - either explicit warning + no-op,
  - or strict option to fail.

## Acceptance

- Tests cover at least one two-stage same-object motion sequence.
- Diagnostics include current barrier, blockers, and recent journal tail.

## Prerequisites

- SB02 adapter proof completed or blocked without impacting JS runtime work.
- Stage runner/barrier/journal source files are present in Components.

## Owned Requirements

- R03 Stage barrier hardening.

## Dependency Impact

Barrier behavior controls whether browser-applied stages execute in visual order. Weak proof invalidates SB05 and SB11 browser smoke.

## Validation Depth

JS audit and/or unit tests must cover active motions, object motions, render idle, event/manual step, unknown policy behavior, bounded journal order, and same-object two-stage sequencing.

## Proof Required

- `node tools/webgllib/audit-stage-runner.cjs`
- `node tools/webgllib/audit-scene-runtime.cjs`
- `bundle://proof/SB03/transcripts/stage-runner-audit.txt`
- `bundle://proof/SB03/transcripts/scene-runtime-audit.txt`
- `bundle://proof/SB03/manifest.md`
- `bundle://proof/SB03/semantic-invariants.md`

## Browser Validation Logging

N/A for this runtime-audit subbundle. SB11 will record browser-visible stage behavior.

## Semantic Adequacy Gate

- Shallow-pass trap: journal entries exist but stages can still execute before barriers release.
- Adversarial negative proof: unknown/stuck barrier path emits warning or strict failure instead of silently advancing.
- Semantic positive proof: two-stage same-object motion sequence waits for the first motion before the dependent stage.
- Anti-stub audit: JS runtime remains JavaScript, bounded, and free of placeholder paths.

## Progression Gate

Pass only when stage sequencing and diagnostics are proven. SB04/SB05 must reopen this subbundle if browser runtime diagnostics contradict the audit.
