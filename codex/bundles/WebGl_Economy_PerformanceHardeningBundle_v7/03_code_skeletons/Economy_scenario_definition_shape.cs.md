# Economy scenario definition shape

```csharp
namespace CanDoItAll.Economy.Simulation.Abstractions;

public sealed class SimulationScenarioDefinition
{
    public string ScenarioId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTimeOffset StartsAtUtc { get; set; }
    public TimeSpan StepDuration { get; set; } = TimeSpan.FromDays(1);
    public int StepCount { get; set; }
    public List<SimulationScenarioActorDefinition> Actors { get; set; } = [];
    public List<SimulationScenarioLocationDefinition> Locations { get; set; } = [];
    public List<SimulationScenarioResourceDefinition> Resources { get; set; } = [];
    public List<SimulationScenarioObjectDefinition> Objects { get; set; } = [];
    public List<SimulationScenarioEventDefinition> Events { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class SimulationScenarioEventDefinition
{
    public string EventId { get; set; } = string.Empty;
    public string EventKind { get; set; } = string.Empty;
    public int StepIndex { get; set; }
    public string SubjectActorId { get; set; } = string.Empty;
    public string TargetActorId { get; set; } = string.Empty;
    public string TargetObjectId { get; set; } = string.Empty;
    public string ResourceId { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = [];
}
```
