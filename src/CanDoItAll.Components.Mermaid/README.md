# CanDoItAll.Components.Mermaid

## Purpose

Mermaid renders authored flowcharts, sequence diagrams, and architecture maps inside a Blazor application. It wraps Mermaid with source normalization, pan/zoom controls, typed render errors, render results, node-click events, and the static assets a host needs.

## Quick start

Register the renderer, include its assets once in the document head, then provide Mermaid source to `MermaidDiagram`:

```csharp
// Program.cs
builder.Services.AddCanDoItAllMermaid();
```

```razor
@using CanDoItAll.Components.Mermaid

@* App.razor <head> *@
<MermaidHeadAssets />

<MermaidDiagram Title="Approval flow"
                Source="""
                    flowchart LR
                        Draft --> Review --> Approved
                    """
                NodeClicked="HandleNodeClicked" />
```

Use the `Rendered` and `Error` callbacks when a page needs to react to diagram lifecycle or explain syntax problems. The Sandbox `/groups/mermaid` route provides a live reference.

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

Use this component for product-rendered Mermaid diagrams. Keep syntax guidance and authoring assistance outside the renderer; this library stays focused on rendering, options, normalization, and typed UI events.

## Security defaults

`MermaidDiagramOptions` defaults to Mermaid's `strict` security level and disables HTML labels. Only opt into `SecurityLevel = "loose"` or `HtmlLabels = true` for diagram source that your application fully trusts; those options allow Mermaid to emit richer HTML and interactive content into the rendered SVG surface.

## Related Docs

- [Repository overview](../../README.md)
- [Sandbox Mermaid examples](../../samples/CanDoItAll.Components.Sandbox/README.md)
