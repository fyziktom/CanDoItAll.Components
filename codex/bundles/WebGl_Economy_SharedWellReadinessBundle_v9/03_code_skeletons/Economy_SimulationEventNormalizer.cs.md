```csharp
namespace CanDoItAll.Economy.Simulation.Abstractions;

public sealed class SimulationEventNormalizationResult
{
    public SimulationEvent Event { get; set; } = new();
    public List<SimulationScenarioDefinitionValidationMessage> Messages { get; set; } = [];
    public bool IsValid => Messages.All(message => !string.Equals(message.Severity, "error", StringComparison.OrdinalIgnoreCase));
}

public interface ISimulationEventNormalizer
{
    SimulationEventNormalizationResult Normalize(SimulationEvent simulationEvent);
}

public sealed class SimulationEventNormalizer : ISimulationEventNormalizer
{
    public SimulationEventNormalizationResult Normalize(SimulationEvent simulationEvent)
    {
        ArgumentNullException.ThrowIfNull(simulationEvent);
        // Normalize aliases into canonical fields.
        // Do not mutate the source event.
        throw new NotImplementedException();
    }
}
```
