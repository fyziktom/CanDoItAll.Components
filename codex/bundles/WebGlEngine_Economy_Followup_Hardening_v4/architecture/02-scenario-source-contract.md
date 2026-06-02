# Target scenario source contract

## Current problem

`IEconomySimulationScenarioCatalog` exposes stream methods, but the descriptor and session service still center on `ExperimentJsonPath`. This blocks embedded, IPFS, database, cloud, tenant, and package-contained scenario sources.

## Target shape

Add a source contract that can represent file-system and non-file-system scenarios:

```csharp
public sealed class EconomySimulationScenarioSource
{
    public string ScenarioId { get; set; } = string.Empty;
    public string SourceKind { get; set; } = "filesystem"; // filesystem, embedded, ipfs, database, package
    public string DisplayName { get; set; } = string.Empty;
    public string SourceUri { get; set; } = string.Empty;  // opaque, not necessarily a file path
    public string PackHash { get; set; } = string.Empty;
    public Func<Stream> OpenExperiment { get; set; } = default!;
    public Func<string, Stream?> OpenCompanionFile { get; set; } = default!;
    public Dictionary<string, string> Metadata { get; set; } = [];
}
```

The exact implementation may differ, but the key requirement is pathless loading from a stream/source object.

## Compatibility

- Keep `Load(string experimentJsonPath, ...)` as legacy convenience.
- Add `Load(EconomySimulationScenarioSource source, ...)` and `TryLoad(...)`.
- Runtime UI must call the source-based API.
- Export/import must use scenario id/source/hash rather than absolute local paths.
