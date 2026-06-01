# Economy snapshot analyzer shape

```csharp
public interface ISimulationSnapshotAnalyzer
{
    SimulationSnapshotAnalysisResult Analyze(SimulationRunSnapshot snapshot, SimulationSnapshotAnalysisOptions options);
}

public interface ISimulationSnapshotAnalysisFacet
{
    string FacetId { get; }

    void Analyze(SimulationRunSnapshot snapshot, SimulationSnapshotAnalysisResult result);
}

public sealed class SimulationSnapshotAnalysisResult
{
    public string SnapshotId { get; set; } = string.Empty;
    public List<SimulationSnapshotAnalysisFinding> Findings { get; set; } = [];
    public List<SimulationMetricValue> Metrics { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class SimulationSnapshotAnalysisFinding
{
    public string FindingId { get; set; } = string.Empty;
    public string Severity { get; set; } = "info";
    public string Category { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<string> RelatedActorIds { get; set; } = [];
    public List<string> RelatedResourceIds { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}
```
