# Validation commands

Run in `CanDoItAll.Components`:

```powershell
git status --short
dotnet build .\CanDoItAll.Components.slnx
dotnet test .\tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj
dotnet test .\tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj
npm run webgllib:audit-scene-runtime
```

Run in `CanDoItAll.Economy`:

```powershell
git status --short
dotnet build .\CanDoItAll.Economy.slnx
dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj
powershell -ExecutionPolicy Bypass -File .\scripts\audit-simulation-boundaries.ps1
```

Bridge-specific proof:

```powershell
dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter EconomyWebGlBridge
dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter SimulationExperimentInputPackStrictMode
```
