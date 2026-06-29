# CanDoItAll.Components.CanvasLib

Package version: `0.1.1`.

## Purpose

Shared canvas, graph, workbench, calendar, preview, and accessibility components for CanDoItAll workbench-style interactive surfaces.

CanvasLib owns the typed Canvas contracts and shipped browser runtime for:

- `CanvasWorkbench` and its workbench state/contracts.
- `CanvasCalendar` and calendar CRUD/export/request contracts.
- graph primitive, composition, interaction, overlay, chrome, and diagnostic preview components.
- `CanvasFloatingWindow`, which adapts Canvas state into OverlayLib's generic `OverlayWindow` runtime.
- generated static asset components that load the runtime in a deterministic order.

## Project Type

- SDK: `Microsoft.NET.Sdk.Razor`
- Target framework(s): `net10.0`
- Packable: `true`
- Package readme: `README.md`

## Package Usage

Reference the package from a Blazor app or another Razor class library, then add the generated asset components once in the host shell:

```razor
<CanvasLibHeadAssets />
<CanvasLibBodyAssets IncludeRuntimeAssets="true"
                     IncludePreviewAssets="true"
                     IncludeCalendarAssets="true" />
```

`CanvasLibHeadAssets` includes OverlayLib head assets first. `CanvasLibBodyAssets` includes OverlayLib runtime assets first, then Canvas service/runtime scripts, optional preview scripts, and optional calendar scripts. Do not hand-edit these generated asset components; update `tools/canvaslib/build-assets.cjs` and run the asset build if the verified order needs to change.

## References

Project references:

- `../CanDoItAll.Components.BaseLib/CanDoItAll.Components.BaseLib.csproj`
- `../CanDoItAll.Components.Common/CanDoItAll.Components.Common.csproj`
- `../CanDoItAll.Components.OverlayLib/CanDoItAll.Components.OverlayLib.csproj`

Framework references:

- None

Direct package references:

- `Microsoft.AspNetCore.Components.Web (10.0.4)`

## Runtime Dependency Policy

No npm runtime dependency is required for CanvasLib workbench, floating-window, preview, or calendar behavior. Runtime implementation is C#/Razor plus plain browser JavaScript loaded from package static web assets.

Node/npm usage in this repository is tooling-only for asset generation/verification, Tailwind, Playwright proof, and related test automation.

## Publishing Validation

Run these checks before publishing or transferring the package:

```powershell
npm run canvaslib:verify-assets
dotnet build src/CanDoItAll.Components.CanvasLib/CanDoItAll.Components.CanvasLib.csproj --no-restore
dotnet test tests/CanDoItAll.Components.BaseLib.Tests/CanDoItAll.Components.BaseLib.Tests.csproj --filter FullyQualifiedName~CanvasOverlayPublishingApprovalTests --no-restore
dotnet pack src/CanDoItAll.Components.CanvasLib/CanDoItAll.Components.CanvasLib.csproj --configuration Release --no-restore --output artifacts/packages/sb09
```

The focused publishing approval tests freeze CanvasLib/OverlayLib public API metadata, packability/readme metadata, and static web asset manifests.

## Architecture Notes

Keep shared UI reusable and typed. Use BaseLib for ordinary product UI, CanvasLib for graph/canvas/calendar surfaces, OverlayLib for floating windows, WebGlLib for WebGL concepts, and sandbox projects only for demos or proof.

CanvasLib must not duplicate generic floating-window lifecycle logic. `canvas-floating-window.js` is a compatibility shim; normal generated assets load OverlayLib first and alias Canvas floating-window behavior to OverlayLib's plain JavaScript runtime.

The Canvas benchmark sandbox route is draw-cost and route-health evidence only. It is not renderer-migration approval and does not replace workbench, calendar, accessibility, export, or floating-window validation.

## Related Docs

- Repository overview: `README.md` at this repo root
- Runtime asset map: `Canvas/README.md`
- Change request notes: `Requests/README.md`
