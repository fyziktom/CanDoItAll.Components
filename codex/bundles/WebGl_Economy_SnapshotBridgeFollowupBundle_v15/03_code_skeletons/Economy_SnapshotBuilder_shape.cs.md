# Code skeleton — Economy snapshot builder

```csharp
namespace CanDoItAll.Economy.Simulation.Abstractions;

public interface ISimulationSnapshotBuilder
{
    SimulationSnapshotBuildResult Build(SimulationSnapshotBuildRequest request);
}

public sealed class SimulationSnapshotBuildRequest
{
    public string SnapshotId { get; set; } = string.Empty;
    public SimulationScenarioDefinition Scenario { get; set; } = new();
    public SimulationFrame Frame { get; set; } = new();
    public SimulationFrameDelta? LastDelta { get; set; }
    public IReadOnlyList<SimulationEvent> AppliedEvents { get; set; } = [];
    public IReadOnlyList<SimulationEvent> PendingEvents { get; set; } = [];
    public IReadOnlyList<SimulationMetricDefinition> Metrics { get; set; } = [];
    public IReadOnlyList<SimulationExpectedInvariantDefinition> Invariants { get; set; } = [];
    public Dictionary<string, string> ProvenanceHashes { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class SimulationSnapshotBuildResult
{
    public SimulationRunSnapshot Snapshot { get; set; } = new();
    public List<SimulationScenarioDefinitionValidationMessage> Messages { get; set; } = [];
    public bool IsValid => Messages.All(message => !string.Equals(message.Severity, "error", StringComparison.OrdinalIgnoreCase));
}
```
