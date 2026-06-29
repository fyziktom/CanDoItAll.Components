# SB05 Semantic Invariants

## Invariant Scenario Coverage

- Invariant ID: `SB05-INV-SCENARIO-COVERAGE`
- Source raw note: RAW03 requires true Canvas validation and RAW05 requires preserving functionality.
- Expected behavior: `route groups/canvas` renders meaningful workbench content for happy-path, dense-content, empty-state, disabled-state, and long-text scenarios.
- Disallowed shallow implementation: Only loading the happy path or asserting HTTP success without checking node counts, projected hot zones, toolbar/stage bounds, accessibility mirror counts, screenshots, and console quality.
- Failing-first test: The verifier rejects wrong scenario node counts, missing projected hot zones for non-empty scenes, missing quick-create state, missing mirror content, insufficient bounds, horizontal overflow, and console warnings/errors/pageerrors.
- Passing test: `bundle://proof/SB05/transcripts/playwright-workbench.txt` records five scenario passes and `bundle://proof/SB05/browser-actions.json` records measured bounds and state for each scenario.
- Changed source files: `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/05-viewport-and-events.js`, `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/07a-runtime-interaction-router.js`, and `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB05/verify-workbench.cjs`.
- Production assertions: `bundle://proof/SB05/transcripts/source-assertions-workbench.txt` verifies sandbox scenario construction and workbench source hooks.
- Red-team negative case: The verifier fails if dense content drops below six nodes, disabled-state exposes quick create, empty-state renders stale mirror items, or any scenario creates browser warnings/errors/pageerrors.
- Downstream dependency check: SB06 calendar and SB07 floating-window proof can rely on stable `route groups/canvas` scenario selection.

## Invariant Viewports

- Invariant ID: `SB05-INV-VIEWPORTS`
- Source raw note: RAW03 requires true validation and RAW05 requires preserving functionality across usable surfaces.
- Expected behavior: The workbench shell, toolbar, and stage remain visible at maximized desktop, 1366x900, 1024x768, and 390x844 without horizontal overflow.
- Disallowed shallow implementation: Desktop-only screenshots or full-page screenshots that do not measure bounds.
- Failing-first test: The verifier rejects missing selectors, shell/stage/toolbar bounds below thresholds, and mobile horizontal overflow above 24px.
- Passing test: `bundle://proof/SB05/transcripts/playwright-workbench.txt` records four viewport passes; `bundle://proof/SB05/screenshots/viewport-mobile-390.png` and `bundle://proof/SB05/screenshots/viewport-max-desktop.png` capture the extremes.
- Changed source files: `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB05/verify-workbench.cjs`.
- Production assertions: `bundle://proof/SB05/browser-actions.json` records `overflowX: 0` for the viewport matrix.
- Red-team negative case: Mobile proof would fail if the workbench collapsed, overflowed sideways, or lost its accessibility mirror.
- Downstream dependency check: SB08 can reuse these viewport thresholds in its broader matrix.

## Invariant Interactions

- Invariant ID: `SB05-INV-INTERACTIONS`
- Source raw note: RAW03 requires true Canvas validation and RAW05 requires preserved user-facing functionality.
- Expected behavior: Selection, context menu, quick create, drag/move, zoom, fit, minimap toggle, diagnostics toggle, help, settings, export, and clipboard hooks execute through the production pure JS workbench runtime.
- Disallowed shallow implementation: Mutating component state manually without invoking the browser runtime or skipping callbacks because screenshots look correct.
- Failing-first test: The verifier rejects missing hot zones, unreadable menus, failed synthetic drag, missing zoom/fit updates, failed diagnostics state toggles, unreadable overlays, small export payloads, and missing clipboard hook writes.
- Passing test: `bundle://proof/SB05/transcripts/playwright-workbench.txt` records the interaction pass; `bundle://proof/SB05/browser-actions.json` records selected node `foundations`, `didDrag: true`, PNG export length `168560`, and one clipboard write.
- Changed source files: `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/05-viewport-and-events.js`, `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/07a-runtime-interaction-router.js`, and `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB05/verify-workbench.cjs`.
- Production assertions: `bundle://proof/SB05/transcripts/source-assertions-workbench.txt` verifies callbacks, state publishing, drag, help, diagnostics, export, and clipboard source paths.
- Red-team negative case: The verifier waits for help to close after Escape and fails if the overlay blocks the settings button; this protects the keyboard-close hardening added in SB05.
- Downstream dependency check: SB07 can validate floating windows without re-proving the core workbench interaction layer.

## Invariant Keyboard Toolbar

