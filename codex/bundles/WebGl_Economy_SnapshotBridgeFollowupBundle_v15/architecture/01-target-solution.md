# Target Solution

The target architecture preserves a one-way dependency direction:

```text
Economy -> Components
```

Components owns generic WebGL runtime, command batches, motion queues, stage execution, diagnostics, run plans, and batch compilation. It must not reference Economy or include scenario-specific terminology in generic runtime/kernel code.

Economy owns simulation abstractions, simulation backends, visualization contracts, WebGL bridge projection, snapshot services, and sandbox orchestration. Only the bridge and sandbox layers may depend on Components WebGL packages when that dependency is part of projection or workflow wiring.

Snapshot services must be usable without WebGL. Visual and runtime attachments are optional, hash-separated metadata.

## Closure Update

SB01-SB16 execution kept the target dependency direction intact. Components remains the generic WebGL runtime/run-plan layer, while Economy owns simulation backends, bridge projection, snapshot building/analysis/store behavior, and sandbox orchestration. Final proof is recorded in `bundle://proof/SB16/manifest.md`.
