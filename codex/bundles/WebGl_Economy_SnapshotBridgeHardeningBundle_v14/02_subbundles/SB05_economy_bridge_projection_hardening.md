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
