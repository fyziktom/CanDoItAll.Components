# SB04 proof manifest

Status: Completed 2026-06-02.

## Changed file hashes

| SHA-256 | Path |
| --- | --- |
| `d8e06a77a944aabfdec778cba3da3903c3cb8f5552c747467b77173395b5025e` | `src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneRevisionPolicy.cs` |
| `b7f9da5292423cdb055a153e70498509ed46997754a8d0485581ad972015c047` | `src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunBrowserApplyAdapter.cs` |
| `0a6c5c0f02d2e2aa144e5aab6b5cb7b728098b7beb1a01b6df9450a353c18415` | `tests/CanDoItAll.Components.WebGlLib.Tests/WebGlSceneDocumentSerializerTests.cs` |
| `96c9c39239f8074ccf98e1cbbf48dd95aec08119e23ee9139d4bd18925e30f4b` | `tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunBrowserApplyAdapterTests.cs` |
| `d6826211cdb9ebc3513af375690f2d9c069cd552d55df7cf39476ff3b885fd5a` | `docs/webgl/run-layer-boundary.md` |

## Command transcripts

| Command | Transcript | Exit | Result |
| --- | --- | --- | --- |
| `dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj --filter FullyQualifiedName~WebGlSceneDocumentSerializerTests` before implementation | `bundle://proof/SB04/transcripts/failing-first-revision-policy-tests.txt` | 1 | Failing-first: stale `UiState.Revision` changed document identity and normalization left UI revision at `2` instead of canonical `7`. |
| `dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj --filter FullyQualifiedName~WebGlRunBrowserApplyAdapterTests.Adapter_reset_treats_initial_scene_runtime_options_as_external` before implementation | `bundle://proof/SB04/transcripts/failing-first-runtime-options-reset-test.txt` | 1 | Failing-first: reset import preserved document runtime options, passing `continuous` to the fake runtime. |
| Focused serializer policy tests after implementation | `bundle://proof/SB04/transcripts/passing-revision-policy-tests.txt` | 0 | 14 serializer tests passed. |
| Focused fake-runtime reset policy test after implementation | `bundle://proof/SB04/transcripts/passing-runtime-options-reset-test.txt` | 0 | Reset strips runtime options to defaults and reports the external-options warning. |
| `dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj` | `bundle://proof/SB04/transcripts/components-webgllib-tests.txt` | 0 | 46 WebGlLib tests passed. |
| `dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj` | `bundle://proof/SB04/transcripts/components-webglrunlib-tests.txt` | 0 | 37 WebGlRunLib tests passed. |
| `dotnet build CanDoItAll.Components.slnx` | `bundle://proof/SB04/transcripts/components-build.txt` | 0 | Components solution build passed with 0 warnings and 0 errors. |
| `npm run webgllib:audit-boundary` | `bundle://proof/SB04/transcripts/components-webgllib-boundary-audit.txt` | 0 | WebGlLib boundary audit passed. |
| `npm run webglrunlib:audit-boundary` | `bundle://proof/SB04/transcripts/components-webglrunlib-boundary-audit.txt` | 0 | WebGlRunLib boundary audit passed. |
| `python scripts/validate_bundle.py --stage prepared --profile initiative` | `bundle://proof/SB04/transcripts/bundle-validator-after-sb04.txt` | 0 | Bundle validator passed after SB04 proof/doc updates. |
| SB04 proof placeholder scan | `bundle://proof/SB04/transcripts/sb04-proof-placeholder-scan.txt` | 0 | No stale SB04 proof placeholders found. |

## Browser artifacts

N/A for SB04. The changed behavior is serializer policy and adapter reset input shape; browser-visible UI was not changed. The required runtime reset proof is covered by fake-runtime adapter tests.

## Source assertions

`bundle://proof/SB04/transcripts/source-policy-assertions.txt` records:

- `WebGlSceneRevisionPolicy.Normalize` calls `Commit(scene, Resolve(scene))`, sharing the same mirror-write path as scene mutation.
- `docs/webgl/run-layer-boundary.md` documents `Scene Revision Policy` and `Browser Reset Runtime Options`.
- `WebGlRunBrowserApplyAdapter` emits the runtime-options warning and imports a reset document whose `RuntimeOptions` is `new WebGlRuntimeOptions()`.

## Anti-stub audit

`bundle://proof/SB04/transcripts/changed-file-placeholder-scan.txt` passed with no `TODO`, `stub`, `placeholder`, or `NotImplementedException` markers in SB04 production/doc changed files. `bundle://proof/SB04/transcripts/sb04-proof-placeholder-scan.txt` passed with no stale SB04 proof placeholders.

## Production Behavior Artifact Matrix

| Artifact / signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Canonical scene revision | `WebGlSceneRevisionPolicy.Resolve` and `Commit` | Serializer, patch reducers, validators, downstream import/export | Normalized before hashing/serialization; mirrored to UI state when UI state is included | Failing-first serializer test showed stale UI revision remained divergent before fix. |
| UI revision mirror | `WebGlSceneRevisionPolicy.Commit` | Backward-compatible UI-state consumers | Mirrors canonical `Scene.Revision`; reset to default only when UI state is excluded | Failing-first serializer test expected mirror `7` but observed `2`. |
| Scene-only browser reset document | `WebGlRunBrowserApplyAdapter.CreateSceneResetDocument` | `IWebGlRunBrowserRuntime.ImportSceneAsync` | Created per reset; strips runtime options and diagnostics to defaults while preserving scene content | Failing-first fake-runtime test observed imported `continuous` render mode before fix. |
| External runtime-options warning | `WebGlRunBrowserApplyAdapter` | Caller result and runtime snapshot warnings | Emitted when a reset source document carries non-default runtime options | Passing fake-runtime test asserts warning text and default imported runtime options. |

## Gate decision

Pass. SB04 defines and enforces the canonical revision policy and browser reset runtime-option policy, documents both policies, records failing-first and passing semantic proof, and preserves Components package boundaries.
