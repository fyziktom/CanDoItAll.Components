# Execution Report

## Status

Execution status: `Completed`

This bundle completed execution. Prepared-stage validation passed in `bundle://reviews/prepared-validation.txt`, and completed-stage validation is recorded in `bundle://reviews/completed-validation.txt`. SB01-SB10 now have proof manifests, semantic invariants, and final red-team closure artifacts.

## Subbundle Gate Results

| Subbundle | Entry gate | Closure gate | Downstream dependencies checked | Progression result | Notes |
|---|---|---|---|---|---|
| SB01 | Passed | Passed | SB02-SB10 source/scope prerequisites checked | Passed | Inventory, WebGL exclusion, RAW07 runtime constraint, and proof manifest complete: `bundle://proof/SB01/manifest.md`. |
| SB02 | Passed | Passed | SB07 dependency checked with browser proof | Passed | OverlayLib state tests, JS source assertions, browser lifecycle proof, and manifest complete: `bundle://proof/SB02/manifest.md`. |
| SB03 | Passed | Passed | SB04-SB08 contract dependencies checked | Passed | Canvas contract tests, source assertions, and manifest complete: `bundle://proof/SB03/manifest.md`. |
| SB04 | Passed | Passed | SB05-SB08 asset/facade/browser dependencies checked | Passed | Runtime asset map, generated asset verification, pure-JS runtime dependency assertion, JS syntax checks, calendar shell width hardening, browser smoke, and manifest complete: `bundle://proof/SB04/manifest.md`. |
| SB05 | Passed | Passed | SB06-SB08 workbench/browser dependencies checked | Passed | Workbench scenarios, viewports, interactions, accessibility mirror, export/clipboard, help Escape hardening, and manifest complete: `bundle://proof/SB05/manifest.md`. |
| SB06 | Passed | Passed | SB08 route matrix and SB09 package/docs dependencies checked | Passed | Calendar scenarios, viewports, CRUD, playlist, export, preview cards, accessibility mirror, pure-JS export fix, and manifest complete: `bundle://proof/SB06/manifest.md`. |
| SB07 | Passed | Passed | SB08 route matrix and SB09 package/docs dependencies checked | Passed | CanvasFloatingWindow/OverlayWindow lifecycle, safe-top/container bounds, runtime ownership alias, focused state tests, asset proof, and manifest complete: `bundle://proof/SB07/manifest.md`. |
| SB08 | Passed | Passed | SB09 package/docs and SB10 final closure dependencies checked | Passed | Canvas, Canvas benchmark, and Overlays route matrix passed with 20 screenshots, benchmark scope repair, overlay geometry wait, and manifest complete: `bundle://proof/SB08/manifest.md`. |
| SB09 | Passed | Passed | SB10 final closure dependencies checked | Passed | CanvasLib/OverlayLib package docs, approval tests, generated asset verification, focused build/pack, package content, runtime dependency proof, and manifest complete: `bundle://proof/SB09/manifest.md`. |
| SB10 | Passed | Passed | Final transfer gate checked | Passed | Final red-team proof audit, raw-note closure, WebGL exclusion, runtime dependency red-team, transfer checklist, focused tests, asset verification, and manifest complete: `bundle://proof/SB10/manifest.md`. |

## Browser Validation Analytics

