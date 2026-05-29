# SB05 — Components model diagnostics batch report

## Goal

Make invisible/broken external GLBs diagnosable.

## Tasks

1. Extend `ModelLab` or add a diagnostics route that can iterate all catalog model assets.
2. Add a deterministic batch diagnostics report:
   - asset id;
   - variant id;
   - uri;
   - load ok;
   - mesh count;
   - visible mesh count;
   - material count;
   - transparent material count;
   - bounds;
   - center;
   - warnings/errors;
   - suggested import recipe.
3. Add import recipe support:
   - unit scale;
   - center mode;
   - fit mode;
   - fixed scale;
   - rotation offset;
   - position offset;
   - double-sided material;
   - material visibility normalization;
   - debug bounds.
4. Export diagnostics to:
   - `artifacts/webgl-engine-prep-v4/model-diagnostics.json`
   - `artifacts/webgl-engine-prep-v4/model-diagnostics.md`

## Validation

- high-detail profile does not crash even when some models fallback;
- failed GLBs do not break scene rendering.
