# Proof manifest SB05

Status: completed
Completed: 2026-06-03

- Objective: Multi-frame ApplyPlayback transaction/cancellation semantics.
- Gate: Passed. Failures and cancellations report target frame, last applied frame, cancellation reason when applicable, transaction policy, and failure snapshot; later frames are not applied after the first failed/canceled frame.
- Owned findings: F05.

## Changed-file hashes

| Path | SHA-256 | Purpose |
| --- | --- | --- |
| `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs` | `57913EBF99DE963197438AD53A734828C8B2D1911CECF84706A14918D9B77CD0` | Adds transaction failure/cancellation handling and failure snapshots. |
| `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyResults.cs` | `F57E1ACE6DFA618D3FC9D3A609C507F2823D500ABB27BFBDD48D91D0946865A0` | Adds transaction policy, last-applied frame, cancellation reason, canceled flag, and failure snapshot fields. |
| `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunBrowserApplyAdapterTests.cs` | `7EDE4CAA8BAD299760516C247DED486D5E98D021BB2FF9DDF7E64143550F1D5F` | Adds transaction/cancellation assertions for multi-frame playback apply. |

## Proof artifact hashes

| Path | SHA-256 | Purpose |
| --- | --- | --- |
| `bundle://proof/SB05/transcripts/failing-first-applyplayback-transaction-gap.txt` | `5D544816C1951C47DBD48F91B76F046B7E9FCBE10BACDE0F1F514153F861BB4C` | Failing-first source-contract gap proof. |
| `bundle://proof/SB05/transcripts/webglrunlib-focused-tests.txt` | `8C58F2D222223A11C871554D79514C0C694DE18915DEFAF632B5272B89C6A7A0` | WebGlRunLib focused tests passed, 56 tests. |
| `bundle://proof/SB05/transcripts/components-build-after-applyplayback-transaction.txt` | `5BB3A56FB133A4D22570EE24CBF67189816995B37AC7EEF199E1D508C202397F` | Solution build passed with zero warnings/errors. |
| `bundle://proof/SB05/transcripts/source-assertion-applyplayback-transaction-scan.txt` | `12A863FFE17729E0AFF979B1D21E995ED84A6B8631BCA616910C31C6ACF836C1` | Source scan proving transaction and cancellation contracts exist. |
| `bundle://proof/SB05/transcripts/domain-neutrality-scan.txt` | `805C3383F8C1CA8651B359C8434D211C2299CA99CE57914B93AADD9202E6B455` | Domain-neutrality scan for changed files. |
| `bundle://proof/SB05/transcripts/anti-stub-audit.txt` | `1A4483082DD638DA94A1F66A24D7BE784CE68866D13FD88DBC316E2AC4452A81` | Anti-stub audit for changed files. |
| `bundle://proof/SB05/transcripts/proof-hygiene-inventory.txt` | `A8518A8409E0599E5C2A919EAAA4B757914563FADC5BB22572143068A6D88AF0` | SB05 inventory: non-empty transcripts and no browser artifacts required for library-only transaction work. |

## Command transcripts

- `bundle://proof/SB05/transcripts/failing-first-applyplayback-transaction-gap.txt`: baseline scan showed no transaction policy, last-applied frame, cancellation reason, or failure snapshot contract.
- `bundle://proof/SB05/transcripts/webglrunlib-focused-tests.txt`: `dotnet test tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj --no-restore` passed 56 tests.
- `bundle://proof/SB05/transcripts/components-build-after-applyplayback-transaction.txt`: solution build passed with zero warnings and zero errors.
- `bundle://proof/SB05/transcripts/source-assertion-applyplayback-transaction-scan.txt`: source scan proves transaction fields and cancellation helpers exist.

## Test proof summary

`Adapter_apply_playback_stops_on_first_failed_frame` proves frame 3 is not applied after frame 2 fails, `TargetFrameIndex = 3`, `LastAppliedFrameIndex = 1`, `FailedFrameIndex = 2`, transaction policy is `StopOnFirstFailure`, and the failure snapshot carries the same metadata.

`Adapter_apply_playback_reports_cancellation_and_does_not_apply_later_frames` proves cancellation during frame 2 stops before frame 3, records `CancellationReason = "frame apply canceled"`, keeps `LastAppliedFrameIndex = 2`, and writes target/last-applied/cancellation fields into `FailureSnapshot.Diagnostics`.

## Semantic adequacy gate

- Shallow-pass trap: stopping on failure without target/last-applied/cancellation metadata would still leave hosts unable to reason about partial application.
- Adversarial negative proof: focused cancellation test cancels inside the runtime fake during frame 2 and asserts frame 3 is never applied.
- Semantic positive proof: focused failure and cancellation tests prove transaction reporting for reset/frame/cancel paths.
- Browser proof: not required for SB05 because the change is a WebGlRunLib transaction result contract with no new UI/browser route.
- Anti-stub audit: `bundle://proof/SB05/transcripts/anti-stub-audit.txt`.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `TransactionPolicy` | `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyResults.cs`. | Browser playback apply hosts and failure snapshots. | Defaults to `StopOnFirstFailure` for every `ApplyPlaybackAsync` result. | Frame-failure test proves frame 3 is not applied after frame 2 fails. |
| `LastAppliedFrameIndex` | `ApplyPlaybackAsync` updates after each successful frame apply. | Hosts, diagnostics, and failure snapshots. | Null before first success; set to latest successful frame before a failure/cancel. | Failure test records 1 before frame 2 failure; cancellation test records 2 before stopping. |
| `CancellationReason` and `Canceled` | `CompletePlaybackCancellationAsync`. | Hosts and failure snapshots. | Set only for cancellation exits. | Cancellation test proves reason and canceled flag are recorded. |
| `FailureSnapshot` | Reset failure, frame failure, and cancellation paths. | Hosts and proof assertions. | Created at the stop point with target/last-applied/failed-frame/transaction metadata. | Tests assert snapshot diagnostics for failed and canceled playback. |
