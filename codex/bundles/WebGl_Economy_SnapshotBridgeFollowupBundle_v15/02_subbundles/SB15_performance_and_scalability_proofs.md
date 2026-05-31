# SB15 — Performance and scalability proofs

## Goal
Catch bottlenecks before building the UI.

## Required probes
- 250 actors
- 500 stores
- 1000 scheduled events
- 1000 visual actions
- 500 staged WebGL commands
- 100 snapshots

## Metrics
- simulation materialization time
- visual mapping time
- bridge projection time
- snapshot export/import time
- command batch normalization time
- average/peak WebGL frame time if browser proof is used

## Policy
Large-screen only: 1440x900 or larger.

## Status
- Completed. Proof recorded under `bundle://proof/SB15/manifest.md`.

## Prerequisites
- SB05 command-batch proof is complete.
- SB10 snapshot store proof is complete.
- SB13 and SB14 generic probe proof is complete.

## Exact Source References
- `bundle://09_performance/performance_risk_register.md`
- `repo://tools/webgllib/audit-scene-runtime.cjs`
- `C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests`

## Dependency Impact
- Critical final readiness signal before any later connected sandbox UI work.

## Validation Depth
- Requires scalability transcript, command-batch normalization timing, snapshot export/import timing, and anti-stub audit for fake performance rows.

## Acceptance Checklist
- Required scale sizes are exercised or explicitly blocked.
- Metrics are recorded with commands and output artifacts.
- Browser frame timing is recorded only if browser proof is used, at 1440x900 or larger.

## Proof Required
- `bundle://proof/SB15/manifest.md`
- `bundle://proof/SB15/semantic-invariants.md`
- Performance transcript and metrics artifact.

## Browser Validation Logging
- Browser validation is optional and only relevant for average/peak frame timing at 1440x900 or larger.

## Progression Gate
- SB16 may close only after performance evidence or explicit blocker is recorded.

## Suggested Agent Prompt
- Run or add scalability probes for actors, stores, events, visual actions, staged commands, snapshots, and timing metrics.
