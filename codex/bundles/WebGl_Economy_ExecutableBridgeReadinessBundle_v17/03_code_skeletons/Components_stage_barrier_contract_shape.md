# Components stage barrier contract shape

```csharp
public sealed class WebGlRunStageBarrier
{
    public string Policy { get; set; } = WebGlRunStageBarrierPolicies.None;
    public double WaitSeconds { get; set; }
    public List<string> ObjectIds { get; set; } = [];
    public string EventId { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public static class WebGlRunStageBarrierPolicies
{
    public const string None = "none";
    public const string WaitSeconds = "wait-seconds";
    public const string WaitForActiveMotions = "wait-for-active-motions";
    public const string WaitForObjectMotions = "wait-for-object-motions";
    public const string WaitForRenderIdle = "wait-for-render-idle";
    public const string WaitForEvent = "wait-for-event";
}
```
