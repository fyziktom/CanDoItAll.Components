# Components stage barrier shape

```csharp
namespace CanDoItAll.Components.WebGlRunLib;

public static class WebGlRunStageBarrierKinds
{
    public const string None = "none";
    public const string WaitSeconds = "wait-seconds";
    public const string WaitForActiveMotions = "wait-for-active-motions";
    public const string WaitForObjectMotions = "wait-for-object-motions";
    public const string WaitForRenderIdle = "wait-for-render-idle";
    public const string WaitForEvent = "wait-for-event";
}

public sealed class WebGlRunStageBarrier
{
    public string Kind { get; set; } = WebGlRunStageBarrierKinds.WaitSeconds;
    public double WaitSeconds { get; set; }
    public List<string> ObjectIds { get; set; } = [];
    public string EventKey { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = [];
}
```
