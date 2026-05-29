# Economy skeleton — Visualization contracts without WebGL dependency

```csharp
namespace CanDoItAll.Economy.Simulation.Visualization;

public sealed class EconomyVisualFrame
{
    public string FrameId { get; set; } = string.Empty;
    public string RunId { get; set; } = string.Empty;
    public int StepIndex { get; set; }
    public List<EconomyVisualNode> Nodes { get; set; } = [];
    public List<EconomyVisualLink> Links { get; set; } = [];
    public List<EconomyVisualLayer> Layers { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class EconomyVisualNode
{
    public string Id { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public EconomyVisualLayoutHint Layout { get; set; } = new();
    public List<EconomyVisualSymbol> Symbols { get; set; } = [];
    public Dictionary<string, string> Metrics { get; set; } = [];
}

public interface IEconomyVisualFrameMapper
{
    EconomyVisualFrame Map(SimulationFrame frame);
}
```

No WebGL types in this project.
```
