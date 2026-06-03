# SB11 Large simulation performance and resource budgets

## Objective

Implement the next hardening step for the WebGL/Economy engine foundation.

## Scope

- Define performance budgets for 100, 500, and 1000+ object scenes and staged motion replay.
- Avoid full payload JSON serialization in `WebGlSceneView.OnParametersSet` for large scenes; use revision/runtime keys where possible.
- Add diagnostics assertions for full scene rebuild count, transform-only patch count, batch duration, resource disposal, asset cache entries, and queued motion depth.
- Run primitive and high-GLB stress where practical.

## Out of scope

- Do not add domain semantics into Components packages.
- Do not rewrite unrelated systems.
- Do not close the subbundle with screenshots only.
- Do not accept empty required proof artifacts.

## Implementation guidance

- Start with a failing-first test or audit where applicable.
- Make the smallest cohesive refactor that fixes the root cause.
- Add source assertions that prove the intended path is used.
- Keep API compatibility where safe; otherwise document the migration.
- Ensure all source-code comments are in English.

## Required proof

- Benchmark/test output with timing and budget assertions.
- Browser diagnostics JSON for large scene.
- Resource ownership/disposal proof.
- No regression in WebGlLib-only sample.

## Refactor gate

Before moving to the next subbundle, record:
- changed files;
- test/build/audit commands;
- proof artifact paths;
- open risks;
- whether public API changed and how users migrate.

## Execution status

Status: completed.

- Changed files: see `../../proof/SB11/changed-file-hashes.md`.
- Test/build/audit commands: see `../../proof/SB11/transcripts/`.
- Browser proof: `../../proof/SB11/browser/performance-proof-diagnostics.json` and `../../proof/SB11/browser/performance-proof-browser.png`.
- Public API changed: additive `WebGlRuntimeBudgetProfiles` helper with `Scene100()`, `Scene500()`, and `Scene1000Plus()`.
- Migration: large-scene callers should bump `WebGlSceneModel.Revision` or `WebGlSceneUiState.Revision` for content changes and set `WebGlRuntimeOptions.RuntimeKey` for runtime option changes.
- Open risks: no SB11 blocker. High-GLB proof used the practical model diagnostics path; primitive browser stress covered live rendering and diagnostics.
