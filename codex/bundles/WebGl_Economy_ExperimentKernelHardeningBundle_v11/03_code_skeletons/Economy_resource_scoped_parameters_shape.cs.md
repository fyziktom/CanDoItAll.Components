# Economy resource-scoped parameter shape

```csharp
public sealed class SimulationActorResourceRequirement
{
    public string ActorId { get; set; } = string.Empty;
    public string ResourceId { get; set; } = string.Empty;
    public decimal QuantityPerStep { get; set; }
    public string Unit { get; set; } = string.Empty;
    public int Priority { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class SimulationActorCapacity
{
    public string ActorId { get; set; } = string.Empty;
    public string ResourceId { get; set; } = string.Empty;
    public decimal CarryCapacity { get; set; }
    public decimal StorageCapacity { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class SimulationRuleLimit
{
    public string RuleId { get; set; } = string.Empty;
    public string ResourceId { get; set; } = string.Empty;
    public string LimitKind { get; set; } = string.Empty; // max-draw, max-land-share, max-debt, etc.
    public decimal Value { get; set; }
    public string Unit { get; set; } = string.Empty;
}
```

These replace water-specific fields in generic abstractions.
```
