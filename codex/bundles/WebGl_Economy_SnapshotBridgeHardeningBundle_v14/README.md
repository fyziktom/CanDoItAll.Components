# CanDoItAll WebGL + Economy Snapshot Bridge Hardening Bundle v14

## Purpose

This follow-up bundle hardens the current cross-repo foundation after the latest Codex implementation.
It focuses on long-term maintainability, genericity, simulation-to-visualization bridge readiness,
WebGL JS runtime module hygiene, and pause/snapshot/replay support.

## Mandatory repository boundary

- `CanDoItAll.Components` owns generic UI/WebGL infrastructure only.
- `CanDoItAll.Components.WebGlLib` must remain domain-neutral.
- `CanDoItAll.Components.WebGlRunLib` may provide generic run/playback/action abstractions.
- `CanDoItAll.Economy` owns all economy-specific simulation, visualization, WebGL bridge, and future sandbox composition.
- `CanDoItAll.Economy.Simulation.WebGlBridge` is the only allowed Economy-side layer that references Components WebGL run contracts.
- No Economy concept may be added to Components.
- No SimpleAccounts or Ledger backend reference may be added to the WebGlBridge project.

## Current key assessment

The bridge is now in the correct repository and has moved beyond metadata-only projection:
it creates an initial `WebGlSceneDocument`, maps visual nodes/links/symbols, maps visual actions into generic WebGL run actions,
plans them, compiles command batches, and emits `WebGlRunActionStage` entries.

However, before building a real `SimulationSandbox`, the foundation still needs:
- snapshot contracts for paused simulation state inspection,
- a replayable bridge contract from a simulation snapshot and/or frame sequence,
- stronger diagnostics when visual nodes/actions cannot be resolved,
- JS runtime module size and responsibility gates,
- clearer separation of visual mapping schema from example vocabularies,
- stricter test probes for multiple example families, not only the shared-well example.

## Do not create branches

Codex must work in the currently checked-out branches in both repositories.
Do not run `git checkout -b`, `git switch -c`, `git branch <new>`, or create PR-only branches unless the user explicitly asks.

## WebGL screen policy

WebGL remains desktop / large-screen only.
Do not add mobile, tablet, small-screen, medium-screen, responsive, or touch-first optimization tasks.
Validation viewports should be 1440x900 or larger.

## Primary execution file

Start with:

`06_prompts/one_shot_codex_prompt.md`

Use the spreadsheet matrix for sequencing:

`05_spreadsheets/implementation_matrix.xlsx`

## Validation summary

Status: Completed.

Readiness gate:
- `scripts/validate_bundle.py --stage prepared` passed before production code changes.
- Dependency order is recorded in `plan/01-phase-plan.md`.
- Raw notes and normalized requirements are mapped in `traceability/01-raw-note-closure.md`.

Execution gate:
- Each subbundle passed entry and closure gates before final closure.
- Critical subbundles have artifact-backed proof under `proof/SBxx/`.
- Final closure passed `scripts/validate_bundle.py --stage completed` plus the validation commands in `04_validation/validation_commands.md`.
- Components validation: build passed, WebGlLib tests passed, WebGlRunLib tests passed, scene runtime audit passed, and stage/motion audits passed after neutral artifact-path proof regeneration.
- Economy validation: build passed with known dependency warnings, full test suite passed, and simulation boundary audit passed.
- Raw notes are closed note-by-note in `traceability/01-raw-note-closure.md`; residual follow-ups are listed in `proof/SB15/manifest.md`.
