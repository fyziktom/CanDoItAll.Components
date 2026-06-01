# Test and proof results observed

## Components

Observed transcript results from the pushed proof bundle:

- `dotnet build CanDoItAll.Components.slnx`
  - Result: passed
  - Warnings: 0
  - Errors: 0
- `dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj`
  - Result: passed
  - Passed: 35
  - Failed: 0
- `dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj`
  - Result: passed
  - Passed: 21
  - Failed: 0

## Economy

Observed transcript results from the pushed proof bundle:

- `dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj`
  - Result: passed
  - Passed: 519
  - Failed: 0
  - Duration: about 4m 50s
- Warning noise remains:
  - `NU1701` for `ncalc 1.3.8` restored against .NET Framework assets.
  - `NU1510` for `Microsoft.Extensions.DependencyInjection.Abstractions` package pruning warning.
  - `NU1902` OpenTelemetry vulnerability warnings from linked IPFS projects during full solution build.

## Required next hardening

Do not ignore warning noise forever. This bundle does not require fixing all historic project warnings, but it requires:
- separating simulation-sandbox proof from unrelated legacy warning noise where possible,
- adding a warning budget file,
- making new simulation/bridge warnings fail the proof gate,
- keeping known legacy warnings explicit and bounded.
