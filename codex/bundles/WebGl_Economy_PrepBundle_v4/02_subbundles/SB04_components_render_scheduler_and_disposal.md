# SB04 — Components render scheduler and resource disposal hardening

## Goal

Ensure static scenes go idle and GLB resources do not leak.

## Tasks

1. Replace perpetual `requestAnimationFrame` scheduling with an idle-aware scheduler:
   - continuous mode: always loop;
   - auto mode: loop while motion, camera damping, animated symbols, or pending invalidation;
   - on-demand mode: render only explicit invalidations.
2. Track:
   - `isRenderLoopActive`
   - `idleSinceTimestamp`
   - last render reason
   - active motion count
   - animated symbol count
3. Add resource audit helper:
   - templates are shared;
   - cloned material ownership is explicit;
   - primitive/generated geometries are disposed;
   - debug bounds helpers are disposed.
4. Add browser proof that a no-motion/no-symbol static scene reaches idle.

## Validation

- `npm run webgllib:audit-scene-runtime`
- browser proof JSON includes idle scheduler state.
