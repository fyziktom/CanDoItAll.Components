# SB07 Semantic Invariants

## Invariant Runtime Ownership

- Invariant ID: `SB07-INV-RUNTIME-OWNERSHIP`
- Source raw note: RAW06 requires clearer maintainability and ownership for publishing, and RAW07 requires pure browser JavaScript without npm runtime dependency.
- Expected behavior: `CanvasFloatingWindow` delegates generic lifecycle behavior to `OverlayWindow`, and generated Canvas assets alias `CanDoItAll.canvasFloatingWindow` to `CanDoItAll.overlayWindow` when OverlayLib is loaded.
- Disallowed shallow implementation: Duplicating generic drag, resize, safe-top, visibility, minimized, reset, or z-index behavior in CanvasLib while claiming the ownership boundary is shared.
- Failing-first test: The SB07 browser verifier fails when `window.CanDoItAll.canvasFloatingWindow !== window.CanDoItAll.overlayWindow`; this would catch the pre-SB07 Canvas runtime overriding the OverlayLib facade after generated asset load.
- Passing test: `bundle://proof/SB07/transcripts/playwright-floating-windows.txt` records `RuntimeAlias: true` for Overlay and Canvas across all four viewports.
- Changed source files and hashes: `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/canvas-floating-window.js` SHA-256 `C30DEFD31A20F73ECE4382A833F6DD5133C42CF4F8A7CFDE3B935AB056F3851C`; `repo://src/CanDoItAll.Components.CanvasLib/Canvas/README.md` SHA-256 `766F9C055EC4721B328E5CB276CF2024FF2E45147B0ACFEA5AA360892CEE52E1`.
- Production assertions: `bundle://proof/SB07/transcripts/source-assertions-floating-windows.txt` verifies Canvas wrapper delegation, Canvas runtime alias shim, OverlayLib ownership, and generated asset ordering.
- Red-team negative case: Loading Canvas generated assets after OverlayLib must not replace the generic OverlayLib runtime with stale Canvas-only logic.
- Downstream dependency check: SB08 and SB09 can cite a single generic floating-window owner for route matrix and publishing docs.

## Invariant Overlay Lifecycle

- Invariant ID: `SB07-INV-OVERLAY-LIFECYCLE`
- Source raw note: RAW03 and RAW05 require true floating-window validation while preserving functionality.
- Expected behavior: OverlayWindow on `route groups/overlays` supports initial, minimized, restored, dragged, resized, reset, hidden, and shown states through real browser controls.
- Disallowed shallow implementation: Checking only the initial overlay render or mutating state directly without clicking the production header actions and sandbox show/reset controls.
- Failing-first test: The browser verifier rejects missing action buttons, failed minimized/restored classes, drag without geometry movement, desktop resize without geometry change, hide/show failures, clipping, lateral overflow, and console warnings/errors/pageerrors.
- Passing test: `bundle://proof/SB07/transcripts/playwright-floating-windows.txt` records Overlay pass results for max-desktop, desktop-1366, tablet-1024, and mobile-390.
- Changed source files and hashes: `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB07/verify-floating-windows.cjs` SHA-256 `D0090E963B9EA5326B1A9240C8706D621BFC8F3C6DE8FD5FDE665D935FC22661`.
- Production assertions: `bundle://proof/SB07/transcripts/source-assertions-floating-windows.txt` verifies OverlayWindow markup, OverlayWindowState, and OverlayLib runtime ownership.
- Red-team negative case: A visible but unmovable, unresizable, or unshowable overlay window fails before it can be accepted as a publishing-ready example.
- Downstream dependency check: SB08 can rely on `route groups/overlays` open-state lifecycle proof for matrix coverage.

## Invariant Canvas Lifecycle

- Invariant ID: `SB07-INV-CANVAS-LIFECYCLE`
- Source raw note: RAW03 and RAW05 require true Canvas floating-window validation and functionality preservation.
- Expected behavior: CanvasFloatingWindow on `route groups/canvas` supports initial, minimized, restored, dragged, resized, reset, hidden, and shown states while staying inside the workbench stage.
- Disallowed shallow implementation: Proving only OverlayLib and assuming the Canvas wrapper remains equivalent without exercising the Canvas container and state conversion path.
- Failing-first test: The browser verifier rejects missing Canvas inspector, missing show/reset proof controls, failed minimize/restore, drag without movement, desktop resize without geometry change, hide/show failures, clipping, lateral overflow, or console failures.
- Passing test: `bundle://proof/SB07/transcripts/playwright-floating-windows.txt` records Canvas pass results for max-desktop, desktop-1366, tablet-1024, and mobile-390.
- Changed source files and hashes: `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor` SHA-256 `51901A10315CEBE4FA7EF1D72B4A9988A2ABCC55C22B39E8252B44F4FCFA1D81`; `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB07/verify-floating-windows.cjs` SHA-256 `D0090E963B9EA5326B1A9240C8706D621BFC8F3C6DE8FD5FDE665D935FC22661`.
- Production assertions: `bundle://proof/SB07/transcripts/source-assertions-floating-windows.txt` verifies `ShowInspectorWindow`, `ResetInspectorWindow`, Canvas wrapper delegation, and state conversion source.
- Red-team negative case: Hiding the Canvas inspector must not require a route reload to return; the sandbox proof controls exercise the real state path.
- Downstream dependency check: SB08 can run Canvas route matrix proof without adding one-off show/reload workarounds.

