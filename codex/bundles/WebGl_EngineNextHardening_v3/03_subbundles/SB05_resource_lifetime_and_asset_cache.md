# SB05 — Resource Lifetime and Asset Cache Hardening

## Goal

Prevent GPU/resource leaks and make GLB cache ownership explicit.

## Current issue

The branch introduced `17-webgl-scene-resources.js`, which is good. However, the state-owned `assetCache` stores loaded GLB template promises. Disposal currently clears runtime scene objects and decorations, but the ownership/lifetime of cached shared templates should be explicit.

## Tasks

1. Define cache mode:
   - state-local cache for now,
   - future global cache only with reference counting.

2. Add:
   ```text
   21-webgl-scene-asset-cache.js
   ```
   Responsibilities:
   - create state asset cache,
   - get/load template promise,
   - dispose state-local cached templates on scene dispose if owned,
   - track cache hit/miss counts,
   - track disposed template count.

3. Resource rules:
   - GLB template resources are shared within one state and must not be disposed by model instance disposal.
   - Cloned/tinted materials are instance-owned and must be disposed.
   - Primitive geometries/materials are instance-owned and must be disposed.
   - Debug bounds helpers are instance-owned.
   - Decorations must be disposed exactly once.

4. Extend diagnostics:
   - `assetCacheEntryCount`
   - `assetCacheHitCount`
   - `assetCacheMissCount`
   - `disposedTemplateCount`
   - `disposedGeometryCount`
   - `disposedMaterialCount`
   - `disposedTextureCount`

5. Add browser proof:
   - create scene,
   - switch model variants repeatedly,
   - dispose scene,
   - recreate scene,
   - diagnostics must not show unbounded cache/resource growth.

## Done criteria

- Asset cache ownership is explicit.
- State dispose clears state-local cache.
- No double-dispose errors.
- Diagnostics expose resource lifetime evidence.
