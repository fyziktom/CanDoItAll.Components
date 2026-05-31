# SimulationRunSnapshot contracts

```csharp
namespace CanDoItAll.Economy.Simulation.Abstractions;

public sealed class SimulationRunSnapshot
{
    public string SnapshotId { get; set; } = string.Empty;
    public string SchemaVersion { get; set; } = "candoitall.economy.simulation-snapshot/v1";
    public string RunId { get; set; } = string.Empty;
    public string ScenarioId { get; set; } = string.Empty;
    public int StepIndex { get; set; }
    public DateTimeOffset SimulatedAtUtc { get; set; }

    public SimulationFrame Frame { get; set; } = new();
    public SimulationFrameDelta? LastDelta { get; set; }

    public List<SimulationEvent> AppliedEvents { get; set; } = [];
    public List<SimulationEvent> PendingEvents { get; set; } = [];

    public List<SimulationMetricValue> Metrics { get; set; } = [];
    public List<SimulationInvariantEvaluationResult> Invariants { get; set; } = [];

    public SimulationSnapshotVisualState? VisualState { get; set; }

    public Dictionary<string, string> Provenance { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];

    public string DeterministicHash { get; set; } = string.Empty;
}

public sealed class SimulationSnapshotVisualState
{
    public string VisualFrameId { get; set; } = string.Empty;
    public long WebGlRunFrameIndex { get; set; }
    public Dictionary<string, string> NodeObjectIds { get; set; } = [];
    public List<string> ActiveStageIds { get; set; } = [];
    public List<string> PendingStageIds { get; set; } = [];
    public Dictionary<string, string> RuntimeDiagnostics { get; set; } = [];
}

public interface ISimulationSnapshotStore
{
    ValueTask SaveAsync(SimulationRunSnapshot snapshot, CancellationToken cancellationToken = default);
    ValueTask<SimulationRunSnapshot?> GetAsync(string snapshotId, CancellationToken cancellationToken = default);
    ValueTask<IReadOnlyList<SimulationRunSnapshot>> ListAsync(string runId, CancellationToken cancellationToken = default);
}
```
