# SB05 - Economy desktop sandbox page skeleton

## Status

Completed. Closure gate passed.

## Goal

Create the first large-screen-only Economy sandbox page.

## Tasks

- Add a minimal page/component in Economy repo, not Components.
- Use `EconomySimulationSandboxSessionService`.
- Allow loading a known fixture.
- Display:
  - basic controls,
  - current step,
  - diagnostics,
  - WebGL view,
  - snapshot analysis panel.
- Use the generic browser apply adapter from Components.
- No mobile/small/medium optimization.

## Acceptance

- Browser smoke test at 1440x900+.
- Can load and apply at least one frame.
- Can pause, step, seek, snapshot, analyze.

## Prerequisites

- SB02, SB03, and SB04 completed or safe blockers recorded.
- Economy page must live in the Economy repo and may consume Components generic primitives.

## Owned Requirements

- R05 Desktop sandbox page.

## Dependency Impact

This is the UI foundation for SB11. If the page cannot host WebGL runtime assets or apply a frame, browser smoke artifacts are blocked.

## Validation Depth

Build/test proof plus large-screen Playwright/browser proof when a host route is available. If host routing is unavailable, record the explicit blocker and keep headless proof separate.

## Proof Required

- Economy build/test transcript covering the page.
- Large-screen browser action log and screenshot when hostable.
- `bundle://proof/SB05/manifest.md`
- `bundle://proof/SB05/semantic-invariants.md`

## Browser Validation Logging

Record route/window, 1440x900+ viewport, load fixture action, frame apply action, pause/step/seek/snapshot/analyze assertions, screenshot path, and result in `reviews/01-execution-report.md`.

## Semantic Adequacy Gate

- Shallow-pass trap: page renders labels but does not use `EconomySimulationSandboxSessionService` or generic browser apply adapter.
- Adversarial negative proof: page/status path reports missing projection/frame/runtime errors instead of pretending success.
- Semantic positive proof: loaded fixture can apply at least one frame and expose diagnostics/snapshot analysis.
- Anti-stub audit: no final-demo/mobile/responsive work and no placeholder event handlers.

## Progression Gate

Pass only when the desktop page can be built and either browser-smoked or blocked with a specific host/runtime reason.
