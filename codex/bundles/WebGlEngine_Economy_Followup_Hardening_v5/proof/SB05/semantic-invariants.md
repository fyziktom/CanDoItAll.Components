# Semantic invariants SB05

Status: completed

## Invariants

| ID | Requirement | Evidence | Closure |
| --- | --- | --- | --- |
| `SB05-STOP-FIRST-FAILURE` | Multi-frame apply must not continue after the first failed frame. | `bundle://proof/SB05/transcripts/webglrunlib-focused-tests.txt`. | Passed. |
| `SB05-PARTIAL-REPORT` | Failures must report target frame, failed frame, and last applied frame. | `Adapter_apply_playback_stops_on_first_failed_frame`; `bundle://proof/SB05/transcripts/source-assertion-applyplayback-transaction-scan.txt`. | Passed. |
| `SB05-CANCELLATION-REPORT` | Cancellation must report cancellation reason and stop later frames. | `Adapter_apply_playback_reports_cancellation_and_does_not_apply_later_frames`. | Passed. |
| `SB05-DOMAIN-NEUTRAL` | Transaction semantics remain generic and do not introduce Economy-specific behavior. | `bundle://proof/SB05/transcripts/domain-neutrality-scan.txt`; `bundle://proof/SB05/transcripts/anti-stub-audit.txt`. | Passed. |

## Semantic Adequacy Gate

| Gate item | SB05 result |
| --- | --- |
| Shallow-pass trap | A result could stop on failure but omit partial-application metadata. |
| Adversarial negative proof | Cancellation test cancels during frame 2 and proves frame 3 is not applied. |
| Semantic positive proof | Failure and cancellation tests assert target/last-applied/failed-frame/cancellation snapshot diagnostics. |
| Anti-stub audit | `bundle://proof/SB05/transcripts/anti-stub-audit.txt`. |
| Raw-note literal closure | F05 is solved for WebGlRunLib multi-frame browser apply transaction reporting. |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `TransactionPolicy` | Browser playback apply result. | Hosts and failure snapshots. | Defaults to `StopOnFirstFailure`. | Failure test proves later frame is not applied. |
| `LastAppliedFrameIndex` | `ApplyPlaybackAsync` after successful frame apply. | Hosts and failure snapshots. | Null until first success; then tracks latest success. | Failure and cancellation tests assert values. |
| `CancellationReason` | Cancellation helper. | Hosts and failure snapshots. | Set on cancellation exits. | Cancellation test asserts reason. |
| `FailureSnapshot` | Failure/cancellation exit paths. | Hosts and proof. | Created at transaction stop point. | Tests assert snapshot metadata. |
