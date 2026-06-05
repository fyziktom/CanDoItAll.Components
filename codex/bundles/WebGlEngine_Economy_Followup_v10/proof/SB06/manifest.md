# Proof manifest SB06

Status: completed

Required proof: Unit + browser proof for scheduled vs settled; no false success while barriers/motions remain.

## Artifacts

- WebGlLib test transcript: `bundle://proof/SB06/components-webgllib-phase-b-test.txt`
- WebGlRunLib test transcript: `bundle://proof/SB05/components-webglrun-phase-b-test.txt`
- Playwright browser assertions: `bundle://proof/SB07/playwright-runtime-state-assertions.txt`
- Source scan: `bundle://proof/SB07/phase-b-source-scan.txt`
- Source hashes: `bundle://proof/SB06/phase-b-source-hashes.txt`
- Anti-stub audit: `bundle://proof/SB06/anti-stub-scan.txt`

## Source Assertions

- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/26-webgl-scene-command-batch.js` uses `applyCommandBatchAndWait` to wait for runtime idle and annotate settled, scheduled, or failed lifecycle state.
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs` copies command lifecycle, settled state, idle flags, and blockers into the runtime snapshot.
- `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunBrowserApplyAdapterTests.cs` proves default settled apply, configured scheduled apply, idle timeout, cancellation, and transaction-stop behavior.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Command lifecycle diagnostics | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/26-webgl-scene-command-batch.js` | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs` | Runtime command result records settled/scheduled/failed; adapter records it in `WebGlRunRuntimeSnapshot.Diagnostics`. | `bundle://proof/SB05/components-webglrun-phase-b-test.txt` includes scheduled vs settled assertions. |
| Runtime idle blockers | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/40-webgl-scene-runtime-idle.js` | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs`; RunPlayback inspector | Idle wait returns blockers; adapter/browser proof fails or annotates when blockers remain. | `bundle://proof/SB07/playwright-runtime-state-assertions.txt` proves the route has no blockers after pause. |

## Gate Result

Pass. Unit tests prove scheduled vs settled behavior and the browser proof confirms no false success while browser blockers remain after stop/drain.
