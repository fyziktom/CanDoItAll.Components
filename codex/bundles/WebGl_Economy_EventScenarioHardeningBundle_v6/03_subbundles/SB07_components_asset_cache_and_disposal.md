# SB07 - Components: asset cache and disposal

Current risk: asset cache helpers exist, but lifecycle must explicitly dispose cache entries when the scene is disposed.

Required:

- call `disposeAssetCache(state)` from lifecycle disposal;
- ensure templates marked as shared are disposed only by cache disposal;
- ensure cloned materials are disposed by object disposal;
- add diagnostics counters:
  - `assetCacheEntryCount`
  - `assetCacheHitCount`
  - `assetCacheMissCount`
  - `disposedTemplateCount`
  - `disposedMaterialCount`
  - `disposedGeometryCount`

Add tests or browser proof where a scene is mounted/unmounted repeatedly and diagnostics do not grow unexpectedly.
