# SB09 - Snapshot visual bridge metadata

Goal:
- Capture enough visual state to relate data snapshot to WebGL scene.

Tasks:
1. Add optional visual snapshot attachment:
   - economy visual frame id,
   - WebGL run frame index/id,
   - node-object mapping,
   - active/pending action/stage ids,
   - runtime diagnostics.
2. Keep this optional and renderer-neutral enough for non-WebGL visualizers.
3. Bridge should be able to attach this metadata when projecting.

Acceptance:
- Snapshot can explain why a visible bad state corresponds to exact data state.

## Status

Completed.

## Prerequisites

SB07 snapshot contracts and SB08 snapshot store/export.

## Validation Depth

Add or verify bridge visual attachment tests for visual frame id, WebGL run frame index/id, node-object mapping, active/pending stages, and runtime diagnostics while keeping the canonical snapshot renderer-neutral.

## Progression Gate

SB14 may proceed only after visual attachment proof connects a data snapshot to visual/WebGL state without making non-WebGL snapshots depend on Components.
