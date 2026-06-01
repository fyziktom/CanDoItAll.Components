# SB02 Semantic Invariants

## Invariant ID: SB02-stage-barrier-runtime

Raw note owned: stage barriers must prove `wait-seconds`, `wait-for-active-motions`, `wait-for-object-motions`, `wait-for-render-idle`, and `wait-for-event`; stage B must not apply while stage A's motion is active; event waits must not spin rendering; failed delayed stages must stay diagnosable.

Expected behavior: the generic stage runner blocks and resumes stages by policy, exposes blockers and queue state, accepts manual event signals, and preserves failure state even after the bounded journal trims older entries.

Shallow-pass trap: a test that only checks policy string constants or immediate stage application would miss queued motion blockers, event-id wakeup, render idle scheduling, and delayed failure state.

Adversarial negative proof: `bundle://proof/SB02/transcripts/stage-runner-audit.txt` includes object-specific motion blocking, event barrier idle scheduling, and delayed failure diagnostics.

Semantic positive proof: `bundle://proof/SB02/transcripts/stage-runner-audit.txt` exercises ordered wait stages and all barrier policies through the real JS runner modules.

Anti-stub audit: `bundle://proof/SB02/transcripts/anti-stub-audit.txt` reports no TODO, NotImplemented, template-only, or fixture-specific production markers in the stage runner surfaces.

Changed source files and hashes: `bundle://proof/SB02/transcripts/source-assertions-and-hashes.txt`.

Production assertions: `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/30-webgl-scene-stage-runner.js`, `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/32-webgl-scene-stage-barriers.js`, and `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/33-webgl-scene-command-journal.js`.

Red-team negative case: the delayed-failure audit would fail if the runner dropped `failedCommandStageIds` or `lastStageError` when journal entries are trimmed.

Downstream dependency check: SB04 and SB10 may depend on stage diagnostics because SB02 closure proves command-stage scheduling and diagnostics are generic and Economy-free.
