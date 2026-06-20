
# CanDoItAll Components WebGL Engine Stabilization Follow-up Bundle v15

Created: 2026-06-06T00:57:40Z

## Goal

Stabilize `CanDoItAll.Components` WebGL/Run engine as a reusable generic release-candidate before moving the majority of future work back into `CanDoItAll.Economy` and other domain packages.

This bundle is intentionally **Components-only**. Codex must not modify `CanDoItAll.Economy` while executing this bundle. Economy and manufacturing/production-line scenarios are used only as external consumer pressure tests.

## Current assessment

Codex moved the engine in the right direction: package scope is explicit, `WebGlRunActionKinds` has generic `DirectedFlowVisual`, API freeze tests exist, domain driver contracts exist, runtime idle policies exist, and domain leakage CI is stronger. The remaining problem is not one missing feature; it is release-candidate discipline: stable public APIs, JS/C# parity, package-mode proof, hard domain-boundary gates, production-line generic canary, and one deterministic validation command.

## Execution policy

- Execute subbundles in order.
- Every checkpoint (`CHECKPOINT-A` to `CHECKPOINT-D`) is mandatory.
- Do not continue past a checkpoint with failing proofs or unresolved architecture doubts.
- Preserve public behavior unless a subbundle explicitly approves a change.
- Any public API/action kind/JS method change must update approval snapshots and explain why it is generic.
- Do not add Economy concepts to generic Components. Use domain drivers.

## Final target

After SB20, Components should be stable enough that further Economy work can proceed mostly inside `CanDoItAll.Economy` through domain mapping drivers, scenario packs, oracles, metrics, and Economy UI.
