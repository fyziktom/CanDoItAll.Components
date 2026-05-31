# SB03 Semantic Invariants

## INV-SB03-001 Same-Object Append Is Sequential

- Expected behavior: a same-object append command waits behind the active motion and starts from the object's final position after the prior motion completes.
- Shallow-pass trap: leaving both same-object motions active in `state.motions`.
- Positive proof: `bundle://proof/SB03/transcripts/motion-queue-audit.txt`.
- Negative proof: the audit asserts `state.motions.size == 1` after enqueue and that the second motion only activates after the first completes.

## INV-SB03-002 Motion Queue Diagnostics Are Real Runtime State

- Expected behavior: queue diagnostics are synchronized from production queue maps and active motions.
- Shallow-pass trap: returning hardcoded diagnostic counts from the audit fixture.
- Source proof: `bundle://proof/SB03/transcripts/source-assertions.txt`.

## Production Behavior Artifact Matrix

| Signal/state | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Motion queue diagnostics | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/29-webgl-scene-motion-queues.js` | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/02-webgl-scene-core.js` | Enqueue, promote, clear, cancel | `bundle://proof/SB03/transcripts/motion-queue-audit.txt` |

