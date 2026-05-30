# SB05 - Components: pose, asset variant, and symbol catalog

Add a generic catalog layer above asset definitions:

- `WebGlVisualStateCatalog`
- `WebGlPoseDefinition`
- `WebGlSymbolDefinition`
- `WebGlActionBinding`

A pose can be implemented by:

- asset variant id;
- asset id replacement;
- rotation/scale/offset patch;
- symbol overlay;
- no-op fallback.

Example generic poses:

- `standing`
- `walking`
- `sitting`
- `working`
- `writing`
- `carrying`
- `waiting`

Example generic symbols:

- `question`
- `warning`
- `resource`
- `admin`
- `trade`
- `trust`
- `conflict`

The action planner should resolve `change-pose` and `show-symbol` through this catalog. If missing, emit warning and use fallback marker.
