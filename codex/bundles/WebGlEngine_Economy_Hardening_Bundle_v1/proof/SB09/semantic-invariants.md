# SB09 Semantic Invariants

Subbundle: `SB09-webglrunlib-runtime-integration`
Status: `Completed`

## Invariants

| Invariant ID | Requirement | Expected behavior | Disallowed shallow implementation | Negative proof | Positive proof |
| --- | --- | --- | --- | --- | --- |
| SB09-ADAPTER-001 | REQ-011 | WebGlRun frames execute through a generic runner/adapter path into a WebGlLib `WebGlSceneCommandBatch`. | Route code calling `WebGlSceneView.ApplyCommandBatchAsync` directly while bypassing the run adapter. | `bundle://proof/SB09/transcripts/failing-first-batch-diagnostics.txt` | `bundle://proof/SB09/transcripts/passing-webglrunlib-tests.txt`, `bundle://proof/SB09/transcripts/browser-run-playback-batch-proof.json` |
| SB09-BATCH-001 | REQ-011, REQ-015 | Large generic frames expose batch diagnostics proving many commands are batched and not applied as per-object interop loops. | Showing moving objects without `batchCommandCount`, `batchStageCount`, normalization counts, or `interopCallsAvoided`. | `bundle://proof/SB09/transcripts/failing-first-batch-diagnostics.txt` | `bundle://proof/SB09/transcripts/passing-batch-diagnostics-adapter-test.txt`, `bundle://proof/SB09/transcripts/browser-run-playback-batch-proof.json` |
| SB09-BROWSER-001 | REQ-015 | WebGlSandbox has a generic non-domain browser route with deterministic controls, reset/cancel behavior, proof snapshot export, and diagnostics JSON. | A static/sample-only route with no real WebGL scene or no browser proof. | `bundle://proof/SB09/transcripts/failing-first-batch-diagnostics.txt` | `bundle://proof/SB09/browser/sb09-run-playback-batch-frame.png`, `bundle://proof/SB09/transcripts/browser-run-playback-batch-proof.json` |
| SB09-BOUNDARY-001 | REQ-010 | WebGlLib remains independent of WebGlRunLib and Economy; WebGlRunLib stays generic. | Adding run or domain references to WebGlLib to make playback convenient. | `bundle://proof/SB09/transcripts/sb09-anti-stub-and-boundary-scan.txt` | `bundle://proof/SB09/transcripts/passing-webgllib-boundary-audit.txt`, `bundle://proof/SB09/transcripts/passing-webglrunlib-boundary-audit.txt` |

## Shallow-Pass Trap

A fake SB09 pass could show a route with Play and Step buttons and a moving object while bypassing `WebGlRunDocumentRunner`, dropping the runtime batch diagnostics, or relying on a tiny two-object fixture that cannot prove batching. SB09 rejects that trap with a failing-first adapter test, a runner/adapter-backed sandbox route, a 24-actor generic batch frame, browser diagnostics JSON, and package boundary audits.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Run browser apply result | `WebGlRunBrowserApplyAdapter` | WebGlSandbox `/run-playback`, future domain bridge hosts, QA proof | Created per frame after runtime import/batch application and diagnostics capture. | `bundle://proof/SB09/transcripts/failing-first-batch-diagnostics.txt` |
| Runtime snapshot diagnostics dictionary | `WebGlRunBrowserApplyAdapter.BuildDiagnostics` | Browser diagnostics JSON, proof transcripts, downstream performance gates | Filled from WebGlLib diagnostics after each frame batch, bounded by adapter snapshot rules. | `bundle://proof/SB09/transcripts/failing-first-batch-diagnostics.txt` |
| Generic run playback route | `RunPlayback.razor` / `RunPlayback.razor.cs` | Browser proof and future cross-repo integration verification | Hosts a generic run document, deterministic controls, a large batch frame, and proof snapshots. | `bundle://proof/SB09/transcripts/browser-run-playback-batch-proof.json` |

## Reopen Triggers

- A later test contradicts these invariants.
- Browser proof shows runtime behavior that unit tests did not cover.
- A package/dependency scan reveals a boundary violation.
- A fallback path is used without explicit diagnostic mode.
- Economy bridge work requires adding domain semantics to Components instead of mapping into generic run contracts.

## Closure

SB09 closes the runtime/browser-playback slice of REQ-010, REQ-011, and REQ-015. SB10 remains responsible for the first real Economy bridge consumer mapping into these generic contracts without weakening the boundary.
