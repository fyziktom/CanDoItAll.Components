# SB06 — Economy visual mapping boundary hardening

## Goal

Separate renderer-neutral visual mapping from WebGL-specific bridge mapping.

## Current risk

`EconomyVisualMappingDefinition` is in `Simulation.Abstractions` and includes validation concepts that mention WebGL or asset-specific terms. This creates subtle renderer leakage into shared simulation abstractions.

## Required changes

Either:

1. Rename renderer-specific validation concepts to neutral terms, e.g. `RendererSpecificAssetMarkers`, or
2. Move WebGL-specific mapping validation into `Simulation.WebGlBridge`, or
3. Split mappings into:
   - `EconomyVisualMappingDefinition` — renderer-neutral categories, action intents, semantic anchors;
   - `EconomyWebGlVisualMappingDefinition` — WebGL asset IDs, GLB assets, WebGL pose/symbol catalogs.

## Required validation

- `Simulation.Abstractions` must not contain `WebGl`, `WebGL`, `GLB`, `glb:`, or Components-specific terms, except explicitly documented schema compatibility terms if absolutely unavoidable.
- Bridge may contain WebGL-specific mapping.