| Subbundle | Route | Viewport | Playwright MCP evidence | Screenshots | Result |
|---|---|---|---|---|---|
| SB01 | N/A inventory | N/A | Source inventory, source-reference assertion, WebGL exclusion assertion, prepared validator | N/A | Passed: `bundle://proof/SB01/manifest.md`. |
| SB02 | `route groups/overlays` | 1920x1080, 1366x900, 390x844 | Playwright opened route and exercised initial, minimized, restored, dragged, resized, hidden, and shown states with geometry assertions | `bundle://proof/SB02/screenshots/max-desktop-resized.png`, `bundle://proof/SB02/screenshots/desktop-1366-resized.png`, `bundle://proof/SB02/screenshots/mobile-390-resized.png` | Passed; visual review in `bundle://proof/SB02/visual-review.md`. |
| SB03 | N/A contract tests | N/A | Focused Canvas contract tests and combined Canvas/Overlay contract tests | N/A | Passed: `bundle://proof/SB03/transcripts/dotnet-test-canvas-contracts.txt`, `bundle://proof/SB03/manifest.md`. |
| SB04 | `route groups/canvas scenario happy-path` smoke | 1920x1080, 1366x900 | Playwright verified workbench shell, toolbar, stage, host, calendar shell, floating inspector, public Canvas facades, measured bounds, and zero console warnings/errors/pageerrors | `bundle://proof/SB04/screenshots/max-desktop-canvas-smoke.png`, `bundle://proof/SB04/screenshots/desktop-1366-canvas-smoke.png` | Passed; visual review in `bundle://proof/SB04/visual-review.md`. |
| SB05 | `route groups/canvas` | maximized desktop, 1366x900, 1024x768, 390x844 | Playwright verified five scenarios, four viewports, selection, context menu, quick create, drag/drop, keyboard, zoom, fit/focus, minimap, diagnostics state, accessibility mirror, export, clipboard, help/settings overlays, and zero console warnings/errors/pageerrors | `bundle://proof/SB05/screenshots/scenario-happy-path.png`, `bundle://proof/SB05/screenshots/viewport-mobile-390.png`, `bundle://proof/SB05/screenshots/interaction-help.png`, `bundle://proof/SB05/screenshots/interaction-settings.png` | Passed; visual review in `bundle://proof/SB05/visual-review.md`. |
| SB06 | `route groups/canvas` and preview cards | 1920x1080, 1366x900, 390x844 | Playwright verified five calendar/preview scenarios, three viewports, navigation, week/list view switching, timezone settings, help, export menu, CSV export callback with two visible events, editor update/create/delete, playlist choice/clone, accessibility mirror, preview cards, and zero console warnings/errors/pageerrors | `bundle://proof/SB06/screenshots/scenario-happy-path-calendar-preview.png`, `bundle://proof/SB06/screenshots/viewport-mobile-390-calendar-preview.png`, `bundle://proof/SB06/screenshots/action-export-menu.png`, `bundle://proof/SB06/screenshots/action-created-event.png` | Passed; visual review in `bundle://proof/SB06/visual-review.md`. |
| SB07 | `route groups/canvas` and `route groups/overlays` | 1920x1080, 1366x900, 1024x768, 390x844 | Playwright verified OverlayWindow and CanvasFloatingWindow initial, minimized, restored, dragged, resized, reset, hidden, and shown states; safe-top/container bounds; runtime alias ownership; action hit areas; and zero console warnings/errors/pageerrors | `bundle://proof/SB07/screenshots/overlay-desktop-1366-resized.png`, `bundle://proof/SB07/screenshots/overlay-mobile-390-minimized.png`, `bundle://proof/SB07/screenshots/canvas-desktop-1366-dragged.png`, `bundle://proof/SB07/screenshots/canvas-mobile-390-initial.png` | Passed; visual review in `bundle://proof/SB07/visual-review.md`. |
| SB08 | `route groups/canvas`, `route groups/canvas/benchmark`, `route groups/overlays` scenario matrix | 1920x1080, 1366x900, 1024x768, 390x844 | Playwright verified six Canvas scenarios, four Canvas viewports, Canvas zoom/context/calendar/floating-window interactions, benchmark route health with painted shipped-workbench preview and nonblank standalone prototype, overlay lifecycle with safe-top/container bounds, and zero console warnings/errors/pageerrors | 20 screenshots under `bundle://proof/SB08/screenshots/matrix/`; inventory in `bundle://proof/SB08/transcripts/screenshot-inventory.txt` | Passed; visual review in `bundle://proof/SB08/visual-review.md`. |
| SB09 | N/A package/API/docs | N/A | Approval tests, generated asset verifier, focused release build, package creation, package content manifest, source assertions, and runtime dependency proof | N/A | Passed; package/docs/API proof in `bundle://proof/SB09/manifest.md`. |
| SB10 | N/A final closure | N/A | Final proof audit, raw-note closure, WebGL exclusion assertion, runtime dependency red-team, focused tests, and completed validator | N/A | Passed; final closure proof in `bundle://proof/SB10/manifest.md`. |

## Analytics Review

- SB05 browser analytics passed with five scenario screenshots, four viewport screenshots, seven interaction screenshots, measured DOM/action JSON, and zero console warnings/errors/pageerrors.
- SB06 browser analytics passed with five calendar/preview scenario screenshots, three viewport screenshots, ten action screenshots, measured event counts, CRUD/playlist/export callbacks, and zero console warnings/errors/pageerrors.
- SB07 browser analytics passed with two routes, four viewports, 64 lifecycle action records, runtime alias checks, measured safe-top/container bounds, and zero console warnings/errors/pageerrors.
- SB08 browser analytics passed with three routes, four viewports, 20 matrix action records, 20 screenshots, benchmark draw-cost/no-migration scope checks, overlay safe-top/container geometry checks, and zero console warnings/errors/pageerrors.
- SB09 package/API analytics passed with approval snapshots, generated asset verification, focused release build/pack proof, nupkg content inspection, runtime dependency proof, and source WebGL exclusion assertions.
- SB10 final analytics passed with proof inventory audit, raw-note closure, WebGL exclusion assertion, runtime dependency red-team, final focused tests, generated asset verification, and completed-stage validator.
- Each UI subbundle must answer readability, clipping, lateral overflow, layering, available-space use, keyboard/focus, state roundtrip, console errors, and reopened-defect questions.

