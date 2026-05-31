# Economy snapshot analyzer shape

```csharp
namespace CanDoItAll.Economy.Simulation.Abstractions;

public sealed class SimulationSnapshotAnalysisRequest
{
    public SimulationRunSnapshot Snapshot { get; set; } = new();
    public List<string> RequestedAnalyzers { get; set; } = [];
    public Dictionary<string, string> Parameters { get; set; } = [];
}

public sealed class SimulationSnapshotAnalysisReport
{
    public string SnapshotId { get; set; } = string.Empty;
    public List<SimulationSnapshotAnalysisFinding> Findings { get; set; } = [];
    public Dictionary<string, string> Summary { get; set; } = [];
}

public sealed class SimulationSnapshotAnalysisFinding
{
    public string AnalyzerId { get; set; } = string.Empty;
    public string Severity { get; set; } = "info";
    public string Category { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string> Evidence { get; set; } = [];
}

public interface ISimulationSnapshotAnalyzer
{
    SimulationSnapshotAnalysisReport Analyze(SimulationSnapshotAnalysisRequest request);
}
```
