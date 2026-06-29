# Canvas Floating Windows Publishing Map

Generated local date: `2026-06-29`

This markdown map is the durable publishing map for SB01. It is used instead of an xlsx workbook so the proof remains readable in source control and does not require Excel automation.

| Surface | Source References | Publishing Concern | Owning Subbundles |
|---|---|---|---|
| CanvasLib package | `repo://src/CanDoItAll.Components.CanvasLib`, `repo://src/CanDoItAll.Components.CanvasLib/CanDoItAll.Components.CanvasLib.csproj`, `repo://src/CanDoItAll.Components.CanvasLib/README.md` | Package/API/docs readiness, generated assets, static web assets, public state contracts | SB01, SB03, SB04, SB09, SB10 |
| OverlayLib package | `repo://src/CanDoItAll.Components.OverlayLib`, `repo://src/CanDoItAll.Components.OverlayLib/CanDoItAll.Components.OverlayLib.csproj`, `repo://src/CanDoItAll.Components.OverlayLib/README.md` | Generic floating-window ownership, package/API/docs readiness | SB01, SB02, SB07, SB09, SB10 |
| Workbench state and graph contracts | `repo://src/CanDoItAll.Components.CanvasLib/Canvas/Workbench`, `repo://src/CanDoItAll.Components.CanvasLib/Canvas/Graph` | Selection, serialization, layout, viewport, node/link state, callback contracts | SB03, SB05, SB08, SB09 |
| Workbench runtime JS/CSS | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench`, `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/css/workbench` | Plain browser JS runtime boundaries, asset order, lifecycle cleanup, no npm runtime dependency | SB04, SB05, SB08, SB10 |
| Canvas floating windows | `repo://src/CanDoItAll.Components.CanvasLib/Components/Core/CanvasFloatingWindow.razor`, `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/canvas-floating-window.js`, `repo://src/CanDoItAll.Components.CanvasLib/Canvas/Workbench/CanvasWorkbenchUiState.cs` | Wrapper ownership, state conversion, safe top/container behavior, visual proof | SB03, SB07, SB08, SB09 |
| Overlay floating windows | `repo://src/CanDoItAll.Components.OverlayLib/Components/Core/OverlayWindow.razor`, `repo://src/CanDoItAll.Components.OverlayLib/Models/OverlayWindowState.cs`, `repo://src/CanDoItAll.Components.OverlayLib/wwwroot/js/runtime/overlay-window.js`, `repo://src/CanDoItAll.Components.OverlayLib/wwwroot/css/overlay-window.css` | Generic lifecycle, geometry, drag/resize/minimize/restore/reset/hide behavior | SB02, SB07, SB08, SB09 |
| Calendar | `repo://src/CanDoItAll.Components.CanvasLib/Components/Calendar`, `repo://src/CanDoItAll.Components.CanvasLib/Canvas/Calendar`, `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/calendar` | Selection, date/time range, CRUD bridge, editor, export, plain JS runtime | SB03, SB06, SB08, SB09, SB10 |
| Preview components | `repo://src/CanDoItAll.Components.CanvasLib/Components/Graph`, `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/preview`, `repo://src/CanDoItAll.Components.Sandbox/Components/Canvas` | Maintainable examples, preview browser proof, no runtime dependency drift | SB04, SB06, SB08, SB09 |
| Generated asset tooling | `repo://tools/canvaslib/asset-manifest.json`, `repo://tools/canvaslib/build-assets.cjs`, `repo://tools/canvaslib/verify-assets.cjs`, `repo://src/CanDoItAll.Components.CanvasLib/Components/Shared/Assets` | Generated include synchronization; no hand edits | SB04, SB09, SB10 |
| Sandbox proof routes | `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor`, `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/CanvasBenchmark.razor`, `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Overlays.razor`, `repo://src/CanDoItAll.Components.Sandbox/SandboxCatalogRegistry.cs` | Browser scenarios, screenshot proof, route matrix, benchmark route health only | SB05, SB06, SB07, SB08 |
| Tests and approvals | `repo://tests/CanDoItAll.Components.BaseLib.Tests`, `repo://tests/CanDoItAll.Components.BaseLib.Tests/fixtures/approvals` | Contract tests, public API approvals, package content approvals | SB02, SB03, SB09, SB10 |
| WebGL boundary | `repo://src/CanDoItAll.Components.WebGlLib`, `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests` | Out of scope. May be cited only as prior proof pattern or negative source assertion. | SB01, SB08, SB09, SB10 |

## Runtime Dependency Boundary

- Canvas, floating-window, calendar, preview, and related interactive runtime code must remain pure browser JavaScript plus C# and Razor.
- Do not add npm runtime dependencies for these surfaces.
- npm/Node usage is allowed only for existing Tailwind, generated asset, test, and browser-validation tooling.

