# SB02 Overlay Window Boundary And Regression Foundation

## Status

- `Completed`

## Objective

Make OverlayLib the proven owner of generic floating-window behavior and create enough regression coverage that Canvas-specific windows can safely depend on it.

## Covered Inputs

- RAW03: Focus on floating windows.
- RAW05: Preserve all functionality.
- R04, R05, R10.

## Prerequisites

- SB01 inventory and WebGL exclusion gate passed.
- Existing OverlayLib source and sandbox overlay route are readable.
- Component MCP was retried or unavailable fallback was recorded.

## Exact Source References

- repo://src/CanDoItAll.Components.OverlayLib/Components/Core/OverlayWindow.razor
- repo://src/CanDoItAll.Components.OverlayLib/Models/OverlayWindowState.cs
- repo://src/CanDoItAll.Components.OverlayLib/wwwroot/js/runtime/overlay-window.js
- repo://src/CanDoItAll.Components.OverlayLib/wwwroot/css/overlay-window.css
- repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Overlays.razor
- repo://tests/CanDoItAll.Components.BaseLib.Tests/LayoutNavigationOverlayBehaviorTests.cs

## Deliverables

- Overlay window ownership note or docs section.
- Expanded unit tests for state normalization, equivalence, visibility/minimize semantics, and geometry edge cases.
- Source assertions for JS create/update/dispose, drag/resize, safe top, and container handling.
- Browser proof for `route groups/overlays` window open states.

## Dependency Impact

- SB07 depends on OverlayLib behavior being stable before CanvasFloatingWindow hardening.
- SB08 visual matrix depends on this foundation for overlay route expectations.
- SB09 package/API approval depends on public OverlayLib behavior being intentional.

## Validation Depth

- Critical foundation and critical UI foundation.
- Contract tests plus browser open-state proof.
- Semantic Adequacy Gate and artifact-backed proof manifest required.

## Implementation Steps

1. Inspect `OverlayWindow`, `OverlayWindowState`, CSS, JS runtime, and sandbox usage.
2. Add or expand tests that fail for incorrect geometry normalization, state equivalence, visibility/minimize behavior, and container-safe-top assumptions.
3. Improve docs/source comments only where they clarify ownership or lifecycle.
4. Run targeted tests and JS syntax checks for `overlay-window.js`.
5. Use browser proof on `route groups/overlays` to open, minimize, restore, reset, hide/show, drag, and resize the window.
6. Update execution report browser analytics and create SB02 proof artifacts.

## Scope Exceptions

- Canvas-specific wrapper behavior is deferred to SB07.
- WebGL selectors in existing defaults may remain unchanged unless proven harmful to OverlayLib; do not edit WebGL source.

## Do Not Do

- Do not move Canvas-specific state into OverlayLib.
- Do not change public OverlayWindow parameters without package/API approval planning.
- Do not close from unit tests only.

## Acceptance Checklist

- Generic window state tests cover negative and positive geometry cases.
- Overlay route proves visible, minimized, restored, hidden, reset, dragged, resized, readable, and unclipped states.
- Source assertions prove JS lifecycle cleanup and container/safe-top handling.
- No Canvas or WebGL implementation is modified by this phase.

## Proof Required

- `dotnet test` targeted transcript.
- Node syntax check transcript for `overlay-window.js`.
- Playwright/MCP transcript and screenshots for `route groups/overlays`.
- `bundle://proof/SB02/manifest.md`
- `bundle://proof/SB02/semantic-invariants.md`
- Anti-stub audit transcript.

## Browser Validation Logging

- Route: `route groups/overlays?scenario=happy-path`, plus dense and long-text when available.
- Viewports: maximized desktop first, then 1366x900 and 390x844.
- Actions: show window, drag, resize, minimize, restore, reset, hide, show, interact with header/body controls.
- Review questions: Is content readable? Are header actions reachable? Is the window clipped? Is it layered above nearby chrome? Does long body copy wrap?

## Progression Gate

- SB07 may start only after OverlayLib state/lifecycle tests and overlay open-state browser proof pass.
- Reopen SB02 if SB07 finds generic window behavior duplicated or inconsistent in CanvasFloatingWindow.

## Suggested Agent Prompt

```text
Execute SB02 only. Prove OverlayLib owns generic floating-window behavior with tests, source assertions, and route groups/overlays browser proof. Preserve public behavior and stop if Canvas-specific changes become necessary.
```

