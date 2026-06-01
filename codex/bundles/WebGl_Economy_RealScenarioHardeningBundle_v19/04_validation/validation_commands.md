# Validation commands

Run from current branches.

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

## Real scenario runner

The runner should create artifact output under:

```text
artifacts/economy/real-scenario-runs/<scenario-id>/
```

Required scenarios:

```text
shared-well
farmer-land
```

Optional next probe:

```text
small-producer-community
```
