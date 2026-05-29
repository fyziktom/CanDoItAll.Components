# Code skeleton: Economy visual actions without WebGL dependency

```csharp
namespace CanDoItAll.Economy.Simulation.Visualization;

public sealed class EconomyVisualAction
{
    public string ActionId { get; set; } = string.Empty;
    public string ActionKind { get; set; } = string.Empty;
    public string SubjectNodeId { get; set; } = string.Empty;
    public string TargetNodeId { get; set; } = string.Empty;
    public double StartsAtSeconds { get; set; }
    public double DurationSeconds { get; set; }
    public string SymbolCategory { get; set; } = string.Empty;
    public string PoseKey { get; set; } = string.Empty;
    public decimal Intensity { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public static class EconomyVisualActionKinds
{
    public const string MoveToTarget = "move-to-target";
    public const string ReturnToHome = "return-to-home";
    public const string ChangePose = "change-pose";
    public const string ShowStatusSymbol = "show-status-symbol";
    public const string HideStatusSymbol = "hide-status-symbol";
    public const string ShowResourceFlow = "show-resource-flow";
    public const string PulseRelationship = "pulse-relationship";
    public const string Wait = "wait";
}
```
