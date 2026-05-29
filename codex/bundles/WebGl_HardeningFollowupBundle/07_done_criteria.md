# 07 - Done criteria

The follow-up is complete when:

- User-provided GLB models are inventoried and available as optional asset variants.
- Current simple primitives remain as default or fallback.
- `WebGlAssetVariant` is actually used by runtime.
- Drag-on-ground-plane works for draggable objects.
- `ObjectsMoved` is fired from JS and handled in Blazor.
- Scene patch API exists and transform-only patches do not rebuild the whole scene.
- Object motion/tween primitive exists and is demonstrated.
- Export/import scene state works.
- Render loop supports auto/on-demand/continuous.
- Create failure path emits `RuntimeError`.
- Empty partial `WebGlSceneSelectionState` smell is removed.
- Long JS/Razor/CSS files are split or justified.
- Browser proof covers primitive + GLB variant profiles.
- Build and asset generation pass.
- Final report explains what belongs to `WebGlLib` vs future `WebGlRunLib`.
