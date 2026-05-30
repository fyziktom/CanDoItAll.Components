# Economy generic behavior expansion skeleton

```csharp
namespace CanDoItAll.Economy.Simulation.Abstractions;

public sealed class SimulationBehaviorExpansionRequest
{
    public SimulationScenarioDefinition Definition { get; set; } = new();
    public SimulationEventStream BaseEvents { get; set; } = new();
    public Dictionary<string, string> Options { get; set; } = [];
}

public sealed class SimulationBehaviorExpansionResult
{
    public SimulationEventStream Events { get; set; } = new();
    public List<SimulationScenarioDefinitionValidationMessage> Diagnostics { get; set; } = [];
}

public interface ISimulationBehaviorExpander
{
    SimulationBehaviorExpansionResult Expand(SimulationBehaviorExpansionRequest request);
}
```

All source code comments must be in English.
