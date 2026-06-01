# CanDoItAll WebGL + Economy Integrated Simulation Readiness Bundle v16

## Validation Summary

Bundle readiness gate: passed with v16-specific validator.

Execution status: SB01-SB15 completed.

Final closure gate: completed after transcript-backed validation.

Browser validation analytics: no final UI demo requested or added; validation remained command/headless and large-screen WebGL policy was retained.

## Purpose
This follow-up bundle hardens the current cross-repo foundation after the snapshot/bridge implementation. It keeps `CanDoItAll.Components` generic and UI-focused, while the joined simulation + visualization workflow stays in `CanDoItAll.Economy`.

The bundle is intentionally **not** a request to build a final shared-well or farmer-land demo. Those examples are only readiness probes used to reveal missing generic capabilities.

## Core findings

1. The joined layer is correctly located in `CanDoItAll.Economy` through `CanDoItAll.Economy.Simulation.WebGlBridge` and `CanDoItAll.Economy.SimulationSandbox`.
2. WebGL runtime has improved modularity: stage runner and motion queue exist, and JS runtime audit gates check line counts, unsafe patterns, import graph, domain neutrality and large-screen policy.
3. Bridge projection is no longer metadata-only: initial scene, node/object mapping and staged action projection exist.
4. Snapshot contracts exist and can export/import/validate deterministic hashes, but production-grade snapshot creation, analysis, diffing and resume semantics are still thin.
5. The current state is close to a real simulation + visualization pipeline, but still needs stronger generic validation, bridge lifecycle proof, snapshot analysis services and backend-neutral sandbox orchestration.

## Hard rules

- Codex must work in the currently checked-out branches in both repositories. Do not create a new branch.
- `CanDoItAll.Components` must not reference Economy projects, Economy concepts, or example vocabulary.
- The joined simulation + visualization workflow belongs in `CanDoItAll.Economy`, not in `CanDoItAll.Components`.
- WebGL validation is desktop / large-screen only. Do not implement mobile, tablet, small-screen or medium-screen optimization work.
- Do not implement a one-off shared-well or farmer-land demo in this wave. Use them only as generic readiness probes.

## Main execution entry

Read and execute:

```text
06_prompts/one_shot_codex_prompt.md
```

Use the XLSX matrix as the implementation tracker:

```text
05_spreadsheets/implementation_matrix.xlsx
```
