# What is still missing before simulation + visualization can be joined safely?

## The desired generic pipeline

```text
Experiment input pack JSON
  -> strict input loader and hash validation
  -> backend-neutral simulation run
  -> frames/deltas/events/metrics/invariants
  -> visual frames and visual actions
  -> Economy.WebGlBridge
  -> WebGlRunDocument + initial scene + staged command batches
  -> large-screen WebGL runtime
  -> pause -> SimulationRunSnapshot + visual state attachment
  -> export/analyze/diff/resume planning
```

## Missing or weak capabilities

1. **Backend-neutral sandbox orchestration**
   - The current sandbox is useful but SimpleAccounts-specific.
   - Add a backend registry and run-plan backend selector.

2. **Executable bridge proof**
   - We have document projection, but need a headless proof that every stage contains executable patches/motions and every target resolves or fails loudly.

3. **Stage barrier semantics**
   - `waitSeconds` alone is not enough for real scenarios. A motion may last longer/shorter than the planned wait if speed/distance or frame timing changes.
   - Add barrier modes: `wait-for-active-motions`, `wait-for-object-motions`, `wait-for-render-idle`, `wait-for-event`.

4. **Snapshot as first-class analysis object**
   - Snapshot exists, but there should be a reusable snapshot builder, analyzer pipeline and persistence abstraction.
   - Tests should not contain the only useful analysis implementation.

5. **Generic visual mapping contracts**
   - Mapping must remain renderer-neutral until WebGlBridge.
   - It should support multiple examples through source-kind/category/action/symbol/pose/anchor mapping, not hardcoded example names.

6. **Readiness probes without demo overfitting**
   - Shared-resource access probe: actors, shared resource, distance, inventory, resale, admin burden, tax/fee/rule enforcement.
   - Constrained-spatial-resource probe: finite land, growth pressure, anti-concentration rules, external demand, ownership transfer.

7. **Performance proof**
   - Prove with increased counts: 100+ actors, 500+ events, 1000+ visual actions, 200+ snapshots or snapshot diffs.

## Explicitly out of scope for this wave

- Final shared-well UI demo.
- Mobile/tablet responsive WebGL optimization.
- Economy logic inside Components.
- Ledger-backed live visual demo.
