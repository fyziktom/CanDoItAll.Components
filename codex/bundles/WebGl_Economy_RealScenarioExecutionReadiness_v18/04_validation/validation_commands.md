# Validation Commands

## Components

```bash
git branch --show-current
git status --short
dotnet build CanDoItAll.Components.slnx
npm run webgllib:audit-scene-runtime
npm run webgllib:build-assets
npm run webgllib:verify-assets
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj
dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj
```

## Economy

```bash
git branch --show-current
git status --short
dotnet build CanDoItAll.Economy.slnx
pwsh ./scripts/audit-simulation-boundaries.ps1
dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj
```

## Required artifact proof

```bash
# Exact command names may be implemented in this bundle if missing.
dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj --filter "RealProbe|SimulationSandbox|WebGlBridge|Snapshot"
```

Expected generated artifact roots:

```text
artifacts/economy/real-probe/shared-resource/
artifacts/economy/real-probe/finite-resource/
```
