# SB02 — Components Runtime Executable Stage Proof

## Goal

Harden and prove that a generic WebGL run stage sequence is executable by runtime primitives, not only representable as DTOs.

## Required actions

- Add or strengthen tests for stage barriers:
  - wait-seconds
  - wait-for-active-motions
  - wait-for-object-motions
  - wait-for-render-idle
  - wait-for-event/manual-step
- Prove queued per-object motions execute sequentially and do not compete for transform updates.
- Prove scheduler stays active while queued motions or stage barriers exist.
- Prove command/stage journal includes delayed stage completions.
- Keep JS files under hard threshold; split warning-level files if they grow.

## Acceptance

A test must simulate:

```text
move actor to target -> wait for actor motion -> apply pose/symbol patch -> wait -> return actor home
```

It must verify final position, journal entries, diagnostics and no duplicate motion conflict.

## Proof

Create:

```text
proof/SB02/transcripts/components-stage-runtime-tests.txt
proof/SB02/transcripts/components-scene-runtime-audit.txt
proof/SB02/semantic-invariants.md
```
