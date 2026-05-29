# 05 - Asset model integration plan

## Problem

The first implementation used the available GLBs:

```text
1gears.glb
gears.glb
lowpoly_person_boxing.glb
question_box.glb
```

It did not use the additional downloaded models. Some of those models may be too detailed, but we need to test them as alternatives.

## Required approach

Do not replace current simple primitives. Add the downloaded models as optional asset variants.

## Asset tiers

```text
primitive     fastest, generated in runtime
model-low     small GLB / safe default
model-medium  better visual fidelity
model-high    user-provided or larger GLB; optional
```

## Sandbox UI

Add a simple selector:

```text
Asset profile:
[Primitive] [Mixed GLB] [High detail GLB]
```

Behavior:
- Primitive: all buildings/trees/props use primitives.
- Mixed GLB: agents/symbols/service buildings use current GLBs; houses/trees use small alternatives where available.
- High detail GLB: use downloaded models where available.

## Proof metrics

Extend proof snapshot:

```text
activeAssetProfile
loadedAssetCount
missingAssetCount
fallbackObjectCount
modelInstanceCount
primitiveInstanceCount
estimatedTriangleCount
estimatedVertexCount
largestLoadedAssetId
```

If exact triangle count is hard, record byte size and optional manual hints first.

## Scripts

Add a script:

```text
tools/webgllib/inventory-glb-assets.cjs
```

Output:

```text
artifacts/webgl-scene-hardening/glb-inventory.json
```

Minimum fields:

```json
{
  "path": "src/.../model.glb",
  "byteSize": 123456,
  "proposedAssetId": "asset.building.house.some-model",
  "category": "building",
  "qualityTier": "high",
  "usedBySandbox": false
}
```
