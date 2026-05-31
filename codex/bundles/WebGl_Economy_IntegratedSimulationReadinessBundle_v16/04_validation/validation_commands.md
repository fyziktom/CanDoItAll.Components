# Validation commands

Run from `CanDoItAll.Components`:

```powershell
git branch --show-current
npm install
npm run webgllib:build-assets
npm run webgllib:verify-assets
node tools/webgllib/audit-scene-runtime.cjs
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj
dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj
dotnet build CanDoItAll.Components.slnx
```

Run from `CanDoItAll.Economy`:

```powershell
git branch --show-current
pwsh ./scripts/audit-simulation-boundaries.ps1
dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj
dotnet build CanDoItAll.Economy.slnx
```

Cross-repo proof expectations:

- No new branch created.
- No Components -> Economy references.
- No generic Economy abstraction -> Components/WebGL references.
- No WebGL small/medium/mobile/tablet optimization work.
- Shared-resource and constrained-resource readiness probes pass without hardcoding example behavior in generic code.
- Snapshot export/import/diff/analyze proof passes.
