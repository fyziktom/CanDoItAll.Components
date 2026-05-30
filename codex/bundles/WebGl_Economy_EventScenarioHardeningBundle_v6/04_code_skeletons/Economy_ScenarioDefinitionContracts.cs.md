# Economy skeleton: scenario definition contracts

```csharp
namespace CanDoItAll.Economy.Simulation.Abstractions;

public sealed class SimulationScenarioDefinition
{
    public string ScenarioId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ScenarioKind { get; set; } = string.Empty;
    public DateTimeOffset StartsAtUtc { get; set; }
    public TimeSpan StepDuration { get; set; } = TimeSpan.FromDays(1);
    public int StepCount { get; set; }
    public List<SimulationScenarioEntityDefinition> Entities { get; set; } = [];
    public List<SimulationScenarioPlaceDefinition> Places { get; set; } = [];
    public List<SimulationScenarioResourceDefinition> Resources { get; set; } = [];
    public List<SimulationScenarioStoreDefinition> Stores { get; set; } = [];
    public List<SimulationScenarioEventTemplate> EventTemplates { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class SimulationScenarioEntityDefinition
{
    public string EntityId { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string HomePlaceId { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class SimulationScenarioPlaceDefinition
{
    public string PlaceId { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public decimal X { get; set; }
    public decimal Y { get; set; }
    public decimal Z { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = [];
}
```
