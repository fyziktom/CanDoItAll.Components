# WebGlLib-Only Viewer Sample

This sample proves a consumer can build a minimal `WebGlSceneView` surface with only `CanDoItAll.Components.WebGlLib`.

It intentionally has no `CanDoItAll.Components.WebGlRunLib` reference and no domain package reference. The component renders one generic primitive asset from a `WebGlSceneModel`.

Build proof:

```powershell
dotnet build samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj
```
