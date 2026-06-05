# CanDoItAll.Components.WebGlRunLib

Package version: `0.1.0`.

## Purpose

Generic run, frame, action, stage, planner, compiler, and playback contracts layered above `CanDoItAll.Components.WebGlLib`.

## Project Type

- SDK: `Microsoft.NET.Sdk`
- Target framework(s): `net10.0`
- Validation command:

```powershell
dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj
```

## References

Project references:

- `../CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj`

Direct package references:

- None

## Contracts

- `WebGlRunDocument` is the generic run document. It contains a run id, initial `WebGlSceneDocument`, `WebGlRunTimeline`, and generic metadata.
- `WebGlRunTimeline` owns ordered `WebGlRunFrame` entries with frame index and time.
- `WebGlRunFrame` owns generic `WebGlRunActionStage` entries plus direct scene patches or motions.
- `WebGlRunActionStage` groups scene patches and motion commands and projects barrier policy into WebGlLib command-batch stages.
- `WebGlRunAction` describes generic visual actions such as move, pose, symbol, wait, sequence, parallel, and scene patch application.
- `WebGlRunActionPlan` carries planned actions, direct patches/motions, object bindings, diagnostics, and metadata.
- `WebGlRunActionCompiler` and `WebGlRunActionPlanBatchCompiler` compile run actions into WebGlLib timelines and `WebGlSceneCommandBatch` payloads.
- `WebGlRunPlaybackController` and `WebGlRunDocumentRunner` provide deterministic seek, step, pause, resume, reset, and current-frame application contracts.
- `WebGlRunBrowserApplyAdapter` and `WebGlSceneViewBrowserRuntime` apply a frame through `WebGlSceneView.ApplyCommandBatchAsync`, capture runtime diagnostics, and export a typed `WebGlRunRuntimeSnapshot`.
- `WebGlRunObserverProof` compares an expected source `WebGlRunDocument` with a browser-loaded document hash and runtime/UI observation snapshot.

## Browser Playback

The browser integration path is:

```text
WebGlRunDocument -> WebGlRunDocumentRunner -> WebGlRunFrameApplyResult -> WebGlRunBrowserApplyAdapter -> WebGlSceneView.ApplyCommandBatchAsync
```

`WebGlSceneView` remains the WebGlLib boundary. The run layer calls its public import, batch, diagnostics, and proof-snapshot APIs; WebGlLib does not reference WebGlRunLib.

Use `WebGlRunDocumentRunner` when a host owns seek, step, first, last, or reset semantics. The runner applies the required replay sequence and stops before browser calls when validation or frame conversion fails.

Use `WebGlRunBrowserApplyAdapter.ApplyAsync(WebGlRunFrameApplyResult)` only for a direct single-frame apply. The legacy `ApplyAsync(WebGlRunPlaybackResult)` overload remains for compatibility with single-frame playback results, but it fails closed when `FramesToApply` contains multiple frames. Use `ApplyPlaybackAsync(WebGlRunPlaybackResult)` for multi-frame playback; it resets the scene once when required, applies each frame in the playback result order, returns per-frame results, and stops on the first failed frame.

Host Pause, Cancel, Stop, Reset, Dispose, and route-change handlers must cancel the host loop and call `WebGlSceneView.StopRuntimeActivityAsync(reason)` through `WebGlSceneViewBrowserRuntime`/`WebGlSceneView`. A C# paused flag is not enough proof if `queuedCommandStageCount`, `activeMotionCount`, or `queuedMotionCount` continue changing in browser diagnostics.

Use the host checklist in `docs/webgl/playback-hosting-and-troubleshooting.md` when integrating playback controls or debugging a scene that keeps moving after Pause.

The WebGlSandbox route `/run-playback` hosts a generic non-domain run document, deterministic controls, reset/cancel behavior, a large batch proof frame, and diagnostics JSON for command batching.

## Observer Proof

`WebGlRunObserverProof.Compare(expectedDocument, browserLoadedDocument, observerSnapshot)` is the generic browser-observer boundary check. It computes deterministic document hashes from the run id, initial scene content hash, timeline identity, and generic metadata, then combines that comparison with browser runtime/UI diagnostics.

Observer proof can validate a visual claim only when:

- the expected and browser-loaded document hashes match
- browser runtime proof was exercised
- UI proof was exercised
- no runtime or UI errors were reported

If the browser document hash differs, the report adds `browser-document-hash-mismatch`. If runtime or UI proof fails, the report status is `observer-failed`. Consumers should keep their domain state, simulation state, or headless artifacts as their own source of truth and treat the observer proof as visual evidence only.

## Domain Driver Versioning

`IWebGlRunDomainMappingDriver.DriverVersion` is a compatibility contract for a driver-owned action vocabulary and metadata scrubber policy. Patch releases may add validation warnings or metadata cleanup that preserves existing mappings. Minor releases may add driver action kinds when every new mapping resolves to an approved `WebGlRunActionKinds` value. Major releases are required for changed meanings, removed driver actions, changed source-provenance policy, or a different manifest schema.

`WebGlRunDomainMappingDriverManifest.CurrentSchemaVersion` and `DriverHash` are the freeze gate for runtime and evidence consumers. A host should record both values with generated run documents and reject cached evidence when either value changes unexpectedly. Generic Components code must not infer domain semantics from driver ids, display names, or driver action names; it may only validate that the manifest maps into the approved generic action vocabulary.

## Validators

Use `WebGlRunDocumentValidator` for run documents and `WebGlRunActionPlanValidator` for action plans before compilation or playback. The validators check schema, run id, timeline consistency, frame/stage shape, action structure, barrier policy, and obvious domain-term leakage.

Run the boundary audit before widening WebGlRunLib contracts:

```powershell
npm run webglrunlib:audit-boundary
```

## Boundary

`WebGlRunLib` must stay generic. It may compile frames and action stages into WebGlLib scene patches, motion commands, and command batches, but it must not contain economy account, ledger, market, price, buyer/seller, production-line, station, machine, work-order, or domain resource-accounting semantics.

Consuming packages own domain-specific mappings. For example, an Economy bridge can map its visual frames into these generic run contracts, but the Economy vocabulary stays outside this package.
