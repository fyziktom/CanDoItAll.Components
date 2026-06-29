# SB04 Canvas Runtime Asset And JavaScript Boundary Refactor

## Status

- `Ready`

## Objective

Harden CanvasLib generated assets, script order, JavaScript module ownership, and runtime source assertions so future refactors are maintainable without changing behavior.

## Covered Inputs

- RAW03: Canvas refactor, improvement, hardening, and true validation.
- RAW05: Preserve all functionality.
- RAW06: Maintainable, clear, documented, open-source-ready.
- R04, R07, R08.

## Prerequisites

- SB01 inventory gate passed.
- SB03 state/contract tests passed or blockers are documented.
- Generated asset tooling is available through `npm run canvaslib:build-assets` and `npm run canvaslib:verify-assets`.

## Exact Source References

- repo://tools/canvaslib/asset-manifest.json
- repo://tools/canvaslib/build-assets.cjs
- repo://tools/canvaslib/verify-assets.cjs
- repo://src/CanDoItAll.Components.CanvasLib/Components/Shared/Assets/CanvasLibHeadAssets.razor
- repo://src/CanDoItAll.Components.CanvasLib/Components/Shared/Assets/CanvasLibBodyAssets.razor
- repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/services
- repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench
- repo://src/CanDoItAll.Components.CanvasLib/wwwroot/css/workbench
- repo://package.json

## Deliverables

- Canvas runtime module map or docs section.
- Generated asset verification and, if needed, generator improvements.
- Node syntax checks for Canvas JS runtime/services/calendar/preview files.
- Source assertions for script order, public browser facades, lifecycle cleanup, and no manual generated-asset edits.
- Browser smoke after runtime hardening.

## Dependency Impact

- SB05-SB08 depend on asset load order and runtime source boundaries.
- SB09 package verification depends on generated assets and static web asset contents.
- Weak proof here can make all UI proof flaky or environment-specific.

## Validation Depth

- Critical foundation.
- Tooling, source, node syntax, browser smoke, and semantic proof.
- Artifact-backed proof manifest required.

## Implementation Steps

1. Build a Canvas JS/CSS/module map that names service, runtime, preview, calendar, and generated include ownership.
2. Run `npm run canvaslib:verify-assets` before changes.
3. If asset order or generated includes need changes, edit `tools/canvaslib/asset-manifest.json` or generator source, then run `npm run canvaslib:build-assets`.
4. Run node syntax checks across CanvasLib and OverlayLib JS files touched or depended on.
5. Refactor only small, behavior-preserving runtime boundaries with source assertions.
6. Browser-smoke `/groups/canvas` after changes and record console result.
7. Update execution report and create SB04 proof artifacts.

## Scope Exceptions

- Full interaction proof is deferred to SB05-SB08.
- True-canvas renderer migration is not allowed.

## Do Not Do

- Do not hand-edit generated `CanvasLibHeadAssets.razor` or `CanvasLibBodyAssets.razor`.
- Do not rename public browser facades without explicit approval.
- Do not mix WebGL runtime changes into Canvas runtime work.

## Acceptance Checklist

- Asset verifier passes before closure.
- Canvas asset include components match generator output.
- JS syntax checks pass.
- Module map explains large runtime files and public facades.
- Browser smoke proves Canvas route still loads meaningful content with zero console errors.

## Proof Required

- `npm run canvaslib:verify-assets` transcript.
- `npm run canvaslib:build-assets` transcript if generated files change.
- Node syntax transcript.
- Source assertion transcript.
- Browser smoke transcript and screenshot.
- `bundle://proof/SB04/manifest.md`
- `bundle://proof/SB04/semantic-invariants.md`
- Anti-stub audit transcript.

## Browser Validation Logging

- Route: `/groups/canvas?scenario=happy-path`.
- Viewports: maximized desktop and 1366x900.
- Actions: route load, wait for workbench host, verify Canvas runtime facade exists, verify no console errors, capture screenshot.
- Review questions: Did the workbench render meaningful content? Did generated assets load in order? Are toolbar and stage visible?

## Progression Gate

- SB05-SB08 may start only after asset verification, JS syntax/source assertions, and browser smoke pass.
- Reopen SB04 if later proof shows asset load order, public facade, or runtime lifecycle drift.

## Suggested Agent Prompt

```text
Execute SB04 only. Harden generated Canvas assets and JavaScript boundaries with module docs, asset verification, node checks, source assertions, and a Canvas browser smoke. Preserve behavior and avoid WebGL work.
```
