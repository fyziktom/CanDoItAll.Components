# SB05 Proof Manifest

Subbundle: `SB05 Workbench Interaction And Accessibility Validation`

Status: `Completed`

Owned raw notes and requirements:

- RAW03 and RAW05.
- R04, R08, and R11.

Semantic invariant contract:

- `bundle://proof/SB05/semantic-invariants.md`

Changed-file hashes:

- `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/05-viewport-and-events.js` current SHA-256 `BB4FC3ACA32D55719659CAFB506438FA211FB63F0E8F0BD677D2B21B4B32883A`
- `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/07a-runtime-interaction-router.js` current SHA-256 `9A289C0D2E469E122BD2BF064E954A40D3340FCA5BC749BCE9035311ED3295E4`
- `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB05/verify-workbench.cjs` current SHA-256 `EB3A1BF6A9398A8DFD7E40B2B74D7017E02E262B88507BC9F39D3C5EE230EA22`
- Full current changed/proof hash transcript: `bundle://proof/SB05/transcripts/changed-file-hashes.txt`

Command transcripts:

- Browser workbench verifier: `bundle://proof/SB05/transcripts/playwright-workbench.txt`
- Node syntax check: `bundle://proof/SB05/transcripts/node-check-workbench-js.txt`
- Source assertions: `bundle://proof/SB05/transcripts/source-assertions-workbench.txt`
- Anti-stub audit: `bundle://proof/SB05/transcripts/anti-stub-audit.txt`
- Semantic adequacy transcript: `bundle://proof/SB05/transcripts/semantic-adequacy.txt`

Failing-first / adversarial negative proof:

- Failing-first: N/A process proof exemption; SB05 relied on browser-verifier assertions and captured the repaired behavior in passing transcripts rather than preserving a separate failing command transcript.
- The SB05 interaction verifier exposed that keyboard-opened help remained visible after Escape and intercepted the settings button. The production pure JS router now closes `state.helpOpen` before generic Escape cleanup in `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/05-viewport-and-events.js` and `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/07a-runtime-interaction-router.js`.
- `bundle://proof/SB05/transcripts/playwright-workbench.txt` now fails if `.cw-help-card` remains visible after Escape, if settings is unreachable, if diagnostics state does not toggle, if export/clipboard behavior is stubbed, if mobile overflows, or if console warnings/errors/pageerrors appear.

Passing / semantic positive proof:

- Passing browser transcript: `bundle://proof/SB05/transcripts/playwright-workbench.txt`
- Browser action record: `bundle://proof/SB05/browser-actions.json`
- Semantic positive proof transcript: `bundle://proof/SB05/transcripts/semantic-adequacy.txt`

Browser proof:

- Scenario screenshots: `bundle://proof/SB05/screenshots/scenario-happy-path.png`, `bundle://proof/SB05/screenshots/scenario-dense-content.png`, `bundle://proof/SB05/screenshots/scenario-empty-state.png`, `bundle://proof/SB05/screenshots/scenario-disabled-state.png`, and `bundle://proof/SB05/screenshots/scenario-long-text.png`
- Viewport screenshots: `bundle://proof/SB05/screenshots/viewport-max-desktop.png`, `bundle://proof/SB05/screenshots/viewport-desktop-1366.png`, `bundle://proof/SB05/screenshots/viewport-tablet-1024.png`, and `bundle://proof/SB05/screenshots/viewport-mobile-390.png`
- Interaction screenshots: `bundle://proof/SB05/screenshots/interaction-selected-foundations.png`, `bundle://proof/SB05/screenshots/interaction-context-menu.png`, `bundle://proof/SB05/screenshots/interaction-quick-create.png`, `bundle://proof/SB05/screenshots/interaction-drag-foundations.png`, `bundle://proof/SB05/screenshots/interaction-diagnostics-toggle-state.png`, `bundle://proof/SB05/screenshots/interaction-help.png`, and `bundle://proof/SB05/screenshots/interaction-settings.png`
- Console log: `bundle://proof/SB05/console-log.txt`
- Visual review: `bundle://proof/SB05/visual-review.md`

Source-level assertions:

- `bundle://proof/SB05/transcripts/source-assertions-workbench.txt` verifies workbench callbacks/state publishing, pure JS runtime actions, accessibility mirror source, scenario source, and verifier invariant coverage.
- `bundle://proof/SB05/transcripts/node-check-workbench-js.txt` verifies the changed pure JS files parse successfully.

Anti-stub audit:

- `bundle://proof/SB05/transcripts/anti-stub-audit.txt` states no stubs or blockers in SB05 scoped source/proof files.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| Scenario and viewport browser proof | `bundle://proof/SB05/verify-workbench.cjs` drives real browser coverage | `bundle://proof/SB05/browser-actions.json` stores measured outcomes | `bundle://proof/SB05/transcripts/playwright-workbench.txt` records zero console warnings/errors/pageerrors | Verifier rejects wrong node counts, missing bounds, mirror drift, and mobile overflow |
| Help Escape hardening | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/07a-runtime-interaction-router.js` closes `state.helpOpen` on Escape | `repo://src/CanDoItAll.Components.CanvasLib/Components/Workbench/CanvasWorkbench.razor` receives `OnHelpToggled` | `bundle://proof/SB05/transcripts/node-check-workbench-js.txt` verifies syntax and `source-assertions-workbench.txt` verifies source paths | Verifier fails if help intercepts settings after Escape |
| Accessibility mirror | `repo://src/CanDoItAll.Components.CanvasLib/Canvas/Core/AccessibilityMirrorLayer.cs` creates workbench mirror snapshots | `repo://src/CanDoItAll.Components.CanvasLib/Components/Core/AccessibilityMirrorLayer.razor` renders mirror content | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/accessibility-mirror-layer.js` updates and disposes mirror runtime | Verifier rejects missing or mismatched mirror items |
| Export and clipboard | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/07-runtime-entry.js` exposes export/clipboard facade behavior | `repo://src/CanDoItAll.Components.CanvasLib/Components/Workbench/CanvasWorkbench.razor` consumes JS interop callbacks | `bundle://proof/SB05/browser-actions.json` records PNG export and clipboard write outcomes | Verifier rejects tiny export payloads and missing clipboard writes |
