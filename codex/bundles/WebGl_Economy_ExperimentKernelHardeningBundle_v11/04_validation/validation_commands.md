# Validation commands

## Components

```powershell
npm run webgllib:audit-scene-runtime
npm run webgllib:audit-command-batch-parity
npm run webgllib:audit-motion-queue
npm run webgllib:verify-assets
dotnet test tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj --no-restore -p:UseSharedCompilation=false
dotnet test tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj --no-restore -p:UseSharedCompilation=false
dotnet build CanDoItAll.Components.slnx --no-restore -p:UseSharedCompilation=false
```

## Economy

```powershell
.\scripts\audit-simulation-boundaries.ps1
dotnet test tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --no-restore -p:UseSharedCompilation=false
dotnet build CanDoItAll.Economy.slnx --no-restore -p:UseSharedCompilation=false
```

## Required new proof

- C#/JS batch normalizer parity fixture.
- Ordered action queue test: same object moves to target, admin stage happens, then returns home.
- Experiment input pack validator rejects missing/duplicate/hash-invalid input refs.
- No-runtime-randomness audit: simulation transition code must not call `Random`, `Guid.NewGuid`, `DateTime.Now`, `DateTimeOffset.Now`, or non-input clock values.
- Shared-well readiness proof based only on JSON input pack.
- Farmer-land probe proof based only on JSON input pack.
- Performance proof with event counts and elapsed times.
```
