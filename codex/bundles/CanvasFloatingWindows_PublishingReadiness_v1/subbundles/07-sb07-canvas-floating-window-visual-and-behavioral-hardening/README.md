# SB07 Canvas Floating Window Visual And Behavioral Hardening

## Status

- `Completed`

## Objective

- Prove CanvasFloatingWindow preserves OverlayWindow behavior while honoring Canvas-specific container, safe-top, state conversion, and visual constraints.

## Covered Inputs

- RAW03: True validation of floating windows.
- RAW05: Preserve all functionality.
- RAW06: Improve maintainability, clarity, and documentation.
- RAW07: Keep floating-window runtime implementation pure JavaScript and avoid npm runtime dependency.
- R04, R05, R10, R11, R12, R14.

## Prerequisites

- SB02 OverlayWindow tests and browser proof passed.
- SB03 Canvas state/window roundtrip tests passed.
- SB05 workbench stage proof passed.
- SB04 asset/runtime foundation passed.
- Sandbox `route groups/canvas` and `route groups/overlays` routes are runnable.

## Exact Source References

- repo://src/CanDoItAll.Components.CanvasLib/Components/Core/CanvasFloatingWindow.razor
- repo://src/CanDoItAll.Components.CanvasLib/Canvas/Workbench/CanvasWorkbenchUiState.cs
- repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/canvas-floating-window.js
- repo://src/CanDoItAll.Components.OverlayLib/Components/Core/OverlayWindow.razor
- repo://src/CanDoItAll.Components.OverlayLib/Models/OverlayWindowState.cs
- repo://src/CanDoItAll.Components.OverlayLib/wwwroot/js/runtime/overlay-window.js
- repo://src/CanDoItAll.Components.OverlayLib/wwwroot/css/overlay-window.css
- repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor
- repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Overlays.razor

## Deliverables

- CanvasFloatingWindow/OverlayWindow comparison proof.
- Browser verifier or Playwright proof for drag, resize, minimize, restore, reset, hide/show, safe-top, and container behavior.
- Source assertions that generic floating-window logic remains owned by OverlayLib.
- Source assertion that floating-window runtime changes, if any, are pure browser JavaScript/C# and Razor with no npm runtime dependency.
- Minimal repairs needed for state conversion, layering, clipping, long content, or mobile behavior.
- Documentation notes explaining Canvas wrapper ownership and OverlayLib ownership.

## Dependency Impact

- SB08 matrix depends on stable open-state routes and repeatable floating-window actions.
- SB09 docs/API proof depends on a clear public ownership boundary.
- SB10 final closure depends on proof that Canvas and Overlay windows did not diverge silently.

## Validation Depth

- Critical UI foundation.
- Browser action proof, contract tests or source assertions, screenshots, console logs, and semantic proof.
- Artifact-backed proof manifest required.

## Implementation Steps

1. Start the sandbox and open `route groups/overlays`; exercise OverlayWindow open-state lifecycle.
2. Open `route groups/canvas`; exercise CanvasFloatingWindow in the workbench container.
3. Compare state conversions between `CanvasWorkbenchWindowState` and `OverlayWindowState`, including null and non-positive geometry.
4. Exercise drag, resize, minimize, restore, reset, hide/show, safe-top, z-index/layering, long title/content, dense content, and mobile constraints.
5. Add test hooks only when the existing sandbox cannot expose a production behavior.
6. Repair defects in the smallest scoped way, preferring OverlayLib for generic behavior and CanvasLib only for wrapper behavior.
7. Capture screenshots, action transcripts, DOM assertions, source assertions, and console logs.
8. Update execution report and create SB07 proof artifacts.

## Scope Exceptions

- Base OverlayWindow foundations are owned by SB02.
- General workbench interactions are owned by SB05.
- Full route matrix proof is owned by SB08.

## Do Not Do

- Do not duplicate generic OverlayWindow lifecycle or geometry logic inside CanvasLib.
- Do not introduce Canvas-only fixes for generic overlay defects.
- Do not change public state semantics without failing-first proof and docs.
- Do not edit WebGL files.
- Do not add npm runtime dependencies for floating-window behavior.

## Acceptance Checklist

- CanvasFloatingWindow and OverlayWindow use equivalent lifecycle behavior where intended.
- Canvas-specific safe-top and container rules are browser-proven.
- Drag, resize, minimize, restore, reset, and hide/show actions preserve state.
- Long titles/content do not overflow destructively.
- Mobile and tablet layouts remain usable.
- Console errors are zero or explicitly classified with blockers.

## Proof Required

- Playwright/MCP action transcript for `route groups/overlays`.
- Playwright/MCP action transcript for `route groups/canvas`.
- Screenshots for each required viewport and open/minimized/restored states.
- DOM assertion JSON or transcript for geometry/state where observable.
- Source assertion transcript for ownership boundary.
- Console log transcript.
- `bundle://proof/SB07/manifest.md`
- `bundle://proof/SB07/semantic-invariants.md`
- Anti-stub audit transcript.

## Browser Validation Logging

- Routes: `route groups/overlays` and `route groups/canvas`.
- Viewports: maximized desktop, 1366x900, 1024x768, 390x844.
- Actions: open, drag, resize, minimize, restore, reset, hide, show, verify safe-top, verify stage/container clipping, inspect long-content state.
- Required evidence paths: `bundle://proof/SB07/screenshots/...`, `bundle://proof/SB07/browser-actions.txt`, `bundle://proof/SB07/console-log.txt`.
- Review questions: Do windows remain reachable? Are controls visible? Does the Canvas toolbar safe area hold? Do state transitions match OverlayLib expectations?

## Progression Gate

- SB08 may proceed only after CanvasFloatingWindow and OverlayWindow behavior proof has no unresolved critical defects.
- Reopen SB02 if generic window behavior is wrong; reopen SB03 if state conversion semantics are wrong.

## Suggested Agent Prompt

```text
Execute SB07 only. Prove CanvasFloatingWindow and OverlayWindow behavior with real browser actions, repair only scoped defects, preserve OverlayLib ownership of generic window behavior, and update proof/report artifacts before closing.
```

