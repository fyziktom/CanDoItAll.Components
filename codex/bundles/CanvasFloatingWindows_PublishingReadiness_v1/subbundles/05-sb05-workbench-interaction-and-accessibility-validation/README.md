# SB05 Workbench Interaction And Accessibility Validation

## Status

- `Completed`

## Objective

Prove CanvasWorkbench production interactions and accessibility support through real browser actions, tests, screenshots, and source assertions.

## Covered Inputs

- RAW03: True validation of Canvas.
- RAW05: Preserve all functionality.
- R04, R08, R11.

## Prerequisites

- SB03 contract tests passed.
- SB04 asset/runtime foundation passed.
- Sandbox `route groups/canvas` route is runnable.

## Exact Source References

- repo://src/CanDoItAll.Components.CanvasLib/Components/Workbench/CanvasWorkbench.razor
- repo://src/CanDoItAll.Components.CanvasLib/Canvas/Workbench
- repo://src/CanDoItAll.Components.CanvasLib/Canvas/Graph/Interaction
- repo://src/CanDoItAll.Components.CanvasLib/Components/Core/AccessibilityMirrorLayer.razor
- repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench
- repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/accessibility-mirror-layer.js
- repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor
- repo://src/CanDoItAll.Components.Sandbox/SandboxCanvasSamples.cs

## Deliverables

- Workbench browser verifier or Playwright proof script.
- Screenshots and action traces for workbench interactions.
- Source assertions for callbacks, state publishing, accessibility mirror, and export behavior.
- Any minimal fixes needed to preserve or restore documented behavior.

## Dependency Impact

- SB07 depends on workbench stage behavior before CanvasFloatingWindow proof.
- SB08 matrix depends on stable workbench interactions and test hooks.
- SB09 docs and examples depend on proven user-facing behavior.

## Validation Depth

- Critical UI foundation.
- Browser action proof, tests where practical, source assertions, and semantic proof.
- Artifact-backed proof manifest required.

## Implementation Steps

1. Start the sandbox and open `route groups/canvas`.
2. Verify route states for happy, dense, empty, disabled, and long-text scenarios.
3. Exercise selection, context menu, quick create, drag/drop or move behavior, keyboard shortcuts, zoom, fit/focus, maximized state, minimap, diagnostics, help/settings overlays, accessibility mirror, and image export where supported.
4. Add test hooks or focused sandbox controls only when production behavior cannot otherwise be exercised.
5. Repair defects in the smallest scoped way.
6. Capture screenshots, console logs, DOM assertions, and visual review notes.
7. Update execution report and create SB05 proof artifacts.

## Scope Exceptions

- Floating-window-specific drag/resize/minimize proof is owned by SB07.
- Calendar-specific proof is owned by SB06.

## Do Not Do

- Do not replace the workbench renderer.
- Do not fake interactions with manually seeded production-only state unless the test is explicitly a fixture/validator.
- Do not edit WebGL files.

## Acceptance Checklist

- Workbench renders meaningful content across planned scenarios.
- Core interactions dispatch callbacks and update state.
- Accessibility mirror content is present and not stale.
- Help/settings overlays are readable and not clipped.
- Console errors are zero or explicitly classified with blockers.

## Proof Required

- Playwright/MCP action transcript.
- Screenshots for each required viewport and state.
- DOM assertion JSON or transcript.
- Console log transcript.
- Source assertion transcript.
- `bundle://proof/SB05/manifest.md`
- `bundle://proof/SB05/semantic-invariants.md`
- Anti-stub audit transcript.

## Browser Validation Logging

- Route: `route groups/canvas` with `happy-path`, `dense-content`, `empty-state`, `disabled-state`, and `long-text` scenarios.
- Viewports: maximized desktop, 1366x900, 1024x768, 390x844.
- Actions: select node, open context menu, open quick create, drag or move where supported, keyboard shortcuts, zoom, fit/focus, toggle maximized, minimap, diagnostics, help, settings, export.
- Review questions: Does text fit? Is the stage usable? Are overlays layered correctly? Does the route use available desktop space without breaking mobile orientation?

## Progression Gate

- SB07 and SB08 may proceed only after workbench interaction proof has no unresolved critical defects.
- Reopen SB03 or SB04 if failures point to state contracts or asset/runtime boundaries.

## Suggested Agent Prompt

```text
Execute SB05 only. Use real browser actions to validate CanvasWorkbench interactions and accessibility, capture screenshots and assertions, repair only scoped defects, and update proof/report artifacts before closing.
```

