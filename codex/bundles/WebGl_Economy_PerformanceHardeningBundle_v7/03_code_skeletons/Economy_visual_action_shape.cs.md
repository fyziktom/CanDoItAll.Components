# Economy visual action shape

```csharp
namespace CanDoItAll.Economy.Simulation.Visualization;

public sealed class EconomyVisualAction
{
    public string ActionId { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string SubjectId { get; set; } = string.Empty;
    public EconomyVisualTargetRef Target { get; set; } = new();
    public string PoseKey { get; set; } = string.Empty;
    public string SymbolKey { get; set; } = string.Empty;
    public double StartsAtSeconds { get; set; }
    public double DurationSeconds { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class EconomyVisualTargetRef
{
    public string ActorId { get; set; } = string.Empty;
    public string ObjectId { get; set; } = string.Empty;
    public string AnchorKey { get; set; } = string.Empty;
}
```
