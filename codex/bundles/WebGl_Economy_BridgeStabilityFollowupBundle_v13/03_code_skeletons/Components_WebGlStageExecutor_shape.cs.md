# WebGl stage executor target shape

```csharp
// Comments in production code must be in English.
public interface IWebGlRunStageExecutor
{
    ValueTask<WebGlRunStageExecutionResult> ExecuteAsync(
        WebGlRunStageExecutionRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class WebGlRunStageExecutionRequest
{
    public WebGlRunDocument Document { get; set; } = new();
    public WebGlRunFrame Frame { get; set; } = new();
    public bool DeterministicMode { get; set; } = true;
}

public sealed class WebGlRunStageExecutionResult
{
    public long FrameIndex { get; set; }
    public List<WebGlSceneCommandBatchResult> BatchResults { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
    public List<string> Errors { get; set; } = [];
}
```
