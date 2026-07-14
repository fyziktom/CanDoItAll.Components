# CanvasLib runtime assets

This note is for contributors who need to change CanvasLib's shipped browser assets. For application setup, workbench composition, and floating-window guidance, start with the [Canvas guide](../../../docs/canvas/README.md).

CanvasLib is the shared runtime for canvas workbenches, canvas overlays, and interactive calendars. Keep shared browser behavior here and preserve its typed public contracts so consuming Blazor applications can upgrade without rebuilding their workspace infrastructure.

## Runtime Asset Map

CanvasLib runtime assets are plain browser JavaScript and CSS shipped as static web assets. Do not add npm runtime dependencies for workbench, floating-window, calendar, preview, or related interactive behavior. npm/Node is reserved for existing Tailwind, generated asset, test, and browser-validation tooling.

Generated asset includes are owned by `tools/canvaslib/asset-manifest.json`, `tools/canvaslib/build-assets.cjs`, and `tools/canvaslib/verify-assets.cjs`. Regenerate `CanvasLibHeadAssets.razor` and `CanvasLibBodyAssets.razor` through the generator; do not hand-edit them.

| Area | Source root | Public surface | Ownership |
|---|---|---|---|
| Workbench services | `wwwroot/js/services` | Selection, text measurement, viewport, and animation helpers | Shared browser services used by the retained workbench runtime. |
| Generic canvas runtime | `wwwroot/js/runtime/canvas-runtime.js` | `CanDoItAll.canvasRuntime` | Low-level DPR sizing, render invalidation, pointer capture, hit regions, and PNG export for reusable canvas components. It owns no application or scheduling state. |
| Workbench runtime | `wwwroot/js/runtime/workbench` | `CanDoItAll.canvasWorkbench` and related host behavior | Retained Canvas workbench rendering, layout, events, and interop lifecycle. |
| Accessibility mirror | `wwwroot/js/runtime/accessibility-mirror-layer.js` | Accessibility mirror DOM synchronization | Browser-side support for screen-reader-friendly workbench content. |
| Canvas floating window | `wwwroot/js/runtime/canvas-floating-window.js` | `CanDoItAll.canvasFloatingWindow` | Compatibility shim for older direct asset loading. Normal generated Canvas assets load OverlayLib first, so `CanDoItAll.canvasFloatingWindow` aliases `CanDoItAll.overlayWindow` and generic lifecycle behavior stays in OverlayLib. |
| Preview components | `wwwroot/js/preview` | Preview-only component behavior | Sandbox/package preview behavior, not a replacement renderer. |
| Calendar | `wwwroot/js/calendar` | `CanDoItAll.canvasCalendar` and calendar support modules | Calendar controller, editor, toolbar, rendering, export, and bridge behavior. |
| Workbench CSS | `wwwroot/css/workbench` | CanvasLib static styles | Shell, toolbar/window chrome, panels, scene/nodes, overlays/composer, motion/responsive styles. |

## Generic canvas runtime

`CanvasLibBodyAssets IncludeRuntimeAssets="true"` exposes `window.CanDoItAll.canvasRuntime`. This is the low-level boundary for reusable components that need CanvasLib mechanics without adopting `CanvasWorkbenchSurface`.

```javascript
const runtime = window.CanDoItAll.canvasRuntime;
const regions = runtime.createHitRegionRegistry();
const surface = runtime.createSurface({
    canvas,
    resizeTarget: host,
    onRender({ context, size }) {
        regions.clear();
        context.clearRect(0, 0, size.width, size.height);
        regions.add({ x: 12, y: 12, width: 120, height: 32 }, { id: "task-1" });
    }
});
const pointers = runtime.createPointerRouter({
    element: canvas,
    onPointerDown(change) {
        return regions.find(change.point.x, change.point.y) !== null;
    },
    onPointerMove(change) {
        surface.requestRender();
    },
    onPointerUp(change) {
        surface.requestRender();
    },
    onPointerCancel(change) {
        surface.requestRender();
    }
});
```

The public runtime surface is:

- `createSurface({ canvas, resizeTarget?, onRender, onResize?, maxPixelRatio? })`
- `createHitRegionRegistry()` with `clear()`, `add(bounds, metadata)`, and topmost-first `find(x, y)`
- `createPointerRouter({ element, coordinateElement?, onPointerDown, onPointerMove, onPointerUp, onPointerCancel })`
- `renderToPngDataUrl({ width, height, pixelRatio?, background?, draw })`
- `downloadDataUrl(dataUrl, fileName)` for validated PNG data URLs and `.png` file names

`CanvasSurface` exposes `canvas`, `context`, `size`, `measure()`, `requestRender()`, `pointFromEvent()`, `toPngDataUrl()`, and `dispose()`. Render and pointer callbacks are synchronous. Pointer changes include the raw event, normalized coordinates, start point, per-event delta, and total delta. Returning `false` from `onPointerDown` declines the gesture and releases capture.

The router uses element pointer capture instead of window-level move/up listeners. Consumers must dispose surfaces and routers explicitly. Missing browser APIs, invalid dimensions, asynchronous callbacks, and invalid export inputs throw actionable errors; the runtime does not substitute hidden fallback behavior.

Keep scene interpretation, business rules, domain mutation, and persistence in the consuming .NET component or application. Browser-side preview state is transient and must be replaced by the authoritative model after an accepted command.
