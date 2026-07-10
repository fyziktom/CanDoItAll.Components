# CanvasLib runtime assets

This note is for contributors who need to change CanvasLib's shipped browser assets. For application setup, workbench composition, and floating-window guidance, start with the [Canvas guide](../../../docs/canvas/README.md).

CanvasLib is the shared runtime for canvas workbenches, canvas overlays, and interactive calendars. Keep shared browser behavior here and preserve its typed public contracts so consuming Blazor applications can upgrade without rebuilding their workspace infrastructure.

## Runtime Asset Map

CanvasLib runtime assets are plain browser JavaScript and CSS shipped as static web assets. Do not add npm runtime dependencies for workbench, floating-window, calendar, preview, or related interactive behavior. npm/Node is reserved for existing Tailwind, generated asset, test, and browser-validation tooling.

Generated asset includes are owned by `tools/canvaslib/asset-manifest.json`, `tools/canvaslib/build-assets.cjs`, and `tools/canvaslib/verify-assets.cjs`. Regenerate `CanvasLibHeadAssets.razor` and `CanvasLibBodyAssets.razor` through the generator; do not hand-edit them.

| Area | Source root | Public surface | Ownership |
|---|---|---|---|
| Workbench services | `wwwroot/js/services` | Selection, text measurement, viewport, and animation helpers | Shared browser services used by the retained workbench runtime. |
| Workbench runtime | `wwwroot/js/runtime/workbench` | `CanDoItAll.canvasWorkbench` and related host behavior | Retained Canvas workbench rendering, layout, events, and interop lifecycle. |
| Accessibility mirror | `wwwroot/js/runtime/accessibility-mirror-layer.js` | Accessibility mirror DOM synchronization | Browser-side support for screen-reader-friendly workbench content. |
| Canvas floating window | `wwwroot/js/runtime/canvas-floating-window.js` | `CanDoItAll.canvasFloatingWindow` | Compatibility shim for older direct asset loading. Normal generated Canvas assets load OverlayLib first, so `CanDoItAll.canvasFloatingWindow` aliases `CanDoItAll.overlayWindow` and generic lifecycle behavior stays in OverlayLib. |
| Preview components | `wwwroot/js/preview` | Preview-only component behavior | Sandbox/package preview behavior, not a replacement renderer. |
| Calendar | `wwwroot/js/calendar` | `CanDoItAll.canvasCalendar` and calendar support modules | Calendar controller, editor, toolbar, rendering, export, and bridge behavior. |
| Workbench CSS | `wwwroot/css/workbench` | CanvasLib static styles | Shell, toolbar/window chrome, panels, scene/nodes, overlays/composer, motion/responsive styles. |
