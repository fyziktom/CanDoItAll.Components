# Skeleton: generic simple transition engine

```csharp
namespace CanDoItAll.Economy.Simulation.SimpleAccounts;

public interface ISimpleSimulationTransitionEngine
{
    SimpleSimulationRunResult Run(SimpleSimulationRunRequest request);
}

public sealed class SimpleSimulationRunRequest
{
    public SimulationExperimentInputPack InputPack { get; set; } = new();
    public SimulationScenarioDefinition Scenario { get; set; } = new();
    public SimulationPlacementDefinition Placement { get; set; } = new();
    public SimulationParameterSetDefinition Parameters { get; set; } = new();
    public SimulationEventStream Events { get; set; } = new();
}

public sealed class SimpleSimulationRunResult
{
    public SimulationRunIdentity Run { get; set; } = new();
    public List<SimulationFrame> Frames { get; set; } = [];
    public List<SimulationFrameDelta> Deltas { get; set; } = [];
    public Dictionary<string, string> OutputHashes { get; set; } = [];
}

public interface ISimpleSimulationEventHandler
{
    bool CanHandle(SimulationEvent simulationEvent);
    void Apply(SimpleSimulationMutableState state, SimulationEvent simulationEvent);
}
```
