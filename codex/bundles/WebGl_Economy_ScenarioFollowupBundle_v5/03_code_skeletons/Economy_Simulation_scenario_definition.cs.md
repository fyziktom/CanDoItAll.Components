# Code skeleton: Economy SimulationScenarioDefinition

```csharp
namespace CanDoItAll.Economy.Simulation.Abstractions;

public sealed class SimulationScenarioDefinition
{
    public string ScenarioId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ScenarioKind { get; set; } = string.Empty;
    public string BaseCurrency { get; set; } = "TOKEN";
    public DateTimeOffset StartsAtUtc { get; set; }
    public int StepCount { get; set; }
    public TimeSpan StepDuration { get; set; } = TimeSpan.FromDays(1);
    public int Seed { get; set; }

    public List<SimulationScenarioActorDefinition> Actors { get; set; } = [];
    public List<SimulationScenarioResourceDefinition> Resources { get; set; } = [];
    public List<SimulationScenarioLocationDefinition> Locations { get; set; } = [];
    public List<SimulationScenarioStoreDefinition> InitialStores { get; set; } = [];
    public List<SimulationScenarioRelationshipDefinition> Relationships { get; set; } = [];
    public List<SimulationScheduledEventDefinition> ScheduledEvents { get; set; } = [];
    public List<SimulationRuleRef> Rules { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class SimulationScenarioLocationDefinition
{
    public string LocationId { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public decimal X { get; set; }
    public decimal Y { get; set; }
    public decimal Z { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = [];
}
```
