# WebGL Playback Hosting And Troubleshooting

This guide is for hosts that use `CanDoItAll.Components.WebGlRunLib` with a `WebGlSceneView` browser runtime. It stays domain-neutral: consuming applications map their own data into generic scene and run documents before calling these APIs.

## Host Integration Recipe

1. Render a `WebGlSceneView` and include scene runtime assets through `WebGlLibHeadAssets` and `WebGlLibBodyAssets`.
2. Wrap the scene view with `WebGlSceneViewBrowserRuntime`.
3. Create a `WebGlRunBrowserApplyAdapter` with the runtime and the reset scene document.
4. Create a `WebGlRunDocumentRunner` for load, seek, step, pause, cancel, stop, and current-frame execution.
5. Keep a host-owned playback generation or cancellation token. Every Play/Step/Seek/Reset creates or validates the current generation; Pause, Cancel, Stop, Reset, Dispose, and route changes invalidate it.
6. On Pause, Cancel, Stop, Reset, and Dispose, cancel host-side work first, then call `WebGlSceneView.StopRuntimeActivityAsync(reason)`.
7. Use `ApplyPlaybackAsync(WebGlRunPlaybackResult, cancellationToken)` for multi-frame browser replay. Use `ApplyAsync(WebGlRunFrameApplyResult)` only for a direct single-frame apply.
8. Store browser diagnostics after every apply or stop so proof can compare C# state with runtime state.

Minimal shape:

```csharp
var runtime = new WebGlSceneViewBrowserRuntime(sceneView);
var adapter = new WebGlRunBrowserApplyAdapter(runtime, resetSceneDocument);
var runner = new WebGlRunDocumentRunner(frameApplier);

await runner.LoadAsync(runDocument, cancellationToken);
WebGlRunExecutionResult step = await runner.StepForwardAsync(cancellationToken);
WebGlRunBrowserPlaybackApplyResult browser = await adapter.ApplyPlaybackAsync(playback, cancellationToken);

playbackCancellation.Cancel();
await runner.PauseAsync("user pause", CancellationToken.None);
await sceneView.StopRuntimeActivityAsync("user pause");
```

## Pause Bug Troubleshooting Checklist

Use this checklist when the UI says playback paused but the scene keeps moving or later frames still apply.

- Confirm the host cancelled its playback loop or invalidated the active playback generation before returning from Pause.
- Confirm Pause, Cancel, Stop, Reset, Dispose, and route changes call `StopRuntimeActivityAsync(reason)`.
- Confirm `queuedCommandStageCount`, `activeMotionCount`, and `queuedMotionCount` are `0` after the pause deadline.
- Confirm `runtimeStopCount` increments and `lastRuntimeStopReason` matches the host action, such as `Paused.` or `user pause`.
- Confirm frame, stage, and motion counters stay stable after the pause deadline.
- Confirm stale motion-completed or command-completed callbacks cannot overwrite the paused status.
- Confirm the browser proof uses the public `WebGlSceneView`/`window.CanDoItAll.webglScene` facade, not a private runtime helper.
- Confirm a second stop call is safe and keeps the runtime idle.

If any runtime queue or motion counter continues to change after Pause, treat the bug as a cross-layer cancellation failure. A C# flag alone is not sufficient proof.

## Replay Modes

Hosts that can prove the browser already holds a stable frame may apply a contiguous forward step as an incremental single-frame replay. Manual apply, first/last seek, backward seek, non-contiguous seek, scenario load, reset, and failed apply recovery should use full deterministic replay from the reset scene through the target frame.

Record these diagnostics for proof:

- requested frame indexes;
- target frame index;
- replay mode, `incremental` or `full`;
- whether a scene reset was applied;
- last stable frame index;
- applied frame, stage, motion, and patch counts;
- cancellation or failure reason when present.

## Package-Mode Proof Rules

Package consumers should validate against freshly packed packages, not older private-feed packages or accidental project references.

- Pack with a unique proof suffix.
- Restore from a proof NuGet.config whose first source is the fresh package output.
- Use an isolated `NUGET_PACKAGES` directory.
- Pass explicit package-mode properties and full package versions to restore and build.
- Build with `--no-restore` after the controlled restore.
- Treat stale package feed warnings or project-reference fallback as proof failures.

## Proof Rules

Playback proof should include:

- a command transcript;
- browser route and viewport;
- browser actions;
- JSON assertions with an `assertions` object;
- screenshot when UI/runtime behavior is involved;
- diagnostics before and after Pause/Cancel/Stop;
- source assertion scan for changed contracts.

Completed bundle proof must not use blank transcripts or screenshot-only browser evidence.
