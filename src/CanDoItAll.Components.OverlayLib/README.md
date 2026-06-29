# CanDoItAll.Components.OverlayLib

Package version: `0.1.0`.

## Purpose

Floating overlay and window component library for workbench and tool surfaces.

OverlayLib owns generic floating-window behavior through `OverlayWindow`, `OverlayWindowState`, generated asset components, and a plain browser JavaScript runtime for bounded placement, drag, resize, minimize, reset, hide, and show behavior.

## Project Type

- SDK: `Microsoft.NET.Sdk.Razor`
- Target framework(s): `net10.0`
- Packable: `true`
- Package readme: `README.md`

## Package Usage

Reference the package from a Blazor app or another Razor class library, then add the generated asset components once in the host shell:

```razor
<OverlayLibHeadAssets />
<OverlayLibBodyAssets IncludeRuntimeAssets="true" />
```

`OverlayWindow` can be hosted inside any bounded container. Use `ContainerSelector` and `SafeTopSelector` when the window must stay inside a specific host frame and below a toolbar/safe-top region.

## References

Project references:

- `../CanDoItAll.Components.BaseLib/CanDoItAll.Components.BaseLib.csproj`

Framework references:

- None

Direct package references:

- `Microsoft.AspNetCore.Components.Web (10.0.4)`

## Runtime Dependency Policy

No npm runtime dependency is required for OverlayLib floating-window behavior. Runtime implementation is C#/Razor plus plain browser JavaScript loaded from package static web assets.

Node/npm usage in this repository is tooling-only for asset generation/verification, Tailwind, Playwright proof, and related test automation.

## Publishing Validation

Run these checks before publishing or transferring the package:

```powershell
dotnet build src/CanDoItAll.Components.OverlayLib/CanDoItAll.Components.OverlayLib.csproj --no-restore
dotnet test tests/CanDoItAll.Components.BaseLib.Tests/CanDoItAll.Components.BaseLib.Tests.csproj --filter FullyQualifiedName~CanvasOverlayPublishingApprovalTests --no-restore
dotnet pack src/CanDoItAll.Components.OverlayLib/CanDoItAll.Components.OverlayLib.csproj --configuration Release --no-restore --output artifacts/packages/sb09
```

The focused publishing approval tests freeze CanvasLib/OverlayLib public API metadata, packability/readme metadata, and static web asset manifests.

## Architecture Notes

Keep shared UI reusable and typed. Use BaseLib for ordinary product UI, CanvasLib for graph/canvas surfaces, OverlayLib for floating windows, WebGlLib for WebGL concepts, and sandbox projects only for demos or proof.

OverlayLib owns generic floating-window behavior. `OverlayWindow` and `OverlayWindowState` define visibility, minimized status, geometry normalization, container/safe-top placement, drag/resize lifecycle, reset/hide/show behavior, and the plain browser JavaScript runtime used by both OverlayLib and CanvasLib wrappers. Canvas-specific wrappers should adapt their own state into `OverlayWindowState` instead of duplicating generic window lifecycle logic.

## Related Docs

- Repository overview: `README.md` at this repo root
- Canvas wrapper ownership note: `../CanDoItAll.Components.CanvasLib/Canvas/README.md`
