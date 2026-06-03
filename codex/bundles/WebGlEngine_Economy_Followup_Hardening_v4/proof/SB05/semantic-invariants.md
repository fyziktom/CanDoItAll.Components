# SB05 semantic invariants

- The implementation must preserve Components genericity.
- The implementation must not introduce Economy semantics into WebGlLib or WebGlRunLib.
- Runtime proof must demonstrate behavior, not only compilation.
- Required proof artifacts must be non-empty.

## Completion assertions

- The new pathless source contract is confined to `CanDoItAll.Economy.SimulationSandbox`; no SB05 changes were made in generic `CanDoItAll.Components` WebGL packages.
- `EconomySimulationSandboxPage` obtains an `EconomySimulationScenarioSource` from the injected catalog and passes it to `SessionService.TryLoad(source, ...)`.
- `Load(string experimentJsonPath, ...)` remains available as a legacy convenience and now routes through `EconomySimulationScenarioSource.FromPath`.
- Runtime source scans show no scoped fixture-path dependencies in SimulationSandbox, Economy Components, or Economy Node source.
- The pathless source test proves a scenario with empty `ExperimentJsonPath` still loads, preserves scenario identity, and produces a valid WebGL run document.
