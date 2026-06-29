# SB07 Proof Manifest

Subbundle: `SB07 Canvas Floating Window Visual And Behavioral Hardening`

Status: `Completed`

Owned raw notes and requirements:

- RAW03, RAW05, RAW06, and RAW07.
- R04, R05, R10, R11, R12, and R14.

Semantic invariant contract:

- `bundle://proof/SB07/semantic-invariants.md`

Changed-file hashes:

- `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/canvas-floating-window.js` current SHA-256 `C30DEFD31A20F73ECE4382A833F6DD5133C42CF4F8A7CFDE3B935AB056F3851C`
- `repo://src/CanDoItAll.Components.CanvasLib/Canvas/README.md` current SHA-256 `766F9C055EC4721B328E5CB276CF2024FF2E45147B0ACFEA5AA360892CEE52E1`
- `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor` current SHA-256 `51901A10315CEBE4FA7EF1D72B4A9988A2ABCC55C22B39E8252B44F4FCFA1D81`
- `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB07/verify-floating-windows.cjs` current SHA-256 `D0090E963B9EA5326B1A9240C8706D621BFC8F3C6DE8FD5FDE665D935FC22661`
- Full current changed/proof hash transcript: `bundle://proof/SB07/transcripts/changed-file-hashes.txt`

Command transcripts:

- Sandbox build: `bundle://proof/SB07/transcripts/dotnet-build-sandbox.txt`
- Focused Canvas/Overlay state tests: `bundle://proof/SB07/transcripts/dotnet-test-floating-window-contracts.txt`
- Canvas asset verification: `bundle://proof/SB07/transcripts/npm-canvaslib-verify-assets.txt`
- Browser floating-window verifier: `bundle://proof/SB07/transcripts/playwright-floating-windows.txt`
- Node syntax check: `bundle://proof/SB07/transcripts/node-check-floating-window-js.txt`
- Source assertions: `bundle://proof/SB07/transcripts/source-assertions-floating-windows.txt`
- Anti-stub audit: `bundle://proof/SB07/transcripts/anti-stub-audit.txt`
- Semantic adequacy transcript: `bundle://proof/SB07/transcripts/semantic-adequacy.txt`

Failing-first / adversarial negative proof:

- Failing-first: N/A process proof exemption; SB07 relied on lifecycle verifier assertions and state-contract tests rather than preserving a separate failing command transcript.
- The SB07 verifier rejects the pre-SB07 ownership drift where generated Canvas assets could load OverlayLib and then replace `CanDoItAll.canvasFloatingWindow` with the older Canvas-only runtime. The final proof requires `RuntimeAlias: true` for both routes and every viewport.
- `bundle://proof/SB07/transcripts/playwright-floating-windows.txt` fails if Overlay or Canvas windows clip outside their frames, overlap safe-top areas, lose action buttons, fail minimize/restore/drag/resize/reset/hide/show, require route reload after hide, or produce console warnings/errors/pageerrors.

Passing / semantic positive proof:

- Passing browser transcript: `bundle://proof/SB07/transcripts/playwright-floating-windows.txt`
- Browser action record: `bundle://proof/SB07/browser-actions.json`
- Semantic positive proof transcript: `bundle://proof/SB07/transcripts/semantic-adequacy.txt`

Browser proof:

- Route matrix: `route groups/overlays scenario long-text` and `route groups/canvas scenario long-text`.
- Viewports: 1920x1080, 1366x900, 1024x768, and 390x844.
- States per surface/viewport: initial, minimized, restored, dragged, resized, reset, hidden, and shown.
- Representative screenshots: `bundle://proof/SB07/screenshots/overlay-desktop-1366-resized.png`, `bundle://proof/SB07/screenshots/overlay-mobile-390-minimized.png`, `bundle://proof/SB07/screenshots/canvas-desktop-1366-dragged.png`, and `bundle://proof/SB07/screenshots/canvas-mobile-390-initial.png`.
- Full screenshot list: `bundle://proof/SB07/transcripts/playwright-floating-windows.txt`.
- Console log: `bundle://proof/SB07/console-log.txt`
- Visual review: `bundle://proof/SB07/visual-review.md`

Source-level assertions:

- `bundle://proof/SB07/transcripts/source-assertions-floating-windows.txt` verifies Canvas wrapper delegation, Canvas runtime alias shim, OverlayLib ownership, sandbox proof controls, docs/assets ownership, and SB07 verifier invariants.
- `bundle://proof/SB07/transcripts/node-check-floating-window-js.txt` verifies the changed pure JS files and verifier parse successfully.

Anti-stub audit:

- `bundle://proof/SB07/transcripts/anti-stub-audit.txt` states no TODO, FIXME, HACK, NotImplementedException, stubbed, fakeImplementation, or placeholderImplementation matches in SB07 scoped production/proof files.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| Floating-window runtime facade | `repo://src/CanDoItAll.Components.OverlayLib/wwwroot/js/runtime/overlay-window.js` exports `overlayWindowApi` | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/canvas-floating-window.js` aliases to `root.overlayWindow` when present | `bundle://proof/SB07/transcripts/playwright-floating-windows.txt` records `RuntimeAlias: true` across both routes and four viewports | Verifier rejects `canvasFloatingWindow !== overlayWindow` |
| Overlay lifecycle state | `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Overlays.razor` renders show/reset controls and `OverlayWindow` | `repo://src/CanDoItAll.Components.OverlayLib/Components/Core/OverlayWindow.razor` consumes button actions and JS callbacks | `bundle://proof/SB07/browser-actions.json` records initial/minimized/restored/dragged/resized/reset/hidden/shown states | `bundle://proof/SB07/transcripts/playwright-floating-windows.txt` rejects failed lifecycle transition or clipped state |
| Canvas lifecycle state | `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor` renders show/reset inspector controls and `CanvasFloatingWindow` | `repo://src/CanDoItAll.Components.CanvasLib/Components/Core/CanvasFloatingWindow.razor` adapts OverlayWindow callbacks to Canvas state | `bundle://proof/SB07/browser-actions.json` records Canvas lifecycle states | `bundle://proof/SB07/transcripts/playwright-floating-windows.txt` rejects route-reload-only show behavior or lost Canvas bounds |
| Canvas/Overlay state contracts | `repo://tests/CanDoItAll.Components.BaseLib.Tests/CanvasContractBehaviorTests.cs` and `LayoutNavigationOverlayBehaviorTests.cs` produce contract proof | `repo://src/CanDoItAll.Components.CanvasLib/Components/Core/CanvasFloatingWindow.razor` and `repo://src/CanDoItAll.Components.OverlayLib/Components/Core/OverlayWindow.razor` consume normalized state records | `bundle://proof/SB07/transcripts/dotnet-test-floating-window-contracts.txt` passes 17 focused tests | Tests cover malformed/null/non-positive state and roundtrip semantics |
| Safe-top/container bounds | `repo://src/CanDoItAll.Components.OverlayLib/wwwroot/js/runtime/overlay-window.js` resolves container and safe-top options | Overlay route frame and Canvas workbench stage consume safe-top/container behavior | `bundle://proof/SB07/transcripts/playwright-floating-windows.txt` records measured bounds and overflow per viewport | `bundle://proof/SB07/transcripts/playwright-floating-windows.txt` rejects clipping, toolbar overlap, collapsed geometry, or lateral overflow |


