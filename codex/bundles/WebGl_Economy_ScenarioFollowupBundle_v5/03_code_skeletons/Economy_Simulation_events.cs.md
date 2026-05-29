# Code skeleton: SimulationEvent contracts

```csharp
namespace CanDoItAll.Economy.Simulation.Abstractions;

public sealed class SimulationEvent
{
    public string EventId { get; set; } = string.Empty;
    public string EventKind { get; set; } = string.Empty;
    public List<string> ActorIds { get; set; } = [];
    public List<string> ResourceIds { get; set; } = [];
    public string SourceId { get; set; } = string.Empty;
    public string TargetId { get; set; } = string.Empty;
    public DateTimeOffset OccurredAtUtc { get; set; }
    public TimeSpan Duration { get; set; }
    public decimal Magnitude { get; set; }
    public List<SimulationEventEffect> Effects { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class SimulationEventEffect
{
    public string EffectKind { get; set; } = string.Empty;
    public string TargetId { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public static class SimulationEventKinds
{
    public const string ResourceUse = "resource-use";
    public const string ResourceTransfer = "resource-transfer";
    public const string Travel = "travel";
    public const string ReturnHome = "return-home";
    public const string Administration = "administration";
    public const string RuleViolation = "rule-violation";
    public const string RuleEnforcement = "rule-enforcement";
    public const string Trade = "trade";
    public const string Loan = "loan";
    public const string Repayment = "repayment";
    public const string Maintenance = "maintenance";
}
```
