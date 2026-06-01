# SB14 — Validation and closure

## Required commands

Run and capture transcripts:

```powershell
# Components
dotnet build CanDoItAll.Components.slnx
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj
dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj
node tools/webgllib/audit-scene-runtime.cjs

# Economy
dotnet build CanDoItAll.Economy.slnx
dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj
pwsh ./scripts/audit-simulation-boundaries.ps1
```

## Closure

- every changed production file must be justified by a subbundle;
- every critical subbundle must have manifest + semantic invariants;
- no new branch;
- no mobile/small-screen WebGL tasks;
- no Economy references in Components;
- no WebGL references in Economy abstractions except approved and documented bridge boundaries.
