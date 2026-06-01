# SB05 - Economy desktop sandbox page skeleton

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
