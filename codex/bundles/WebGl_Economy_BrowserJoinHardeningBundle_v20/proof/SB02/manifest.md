# SB02 Proof Manifest

Status: Completed

## Scope

Generic Components browser apply adapter for `WebGlRunFrameApplyResult`.

## Changed File Hashes

- `bundle://proof/SB02/transcripts/changed-file-hashes.txt`

## Command Transcripts

- `bundle://proof/SB02/transcripts/webglrunlib-browser-adapter-tests.txt`
- `bundle://proof/SB02/transcripts/source-assertions.txt`
- `bundle://proof/SB02/transcripts/anti-stub-audit.txt`

## Source Assertions

- `repo://src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs` defines `IWebGlRunBrowserApplyAdapter`, fakeable `IWebGlRunBrowserRuntime`, typed `WebGlRunBrowserApplyResult`, and `WebGlSceneViewBrowserRuntime`.
- `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunBrowserApplyAdapterTests.cs` proves reset import, command batch application, patch/motion/stage counts, runtime diagnostics, barrier state, journal tail, and failure propagation.
- `bundle://proof/SB02/transcripts/source-assertions.txt` proves Components remains Economy-free in the touched WebGL surfaces.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative-test citation |
|---|---|---|---|---|
| `WebGlRunBrowserApplyResult` | `WebGlRunBrowserApplyAdapter.ApplyAsync` | Economy desktop sandbox page and browser smoke proof | Produced per applied frame/playback result | `Adapter_reports_runtime_failure_in_typed_result_and_snapshot` in `bundle://proof/SB02/transcripts/webglrunlib-browser-adapter-tests.txt` proves failure is not hidden. |
| `WebGlRunRuntimeSnapshot` browser fields | `WebGlRunBrowserApplyAdapter.BuildSnapshot` | Economy snapshot attachment and smoke artifacts | Captured from runtime diagnostics after frame apply | `Adapter_applies_frame_to_runtime_and_returns_counts_and_snapshot` proves journal/barrier/motion data is copied from fake runtime diagnostics. |

## Semantic Adequacy Evidence

- Failing-first equivalent: baseline prior to this subbundle lacked `IWebGlRunBrowserApplyAdapter` and typed browser apply result.
- Semantic positive proof: 27 WebGlRunLib tests passed, including three adapter tests.
- Adversarial negative proof: runtime failure test returns `Success == false` and carries errors into the snapshot.
- Anti-stub audit: `bundle://proof/SB02/transcripts/anti-stub-audit.txt`.

## Closure

SB02 passed. SB03/SB04 can rely on the generic adapter and runtime diagnostic shape.
