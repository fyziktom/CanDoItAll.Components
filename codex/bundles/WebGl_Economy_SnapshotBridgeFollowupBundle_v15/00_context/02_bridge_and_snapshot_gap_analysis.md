# Bridge and snapshot gap analysis

## What is now possible

The current implementation can conceptually perform:

```text
experiment input pack
  -> scenario definition
  -> simple simulation frames/deltas
  -> EconomyVisualFrame / EconomyVisualAction
  -> Economy WebGL bridge
  -> WebGlRunDocument with InitialScene and action stages
```

It also can:

```text
pause at simulation step
  -> build SimulationRunSnapshot
  -> attach visual state
  -> export JSON
  -> re-import and validate deterministic hash
```

## What is not yet mature enough

### 1. Bridge execution proof

The bridge now emits staged actions and has an initial scene. The next hardening must prove that these stages can be fed into the actual WebGL runtime command batch execution without semantic loss.

Required proof:

- each visual action maps to exactly one or more traceable WebGL stages
- stages produce command batches with actual patches/motions
- delayed stages are observable in diagnostics
- unresolved mappings fail or are explicitly marked as diagnostic fallback
- no global action is duplicated into each frame

### 2. Snapshot as first-class analysis artifact

The snapshot model exists, but analysis is still mostly test-local. A real user workflow needs reusable analyzers:

- active admin burden
- issue pressure
- relationship stress
- resource concentration
- queue/stage pressure
- pending-event pressure
- invariant status
- visual-action pressure

### 3. Backend-neutral sandbox orchestration

`SimulationSandbox` currently uses simple accounts directly. That is acceptable for first wiring, but the next step should introduce a backend-neutral `IEconomySimulationSandboxOrchestrator`.

It should accept:

- input pack path
- backend selection
- run plan
- visual mapping
- projection options

And produce:

- load diagnostics
- scenario/run result
- visual frames
- WebGL run document
- snapshots
- bridge diagnostics

### 4. Generic probes

The shared resource / shared well example should remain a probe, not the architecture. The finite resource / farmer land example should also stay a probe.

Generic concepts needed:

- actors
- resources
- finite resource capacity
- shared resource access
- distance/topology
- inventory/carry capacity
- market/trade
- rule/tax/fee/admin burden
- issue/risk
- relationships/trust/conflict
- metrics/invariants
- visual actions
- snapshot analysis