## Invariant Safe Top And Container

- Invariant ID: `SB07-INV-SAFE-TOP-CONTAINER`
- Source raw note: RAW03 and RAW05 require true floating-window visual/behavior validation across desktop and mobile.
- Expected behavior: OverlayWindow and CanvasFloatingWindow stay inside their host frame, remain below the safe-top toolbar/strip, keep action buttons reachable, and avoid lateral overflow at 1920x1080, 1366x900, 1024x768, and 390x844.
- Disallowed shallow implementation: Full-page screenshots without measured bounds, or desktop-only proof that misses mobile frame compression.
- Failing-first test: The verifier rejects bounds outside frame, top above safe area, bottom outside frame, action hit areas under 28px, window width/height collapse, and lateral overflow above 24px.
- Passing test: `bundle://proof/SB07/transcripts/playwright-floating-windows.txt` records measured initial bounds and `overflowX: 0` for all surface/viewport combinations.
- Changed source files and hashes: `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB07/verify-floating-windows.cjs` SHA-256 `D0090E963B9EA5326B1A9240C8706D621BFC8F3C6DE8FD5FDE665D935FC22661`.
- Production assertions: `bundle://proof/SB07/browser-actions.json` stores measured frame, safe-top, window, action, z-index, resize, and overflow data for 64 lifecycle records.
- Red-team negative case: Mobile windows may narrow to fit the sandbox frame, but they must remain visible, bounded, and operable.
- Downstream dependency check: SB08 can treat the safe-top/container verifier as the baseline for broader route open-state matrix proof.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| Floating-window runtime facade | `repo://src/CanDoItAll.Components.OverlayLib/wwwroot/js/runtime/overlay-window.js` exports `overlayWindowApi` | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/canvas-floating-window.js` aliases to `root.overlayWindow` when present | `bundle://proof/SB07/transcripts/playwright-floating-windows.txt` records `RuntimeAlias: true` for both routes | Verifier rejects `canvasFloatingWindow !== overlayWindow` |
| Canvas wrapper state conversion | `repo://src/CanDoItAll.Components.CanvasLib/Components/Core/CanvasFloatingWindow.razor` converts Canvas state to Overlay state | `repo://src/CanDoItAll.Components.CanvasLib/Canvas/Workbench/CanvasWorkbenchUiState.cs` converts Overlay state back to Canvas state | `bundle://proof/SB07/transcripts/dotnet-test-floating-window-contracts.txt` passes focused state tests | Tests cover null/non-positive geometry and roundtrip normalization |
| Overlay lifecycle controls | `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Overlays.razor` renders show/reset and OverlayWindow controls | `repo://src/CanDoItAll.Components.OverlayLib/Components/Core/OverlayWindow.razor` consumes button actions and JS geometry callbacks | `bundle://proof/SB07/browser-actions.json` records Overlay lifecycle states | `bundle://proof/SB07/transcripts/playwright-floating-windows.txt` rejects failed minimize, restore, drag, resize, reset, hide, or show |
| Canvas lifecycle controls | `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor` renders show/reset inspector controls and CanvasFloatingWindow | `repo://src/CanDoItAll.Components.CanvasLib/Components/Core/CanvasFloatingWindow.razor` consumes OverlayWindow callbacks | `bundle://proof/SB07/browser-actions.json` records Canvas lifecycle states | `bundle://proof/SB07/transcripts/playwright-floating-windows.txt` rejects requiring route reload after hide or losing Canvas bounds |
| Safe-top/container bounds | `repo://src/CanDoItAll.Components.OverlayLib/wwwroot/js/runtime/overlay-window.js` resolves container and safe-top options | Overlay and Canvas sandbox routes consume frame/stage safe-top selectors | `bundle://proof/SB07/transcripts/playwright-floating-windows.txt` records bounds and overflow per viewport | `bundle://proof/SB07/transcripts/playwright-floating-windows.txt` rejects clipping, toolbar overlap, collapsed geometry, or lateral overflow |


