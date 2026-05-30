# Economy skeleton: simulation events

```csharp
namespace CanDoItAll.Economy.Simulation.Abstractions;

public sealed class SimulationEvent
{
    public string EventId { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public int StepIndex { get; set; }
    public DateTimeOffset OccurredAtUtc { get; set; }
    public string ActorId { get; set; } = string.Empty;
    public string TargetActorId { get; set; } = string.Empty;
    public string PlaceId { get; set; } = string.Empty;
    public string TargetPlaceId { get; set; } = string.Empty;
    public string ResourceId { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public double DurationSeconds { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public static class SimulationEventKinds
{
    public const string ResourceUse = "resource-use";
    public const string ResourceTransfer = "resource-transfer";
    public const string ActorMoveIntention = "actor-move-intention";
    public const string ActorWork = "actor-work";
    public const string ActorAdmin = "actor-admin";
    public const string StoreChanged = "store-changed";
    public const string RelationshipChanged = "relationship-changed";
    public const string IssueRaised = "issue-raised";
    public const string IssueResolved = "issue-resolved";
    public const string RuleApplied = "rule-applied";
}
```
