# Economy skeleton: visual action contracts without WebGL dependency

```csharp
namespace CanDoItAll.Economy.Simulation.Visualization;

public sealed class EconomyVisualAction
{
    public string ActionId { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string ActorNodeId { get; set; } = string.Empty;
    public EconomyVisualActionTarget Target { get; set; } = new();
    public string PoseHint { get; set; } = string.Empty;
    public string SymbolHint { get; set; } = string.Empty;
    public double DurationSeconds { get; set; }
    public List<EconomyVisualAction> Steps { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class EconomyVisualActionTarget
{
    public string NodeId { get; set; } = string.Empty;
    public string PlaceId { get; set; } = string.Empty;
    public string AnchorHint { get; set; } = "center";
}

public interface IEconomyVisualActionMapper
{
    IReadOnlyList<EconomyVisualAction> MapActions(SimulationFrame frame, IReadOnlyList<SimulationEvent> events);
}
```
