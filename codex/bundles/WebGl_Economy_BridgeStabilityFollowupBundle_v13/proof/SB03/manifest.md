# SB03 Proof - Components action plan to stage batch converter

## Scope

Added a generic `WebGlRunActionPlanBatchCompiler` wrapper over the existing batch builder so Economy can consume a stable compiler-shaped API without introducing domain coupling.

## Changed-file hashes

- `dbaafa4df548711353766baedf45a90b43ac0eb4680cab6df7d84b9cc2e4515d  C:\repositories\CanDoItAll.Components\src\CanDoItAll.Components.WebGlRunLib\Planning\WebGlRunActionPlanBatchCompiler.cs`
- `4ad420c7479e2c348318aaa2bf1abd4bfe36c712356507040825030a3080b583  C:\repositories\CanDoItAll.Components\tests\CanDoItAll.Components.WebGlRunLib.Tests\WebGlRunActionCompilerTests.cs`

## Validation transcript

- `dotnet test .\tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj --no-restore`: pass, 19 tests.
- `npm run webgllib:audit-command-batch-parity`: pass for 5 fixtures.
- `dotnet build .\CanDoItAll.Components.slnx`: pass, 0 warnings, 0 errors.

## Semantic invariants

- Action plans compile to ordered command batches through generic WebGL run contracts.
- The compiler API adds no Economy references, no fixture names, and no simulation-domain terms.
