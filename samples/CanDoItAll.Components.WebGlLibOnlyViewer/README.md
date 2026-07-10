# WebGlLib-Only Viewer Sample

This is the smallest usable `WebGlSceneView` application: a generic `WebGlSceneModel` with one primitive asset, rendered with only `CanDoItAll.Components.WebGlLib`.

Use it as a starting point when your Blazor app needs an interactive 3D scene but does not need timeline/run playback. It intentionally has no `CanDoItAll.Components.WebGlRunLib` or domain-package reference, so the scene model, its assets, and its behavior remain application-owned.

## Build from source

```powershell
dotnet build samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj
```

## Validate package consumption

```powershell
$proofVersion = "0.1.0-sb11.20260602.1"
$env:NUGET_PACKAGES = "C:\temp\candoitall-webgllib-only-packages"
dotnet restore samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj --configfile C:\path\to\fresh-components.NuGet.config /p:UseComponentsWebGlLibPackage=true /p:ComponentsWebGlLibPackageVersion=$proofVersion
dotnet build samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj --no-restore /p:UseComponentsWebGlLibPackage=true /p:ComponentsWebGlLibPackageVersion=$proofVersion
```

Use a fresh local package source, a unique proof version, and an isolated `NUGET_PACKAGES` folder when proving package consumption. This prevents an older package from a global cache or private feed from shadowing the package that was just packed.

Restore again without `UseComponentsWebGlLibPackage=true` before switching back to the project-reference build. Static web asset restore metadata is mode-specific, so reusing package-mode assets with a project-reference build can make the same WebGlLib assets appear twice.
