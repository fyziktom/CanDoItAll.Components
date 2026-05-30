# Skeleton: Economy experiment input pack contracts

```csharp
namespace CanDoItAll.Economy.Simulation.Abstractions;

public sealed class SimulationExperimentInputPack
{
    public string SchemaVersion { get; set; } = "candoitall.economy.experiment-input/v1";
    public string ExperimentId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public SimulationHypothesisDefinition Hypothesis { get; set; } = new();
    public List<SimulationInputDocumentRef> Inputs { get; set; } = [];
    public SimulationInputHashManifest Hashes { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class SimulationInputDocumentRef
{
    public string Kind { get; set; } = string.Empty; // scenario, placement, parameters, rules, run-plan, visual-mapping, invariants
    public string Path { get; set; } = string.Empty;
    public string ContentHash { get; set; } = string.Empty;
    public string SchemaVersion { get; set; } = string.Empty;
}

public sealed class SimulationHypothesisDefinition
{
    public string HypothesisId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public List<string> MetricIds { get; set; } = [];
}

public sealed class SimulationInputHashManifest
{
    public string PackHash { get; set; } = string.Empty;
    public Dictionary<string, string> InputHashes { get; set; } = [];
}
```
