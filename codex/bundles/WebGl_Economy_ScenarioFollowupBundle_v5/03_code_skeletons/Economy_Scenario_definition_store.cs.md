# Code skeleton: Scenario definition store interfaces

```csharp
namespace CanDoItAll.Economy.Simulation.Abstractions;

public interface ISimulationScenarioDefinitionStore
{
    ValueTask<IReadOnlyList<SimulationScenarioDefinition>> ListAsync(
        CancellationToken cancellationToken = default);

    ValueTask<SimulationScenarioDefinition?> GetAsync(
        string scenarioId,
        CancellationToken cancellationToken = default);

    ValueTask SaveAsync(
        SimulationScenarioDefinition definition,
        CancellationToken cancellationToken = default);
}

public interface ISimulationScenarioDefinitionSerializer
{
    string Serialize(SimulationScenarioDefinition definition);
    SimulationScenarioDefinition Deserialize(string json);
}

public interface ISimulationScenarioDefinitionValidator
{
    SimulationScenarioDefinitionValidationResult Validate(SimulationScenarioDefinition definition);
}
```
