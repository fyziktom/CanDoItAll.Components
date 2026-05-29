# Economy skeleton — Simulation.Abstractions

```csharp
namespace CanDoItAll.Economy.Simulation.Abstractions;

public sealed class SimulationScenarioManifest
{
    public string ScenarioId { get; set; } = string.Empty;
    public string ScenarioKind { get; set; } = string.Empty;
    public string BaseCurrency { get; set; } = "USD";
    public int StepCount { get; set; }
    public string DeterministicHash { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class SimulationFrame
{
    public string RunId { get; set; } = string.Empty;
    public int StepIndex { get; set; }
    public DateTimeOffset SimulatedAtUtc { get; set; }
    public List<SimulationActor> Actors { get; set; } = [];
    public List<SimulationResourceStore> ResourceStores { get; set; } = [];
    public List<SimulationResourceFlow> Flows { get; set; } = [];
    public List<SimulationRelationship> Relationships { get; set; } = [];
    public List<SimulationIssue> Issues { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public interface ISimulationBackend
{
    string BackendId { get; }
    SimulationBackendCapabilities Capabilities { get; }
    ValueTask<SimulationFrame> GetFrameAsync(string runId, int stepIndex, CancellationToken cancellationToken = default);
}
```

This project must not reference Ledger, BusinessObjects, SDK, or Components.
```
