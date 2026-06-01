# Validation commands

## Components

```powershell
dotnet build CanDoItAll.Components.slnx
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj
dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj
node tools/webgllib/audit-scene-runtime.cjs
```

## Economy

```powershell
dotnet build CanDoItAll.Economy.slnx
dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj
pwsh ./scripts/audit-simulation-boundaries.ps1
```

## Real headless scenario proof

```powershell
dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj --filter "FullyQualifiedName~EconomyRealProbeArtifactExporterTests"
```

## Future browser smoke proof

Use only desktop/large-screen viewport, for example 1440x900 or 1920x1080.
No small/medium/mobile/tablet proof.