## Raw Note Closure

| Raw note | Status | Proof |
|---|---|---|
| RAW01 | Solved | SB01 reused the prior publishing readiness workflow and created `bundle://proof/SB01/manifest.md`; SB08 reused the prior visual-matrix pattern for a focused Canvas/benchmark/overlays proof; SB09 reused the publishing approval/package-doc proof pattern for CanvasLib and OverlayLib; SB10 closes the final proof audit in `bundle://proof/SB10/final-proof-audit.md`. |
| RAW02 | Solved | SB01 records recent-bundle pattern reuse and current scope in `bundle://inventories/canvas-floating-windows-publishing-map.md`; SB10 audits the full bundle workflow and raw-note closure in `bundle://proof/SB10/raw-note-closure.md`. |
| RAW03 | Solved | SB01 prepared scope; SB02 completed OverlayLib floating-window proof; SB03 completed Canvas contract proof; SB04 completed Canvas asset/runtime boundary proof and fixed collapsed calendar shell rendering; SB05 completed workbench interaction/accessibility/export browser proof; SB06 completed calendar and preview scenario/action proof; SB07 completed Canvas/Overlay floating-window lifecycle proof; SB08 completed Canvas, benchmark, and Overlays route matrix proof; SB09 completed package/API/docs publishing proof; SB10 final focused tests passed. |
| RAW04 | Solved | SB01 source assertion proves no WebGL changed files; SB08 matrix explicitly excluded WebGL routes and renderer-migration approval; SB09 and SB10 source assertions prove no WebGL implementation/package/sample/test/tool files changed; WebGL build restore drift is documented as future work. |
| RAW05 | Solved | SB02 added no-regression OverlayWindowState tests and browser lifecycle proof; SB03 added Canvas state/selection/layout/calendar contract tests; SB04 preserved runtime facades and corrected calendar visible bounds; SB05 preserved workbench interactions and hardened keyboard-opened help close behavior; SB06 preserved calendar CRUD/export/playlist behavior and fixed visible-range export payloads; SB07 preserved Canvas/Overlay floating-window lifecycle behavior; SB08 preserved route-level behavior through scenario, viewport, interaction, benchmark, and overlay matrix proof; SB09 added approval/package-content proof; SB10 final focused tests passed. |
| RAW06 | Solved | SB04 added a Canvas runtime asset map documenting ownership and pure-JS browser boundaries; SB06 added sandbox callback wiring and artifact-backed calendar/preview proof; SB07 clarified Canvas floating-window shim ownership and added sandbox proof controls; SB08 corrected misleading benchmark language; SB09 aligned CanvasLib/OverlayLib READMEs, validation commands, package usage examples, and open-source transfer notes; SB10 transfer checklist is recorded in `bundle://proof/SB10/open-source-transfer-checklist.md`. |
| RAW07 | Solved | SB01 added runtime dependency constraint to requirements, traceability, architecture, and downstream subbundles; SB04 proved no npm runtime dependency and documented npm as tooling-only; SB06-SB08 changes stayed C# and Razor plus pure JS with Node used only as proof tooling; SB09 proved package.json has no runtime dependencies and CanvasLib/OverlayLib runtime JS has no import/require path; SB10 runtime dependency red-team passed in `bundle://proof/SB10/runtime-dependency-red-team.txt`. |

## SB01 Semantic Adequacy Evidence

- Raw note owned: RAW01, RAW02, RAW03, RAW04, with RAW07 recorded as a cross-cutting execution constraint.
- Shipped behavior: The bundle now has a source-only inventory, markdown publishing map, source-reference assertions, WebGL changed-file assertion, prepared validator proof, and pure-JS/no-npm runtime constraint for downstream phases.
- Source proof: `bundle://inventories/current-state-data.json`, `bundle://inventories/canvas-floating-windows-publishing-map.md`, `bundle://inputs/03-runtime-constraint-update.md`, and `bundle://proof/SB01/manifest.md`.
- Test proof: `python scripts\validate_bundle.py . --profile initiative --stage prepared --repo-root <repo root>` in `bundle://proof/SB01/transcripts/prepared-validator.txt`.
- Shallow-pass trap: A bundle could list Canvas and Overlay names without checking current source references, changed-file scope, or runtime dependency constraints.
- Adversarial negative proof: `bundle://proof/SB01/transcripts/inventory-generation.txt` rejects noisy bin/obj inventory and `bundle://proof/SB01/transcripts/webgl-exclusion-source-assertion.txt` rejects any WebGL changed-file drift.
- Semantic positive proof: `bundle://proof/SB01/transcripts/semantic-adequacy.txt` ties `SB01-INV-INVENTORY`, `SB01-INV-WEBGL-SCOPE`, and `SB01-INV-RUNTIME-CONSTRAINT` to durable artifacts.
- Anti-stub audit: No production TODO, NotImplemented, placeholder, or fixture-specific branching matches were found in scoped Canvas/Overlay/Sandbox/tools roots; see `bundle://proof/SB01/transcripts/anti-stub-audit.txt`.

