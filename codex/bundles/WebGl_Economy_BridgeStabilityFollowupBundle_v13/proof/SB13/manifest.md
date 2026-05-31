# SB13 Proof - Economy simulation sandbox skeleton

## Scope

Added an Economy-owned simulation sandbox project for future joined simulation/WebGL workflows. It references Economy simulation and bridge projects, not Components directly.

## Changed-file hashes

- `c74dc12faf77e56e841a1545dfe99d245a2d023ccad0afede26ac5526a1959d0  C:\repositories\CanDoItAll.Economy\CanDoItAll.Economy.slnx`
- `5fefccc807139dba2da87ee8ff1e89ae9d026ed2d40ba620485988eba4fd9ae9  C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.SimulationSandbox\CanDoItAll.Economy.SimulationSandbox.csproj`
- `20db438aef873dde8a250f287465ade41938d9478b3453fd5f73665042fcc27c  C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.SimulationSandbox\EconomySimulationSandboxWorkflow.cs`
- `eda68e7874fd8439a2f5d6cb954a8f6eeb819e8fa42895f5d7909a32e51407c7  C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj`

## Validation transcript

- Economy WebGL bridge tests include sandbox compile/ownership check: pass.
- `dotnet build .\CanDoItAll.Economy.slnx`: pass, 44 warnings, 0 errors.
- Full Economy tests: pass, 483 tests.

## Semantic invariants

- Joined simulation/WebGL orchestration remains Economy-owned.
- The sandbox does not introduce direct Components references beyond the bridge dependency.
