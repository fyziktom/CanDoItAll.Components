# SB05 - Economy WebGL bridge projection hardening

Goal:
- Turn bridge into a reliable generic projector.

Tasks:
1. Verify `EconomyWebGlInitialSceneProjector` maps:
   - nodes to objects,
   - links to links,
   - symbols to status symbols,
   - layers to layers,
   - node-object mapping to context.
2. Verify `EconomyWebGlActionStageProjector` maps actions to real stages containing patches/motions.
3. Do not duplicate global input actions across every frame.
4. Add bridge diagnostics when:
   - subject node unresolved,
   - target node unresolved,
   - visual mapping missing asset,
   - action kind mapped to Wait fallback,
   - planned action invalid.
5. Add bridge proof for both shared-resource and finite-resource probes.

Acceptance:
- Projected WebGlRunDocument contains an InitialScene and at least one frame with non-empty executable stages for a non-trivial action sequence.

## Status

Completed.

## Prerequisites

SB03 stage/motion proof and SB04 action plan to batch proof.

## Validation Depth

Add or verify bridge tests for initial scene projection, visual action stage projection, non-duplicated global input actions, diagnostics for unresolved/missing/fallback mappings, and shared-resource plus finite-resource probes.

## Progression Gate

SB06, SB11, SB12, and SB13 may proceed only after bridge proof demonstrates a traceable executable WebGL run document with non-empty stages and diagnostics for rejected shallow implementations.
