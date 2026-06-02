# WebGlRun frame command semantics

## Problem

A `WebGlRunFrame` can contain direct `ScenePatches`/`Motions` and also `Stages`. Current frame application emits direct commands only when there are no stages. This creates a silent command-loss trap.

## Required decision

Choose one and enforce it:

### Option A — validator rejects mixed direct and staged commands

If `frame.Stages.Count > 0`, then `frame.ScenePatches` and `frame.Motions` must be empty. This is the simplest and safest near-term policy.

### Option B — direct commands become deterministic implicit stages

Direct frame patches/motions are converted into generated stages such as:

- `frame:{index}:prelude` before explicit stages; or
- `frame:{index}:direct` based on metadata policy.

## Acceptance

No frame may pass validation and then silently drop commands. Tests must prove the previous shallow implementation fails.
