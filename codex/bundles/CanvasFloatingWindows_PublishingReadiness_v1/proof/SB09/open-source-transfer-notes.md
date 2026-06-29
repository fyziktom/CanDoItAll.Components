# SB09 Open Source Transfer Notes

## Package Status

- CanvasLib package artifact: `bundle://proof/SB09/packages/CanDoItAll.Components.CanvasLib.0.1.1.nupkg`.
- OverlayLib package artifact: `bundle://proof/SB09/packages/CanDoItAll.Components.OverlayLib.0.1.0.nupkg`.
- Package content manifests verify expected assemblies, README files, and static web assets.

## Public Contracts

- CanvasLib exposes the Canvas workbench contracts, state records, calendar payloads, preview records, and generated asset host components.
- OverlayLib owns the shared floating-window runtime and `OverlayWindowState` behavior.
- `canvas-floating-window.js` remains a CanvasLib compatibility shim over the OverlayLib runtime instead of a duplicated lifecycle implementation.

## Runtime Dependency Boundary

- Canvas, floating-window, calendar, and preview runtime implementation is C# and Razor plus plain browser JavaScript.
- Root npm packages remain tooling-only. Current dev dependencies are Playwright and Three.js; Three.js is WebGL tooling/runtime support outside this bundle's scope and is not introduced for Canvas/Overlay runtime.
- CanvasLib/OverlayLib runtime JS contains no `import()` or `require()` dependency path.

## Validation Commands

```powershell
npm run canvaslib:verify-assets
dotnet test tests\CanDoItAll.Components.BaseLib.Tests\CanDoItAll.Components.BaseLib.Tests.csproj --filter FullyQualifiedName~CanvasOverlayPublishingApprovalTests --no-restore
dotnet build src\CanDoItAll.Components.CanvasLib\CanDoItAll.Components.CanvasLib.csproj --configuration Release --no-restore
dotnet build src\CanDoItAll.Components.OverlayLib\CanDoItAll.Components.OverlayLib.csproj --configuration Release --no-restore
dotnet pack src\CanDoItAll.Components.CanvasLib\CanDoItAll.Components.CanvasLib.csproj --configuration Release --no-restore --output artifacts\packages\sb09
dotnet pack src\CanDoItAll.Components.OverlayLib\CanDoItAll.Components.OverlayLib.csproj --configuration Release --no-restore --output artifacts\packages\sb09
```

## Known Follow-Ups

- Broad solution Release build currently trips on out-of-scope WebGL sample/test restore gaps under `--no-restore`; this bundle intentionally did not repair WebGL.
- WebGL publishing readiness should be handled as a separate bundle.
- Before public release, decide whether package artifacts should be rebuilt into the repository's canonical release output folder rather than the SB09 proof folder.

