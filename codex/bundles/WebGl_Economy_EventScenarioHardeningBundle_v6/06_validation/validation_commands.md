# Validation commands

## Components

```powershell
git status --short
git branch --show-current
npm install
npm run webgllib:build-assets
npm run webgllib:verify-assets
npm run webgllib:audit-scene-runtime
dotnet build CanDoItAll.Components.slnx -p:UseSharedCompilation=false
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj -p:UseSharedCompilation=false
```

## Economy

```powershell
git status --short
git branch --show-current
powershell -ExecutionPolicy Bypass -File scripts/audit-simulation-boundaries.ps1
dotnet build CanDoItAll.Economy.slnx -p:UseSharedCompilation=false
dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj -p:UseSharedCompilation=false
```

## Required scans

- no `CanDoItAll.Economy` references in Components;
- no `CanDoItAll.Components` or `WebGl` references in Economy `Simulation.*` projects;
- no `Ledger`, `BusinessObjects`, or `Sdk` references from `Simulation.SimpleAccounts`;
- no `Simulation.SimpleAccounts` reference from `Simulation.Ledger`;
- JS runtime line-count thresholds are respected or explicitly justified.
