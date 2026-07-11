# CanDoItAll.Components.CanvasLib

CanvasLib is a small framework for building stateful, interactive workspace surfaces in Blazor. Use it when a normal document-style page is no longer enough: your users need to pan and zoom a node graph, inspect context without losing their place, compose work on a stage, or work through a rich calendar.

It is deliberately more than a collection of isolated controls. The library provides the contracts, workbench host, browser runtime, accessibility mirror, interaction callbacks, canvas-specific overlays, and host asset order that let an application own the business state while CanvasLib owns the interaction mechanics.

![CanvasLib layered model](../../docs/assets/canvas-layered-model.png)

## Is CanvasLib the right fit?

Use CanvasLib for an authoring workspace, graph, process map, planning board, visual inspector, or dense calendar where state and interaction belong together. Use `BaseLib` for standard forms, pages, and cards. Use `OverlayLib` directly when a floating tool belongs to a normal page rather than a canvas. CanvasLib already depends on and composes both libraries where appropriate.

## Core concepts

| Concept | Your application owns | CanvasLib owns |
| --- | --- | --- |
| `CanvasWorkbenchSurface` | Nodes, links, mode, chrome, and the current `CanvasWorkbenchUiState`. | Rendering a workbench from that typed surface. |
| `CanvasWorkbench` | Handling selection, movement, context actions, editing, clipboard, and persistence callbacks. | Canvas lifecycle, pan/zoom interaction, toolbar, accessibility mirror, and runtime interop. |
| `CanvasFloatingWindow` | `CanvasWorkbenchWindowState` and the inspector's content. | A canvas-bounded, draggable, resizable window backed by OverlayLib. |
| `CanvasCalendar` | Events, commands, and save/export/search callbacks. | Interactive calendar surface and its browser-side behavior. |

The boundary is intentional: do not place business rules in JavaScript, and do not rebuild the workbench shell in every consuming page.

## Add CanvasLib to a host

Reference the package and add the namespace to `_Imports.razor` or the consuming component:

```razor
@using CanDoItAll.Components.CanvasLib
```

Add generated assets once in the host document. The Canvas asset components include OverlayLib in the required order; do not add hand-maintained copies of their runtime scripts.

```razor
@* App.razor *@
<head>
    ...
    <CanvasLibHeadAssets />
</head>
<body>
    ...
    <CanvasLibBodyAssets IncludeRuntimeAssets="true"
                         IncludePreviewAssets="false"
                         IncludeCalendarAssets="true" />
</body>
```

## Minimal workbench

Create a `CanvasWorkbenchSurface` in application code, render it, and react to the typed events. Keep the surface in your page, feature state store, or domain layer so it can be saved and restored like any other application state.

```razor
<CanvasWorkbench Surface="@surface"
                 SelectionChanged="HandleSelectionChanged"
                 NodesMoved="HandleNodesMoved"
                 ContextActionRequested="HandleContextAction">
    <ToolbarLeftContent>
        <StatusBadge Text="Planning board" Tone="info" />
    </ToolbarLeftContent>
</CanvasWorkbench>

@code {
    private readonly CanvasWorkbenchSurface surface = new()
    {
        SurfaceId = "planning-board",
        Nodes = [ /* map application records to CanvasWorkbenchNode */ ],
        Links = [ /* map relationships to CanvasWorkbenchLink */ ],
        UiState = new CanvasWorkbenchUiState()
    };

    private Task HandleSelectionChanged(CanvasWorkbenchSelectionChangedEventArgs change)
        => Task.CompletedTask;

    private Task HandleNodesMoved(CanvasWorkbenchNodesMovedEventArgs change)
        => Task.CompletedTask;

    private Task HandleContextAction(CanvasWorkbenchContextActionRequest request)
        => Task.CompletedTask;
}
```

## Floating inspector in a canvas

Render a `CanvasFloatingWindow` inside `OverlayContent`, and keep its state with the workbench UI state when you need persistence. The wrapper adapts `CanvasWorkbenchWindowState` to OverlayLib's generic window runtime; it does not create a competing window lifecycle.

```razor
<CanvasWorkbench Surface="@surface">
    <OverlayContent>
        @if (inspector.IsVisible)
        {
            <CanvasFloatingWindow WindowId="selection-inspector"
                                  Title="Selection"
                                  Kicker="Inspector"
                                  Summary="Context for the selected node."
                                  State="@inspector"
                                  StateChanged="HandleInspectorChanged">
                <TextBlock TextStyle="TextStyle.Body2"
                           Value="Inspector content stays near the canvas." />
            </CanvasFloatingWindow>
        }
    </OverlayContent>
</CanvasWorkbench>

@code {
    private CanvasWorkbenchWindowState inspector = new();

    private Task HandleInspectorChanged(CanvasWorkbenchWindowState next)
    {
        inspector = CanvasWorkbenchWindowState.Normalize(next);
        return Task.CompletedTask;
    }
}
```

Use the `WindowStates` dictionary on `CanvasWorkbenchUiState` when several windows should survive a route change or a saved workspace. Give each window a stable `WindowId` and dictionary key.

![Canvas floating inspector over the workbench stage](../../docs/assets/canvas-floating-window.png)

## What to validate

The [Sandbox Canvas route](../../samples/CanDoItAll.Components.Sandbox/README.md#canvas-and-floating-window-examples) demonstrates the real workbench, the selected-node inspector, and overlapping windows. Validate selection, drag/pan, keyboard interactions, geometry persistence, minimize/hide/show behavior, and an accessible representation of the important canvas content, not only how the stage first renders.

## Development and package checks

CanvasLib targets `net10.0` and has no npm runtime dependency. Node tooling is used only to generate or verify assets, style components, and run browser proof.

```powershell
npm run canvaslib:verify-assets
dotnet build src/CanDoItAll.Components.CanvasLib/CanDoItAll.Components.CanvasLib.csproj --no-restore
dotnet test tests/CanDoItAll.Components.BaseLib.Tests/CanDoItAll.Components.BaseLib.Tests.csproj --filter FullyQualifiedName~CanvasOverlayPublishingApprovalTests --no-restore
```

For the static-runtime asset map and ownership notes, see [Canvas/README.md](Canvas/README.md). For the repository overview, see the [main README](../../README.md).
