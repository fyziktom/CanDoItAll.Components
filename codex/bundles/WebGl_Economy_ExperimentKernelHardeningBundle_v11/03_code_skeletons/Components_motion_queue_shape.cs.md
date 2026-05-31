# Components motion queue shape

```csharp
public sealed class WebGlObjectMotionQueuePolicy
{
    public string ObjectId { get; set; } = string.Empty;
    public string QueueMode { get; set; } = "replace"; // replace, append, sequence
    public bool StartNextAfterCompletion { get; set; } = true;
}

public sealed class WebGlRunStageExecutionResult
{
    public string StageId { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public List<WebGlSceneCommandResult> CommandResults { get; set; } = [];
    public List<string> Errors { get; set; } = [];
}
```

Rules:

- `append` must not mean "run all motions for the same object at once".
- ordered stages must wait for motion completion or explicit stage duration.
- WebGlLib can expose primitives; WebGlRunLib owns temporal orchestration.
```
