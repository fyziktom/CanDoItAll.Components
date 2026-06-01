# SB14 - Final validation and closure

## Required commands

Components:
```powershell
dotnet build CanDoItAll.Components.slnx
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj
dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj
node tools/webgllib/audit-scene-runtime.cjs
```

Economy:
```powershell
dotnet build CanDoItAll.Economy.slnx
dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj
pwsh ./scripts/audit-simulation-boundaries.ps1
```

## Required proof

- All transcripts must be non-empty.
- Summaries must include test counts.
- Warning budget must be updated.
- Readiness report must say whether the next step is:
  - headless test,
  - browser smoke,
  - or full UI demo.
