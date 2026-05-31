# SB15 - Components Large-Screen Policy Regression

## Goal

Keep WebGL desktop-only.

## Rules

Codex must not:

- add small-screen optimization work
- add mobile/tablet screenshots
- add responsive redesign for WebGL surfaces
- spend time on phone/tablet layout proof

Codex may:

- add unsupported-size warning for below-minimum viewport
- run desktop proof with 1440x900 or larger viewport
- keep generic UI around the canvas usable on desktop

## Validation

Update audit to scan new bundle prompts and source docs for unguarded small/medium/mobile tasks.
