# SB14 - Performance and scalability gates

Codex must extend performance probes:

Economy headless:
- 100 actors
- 500 stores/resources
- 1000 events
- 500 visual actions
- deterministic snapshot hash under acceptable time

WebGL data/runtime:
- 500 scene objects
- 1000 links
- 1000 symbols
- 500 staged commands
- stage queue/journal bounded

Report actual elapsed times, counts and thresholds.

## Status

Completed.

## Completion Notes

- Extended `EconomyPerformanceProbeTests` to generate `repo://CanDoItAll.Economy/artifacts/economy/performance/simulation-performance-proof.json` with the required Economy headless counts, thresholds, elapsed timings, deterministic hash, and WebGL projection counts.
- Extended `tools/webgllib/audit-sharedwell-performance.cjs` to generate `repo://CanDoItAll.Components/artifacts/webgl-economy-kernel-bridge-hardening-v12/performance/components-performance-proof.json` with 500 scene objects, 1000 links, 1000 symbols, 500 staged commands, drained queue, and bounded journal proof.
- Captured the bundle-level performance summary at `bundle://proof/SB14/performance-result-summary.json`.

## Goal

Extend performance probes for Economy headless pipeline and WebGL data/runtime scale, recording actual counts, elapsed times, thresholds, and bounded queues.

## Prerequisites

- SB08 runner and SB12 readiness artifacts must exist.
- SB04 audit must pass for WebGL runtime structure.

## Owned Requirements

- R14 Performance Gates.

## Dependency Impact

Feeds final closure confidence and later large-screen browser smoke readiness.

## Validation Depth

Critical performance proof with generated JSON/transcript, thresholds, counts, elapsed times, and anti-stub audit.

## Proof Required

- Performance test or command transcript.
- Performance result artifact with all required counts and thresholds.
- Proof manifest and semantic invariant contract.

## Progression Gate

Pass only when performance output reports real elapsed times and counts for every required Economy and WebGL scale target, and stage queue/journal bounds are asserted.

Progression gate: Passed.
