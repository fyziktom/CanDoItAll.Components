# CanDoItAll_WebGl_Economy_ExecutableBridgeReadinessBundle_v17

Follow-up hardening/refactoring bundle for the current cross-repo WebGL + Economy simulation preparation.

## Scope

This bundle audits and hardens the latest implementation across:

- `fyziktom/CanDoItAll.Components`, branch `webgl-engine`
- `fyziktom/CanDoItAll.Economy`, current `main`

The goal is not to build the final UI demo yet. The goal is to make the foundation stable, generic, testable, and ready for a later simulation + visualization integration demo.

## Non-negotiable rules

1. Do **not** create a new branch. Work in the currently checked-out branches in both repositories.
2. `CanDoItAll.Components` must remain a generic UI/WebGL/runtime component repository. It must not reference Economy.
3. The connected simulation + visualization stack belongs in `CanDoItAll.Economy`, especially under `Simulation.WebGlBridge` and `SimulationSandbox`.
4. WebGL is **desktop / large-screen only**. Do not optimize for small/medium/mobile/tablet screens and do not add mobile proof tasks.
5. Keep JavaScript modular, small, deterministic, safe, and domain-neutral. Do not migrate to TypeScript in this wave.
6. All source-code comments must be in English.
7. Keep bundle execution artifact-backed: proof manifests, changed-file hashes, validation transcripts, source assertions, and closure notes.

## Main files

- `00_context/01_current_review_summary.md`
- `01_architecture/01_target_pipeline.md`
- `01_architecture/02_gap_analysis_shared_resource_and_finite_land.md`
- `02_subbundles/`
- `05_spreadsheets/implementation_matrix.xlsx`
- `06_prompts/one_shot_codex_prompt.md`
