# SB16 - Validation and performance proofs

Run after all changes.

Required validations:

Components:

- `npm run webgllib:build-assets`
- `npm run webgllib:verify-assets`
- `npm run webgllib:audit-scene-runtime`
- `dotnet build CanDoItAll.Components.slnx`
- `dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj`

Economy:

- `powershell scripts/audit-simulation-boundaries.ps1`
- `dotnet build CanDoItAll.Economy.slnx`
- `dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj`

Performance proof requirements:

- command batch proof with at least 50 actions;
- run playback proof without per-action JS interop from the Razor host;
- scenario definition materialization proof for shared-well and entrepreneurs;
- deterministic hash proof after JSON property order change;
- boundary proof: no `CanDoItAll.Components` references in `Simulation.*` projects and no Economy references in Components.
