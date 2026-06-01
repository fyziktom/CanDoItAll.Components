# Components browser apply adapter shape

```csharp
public interface IWebGlRunBrowserApplyAdapter
{
    ValueTask<WebGlRunBrowserApplyResult> ApplyAsync(
        WebGlRunFrameApplyResult frameApplyResult,
        CancellationToken cancellationToken = default);
}

public sealed class WebGlRunBrowserApplyResult
{
    public bool Success => Errors.Count == 0;
    public long FrameIndex { get; set; }
    public int AppliedStageCount { get; set; }
    public int AppliedPatchCount { get; set; }
    public int AppliedMotionCount { get; set; }
    public WebGlRunRuntimeSnapshot RuntimeSnapshot { get; set; } = new();
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
}
```

Implementation note: this must remain generic and must not reference Economy.
