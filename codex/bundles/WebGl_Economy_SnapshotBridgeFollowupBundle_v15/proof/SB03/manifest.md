# Proof manifest SB03

Status: Completed

## Scope

Runtime stage runner lifecycle hardening for named stage barriers and durable diagnostics.

## Changed Files

- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/30-webgl-scene-stage-runner.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/22-webgl-scene-scheduler.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/02-webgl-scene-core.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/08-webgl-scene-proof.js`
- `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interaction/WebGlSceneCommandBatch.cs`
- `repo://tools/webgllib/audit-stage-runner.cjs`

SHA-256 hashes:

- `bundle://proof/SB03/hashes/changed-file-hashes.txt`

## Command Transcripts

- Failing-first contract scan: `bundle://proof/SB03/transcripts/failing-first-stage-barrier-contract.txt`
- Semantic positive stage-runner audit: `bundle://proof/SB03/transcripts/stage-runner-audit.txt`
- WebGlLib tests: `bundle://proof/SB03/transcripts/webgllib-tests.txt`
- Runtime audit: `bundle://proof/SB03/transcripts/runtime-audit.txt`

## Source Assertions

- Barrier/diagnostic source assertions and anti-stub scan: `bundle://proof/SB03/source-assertions/stage-runner-source-assertions.txt`
- Runtime audit artifact: `repo://artifacts/webgl-runtime-stage-runner-hardening-v15/stage-runner/stage-runner-proof.json`
- `time-delay`, `wait-for-active-motions`, `wait-for-object-motions`, `wait-for-render-idle`, and `manual-step` are implemented in the stage runner.
- Diagnostics now include current stage, completed/failed/skipped IDs, result log, queue snapshot, barrier policy, barrier object IDs, and last stage error.
- Proof snapshots and diagnostics snapshots expose the stage result log and queue snapshot.

## Semantic Adequacy Gate

- Shallow-pass trap: a timer-only implementation could pass old waitSeconds tests while starting the next stage before active motions finish.
- Adversarial negative proof: `bundle://proof/SB03/transcripts/failing-first-stage-barrier-contract.txt` shows the named barrier/result-log contract was absent before implementation.
- Semantic positive proof: `bundle://proof/SB03/transcripts/stage-runner-audit.txt` proves time, active-motion, object-motion, render-idle, manual-step, scheduler, and diagnostics behavior.
- Anti-stub audit: `bundle://proof/SB03/source-assertions/stage-runner-source-assertions.txt` records no `TODO`, `NotImplemented`, `not implemented`, or fixture-specific markers in the changed stage runner/audit files.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| `commandStageBarrierPolicy` / `commandStageBarrierObjectIds` | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/30-webgl-scene-stage-runner.js` | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/02-webgl-scene-core.js`, `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/08-webgl-scene-proof.js` | Set after each applied stage, synced while waiting, cleared at queue completion. | Failing-first scan plus `wait-for-active-motions` and `manual-step` assertions in `bundle://proof/SB03/transcripts/stage-runner-audit.txt`. |
| `commandStageResultLog` / `commandStageQueueSnapshot` | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/30-webgl-scene-stage-runner.js` | Diagnostics/proof snapshot consumers in core/proof modules. | Appended on completed/failed/skipped stages and exposed after delayed stages. | Stage audit asserts result log length and queue blocking behavior. |

## Failures / Blockers

- No SB03 blocker.
- Runtime audit now reports 10 warning-threshold files because stage-runner grew to support named barriers; still below the 320-line hard threshold.
