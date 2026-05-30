# Skeleton: placement and parameter contracts

```csharp
namespace CanDoItAll.Economy.Simulation.Abstractions;

public sealed class SimulationPlacementDefinition
{
    public string PlacementId { get; set; } = string.Empty;
    public string ScenarioId { get; set; } = string.Empty;
    public string CoordinateSystem { get; set; } = "cartesian-2d";
    public List<SimulationPlacedEntity> Entities { get; set; } = [];
    public List<SimulationTopologyEdge> Topology { get; set; } = [];
    public SimulationGeneratedInputProvenance? GeneratedBy { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class SimulationPlacedEntity
{
    public string RefKind { get; set; } = string.Empty; // actor, location, object, store
    public string RefId { get; set; } = string.Empty;
    public decimal X { get; set; }
    public decimal Y { get; set; }
    public decimal Z { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class SimulationParameterSetDefinition
{
    public string ParameterSetId { get; set; } = string.Empty;
    public string ScenarioId { get; set; } = string.Empty;
    public List<SimulationActorParameter> ActorParameters { get; set; } = [];
    public List<SimulationResourceParameter> ResourceParameters { get; set; } = [];
    public List<SimulationRuleParameter> RuleParameters { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class SimulationGeneratedInputProvenance
{
    public string GeneratorId { get; set; } = string.Empty;
    public string GeneratorVersion { get; set; } = string.Empty;
    public int Seed { get; set; }
    public DateTimeOffset GeneratedAtUtc { get; set; }
    public string RequestHash { get; set; } = string.Empty;
}
```
