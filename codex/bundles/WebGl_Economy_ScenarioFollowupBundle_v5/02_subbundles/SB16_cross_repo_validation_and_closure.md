# SB16 - Cross-repo validation and closure

Repositories: both

## Components validation

```powershell
npm run webgllib:audit-scene-runtime
npm run webgllib:build-assets
npm run webgllib:verify-assets
dotnet build src\CanDoItAll.Components.WebGlLib\CanDoItAll.Components.WebGlLib.csproj --no-restore
dotnet build src\CanDoItAll.Components.WebGlRunLib\CanDoItAll.Components.WebGlRunLib.csproj --no-restore
dotnet test tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj --no-restore
dotnet build CanDoItAll.Components.slnx --no-restore
```

## Economy validation

```powershell
pwsh scripts\audit-simulation-boundaries.ps1
dotnet test tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --no-restore
dotnet build CanDoItAll.Economy.slnx --no-restore
```

## Closure report

Write:
- what was changed;
- what was intentionally deferred;
- whether any files exceeded line thresholds;
- whether any forbidden references were found;
- whether shared-well and entrepreneur definitions can be serialized/validated/materialized.
