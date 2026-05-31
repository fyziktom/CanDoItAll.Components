# Economy Experiment Pack Loader Shape

```csharp
public sealed class SimulationExperimentLoadResult
{
    public SimulationExperimentInputPack Pack { get; set; } = new();
    public SimulationScenarioDefinition Scenario { get; set; } = new();
    public SimulationPlacementDefinition Placement { get; set; } = new();
    public SimulationParameterSetDefinition Parameters { get; set; } = new();
    public SimulationRunPlanDefinition RunPlan { get; set; } = new();
    public SimulationEventStream EventStream { get; set; } = new();
    public Dictionary<string, string> Hashes { get; set; } = [];
    public List<SimulationScenarioDefinitionValidationMessage> Diagnostics { get; set; } = [];
}

public interface ISimulationExperimentInputPackLoader
{
    SimulationExperimentLoadResult Load(string experimentJsonPath, SimulationExperimentLoadOptions options);
}
```
