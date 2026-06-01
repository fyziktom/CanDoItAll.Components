# Economy Sandbox Session Result Shape

```csharp
namespace CanDoItAll.Economy.SimulationSandbox;

public sealed class EconomySandboxOperationResult<T>
{
    public bool Succeeded => Errors.Count == 0;
    public T? Value { get; set; }
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
}

public sealed class EconomySimulationSandboxSessionStatus
{
    public string SessionId { get; set; } = string.Empty;
    public int CurrentStepIndex { get; set; }
    public bool CanStepForward { get; set; }
    public bool CanStepBackward { get; set; }
    public bool IsPaused { get; set; }
    public bool IsProjected { get; set; }
    public bool IsValid { get; set; }
    public IReadOnlyList<int> AvailableSteps { get; set; } = [];
    public Dictionary<string, string> Diagnostics { get; set; } = [];
}
```
