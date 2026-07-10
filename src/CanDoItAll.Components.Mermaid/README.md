# CanDoItAll.Components.Mermaid

Package version: `0.1.0`.

## Purpose

Shared Razor Mermaid diagram component with source normalization, render result models, render error models, node-click event arguments, and Mermaid head assets.

## Project Type

- SDK: `Microsoft.NET.Sdk.Razor`
- Target framework(s): `net10.0`
- Validation command:

```powershell
dotnet build src/CanDoItAll.Components.Mermaid/CanDoItAll.Components.Mermaid.csproj
```

## References

Project references:

- None

Framework references:

- None

Direct package references:

- `Microsoft.AspNetCore.Components.Web (10.0.4)`

## Architecture Notes

Use this component for product-rendered Mermaid diagrams. Keep syntax guidance and authoring assistance in `CanDoItAll.Mcp.Mermaid`; this library should stay focused on rendering, options, normalization, and typed UI events.

## Security defaults

`MermaidDiagramOptions` defaults to Mermaid's `strict` security level and disables HTML labels. Only opt into `SecurityLevel = "loose"` or `HtmlLabels = true` for diagram source that your application fully trusts; those options allow Mermaid to emit richer HTML and interactive content into the rendered SVG surface.

## Related Docs

- Repository overview: `README.md` at this repo root
- Main repo shared component docs: `C:\repositories\CanDoItAll\docs\ui-shared-components\README.md`
