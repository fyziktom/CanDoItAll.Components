# CanDoItAll.Components.OverlayLib

OverlayLib supplies floating, task-focused windows for Blazor pages. Use it when a user needs a supporting tool - an inspector, filter panel, live status, or preview - without navigating away from the current work.

`OverlayWindow` is deliberately bounded: it lives inside a host frame, can respect a toolbar/safe-top area, and provides consistent drag, resize, minimize, reset, hide, and show behavior. It complements normal page content; it should not become a replacement for page navigation or an unbounded desktop-window imitation.

## Add it once

Import the namespace and include the generated assets in the host document. This has no npm runtime dependency.

```razor
@using CanDoItAll.Components.OverlayLib

@* App.razor *@
<head>
    ...
    <OverlayLibHeadAssets />
</head>
<body>
    ...
    <OverlayLibBodyAssets IncludeRuntimeAssets="true" />
</body>
```

## A bounded floating inspector

The host frame must be positioned and have a meaningful size. Pass selectors for the container and, when present, the toolbar that the window must not cover. Store `OverlayWindowState` in the page or feature state; the `StateChanged` callback is where drag, resize, and visibility changes come back to your application.

```razor
<div class="relative min-h-[28rem] overflow-hidden rounded-2xl border"
     data-testid="review-frame">
    <header class="relative z-10 border-b p-3" data-testid="review-toolbar">
        Review tools
    </header>

    <section class="p-6">Your ordinary page content remains here.</section>

    @if (inspector.IsVisible)
    {
        <OverlayWindow WindowId="review-inspector"
                       Title="Inspector"
                       Summary="Supporting context for this review."
                       State="@inspector"
                       StateChanged="HandleInspectorChanged"
                       DefaultPlacement="top-right"
                       ContainerSelector="[data-testid='review-frame']"
                       SafeTopSelector="[data-testid='review-toolbar']">
            <TextBlock TextStyle="TextStyle.Body2"
                       Value="This panel can move without taking the user away from the page." />
        </OverlayWindow>
    }
</div>

@code {
    private OverlayWindowState inspector = new();

    private Task HandleInspectorChanged(OverlayWindowState next)
    {
        inspector = OverlayWindowState.Normalize(next);
        return Task.CompletedTask;
    }
}
```

![OverlayLib inspector over a bounded ordinary page frame](../../docs/assets/overlay-window-page.png)

## OverlayLib and CanvasLib

OverlayLib owns the generic window lifecycle and browser runtime. `CanvasLib` uses it through `CanvasFloatingWindow`, adding a canvas-specific state type and default stage boundaries. Use `OverlayWindow` on ordinary pages; use `CanvasFloatingWindow` inside `CanvasWorkbench.OverlayContent` when the panel belongs to a workbench. See the [Canvas guide](../../docs/canvas/README.md) for that composition.

## See it running

The Sandbox route `/groups/overlays` renders a standard BaseLib review frame with a real `OverlayWindow`. It is the visual reference for bounded placement, safe-top behavior, and hide/show/minimize flows. The companion Canvas example lives at `/groups/canvas`.

## Development

```powershell
dotnet build src/CanDoItAll.Components.OverlayLib/CanDoItAll.Components.OverlayLib.csproj --no-restore
dotnet test tests/CanDoItAll.Components.BaseLib.Tests/CanDoItAll.Components.BaseLib.Tests.csproj --filter FullyQualifiedName~CanvasOverlayPublishingApprovalTests --no-restore
```

Targets `net10.0`. The package references BaseLib and `Microsoft.AspNetCore.Components.Web`; it requires no npm runtime package.
