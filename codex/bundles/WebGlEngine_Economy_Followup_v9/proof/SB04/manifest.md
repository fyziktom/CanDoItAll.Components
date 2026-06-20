# Proof manifest - SB04

Status: completed

## Scope

SB04 clarifies the command-batch lifecycle boundary between JS runtime results and the C# browser apply adapter. Default browser observer applies now use `ApplyCommandBatchAndWaitAsync` when no explicit idle wait policy is configured. Explicit idle policies still preserve non-waiting `ApplyCommandBatchAsync` behavior plus separate idle waits. Adapter snapshots surface command lifecycle and runtime idle blocker diagnostics.

## Changed files

Changed-file hashes:

- `bundle://proof/SB04/transcripts/changed-file-hashes.txt`

Production files:

- `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserRuntimeContracts.cs`
- `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlSceneViewBrowserRuntime.cs`
- `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs`

Test/proof files:

- `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunBrowserApplyAdapterTests.cs`
- `bundle://proof/SB04/browser/command-batch-lifecycle-proof.mjs`
- `bundle://proof/SB04/semantic-invariants.md`

## Proof artifacts

- Adapter scheduled-vs-settled tests: `bundle://proof/SB04/transcripts/webglrunlib-tests.txt`
- WebGlLib regression proof: `bundle://proof/SB04/transcripts/webgllib-tests.txt`
- Sandbox build proof: `bundle://proof/SB04/transcripts/webglsandbox-build.txt`
- Browser proof transcript: `bundle://proof/SB04/transcripts/command-batch-lifecycle-playwright.txt`
- Browser assertions/diagnostics JSON: `bundle://proof/SB04/browser/command-batch-lifecycle-assertions.json`
- Browser screenshot: `bundle://proof/SB04/browser/command-batch-lifecycle-after.png`
- Browser console log: `bundle://proof/SB04/browser/command-batch-lifecycle-console.log`
- Source assertions: `bundle://proof/SB04/transcripts/source-assertions.txt`
- Anti-stub scan: `bundle://proof/SB04/transcripts/anti-stub-scan.txt`
- Bundle validator transcript: `bundle://proof/SB04/transcripts/bundle-validator.txt`
- Sandbox server logs: `bundle://proof/SB04/transcripts/webgl-sandbox-sb04.out.txt`, `bundle://proof/SB04/transcripts/webgl-sandbox-sb04.err.txt`

## Semantic adequacy gate

- Shallow-pass trap: a browser apply adapter could report success while the JS command result was only `scheduled`, leaving motion/barrier work for a later observer to stumble over.
- Positive default proof: `Adapter_default_apply_uses_command_batch_and_wait_for_settled_result` verifies default C# applies call the wait-capable runtime method and expose settled lifecycle diagnostics.
- Negative/configured proof: `Adapter_configured_idle_policy_preserves_scheduled_result_and_idle_blockers` verifies explicit wait policies keep non-waiting scheduled results visible and surface `runtimeIdleBlockers`.
- Browser proof: `command-batch-lifecycle-proof.mjs` calls `applyCommandBatch` with a `wait-for-active-motions` barrier and observes `lifecycleState=scheduled`, `settled=false`, and runtime blockers; then calls `applyCommandBatchAndWait` with the same barrier shape and observes `lifecycleState=settled`, `settled=true`, runtime idle true, and final active/queued blockers empty.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative / proof citation |
| --- | --- | --- | --- | --- |
| `ApplyCommandBatchAndWaitAsync` runtime contract | `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserRuntimeContracts.cs` | `WebGlRunBrowserApplyAdapter`, `WebGlSceneViewBrowserRuntime` | Exposes the existing WebGlSceneView JS wait-capable command-batch API to RunLib adapters. | `bundle://proof/SB04/transcripts/webglrunlib-tests.txt` |
| Default wait-capable apply path | `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs` | Browser observer applies with no explicit idle policy | Calls `ApplyCommandBatchAndWaitAsync` using configured timeout/poll defaults and marks settled lifecycle from the JS result. | `bundle://proof/SB04/transcripts/webglrunlib-tests.txt` |
| Configured non-waiting apply path | `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs` | Browser observer applies with `AfterEachFrame` or `AfterPlayback` policies | Preserves `ApplyCommandBatchAsync` and follows with separate idle waits per policy, so scheduled lifecycle and blockers stay visible. | `bundle://proof/SB04/transcripts/webglrunlib-tests.txt`, `bundle://proof/SB04/browser/command-batch-lifecycle-assertions.json` |
| Command lifecycle and blocker snapshot diagnostics | `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs` | `WebGlRunRuntimeSnapshot.Diagnostics` | Copies `commandLifecycleState`, `commandSettled`, `runtimeIdle`, `runtimeIdleTimedOut`, `runtimeIdleElapsedMs`, `runtimeIdleBlockers`, and `runtimeIdleRequired` from command result diagnostics/metadata. | `bundle://proof/SB04/transcripts/source-assertions.txt` |

## Closure

SB04 passes. Adapter tests prove default settled apply and configured scheduled apply behavior. Browser proof route: `http://localhost:5298/run-playback`, viewport `1920x1080`, wait-for-active-motions barrier, non-waiting result scheduled with runtime blockers, wait-capable result settled with final blockers `[]`, and no disallowed console errors.
