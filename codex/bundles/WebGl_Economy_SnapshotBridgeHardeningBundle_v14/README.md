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
