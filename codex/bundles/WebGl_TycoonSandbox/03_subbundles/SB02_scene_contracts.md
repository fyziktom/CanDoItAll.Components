# SB02 - Generic scene contracts

## Goal

Add domain-neutral C# models for generic WebGL scenes.

## Required files

```text
src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneModel.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneObject.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneLink.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneCamera.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneEnvironment.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneProofSnapshot.cs
```

## Contract requirements

`WebGlSceneModel` should include:

- `SceneId`
- `Title`
- `Description`
- `AssetCatalog`
- `Environment`
- `Camera`
- `Objects`
- `Links`
- `UiState`
- `Interaction`
- `Metadata`

`WebGlSceneObject` should include:

- `Id`
- `Kind`
- `Family`
- `Title`
- `Subtitle`
- `Description`
- `AssetId`
- `Position`
- `Rotation`
- `Scale`
- `Size`
- `Tone`
- `Color`
- `IsSelectable`
- `IsDraggable`
- `Symbols`
- `Tags`
- `Metadata`

No economy-specific property names.

## Acceptance criteria

- All DTOs compile.
- Defaults are safe.
- DTOs serialize with `JsonSerializerDefaults.Web`.
- The contracts can describe:
  - building
  - prop
  - agent
  - status symbol
  - relation/link
  - village/city scene
