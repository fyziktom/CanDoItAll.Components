# Economy backend-neutral sandbox shape

```csharp
namespace CanDoItAll.Economy.SimulationSandbox;

public sealed class EconomySimulationSandboxRunRequest
{
    public string ExperimentJsonPath { get; set; } = string.Empty;
    public string BackendId { get; set; } = string.Empty;
    public bool IncludeWebGlProjection { get; set; } = true;
    public bool IncludeSnapshots { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public interface IEconomySimulationBackendRegistry
{
    ISimulationBackend Resolve(string backendId);
}
```
