# WebGlLib-Only Viewer Sample

This sample proves a consumer can build a minimal `WebGlSceneView` surface with only `CanDoItAll.Components.WebGlLib`.

It intentionally has no `CanDoItAll.Components.WebGlRunLib` reference and no domain package reference. The component renders one generic primitive asset from a `WebGlSceneModel`.

Build proof:

```powershell
dotnet build samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj
```

Package-mode proof:

```powershell
$proofVersion = "0.1.0-sb11.20260602.1"
$env:NUGET_PACKAGES = "C:\temp\candoitall-webgllib-only-packages"
dotnet restore samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj --configfile C:\path\to\fresh-components.NuGet.config /p:UseComponentsWebGlLibPackage=true /p:ComponentsWebGlLibPackageVersion=$proofVersion
dotnet build samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj --no-restore /p:UseComponentsWebGlLibPackage=true /p:ComponentsWebGlLibPackageVersion=$proofVersion
```

Use a fresh local package source, a unique proof version, and an isolated `NUGET_PACKAGES` folder when proving package consumption. This prevents an older package from a global cache or private feed from shadowing the package that was just packed.
