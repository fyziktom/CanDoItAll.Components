# SB02 Proof Manifest

Subbundle: `SB02 Overlay Window Boundary And Regression Foundation`

Status: `Completed`

Owned raw notes and requirements:

- RAW03, RAW05.
- R04, R05, R10.

Semantic invariant contract:

- `bundle://proof/SB02/semantic-invariants.md`

Changed-file hashes:

- `repo://src/CanDoItAll.Components.OverlayLib/README.md` current SHA-256 `493e49611d4ec564d85f5fd2824f37046079656dc1abf3275dc584dc426eeedc`
- `repo://tests/CanDoItAll.Components.BaseLib.Tests/LayoutNavigationOverlayBehaviorTests.cs` current SHA-256 `fcb1b4f0171ae39c7026f164b3423d832691feaa2f5a4b9b831171a8f8176823`
- Full current changed-file hash transcript: `bundle://proof/SB02/transcripts/changed-file-hashes.txt`

Command transcripts:

- Targeted test transcript: `bundle://proof/SB02/transcripts/dotnet-test-overlay-state.txt`
- Node syntax transcript: `bundle://proof/SB02/transcripts/node-check-overlay-window.txt`
- Source assertion transcript: `bundle://proof/SB02/transcripts/source-assertions-overlay-window.txt`
- Playwright browser transcript: `bundle://proof/SB02/transcripts/playwright-overlays.txt`
- Semantic adequacy transcript: `bundle://proof/SB02/transcripts/semantic-adequacy.txt`

Falling-first / adversarial negative proof:

- Failing-first: N/A process/no production behavior change. This phase adds regression tests, documentation, and proof artifacts without changing OverlayWindow production runtime behavior.
- Negative proof coverage: `bundle://proof/SB02/transcripts/dotnet-test-overlay-state.txt` covers geometry-only equivalence, null/default equivalence, non-positive geometry, and clone independence; `bundle://proof/SB02/browser-actions.json` rejects route-load-only proof by measuring real window bounds and actions.

Passing / semantic positive proof:

- Passing transcript: `bundle://proof/SB02/transcripts/dotnet-test-overlay-state.txt`
- Passing browser transcript: `bundle://proof/SB02/transcripts/playwright-overlays.txt`
- Semantic positive proof transcript: `bundle://proof/SB02/transcripts/semantic-adequacy.txt`

Browser and screenshot proof:

- Browser actions JSON: `bundle://proof/SB02/browser-actions.json`
- Console log: `bundle://proof/SB02/console-log.txt`
- Visual review: `bundle://proof/SB02/visual-review.md`
- Desktop screenshot: `bundle://proof/SB02/screenshots/max-desktop-resized.png`
- Fixed desktop screenshot: `bundle://proof/SB02/screenshots/desktop-1366-resized.png`
- Mobile screenshot: `bundle://proof/SB02/screenshots/mobile-390-resized.png`

Source-level assertions:

- `bundle://proof/SB02/transcripts/source-assertions-overlay-window.txt` verifies shared OverlayWindow runtime ownership, container/safe-top handling, pointer drag, ResizeObserver resize lifecycle, disposal cleanup, and geometry callback publication.
- `repo://src/CanDoItAll.Components.OverlayLib/README.md` documents OverlayLib ownership of generic floating-window behavior.

Anti-stub audit:

- `bundle://proof/SB02/transcripts/anti-stub-audit.txt` states no stubs or blockers in SB02 production/test scope.

Downstream smoke proof:

- `bundle://proof/SB02/transcripts/playwright-overlays.txt` proves `route groups/overlays` open-state behavior across max desktop, 1366x900, and 390x844, allowing SB07 to depend on OverlayLib behavior.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| `OverlayWindowState` | `repo://src/CanDoItAll.Components.OverlayLib/Components/Core/OverlayWindow.razor` publishes normalized geometry and UI action state | `repo://src/CanDoItAll.Components.OverlayLib/Components/Core/OverlayWindow.razor` consumes `State` and `StateChanged`; `repo://src/CanDoItAll.Components.CanvasLib/Components/Core/CanvasFloatingWindow.razor` adapts Canvas wrapper state to OverlayLib state | `bundle://proof/SB02/transcripts/source-assertions-overlay-window.txt` verifies create/update/dispose and listener cleanup | `bundle://proof/SB02/transcripts/dotnet-test-overlay-state.txt` rejects non-positive geometry, geometry-only equivalence, and shared clone mutation |


