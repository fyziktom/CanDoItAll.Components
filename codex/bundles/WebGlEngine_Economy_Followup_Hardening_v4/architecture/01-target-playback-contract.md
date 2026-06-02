# Target playback contract

## Principles

- `WebGlRunDocumentRunner` owns deterministic replay.
- `WebGlRunBrowserApplyAdapter.ApplyAsync(WebGlRunFrameApplyResult)` applies exactly one already-validated frame result.
- A separate multi-frame method must apply `WebGlRunPlaybackResult.FramesToApply` in order and stop on first failure.
- `ApplyAsync(WebGlRunPlaybackResult)` must either become the multi-frame implementation or be deprecated/renamed so it cannot silently apply only `CurrentFrame` or the last frame.
- Economy UI must not directly convert `CurrentRunFrame` to a browser batch for seek/last operations unless the frame is explicitly marked absolute.

## Required result shape

Introduce or harden a result shape similar to:

```csharp
public sealed class WebGlRunBrowserPlaybackApplyResult
{
    public bool Success => Errors.Count == 0;
    public bool AppliedInitialScene { get; set; }
    public long TargetFrameIndex { get; set; }
    public List<long> AppliedFrameIndexes { get; set; } = [];
    public List<WebGlRunBrowserApplyResult> FrameResults { get; set; } = [];
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
}
```

The implementation must not proceed after reset failure, frame conversion errors, command batch failure, or cancellation.