## SB02 Semantic Adequacy Evidence

- Raw note owned: RAW03 and RAW05 for floating-window validation and preserving functionality.
- Shipped behavior: OverlayLib now has expanded state contract tests, an ownership note, source assertions for the plain JS runtime, and browser proof for visible/minimized/restored/dragged/resized/hidden/shown OverlayWindow behavior.
- Source proof: `repo://src/CanDoItAll.Components.OverlayLib/README.md`, `repo://tests/CanDoItAll.Components.BaseLib.Tests/LayoutNavigationOverlayBehaviorTests.cs`, and `bundle://proof/SB02/manifest.md`.
- Test proof: `dotnet test tests\CanDoItAll.Components.BaseLib.Tests\CanDoItAll.Components.BaseLib.Tests.csproj --filter FullyQualifiedName~LayoutNavigationOverlayBehaviorTests --no-restore` in `bundle://proof/SB02/transcripts/dotnet-test-overlay-state.txt`; JS syntax proof in `bundle://proof/SB02/transcripts/node-check-overlay-window.txt`.
- Shallow-pass trap: A route-load-only or geometry-only proof could miss visibility/minimized semantics, lifecycle cleanup, safe-top bounds, and hidden/show transitions.
- Adversarial negative proof: `bundle://proof/SB02/transcripts/dotnet-test-overlay-state.txt` covers non-positive geometry, null defaults, clone independence, and visibility/minimized equivalence differences.
- Semantic positive proof: `bundle://proof/SB02/transcripts/playwright-overlays.txt` and `bundle://proof/SB02/browser-actions.json` prove real OverlayWindow lifecycle actions across desktop and mobile viewports; invariant IDs appear in `bundle://proof/SB02/transcripts/semantic-adequacy.txt`.
- Anti-stub audit: No stubs or blockers were found in SB02 production/test scope; see `bundle://proof/SB02/transcripts/anti-stub-audit.txt`.

## SB03 Semantic Adequacy Evidence

- Raw note owned: RAW03 and RAW05 for Canvas contract hardening and preserving functionality.
- Shipped behavior: CanvasLib has contract tests for malformed UI state JSON, selection normalization, Canvas-to-Overlay window roundtrip, serialization fallback, layout collision separation, and calendar defaults/request records.
- Source proof: `repo://tests/CanDoItAll.Components.BaseLib.Tests/CanvasContractBehaviorTests.cs`, `repo://tests/CanDoItAll.Components.BaseLib.Tests/CanDoItAll.Components.BaseLib.Tests.csproj`, and `bundle://proof/SB03/manifest.md`.
- Test proof: `dotnet test tests\CanDoItAll.Components.BaseLib.Tests\CanDoItAll.Components.BaseLib.Tests.csproj --filter FullyQualifiedName~CanvasContractBehaviorTests --no-restore` in `bundle://proof/SB03/transcripts/dotnet-test-canvas-contracts.txt`; combined Canvas/Overlay proof in `bundle://proof/SB03/transcripts/dotnet-test-canvas-overlay-contracts.txt`.
- Shallow-pass trap: A happy-path deserialize test could miss malformed input, duplicate/blank ids, non-positive geometry, pinned layout collisions, and calendar request payload semantics.
- Adversarial negative proof: `bundle://proof/SB03/transcripts/dotnet-test-canvas-contracts.txt` covers malformed JSON fallback, duplicate/blank selection ids, non-positive geometry, stale missing selections, and overlapping pinned layout boxes.
- Semantic positive proof: `bundle://proof/SB03/transcripts/semantic-adequacy.txt` ties `SB03-INV-UISTATE-SERIALIZATION`, `SB03-INV-SELECTION-LAYOUT`, and `SB03-INV-CALENDAR-CONTRACTS` to passing contract tests and source assertions.
- Anti-stub audit: No stubs or blockers were found in SB03 production/test scope; see `bundle://proof/SB03/transcripts/anti-stub-audit.txt`.

