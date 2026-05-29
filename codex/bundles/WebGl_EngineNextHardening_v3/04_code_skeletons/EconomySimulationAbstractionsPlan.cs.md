# Skeleton — Future Economy Simulation Abstractions

This belongs in the Economy repo later, not in Components.

```csharp
namespace CanDoItAll.Economy.Simulation.Abstractions;

public readonly record struct EconomicSimulationRunId(string Value);
public readonly record struct EconomicScenarioId(string Value);
public readonly record struct EconomicActorId(string Value);
public readonly record struct EconomicResourceId(string Value);
public readonly record struct EconomicFlowId(string Value);

public sealed class EconomicSimulationStep
{
    public int Index { get; set; }
    public DateOnly Date { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class EconomicActorSnapshot
{
    public EconomicActorId ActorId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public Dictionary<string, decimal> Measures { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class EconomicResourceSnapshot
{
    public EconomicResourceId ResourceId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public sealed class EconomicFlowSnapshot
{
    public EconomicFlowId FlowId { get; set; }
    public EconomicActorId SourceActorId { get; set; }
    public EconomicActorId TargetActorId { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public sealed class EconomicRunFrame
{
    public EconomicSimulationRunId RunId { get; set; }
    public EconomicSimulationStep Step { get; set; } = new();
    public List<EconomicActorSnapshot> Actors { get; set; } = [];
    public List<EconomicResourceSnapshot> Resources { get; set; } = [];
    public List<EconomicFlowSnapshot> Flows { get; set; } = [];
}

public interface IEconomicSimulationBackend
{
    string BackendKind { get; }
    ValueTask<EconomicRunFrame> StepAsync(EconomicRunFrame current, CancellationToken cancellationToken = default);
}
```

Dependency rule:
- This project must not reference Ledger or Accounts.
- Ledger and SimpleAccounts adapters reference this project, not the reverse.
