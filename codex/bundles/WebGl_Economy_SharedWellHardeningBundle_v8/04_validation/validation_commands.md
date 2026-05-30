# Validation commands

## Components

```powershell
git branch --show-current
git status --short
npm install
npm run webgllib:build-assets
npm run webgllib:verify-assets
npm run webgllib:audit-scene-runtime
dotnet build CanDoItAll.Components.slnx -p:UseSharedCompilation=false
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj -p:UseSharedCompilation=false
dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj -p:UseSharedCompilation=false
```

Desktop proof only:
- 1440x900 or larger.
- Do not add small/medium/mobile/tablet screenshots.

## Economy

```powershell
git branch --show-current
git status --short
powershell -ExecutionPolicy Bypass -File scripts/audit-simulation-boundaries.ps1
dotnet build CanDoItAll.Economy.slnx -p:UseSharedCompilation=false
dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj -p:UseSharedCompilation=false
```

## Cross-repo scan

- Economy must not reference `CanDoItAll.Components.*`.
- Components must not reference `CanDoItAll.Economy.*`.
- Shared sample names may exist only in docs/tests/sample data.
