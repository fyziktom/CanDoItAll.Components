# SB12 — Validation And Closure

## Required validation commands

Components:

```bash
dotnet build CanDoItAll.Components.slnx
npm run webgllib:audit-scene-runtime
npm run webgllib:build-assets
npm run webgllib:verify-assets
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj
dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj
```

Economy:

```bash
dotnet build CanDoItAll.Economy.slnx
pwsh ./scripts/audit-simulation-boundaries.ps1
dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj
```

Bundle validation:

```bash
python codex/bundles/<bundle>/scripts/validate_bundle.py --stage prepared
python codex/bundles/<bundle>/scripts/validate_bundle.py --stage completed
```

## Closure criteria

- all relevant builds/tests/audits pass or blockers are explicit
- real headless probe artifacts exist
- joined pipeline readiness report exists
- no new branch was created
- Components remains Economy-free
- WebGL remains large-screen only
