# SB02 Semantic Invariants

## INV-SB02-001 Stage Waits Are Barriers

- Expected behavior: stage `waitSeconds` delays later stages without blocking the UI thread.
- Shallow-pass trap: storing `waitSeconds` as metadata while all stage commands apply immediately.
- Positive proof: `bundle://proof/SB02/transcripts/stage-runner-audit.txt`.
- Negative proof: the same audit advances by `0.49` seconds for a `0.5` second wait and asserts the second stage has not applied.
- Source assertions: `bundle://proof/SB02/transcripts/source-assertions.txt`.

## INV-SB02-002 Stage Work Is Cancellable

- Expected behavior: scene update/import/dispose cancels queued command stages and records the cancellation reason.
- Shallow-pass trap: only clearing local arrays in the command-batch module.
- Positive proof: source assertions cite lifecycle calls to `cancelCommandStageRunner`; audit asserts `commandStageCancelledCount` and `lastStageCancelReason`.

## Production Behavior Artifact Matrix

| Signal/state | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Stage diagnostics fields | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/30-webgl-scene-stage-runner.js` | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/02-webgl-scene-core.js` | Enqueue, advance, complete, cancel | `bundle://proof/SB02/transcripts/stage-runner-audit.txt` |

