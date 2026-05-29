# SB06 - Motion interpolation primitive

## Goal

Add generic smooth object motion support in `WebGlLib`.

This is not a game engine. It is a render-layer transform tween.

## Tasks

1. Add `WebGlObjectMotionCommand`.
2. Add easing constants:
   - linear
   - ease-in
   - ease-out
   - ease-in-out
3. Add JS runtime motion queue.
4. Add public JS:
   - `enqueueMotion(host, command)`
   - `clearMotions(host, objectId?)`
5. Add Blazor methods:
   - `EnqueueMotionAsync`
   - `ClearMotionsAsync`
6. Update proof snapshot with active motion count.
7. Add sandbox demo button:
   - "Move runner to plaza"
   - object moves smoothly, not by teleport.

## Acceptance criteria

- Object position changes smoothly frame-by-frame.
- Final position is exact within tolerance.
- If scene updates while motion active, runtime handles it safely.
- Render loop switches to continuous while motion is active.