## SB04 Semantic Adequacy Evidence

- Raw note owned: RAW03, RAW05, RAW06, and RAW07 for Canvas runtime hardening, functionality preservation, documentation, and pure-JS/no-npm runtime dependency.
- Shipped behavior: CanvasLib now has a runtime asset map, final generated asset verification, pure-JS runtime dependency assertion, JS syntax proof, source assertions for public facades/lifecycle cleanup, and a production CSS fix that prevents the calendar runtime shell from collapsing under sandbox `SectionCard` layout.
- Source proof: `repo://src/CanDoItAll.Components.CanvasLib/Canvas/README.md`, `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/css/workbench/panels/03-help-settings-and-preview.css`, and `bundle://proof/SB04/manifest.md`.
- Test proof: `npm run canvaslib:verify-assets` in `bundle://proof/SB04/transcripts/npm-canvaslib-verify-assets-final.txt`; JS syntax proof in `bundle://proof/SB04/transcripts/node-check-canvas-overlay-js-final.txt`; browser proof in `bundle://proof/SB04/transcripts/playwright-canvas-smoke.txt`.
- Shallow-pass trap: A route-load-only proof could miss generated include drift, npm runtime dependency drift, missing browser facades, lifecycle cleanup regressions, or the collapsed 2px calendar shell found during the first smoke.
- Adversarial negative proof: `bundle://proof/SB04/transcripts/playwright-canvas-smoke.txt` measures workbench/calendar bounds and fails on missing facades, missing runtime nodes, missing inspector, insufficient bounds, or console warnings/errors/pageerrors.
- Semantic positive proof: `bundle://proof/SB04/transcripts/semantic-adequacy.txt` ties `SB04-INV-ASSET-ORDER`, `SB04-INV-PURE-JS-RUNTIME`, `SB04-INV-PUBLIC-FACADES`, and `SB04-INV-BROWSER-SMOKE` to durable transcripts and screenshots.
- Anti-stub audit: No stubs or blockers were found in SB04 scoped changed files; see `bundle://proof/SB04/transcripts/anti-stub-audit.txt`.

## SB05 Semantic Adequacy Evidence

- Raw note owned: RAW03 and RAW05 for true Canvas workbench validation and preserving existing interactions.
- Shipped behavior: CanvasLib now has real-browser proof for `route groups/canvas` happy-path, dense-content, empty-state, disabled-state, and long-text scenarios; four viewport sizes; selection, context menu, quick create, drag/move, zoom, fit, minimap, diagnostics state, help/settings, accessibility mirror, export, clipboard, and console quality. SB05 also hardens the pure JS workbench keyboard router so help opened by keyboard closes with Escape before generic Escape cleanup.
- Source proof: `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/05-viewport-and-events.js`, `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/07a-runtime-interaction-router.js`, `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB05/verify-workbench.cjs`, and `bundle://proof/SB05/manifest.md`.
- Test proof: `node codex\bundles\CanvasFloatingWindows_PublishingReadiness_v1\proof\SB05\verify-workbench.cjs` in `bundle://proof/SB05/transcripts/playwright-workbench.txt`; JS syntax proof in `bundle://proof/SB05/transcripts/node-check-workbench-js.txt`; source assertions in `bundle://proof/SB05/transcripts/source-assertions-workbench.txt`.
- Shallow-pass trap: A selector-only or screenshot-only pass could miss stale accessibility mirror entries, wrong scenario node counts, offscreen mobile overflow, hidden diagnostics contract, unreadable overlays, broken help Escape close, stubbed export, missing clipboard writes, or console errors.
- Adversarial negative proof: `bundle://proof/SB05/transcripts/playwright-workbench.txt` fails on wrong scenario counts, missing hot zones, insufficient shell/stage/toolbar bounds, horizontal overflow, unreadable menus/overlays, failed drag, failed diagnostics state toggle, help overlay still intercepting settings after Escape, tiny export payloads, missing clipboard writes, and console warnings/errors/pageerrors.
- Semantic positive proof: `bundle://proof/SB05/transcripts/semantic-adequacy.txt` ties `SB05-INV-SCENARIO-COVERAGE`, `SB05-INV-VIEWPORTS`, `SB05-INV-INTERACTIONS`, `SB05-INV-KEYBOARD-TOOLBAR`, `SB05-INV-ACCESSIBILITY-MIRROR`, and `SB05-INV-EXPORT-CLIPBOARD` to durable transcripts, screenshots, and `bundle://proof/SB05/semantic-invariants.md`.
- Anti-stub audit: No stubs or blockers were found in SB05 scoped source/proof files; see `bundle://proof/SB05/transcripts/anti-stub-audit.txt`.

