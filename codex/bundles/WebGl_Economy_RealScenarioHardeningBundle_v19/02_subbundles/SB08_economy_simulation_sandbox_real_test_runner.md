# SB08 - Economy simulation sandbox real test runner

Codex must add a real headless runner command or test helper that produces artifacts:

- `input-pack.validation.json`
- `simulation.frames.json`
- `simulation.deltas.json`
- `visual.frames.json`
- `webgl.run-document.json`
- `snapshots/<snapshot-id>.json`
- `snapshot-analysis/<snapshot-id>.json`
- `readiness-report.json`

Use existing shared-resource and finite-resource probes, but keep the runner generic.

## Status

Completed.

## Goal

Add a generic headless real-scenario runner that exports the required pipeline artifacts for shared-resource and finite-resource probes.

## Prerequisites

- SB03 controller proof must be available for run-document semantics.
- SB05 and SB07 bridge/boundary gates must pass.

## Owned Requirements

- R08 Headless Real Scenario Runner.

## Dependency Impact

Provides primary artifacts for SB09 snapshot attachment, SB12 readiness report, and SB14 performance work.

## Validation Depth

Critical feature proof with generated artifacts, positive scenarios, negative or strict validation failure coverage, source assertions, and anti-stub audit.

## Proof Required

- Runner command or test transcript with real output.
- Generated artifact paths for required files under `artifacts/economy/real-scenario-runs/<scenario-id>/`.
- Proof manifest and semantic invariant contract.

Proof captured in `bundle://proof/SB08/manifest.md`.

## Progression Gate

Pass only when required artifacts exist for `shared-well` and `farmer-land`, the runner remains generic, and strict validation does not silently pass invalid inputs.

Gate result: Passed. `EconomyRealScenarioRunner` exported both required scenarios to `repo://CanDoItAll.Economy/artifacts/economy/real-scenario-runs/`, the production runner has no shared-well/farmer-land branching, and the negative strict mutation test rejects `missing-source-input-pack-hash` before export.
