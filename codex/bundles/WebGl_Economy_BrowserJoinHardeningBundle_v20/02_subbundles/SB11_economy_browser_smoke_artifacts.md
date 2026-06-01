# SB11 - Economy browser smoke artifacts

## Status

Completed. Closure gate passed.

## Goal

Generate proof artifacts from the first desktop browser smoke.

## Tasks

- Produce:
  - browser-smoke-readiness.json,
  - initial-scene-proof.json,
  - applied-frame-proof.json,
  - snapshot-analysis-proof.json,
  - screenshot if Playwright is available.
- Use desktop viewport only.

## Acceptance

- Browser proof is optional until UI skeleton exists, then required.
- No mobile proof.

## Prerequisites

- SB05 page exists and can be hosted or has a specific host blocker.
- SB07 strict mapping proof completed for the selected smoke input.
- SB08/SB09 snapshot and analysis outputs available.

## Owned Requirements

- R11 Browser smoke artifacts.

## Dependency Impact

Browser smoke artifacts feed SB12 performance confidence, SB13 leakage/refactor proof, and SB14 final readiness answer.

## Validation Depth

Use Playwright/browser proof at 1440x900+ when hostable. Produce JSON artifacts for readiness, initial scene, applied frame, snapshot analysis, and screenshot. If unavailable, record an explicit blocker with attempted route/command.

## Proof Required

- `bundle://proof/SB11/browser-smoke-readiness.json`
- `bundle://proof/SB11/initial-scene-proof.json`
- `bundle://proof/SB11/applied-frame-proof.json`
- `bundle://proof/SB11/snapshot-analysis-proof.json`
- Screenshot path when browser proof is available.
- `bundle://proof/SB11/manifest.md`
- `bundle://proof/SB11/semantic-invariants.md`

## Browser Validation Logging

Record route/window, 1440x900+ viewport, Playwright actions, assertions, screenshot path, and result in `reviews/01-execution-report.md`.

## Semantic Adequacy Gate

- Shallow-pass trap: JSON artifacts are generated from headless data without exercising browser apply.
- Adversarial negative proof: smoke readiness records missing host/runtime/apply blockers when browser proof is unavailable.
- Semantic positive proof: browser runtime loads initial scene, applies frame, and exposes snapshot analysis when hostable.
- Anti-stub audit: browser proof files contain real observed values and not static success templates.

## Progression Gate

Pass only when browser smoke artifacts exist with real proof or a precise blocker is recorded and final readiness remains `browser smoke next`, not `full UI demo ready`.
