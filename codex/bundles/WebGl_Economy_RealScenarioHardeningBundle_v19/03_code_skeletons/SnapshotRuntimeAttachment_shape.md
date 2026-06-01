# Snapshot runtime attachment shape

```csharp
public sealed class SimulationSnapshotRuntimeAttachment
{
    public string RuntimeKind { get; set; } = "webgl-run";
    public long CurrentFrameIndex { get; set; }
    public List<string> ActiveMotionIds { get; set; } = [];
    public List<string> QueuedMotionIds { get; set; } = [];
    public List<string> ActiveStageIds { get; set; } = [];
    public List<string> PendingStageIds { get; set; } = [];
    public Dictionary<string, string> Diagnostics { get; set; } = [];
}
```