## SB06 Semantic Adequacy Evidence

- Raw note owned: RAW03, RAW05, RAW06, and RAW07 for true calendar/preview validation, functionality preservation, documentation clarity, and pure-JS/no-npm runtime dependency.
- Shipped behavior: CanvasLib calendar and preview surfaces now have browser proof for five scenarios, three viewports, calendar navigation, week/list view switching, timezone settings, help, export menu, CSV export callback, editor update/create/delete, playlist choice/clone, accessibility mirror, and preview/boundary cards. SB06 also fixes the pure JS calendar export path so the callback receives the current visible events instead of stale zero-event data.
- Source proof: `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/calendar/controller/03-editor-and-toolbar.js`, `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/calendar/canvasCalendarInterop.js`, `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor`, and `bundle://proof/SB06/manifest.md`.
- Test proof: `dotnet build src\CanDoItAll.Components.Sandbox\CanDoItAll.Components.Sandbox.csproj --no-restore` in `bundle://proof/SB06/transcripts/dotnet-build-sandbox.txt`; JS syntax proof in `bundle://proof/SB06/transcripts/node-check-calendar-js.txt`; browser proof in `bundle://proof/SB06/transcripts/playwright-calendar-preview.txt`.
- Shallow-pass trap: A route-load, static screenshot, or menu-open proof could miss wrong visible event counts, missing callbacks, zero-event export payloads, stale accessibility mirror rows, static playlist chips, or mobile lateral overflow.
- Adversarial negative proof: `bundle://proof/SB06/transcripts/playwright-calendar-preview.txt` rejects wrong scenario counts, missing preview cards, missing action state changes, `CSV (0 EVENTS)` export drift, missing playlist mutation, stale mirror content, lateral overflow, and console warnings/errors/pageerrors.
- Semantic positive proof: `bundle://proof/SB06/transcripts/semantic-adequacy.txt` ties `SB06-INV-CALENDAR-SCENARIOS`, `SB06-INV-CALENDAR-VIEWPORTS`, `SB06-INV-CALENDAR-ACTIONS`, `SB06-INV-CALENDAR-CRUD`, `SB06-INV-CALENDAR-EXPORT`, `SB06-INV-PLAYLISTS`, `SB06-INV-PREVIEW-CARDS`, and `SB06-INV-CALENDAR-A11Y` to durable transcripts, screenshots, and `bundle://proof/SB06/semantic-invariants.md`.
- Anti-stub audit: No stubs or blockers were found in SB06 scoped source/proof files; see `bundle://proof/SB06/transcripts/anti-stub-audit.txt`.

## SB07 Semantic Adequacy Evidence

- Raw note owned: RAW03, RAW05, RAW06, and RAW07 for true floating-window validation, functionality preservation, ownership clarity, and pure-JS/no-npm runtime dependency.
- Shipped behavior: CanvasLib now keeps `canvas-floating-window.js` as a compatibility shim that aliases to OverlayLib's runtime when generated Canvas assets load normally. The sandbox Canvas route has show/reset inspector proof controls, and browser proof covers OverlayWindow and CanvasFloatingWindow initial, minimized, restored, dragged, resized, reset, hidden, and shown states across four viewports.
- Source proof: `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/canvas-floating-window.js`, `repo://src/CanDoItAll.Components.CanvasLib/Canvas/README.md`, `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor`, and `bundle://proof/SB07/manifest.md`.
- Test proof: `dotnet test tests\CanDoItAll.Components.BaseLib.Tests\CanDoItAll.Components.BaseLib.Tests.csproj --filter "FullyQualifiedName~CanvasContractBehaviorTests|FullyQualifiedName~LayoutNavigationOverlayBehaviorTests" --no-restore` in `bundle://proof/SB07/transcripts/dotnet-test-floating-window-contracts.txt`; browser proof in `bundle://proof/SB07/transcripts/playwright-floating-windows.txt`; generated asset proof in `bundle://proof/SB07/transcripts/npm-canvaslib-verify-assets.txt`.
- Shallow-pass trap: Overlay-only proof or route-load proof could miss Canvas runtime ownership drift, stale Canvas-only duplicate window logic, missing Canvas show path after hide, mobile frame compression, or safe-top overlap.
- Adversarial negative proof: `bundle://proof/SB07/transcripts/playwright-floating-windows.txt` rejects `canvasFloatingWindow !== overlayWindow`, clipping, toolbar overlap, collapsed mobile windows, tiny action hit areas, failed drag/resize/reset/hide/show, and console warnings/errors/pageerrors.
- Semantic positive proof: `bundle://proof/SB07/transcripts/semantic-adequacy.txt` ties `SB07-INV-RUNTIME-OWNERSHIP`, `SB07-INV-OVERLAY-LIFECYCLE`, `SB07-INV-CANVAS-LIFECYCLE`, and `SB07-INV-SAFE-TOP-CONTAINER` to durable transcripts, screenshots, and `bundle://proof/SB07/semantic-invariants.md`.
- Anti-stub audit: No stubs or blockers were found in SB07 scoped source/proof files; see `bundle://proof/SB07/transcripts/anti-stub-audit.txt`.

