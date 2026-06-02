# Scenario Pack And Session Architecture

## Current issue

The current catalog is safer than test fixture lookup, but it still exposes absolute `ExperimentJsonPath` values and the session service still loads by path.

## Target

Introduce a portable runtime scenario pack concept:

```csharp
public sealed class EconomySimulationScenarioManifest
{
    public string ScenarioId { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ExperimentFileName { get; set; } = "experiment.json";
    public string ContentHash { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = [];
}
```

Expected flow:

```text
IEconomySimulationScenarioCatalog
  -> ListScenarios()
  -> OpenScenarioPack(scenarioId)
  -> LoadSessionByScenarioId(scenarioId)
  -> ExportSession with scenario id + pack hash, not only absolute path
```

## Requirements

- Keep file-system catalog as one implementation.
- Add an in-memory catalog for tests and package consumers.
- Add service registration extension(s) that register:
  - session service,
  - workflow,
  - backends,
  - catalog,
  - optional persistence settings.
- Keep Node-specific catalog root in Node, but do not force all consumers to copy Node's registration code.
