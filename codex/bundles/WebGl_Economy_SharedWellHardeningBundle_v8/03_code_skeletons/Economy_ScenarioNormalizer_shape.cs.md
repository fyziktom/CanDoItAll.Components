# Economy scenario normalizer skeleton

```csharp
namespace CanDoItAll.Economy.Simulation.Abstractions;

public sealed class SimulationScenarioNormalizationResult
{
    public SimulationScenarioDefinition Definition { get; set; } = new();
    public List<SimulationScenarioDefinitionValidationMessage> Warnings { get; set; } = [];
    public List<SimulationScenarioDefinitionValidationMessage> Errors { get; set; } = [];
    public bool IsValid => Errors.Count == 0;
}

public interface ISimulationScenarioDefinitionNormalizer
{
    SimulationScenarioNormalizationResult Normalize(SimulationScenarioDefinition definition);
}
```

All source code comments must be in English.