## SB08 Semantic Adequacy Evidence

- Raw note owned: RAW01, RAW03, RAW04, RAW05, RAW06, and RAW07 for workflow reuse, true route/browser validation, WebGL exclusion, functionality preservation, documentation clarity, and pure-JS/no-npm runtime dependency.
- Shipped behavior: The sandbox now has completed route-matrix proof for Canvas scenarios, Canvas viewports, Canvas interactions, Canvas benchmark route health, and Overlays floating-window lifecycle. SB08 also corrected the benchmark page from stale retained DOM-SVG language to shipped-workbench/standalone-prototype wording and ensured the shipped preview has a full-width painted workbench surface.
- Source proof: `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/CanvasBenchmark.razor`, `repo://src/CanDoItAll.Components.Sandbox/CanvasBenchmarkSamples.cs`, `repo://src/CanDoItAll.Components.Sandbox/wwwroot/js/canvasBenchmarkPage.js`, `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB08/verify-sandbox-matrix.cjs`, and `bundle://proof/SB08/manifest.md`.
- Test proof: `dotnet build src\CanDoItAll.Components.Sandbox\CanDoItAll.Components.Sandbox.csproj --no-restore` in `bundle://proof/SB08/transcripts/dotnet-build-sandbox.txt`; browser matrix proof in `bundle://proof/SB08/transcripts/playwright-sandbox-matrix.txt`; JS syntax proof in `bundle://proof/SB08/transcripts/node-check-sandbox-matrix.txt` and `bundle://proof/SB08/transcripts/node-check-canvas-benchmark-page.txt`.
- Shallow-pass trap: A route-load-only proof could miss the benchmark preview rendering as a collapsed/misleading panel, stale renderer wording, Canvas interaction failures, mobile overflow, or overlay hide/show geometry settling after remount.
- Adversarial negative proof: The final verifier rejects missing runtimes, wrong scenario counts, stale calendar event counts, missing accessibility mirror rows, lateral overflow, blank shipped benchmark preview canvas layers, blank standalone prototype canvas, missing benchmark scope warning, overlay safe-top/container escape, WebGL route inclusion, and console warnings/errors/pageerrors.
- Semantic positive proof: `bundle://proof/SB08/transcripts/semantic-adequacy.txt` ties `SB08-INV-CANVAS-SCENARIOS`, `SB08-INV-ROUTE-VIEWPORTS`, `SB08-INV-CANVAS-INTERACTIONS`, `SB08-INV-BENCHMARK-SCOPE`, and `SB08-INV-OVERLAY-MATRIX` to durable transcripts, screenshots, JSON results, and `bundle://proof/SB08/semantic-invariants.md`.
- Anti-stub audit: No stubs or blockers were found in SB08 scoped source/proof files; see `bundle://proof/SB08/transcripts/anti-stub-audit.txt`.

## SB09 Semantic Adequacy Evidence

