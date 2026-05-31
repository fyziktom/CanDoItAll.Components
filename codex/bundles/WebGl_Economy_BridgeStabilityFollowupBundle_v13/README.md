# CanDoItAll WebGL + Economy Bridge Stability Follow-up Bundle v13

## Purpose

This bundle is a follow-up hardening/refactoring workflow after the latest Codex implementation in:

- `fyziktom/CanDoItAll.Components`, branch `webgl-engine`
- `fyziktom/CanDoItAll.Economy`, branch/default `main`

The goal is not to build the final demo yet. The goal is to stabilize the generic foundation so the later demo can connect simulation and visualization without hardcoded shared-well, farmer-land, ledger, or simple-account assumptions.

## Non-negotiable rules for Codex

1. Do not create a new branch. Work in the currently checked-out branch in each repository.
2. `CanDoItAll.Components` must remain generic UI/WebGL infrastructure. It must not reference `CanDoItAll.Economy` or contain economy, well, water, farmer, land, ledger, account, or simulation-domain vocabulary in generic runtime code.
3. The joined simulation + visualization layer belongs in `CanDoItAll.Economy`, preferably under `CanDoItAll.Economy.Simulation.WebGlBridge` and later an Economy-side simulation sandbox. Do not move this bridge into Components.
4. WebGL is desktop / large-screen only. Do not spend time on small/medium/mobile/tablet optimization or proof.
5. Avoid one-example implementation. Use shared-well and farmer-land only as readiness probes.
6. Every critical subbundle must record proof under `proof/SBxx/` with changed-file hashes, validation transcripts, and semantic invariants.
7. Prefer generic resource-scoped and policy-driven models over fields like `waterNeed`, `landCap`, or `wellDistance` in shared abstractions.
8. Do not weaken validation gates to make tests pass.

## Current high-level state

Codex made useful progress:

- Economy now has `CanDoItAll.Economy.Simulation.WebGlBridge` in the solution.
- Bridge references `Simulation.Abstractions`, `Simulation.Visualization`, and `CanDoItAll.Components.WebGlRunLib`.
- Experiment input packs, strict hashing, placement/parameters, normalizers, metric/invariant evaluators, simple transition engine, and bridge tests exist.
- Components has stage-aware command batching and JS batch normalization.

But several critical issues remain:

- The bridge projector currently creates mostly metadata-only WebGL run frames; it does not yet build an initial scene or staged command batches from visual actions.
- `input.Actions` can be duplicated into every frame.
- Bridge context does not populate node-to-object mappings from visual frames.
- Components stage execution still lacks real asynchronous stage barriers and per-object motion queues.
- Strict input pack validation exists, but fixture packs still appear to use placeholder hashes unless the test creates temporary strict packs.
- Some source files are still large enough to justify split/refactor gates.
- The bridge project reference to Components is a fragile sibling-repo path and needs a sustainable local/package strategy.
