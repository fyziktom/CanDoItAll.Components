# SB19 - Validation and Closure

## Required commands

Components:

```powershell
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj
dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj
npm run webgllib:audit-scene-runtime
```

Economy:

```powershell
dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj
pwsh ./scripts/audit-simulation-boundaries.ps1
```

## Required proof

- changed-file hashes
- command transcripts
- source assertions
- semantic invariants
- anti-stub audit
- closure report

## Final audit

Review the original goals:

- stable
- maintainable
- generic
- no example-specific leakage
- ready for simulation-to-visualization bridge
- no small/medium/mobile WebGL optimization drift
