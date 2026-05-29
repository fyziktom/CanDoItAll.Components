# Asset Variant Inventory

Detected 43 GLB/GLTF assets, including 39 external/user-provided models under `3DModels/glb`.

The sandbox catalog exposes the external models as optional high-detail catalog entries and uses selected building/person models as `model-high` variants. Primitive variants remain the default fallback profile.

Profiles:

- `primitive`: generated runtime primitives only.
- `model-low`: WebGlLib GLBs plus safe low-detail external alternatives.
- `model-high`: optional external building/person models from `3DModels/glb` with primitive fallback.

Generated source of truth: `glb-inventory.json`.
