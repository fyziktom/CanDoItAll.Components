# Skeleton: visual action normalizer

```csharp
namespace CanDoItAll.Economy.Simulation.Visualization;

public sealed class EconomyVisualActionNormalizationResult
{
    public List<EconomyVisualAction> Actions { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
    public List<string> Errors { get; set; } = [];
}

public sealed class EconomyVisualActionNormalizer
{
    public EconomyVisualActionNormalizationResult Normalize(IEnumerable<EconomyVisualAction> actions)
    {
        // 1. normalize aliases
        // 2. remove duplicate top-level nested steps unless emitAsStandalone is true
        // 3. sort by step/start/stage/order
        // 4. validate targets and sequence containment
        throw new NotImplementedException();
    }
}
```
