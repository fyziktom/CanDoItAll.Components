# Economy Real Probe Artifact Exporter Shape

```csharp
namespace CanDoItAll.Economy.SimulationSandbox;

public interface IEconomyRealProbeArtifactExporter
{
    EconomyRealProbeArtifactExportResult Export(EconomySimulationSandboxSession session, string outputDirectory);
}

public sealed class EconomyRealProbeArtifactExportResult
{
    public bool Succeeded => Errors.Count == 0;
    public string OutputDirectory { get; set; } = string.Empty;
    public List<string> ArtifactPaths { get; set; } = [];
    public List<string> Errors { get; set; } = [];
    public Dictionary<string, string> Metrics { get; set; } = [];
}
```
