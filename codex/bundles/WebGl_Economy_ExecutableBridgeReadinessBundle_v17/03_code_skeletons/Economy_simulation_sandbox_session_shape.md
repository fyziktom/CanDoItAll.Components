# Economy SimulationSandbox session shape

```csharp
public interface IEconomySimulationSandboxSessionService
{
    EconomySimulationSandboxSession Load(string experimentJsonPath, EconomySimulationSandboxSessionOptions options);
    EconomySimulationSandboxSession Seek(EconomySimulationSandboxSession session, int stepIndex);
    EconomySimulationSandboxSession Pause(EconomySimulationSandboxSession session);
    EconomySimulationSandboxSession Resume(EconomySimulationSandboxSession session);
    SimulationRunSnapshot Snapshot(EconomySimulationSandboxSession session, string reason);
}

public sealed class EconomySimulationSandboxSession
{
    public string SessionId { get; set; } = string.Empty;
    public SimulationExperimentLoadResult Input { get; set; } = new();
    public EconomySimulationBackendResult Backend { get; set; } = new();
    public IReadOnlyList<EconomyVisualFrame> VisualFrames { get; set; } = [];
    public WebGlRunDocument RunDocument { get; set; } = new();
    public int CurrentStepIndex { get; set; }
    public bool IsPaused { get; set; }
    public List<SimulationRunSnapshot> Snapshots { get; set; } = [];
    public SimulationScenarioDefinitionValidationResult Diagnostics { get; set; } = new();
}
```
