# SB11 — First Large-Screen Browser Probe Design

## Goal

Prepare, but do not overbuild, the first interactive browser proof.

## Scope

The browser proof belongs in Economy, not Components.

It should use `CanDoItAll.Economy.SimulationSandbox` to load a real probe and feed a WebGL view through the existing generic Components WebGL primitives.

## Required large-screen policy

- Validate only at desktop/large-screen sizes, e.g. 1440x900 or 1920x1080.
- No small/medium/mobile/tablet optimization.

## Minimum UI behaviors

- load one selected experiment input pack
- show projection diagnostics
- show WebGL scene/run view
- step/seek/pause/resume
- show current frame/stage IDs
- export current snapshot
- show snapshot analysis text

## Acceptance

This is design/contract readiness only unless Codex is explicitly instructed to build the UI demo.
