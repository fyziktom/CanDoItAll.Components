# SB08 Semantic Invariants

## SB08-INV-CANVAS-SCENARIOS

- `route groups/canvas` must render the happy-path, dense-content, empty-state, disabled-state, loading-state, and long-text scenarios without console warnings, console errors, or page errors.
- Each scenario must expose the Canvas workbench runtime, calendar runtime, floating-window runtime alias, expected node/event counts, accessibility mirror rows when content exists, stable workbench/calendar/toolbar bounds, and no lateral overflow.
- Evidence: `bundle://proof/SB08/transcripts/playwright-sandbox-matrix.txt` and `bundle://proof/SB08/matrix-results.json`.

## SB08-INV-ROUTE-VIEWPORTS

- Canvas, Canvas benchmark, and Overlays must remain usable at 1920x1080, 1366x900, 1024x768, and 390x844.
- Required checks include measured bounds, no lateral overflow beyond the tolerance, route screenshots, and no WebGL route coverage.
- Evidence: `bundle://proof/SB08/transcripts/screenshot-inventory.txt`.

## SB08-INV-CANVAS-INTERACTIONS

- Canvas smoke interactions must exercise zoom, context menu open, calendar next navigation, and Canvas floating-window minimize, expand, hide, and show.
- The proof must assert state changes rather than only route load.
- Evidence: `bundle://proof/SB08/screenshots/matrix/canvas-interaction-context-menu.png`, `bundle://proof/SB08/screenshots/matrix/canvas-interaction-floating-window-shown.png`, and `bundle://proof/SB08/matrix-results.json`.

## SB08-INV-BENCHMARK-SCOPE

- `route groups/canvas/benchmark` is route-health and draw-cost evidence only; it is not renderer-migration approval and not Canvas feature parity proof.
- The shipped workbench preview must visibly paint its current pure-JS canvas layers, and the standalone prototype canvas must be nonblank.
- The page copy must avoid stale "retained DOM-SVG" claims and clearly say the prototype omits accessibility, overlay, export, and editor parity.
- Evidence: `bundle://proof/SB08/screenshots/matrix/benchmark-viewport-desktop-1366.png`, `bundle://proof/SB08/transcripts/source-assertions-sandbox-matrix.txt`, and `bundle://proof/SB08/matrix-results.json`.

## SB08-INV-OVERLAY-MATRIX

- `route groups/overlays scenario long-text` must keep the floating window inside its host frame and below the safe-top area through visible, minimized, restored, hidden, and shown states.
- The proof must wait for actual bounded geometry after remount, not just DOM presence.
- Evidence: `bundle://proof/SB08/screenshots/matrix/overlay-viewport-desktop-1366.png` and `bundle://proof/SB08/matrix-results.json`.

## Reopen Decisions

- The benchmark preview was reopened during SB08 because the shipped preview was visually collapsed/misleading. It was fixed by giving the benchmark preview layout full width, using the shared workbench default viewport path, and updating public benchmark copy.
- The overlay hide/show proof was reopened because immediate DOM visibility could precede bounded geometry. The verifier now waits for the true safe-top/container invariant before summarizing state.
- No SB03-SB07 source reopen remains after the final matrix pass.


## Validator Contract Summary

- Invariant ID: `SB08-INV-CANVAS-SCENARIOS`
- Source raw note: RAW01, RAW03, RAW04, RAW05, RAW06, RAW07.
- Expected behavior: Canvas, benchmark, and overlay sandbox routes must have real browser matrix proof without WebGL scope.
- Disallowed shallow implementation: Route-load-only proof, blank benchmark canvases, unbounded overlay geometry, stale benchmark language, or hidden console errors.
- Failing-first test: N/A process proof exemption; SB08 reopened matrix defects during execution and captured the repaired verifier proof.
- Passing test: `bundle://proof/SB08/transcripts/playwright-sandbox-matrix.txt`
- Changed source files: `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/CanvasBenchmark.razor`, `repo://src/CanDoItAll.Components.Sandbox/CanvasBenchmarkSamples.cs`, `repo://src/CanDoItAll.Components.Sandbox/wwwroot/js/canvasBenchmarkPage.js`, and `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB08/verify-sandbox-matrix.cjs`.
- Production assertions: `bundle://proof/SB08/transcripts/source-assertions-sandbox-matrix.txt`
- Red-team negative case: `bundle://proof/SB08/transcripts/playwright-sandbox-matrix.txt` rejects missing runtimes, blank previews, WebGL route inclusion, unsafe overlay bounds, and console warnings/errors/pageerrors.
- Downstream dependency check: SB09 package/docs proof and SB10 final closure can depend on `bundle://proof/SB08/manifest.md`.


