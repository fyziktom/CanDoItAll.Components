# SB02 Semantic Invariants

## Invariant SB02-SCHED-001

Raw notes:
- RN-003: "Keep JavaScript runtime modular and maintainable. Do not introduce TypeScript."
- RN-007: "Scheduler explicitly detects command stage runner state, not only indirect renderRequested flags."

Expected behavior:
- `resolveRenderReason` returns `command-stage` when the production `state.commandStageRunner` has queued stages or active wait time.
- Cancelled runner state must not keep the render loop alive.
- Runtime audit remains acyclic, under hard line thresholds, and domain-neutral.

Shallow-pass trap:
- A stale implementation can check only `state.pendingCommandStages` or `state.renderRequested` and still pass tests that rely on explicit scheduling side effects.

Adversarial negative proof:
- `tools/webgllib/audit-stage-runner.cjs` constructs scheduler states with empty render-request flags and only `commandStageRunner.queue` or `commandStageRunner.waitSeconds`.
- The old `pendingCommandStages` implementation would return an empty reason for those states.
- Transcript: `bundle://proof/SB02/transcripts/stage-runner-audit.txt`.

Semantic positive proof:
- `repo://CanDoItAll.Components/artifacts/webgl-runtime-stage-runner-hardening-v14/stage-runner/stage-runner-proof.json` records `queuedRunnerReason = "command-stage"`, `waitingRunnerReason = "command-stage"`, and `cancelledRunnerReason = ""`.
- Runtime audit transcript proves modules remain acyclic, hard-threshold compliant, and domain-neutral: `bundle://proof/SB02/transcripts/runtime-audit.txt`.

Anti-stub audit:
- `bundle://proof/SB02/source-assertions/anti-stub-scan.txt` shows no TODO, NotImplemented, fixture-specific branch, hardcode marker, or stale `pendingCommandStages` use in changed files.

Changed source files:
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/22-webgl-scene-scheduler.js`.
- `repo://CanDoItAll.Components/tools/webgllib/audit-stage-runner.cjs`.

Downstream dependency check:
- SB03 can rely on the render scheduler staying active for queued/waiting command stages and idling for cancelled runner state.
