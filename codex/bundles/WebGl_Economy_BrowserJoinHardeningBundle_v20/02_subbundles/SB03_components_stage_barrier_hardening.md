# SB03 - Components stage barrier hardening

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
