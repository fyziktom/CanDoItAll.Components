# SB10 Open Source Transfer Checklist

## Ready Items

- CanvasLib package docs aligned to version `0.1.1`.
- OverlayLib package docs aligned to inherited version `0.1.0`.
- CanvasLib and OverlayLib publishing approval tests pass.
- Canvas generated assets verify cleanly.
- CanvasLib and OverlayLib focused Release builds pass.
- CanvasLib and OverlayLib packages pack and include expected static web assets.
- Browser proof exists for Canvas workbench, calendar/preview, floating windows, benchmark route health, and overlay lifecycle.
- Runtime dependency proof confirms npm is tooling-only for Canvas/Floating Windows/Calendar/Preview scope.

## Validation Commands

```powershell
npm run canvaslib:verify-assets
dotnet test tests\CanDoItAll.Components.BaseLib.Tests\CanDoItAll.Components.BaseLib.Tests.csproj --filter "FullyQualifiedName~CanvasContractBehaviorTests|FullyQualifiedName~LayoutNavigationOverlayBehaviorTests|FullyQualifiedName~CanvasOverlayPublishingApprovalTests" --no-restore
dotnet build src\CanDoItAll.Components.CanvasLib\CanDoItAll.Components.CanvasLib.csproj --configuration Release --no-restore
dotnet build src\CanDoItAll.Components.OverlayLib\CanDoItAll.Components.OverlayLib.csproj --configuration Release --no-restore
dotnet pack src\CanDoItAll.Components.CanvasLib\CanDoItAll.Components.CanvasLib.csproj --configuration Release --no-restore --output artifacts\packages
dotnet pack src\CanDoItAll.Components.OverlayLib\CanDoItAll.Components.OverlayLib.csproj --configuration Release --no-restore --output artifacts\packages
```

## Explicit Follow-Ups

- WebGL publishing readiness, including its sample/test restore drift, is a separate bundle.
- Public release packaging should decide final artifact output location and versioning cadence.
- If future package docs alter sandbox-visible output, rerun the SB08 route matrix or the narrower affected route/viewport.
