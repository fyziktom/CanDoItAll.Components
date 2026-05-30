# SB07 — Components: asset cache and resource lifetime

## Required work
- Verify `disposeAssetCache` is called on scene dispose and import/reload paths as appropriate.
- Add lifecycle tests or browser proof for repeated scene import/dispose.
- Track:
  - asset cache hits/misses;
  - disposed template count;
  - model instance count;
  - material clone count.
- Avoid disposing shared template geometry/materials used by cached templates.
- Avoid leaking per-instance cloned materials.
