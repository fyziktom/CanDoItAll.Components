# SB07 Proof - Economy visual mapping contracts and loader

## Scope

Expanded visual mapping contracts to carry category, anchor, action, pose, symbol, and no-op fallback information, and routed experiment pack loading through the typed visual mapping loader.

## Changed-file hashes

- `2fb3f418422f171e4d433856c84ec6101ab63bc1a4ba77d89ad48e1886e59a3c  C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.Abstractions\Experiment\EconomyVisualMappingDefinition.cs`
- `60f772ff19697314903a2d1927a4629400eef953a5eb43c5356386b7e0cdf12b  C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.Abstractions\Experiment\SimulationExperimentInputPackLoader.cs`
- `6d19af1963a624accebd0f83fbbf176b365259b808fe6c7263686c1b8e22546a  C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\Fixtures\ExperimentInputs\shared-well\visual.mapping.json`
- `36bb96ffaa2272efde8a2820670a99c9bfd83d73c0f5c037904b20b3e2e51782  C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\Fixtures\ExperimentInputs\farmer-land\visual.mapping.json`

## Validation transcript

- `dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter SimulationExperimentInputPackStrictMode --no-restore`: pass, 10 tests.
- Targeted readiness tests for input, loader, transition, metrics, readiness, and performance probes: pass, 24 tests.
- Full Economy tests: pass, 483 tests.

## Semantic invariants

- Mapping decisions come from schema data rather than hardcoded example logic.
- No-op fallbacks are explicit and validated.
- Visual mapping keys remain WebGL-neutral and reusable across probes.
