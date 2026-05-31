# Economy snapshot builder shape

```csharp
namespace CanDoItAll.Economy.Simulation.Abstractions;

public sealed class SimulationRunSnapshotBuildRequest
{
    public string SnapshotId { get; set; } = string.Empty;
    public SimulationRunIdentity Run { get; set; } = new();
    public SimulationScenarioDefinition Definition { get; set; } = new();
    public SimulationFrame Frame { get; set; } = new();
    public SimulationFrameDelta? LastDelta { get; set; }
    public IReadOnlyList<SimulationEvent> AppliedEvents { get; set; } = [];
    public IReadOnlyList<SimulationEvent> PendingEvents { get; set; } = [];
    public IReadOnlyList<SimulationMetricDefinition> Metrics { get; set; } = [];
    public IReadOnlyList<SimulationExpectedInvariantDefinition> Invariants { get; set; } = [];
    public SimulationSnapshotVisualState? VisualState { get; set; }
    public Dictionary<string, string> ProvenanceHashes { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public interface ISimulationRunSnapshotBuilder
{
    SimulationRunSnapshot Build(SimulationRunSnapshotBuildRequest request);
}
```