- Invariant ID: `SB05-INV-KEYBOARD-TOOLBAR`
- Source raw note: RAW03 and RAW05 require maintainable interactions that still work through keyboard and toolbar paths.
- Expected behavior: Keyboard shortcuts operate when focus is inside the workbench shell, help opened by keyboard closes with Escape, and settings remains reachable afterward.
- Disallowed shallow implementation: Opening help through Blazor state and then bypassing the keyboard path with direct DOM removal.
- Failing-first test: The interaction proof originally exposed help staying open after Escape and intercepting the settings button; the production runtime now closes `state.helpOpen` before running generic Escape selection cleanup.
- Passing test: `bundle://proof/SB05/transcripts/playwright-workbench.txt` passes after `Escape` hides `.cw-help-card`, then opens and closes settings through visible controls.
- Changed source files: `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/05-viewport-and-events.js` and `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/07a-runtime-interaction-router.js`.
- Production assertions: `bundle://proof/SB05/transcripts/node-check-workbench-js.txt` proves the changed pure JS files parse, and `source-assertions-workbench.txt` finds the keyboard/help routing source.
- Red-team negative case: If future refactors remove the Escape help close path, the settings click is intercepted and SB05 fails.
- Downstream dependency check: SB08 keyboard/focus matrix can assert this behavior as a baseline.

## Invariant Accessibility Mirror

- Invariant ID: `SB05-INV-ACCESSIBILITY-MIRROR`
- Source raw note: RAW03 and RAW05 require true validation without losing accessible mirror content.
- Expected behavior: The accessibility mirror renders for every scenario, carries `data-surface-kind="canvas-workbench"`, and mirrors the scenario node count.
- Disallowed shallow implementation: Checking the visual canvas only while stale or missing mirror content goes unnoticed.
- Failing-first test: The verifier rejects missing `.cw-accessibility-mirror`, mismatched mirror item counts, and too-short mirror text for non-empty scenes.
- Passing test: `bundle://proof/SB05/browser-actions.json` records mirror item counts matching node counts for all five scenarios and all four viewport passes.
- Changed source files: `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB05/verify-workbench.cjs`.
- Production assertions: `bundle://proof/SB05/transcripts/source-assertions-workbench.txt` verifies the Razor mirror component, C# snapshot factory, and pure JS mirror runtime.
- Red-team negative case: Empty-state must keep zero mirror items while non-empty scenarios must keep meaningful mirror text.
- Downstream dependency check: SB09 docs can describe accessibility mirror behavior with artifact-backed proof.

## Invariant Export Clipboard

- Invariant ID: `SB05-INV-EXPORT-CLIPBOARD`
- Source raw note: RAW03 and RAW05 require functionality preservation, including browser-side export and clipboard interactions.
- Expected behavior: Workbench image export returns a real PNG payload and `Ctrl/Cmd+C` writes through the canvas clipboard hook.
- Disallowed shallow implementation: Mocking export output or checking only that the methods exist.
- Failing-first test: The verifier rejects export payloads under 1000 characters and missing clipboard writes.
- Passing test: `bundle://proof/SB05/browser-actions.json` records export prefix `iVBORw0KGgoA`, export length `168560`, and `clipboardWriteCount: 1`.
- Changed source files: `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB05/verify-workbench.cjs`.
- Production assertions: `bundle://proof/SB05/transcripts/source-assertions-workbench.txt` verifies `exportImageData`, `requestSceneImage`, and clipboard source paths.
- Red-team negative case: A stubbed export string or missing clipboard bridge would fail the interaction proof even if the page still renders.
- Downstream dependency check: SB09 package/API docs can cite proven export and clipboard behavior.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| Workbench scenario state | `repo://src/CanDoItAll.Components.Sandbox/SandboxCanvasSamples.cs` builds scenario surfaces | `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor` consumes scenario query state | `bundle://proof/SB05/browser-actions.json` records scenario state per route load | `bundle://proof/SB05/transcripts/playwright-workbench.txt` rejects wrong counts/bounds/mirror state |
| Workbench keyboard/help runtime | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/07a-runtime-interaction-router.js` handles keyboard routing | `repo://src/CanDoItAll.Components.CanvasLib/Components/Workbench/CanvasWorkbench.razor` receives `OnHelpToggled` | `bundle://proof/SB05/transcripts/node-check-workbench-js.txt` and `source-assertions-workbench.txt` verify parse/source paths | `bundle://proof/SB05/transcripts/playwright-workbench.txt` fails if Escape leaves help intercepting settings |
| Accessibility mirror | `repo://src/CanDoItAll.Components.CanvasLib/Canvas/Core/AccessibilityMirrorLayer.cs` creates snapshots | `repo://src/CanDoItAll.Components.CanvasLib/Components/Core/AccessibilityMirrorLayer.razor` renders mirror markup | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/accessibility-mirror-layer.js` updates/disposes mirror runtime | `bundle://proof/SB05/browser-actions.json` rejects missing or stale mirror counts |
| Export and clipboard | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/07-runtime-entry.js` exposes facade methods | `repo://src/CanDoItAll.Components.CanvasLib/Components/Workbench/CanvasWorkbench.razor` consumes JS interop and callbacks | `bundle://proof/SB05/browser-actions.json` records export and clipboard outcomes | `bundle://proof/SB05/transcripts/playwright-workbench.txt` fails on tiny export payload or missing clipboard write |

