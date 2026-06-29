# CanvasLib Ownership

`CanDoItAll.Components.CanvasLib` is the canonical shared canvas implementation for active CanDoItAll consumers.

Current direct project references:
- `CanDoItAll.Components`
- `CanDoItAll.Components.Sandbox`
- `CanDoItAll.Mcp.Components`
- `CanDoItAll.Modules.Factory`
- `CanDoItAll.Modules.Workbench`
- `CanDoItAll.Web`

Rules:
- Put new shared canvas runtime work here.
- Keep public runtime behavior stable for ProjectStructure and PromptFactory.
- Treat other canvas trees as compatibility-only until a measured consolidation plan is approved.

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
