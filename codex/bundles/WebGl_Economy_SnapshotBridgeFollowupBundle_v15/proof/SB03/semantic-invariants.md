# Semantic invariants SB03

Status: Completed

- Invariant ID: `SB03-STAGE-BARRIER-DIAGNOSTICS`
- Source raw note: RN-004
- Expected behavior: command stages can pause on time, active motions, selected object motions, render-idle, or manual step, and delayed stage outcomes remain visible through diagnostics.
- Disallowed shallow implementation: only decrementing `waitSeconds` while ignoring active motion completion and omitting delayed stage result logs.
- Failing-first test: `bundle://proof/SB03/transcripts/failing-first-stage-barrier-contract.txt`
- Passing test: `bundle://proof/SB03/transcripts/stage-runner-audit.txt`; `bundle://proof/SB03/transcripts/webgllib-tests.txt`
- Changed source files: `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/30-webgl-scene-stage-runner.js`, `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/22-webgl-scene-scheduler.js`, `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/02-webgl-scene-core.js`, `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/08-webgl-scene-proof.js`, `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interaction/WebGlSceneCommandBatch.cs`, `repo://tools/webgllib/audit-stage-runner.cjs`
- Production assertions: `bundle://proof/SB03/source-assertions/stage-runner-source-assertions.txt`
- Red-team negative case: timer-only stage runner fails the named contract scan and would fail the active-motion/manual-step assertions in the audit.
- Downstream dependency check: SB04 and SB05 may depend on stage barriers and stage diagnostics.
- Shallow-pass trap: a test that only waits 0.5 seconds would not prove motion-completion or manual-step behavior.
- Adversarial negative proof: failing-first scan proves the named barrier contract did not exist before the change.
- Semantic positive proof: targeted audit covers all five barrier policies and scheduler behavior.
- Anti-stub audit: anti-stub scan found no placeholder markers in the changed stage runner or audit.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Stage barrier diagnostics | Stage runner `syncStageDiagnostics` | Core diagnostics snapshot and proof snapshot | Created after a stage applies, updated during waits, cleared when the stage queue completes. | Failing-first scan and audit assertions for active-motion/manual-step barriers. |
| Stage result log and queue snapshot | Stage runner drain/result logging | Runtime diagnostics, proof snapshot, downstream bridge/sandbox proof | Completed/failed/skipped rows are appended for initial and delayed stages; queued stages are exposed while barriers hold. | Audit asserts result log and queue behavior instead of only checking non-empty diagnostics. |
