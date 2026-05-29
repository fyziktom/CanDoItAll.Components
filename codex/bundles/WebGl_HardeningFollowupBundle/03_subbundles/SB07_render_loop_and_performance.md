# SB07 - Render loop and performance hardening

## Goal

Avoid permanent 60 FPS rendering when nothing is moving.

## Tasks

1. Add `WebGlRenderModes`:
   - auto
   - continuous
   - on-demand
2. Add runtime invalidation model:
   - camera changed,
   - asset loaded,
   - symbol effect active,
   - motion active,
   - scene patched,
   - hover/selection changed.
3. In auto mode:
   - continuous while motion/symbol effects/camera damping are active,
   - on-demand otherwise.
4. Add diagnostics:
   - renderMode,
   - activeMotionCount,
   - animatedSymbolCount,
   - lastFrameReason,
   - frameTimeMs.
5. Add optional max DPR cap per scene.

## Acceptance criteria

- Static scene settles into on-demand rendering in auto mode.
- Symbol/motion scene uses continuous rendering only while needed.
- Diagnostics show render reason and active counts.
