# Normalized Requirements

## Requirements

| ID | Requirement | Bundle Destination | Owning Subbundle | Observable Acceptance |
|---|---|---|---|---|
| R01 | Preserve the publishing-prep workflow proven by the standard-components bundle. | README, plan, prompts | SB01, SB10 | Bundle uses inventory, foundations, checkpoints, visual matrix, package/API closure, and red-team gates. |
| R02 | Inventory all CanvasLib, OverlayLib, floating-window, sandbox, test, asset, package, and docs surfaces before implementation. | analysis, inventories | SB01 | Inventory JSON/xlsx or markdown map is generated with source references and owner subbundles. |
| R03 | Exclude WebGL implementation work completely. | README, requirements, all subbundles | SB01-SB10 | Validator/source assertions prove no WebGL source/package/docs files changed except non-scope references if explicitly approved. |
| R04 | Preserve all current Canvas and floating-window functionality. | all subbundles | SB02-SB10 | Behavior-changing work has failing-first and passing proof; refactors have no-regression test/browser proof. |
| R05 | Clarify OverlayLib versus CanvasLib floating-window ownership. | architecture, SB02, SB07 | SB02, SB07 | Generic window behavior remains in OverlayLib; Canvas wrapper behavior is tested, documented, and browser-proven. |
| R06 | Add or expand contract tests for Canvas state, selection, layout, serialization, calendar, and window roundtrip behavior. | SB03 | SB03 | Dedicated Canvas tests or approved test placement runs cleanly with meaningful negative/positive cases. |
| R07 | Harden Canvas runtime assets and JavaScript boundaries without hand-editing generated asset components. | SB04 | SB04 | `npm run canvaslib:verify-assets`, node syntax checks, source assertions, and browser smoke pass. |
| R08 | Validate workbench interactions and accessibility behavior through real browser actions. | SB05 | SB05 | Selection, context menu, quick create, drag/drop, keyboard, zoom, fit/focus, minimap, diagnostics, accessibility mirror, and export are proven. |
| R09 | Validate calendar and preview surfaces as first-class CanvasLib publishing surfaces. | SB06 | SB06 | Calendar selection, CRUD callbacks, timezone/date handling, playlist/search/export, preview cards, empty/dense/long states, and visual proof pass. |
| R10 | Validate Canvas floating windows and OverlayWindow open states across container/safe-top, resize, drag, minimize, restore, reset, hide, and long content. | SB07 | SB07 | Browser proof captures both `/groups/overlays` and `/groups/canvas` states with clipping/layering/state assertions. |
| R11 | Expand sandbox coverage and run a Canvas/floating-window Playwright matrix. | SB08 | SB08 | Route/scenario matrix covers Canvas, Canvas benchmark, overlays, open interactive states, desktop/tablet/mobile viewports, screenshots, and console errors. |
| R12 | Prepare CanvasLib and OverlayLib package/API/docs for open-source publishing. | SB09 | SB09 | README/package metadata, generated assets, NuGet package contents, public API approvals, and consumer guidance are aligned. |
| R13 | Close with red-team proof that rejects fake proof, shallow tests, missing screenshots, and hidden WebGL scope drift. | SB10 | SB10 | Completed validator, proof manifest audit, raw-note closure, final package/build/test proof, and follow-up list pass. |

## Hard Constraints

- Do not edit WebGL implementation files in this bundle.
- Do not replace the shipped retained Canvas workbench renderer with the narrow benchmark canvas prototype.
- Do not weaken "preserve all functionality"; any intentional behavior change must be named, justified, and proven.
- Do not accept visual or interactive claims without browser evidence and screenshot review.
- Do not accept generated asset changes without regeneration and verification commands.
