# SB03 - Asset catalog and services

## Goal

Add reusable asset catalog contracts and lightweight services.

## Required files

```text
src/CanDoItAll.Components.WebGlLib/WebGl/Assets/WebGlAssetCatalog.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Assets/WebGlAssetDefinition.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Assets/WebGlAssetVariant.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Assets/WebGlAssetAnimation.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Assets/WebGlAssetMaterialOverride.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Assets/IWebGlAssetCatalogProvider.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Assets/InMemoryWebGlAssetCatalogProvider.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Assets/WebGlAssetCatalogValidator.cs
```

## Asset definition requirements

Support:

- GLB/GLTF model assets.
- Primitive fallback assets.
- Material tint support.
- Thumbnail URL.
- Tags.
- License/source metadata.
- Default scale.
- Bounds hint.
- LOD hint.
- Animation clips.
- Variant definitions.

## Runtime behavior

The JS runtime should:

- Try to resolve `assetId` from the catalog.
- Load GLB/GLTF via GLTFLoader.
- Clone cached templates.
- Use primitive fallback if loading fails.
- Track missing assets in diagnostics.
- Avoid blocking scene render while GLB assets load asynchronously.

## Acceptance criteria

- A scene can reference assets by logical id.
- A sandbox factory can build a catalog from discovered GLB paths.
- Missing asset id does not crash the scene.
- Diagnostics include missing/loaded asset counts.
