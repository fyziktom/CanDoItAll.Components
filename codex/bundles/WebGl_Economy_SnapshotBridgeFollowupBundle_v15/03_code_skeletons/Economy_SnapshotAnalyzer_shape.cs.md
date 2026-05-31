# Code skeleton — Snapshot analyzer registry

```csharp
namespace CanDoItAll.Economy.Simulation.Abstractions;

public interface ISimulationSnapshotAnalyzer
{
    string AnalyzerId { get; }
    IReadOnlyList<SimulationSnapshotFinding> Analyze(SimulationRunSnapshot snapshot);
}

public sealed class SimulationSnapshotFinding
{
    public string FindingId { get; set; } = string.Empty;
    public string Severity { get; set; } = "info";
    public string Category { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string> Evidence { get; set; } = [];
}

public sealed class SimulationSnapshotAnalysisReport
{
    public string SnapshotId { get; set; } = string.Empty;
    public List<SimulationSnapshotFinding> Findings { get; set; } = [];
    public Dictionary<string, string> Summary { get; set; } = [];
}
```
