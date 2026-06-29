# SB04 Semantic Invariants

## Invariant Asset Order

- Invariant ID: `SB04-INV-ASSET-ORDER`
- Source raw note: RAW03 requires Canvas preparation, refactor, hardening, and true validation; RAW07 requires runtime Canvas behavior to remain pure browser JavaScript.
- Expected behavior: CanvasLib static web assets are generated from `tools/canvaslib/asset-manifest.json`, include OverlayLib runtime first, then Canvas services/runtime, preview assets, and calendar assets in the order required by their public browser facades.
- Disallowed shallow implementation: Route-load proof that never verifies generated include files or script ordering.
- Failing-first test: N/A process/no production behavior change for generated includes; this phase preserved generated asset ownership and did not hand-edit generated include components.
- Passing test: `bundle://proof/SB04/transcripts/npm-canvaslib-verify-assets-final.txt` and `bundle://proof/SB04/transcripts/semantic-adequacy.txt` include `SB04-INV-ASSET-ORDER`.
- Changed source files: `repo://src/CanDoItAll.Components.CanvasLib/Canvas/README.md` and `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/css/workbench/panels/03-help-settings-and-preview.css`.
- Production assertions: `bundle://proof/SB04/transcripts/source-assertions-generated-assets.txt` verifies generated include coverage for styles, runtime scripts, preview scripts, and calendar scripts.
- Red-team negative case: Asset verification would fail if generated include components drifted from the manifest or if manual generated edits changed the published script/css list.
- Downstream dependency check: SB05-SB08 browser validation can rely on deterministic static web asset loading.

## Invariant Pure JS Runtime

- Invariant ID: `SB04-INV-PURE-JS-RUNTIME`
- Source raw note: RAW07 requires Canvas, floating windows, calendar, and related runtime behavior to avoid npm/runtime dependency.
- Expected behavior: Canvas workbench, floating-window wrapper, calendar, preview modules, and runtime services ship as plain browser JavaScript/CSS static assets; npm remains tooling-only for Tailwind, generated assets, and browser validation.
- Disallowed shallow implementation: Adding a runtime npm package while leaving browser proof green on a developer machine.
- Failing-first test: N/A process/no runtime dependency change; this phase added no package dependency and documented the boundary.
- Passing test: `bundle://proof/SB04/transcripts/runtime-dependency-assertion.txt` and `bundle://proof/SB04/transcripts/semantic-adequacy.txt` include `SB04-INV-PURE-JS-RUNTIME`.
- Changed source files: `repo://src/CanDoItAll.Components.CanvasLib/Canvas/README.md`.
- Production assertions: `bundle://proof/SB04/transcripts/source-assertions-runtime-boundaries.txt` verifies public `window.CanDoItAll` browser facades and lifecycle cleanup in the plain JS runtime roots.
- Red-team negative case: `bundle://proof/SB04/transcripts/runtime-dependency-assertion.txt` rejects runtime `dependencies` in `package.json`; existing dev dependencies remain tooling-only.
- Downstream dependency check: SB09 package/docs can state the runtime implementation is not npm-hosted.

## Invariant Public Facades

- Invariant ID: `SB04-INV-PUBLIC-FACADES`
- Source raw note: RAW03 and RAW05 require maintainability while preserving current functionality.
- Expected behavior: `CanDoItAll.canvasWorkbench`, `CanDoItAll.canvasCalendar`, and `CanDoItAll.canvasFloatingWindow` remain present in the browser and continue to initialize meaningful workbench, calendar, and floating-window content.
- Disallowed shallow implementation: Source-only proof that does not confirm the facades load in a real browser.
- Failing-first test: `bundle://proof/SB04/transcripts/playwright-canvas-smoke-initial-fail.txt` records the collapsed calendar runtime surface before the final passing run.
- Passing test: `bundle://proof/SB04/transcripts/playwright-canvas-smoke.txt` and `bundle://proof/SB04/transcripts/semantic-adequacy.txt` include `SB04-INV-PUBLIC-FACADES`.
- Changed source files: `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/css/workbench/panels/03-help-settings-and-preview.css` and `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB04/verify-canvas-smoke.cjs`.
- Production assertions: `bundle://proof/SB04/transcripts/source-assertions-runtime-boundaries.txt` verifies facade assignments, disposal, event cleanup, observer disconnection, and .NET interop callback boundaries.
- Red-team negative case: Browser smoke fails if public facades are missing, if runtime nodes do not initialize, or if console warnings/errors/pageerrors appear.
- Downstream dependency check: SB05, SB06, SB07, and SB08 can use the same facades for deeper action proof.

