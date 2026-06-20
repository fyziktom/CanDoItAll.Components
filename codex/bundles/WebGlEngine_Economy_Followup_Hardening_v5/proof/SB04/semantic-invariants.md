# Semantic invariants SB04

Status: completed

## Invariants

| ID | Requirement | Evidence | Closure |
| --- | --- | --- | --- |
| `SB04-RUNNER-LIFECYCLE` | Runner state and results must represent paused, canceled, and stopped operations. | `bundle://proof/SB04/transcripts/webglrunlib-focused-tests.txt`; `bundle://proof/SB04/transcripts/source-assertion-runner-lifecycle-scan.txt`. | Passed. |
| `SB04-CANCELED-NOT-COMPLETED` | A frame canceled during apply must not append its stages to `CompletedStageIds`. | `Runner_cancellation_during_frame_apply_does_not_mark_canceled_frame_completed` in `bundle://proof/SB04/transcripts/webglrunlib-focused-tests.txt`. | Passed. |
| `SB04-DIAGNOSTICS` | Lifecycle state, reason, counters, and canceled stage IDs must be visible to hosts. | `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunExecutionContracts.cs`; `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunRuntimeSnapshot.cs`. | Passed. |
| `SB04-DOMAIN-NEUTRAL` | WebGlRunLib contracts remain generic and browser/Economy neutral. | `bundle://proof/SB04/transcripts/domain-neutrality-scan.txt`; `bundle://proof/SB04/transcripts/anti-stub-audit.txt`. | Passed. |

## Semantic Adequacy Gate

| Gate item | SB04 result |
| --- | --- |
| Shallow-pass trap | A runner could expose Pause/Cancel/Stop methods but still complete the frame after cancellation. |
| Adversarial negative proof | Focused test cancels the token during frame apply and proves completed stage IDs remain empty. |
| Semantic positive proof | Focused test proves explicit Pause, Cancel, and Stop clear pending/active state and record lifecycle diagnostics. |
| Anti-stub audit | `bundle://proof/SB04/transcripts/anti-stub-audit.txt`. |
| Raw-note literal closure | F11 is solved at the reusable WebGlRun runner contract layer. |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `PlaybackLifecycleState` | Runner and playback controller lifecycle helpers. | Execution result, execution state, playback state, runtime snapshot. | `idle` by default; explicit controls set paused/canceled/stopped. | Tests prove each transition. |
| `LastPlaybackStopReason` | Lifecycle command reason or cancellation guard reason. | Diagnostics and host-facing result/snapshot objects. | Rewritten by each lifecycle transition. | Tests assert explicit and cancellation reasons. |
| Lifecycle counters | Runner and controller lifecycle helpers. | Diagnostics and snapshots. | Counts pause/cancel/stop transitions independently. | Tests assert one count for each transition. |
| `CanceledStageIds` | Runner cancel/cancellation helper. | Execution result and runner state. | Records canceled active/pending stages; reset clears state. | Cancellation-during-apply test proves canceled stage IDs exist while completed stages remain empty. |
