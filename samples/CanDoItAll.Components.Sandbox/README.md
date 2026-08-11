# CanDoItAll.Components.Sandbox

Package version: `0.1.0`.

## Purpose

The Sandbox is the living visual catalog for CanDoItAll.Components. It lets developers preview components in realistic compositions, compare happy/dense/empty scenarios, and capture browser-level regression evidence before taking a dependency into a product page.

Run it locally with:

```powershell
dotnet run --project samples/CanDoItAll.Components.Sandbox/CanDoItAll.Components.Sandbox.csproj
```

Then open the local URL shown by ASP.NET Core. The home page links to the catalog groups; focused routes include `/groups/charts`, `/groups/mermaid`, `/groups/qr`, `/groups/overlays`, and `/groups/canvas`.

## Validation Checklist

Every standard group needs happy, dense, and empty-state proof (a screenshot or a manual walkthrough) before it's considered signed off. Use these questions as the review checklist:

- Can I read all texts properly?
- Will I like and understand this UI or layout as a new user?
- Is there any too large component, gap, or visual disruption?
- Do we use proper shared components instead of ad-hoc markup?
- Do we use available space properly?
- Can the page be understood by scanning headings only?
- Is the hierarchy clear without decorative styling?
- Do focus, hover, disabled, loading, and empty states read clearly?
- On mobile, does the first viewport orient the user quickly?
- On desktop, are we avoiding dead horizontal space and accidental narrow columns?

The same list backs the "Validation" tab in each group page's proof dialog (`SandboxCatalogRegistry.ValidationQuestions`).

## Coverage Focus

Each group page has its own coverage notes — the areas the happy/dense/empty screenshots need to make obvious. Toggle **Coverage** in the top toolbar (next to the theme switch) to show that group's notes at the top of the page. If a note's proof looks weak or uncertain, tune spacing, hierarchy, or variant usage rather than adding new markup.

## Project Type

- SDK: `Microsoft.NET.Sdk.Web`
- Target framework(s): `net10.0`
- Validation command:

```powershell
dotnet build samples/CanDoItAll.Components.Sandbox/CanDoItAll.Components.Sandbox.csproj
```

## References

Project references:

- `../CanDoItAll.Components.BaseLib/CanDoItAll.Components.BaseLib.csproj`
- `../CanDoItAll.Components.CanvasLib/CanDoItAll.Components.CanvasLib.csproj`
- `../CanDoItAll.Components.Common/CanDoItAll.Components.Common.csproj`

Framework references:

- None

Direct package references:

- None

## Canvas and floating-window examples

- `/groups/overlays` demonstrates `OverlayWindow` above a normal BaseLib review frame. The window is bounded by the frame and respects a safe-top toolbar.
- `/groups/canvas` demonstrates `CanvasFloatingWindow` inside `CanvasWorkbench.OverlayContent`, where its geometry and visibility remain part of typed workbench state. It also includes an overlap scenario for activation-order proof.

Use these routes to understand the difference between a page-local supporting tool and a canvas-owned inspector. The [Canvas guide](../../docs/canvas/README.md) explains the architecture behind the examples.

## Overlay Service Examples

The `/groups/overlays` route mounts and exercises BaseLib overlay services:

- `DialogService` examples cover compact, wide, full, backdrop-locked, and returned-object dialogs.
- `TooltipService` examples prove host-mounted tooltip rendering from a local trigger with configurable placement.
- `NotificationService` examples show service-triggered toasts through the shared layout host, including non-default positions.

The sandbox layout mounts `<DialogHost />`, `<Tooltip />`, and `<Notification />` once so pages can focus on service calls instead of overlay plumbing.

Use the overlay examples as the visual proof target for position changes. Notifications should be checked at the positions that matter to the consuming workflow, especially `TopRight`, `TopCenter`, `BottomCenter`, and any side-aligned stack used near list/detail or rail layouts. Tooltip checks should include the requested `TooltipPosition` value and at least one constrained viewport when using corner or edge placements such as `TopLeft`, `BottomRight`, `LeftTop`, or `RightBottom`.

The sandbox examples should stay aligned with the Components MCP guidance: choose positions that avoid covering primary controls, keep notification alerts on the compact X close control, and use Playwright snapshots or locator checks for non-default overlay placement.

## Related Docs

- [Repository overview](../../README.md)
- [Canvas workspace guide](../../docs/canvas/README.md)
