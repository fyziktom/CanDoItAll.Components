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

## Browser Playback

The browser integration path is:

```text
WebGlRunDocument -> WebGlRunDocumentRunner -> WebGlRunFrameApplyResult -> WebGlRunBrowserApplyAdapter -> WebGlSceneView.ApplyCommandBatchAsync
```

`WebGlSceneView` remains the WebGlLib boundary. The run layer calls its public import, batch, diagnostics, and proof-snapshot APIs; WebGlLib does not reference WebGlRunLib.

Use `WebGlRunDocumentRunner` when a host owns seek, step, first, last, or reset semantics. The runner applies the required replay sequence and stops before browser calls when validation or frame conversion fails.

Use `WebGlRunBrowserApplyAdapter.ApplyAsync(WebGlRunFrameApplyResult)` only for a direct single-frame apply. The legacy `ApplyAsync(WebGlRunPlaybackResult)` overload remains for compatibility with single-frame playback results, but it fails closed when `FramesToApply` contains multiple frames. Use `ApplyPlaybackAsync(WebGlRunPlaybackResult)` for multi-frame playback; it resets the scene once when required, applies each frame in the playback result order, returns per-frame results, and stops on the first failed frame.

The WebGlSandbox route `/run-playback` hosts a generic non-domain run document, deterministic controls, reset/cancel behavior, a large batch proof frame, and diagnostics JSON for command batching.

## Validators

Use `WebGlRunDocumentValidator` for run documents and `WebGlRunActionPlanValidator` for action plans before compilation or playback. The validators check schema, run id, timeline consistency, frame/stage shape, action structure, barrier policy, and obvious domain-term leakage.

Run the boundary audit before widening WebGlRunLib contracts:

```powershell
npm run webglrunlib:audit-boundary
```

## Boundary

`WebGlRunLib` must stay generic. It may compile frames and action stages into WebGlLib scene patches, motion commands, and command batches, but it must not contain economy account, ledger, market, price, buyer/seller, production-line, station, machine, work-order, or domain resource-accounting semantics.

Consuming packages own domain-specific mappings. For example, an Economy bridge can map its visual frames into these generic run contracts, but the Economy vocabulary stays outside this package.