## Invariant Browser Smoke

- Invariant ID: `SB04-INV-BROWSER-SMOKE`
- Source raw note: RAW03 requires true validation and RAW05 requires preserving functionality.
- Expected behavior: `route groups/canvas scenario happy-path` renders visible workbench shell, toolbar, stage, host, calendar shell, and canvas floating inspector across maximized desktop and 1366x900 viewports with zero console warnings/errors/pageerrors.
- Disallowed shallow implementation: HTTP 200 or selector-only proof that does not measure element bounds or capture screenshots.
- Failing-first test: `bundle://proof/SB04/transcripts/playwright-canvas-smoke-initial-fail.txt` records the first SB04 browser smoke failure where the calendar shell collapsed to near-zero width under `SectionCard`; `source-assertions-calendar-width.txt` and the final smoke prove the fix.
- Passing test: `bundle://proof/SB04/transcripts/playwright-canvas-smoke.txt` and `bundle://proof/SB04/transcripts/semantic-adequacy.txt` include `SB04-INV-BROWSER-SMOKE`.
- Changed source files: `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/css/workbench/panels/03-help-settings-and-preview.css`.
- Production assertions: `bundle://proof/SB04/transcripts/source-assertions-calendar-width.txt` verifies `.cdi-canvas-calendar-shell` has `width: 100%` and `min-width: 0`.
- Red-team negative case: The passing smoke records calendar bounds of 537px and 416px width after the fix, rejecting the earlier 2px collapsed rendering.
- Downstream dependency check: SB06 calendar validation starts from a browser-proven visible runtime surface.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| `CanDoItAll.canvasWorkbench` | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/07-runtime-entry.js` assigns the public browser facade | `repo://src/CanDoItAll.Components.CanvasLib/Components/Workbench/CanvasWorkbench.razor` invokes the facade through JS interop | `bundle://proof/SB04/transcripts/source-assertions-runtime-boundaries.txt` verifies facade, dispose, observer, and interop paths | `bundle://proof/SB04/transcripts/playwright-canvas-smoke.txt` fails if the facade is missing or workbench nodes do not initialize |
| `CanDoItAll.canvasCalendar` | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/calendar/canvasCalendarInterop.js` assigns the public browser facade | `repo://src/CanDoItAll.Components.CanvasLib/Components/Calendar/CanvasCalendar.razor` creates, updates, and disposes through JS interop | `bundle://proof/SB04/transcripts/source-assertions-runtime-boundaries.txt` verifies calendar facade presence and cleanup patterns | `bundle://proof/SB04/transcripts/playwright-canvas-smoke.txt` and `source-assertions-calendar-width.txt` reject collapsed or missing rendered calendar content |
| `CanDoItAll.canvasFloatingWindow` | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/canvas-floating-window.js` and `repo://src/CanDoItAll.Components.OverlayLib/wwwroot/js/runtime/overlay-window.js` assign floating-window facades | Canvas and Overlay wrapper components consume the facade from Razor JS interop | `bundle://proof/SB04/transcripts/source-assertions-runtime-boundaries.txt` verifies `removeEventListener`, `disconnect`, `dispose`, and `invokeMethodAsync` paths | SB02 browser proof plus SB04 smoke fail if the floating inspector/facade disappears |

