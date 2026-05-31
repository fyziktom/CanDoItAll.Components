# Final validation summary

## Components

| Command | Result |
|---|---|
| `dotnet test .\tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj --no-restore` | Pass, 19 tests |
| `dotnet test .\tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj --no-restore` | Pass, 35 tests |
| `npm run webgllib:audit-motion-queue` | Pass |
| `npm run webgllib:audit-stage-runner` | Pass |
| `npm run webgllib:audit-command-batch-parity` | Pass, 5 fixtures |
| `npm run webgllib:audit-scene-runtime` | Pass, 9 existing file-size warnings |
| `dotnet build .\CanDoItAll.Components.slnx` | Pass, 0 warnings, 0 errors |

## Economy

| Command | Result |
|---|---|
| `dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter EconomyWebGlBridge --no-restore` | Pass, 6 tests |
| `dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter SimulationExperimentInputPackStrictMode --no-restore` | Pass, 10 tests |
| `powershell -ExecutionPolicy Bypass -File .\scripts\audit-simulation-boundaries.ps1` | Pass |
| `dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter "SimulationExperimentInputTests\|SimulationExperimentLoaderTests\|SimulationTransitionAndMetricHardeningTests\|EconomyReadinessProbeTests\|EconomyPerformanceProbeTests" --no-restore` | Pass, 24 tests |
| `dotnet build .\CanDoItAll.Economy.slnx` | Pass, 44 warnings, 0 errors |
| `dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --no-restore` | Pass, 483 tests, 0 skipped, duration 3 m 18 s |

## Warning notes

- Economy warnings are existing package/build warnings observed during validation, including `ncalc` `NU1701`, `Microsoft.Extensions.DependencyInjection.Abstractions` `NU1510`, and existing OpenTelemetry advisories from sibling IPFS NodeControl during solution build.
- Components solution build completed with 0 warnings.
