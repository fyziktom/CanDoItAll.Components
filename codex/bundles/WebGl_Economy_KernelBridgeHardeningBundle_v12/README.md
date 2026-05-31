# CanDoItAll WebGL + Economy Kernel Bridge Hardening Bundle v12

Generated: 2026-05-31T11:24:03Z

## Purpose

This follow-up bundle audits the current post-v11 implementation in both repositories and prepares the next hardening wave. The goal is not to build the final shared-well demo yet. The goal is to make the foundation stable, generic, deterministic, and ready for a future simulation-to-visualization bridge.

The bundle covers:

- `fyziktom/CanDoItAll.Components` on the active `webgl-engine` branch.
- `fyziktom/CanDoItAll.Economy` on the active branch that contains the pushed simulation/experiment work.
- No branch creation. Codex must work in the currently checked-out branches in both local clones.
- WebGL remains desktop / large-screen only. Do not optimize for small/medium/mobile/tablet screens.

## Current review summary

The implementation moved in the right direction:

- Components now has `WebGlRunLib`, stage-aware command batches, C# and JS batch normalization, target/anchor resolution, visual state catalog checks, and motion/runtime diagnostics.
- Economy now has experiment input pack contracts, placement/parameter files, deterministic input generation, scenario/event normalization, typed refs, a simple state transition engine, visual action normalization, and shared-well/farmer-land probes.

The main remaining risks:

1. **Bridge gap:** there is still no clean project that maps `EconomyVisualFrame` / `EconomyVisualAction` into `WebGlRunDocument`, `WebGlRunFrame`, `WebGlScenePatch`, and `WebGlObjectMotionCommand`.
2. **Stage semantics gap:** WebGL batch stages exist, but the JS executor applies stage contents immediately; `waitSeconds` is not a true asynchronous stage barrier.
3. **Motion queue gap:** `queueMode=append` prevents replacement but does not yet mean a real sequential per-object queue.
4. **Genericity risk:** generic Economy abstractions improved, but policy classes still include concrete assumptions such as fee/tax/trade in shared policy classes. These can stay in Economy, but should be plugin/strategy-based and not hardwired into the core transition loop.
5. **Input pack integrity:** input pack validation improved, but fixtures still use placeholder hashes such as `sha256:scenario`. This must be separated into documentation examples or replaced with real hashes in strict validation fixtures.
6. **Overfitting risk:** shared-well and farmer-land probes are useful, but the next tests must prove that generic event/action/policy mechanisms can express both without custom one-off branches in core code.

## Required outcome

After executing this bundle, the codebase should have a stable bridge-ready architecture:

```text
Economy experiment JSON inputs
  -> scenario definition / placement / parameters / rules / run plan / visual mapping / invariants
  -> deterministic event stream
  -> generic state transition engine
  -> frames + deltas
  -> EconomyVisualFrame + EconomyVisualAction
  -> bridge adapter (future/new)
  -> WebGlRunDocument / WebGlRunFrame / WebGlScenePatch / WebGlObjectMotionCommand
  -> WebGlLib runtime
```

Do not implement a finished demo. Do implement the missing foundation that makes the demo possible without shortcuts.
