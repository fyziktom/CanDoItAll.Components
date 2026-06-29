# SB04 Proof Manifest

Subbundle: `SB04 Canvas Runtime Asset And JavaScript Boundary Refactor`

Status: `Completed`

Owned raw notes and requirements:

- RAW03, RAW05, RAW06, RAW07.
- R04, R07, R08, R14.

Semantic invariant contract:

- `bundle://proof/SB04/semantic-invariants.md`

Changed-file hashes:

- `repo://src/CanDoItAll.Components.CanvasLib/Canvas/README.md` current SHA-256 `f4191d2eecd75475ed21ccab1b652c8bbc4769e60b8ff5b7ebee6d958335670f`
- `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/css/workbench/panels/03-help-settings-and-preview.css` current SHA-256 `b6838d903e944f12912550b39c0459ee42bf0997625ea94be02463c57b7d82cc`
- `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB04/verify-canvas-smoke.cjs` current SHA-256 `cdd2797b879db0d8230381253a952e6eb08e38f8ca813530b42d5705e0df52d7`
- Full current changed-file hash transcript: `bundle://proof/SB04/transcripts/changed-file-hashes.txt`

Command transcripts:

- Asset verification before docs: `bundle://proof/SB04/transcripts/npm-canvaslib-verify-assets-before.txt`
- Asset verification after docs: `bundle://proof/SB04/transcripts/npm-canvaslib-verify-assets-after.txt`
- Final asset verification after CSS/browser fix: `bundle://proof/SB04/transcripts/npm-canvaslib-verify-assets-final.txt`
- Runtime dependency assertion: `bundle://proof/SB04/transcripts/runtime-dependency-assertion.txt`
- Node syntax check: `bundle://proof/SB04/transcripts/node-check-canvas-overlay-js-final.txt`
- Generated asset source assertions: `bundle://proof/SB04/transcripts/source-assertions-generated-assets.txt`
- Runtime boundary source assertions: `bundle://proof/SB04/transcripts/source-assertions-runtime-boundaries.txt`
- Calendar width source assertions: `bundle://proof/SB04/transcripts/source-assertions-calendar-width.txt`
- Initial failing browser smoke transcript: `bundle://proof/SB04/transcripts/playwright-canvas-smoke-initial-fail.txt`
- Browser smoke transcript: `bundle://proof/SB04/transcripts/playwright-canvas-smoke.txt`
- Semantic adequacy transcript: `bundle://proof/SB04/transcripts/semantic-adequacy.txt`

Failing-first / adversarial negative proof:

- Failing-first: `bundle://proof/SB04/transcripts/playwright-canvas-smoke-initial-fail.txt` records that the initial SB04 browser smoke failed because the calendar runtime shell collapsed to near-zero width under `SectionCard`; the production fix in `.cdi-canvas-calendar-shell` sets `width: 100%` and `min-width: 0`.
- Negative proof coverage: `bundle://proof/SB04/transcripts/playwright-canvas-smoke.txt` measures calendar bounds after the fix and would fail on missing facades, missing runtime nodes, missing inspector, insufficient bounds, or console warnings/errors/pageerrors.

Passing / semantic positive proof:

- Passing transcript: `bundle://proof/SB04/transcripts/playwright-canvas-smoke.txt`
- Passing asset verification: `bundle://proof/SB04/transcripts/npm-canvaslib-verify-assets-final.txt`
- Semantic positive proof transcript: `bundle://proof/SB04/transcripts/semantic-adequacy.txt`

Browser proof:

- Screenshots: `bundle://proof/SB04/screenshots/max-desktop-canvas-smoke.png` and `bundle://proof/SB04/screenshots/desktop-1366-canvas-smoke.png`
- Browser action record: `bundle://proof/SB04/browser-actions.json`
- Console log: `bundle://proof/SB04/console-log.txt`
- Visual review: `bundle://proof/SB04/visual-review.md`

Source-level assertions:

- `bundle://proof/SB04/transcripts/source-assertions-generated-assets.txt` verifies generated style/runtime/preview/calendar includes.
- `bundle://proof/SB04/transcripts/source-assertions-runtime-boundaries.txt` verifies public facades, lifecycle cleanup, observer disconnection, and .NET interop boundaries.
- `bundle://proof/SB04/transcripts/source-assertions-calendar-width.txt` verifies the calendar shell width hardening.

Anti-stub audit:

- `bundle://proof/SB04/transcripts/anti-stub-audit.txt` states no stubs or blockers in SB04 scoped changed files.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| `CanDoItAll.canvasWorkbench` | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/07-runtime-entry.js` assigns the browser facade | `repo://src/CanDoItAll.Components.CanvasLib/Components/Workbench/CanvasWorkbench.razor` consumes the facade through JS interop | `bundle://proof/SB04/transcripts/source-assertions-runtime-boundaries.txt` verifies facade and disposal/lifecycle paths | `bundle://proof/SB04/transcripts/playwright-canvas-smoke.txt` fails if facade or runtime nodes are absent |
| `CanDoItAll.canvasCalendar` | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/calendar/canvasCalendarInterop.js` assigns the browser facade | `repo://src/CanDoItAll.Components.CanvasLib/Components/Calendar/CanvasCalendar.razor` consumes create/update/dispose methods | `bundle://proof/SB04/transcripts/source-assertions-runtime-boundaries.txt` and `source-assertions-calendar-width.txt` verify facade and visible shell behavior | `bundle://proof/SB04/transcripts/playwright-canvas-smoke.txt` rejects missing or collapsed calendar rendering |
| `CanDoItAll.canvasFloatingWindow` | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/canvas-floating-window.js` and OverlayLib runtime assign floating-window facades | Canvas and Overlay wrapper components consume floating-window interop | `bundle://proof/SB04/transcripts/source-assertions-runtime-boundaries.txt` verifies cleanup, observer, and geometry callback paths | SB02 browser lifecycle proof plus SB04 smoke reject missing floating-window behavior |

