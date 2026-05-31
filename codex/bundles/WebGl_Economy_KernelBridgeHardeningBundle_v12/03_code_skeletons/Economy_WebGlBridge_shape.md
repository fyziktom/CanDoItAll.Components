# Economy WebGL Bridge Shape

```csharp
public sealed class EconomyWebGlRunInput
{
    public string ExperimentId { get; set; } = string.Empty;
    public string ExperimentHash { get; set; } = string.Empty;
    public IReadOnlyList<EconomyVisualFrame> Frames { get; set; } = [];
    public EconomyVisualMappingDefinition VisualMapping { get; set; } = new();
}

public interface IEconomyWebGlRunProjector
{
    WebGlRunDocument Project(EconomyWebGlRunInput input, EconomyWebGlProjectionOptions options);
}

public sealed class EconomyWebGlProjectionOptions
{
    public bool IncludeDiagnostics { get; set; } = true;
    public string DefaultAssetProfile { get; set; } = "primitive";
}
```
