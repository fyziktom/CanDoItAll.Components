# Performance and Resource Model

## Performance principles

- Prefer batched interop over per-object interop.
- Prefer patch classification and partial updates over full rebuild.
- Keep primitive profile fast and deterministic.
- Let GLB profiles be optional and diagnostic-rich.
- Add stress tests that can fail on regressions.

## Suggested diagnostic counters

```text
fullSceneRebuildCount
partialGraphMutationCount
transformOnlyPatchCount
symbolOnlyPatchCount
visualReplacePatchCount
linkGeometryUpdateCount
assetCacheEntryCount
assetCacheHitCount
assetCacheMissCount
disposedTemplateCount
disposedGeometryCount
disposedMaterialCount
disposedTextureCount
sharedTextureSkippedDisposeCount
textureOwnershipViolationCount
estimatedTriangleCount
estimatedVertexCount
averageFrameTimeMs
peakFrameTimeMs
```

## Stress tiers

| Tier | Purpose | Minimum proof |
| --- | --- | --- |
| Smoke | Normal demo scene | /tycoon-village create/select/drag/motion |
| Medium | Many primitive objects | 250 objects, 100 transform patches, no full rebuild |
| Large optional | Large simulation readiness | 1000 objects or documented local limit |
| GLB lifecycle | Asset safety | profile switching, repeated import/export/dispose |
| Economy projection | Consumer proof | large visual frame/action projection and bridge validation |