- Raw note owned: RAW01, RAW03, RAW05, RAW06, and RAW07 for publishing workflow reuse, package/API hardening, behavior preservation, open-source docs, and pure-JS/no-npm runtime policy.
- Shipped behavior: CanvasLib and OverlayLib now have package-ready READMEs, focused publishing approval tests, public API/package/static-asset approval fixtures, generated asset verification, focused release build and pack proof, nupkg content inspection, runtime dependency proof, and open-source transfer notes.
- Source proof: `repo://src/CanDoItAll.Components.CanvasLib/README.md`, `repo://src/CanDoItAll.Components.OverlayLib/README.md`, `repo://tests/CanDoItAll.Components.BaseLib.Tests/CanvasOverlayPublishingApprovalTests.cs`, approval fixtures under `repo://tests/CanDoItAll.Components.BaseLib.Tests/fixtures/approvals/`, and `bundle://proof/SB09/manifest.md`.
- Test proof: focused approval tests in `bundle://proof/SB09/transcripts/dotnet-test-canvas-overlay-approvals.txt`; generated asset verifier in `bundle://proof/SB09/transcripts/npm-canvaslib-verify-assets.txt`; focused release build in `bundle://proof/SB09/transcripts/dotnet-build-canvas-overlay-release.txt`; pack proof in `bundle://proof/SB09/transcripts/dotnet-pack-canvas-overlay.txt`.
- Shallow-pass trap: README edits alone could miss public API drift, missing static web assets, generated include drift, accidental WebGL package inclusion, npm runtime dependency creep, or package artifacts that do not contain the browser runtime assets.
- Adversarial negative proof: `bundle://proof/SB09/transcripts/package-content-manifest.txt` fails on missing required package entries or WebGL package entries; `bundle://proof/SB09/transcripts/runtime-dependency-proof.txt` fails on npm runtime dependencies or import/require runtime JS; `bundle://proof/SB09/transcripts/source-assertions-package-docs.txt` fails on README/package metadata drift, missing approval files, or WebGL source drift.
- Semantic positive proof: `bundle://proof/SB09/transcripts/semantic-adequacy.txt` ties `SB09-INV-PACKAGE-DOCS`, `SB09-INV-API-APPROVALS`, `SB09-INV-GENERATED-ASSETS`, `SB09-INV-PACKAGE-CONTENTS`, and `SB09-INV-RUNTIME-DEPENDENCIES` to durable transcripts and `bundle://proof/SB09/semantic-invariants.md`.
- Anti-stub audit: No blocking stubs or markers were found in SB09 package/docs/API scope; generated approval snapshot placeholder-property names were classified as existing public API metadata, not SB09 stubs; see `bundle://proof/SB09/transcripts/anti-stub-audit.txt`.

## SB10 Semantic Adequacy Evidence

- Raw note owned: RAW01-RAW07 for final workflow closure, proof audit, raw-note closure, WebGL separation, behavior preservation, transfer readiness, and npm runtime dependency red-team.
- Shipped behavior: The bundle is now closed with final proof audit, raw-note closure, fake-proof resistance, WebGL exclusion assertion, runtime dependency red-team, open-source transfer checklist, final focused tests, final generated asset verification, and completed-stage validator.
- Source proof: `bundle://proof/SB10/final-proof-audit.md`, `bundle://proof/SB10/raw-note-closure.md`, `bundle://proof/SB10/fake-proof-resistance.md`, `bundle://proof/SB10/open-source-transfer-checklist.md`, and `bundle://proof/SB10/manifest.md`.
- Test proof: final focused tests in `bundle://proof/SB10/transcripts/dotnet-test-final-in-scope.txt`; generated asset verification in `bundle://proof/SB10/transcripts/npm-canvaslib-verify-assets-final.txt`; completed validator in `bundle://reviews/completed-validation.txt`.
- Shallow-pass trap: Marking complete after screenshots alone could miss missing proof manifests, unresolved raw notes, package/API gaps, hidden WebGL drift, or npm runtime dependency creep.
- Adversarial negative proof: `bundle://proof/SB10/transcripts/proof-inventory-audit.txt` fails on missing manifests or semantic invariants; `bundle://proof/SB10/webgl-exclusion-source-assertion.txt` fails on WebGL source drift; `bundle://proof/SB10/runtime-dependency-red-team.txt` fails on npm runtime dependencies or import/require JS patterns.
- Semantic positive proof: `bundle://proof/SB10/semantic-invariants.md` ties `SB10-INV-PROOF-COVERAGE`, `SB10-INV-RAW-NOTES`, `SB10-INV-WEBGL-EXCLUSION`, `SB10-INV-RUNTIME-DEPENDENCY`, and `SB10-INV-TRANSFER` to durable closure artifacts.
- Anti-stub audit: No stubs or blockers were found in SB10 final closure proof scope; see `bundle://proof/SB10/transcripts/anti-stub-audit.txt`.

## Semantic Invariant Contract Index

- SB01: `bundle://proof/SB01/semantic-invariants.md`
- SB02: `bundle://proof/SB02/semantic-invariants.md`
- SB03: `bundle://proof/SB03/semantic-invariants.md`
- SB04: `bundle://proof/SB04/semantic-invariants.md`
- SB05: `bundle://proof/SB05/semantic-invariants.md`
- SB06: `bundle://proof/SB06/semantic-invariants.md`
- SB07: `bundle://proof/SB07/semantic-invariants.md`
- SB08: `bundle://proof/SB08/semantic-invariants.md`
- SB09: `bundle://proof/SB09/semantic-invariants.md`
- SB10: `bundle://proof/SB10/semantic-invariants.md`


