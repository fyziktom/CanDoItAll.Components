# Snapshot Architecture

## Snapshot is simulation-first

The canonical snapshot belongs in Economy abstractions and should be independent of WebGL.

Suggested shape:

```csharp
public sealed class SimulationRunSnapshot
{
    public string SnapshotId { get; set; } = string.Empty;
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
    public string DeterministicHash { get; set; } = string.Empty;
}
```

## Optional visual state

```csharp
public sealed class SimulationSnapshotVisualState
{
    public string VisualFrameId { get; set; } = string.Empty;
    public string WebGlRunFrameId { get; set; } = string.Empty;
    public Dictionary<string, string> NodeObjectIds { get; set; } = [];
    public List<string> ActiveStageIds { get; set; } = [];
    public List<string> PendingStageIds { get; set; } = [];
    public Dictionary<string, string> RuntimeDiagnostics { get; set; } = [];
}
```

## Snapshot use cases

- pause and inspect exact state,
- export JSON for analysis,
- compare two snapshots,
- replay from snapshot if supported,
- attach human interpretation notes,
- feed snapshot to LLM analysis later without losing provenance.
