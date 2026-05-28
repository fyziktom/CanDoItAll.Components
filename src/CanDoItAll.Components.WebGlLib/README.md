# CanDoItAll.Components.WebGlLib

Package version: `0.1.0`.

## Purpose

WebGL workbench concept runtime and typed scene/component contracts.

## Project Type

- SDK: `Microsoft.NET.Sdk.Razor`
- Target framework(s): `net10.0`
- Validation command:

```powershell
dotnet build src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj
```

## References

Project references:

- `../CanDoItAll.Components.OverlayLib/CanDoItAll.Components.OverlayLib.csproj`

Framework references:

- None

Direct package references:

- `Microsoft.AspNetCore.Components.Web (10.0.4)`

## Architecture Notes

Keep shared UI reusable and typed. Use BaseLib for ordinary product UI, CanvasLib for graph/canvas surfaces, OverlayLib for floating windows, WebGlLib for WebGL concepts, and sandbox projects only for demos or proof.

## Related Docs

- Repository overview: `README.md` at this repo root
- Main repo shared component docs: `C:\repositories\CanDoItAll\docs\ui-shared-components\README.md`
