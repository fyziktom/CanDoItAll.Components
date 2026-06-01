# Real scenario runner options shape

```csharp
public sealed class EconomyRealScenarioRunnerOptions
{
    public bool StrictInputPackValidation { get; set; } = true;
    public bool CleanOutputDirectory { get; set; } = true;
    public bool IncludeVolatileTimestampedReports { get; set; } = true;
    public bool FailOnFallbackObject { get; set; } = true;
    public bool FailOnNoOpFallback { get; set; }
    public string ReadinessMode { get; set; } = "headless"; // headless|browser-smoke|ui-demo
}
```
