# SB02 Semantic Invariants

## Invariant State Normalization

- Invariant ID: `SB02-INV-STATE-NORMALIZATION`
- Source raw note: RAW03 requires true validation of floating windows and RAW05 requires preserving all functionality.
- Expected behavior: `OverlayWindowState.Normalize` preserves visibility/minimized semantics, rounds positive geometry, clears non-positive geometry, and treats null as the default visible window.
- Disallowed shallow implementation: A normalizer that only checks non-null dimensions or ignores visibility/minimized differences.
- Failing-first test: N/A process/no production behavior change; this subbundle adds regression tests and docs without altering runtime behavior.
- Passing test: `bundle://proof/SB02/transcripts/dotnet-test-overlay-state.txt` and `bundle://proof/SB02/transcripts/semantic-adequacy.txt` include `SB02-INV-STATE-NORMALIZATION`.
- Changed source files: `repo://tests/CanDoItAll.Components.BaseLib.Tests/LayoutNavigationOverlayBehaviorTests.cs`.
- Production assertions: `repo://src/CanDoItAll.Components.OverlayLib/Models/OverlayWindowState.cs` contains `Normalize`, `AreEquivalent`, `Clone`, and `HasCustomGeometry`.
- Red-team negative case: `OverlayWindowStateEquivalenceDistinguishesVisibilityAndMinimizedSemantics` rejects a shallow geometry-only equivalence check.
- Downstream dependency check: SB07 can depend on generic OverlayLib state semantics before proving Canvas wrapper conversion.

## Invariant Runtime Ownership

- Invariant ID: `SB02-INV-WINDOW-RUNTIME`
- Source raw note: RAW03 requires floating-window hardening and R05 requires OverlayLib versus CanvasLib ownership to be clear.
- Expected behavior: OverlayLib owns generic create/update/dispose, drag, resize, container, safe-top, z-index, geometry publication, and lifecycle cleanup in plain browser JavaScript.
- Disallowed shallow implementation: Canvas-specific wrapper code duplicating generic drag/resize or geometry lifecycle behavior.
- Failing-first test: N/A process/no production behavior change; source assertions verify the existing production runtime boundary.
- Passing test: `bundle://proof/SB02/transcripts/node-check-overlay-window.txt`, `bundle://proof/SB02/transcripts/source-assertions-overlay-window.txt`, and `bundle://proof/SB02/transcripts/semantic-adequacy.txt` include `SB02-INV-WINDOW-RUNTIME`.
- Changed source files: `repo://src/CanDoItAll.Components.OverlayLib/README.md`.
- Production assertions: `repo://src/CanDoItAll.Components.OverlayLib/wwwroot/js/runtime/overlay-window.js` exports `root.overlayWindow` and `root.canvasFloatingWindow` through the same generic runtime API.
- Red-team negative case: Source assertions require create/update/dispose, resize observer, pointer listeners, safe top, container lookup, and geometry callback paths together.
- Downstream dependency check: SB07 must reopen SB02 if CanvasFloatingWindow duplicates or contradicts this generic runtime behavior.

## Invariant Browser Lifecycle

- Invariant ID: `SB02-INV-BROWSER-LIFECYCLE`
- Source raw note: RAW03 and R10 require true floating-window validation across visible, minimized, restored, hidden, dragged, resized, and readable states.
- Expected behavior: `route groups/overlays` renders an OverlayWindow inside its host frame, keeps header actions reachable, preserves safe-top/container bounds, and survives minimize/restore/reset/hide/show plus drag/resize actions across desktop and mobile viewports.
- Disallowed shallow implementation: A route smoke test that only proves the page loaded without opening or moving the floating window.
- Failing-first test: N/A process/no production behavior change; browser proof exercises existing production UI behavior.
- Passing test: `bundle://proof/SB02/transcripts/playwright-overlays.txt`, `bundle://proof/SB02/browser-actions.json`, and `bundle://proof/SB02/transcripts/semantic-adequacy.txt` include `SB02-INV-BROWSER-LIFECYCLE`.
- Changed source files: `repo://tests/CanDoItAll.Components.BaseLib.Tests/LayoutNavigationOverlayBehaviorTests.cs` and `repo://src/CanDoItAll.Components.OverlayLib/README.md`.
- Production assertions: `bundle://proof/SB02/browser-actions.json` records geometry snapshots for initial, minimized, restored, dragged, resized, hidden, and shown states.
- Red-team negative case: A hidden-only or screenshot-only proof would miss geometry bounds; the Playwright verifier asserts frame containment and action button sizes.
- Downstream dependency check: SB08 can reuse this route in the broader visual matrix.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| `OverlayWindowState` | `repo://src/CanDoItAll.Components.OverlayLib/Components/Core/OverlayWindow.razor` publishes normalized geometry through `OnGeometryChanged` and UI actions | `repo://src/CanDoItAll.Components.OverlayLib/Components/Core/OverlayWindow.razor` consumes `State` and `StateChanged`; `repo://src/CanDoItAll.Components.CanvasLib/Components/Core/CanvasFloatingWindow.razor` adapts Canvas wrapper state to OverlayLib state | `bundle://proof/SB02/transcripts/source-assertions-overlay-window.txt` verifies create/update/dispose and pointer/resize listener lifecycle | `bundle://proof/SB02/transcripts/dotnet-test-overlay-state.txt` covers non-positive geometry, null defaults, clone independence, and visibility/minimized equivalence differences |

