# SB05 — Economy SimulationSandbox Session Hardening

## Goal

Make `SimulationSandbox` a stable interactive session foundation.

## Required actions

- Harden `EconomySimulationSandboxSessionService` lifecycle:
  - Load
  - Project
  - Seek
  - Step
  - Pause
  - Resume
  - Snapshot
  - Analyze
  - Export
- Add explicit result types for operations that can fail instead of throwing for common user actions.
- Add `CanStepForward`, `CanStepBackward`, `AvailableSteps`, `CurrentDiagnostics` helpers.
- Add session export/import DTO if not present.
- Keep backend-neutral behavior.

## Acceptance

Tests must verify:

- shared-resource and finite-resource fixtures can be loaded into sessions
- stepping changes current frame and snapshot
- pause/resume toggles state without recomputing scenario
- snapshot/analyze work at arbitrary step
- injected fake backend still works
