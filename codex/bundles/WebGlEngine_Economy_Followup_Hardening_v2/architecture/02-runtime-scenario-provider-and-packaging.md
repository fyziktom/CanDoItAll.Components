# Runtime scenario provider and packaging architecture

## Problem

The browser/Node sandbox currently resolves fixtures from the test project directory. This is acceptable for tests but not for product runtime, Docker, package consumers, or enterprise demos.

## Target

Introduce an explicit scenario catalog boundary:

```csharp
public interface IEconomySimulationScenarioCatalog
{
    IReadOnlyList<EconomySimulationScenarioDescriptor> ListScenarios();
    Stream OpenExperiment(string scenarioId);
    Stream? OpenCompanionFile(string scenarioId, string relativePath);
}
```

Acceptable implementations:

- `FileSystemEconomySimulationScenarioCatalog` for local/dev/demo directories.
- `EmbeddedEconomySimulationScenarioCatalog` or content-root catalog for package/demo samples.
- `TestFixtureEconomySimulationScenarioCatalog` in test projects only.

## Component behavior

`EconomySimulationSandboxPage` must receive its session service and scenario catalog through DI or explicit parameters. It must not call `new EconomySimulationSandboxSessionService()` or search for test paths internally unless it is in a test-only wrapper.

## Proof

- Node route `/economy/simulation-sandbox` loads a scenario without test fixture paths.
- Package-mode build and browser proof use a content-root or embedded sample provider.
- Tests can still use test fixtures, but test paths must be isolated to test code.
