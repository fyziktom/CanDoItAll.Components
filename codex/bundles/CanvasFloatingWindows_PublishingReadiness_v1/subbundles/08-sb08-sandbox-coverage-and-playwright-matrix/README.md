# SB08 Sandbox Coverage And Playwright Matrix

## Status

- `Ready`

## Objective

- Run the focused sandbox and Playwright validation matrix for Canvas, Canvas benchmark, and floating windows after foundations are proven.

## Covered Inputs

- RAW01: Reuse the prior publishing-prep validation pattern.
- RAW03: True validation of Canvas and floating windows.
- RAW04: Do not do WebGL part yet.
- RAW05: Preserve all functionality.
- R01, R03, R04, R08, R09, R10, R11, R13.

## Prerequisites

- SB01-SB07 progression gates passed.
- Sandbox can be started consistently.
- Playwright or Browser MCP tooling is available.
- Screenshot destination under `bundle://proof/SB08/screenshots/matrix` exists.

## Exact Source References

- repo://src/CanDoItAll.Components.Sandbox/SandboxCatalogRegistry.cs
- repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor
- repo://src/CanDoItAll.Components.Sandbox/Components/Pages/CanvasBenchmark.razor
- repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Overlays.razor
- repo://src/CanDoItAll.Components.Sandbox/SandboxCanvasSamples.cs
- repo://src/CanDoItAll.Components.Sandbox/CanvasBenchmarkSamples.cs
- repo://src/CanDoItAll.Components.Sandbox/Components/Canvas
- repo://codex/bundles/StandardComponents_PublishingReadiness_v1/scripts/verify-sb11-visual-matrix.mjs

## Deliverables

- Canvas/floating-window route and scenario matrix.
- Playwright matrix script or repeatable browser transcript.
- Screenshot set across required routes, open states, and viewports.
- Console and network error logs.
- Defect log with reopen decisions for SB03-SB07 when needed.
- Explicit statement that CanvasBenchmark is draw-cost evidence only and not a renderer-migration approval.

## Dependency Impact

- SB09 package/docs readiness depends on user-facing proof that public examples work.
- SB10 final closure depends on complete browser analytics and honest reopen decisions.
- Weak matrix proof would allow shallow route-render success to hide interaction, layout, or mobile regressions.

## Validation Depth

- Critical visual closure.
- End-to-end sandbox route matrix with screenshots, console checks, interaction assertions, and semantic proof.
- Artifact-backed proof manifest required.

## Implementation Steps

1. Start the sandbox and verify the catalog includes Canvas, Canvas benchmark, and Overlays routes.
2. Build or adapt a matrix script using the prior standard-components visual matrix pattern.
3. Cover `/groups/canvas` scenarios including happy, dense, empty, disabled/loading, long text, workbench interactions, calendar/preview states, and open floating windows.
4. Cover `/groups/canvas/benchmark` as benchmark route health only; record that it is not feature parity proof.
5. Cover `/groups/overlays` with open, minimized, restored, reset, and long-content states.
6. Run maximized desktop, 1366x900, 1024x768, and 390x844 passes.
7. Capture screenshots, console logs, DOM assertions, and a pass/fail matrix.
8. Reopen the owning subbundle for any critical defect instead of hiding it as residual risk.
9. Update execution report and create SB08 proof artifacts.

## Scope Exceptions

- Package/API/doc proof is owned by SB09.
- Final fake-proof and raw-note closure audit is owned by SB10.

## Do Not Do

- Do not claim the Canvas benchmark proves Canvas feature parity or replaces workbench validation.
- Do not add unrelated route redesigns.
- Do not edit WebGL files or include WebGL routes in the matrix.

## Acceptance Checklist

- Route matrix includes Canvas, Canvas benchmark, and Overlays.
- Matrix includes required scenario states and open interactive states.
- Screenshots exist and have been visually reviewed.
- Console errors are zero or classified with owners.
- The execution report browser analytics table is updated with route, viewport, actions, screenshots, and review findings.
- Any critical issue has a reopen owner.

## Proof Required

- Matrix command transcript.
- Screenshot inventory under `bundle://proof/SB08/screenshots/matrix`.
- Browser action transcript.
- Console/network log transcript.
- DOM assertion JSON or transcript.
- Visual review notes.
- `bundle://proof/SB08/manifest.md`
- `bundle://proof/SB08/semantic-invariants.md`
- Anti-stub audit transcript.

## Browser Validation Logging

- Routes: `/groups/canvas`, `/groups/canvas/benchmark`, `/groups/overlays`.
- Viewports: maximized desktop, 1366x900, 1024x768, 390x844.
- Actions: route render, scenario selection, workbench selection/context menu/zoom, calendar navigation, floating-window lifecycle, benchmark route health.
- Required evidence paths: `bundle://proof/SB08/screenshots/matrix/...`, `bundle://proof/SB08/matrix-results.json`, `bundle://proof/SB08/browser-actions.txt`, `bundle://proof/SB08/console-log.txt`.
- Review questions: Are all required states visible? Does text fit? Are windows reachable? Is mobile usable? Are benchmark claims limited to route health and draw-cost evidence?

## Progression Gate

- SB09 may proceed only after the matrix passes or critical findings are reopened and resolved by their owning subbundles.
- SB10 cannot close unless the matrix evidence is complete and browser analytics are updated.

## Suggested Agent Prompt

```text
Execute SB08 only. Run the focused Canvas, Canvas benchmark, and Overlays sandbox matrix with real browser proof, capture screenshots and console logs, reopen owners for critical defects, and update proof/report artifacts before closing.
```
