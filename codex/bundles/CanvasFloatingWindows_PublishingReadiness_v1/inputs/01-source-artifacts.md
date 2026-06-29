# Source Artifacts

## Prior Bundle Pattern Inputs

- `repo://codex/bundles/StandardComponents_PublishingReadiness_v1/README.md`
- `repo://codex/bundles/StandardComponents_PublishingReadiness_v1/plan/01-phase-plan.md`
- `repo://codex/bundles/StandardComponents_PublishingReadiness_v1/reviews/01-execution-report.md`
- `repo://codex/bundles/StandardComponents_PublishingReadiness_v1/subbundles/08-layout-navigation-and-overlay-hardening/README.md`
- `repo://codex/bundles/StandardComponents_PublishingReadiness_v1/subbundles/11-full-playwright-visual-validation-matrix/README.md`
- `repo://codex/bundles/StandardComponents_PublishingReadiness_v1/subbundles/12-final-publishing-transfer-readiness-audit/README.md`
- `repo://codex/bundles/WebGlEngine_Stabilization_v17/README.md`
- `repo://codex/bundles/WebGlEngine_Stabilization_v17/plan/01-subbundle-index.md`
- `repo://codex/bundles/WebGlEngine_Stabilization_v17/subbundles/cp-a-review-checkpoint-a-proof-truthfulness/README.md`

## Canvas And Floating Window Source Inputs

- `repo://src/CanDoItAll.Components.CanvasLib`
- `repo://src/CanDoItAll.Components.CanvasLib/Canvas/README.md`
- `repo://src/CanDoItAll.Components.CanvasLib/Components/Core/CanvasFloatingWindow.razor`
- `repo://src/CanDoItAll.Components.CanvasLib/Components/Workbench/CanvasWorkbench.razor`
- `repo://src/CanDoItAll.Components.CanvasLib/Components/Calendar/CanvasCalendar.razor`
- `repo://src/CanDoItAll.Components.CanvasLib/Components/Shared/Assets/CanvasLibHeadAssets.razor`
- `repo://src/CanDoItAll.Components.CanvasLib/Components/Shared/Assets/CanvasLibBodyAssets.razor`
- `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js`
- `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/css`
- `repo://src/CanDoItAll.Components.OverlayLib`
- `repo://src/CanDoItAll.Components.OverlayLib/Components/Core/OverlayWindow.razor`
- `repo://src/CanDoItAll.Components.OverlayLib/Models/OverlayWindowState.cs`
- `repo://src/CanDoItAll.Components.OverlayLib/wwwroot/js/runtime/overlay-window.js`
- `repo://src/CanDoItAll.Components.OverlayLib/wwwroot/css/overlay-window.css`

## Sandbox, Tests, And Tooling Inputs

- `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor`
- `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/CanvasBenchmark.razor`
- `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Overlays.razor`
- `repo://src/CanDoItAll.Components.Sandbox/SandboxCatalogRegistry.cs`
- `repo://src/CanDoItAll.Components.Sandbox/SandboxCanvasSamples.cs`
- `repo://tests/CanDoItAll.Components.BaseLib.Tests/LayoutNavigationOverlayBehaviorTests.cs`
- `repo://tests/CanDoItAll.Components.BaseLib.Tests/StandardPublishingApprovalTests.cs`
- `repo://tools/canvaslib/build-assets.cjs`
- `repo://tools/canvaslib/verify-assets.cjs`
- `repo://tools/canvaslib/asset-manifest.json`
- `repo://package.json`
- `repo://Directory.Build.props`

## Tooling Observation

- The `candoitall_components` MCP transport closed during preparation when queried for Canvas/floating-window inventory. Execution should retry it before layout refactors, but this preparation uses repository files as the durable source of truth.
