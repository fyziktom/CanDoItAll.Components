```csharp
namespace CanDoItAll.Economy.Simulation.SimpleAccounts;

public interface ISimpleSimulationStateTransitionEngine
{
    SimpleSimulationState Initialize(SimulationScenarioDefinition definition);
    SimpleSimulationStepResult Apply(SimpleSimulationState state, IReadOnlyList<SimulationEvent> events);
}

public sealed class SimpleSimulationState
{
    public int StepIndex { get; set; }
    public Dictionary<string, SimulationResourceStore> StoresById { get; set; } = [];
    public Dictionary<string, SimulationActor> ActorsById { get; set; } = [];
    public List<SimulationIssue> Issues { get; set; } = [];
}

public sealed class SimpleSimulationStepResult
{
    public SimulationFrame Frame { get; set; } = new();
    public SimulationFrameDelta Delta { get; set; } = new();
    public List<SimulationScenarioDefinitionValidationMessage> Messages { get; set; } = [];
}
```
